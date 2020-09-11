Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components
Imports System.IO
Imports System.Configuration.ConfigurationManager

Partial Class admin_products_productimport
    Inherits AdminPage

    Dim uomcollection As UnitOfMeasureCollection
    Dim supphase As DataTable
    Dim prSKU As DataTable
    Dim RootSupplyPhaseId As String
    Dim LLC As DataTable
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("PRODUCTS")
    End Sub

    Protected Sub btnImport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnImport.Click
        Dim FileInfo As System.IO.FileInfo
        Dim OriginalExtension As String
        Dim NewFileName As String

        'prepare for import
        uomcollection = UnitOfMeasureRow.GetUnitOfMesaureList(DB)
        prSKU = ProductRow.GetList(DB)
        supphase = SupplyPhaseRow.GetList(DB)
        LLC = LLCRow.GetList(DB)
        RootSupplyPhaseId = supphase.Select("SupplyPhase='root'")(0)("SupplyPhaseId")

        If fulDocument.NewFileName <> String.Empty Then
            OriginalExtension = System.IO.Path.GetExtension(fulDocument.MyFile.FileName)
            If OriginalExtension <> ".csv" And OriginalExtension <> ".xls" Then
                AddError("Please enter a valid .csv or .xls file. Your file extension was: " & OriginalExtension)
                Exit Sub
            End If

            fulDocument.Folder = "/assets/productimport/file/"
            fulDocument.SaveNewFile()

            FileInfo = New System.IO.FileInfo(Server.MapPath(fulDocument.Folder & fulDocument.NewFileName))

            NewFileName = Core.GenerateFileID
            FileInfo.CopyTo(Server.MapPath(fulDocument.Folder & NewFileName & OriginalExtension))

            FileInfo.Delete()
        Else
            AddError("Please enter the file you want to upload")
            Exit Sub
        End If

       
        Try
            ImportProduct(NewFileName & OriginalExtension)
        Catch ex As Exception
            AddError(Err.Description)
        End Try
    End Sub
    Private Function GetSupplyPhaseId(ByVal Phase As String) As String
        Dim Phaselevels() As String = Phase.Split("|")
        Dim ParentId As String = RootSupplyPhaseId
        For Each Phaselevel As String In Phaselevels
            Dim phaseids() As DataRow = supphase.Select("SupplyPhase=" & DB.Quote(Phaselevel) & " and ParentSupplyPhaseId=" & DB.Quote(ParentId))
            If phaseids.Length > 0 Then
                ParentId = phaseids(0)("SupplyPhaseid")
            End If
        Next
        'The last iteration will set the ParentId to the SupplyPhaseid of a leaf level element
        If ParentId = RootSupplyPhaseId AndAlso Not Phase.Contains("root") Then
            Return String.Empty
        Else
            Return ParentId
        End If
    End Function
    Private Function GetLLCIdString(ByVal LLCString As String) As String
        Dim LLCS() As String = LLCString.Split("|")
        Dim LLCIds As String = String.Empty
        Dim comma As String = String.Empty
        For Each LLCName As String In LLCS
            Dim matchingllcs() As DataRow = LLC.Select("LLC=" & DB.Quote(LLCName))
            If matchingllcs.Length > 0 Then
                LLCIds &= comma & matchingllcs(0)("LLCId")
                comma = ","
            End If
        Next
        Return LLCIds
    End Function
    Private Sub ImportProduct(ByVal filename As String)
        Dim aLine As String()
        Dim Count As Integer = 0
        Dim BadCount As Integer = 0
        Dim InsertCount As Integer = 0
        Dim UpdateCount As Integer = 0
        Dim txtErr As String = String.Empty
        Dim tblErr As String = String.Empty
        Dim ProductId As Integer = 0
        Dim ProductName As String = String.Empty
        Dim SKU As String = String.Empty
        Dim Description As String = String.Empty
        Dim SupplyPhase1 As String = String.Empty
        Dim SupplyPhase2 As String = String.Empty
        Dim SupplyPhase3 As String = String.Empty
        Dim SupplyPhase4 As String = String.Empty
        Dim SupplyPhase5 As String = String.Empty
        Dim LLCPricing As String = String.Empty
        Dim UpdateRow As Boolean = False
        Dim errorMsg As String = String.Empty
        Dim IsActive As String = String.Empty

        Dim Folder As String = AppSettings("ErrorFolderName")
        Dim i As Integer = 0
        Dim ErrorFileName As String = Core.GetFileNameWithoutExtension(filename) & "Error" & Date.Now.ToFileTime & ".txt"
        Dim sw As StreamWriter = New StreamWriter(Folder & ErrorFileName, False)

        sw.WriteLine("Error Report for Import file")
        sw.WriteLine("Report generated on " & DateTime.Now.ToString("d"))
        sw.WriteLine("The following records corresponding to following lines on the file could not be correctly imported.")
        sw.WriteLine(String.Empty)
        sw.WriteLine(String.Empty)
        ltlErrorFileLink.Text = String.Empty
        ltlMessage.Text = String.Empty


        If Not File.Exists(Server.MapPath(fulDocument.Folder & filename)) Then
            AddError("Cannot find the file to process")
            Exit Sub
        End If


        Dim f As StreamReader = New StreamReader(Server.MapPath(fulDocument.Folder & filename))
        While Not f.EndOfStream
            Dim sOriginalLine As String = String.Empty
            Try
                Count = Count + 1

                Dim sLine As String = f.ReadLine()
                sOriginalLine = sLine

                Dim bInside As Boolean = False
                For iLoop As Integer = 1 To Len(sLine)
                    If Mid(sLine, iLoop, 1) = """" Then
                        If bInside = False Then
                            bInside = True
                        Else
                            bInside = False
                        End If
                    End If
                    If Mid(sLine, iLoop, 1) = "," Then
                        If Not bInside Then
                            sLine = Left(sLine, iLoop - 1) & "~" & Mid(sLine, iLoop + 1, Len(sLine) - iLoop)
                        End If
                    End If
                Next

                aLine = sLine.Split("~")

                If aLine.Length >= 1 AndAlso aLine(1) <> String.Empty Then
                    ProductId = 0

                    SKU = Trim(Core.StripDblQuote(aLine(0))).Replace("""""", """")

                    ProductName = Trim(Core.StripDblQuote(aLine(1))).Replace("""""", """")

                    SupplyPhase1 = Trim(Core.StripDblQuote(aLine(2))).Replace("""""", """")
                    SupplyPhase2 = Trim(Core.StripDblQuote(aLine(3))).Replace("""""", """")
                    SupplyPhase3 = Trim(Core.StripDblQuote(aLine(4))).Replace("""""", """")
                    SupplyPhase4 = Trim(Core.StripDblQuote(aLine(5))).Replace("""""", """")
                    SupplyPhase5 = Trim(Core.StripDblQuote(aLine(6))).Replace("""""", """")

                    'Assuming all LLCs to be included are spaced with Pipes
                    Try
                        LLCPricing = Trim(Core.StripDblQuote(aLine(7))).Replace("""""", """")
                        Description = Trim(Core.StripDblQuote(aLine(8))).Replace("""""", """")
                        IsActive = Trim(Core.StripDblQuote(aLine(9))).Replace("""""", """")
                    Catch ex As IndexOutOfRangeException
                    End Try
                    If Count = 1 And ProductName.ToUpper <> "NAME" Then
                        AddError("The file you uploaded does not appear to be in the correct format.")
                        Exit Sub
                    End If

                    If ProductName.ToUpper = "NAME" Then
                        Continue While
                    End If

                    txtErr = String.Empty

                    DB.BeginTransaction()

                    If txtErr = String.Empty Then

                        ProductId = 0

                        Dim productrows As DataRow() = prSKU.Select("SKU=" & DB.Quote(SKU))
                        If productrows.Length > 0 Then
                            ProductId = productrows(0)("ProductId")
                        End If

                        Dim dbproduct As ProductRow
                        If ProductId = 0 Then
                            UpdateRow = False
                            dbproduct = New ProductRow(DB)
                            'SKU must be generated now, so cannot update products, only insert
                            SKU = ProductRow.GetNextSKU(DB)
                        Else
                            dbproduct = ProductRow.GetRow(DB, ProductId)
                            UpdateRow = True
                        End If

                        dbproduct.Description = Description
                        dbproduct.SKU = SKU
                        'dbproduct.IsActive = True

                        If IsActive.ToLower.Trim = "yes" Then
                            dbproduct.IsActive = True
                        Else
                            dbproduct.IsActive = False

                        End If
                        dbproduct.Product = ProductName
                        dbproduct.ProductTypeID = 1

                        If UpdateRow = False Then
                            dbproduct.CreatorAdminID = CType(Page, AdminPage).LoggedInAdminId
                            ProductId = dbproduct.Insert()
                            InsertCount += 1
                        Else
                            dbproduct.UpdaterAdminID = CType(Page, AdminPage).LoggedInAdminId
                            dbproduct.Update()
                            UpdateCount += 1
                        End If

                        If Not LLCPricing = String.Empty Then
                            Dim LLCids As String = GetLLCIdString(LLCPricing)
                            dbproduct.DeleteFromAllLLCProductPriceRequirements()
                            dbproduct.InsertToLLCProductPriceRequirements(LLCids)
                        End If

                        Dim SupplyPhaseIds As String = String.Empty
                        Dim comma As String = String.Empty
                        Dim supplyphaseid As String = String.Empty

                        If Not SupplyPhase1 = String.Empty Then
                            supplyphaseid = GetSupplyPhaseId(SupplyPhase1)
                            If Not supplyphaseid = String.Empty Then
                                SupplyPhaseIds &= comma & supplyphaseid
                                comma = ","
                            End If
                        End If

                        If Not SupplyPhase2 = String.Empty Then
                            supplyphaseid = GetSupplyPhaseId(SupplyPhase2)
                            If Not supplyphaseid = String.Empty Then
                                SupplyPhaseIds &= comma & supplyphaseid
                                comma = ","
                            End If
                        End If

                        If Not SupplyPhase3 = String.Empty Then
                            supplyphaseid = GetSupplyPhaseId(SupplyPhase3)
                            If Not supplyphaseid = String.Empty Then
                                SupplyPhaseIds &= comma & supplyphaseid
                                comma = ","
                            End If
                        End If

                        If Not SupplyPhase4 = String.Empty Then
                            supplyphaseid = GetSupplyPhaseId(SupplyPhase4)
                            If Not supplyphaseid = String.Empty Then
                                SupplyPhaseIds &= comma & supplyphaseid
                                comma = ","
                            End If
                        End If

                        If Not SupplyPhase5 = String.Empty Then
                            supplyphaseid = GetSupplyPhaseId(SupplyPhase5)
                            If Not supplyphaseid = String.Empty Then
                                SupplyPhaseIds &= comma & supplyphaseid
                                comma = ","
                            End If
                        End If

                        If Not SupplyPhaseIds = String.Empty Then
                            dbproduct.DeleteFromAllSupplyPhases()
                            dbproduct.InsertToSupplyPhases(SupplyPhaseIds)
                        End If

                    End If
                    DB.CommitTransaction()
                Else
                    BadCount = BadCount + 1
                    sw.WriteLine(sOriginalLine & "- Bad row ")
                    Continue While
                End If
            Catch ex As Exception
                If Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
                BadCount = BadCount + 1
                sw.WriteLine(sOriginalLine & "- " & Err.Description)
            End Try
        End While
        sw.Flush()
        sw.Close()
        ltlMessage.Text &= InsertCount & " products inserted. " & UpdateCount & " products updated. " & BadCount & " records could not be inserted/updated. "
        If BadCount > 0 Then ltlErrorFileLink.Text = "<a href=""" & Folder & ErrorFileName & """ > Click Here to view detailed error report. </a>"
    End Sub
    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub


End Class
