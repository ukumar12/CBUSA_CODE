Imports Components
Imports DataLayer
Imports System.IO
Imports System.Linq
Imports System.Data
Imports System.Configuration.ConfigurationManager

Public Class ProcessExportQueue
    Public Shared Sub Run(ByVal DB As Database)
        Dim dbTaskLog As TaskLogRow = Nothing
        Try
            Console.WriteLine("Running ProcessExportQueue...")

            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "ProcessExportQueue"
            dbTaskLog.Status = "Started"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ""
            dbTaskLog.Insert()

            Dim SQL As String = "SELECT * FROM ExportQueue WHERE ProcessDate is null order by CreateDate desc"
            Dim dt As DataTable = DB.GetDataTable(SQL)
            For Each row As DataRow In dt.Rows
                Dim ExportQueueId As Integer = 0
                Dim IDValue As Integer = 0
                Dim IDField As String = String.Empty
                Dim ExportCode As String = String.Empty
                Dim Parameters As String = String.Empty

                If Not row("ExportQueueId") Is Convert.DBNull Then
                    ExportQueueId = Convert.ToInt32(row("ExportQueueId"))
                End If

                If Not row("AdminId") Is Convert.DBNull Then
                    IDValue = Convert.ToInt32(row("AdminId"))
                    IDField = "AdminId"
                ElseIf Not row("VendorAccountId") Is Convert.DBNull Then
                    IDValue = Convert.ToInt32(row("VendorAccountId"))
                    IDField = "VendorAccountId"
                ElseIf Not row("BuilderAccountId") Is Convert.DBNull Then
                    IDValue = Convert.ToInt32(row("BuilderAccountId"))
                    IDField = "BuilderAccountId"
                ElseIf Not row("PIQAccountId") Is Convert.DBNull Then
                    IDValue = Convert.ToInt32(row("PIQAccountId"))
                    IDField = "PIQAccountId"
                End If

                If Not row("ExportCode") Is Convert.DBNull Then
                    ExportCode = Convert.ToString(row("ExportCode"))
                End If
                If Not row("Parameters") Is Convert.DBNull Then
                    Parameters = Convert.ToString(row("Parameters"))
                End If

                Select Case ExportCode.ToLower
                    Case "product_catalog"
                        Call ExportProductCatalog(DB, IDValue, IDField, Parameters)
                    Case "member_directory"
                        Call ExportMemberDirectory(DB, IDValue, IDField, Parameters)
                End Select

                If ExportQueueId > 0 Then
                    Dim dbExport As ExportQueueRow = ExportQueueRow.GetRow(DB, ExportQueueId)
                    If Not dbExport Is Nothing Then
                        dbExport.ProcessDate = Now
                        dbExport.Update()
                    End If
                End If
            Next
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "ProcessExportQueue"
            dbTaskLog.Status = "Completed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ""
            dbTaskLog.Insert()
        Catch ex As Exception
            Logger.Error(Logger.GetErrorMessage(ex))
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "ProcessExportQueue"
            dbTaskLog.Status = "Failed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ex.Message
            dbTaskLog.Insert()
        Finally
            Console.WriteLine("ProcessExportQueue completed.")
        End Try
    End Sub

    Private Shared Sub ExportProductCatalog(ByVal DB As Database, ByVal IDValue As Integer, ByVal IDField As String, ByVal Parameters As String)
        If Parameters = String.Empty Then Exit Sub

        Dim NotificationEmail As String = String.Empty
        Dim VendorId As Integer = 0
        Dim ProductIdList As String = String.Empty

        Dim TempArray As String() = Parameters.Split("&")
        If UBound(TempArray) <> 2 Then Exit Sub

        Dim TempArray2 As String()

        TempArray2 = TempArray(0).Trim.Split("=")
        If UBound(TempArray2) = 1 Then
            NotificationEmail = TempArray2(1).Trim
        End If

        TempArray2 = TempArray(1).Trim.Split("=")
        If UBound(TempArray2) = 1 Then
            Try
                VendorId = Convert.ToInt32(TempArray2(1).Trim)
            Catch ex As Exception
                VendorId = 0
            End Try
        End If

        TempArray2 = TempArray(2).Trim.Split("=")
        If UBound(TempArray2) = 1 Then
            ProductIdList = TempArray2(1).Trim
        End If

        Dim SQL As String = String.Empty
        If VendorId > 0 Then
            SQL = "SELECT r.ProductId, r.Product As Productname, r.Sku, vt.VendorPrice, vt.VendorSku, vt.SubstitutePrice, vt.SubstituteSku, vt.SubstituteQuantityMultiplier, vt.IsSubstitution, vt.SubstituteProduct FROM Product r " _
                & "left outer join (" _
                & "	select vpp.ProductId, vpp.VendorPrice, vpp.VendorSku, vpp.IsSubstitution, vs.SubstituteProductID, vs.VendorSKU as SubstituteSku, vs.VendorPrice as SubstitutePrice, vs.QuantityMultiplier as SubstituteQuantityMultiplier, vs.Product as SubstituteProduct " _
                & "	from VendorProductPrice vpp " _
                & "	left outer join (" _
                & "		select vps.*, vppi.VendorPrice, vppi.VendorSKU, p.Product " _
                & "		from Product p " _
                & "		inner join VendorProductSubstitute vps on p.ProductID = vps.SubstituteProductID " _
                & "		inner join VendorProductPrice vppi on vps.SubstituteProductID = vppi.ProductID " _
                & "            where(vps.VendorID = " & DB.Number(VendorId) & ") " _
                & "		and vppi.VendorID=" & DB.Number(VendorId) _
                & "	) as vs on vpp.ProductID = vs.ProductID " _
                & "            where(vpp.VendorID = " & DB.Number(VendorId) & ") " _
                & ") as vt ON r.ProductId = vt.ProductId " _
                & " WHERE r.ProductId IN " & DB.NumberMultiple(ProductIdList) _
                & " order by ProductID asc"
        Else
            SQL = "SELECT r.ProductId, r.Product As Productname, r.Sku FROM Product r WHERE r.ProductId IN " & DB.NumberMultiple(ProductIdList)
        End If
        Dim ds As DataSet = DB.GetDataSet(SQL)
        Dim dv As DataView = ds.Tables(0).DefaultView

        Dim Folder As String = AppSettings("AssetFolder") & "catalogs/"
        Dim FolderPath As String = AppSettings("AssetFolderPath") & "catalogs\"
        Dim i As Integer = 0
        Dim FileName As String = Core.GenerateFileID & ".csv"

        Dim sw As StreamWriter = New StreamWriter((FolderPath & FileName), False)

        sw.WriteLine("CBUSA Catalog")
        sw.WriteLine("Report generated on " & DateTime.Now.ToString("d"))
        sw.WriteLine(String.Empty)

        If VendorId = 0 Then
            sw.WriteLine("SKU,Name")
        Else
            Dim dbVendor As VendorRow = VendorRow.GetRow(DB, VendorId)
            sw.WriteLine("Vendor:," & dbVendor.CompanyName & vbCrLf)
            sw.WriteLine("SKU,Name,Substitute Product,Substitute Product SKU,Price")
        End If

        Dim ProductIdArray As String() = ProductIdList.Split(",")
        For iLoop As Integer = LBound(ProductIdArray) To UBound(ProductIdArray)
            'Console.WriteLine("Product " & ProductIdArray(iLoop).ToString.Trim)
            dv.RowFilter = "ProductId=" & DB.Number(ProductIdArray(iLoop))
            If dv.Count > 0 Then
                For jLoop As Integer = 0 To (dv.Count - 1)
                    If VendorId = 0 Then
                        Dim SKU As String = Core.GetString(dv(jLoop)("SKU"))
                        Dim ProductName As String = Core.GetString(dv(jLoop)("ProductName"))

                        sw.WriteLine(Core.QuoteCSV(SKU) & _
                                     "," & Core.QuoteCSV(ProductName))
                    Else
                        Dim SKU As String = Core.GetString(dv(jLoop)("SKU"))
                        Dim ProductName As String = Core.GetString(dv(jLoop)("ProductName"))
                        Dim SubProduct As String = Core.GetString(dv(jLoop)("SubstituteProduct"))
                        Dim SubSku As String = Core.GetString(dv(jLoop)("SubstituteSku"))
                        Dim Price As Double = IIf(Core.GetBoolean(dv(jLoop)("IsSubstitution")), Core.GetDouble(dv(jLoop)("SubstitutePrice")), Core.GetDouble(dv(jLoop)("VendorPrice")))

                        sw.WriteLine(Core.QuoteCSV(SKU) & _
                                     "," & Core.QuoteCSV(ProductName) & "," & Core.QuoteCSV(SubProduct) & _
                                     "," & Core.QuoteCSV(SubSku) & "," & Core.QuoteCSV(Price))
                    End If
                Next
            End If
        Next

        sw.Flush()
        sw.Close()
        sw.Dispose()

        NotificationRow.Add(DB, IDValue, IDField, "The Product Catalog export you requested has completed.  <a href=""" & (Folder & FileName).Trim & """ target=""_blank"">Click here to download the file.</a>")

        If NotificationEmail = String.Empty Then Exit Sub

        Dim sMsg As String = String.Empty
        sMsg &= "The Product Catalog export you requested has completed. " & vbCrLf
        sMsg &= "Use the link below to download the file. " & vbCrLf & vbCrLf
        sMsg &= AppSettings("GlobalRefererName").ToString.Trim & (Folder & FileName).Trim & vbCrLf & vbCrLf

        ''
        ' FOR TESTING...
        sMsg = NotificationEmail & vbCrLf & vbCrLf & sMsg
        If SysParam.GetValue(DB, "AutoMessageTestMode") Then
            NotificationEmail = SysParam.GetValue(DB, "AdminEmail")
        End If
        ''

        Core.SendSimpleMail(AppSettings("ExportEmailFrom"), AppSettings("ExportEmailFromName"), NotificationEmail, NotificationEmail, AppSettings("ExportEmailSubject"), sMsg)
    End Sub

    Private Shared Sub ExportMemberDirectory(ByVal DB As Database, ByVal IDValue As Integer, ByVal IDField As String, ByVal Parameters As String)
        If Parameters = String.Empty Then Exit Sub

        Dim NotificationEmail As String = String.Empty
        Dim BuilderId As Integer = 0
        Dim VendorId As Integer = 0
        Dim LLCID As Integer = 0

        Dim F_MemberType As String = String.Empty
        Dim F_SupplyPhase As String = String.Empty
        Dim F_ContactFirstName As String = String.Empty
        Dim F_ContactLastName As String = String.Empty
        Dim F_CompanyName As String = String.Empty

        Dim TempArray As String() = Parameters.Split("&")
        For Each ParamPair As String In TempArray
            Dim TempArray2 As String() = ParamPair.Trim.Split("=")
            If UBound(TempArray2) = 1 Then
                Select Case TempArray2(0).ToLower.Trim
                    Case "NotificationEmail".ToLower
                        NotificationEmail = (TempArray2(1).Trim)
                    Case "BuilderId".ToLower
                        Try
                            BuilderId = Convert.ToInt32(TempArray2(1).Trim)
                        Catch ex As Exception
                            BuilderId = 0
                        End Try
                    Case "VendorId".ToLower
                        Try
                            VendorId = Convert.ToInt32(TempArray2(1).Trim)
                        Catch ex As Exception
                            VendorId = 0
                        End Try
                    Case "LLCID".ToLower
                        Try
                            LLCID = Convert.ToInt32(TempArray2(1).Trim)
                        Catch ex As Exception
                            LLCID = 0
                        End Try
                    Case "F_MemberType".ToLower
                        F_MemberType = (TempArray2(1).Trim)
                    Case "F_SupplyPhase".ToLower
                        F_SupplyPhase = (TempArray2(1).Trim)
                    Case "F_ContactFirstName".ToLower
                        F_ContactFirstName = (TempArray2(1).Trim)
                    Case "F_ContactLastName".ToLower
                        F_ContactLastName = (TempArray2(1).Trim)
                    Case "F_CompanyName".ToLower
                        F_CompanyName = (TempArray2(1).Trim)
                End Select
            End If
        Next

        Dim EntityTable As String = ""
        Dim EntityTableAccount As String = ""
        Dim EntityTableId As String = ""
        Dim EntityRegistration As String = ""
        Dim SQL As String = ""
        Dim WhereClause As String = ""
        Dim JoinClause As String = ""

        'select the correct entity table and 
        Select Case F_MemberType
            Case "Builder"
                EntityTable = F_MemberType
                EntityTableAccount = F_MemberType & "Account"
                EntityTableId = F_MemberType & "ID"
                EntityRegistration = F_MemberType & "Registration"
            Case "Vendor"
                EntityTable = F_MemberType
                EntityTableAccount = F_MemberType & "Account"
                EntityTableId = F_MemberType & "ID"
                EntityRegistration = F_MemberType & "Registration"
            Case "PIQ"
                EntityTable = F_MemberType
                EntityTableAccount = F_MemberType & "Account"
                EntityTableId = F_MemberType & "ID"
                EntityRegistration = F_MemberType & "Registration"
            Case Else
                Exit Sub
        End Select

        SQL = "SELECT" & vbCrLf
        SQL = SQL & " Row_Number() Over(order by CompanyName) as Rank, et." & EntityTableId & " ID, et.CompanyName, et.WebsiteURL, et.Address, et.Address2, et.City, et.State, et.Zip, et.Phone, et.Fax "

        If F_MemberType = "Vendor" Then
            SQL = SQL & "  , et.LogoFile as LogoFile" & vbCrLf
        Else
            SQL = SQL & "  , NULL as LogoFile" & vbCrLf
        End If

        SQL &= "FROM" & vbCrLf

        JoinClause = EntityTable & " et " & vbCrLf

        'join clause is changed by the supply phase
        If F_MemberType = "Vendor" AndAlso F_SupplyPhase <> "" Then
            If WhereClause <> "" Then WhereClause = WhereClause & " AND "
            WhereClause = WhereClause & " et." & EntityTableId & " in (select " & EntityTableId & " from VendorSupplyPhase where SupplyPhaseID = " & DB.Number(F_SupplyPhase) & ")" & vbCrLf
        End If

        SQL = SQL & JoinClause

        If F_ContactFirstName <> "" Then
            If WhereClause <> "" Then WhereClause = WhereClause & " AND "
            WhereClause = WhereClause & " et." & EntityTableId & " in (select " & EntityTableId & " from " & EntityTableAccount & " where FirstName LIKE " & DB.FilterQuote(F_ContactFirstName) & ")" & vbCrLf
        End If

        If F_ContactLastName <> "" Then
            If WhereClause <> "" Then WhereClause = WhereClause & " AND "
            WhereClause = WhereClause & " et." & EntityTableId & " in (select " & EntityTableId & " from " & EntityTableAccount & " where LastName LIKE " & DB.FilterQuote(F_ContactLastName) & ")" & vbCrLf
        End If

        If F_CompanyName <> "" Then
            If WhereClause <> "" Then WhereClause = WhereClause & " AND "
            WhereClause = WhereClause & " et.CompanyName LIKE " & DB.FilterQuote(F_CompanyName) & vbCrLf
        End If

        If F_MemberType = "Builder" AndAlso BuilderId > 0 Then
            If WhereClause <> "" Then WhereClause = WhereClause & " AND "
            WhereClause = WhereClause & " et.LLCID = " & DB.Number(LLCID) & vbCrLf
        ElseIf F_MemberType = "Builder" AndAlso VendorId > 0 Then
            If WhereClause <> "" Then WhereClause = WhereClause & " AND "
            WhereClause = WhereClause & " et.LLCID in (select LLCID from LLCVendor where VendorID=" & DB.Number(VendorId) & ")"
        End If

        If F_MemberType = "Vendor" AndAlso BuilderId > 0 Then
            If WhereClause <> "" Then WhereClause = WhereClause & " AND "
            WhereClause = WhereClause & " et." & EntityTableId & " in (select " & EntityTableId & " from LLCVendor where IsActive = 1 and LLCID = " & DB.Number(LLCID) & ")" & vbCrLf
        End If

        If WhereClause <> "" Then WhereClause = WhereClause & " AND "
        WhereClause = WhereClause & " et.IsActive = 1"

        'add where clause to the main SQL string
        If WhereClause <> "" Then
            SQL = SQL & " WHERE " & vbCrLf & WhereClause & vbCrLf
        End If

        Dim curPage As Integer = 1
        Dim perPage As Integer = 32000

        SQL = "select top " & perPage & " * from (" & SQL & ") as tmp where Rank > " & DB.Number(perPage * (curPage - 1))

        ' order by clause, sort by company name
        SQL = SQL & " ORDER BY Rank " & vbCrLf

        Dim dt As DataTable = DB.GetDataTable(SQL)
        Dim dv As DataView = dt.DefaultView

        Dim Folder As String = AppSettings("AssetFolder") & "directories/"
        Dim FolderPath As String = AppSettings("AssetFolderPath") & "directories\"
        Dim i As Integer = 0
        Dim FileName As String = Core.GenerateFileID & ".csv"

        Dim sw As StreamWriter = New StreamWriter((FolderPath & FileName), False)

        sw.WriteLine("CBUSA Directory")
        sw.WriteLine("Report generated on " & DateTime.Now.ToString("d"))
        sw.WriteLine(String.Empty)

        'If BuilderId > 0 Then
        '    '''
        'End If
        'If VendorId > 0 Then
        '    '''
        'End If

        If F_MemberType <> String.Empty Then
            sw.WriteLine("Member Type," & F_MemberType)
        End If
        If F_CompanyName <> String.Empty Then
            sw.WriteLine("Company Name," & F_CompanyName)
        End If
        If F_ContactFirstName <> String.Empty Then
            sw.WriteLine("Contact First Name," & F_ContactFirstName)
        End If
        If F_ContactLastName <> String.Empty Then
            sw.WriteLine("Contact Last Name," & F_ContactLastName)
        End If
        If F_SupplyPhase <> String.Empty Then
            Dim dbSupplyPhase As SupplyPhaseRow = SupplyPhaseRow.GetRow(DB, Convert.ToInt32(F_SupplyPhase))
            If Not dbSupplyPhase Is Nothing Then
                sw.WriteLine("Supply Phase," & dbSupplyPhase.SupplyPhase)
            End If
        End If
        sw.WriteLine(String.Empty)

        If F_MemberType = "Vendor" Then
            sw.WriteLine("Company Name,Website URL,Address 1,Address 2,City,State,Zip,Supply Phases,Primary Contact First Name,Primary Contact Last Name,Title,Roles,Email,Phone,Mobile,Fax")
        Else
            sw.WriteLine("Company Name,Website URL,Address 1,Address 2,City,State,Zip,Primary Contact First Name,Primary Contact Last Name,Title,Email,Phone,Mobile,Fax")
        End If
        If dv.Count > 0 Then
            For jLoop As Integer = 0 To (dv.Count - 1)
                Dim WebsiteURL As String = IIf(Core.GetString(dv(jLoop)("WebsiteURL")).ToString.StartsWith("http"), Core.GetString(dv(jLoop)("WebsiteURL")), "http://" & Core.GetString(dv(jLoop)("WebsiteURL")))
                If WebsiteURL = "http://" Then WebsiteURL = String.Empty

                Dim dtContacts As DataTable
                Dim sSQL As String = String.Empty
                Select Case F_MemberType
                    Case "Builder"
                        'sSQL = "SELECT ba.FirstName, ba.LastName, ba.Title, ba.Email, ba.Phone, ba.Mobile, ba.Fax FROM BuilderAccount AS ba INNER JOIN Builder AS bldr ON ba.BuilderID = bldr.BuilderID WHERE ba.IsPrimary = 1 AND ba.IsActive = 1 AND ba.BuilderID=" & DB.Number(Core.GetInt(dv(jLoop)("ID"))) & " ORDER BY ba.FirstName ASC, ba.LastName ASC "
                        sSQL = "SELECT Distinct ba.FirstName, ba.LastName, ba.Title, ba.Email, ba.Phone, ba.Mobile, ba.Fax FROM BuilderAccount AS ba INNER JOIN Builder AS bldr ON ba.BuilderID = bldr.BuilderID WHERE ba.IsActive = 1 AND ba.BuilderID=" & DB.Number(Core.GetInt(dv(jLoop)("ID"))) & " ORDER BY ba.FirstName ASC, ba.LastName ASC "
                    Case "Vendor"
                        'Dim VendorRoleId As Integer
                        'VendorRoleId = DB.ExecuteScalar("SELECT Top 1 VendorRoleId FROM VendorRole WHERE VendorRole = 'Primary Contact'")
                        'sSQL = "SELECT Top 1 va.VendorAccountID, va.FirstName, va.LastName, va.Title, va.Email, va.Phone, va.Mobile, va.Fax FROM VendorAccount AS va INNER JOIN Vendor AS vndr ON va.VendorID = vndr.VendorID INNER JOIN VendorAccountVendorRole AS vavr ON vavr.VendorAccountId = va.VendorAccountId WHERE va.IsActive = 1 AND va.VendorID=" & DB.Number(Core.GetInt(dv(jLoop)("ID"))) & " AND vavr.VendorRoleID = " & DB.Number(VendorRoleId) & " ORDER BY va.FirstName ASC, va.LastName ASC"
                        sSQL = "SELECT Distinct va.VendorAccountID, va.FirstName, va.LastName, va.Title, va.Email, va.Phone, va.Mobile, va.Fax FROM VendorAccount AS va INNER JOIN Vendor AS vndr ON va.VendorID = vndr.VendorID INNER JOIN VendorAccountVendorRole AS vavr ON vavr.VendorAccountId = va.VendorAccountId WHERE va.IsActive = 1 AND va.VendorID=" & DB.Number(Core.GetInt(dv(jLoop)("ID"))) & " ORDER BY va.FirstName ASC, va.LastName ASC"
                    Case "PIQ"
                        'sSQL = "SELECT Top 1 pa.FirstName, pa.LastName, NULL AS Title, piq.Email, piq.Phone, piq.Mobile, piq.Fax FROM PIQAccount AS pa INNER JOIN PIQ AS piq ON pa.PIQID = piq.PIQID WHERE pa.IsPrimary = 1 AND piq.IsActive = 1 AND pa.PIQID=" & DB.Number(Core.GetInt(dv(jLoop)("ID"))) & " ORDER BY newid() "
                        sSQL = "SELECT Distinct pa.FirstName, pa.LastName, NULL AS Title, piq.Email, piq.Phone, piq.Mobile, piq.Fax FROM PIQAccount AS pa INNER JOIN PIQ AS piq ON pa.PIQID = piq.PIQID WHERE piq.IsActive = 1 AND pa.PIQID=" & DB.Number(Core.GetInt(dv(jLoop)("ID"))) & " ORDER BY newid() "
                End Select
                dtContacts = DB.GetDataTable(sSQL)

                For Each v As DataRow In dtContacts.Rows


                    Dim FirstName As String = String.Empty
                    Dim LastName As String = String.Empty
                    Dim Title As String = String.Empty
                    Dim sVendorAccountRoles As String = String.Empty
                    Dim sVendorSupplyPhases As String = String.Empty
                    Dim Email As String = String.Empty
                    Dim Phone As String = String.Empty
                    Dim Mobile As String = String.Empty
                    Dim Fax As String = String.Empty

                    FirstName = Core.GetString(v("FirstName"))
                    LastName = Core.GetString(v("LastName"))
                    Title = Core.GetString(v("Title"))
                    If F_MemberType = "Vendor" Then
                        Dim dbVendorAccount As VendorAccountRow
                        dbVendorAccount = VendorAccountRow.GetRow(DB, v("VendorAccountID"))
                        sVendorAccountRoles = dbVendorAccount.GetSelectedVendorRoleLabels()
                        sVendorSupplyPhases = SupplyPhaseRow.GetNames(DB, SupplyPhaseRow.GetVendorSupplyPhaseString(DB, Core.GetInt(dv(jLoop)("ID")), True), ", ")
                    End If
                    Email = Core.GetString(v("Email"))
                    Phone = Core.GetString(v("Phone"))
                    Mobile = Core.GetString(v("Mobile"))
                    Fax = Core.GetString(v("Fax"))

                    If F_MemberType = "Vendor" Then
                        sw.WriteLine(Core.QuoteCSV(Core.GetString(dv(jLoop)("CompanyName"))) _
                                     & "," & Core.QuoteCSV(WebsiteURL) _
                                     & "," & Core.QuoteCSV(Core.GetString(dv(jLoop)("Address"))) _
                                     & "," & Core.QuoteCSV(Core.GetString(dv(jLoop)("Address2"))) _
                                     & "," & Core.QuoteCSV(Core.GetString(dv(jLoop)("City"))) _
                                     & "," & Core.QuoteCSV(Core.GetString(dv(jLoop)("State"))) _
                                     & "," & Core.QuoteCSV(Core.GetString(dv(jLoop)("Zip"))) _
                                     & "," & Core.QuoteCSV(sVendorSupplyPhases) _
                                     & "," & Core.QuoteCSV(FirstName) _
                                     & "," & Core.QuoteCSV(LastName) _
                                     & "," & Core.QuoteCSV(Title) _
                                     & "," & Core.QuoteCSV(sVendorAccountRoles) _
                                     & "," & Core.QuoteCSV(Email) _
                                     & "," & Core.QuoteCSV(Phone) _
                                     & "," & Core.QuoteCSV(Mobile) _
                                     & "," & Core.QuoteCSV(Fax))
                    Else
                        sw.WriteLine(Core.QuoteCSV(Core.GetString(dv(jLoop)("CompanyName"))) _
                                     & "," & Core.QuoteCSV(WebsiteURL) _
                                     & "," & Core.QuoteCSV(Core.GetString(dv(jLoop)("Address"))) _
                                     & "," & Core.QuoteCSV(Core.GetString(dv(jLoop)("Address2"))) _
                                     & "," & Core.QuoteCSV(Core.GetString(dv(jLoop)("City"))) _
                                     & "," & Core.QuoteCSV(Core.GetString(dv(jLoop)("State"))) _
                                     & "," & Core.QuoteCSV(Core.GetString(dv(jLoop)("Zip"))) _
                                     & "," & Core.QuoteCSV(FirstName) _
                                     & "," & Core.QuoteCSV(LastName) _
                                     & "," & Core.QuoteCSV(Title) _
                                     & "," & Core.QuoteCSV(Email) _
                                     & "," & Core.QuoteCSV(Phone) _
                                     & "," & Core.QuoteCSV(Mobile) _
                                     & "," & Core.QuoteCSV(Fax))
                    End If
                Next
            Next
        Else
                sw.WriteLine("There are no results that match your search criteria.")
        End If

        sw.Flush()
        sw.Close()
        sw.Dispose()

        NotificationRow.Add(DB, IDValue, IDField, "The Member Directory export you requested has completed.  <a href=""" & Folder & FileName & """ target=""_blank"">Click here to download the file.</a>")

        If NotificationEmail = String.Empty Then Exit Sub

        Dim sMsg As String = String.Empty
        sMsg &= "The Member Directory export you requested has completed. " & vbCrLf
        sMsg &= "Use the link below to download the file. " & vbCrLf & vbCrLf
        sMsg &= AppSettings("GlobalRefererName").ToString.Trim & (Folder & FileName).Trim & vbCrLf & vbCrLf

        ''
        ' FOR TESTING...
        sMsg = NotificationEmail & vbCrLf & vbCrLf & sMsg
        If SysParam.GetValue(DB, "AutoMessageTestMode") Then
            NotificationEmail = SysParam.GetValue(DB, "AdminEmail")
        End If
        ''

        Core.SendSimpleMail(AppSettings("ExportEmailFrom"), AppSettings("ExportEmailFromName"), NotificationEmail, NotificationEmail, AppSettings("ExportEmailSubject"), sMsg)
    End Sub
End Class