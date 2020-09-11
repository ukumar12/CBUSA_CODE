Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
Imports System.Collections.Generic

Namespace DataLayer

    Public Class VendorRow
        Inherits VendorRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, VendorID As Integer)
            MyBase.New(DB, VendorID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal VendorID As Integer) As VendorRow
            Dim row As VendorRow

            row = New VendorRow(DB, VendorID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal VendorID As Integer)
            Dim row As VendorRow

            row = New VendorRow(DB, VendorID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from Vendor"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        '=================== ADDED BY APALA ON 18th JUNE 2020 ===========================
        Public Shared Function GetActiveOnlyList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "SELECT * FROM VENDOR WHERE IsActive = 1 "
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " ORDER BY " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Function GetCollection(ByVal DB As Database, ByVal FilteredByVendorIds As String)
            Dim sqlFull As String = "SELECT * FROM Vendor"
            If FilteredByVendorIds Then
                sqlFull &= " WHERE VendorId in " & DB.NumberMultiple(FilteredByVendorIds)
            End If
            Dim colVendors As New List(Of VendorRow)
            Using sqldr As SqlDataReader = DB.GetReader(sqlFull)
                While sqldr.Read
                    Dim dbVendor As New VendorRow(DB)
                    dbVendor.Load(sqldr)
                    colVendors.Add(dbVendor)
                End While
            End Using
            Return colVendors
        End Function
        Public Shared Function GetVendorPriceRequests(ByVal DB As Database, ByVal VendorId As Integer) As DataTable
            Dim sql As String =
                  " select t.*, p.*, b.CompanyName " _
                & " from TakeoffProduct t inner join SpecialOrderProduct s on t.SpecialOrderProductID=s.SpecialOrderProductID " _
                & "     inner join Builder b on s.BuilderId=b.BuilderId" _
                & " where not exists (select * from VendorSpecialOrderProductPrice where VendorId=" & DB.Number(VendorId) & " and SpecialOrderProductID=t.SpecialOrderProductID)"
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetAllBuilders(ByVal DB As Database, ByVal VendorId As Integer) As DataTable
            Dim sql As String = "select * from Builder where IsActive=1 and LLCID in (select LLCID from LLCVendor where VendorId=" & DB.Number(VendorId) & ")"
            Return DB.GetDataTable(sql)
        End Function
        Public Shared Function GetAllBuildersForSalesReport(ByVal DB As Database, ByVal VendorId As Integer) As DataTable
            Dim CurrentQuarter As Integer = Math.Ceiling(DatePart(DateInterval.Month, Now) / 3)
            Dim LastQuarter As Integer = IIf(CurrentQuarter - 1 = 0, 4, CurrentQuarter - 1)
            Dim LastYear As Integer = IIf(LastQuarter = 4, DatePart(DateInterval.Year, Now) - 1, DatePart(DateInterval.Year, Now))
            Dim LastQuarterEnd As DateTime = (LastQuarter * 3) & "/" & Date.DaysInMonth(LastYear, LastQuarter * 3) & "/" & LastYear
            Dim sql As String = "select * from Builder where QuarterlyReportingOn=1 and IsActive=1 and LLCID in (select LLCID from LLCVendor where VendorId=" & DB.Number(VendorId) & ") AND Submitted <" & DB.Quote(LastQuarterEnd)
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetFilteredBuildersReader(ByVal DB As Database, ByVal VendorID As Integer, ByVal Filter As String) As SqlDataReader
            Dim sql As String = "select * from Builder where IsActive=1 and LLCID in (select LLCID from LLCVendor where VendorId=" & DB.Number(VendorID) & ")"
            If Filter <> String.Empty Then
                sql &= " and CompanyName like " & DB.Quote(Filter & "%")
            End If
            Return DB.GetReader(sql)
        End Function

        Public Shared Function GetVendorByGuid(ByVal DB As Database, ByVal Guid As String) As VendorRow
            Dim out As New VendorRow(DB)
            Dim sql As String = "select * from Vendor where Guid=" & DB.Quote(Guid)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function

        Public Shared Function GetListByLLC(ByVal DB As Database, ByVal LLCID As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim sql As String = "select v.* from Vendor v inner join LLCVendor l on v.VendorId=l.VendorId where v.IsActive=1 and l.LLCID=" & DB.Number(LLCID)
            If SortBy <> String.Empty Then
                sql &= " order by " & Core.ProtectParam(SortBy & " " & SortOrder)
            End If
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetLLCList(ByVal DB As Database, ByVal VendorID As String) As DataTable
            Dim SQL As String = "select * from LLCVendor  LV INNER JOIN LLC ON LLC.LLCID=LV.LLCID WHERE LV.VendorID =" & VendorID
            Return DB.GetDataTable(SQL)
        End Function

        Public Shared Function GetTakeoffVendors(ByVal DB As Database, ByVal TakeoffID As Integer) As DataTable
            Dim sql As String = "select BuilderID from Takeoff where TakeoffID=" & DB.Number(TakeoffID)
            Dim BuilderID As Integer = DB.ExecuteScalar(sql)
            sql =
                  " select v.*, tmp.PriceCount, v.CompanyName + ' (' + cast(tmp.PriceCount as varchar) + '/' + cast(temp.TotalCount as varchar) + ')' as Label" _
                & " from Vendor v inner join LLCVendor l on v.VendorId=l.VendorId" _
                & "     inner join Builder b on l.LLCID=b.LLCID" _
                & "     inner join (" _
                & "         select VendorID, count(*) as PriceCount " _
                & "         from TakeoffProduct tp inner join VendorProductPrice vpp on tp.ProductID=vpp.ProductID " _
                & "         where tp.TakeoffID=" & DB.Number(TakeoffID) & " and (VendorPrice is not null or vpp.IsSubstitution=1)" _
                & "         group by VendorID) as tmp on v.VendorID=tmp.VendorID," _
                & "     (select count(*) as TotalCount from TakeoffProduct where TakeoffID=" & DB.Number(TakeoffID) & ") as temp" _
                & " where v.IsActive = 1 and b.BuilderID=" & DB.Number(BuilderID) _
                & "  AND (SELECT Count(ProductID)   FROM VendorProductPrice WHERE VendorID = v.VendorId AND ProductID in (Select ProductID from Product where IsActive =1) AND ( " _
                & " VendorPrice IS NOT NULL OR ProductID IN ( SELECT ProductID FROM VendorProductSubstitute WHERE VendorID = v.VendorId  ) ) ) > 0 " _
              & " order by tmp.PriceCount desc"

            'Dim sql As String = _
            '      " select v.*, tmp.PriceCount, v.CompanyName + ' (' + cast(tmp.PriceCount as varchar) + '/' + cast(temp.TotalCount as varchar) + ')' as Label" _
            '    & " from Vendor v inner join LLCVendor l on v.VendorId=l.VendorId" _
            '    & "     inner join Builder b on l.LLCID=b.LLCID" _
            '    & "     inner join Takeoff t on b.BuilderID=t.BuilderID" _
            '    & "     inner join (" _
            '    & "         select VendorID, count(*) as PriceCount " _
            '    & "         from TakeoffProduct tp inner join VendorProductPrice vpp on tp.ProductID=vpp.ProductID " _
            '    & "         where tp.TakeoffID=" & DB.Number(TakeoffID) _
            '    & "         group by VendorID) as tmp on v.VendorID=tmp.VendorID," _
            '    & "     (select count(*) as TotalCount from TakeoffProduct where TakeoffID=" & DB.Number(TakeoffID) & ") as temp" _
            '    & " where t.TakeoffID=" & DB.Number(TakeoffID) _
            '    & " and exists (select * from VendorRegistration where VendorID=v.VendorID and datepart(yy,Approved) = " & DB.Number(Now.Year) & ")" _
            '    & " order by tmp.PriceCount desc"


            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetLLCList(ByVal DB As Database, ByVal VendorID As Integer) As String
            Dim out As New StringBuilder()
            Dim conn As String = ""
            Dim sql As String = "select * from LLCVendor where VendorID=" & DB.Number(VendorID)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            While sdr.Read
                out.Append(conn & Core.GetString(sdr("LLCID")))
                conn = ","
            End While
            sdr.Close()
            Return out.ToString
        End Function

        Public ReadOnly Property LLCID() As Integer
            Get
                Return DB.ExecuteScalar("select top 1 LLCID from LLCVendor where VendorID=" & DB.Number(VendorID))
            End Get
        End Property

        Public ReadOnly Property GetSelectedLLCs() As String
            Get
                Dim dr As SqlDataReader = DB.GetReader("select LLCID from LLCVendor where VendorID = " & VendorID)
                Dim Conn As String = String.Empty
                Dim Result As String = String.Empty

                While dr.Read()
                    Result &= Conn & dr("LLCID")
                    Conn = ","
                End While
                dr.Close()
                Return Result
            End Get
        End Property



        Public ReadOnly Property GetSelectedLLCNames() As String
            Get
                Dim dr As SqlDataReader = DB.GetReader("select l.LLC as llcname from LLCVendor lv LEft Join LLC l on  lv.LLCID = l.LLCID where lv.VendorID = " & VendorID)
                Dim Conn As String = String.Empty
                Dim Result As String = String.Empty

                While dr.Read()
                    Result &= Conn & dr("llcname")
                    Conn = ","
                End While
                dr.Close()
                Return Result
            End Get
        End Property
        Public Sub DeleteFromAllLLCs()
            DB.ExecuteSQL("delete from LLCVendor where VendorID = " & VendorID)
        End Sub

        Public Sub InsertToLLCs(ByVal Elements As String)
            If Elements = String.Empty Then Exit Sub

            Dim aElements As String() = Elements.Split(",")
            For Each Element As String In aElements
                InsertToLLC(Element)
            Next
        End Sub

        Public Sub InsertToLLCsByNames(ByVal Elements As String)
            If Elements = String.Empty Then Exit Sub

            Dim aElements As String() = Elements.Split(",")
            For Each Element As String In aElements
                InsertToLLCByName(Element.Trim())
            Next
        End Sub

        Public Sub InsertToLLCByName(ByVal LLCIDName As String)
            Dim SQL1 As String = "select top 1 LLCID from LLC where LLC = '" & LLCIDName & "'"
            Dim LLCID As Integer = DB.ExecuteScalar(SQL1)
            If LLCID > 0 Then
                Dim SQL As String = "insert into LLCVendor (VendorID, LLCID) values (" & VendorID & "," & LLCID & ")"
                DB.ExecuteSQL(SQL)
            End If
        End Sub

        Public Sub InsertToLLC(ByVal LLCID As Integer)
            Dim SQL As String = "insert into LLCVendor (VendorID, LLCID) values (" & VendorID & "," & LLCID & ")"
            DB.ExecuteSQL(SQL)
        End Sub

        Public Shared Function GetNextHistoricID(ByVal DB As Database)
            Return DB.InsertSQL("insert into HistoricIDSequence(CreateDate) values (" & DB.NullQuote(Now) & ")")
        End Function
        Public Shared Function GetRowByCRMID(ByVal DB As Database, ByVal CRMID As String) As VendorRow
            Dim out As New VendorRow(DB)
            Dim sql As String = "select * from Vendor where CRMID=" & DB.Quote(CRMID)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function


        Public Shared Function GetRowByLikeCRMID(ByVal DB As Database, ByVal CRMID As String) As VendorRow
            Dim out As New VendorRow(DB)
            Dim sql As String = "select * from Vendor where CRMID like " & DB.FilterQuote(CRMID)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function

        Public Shared Function GetVendorByName(ByVal DB As Database, ByVal Name As String) As VendorRow
            Dim out As New VendorRow(DB)
            Dim sql As String = "select top 1 * from Vendor where (CRMID is NULL or CRMID='') and CompanyName=" & DB.Quote(Name)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function
        Public Shared Function GetRowByHistoricId(ByVal DB As Database, ByVal HistoricId As String) As VendorRow
            Dim out As New VendorRow(DB)
            Dim sql As String = "select * from Vendor where HistoricID=" & DB.Quote(HistoricId)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function
        Public ReadOnly Property GetSelectedVendorCategories() As String
            Get
                Dim dr As SqlDataReader = DB.GetReader("select VendorCategoryID from VendorCategoryVendor where VendorID = " & VendorID)
                Dim Conn As String = String.Empty
                Dim Result As String = String.Empty

                While dr.Read()
                    Result &= Conn & dr("VendorCategoryID")
                    Conn = ","
                End While
                dr.Close()
                Return Result
            End Get
        End Property

        Public Sub DeleteFromAllVendorCategories()
            DB.ExecuteSQL("delete from VendorCategoryVendor where VendorID = " & VendorID)
        End Sub

        Public Sub InsertToVendorCategories(ByVal Elements As String)
            If Elements = String.Empty Then Exit Sub

            Dim aElements As String() = Elements.Split(",")
            For Each Element As String In aElements
                InsertToVendorCategory(Element)
            Next
        End Sub

        Public Sub InsertToVendorCategory(ByVal VendorCategoryID As Integer)
            Dim SQL As String = "insert into VendorCategoryVendor (VendorID, VendorCategoryID) values (" & VendorID & "," & VendorCategoryID & ")"
            DB.ExecuteSQL(SQL)
        End Sub

    End Class

    Public MustInherit Class VendorRowBase
        Private m_DB As Database
        Private m_VendorID As Integer = Nothing
        Private m_HistoricID As Integer = Nothing
        Private m_HistoricParentID As Integer = Nothing
        Private m_HistoricVendorID As Integer = Nothing
        Private m_PrimaryVendorCategoryID As Integer = Nothing
        Private m_SecondaryVendorCategoryID As Integer = Nothing
        Private m_CRMID As String = Nothing
        Private m_CompanyName As String = Nothing
        Private m_Address As String = Nothing
        Private m_Address2 As String = Nothing
        Private m_City As String = Nothing
        Private m_State As String = Nothing
        Private m_Zip As String = Nothing
        Private m_Phone As String = Nothing
        Private m_Mobile As String = Nothing
        Private m_Pager As String = Nothing
        Private m_OtherPhone As String = Nothing
        Private m_Fax As String = Nothing
        Private m_Email As String = Nothing
        Private m_WebsiteURL As String = Nothing
        Private m_LogoFile As String = Nothing
        Private m_LogoGUID As String = Nothing
        Private m_AboutUs As String = Nothing
        Private m_Submitted As DateTime = Nothing
        Private m_Updated As DateTime = Nothing
        Private m_IsActive As Boolean = Nothing
        Private m_IsPlansOnline As Boolean = Nothing
        Private m_EnableMarketShare As Boolean = Nothing
        Private m_Comments As String = Nothing
        Private m_GUID As String = Nothing
        Private m_ServicesOffered As String = Nothing
        Private m_Discounts As String = Nothing
        Private m_RebateProgram As String = Nothing
        Private m_PaymentTerms As String = Nothing
        Private m_SpecialtyServices As String = Nothing
        Private m_AcceptedCards As String = Nothing
        Private m_BillingAddress As String = Nothing
        Private m_BillingCity As String = Nothing
        Private m_BillingState As String = Nothing
        Private m_BillingZip As String = Nothing
        Private m_BrandsSupplied As String = Nothing
        Private m_ExcludedVendor As Boolean = Nothing
        Private m_HasDocumentsAccess As Boolean = Nothing
        Private m_QuarterlyReportingOn As Boolean = Nothing
        Private m_DashboardCategoryID As Integer = Nothing

        Public Property VendorID() As Integer
            Get
                Return m_VendorID
            End Get
            Set(ByVal Value As Integer)
                m_VendorID = Value
            End Set
        End Property

        Public Property HistoricID() As Integer
            Get
                Return m_HistoricID
            End Get
            Set(ByVal Value As Integer)
                m_HistoricID = Value
            End Set
        End Property

        Public Property HistoricParentID() As Integer
            Get
                Return m_HistoricParentID
            End Get
            Set(ByVal Value As Integer)
                m_HistoricParentID = Value
            End Set
        End Property

        Public Property HistoricVendorID() As Integer
            Get
                Return m_HistoricVendorID
            End Get
            Set(ByVal Value As Integer)
                m_HistoricVendorID = Value
            End Set
        End Property

        Public Property PrimaryVendorCategoryID() As Integer
            Get
                Return m_PrimaryVendorCategoryID
            End Get
            Set(ByVal Value As Integer)
                m_PrimaryVendorCategoryID = Value
            End Set
        End Property

        Public Property SecondaryVendorCategoryID() As Integer
            Get
                Return m_SecondaryVendorCategoryID
            End Get
            Set(ByVal Value As Integer)
                m_SecondaryVendorCategoryID = Value
            End Set
        End Property

        Public Property CRMID() As String
            Get
                Return m_CRMID
            End Get
            Set(ByVal Value As String)
                m_CRMID = Value
            End Set
        End Property

        Public Property CompanyName() As String
            Get
                Return m_CompanyName
            End Get
            Set(ByVal Value As String)
                m_CompanyName = Value
            End Set
        End Property

        Public Property Address() As String
            Get
                Return m_Address
            End Get
            Set(ByVal Value As String)
                m_Address = Value
            End Set
        End Property

        Public Property Address2() As String
            Get
                Return m_Address2
            End Get
            Set(ByVal Value As String)
                m_Address2 = Value
            End Set
        End Property

        Public Property City() As String
            Get
                Return m_City
            End Get
            Set(ByVal Value As String)
                m_City = Value
            End Set
        End Property

        Public Property State() As String
            Get
                Return m_State
            End Get
            Set(ByVal Value As String)
                m_State = Value
            End Set
        End Property

        Public Property Zip() As String
            Get
                Return m_Zip
            End Get
            Set(ByVal Value As String)
                m_Zip = Value
            End Set
        End Property

        Public Property Phone() As String
            Get
                Return m_Phone
            End Get
            Set(ByVal Value As String)
                m_Phone = Value
            End Set
        End Property

        Public Property Mobile() As String
            Get
                Return m_Mobile
            End Get
            Set(ByVal Value As String)
                m_Mobile = Value
            End Set
        End Property

        Public Property Pager() As String
            Get
                Return m_Pager
            End Get
            Set(ByVal Value As String)
                m_Pager = Value
            End Set
        End Property

        Public Property OtherPhone() As String
            Get
                Return m_OtherPhone
            End Get
            Set(ByVal Value As String)
                m_OtherPhone = Value
            End Set
        End Property

        Public Property Fax() As String
            Get
                Return m_Fax
            End Get
            Set(ByVal Value As String)
                m_Fax = Value
            End Set
        End Property

        Public Property Email() As String
            Get
                Return m_Email
            End Get
            Set(ByVal Value As String)
                m_Email = Value
            End Set
        End Property

        Public Property WebsiteURL() As String
            Get
                Return m_WebsiteURL
            End Get
            Set(ByVal Value As String)
                m_WebsiteURL = Value
            End Set
        End Property

        Public Property LogoFile() As String
            Get
                Return m_LogoFile
            End Get
            Set(ByVal Value As String)
                m_LogoFile = Value
            End Set
        End Property

        Public Property LogoGUID() As String
            Get
                Return m_LogoGUID
            End Get
            Set(ByVal Value As String)
                m_LogoGUID = Value
            End Set
        End Property

        Public Property AboutUs() As String
            Get
                Return m_AboutUs
            End Get
            Set(ByVal Value As String)
                m_AboutUs = Value
            End Set
        End Property

        Public ReadOnly Property Submitted() As DateTime
            Get
                Return m_Submitted
            End Get
        End Property

        Public ReadOnly Property Updated() As DateTime
            Get
                Return m_Updated
            End Get
        End Property

        Public Property IsActive() As Boolean
            Get
                Return m_IsActive
            End Get
            Set(ByVal Value As Boolean)
                m_IsActive = Value
            End Set
        End Property

        Public Property IsPlansOnline() As Boolean
            Get
                Return m_IsPlansOnline
            End Get
            Set(ByVal Value As Boolean)
                m_IsPlansOnline = Value
            End Set
        End Property

        Public Property EnableMarketShare() As Boolean
            Get
                Return m_EnableMarketShare
            End Get
            Set(ByVal Value As Boolean)
                m_EnableMarketShare = Value
            End Set
        End Property

        Public Property ExcludedVendor() As Boolean
            Get
                Return m_ExcludedVendor
            End Get
            Set(ByVal Value As Boolean)
                m_ExcludedVendor = Value
            End Set
        End Property

        Public Property HasDocumentsAccess() As Boolean
            Get
                Return m_HasDocumentsAccess
            End Get
            Set(ByVal Value As Boolean)
                m_HasDocumentsAccess = Value
            End Set
        End Property

        Public Property Comments() As String
            Get
                Return m_Comments
            End Get
            Set(ByVal Value As String)
                m_Comments = Value
            End Set
        End Property

        Public Property GUID() As String
            Get
                Return m_GUID
            End Get
            Set(ByVal Value As String)
                m_GUID = Value
            End Set
        End Property

        Public Property ServicesOffered() As String
            Get
                Return m_ServicesOffered
            End Get
            Set(ByVal value As String)
                m_ServicesOffered = value
            End Set
        End Property

        Public Property Discounts() As String
            Get
                Return m_Discounts
            End Get
            Set(ByVal value As String)
                m_Discounts = value
            End Set
        End Property

        Public Property RebateProgram() As String
            Get
                Return m_RebateProgram
            End Get
            Set(ByVal value As String)
                m_RebateProgram = value
            End Set
        End Property

        Public Property PaymentTerms() As String
            Get
                Return m_PaymentTerms
            End Get
            Set(ByVal value As String)
                m_PaymentTerms = value
            End Set
        End Property

        Public Property SpecialtyServices() As String
            Get
                Return m_SpecialtyServices
            End Get
            Set(ByVal value As String)
                m_SpecialtyServices = value
            End Set
        End Property

        Public Property AcceptedCards() As String
            Get
                Return m_AcceptedCards
            End Get
            Set(ByVal value As String)
                m_AcceptedCards = value
            End Set
        End Property

        Public Property BillingAddress() As String
            Get
                Return m_BillingAddress
            End Get
            Set(ByVal value As String)
                m_BillingAddress = value
            End Set
        End Property

        Public Property BillingCity() As String
            Get
                Return m_BillingCity
            End Get
            Set(ByVal value As String)
                m_BillingCity = value
            End Set
        End Property

        Public Property BillingState() As String
            Get
                Return m_BillingState
            End Get
            Set(ByVal value As String)
                m_BillingState = value
            End Set
        End Property

        Public Property BillingZip() As String
            Get
                Return m_BillingZip
            End Get
            Set(ByVal value As String)
                m_BillingZip = value
            End Set
        End Property

        Public Property BrandsSupplied() As String
            Get
                Return m_BrandsSupplied
            End Get
            Set(ByVal value As String)
                m_BrandsSupplied = value
            End Set
        End Property

        Public Property QuarterlyReportingOn() As Boolean
            Get
                Return m_QuarterlyReportingOn
            End Get
            Set(ByVal Value As Boolean)
                m_QuarterlyReportingOn = Value
            End Set
        End Property

        Public Property DashboardCategoryID() As Integer
            Get
                Return m_DashboardCategoryID
            End Get
            Set(ByVal Value As Integer)
                m_DashboardCategoryID = Value
            End Set
        End Property

        Public Property DB() As Database
            Get
                DB = m_DB
            End Get
            Set(ByVal Value As Database)
                m_DB = Value
            End Set
        End Property

        Public Sub New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal VendorID As Integer)
            m_DB = DB
            m_VendorID = VendorID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM Vendor WHERE VendorID = " & DB.Number(VendorID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_VendorID = Core.GetInt(r.Item("VendorID"))
            m_HistoricID = Core.GetInt(r.Item("HistoricID"))
            m_HistoricParentID = Core.GetInt(r.Item("HistoricParentID"))
            m_HistoricVendorID = Core.GetInt(r.Item("HistoricVendorID"))
            m_PrimaryVendorCategoryID = Core.GetInt(r.Item("PrimaryVendorCategoryID"))
            m_SecondaryVendorCategoryID = Core.GetInt(r.Item("SecondaryVendorCategoryID"))
            m_CRMID = Core.GetString(r.Item("CRMID"))
            m_CompanyName = Core.GetString(r.Item("CompanyName"))
            m_Address = Core.GetString(r.Item("Address"))
            m_Address2 = Core.GetString(r.Item("Address2"))
            m_City = Core.GetString(r.Item("City"))
            m_State = Core.GetString(r.Item("State"))
            m_Zip = Core.GetString(r.Item("Zip"))
            m_Phone = Core.GetString(r.Item("Phone"))
            m_Mobile = Core.GetString(r.Item("Mobile"))
            m_Pager = Core.GetString(r.Item("Pager"))
            m_OtherPhone = Core.GetString(r.Item("OtherPhone"))
            m_Fax = Core.GetString(r.Item("Fax"))
            m_Email = Core.GetString(r.Item("Email"))
            m_WebsiteURL = Core.GetString(r.Item("WebsiteURL"))
            m_LogoFile = Core.GetString(r.Item("LogoFile"))
            m_LogoGUID = Core.GetString(r.Item("LogoGUID"))
            m_AboutUs = Core.GetString(r.Item("AboutUs"))
            m_Submitted = Core.GetDate(r.Item("Submitted"))
            m_Updated = Core.GetDate(r.Item("Updated"))
            m_IsActive = Core.GetBoolean(r.Item("IsActive"))
            m_IsPlansOnline = Core.GetBoolean(r.Item("IsPlansOnline"))
            m_EnableMarketShare = Core.GetBoolean(r.Item("EnableMarketShare"))
            m_Comments = Core.GetString(r.Item("Comments"))
            m_GUID = Core.GetString(r.Item("GUID"))
            m_ServicesOffered = Core.GetString(r.Item("ServicesOffered"))
            m_Discounts = Core.GetString(r.Item("Discounts"))
            m_RebateProgram = Core.GetString(r.Item("RebateProgram"))
            m_PaymentTerms = Core.GetString(r.Item("PaymentTerms"))
            m_SpecialtyServices = Core.GetString(r.Item("SpecialtyServices"))
            m_AcceptedCards = Core.GetString(r.Item("AcceptedCards"))
            m_BillingAddress = Core.GetString(r.Item("BillingAddress"))
            m_BillingCity = Core.GetString(r.Item("BillingCity"))
            m_BillingState = Core.GetString(r.Item("BillingState"))
            m_BillingZip = Core.GetString(r.Item("BillingZip"))
            m_BrandsSupplied = Core.GetString(r.Item("BrandsSupplied"))
            m_ExcludedVendor = Core.GetBoolean(r.Item("ExcludedVendor"))
            m_HasDocumentsAccess = Core.GetBoolean(r.Item("HasDocumentsAccess"))
            m_QuarterlyReportingOn = Core.GetBoolean(r.Item("QuarterlyReportingOn"))
            m_DashboardCategoryID = Core.GetInt(r.Item("DashboardCategoryID"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            Dim MaximumHistoricId As Integer = Me.DB.ExecuteScalar("SELECT MAX(HistoricId) FROM (SELECT MAX(HistoricId) HistoricId FROM Builder UNION SELECT MAX(HistoricId) HistoricId FROM Vendor UNION SELECT MAX(HistoricId) HistoricId FROM NCPManufacturer) a")
            MaximumHistoricId = MaximumHistoricId + 1

            SQL = " INSERT INTO Vendor (" _
             & " HistoricID" _
             & ",HistoricParentID" _
             & ",HistoricVendorID" _
             & ",PrimaryVendorCategoryID" _
             & ",SecondaryVendorCategoryID" _
             & ",CRMID" _
             & ",CompanyName" _
             & ",Address" _
             & ",Address2" _
             & ",City" _
             & ",State" _
             & ",Zip" _
             & ",Phone" _
             & ",Mobile" _
             & ",Pager" _
             & ",OtherPhone" _
             & ",Fax" _
             & ",Email" _
             & ",WebsiteURL" _
             & ",LogoFile" _
             & ",LogoGUID" _
             & ",AboutUs" _
             & ",Submitted" _
             & ",Updated" _
             & ",IsActive" _
             & ",IsPlansOnline" _
             & ",EnableMarketShare" _
             & ",Comments" _
             & ",GUID" _
             & ",ServicesOffered" _
             & ",Discounts" _
             & ",RebateProgram" _
             & ",PaymentTerms" _
             & ",SpecialtyServices" _
             & ",AcceptedCards" _
             & ",BillingAddress" _
             & ",BillingCity" _
             & ",BillingState" _
             & ",BillingZip" _
             & ",BrandsSupplied" _
             & ",ExcludedVendor" _
             & ",HasDocumentsAccess" _
             & ",QuarterlyReportingOn" _
             & ",DashboardCategoryID" _
             & ") VALUES (" _
             & m_DB.NullNumber(MaximumHistoricId) _
             & "," & m_DB.NullNumber(HistoricParentID) _
             & "," & m_DB.NullNumber(HistoricVendorID) _
             & "," & m_DB.NullNumber(PrimaryVendorCategoryID) _
             & "," & m_DB.NullNumber(SecondaryVendorCategoryID) _
             & "," & m_DB.Quote(CRMID) _
             & "," & m_DB.Quote(CompanyName) _
             & "," & m_DB.Quote(Address) _
             & "," & m_DB.Quote(Address2) _
             & "," & m_DB.Quote(City) _
             & "," & m_DB.Quote(State) _
             & "," & m_DB.Quote(Zip) _
             & "," & m_DB.Quote(Phone) _
             & "," & m_DB.Quote(Mobile) _
             & "," & m_DB.Quote(Pager) _
             & "," & m_DB.Quote(OtherPhone) _
             & "," & m_DB.Quote(Fax) _
             & "," & m_DB.Quote(Email) _
             & "," & m_DB.Quote(WebsiteURL) _
             & "," & m_DB.Quote(LogoFile) _
             & "," & m_DB.Quote(LogoGUID) _
             & "," & m_DB.Quote(AboutUs) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullQuote(Now) _
             & "," & CInt(IsActive) _
             & "," & CInt(IsPlansOnline) _
             & "," & CInt(EnableMarketShare) _
             & "," & m_DB.Quote(Comments) _
             & "," & m_DB.Quote(GUID) _
             & "," & m_DB.Quote(ServicesOffered) _
             & "," & m_DB.Quote(Discounts) _
             & "," & m_DB.Quote(RebateProgram) _
             & "," & m_DB.Quote(PaymentTerms) _
             & "," & m_DB.Quote(SpecialtyServices) _
             & "," & m_DB.Quote(AcceptedCards) _
             & "," & m_DB.Quote(BillingAddress) _
             & "," & m_DB.Quote(BillingCity) _
             & "," & m_DB.Quote(BillingState) _
             & "," & m_DB.Quote(BillingZip) _
             & "," & m_DB.Quote(BrandsSupplied) _
             & "," & CInt(ExcludedVendor) _
             & "," & CInt(HasDocumentsAccess) _
             & "," & CInt(QuarterlyReportingOn) _
             & "," & m_DB.NullNumber(DashboardCategoryID) _
             & ")"

            VendorID = m_DB.InsertSQL(SQL)

            Return VendorID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE Vendor SET " _
             & "HistoricParentID = " & m_DB.NullNumber(HistoricParentID) _
             & ",HistoricVendorID = " & m_DB.NullNumber(HistoricVendorID) _
             & ",PrimaryVendorCategoryID = " & m_DB.NullNumber(PrimaryVendorCategoryID) _
             & ",SecondaryVendorCategoryID = " & m_DB.NullNumber(SecondaryVendorCategoryID) _
             & ",CRMID = " & m_DB.Quote(CRMID) _
             & ",CompanyName = " & m_DB.Quote(CompanyName) _
             & ",Address = " & m_DB.Quote(Address) _
             & ",Address2 = " & m_DB.Quote(Address2) _
             & ",City = " & m_DB.Quote(City) _
             & ",State = " & m_DB.Quote(State) _
             & ",Zip = " & m_DB.Quote(Zip) _
             & ",Phone = " & m_DB.Quote(Phone) _
             & ",Mobile = " & m_DB.Quote(Mobile) _
             & ",Pager = " & m_DB.Quote(Pager) _
             & ",OtherPhone = " & m_DB.Quote(OtherPhone) _
             & ",Fax = " & m_DB.Quote(Fax) _
             & ",Email = " & m_DB.Quote(Email) _
             & ",WebsiteURL = " & m_DB.Quote(WebsiteURL) _
             & ",LogoFile = " & m_DB.Quote(LogoFile) _
             & ",LogoGUID = " & m_DB.Quote(LogoGUID) _
             & ",AboutUs = " & m_DB.Quote(AboutUs) _
             & ",Updated = " & m_DB.NullQuote(Now) _
             & ",IsActive = " & CInt(IsActive) _
             & ",IsPlansOnline = " & CInt(IsPlansOnline) _
             & ",EnableMarketShare = " & CInt(EnableMarketShare) _
             & ",Comments = " & m_DB.Quote(Comments) _
             & ",GUID = " & m_DB.Quote(GUID) _
             & ",ServicesOffered = " & m_DB.Quote(ServicesOffered) _
             & ",Discounts = " & m_DB.Quote(Discounts) _
             & ",RebateProgram = " & m_DB.Quote(RebateProgram) _
             & ",PaymentTerms = " & m_DB.Quote(PaymentTerms) _
             & ",SpecialtyServices = " & m_DB.Quote(SpecialtyServices) _
             & ",AcceptedCards = " & m_DB.Quote(AcceptedCards) _
             & ",BillingAddress = " & m_DB.Quote(BillingAddress) _
             & ",BillingCity = " & m_DB.Quote(BillingCity) _
             & ",BillingState = " & m_DB.Quote(BillingState) _
             & ",BillingZip = " & m_DB.Quote(BillingZip) _
             & ",BrandsSupplied = " & m_DB.Quote(BrandsSupplied) _
             & ",ExcludedVendor = " & CInt(ExcludedVendor) _
             & ",HasDocumentsAccess = " & CInt(HasDocumentsAccess) _
             & ",QuarterlyReportingOn = " & CInt(QuarterlyReportingOn) _
             & ",DashboardCategoryID = " & m_DB.NullNumber(DashboardCategoryID) _
             & " WHERE VendorID = " & m_DB.Quote(VendorID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM Vendor WHERE VendorID = " & m_DB.Number(VendorID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove

        Public Shared Function GetActiveList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from Vendor WHERE IsActive = 1 "
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

    End Class

    Public Class VendorCollection
        Inherits GenericCollection(Of VendorRow)
    End Class

End Namespace

