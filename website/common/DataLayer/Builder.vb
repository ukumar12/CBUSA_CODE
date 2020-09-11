Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class BuilderRow
        Inherits BuilderRowBase

        Public Sub New()
            MyBase.New
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, BuilderID As Integer)
            MyBase.New(DB, BuilderID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal BuilderID As Integer) As BuilderRow
            Dim row As BuilderRow

            row = New BuilderRow(DB, BuilderID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal BuilderID As Integer)
            Dim row As BuilderRow

            row = New BuilderRow(DB, BuilderID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from Builder"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        '=================== ADDED BY APALA ON 18th JUNE 2020 ===========================
        Public Shared Function GetActiveOnlyList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "SELECT * FROM Builder WHERE IsActive = 1 "
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " ORDER BY " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Function GetBuilderByGuid(ByVal DB As Database, ByVal Guid As String) As BuilderRow
            Dim out As New BuilderRow(DB)
            Dim sql As String = "select * from Builder where Guid=" & DB.Quote(Guid)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function

        Public Shared Function GetByHistoricID(ByVal DB As Database, ByVal HistoricID As String) As BuilderRow
            Dim out As New BuilderRow(DB)
            Dim sql As String = "select * from Builder where HistoricID=" & DB.Quote(HistoricID)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function

        Public Shared Function GetListByLLC(ByVal DB As Database, ByVal LLCID As Integer, Optional ByVal CompanyNameLike As String = "") As DataTable
            Dim sql As String = "select * from Builder where IsActive=1 and LLCID=" & DB.Number(LLCID)
            If CompanyNameLike <> String.Empty Then
                sql &= " and CompanyName like " & DB.FilterQuote(CompanyNameLike)
            End If
            Return DB.GetDataTable(sql)
        End Function
        Public Shared Function GetListByLLCs(ByVal DB As Database, ByVal LLCs As String, Optional ByVal CompanyNameLike As String = "") As DataTable
            Dim sql As String = "select * from Builder where IsActive=1 and LLCID in " & DB.NumberMultiple(LLCs)
            If CompanyNameLike <> String.Empty Then
                sql &= " and CompanyName like " & DB.FilterQuote(CompanyNameLike)
            End If
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetSortedListByLLC(ByVal DB As Database, ByVal LLCID As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC")
            Dim sql As String = "select * from Builder where IsActive=1 and LLCID=" & DB.Number(LLCID)
            If SortBy <> String.Empty Then
                sql &= " order by " & Core.ProtectParam(SortBy & " " & SortOrder)
            End If
            Return DB.GetDataTable(sql)
        End Function
        Public Shared Function GetActiveList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from Builder where IsActive=1"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function
        Public Shared Function GetAllVendors(ByVal DB As Database, ByVal BuilderID As Integer) As DataTable
            Dim sql As String = "select * from Vendor where IsActive=1 and QuarterlyReportingOn=1 and VendorID in (select l.VendorID from LLCVendor l inner join Builder b on l.LLCID=b.LLCID where b.BuilderID=" & DB.Number(BuilderID) & ")"
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetAllVendors(ByVal DB As Database, ByVal BuilderID As Integer, ByVal RegistrationDeadline As DateTime) As DataTable
            Dim sql As String = "select * from Vendor where QuarterlyReportingOn=1 and IsActive=1 and Submitted < " & DB.NullQuote(RegistrationDeadline) & " and VendorID in (select l.VendorID from LLCVendor l inner join Builder b on l.LLCID=b.LLCID where b.BuilderID=" & DB.Number(BuilderID) & ")"
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetBuilderSubstitutions(ByVal DB As Database, ByVal BuilderId As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select bps.*, p1.Product, p2.Product as SubstituteProduct from BuilderProductSubstitution bps JOIN Product p1 on bps.ProductId = p1.ProductId JOIN Product p2 on bps.SubstituteProductId = p2.ProductId"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Public ReadOnly Property SupplyPhasesString() As String
        '    Get
        '        If Me.LLCID = 0 Then Return String.Empty
        '        Dim sql As String = "SELECT SupplyPhase FROM LLCSupplyPhases lsp JOIN SupplyPhase sp ON lsp.SupplyPhaseID = sp.SupplyPhaseID WHERE lsp.LLCID = " & DB.Number(Me.LLCID)
        '        Dim dt As DataTable = DB.GetDataTable(sql)
        '        Dim row As DataRow
        '        Dim buffer As String = ""

        '        For Each row In dt.Rows
        '            If buffer <> String.Empty Then buffer &= ","
        '            buffer &= row("SupplyPhase")
        '        Next
        '        SupplyPhasesString = buffer
        '    End Get
        'End Property

        Public Function UpdateGuid(ByVal NewGuid As String) As Boolean
            Dim sql As String = "update Builder set Guid=" & DB.Quote(NewGuid) & " where BuilderId=" & DB.Number(BuilderID)
            Return DB.ExecuteSQL(sql)
        End Function

        Public Shared Function IsNewBuilder(ByVal DB As Database, ByVal BuilderID As Integer) As Boolean
            'Dim sql As String = "select count(*) from BuilderAccount where HistoricPasswordSha1 is not null and BuilderID=" & DB.Number(BuilderID)
            'Return DB.ExecuteScalar(sql) = 0
            Return GetRow(DB, BuilderID).IsNew
        End Function

        Public Shared Function GetNextHistoricID(ByVal DB As Database)
            Return DB.InsertSQL("insert into HistoricIDSequence(CreateDate) values (" & DB.NullQuote(Now) & ")")
        End Function
        Public Shared Function GetBuilderByCRMID(ByVal DB As Database, ByVal CRMID As String) As BuilderRow
            Dim out As New BuilderRow(DB)
            Dim sql As String = "select * from Builder where CRMID=" & DB.Quote(CRMID)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function

        Public Shared Function GetBuilderByLikeCRMID(ByVal DB As Database, ByVal CRMID As String) As BuilderRow
            Dim out As New BuilderRow(DB)
            Dim sql As String = "select * from Builder where CRMID Like " & DB.FilterQuote(CRMID)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function



        Public Shared Function GetBuilderByName(ByVal DB As Database, ByVal Name As String) As BuilderRow
            Dim out As New BuilderRow(DB)
            Dim sql As String = "select top 1 * from Builder where (CRMID is NULL or CRMID='') and CompanyName=" & DB.Quote(Name)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function


    End Class

    Public MustInherit Class BuilderRowBase
        Private m_DB As Database
        Private m_BuilderID As Integer = Nothing
        Private m_CRMID As String = Nothing
        Private m_CompanyName As String = Nothing
        Private m_CompanyAlias As String = Nothing                 'Added by Apala (Medullus) on 11.09.2017 for VSO#8857
        Private m_Address As String = Nothing
        Private m_Address2 As String = Nothing
        Private m_City As String = Nothing
        Private m_State As String = Nothing
        Private m_Zip As String = Nothing
        Private m_Phone As String = Nothing
        Private m_Mobile As String = Nothing
        Private m_Fax As String = Nothing
        Private m_Email As String = Nothing
        Private m_WebsiteURL As String = Nothing
        Private m_RegistrationStatusID As Integer = Nothing
        Private m_PreferredVendorID As Integer = Nothing
        Private m_Submitted As DateTime = Nothing
        Private m_Updated As DateTime = Nothing
        Private m_IsActive As Boolean = Nothing
        Private m_Guid As String = Nothing
        Private m_LLCID As Integer = Nothing
        Private m_HistoricID As Integer = Nothing
        Private m_SkipEntitlementCheck As Boolean = Nothing
        Private m_UseAlternateBillingPlan As Boolean = Nothing
        Private m_IsNew As Boolean = Nothing
        Private m_IsPlansOnline As Boolean = Nothing
        Private m_HasDocumentsAccess As Boolean = Nothing
        Private m_RebatesEmailPreferences As Boolean = Nothing
        Private m_QuarterlyReportingOn As Boolean = Nothing ''''' added by Indranil

        Public Property BuilderID As Integer
            Get
                Return m_BuilderID
            End Get
            Set(ByVal Value As Integer)
                m_BuilderID = Value
            End Set
        End Property

        Public Property CRMID As String
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

        Public Property CompanyAlias() As String
            Get
                Return m_CompanyAlias
            End Get
            Set(ByVal Value As String)
                m_CompanyAlias = Value
            End Set
        End Property

        Public Property Address As String
            Get
                Return m_Address
            End Get
            Set(ByVal Value As String)
                m_Address = Value
            End Set
        End Property

        Public Property Address2 As String
            Get
                Return m_Address2
            End Get
            Set(ByVal Value As String)
                m_Address2 = Value
            End Set
        End Property

        Public Property City As String
            Get
                Return m_City
            End Get
            Set(ByVal Value As String)
                m_City = Value
            End Set
        End Property

        Public Property State As String
            Get
                Return m_State
            End Get
            Set(ByVal Value As String)
                m_State = Value
            End Set
        End Property

        Public Property Zip As String
            Get
                Return m_Zip
            End Get
            Set(ByVal Value As String)
                m_Zip = Value
            End Set
        End Property

        Public Property Phone As String
            Get
                Return m_Phone
            End Get
            Set(ByVal Value As String)
                m_Phone = Value
            End Set
        End Property

        Public Property Mobile As String
            Get
                Return m_Mobile
            End Get
            Set(ByVal Value As String)
                m_Mobile = Value
            End Set
        End Property

        Public Property Fax As String
            Get
                Return m_Fax
            End Get
            Set(ByVal Value As String)
                m_Fax = Value
            End Set
        End Property

        Public Property Email As String
            Get
                Return m_Email
            End Get
            Set(ByVal Value As String)
                m_Email = Value
            End Set
        End Property

        Public Property WebsiteURL As String
            Get
                Return m_WebsiteURL
            End Get
            Set(ByVal Value As String)
                m_WebsiteURL = Value
            End Set
        End Property

        Public Property RegistrationStatusID As Integer
            Get
                Return m_RegistrationStatusID
            End Get
            Set(ByVal Value As Integer)
                m_RegistrationStatusID = Value
            End Set
        End Property

        Public Property PreferredVendorID As Integer
            Get
                Return m_PreferredVendorID
            End Get
            Set(ByVal Value As Integer)
                m_PreferredVendorID = Value
            End Set
        End Property

        Public Property Submitted As DateTime
            Get
                Return m_Submitted
            End Get
            Set(ByVal Value As DateTime)
                m_Submitted = Value
            End Set
        End Property

        Public Property Updated As DateTime
            Get
                Return m_Updated
            End Get
            Set(ByVal Value As DateTime)
                m_Updated = Value
            End Set
        End Property

        Public Property IsActive As Boolean
            Get
                Return m_IsActive
            End Get
            Set(ByVal Value As Boolean)
                m_IsActive = Value
            End Set
        End Property

        Public Property Guid() As String
            Get
                Return m_Guid
            End Get
            Set(ByVal value As String)
                m_Guid = value
            End Set
        End Property

        Public Property LLCID() As Integer
            Get
                Return m_LLCID
            End Get
            Set(ByVal value As Integer)
                m_LLCID = value
            End Set
        End Property

        Public Property HistoricID() As Integer
            Get
                Return m_HistoricID
            End Get
            Set(ByVal value As Integer)
                m_HistoricID = value
            End Set
        End Property

        Public Property SkipEntitlementCheck() As Boolean
            Get
                Return m_SkipEntitlementCheck
            End Get
            Set(ByVal value As Boolean)
                m_SkipEntitlementCheck = value
            End Set
        End Property

        Public Property UseAlternateBillingPlan() As Boolean
            Get
                Return m_UseAlternateBillingPlan
            End Get
            Set(ByVal value As Boolean)
                m_UseAlternateBillingPlan = value
            End Set
        End Property

        Public Property IsNew() As Boolean
            Get
                Return m_IsNew
            End Get
            Set(ByVal value As Boolean)
                m_IsNew = value
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
        Public Property HasDocumentsAccess() As Boolean
            Get
                Return m_HasDocumentsAccess
            End Get
            Set(ByVal Value As Boolean)
                m_HasDocumentsAccess = Value
            End Set
        End Property

        Public Property RebatesEmailPreferences() As Boolean
            Get
                Return m_RebatesEmailPreferences
            End Get
            Set(ByVal Value As Boolean)
                m_RebatesEmailPreferences = Value
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



        Public Property DB() As Database
            Get
                DB = m_DB
            End Get
            Set(ByVal Value As DataBase)
                m_DB = Value
            End Set
        End Property




        Public Sub New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        Public Sub New(ByVal DB As Database, BuilderID As Integer)
            m_DB = DB
            m_BuilderID = BuilderID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM Builder WHERE BuilderID = " & DB.Number(BuilderID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_BuilderID = Convert.ToInt32(r.Item("BuilderID"))
            If IsDBNull(r.Item("CRMID")) Then
                m_CRMID = Nothing
            Else
                m_CRMID = Convert.ToString(r.Item("CRMID"))
            End If
            m_CompanyName = Convert.ToString(r.Item("CompanyName"))
            If IsDBNull(r.Item("Alias")) Then
                m_CompanyAlias = Nothing
            Else
                m_CompanyAlias = Convert.ToString(r.Item("Alias"))
            End If
            m_Address = Convert.ToString(r.Item("Address"))
            If IsDBNull(r.Item("Address2")) Then
                m_Address2 = Nothing
            Else
                m_Address2 = Convert.ToString(r.Item("Address2"))
            End If
            m_City = Convert.ToString(r.Item("City"))
            m_State = Convert.ToString(r.Item("State"))
            m_Zip = Convert.ToString(r.Item("Zip"))
            m_Phone = Convert.ToString(r.Item("Phone"))
            If IsDBNull(r.Item("Mobile")) Then
                m_Mobile = Nothing
            Else
                m_Mobile = Convert.ToString(r.Item("Mobile"))
            End If
            If IsDBNull(r.Item("Fax")) Then
                m_Fax = Nothing
            Else
                m_Fax = Convert.ToString(r.Item("Fax"))
            End If
            m_Email = Convert.ToString(r.Item("Email"))
            If IsDBNull(r.Item("WebsiteURL")) Then
                m_WebsiteURL = Nothing
            Else
                m_WebsiteURL = Convert.ToString(r.Item("WebsiteURL"))
            End If
            m_RegistrationStatusID = Convert.ToInt32(r.Item("RegistrationStatusID"))
            If IsDBNull(r.Item("PreferredVendorID")) Then
                m_PreferredVendorID = Nothing
            Else
                m_PreferredVendorID = Convert.ToInt32(r.Item("PreferredVendorID"))
            End If
            m_Submitted = Convert.ToDateTime(r.Item("Submitted"))
            If IsDBNull(r.Item("Updated")) Then
                m_Updated = Nothing
            Else
                m_Updated = Convert.ToDateTime(r.Item("Updated"))
            End If
            m_IsActive = Convert.ToBoolean(r.Item("IsActive"))
            m_Guid = Core.GetString(r.Item("Guid"))
            If IsDBNull(r.Item("LLCID")) Then
                m_LLCID = Nothing
            Else
                m_LLCID = Convert.ToInt32(r.Item("LLCID"))
            End If
            m_HistoricID = Core.GetInt(r.Item("HistoricID"))
            m_SkipEntitlementCheck = Core.GetBoolean(r.Item("SkipEntitlementCheck"))
            m_UseAlternateBillingPlan = Core.GetBoolean(r.Item("UseAlternateBillingPlan"))
            m_IsNew = Core.GetBoolean(r.Item("IsNew"))
            m_IsPlansOnline = Core.GetBoolean(r.Item("IsPlansOnline"))
            m_HasDocumentsAccess = Core.GetBoolean(r.Item("HasDocumentsAccess"))
            m_RebatesEmailPreferences = Core.GetBoolean(r.Item("RebatesEmailPreferences"))
            m_QuarterlyReportingOn = Core.GetBoolean(r.Item("QuarterlyReportingOn"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            Dim MaximumHistoricId As Integer = Me.DB.ExecuteScalar("SELECT MAX(HistoricId) FROM (SELECT MAX(HistoricId) HistoricId FROM Builder UNION SELECT MAX(HistoricId) HistoricId FROM Vendor UNION SELECT MAX(HistoricId) HistoricId FROM NCPManufacturer) a")
            MaximumHistoricId = MaximumHistoricId + 1

            SQL = " INSERT INTO Builder (" _
             & " CRMID" _
             & ",HistoricId" _
             & ",CompanyName" _
             & ",Address" _
             & ",Address2" _
             & ",City" _
             & ",State" _
             & ",Zip" _
             & ",Phone" _
             & ",Mobile" _
             & ",Fax" _
             & ",Email" _
             & ",WebsiteURL" _
             & ",RegistrationStatusID" _
             & ",PreferredVendorID" _
             & ",Submitted" _
             & ",Updated" _
             & ",IsActive" _
             & ",Guid" _
             & ",LLCID" _
             & ",SkipEntitlementCheck" _
             & ",UseAlternateBillingPlan" _
             & ",IsNew" _
             & ",IsPlansOnline" _
             & ",HasDocumentsAccess" _
             & ",RebatesEmailPreferences" _
             & ",Alias" _
             & ",QuarterlyReportingOn) VALUES (" _
             & m_DB.Quote(CRMID) _
             & "," & m_DB.NullNumber(MaximumHistoricId) _
             & "," & m_DB.Quote(CompanyName) _
             & "," & m_DB.Quote(Address) _
             & "," & m_DB.Quote(Address2) _
             & "," & m_DB.Quote(City) _
             & "," & m_DB.Quote(State) _
             & "," & m_DB.Quote(Zip) _
             & "," & m_DB.Quote(Phone) _
             & "," & m_DB.Quote(Mobile) _
             & "," & m_DB.Quote(Fax) _
             & "," & m_DB.Quote(Email) _
             & "," & m_DB.Quote(WebsiteURL) _
             & "," & m_DB.NullNumber(RegistrationStatusID) _
             & "," & m_DB.NullNumber(PreferredVendorID) _
             & "," & m_DB.NullQuote(Submitted) _
             & "," & m_DB.NullQuote(Updated) _
             & "," & CInt(IsActive) _
             & "," & m_DB.Quote(Guid) _
             & "," & m_DB.Number(LLCID) _
             & "," & CInt(SkipEntitlementCheck) _
             & "," & CInt(UseAlternateBillingPlan) _
             & "," & CInt(IsNew) _
             & "," & CInt(IsPlansOnline) _
             & "," & CInt(HasDocumentsAccess) _
             & "," & CInt(RebatesEmailPreferences) _
             & "," & m_DB.Quote(CompanyAlias) _
             & "," & CInt(QuarterlyReportingOn) _
             & ")"

            BuilderID = m_DB.InsertSQL(SQL)

            Return BuilderID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE Builder SET " _
             & " CRMID = " & m_DB.Quote(CRMID) _
             & ",CompanyName = " & m_DB.Quote(CompanyName) _
             & ",Address = " & m_DB.Quote(Address) _
             & ",Address2 = " & m_DB.Quote(Address2) _
             & ",City = " & m_DB.Quote(City) _
             & ",State = " & m_DB.Quote(State) _
             & ",Zip = " & m_DB.Quote(Zip) _
             & ",Phone = " & m_DB.Quote(Phone) _
             & ",Mobile = " & m_DB.Quote(Mobile) _
             & ",Fax = " & m_DB.Quote(Fax) _
             & ",Email = " & m_DB.Quote(Email) _
             & ",WebsiteURL = " & m_DB.Quote(WebsiteURL) _
             & ",RegistrationStatusID = " & m_DB.NullNumber(RegistrationStatusID) _
             & ",PreferredVendorID = " & m_DB.NullNumber(PreferredVendorID) _
             & ",Submitted = " & m_DB.NullQuote(Submitted) _
             & ",Updated = " & m_DB.NullQuote(Updated) _
             & ",IsActive = " & CInt(IsActive) _
             & ",LLCID = " & m_DB.Number(LLCID) _
             & ",Guid = " & m_DB.Quote(Guid) _
             & ",HistoricID = " & m_DB.Number(HistoricID) _
             & ",SkipEntitlementCheck = " & CInt(SkipEntitlementCheck) _
             & ",UseAlternateBillingPlan = " & CInt(UseAlternateBillingPlan) _
             & ",IsNew = " & CInt(IsNew) _
             & ",IsPlansOnline = " & CInt(IsPlansOnline) _
             & ",HasDocumentsAccess = " & CInt(HasDocumentsAccess) _
             & ",RebatesEmailPreferences = " & CInt(RebatesEmailPreferences) _
             & ",Alias = " & m_DB.Quote(CompanyAlias) _
             & ",QuarterlyReportingOn = " & CInt(QuarterlyReportingOn) _
             & " WHERE BuilderID = " & m_DB.Quote(BuilderID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM Builder WHERE BuilderID = " & m_DB.Number(BuilderID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class BuilderCollection
        Inherits GenericCollection(Of BuilderRow)
    End Class

End Namespace

