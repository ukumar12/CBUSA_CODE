Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class LLCRow
        Inherits LLCRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal LLCID As Integer)
            MyBase.New(DB, LLCID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal LLCID As Integer) As LLCRow
            Dim row As LLCRow

            row = New LLCRow(DB, LLCID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal LLCID As Integer)
            Dim row As LLCRow

            row = New LLCRow(DB, LLCID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from LLC"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Function GetPricedProductsList(ByVal DB As Database, ByVal LLCId As Integer) As Generic.List(Of String)
            Dim out As New Generic.List(Of String)
            Dim sql As String = "select ProductId from VendorProductPrice where   VendorId in (select VendorId from LLCVendor where LLCId=" & DB.Number(LLCId) & ") order by ProductId Asc"
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            While sdr.Read
                out.Add(sdr.Item("ProductId"))
            End While
            sdr.Close()
            Return out
        End Function

        Public Shared Function GetPricedProductsListForSearch(ByVal DB As Database, ByVal LLCId As Integer) As Generic.List(Of String)
            Dim out As New Generic.List(Of String)
            Dim sql As String = "select vpp.ProductId from VendorProductPrice vpp inner Join vendor v on v.vendorid= vpp.vendorid    where v.isactive = 1 AND VendorPrice is not NULL AND vpp.VendorId in (select VendorId from LLCVendor where LLCId=" & DB.Number(LLCId) & ") order by ProductId Asc"
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            While sdr.Read
                out.Add(sdr.Item("ProductId"))
            End While
            sdr.Close()
            Return out
        End Function


        Public Shared Function GetPricedProductsListFromLLCs(ByVal DB As Database, ByVal LLCIds As String) As Generic.List(Of String)
            Dim out As New Generic.List(Of String)
            Dim sql As String = "select ProductId from VendorProductPrice where VendorId in (select VendorId from LLCVendor where LLCId in " & DB.NumberMultiple(LLCIds) & ") order by ProductId Asc"
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            While sdr.Read
                out.Add(sdr.Item("ProductId"))
            End While
            sdr.Close()
            Return out
        End Function

        Public Shared Function GetBuilderLLC(ByVal DB As Database, ByVal BuilderID As Integer) As LLCRow
            Dim out As New LLCRow(DB)
            Dim sql As String = "select l.* from LLC l inner join Builder b on l.LLCID=b.LLCID where b.BuilderID=" & DB.Number(BuilderID)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function

        Public Shared Function GetRowByCity(ByVal DB As Database, ByVal City As String) As LLCRow
            Dim out As New LLCRow(DB)
            Dim sql As String = "select * from LLC where LLC=" & DB.Quote(City)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function

        Public Shared Function RemoveDuplicates(ByVal stext As String) As String
            Dim CommaDelimitedString As CommaDelimitedStringCollection = New CommaDelimitedStringCollection

            CommaDelimitedString.AddRange(stext.Split(",").Distinct.ToArray())
            Return CommaDelimitedString.ToString

        End Function


        Public Shared Function GetLLCNotificationList(ByVal DB As Database, Optional ByVal LLCId As String = "") As String
            Dim out As String = String.Empty
            Dim sql As String = "SELECT   STUFF((SELECT ', ' + NotificationEmailList FROM LLC "

            If LLCId <> String.Empty Then
                sql &= " where LLCID in  " & DB.QuoteMultiple(LLCId)
            End If
            sql &= " FOR XML PATH(''), TYPE) .value('.','NVARCHAR(MAX)'),1,2,' ') NotificationEmailList "
            out = Core.GetString(DB.ExecuteScalar(sql))

            If out Is Nothing Then
                Return String.Empty
            Else
                Return RemoveDuplicates(out)
            End If

            Return out
        End Function

    End Class

    Public MustInherit Class LLCRowBase
        Private m_DB As Database
        Private m_LLCID As Integer = Nothing
        Private m_LLC As String = Nothing
        Private m_Description As String = Nothing
        Private m_DiscrepencyTolerance As Double = Nothing
        Private m_IsActive As Boolean = Nothing
        Private m_Created As DateTime = Nothing
        Private m_ExcludeFromReports As Boolean = Nothing
        Private m_EnableTakeOffService As Boolean = Nothing
        Private m_AllowExcludingVendors As Boolean = Nothing
        Private m_DefaultRebate As Double = Nothing
        Private m_BuilderGroup As String = Nothing
        Private m_OperationsManager As String = Nothing
        Private m_NotificationEmailList As String = Nothing
        Private m_AffiliateID As Integer = Nothing
        Private m_RegBillingPlanId As Integer = Nothing
        Private m_SubBillingPlanId As Integer = Nothing
        Private m_BillingStartDate As DateTime = Nothing

        Public Property EnableTakeOffService() As Boolean
            Get
                Return m_EnableTakeOffService
            End Get
            Set(ByVal value As Boolean)
                m_EnableTakeOffService = value
            End Set
        End Property

        Public Property LLCID() As Integer
            Get
                Return m_LLCID
            End Get
            Set(ByVal Value As Integer)
                m_LLCID = value
            End Set
        End Property
        Public Property DefaultRebate() As Double
            Get
                Return m_DefaultRebate
            End Get
            Set(ByVal Value As Double)
                m_DefaultRebate = Value
            End Set
        End Property

        Public Property LLC() As String
            Get
                Return m_LLC
            End Get
            Set(ByVal Value As String)
                m_LLC = value
            End Set
        End Property

        Public Property Description() As String
            Get
                Return m_Description
            End Get
            Set(ByVal Value As String)
                m_Description = value
            End Set
        End Property

        Public Property DiscrepencyTolerance() As Double
            Get
                Return m_DiscrepencyTolerance
            End Get
            Set(ByVal Value As Double)
                m_DiscrepencyTolerance = value
            End Set
        End Property
        Public Property AllowExcludingVendors() As Boolean
            Get
                Return m_AllowExcludingVendors
            End Get
            Set(ByVal Value As Boolean)
                m_AllowExcludingVendors = Value
            End Set
        End Property
        Public Property IsActive() As Boolean
            Get
                Return m_IsActive
            End Get
            Set(ByVal Value As Boolean)
                m_IsActive = value
            End Set
        End Property
        Public Property BuilderGroup() As String
            Get
                Return m_BuilderGroup
            End Get
            Set(ByVal Value As String)
                m_BuilderGroup = Value
            End Set
        End Property
        Public Property OperationsManager() As String
            Get
                Return m_OperationsManager
            End Get
            Set(ByVal Value As String)
                m_OperationsManager = Value
            End Set
        End Property
        Public Property NotificationEmailList() As String
            Get
                Return m_NotificationEmailList
            End Get
            Set(ByVal Value As String)
                m_NotificationEmailList = Value
            End Set
        End Property
        Public ReadOnly Property Created() As DateTime
            Get
                Return m_Created
            End Get
        End Property

        Public Property ExcludeFromReports() As Boolean
            Get
                Return m_ExcludeFromReports
            End Get
            Set(ByVal value As Boolean)
                m_ExcludeFromReports = value
            End Set
        End Property

        Public Property AffiliateID() As Integer
            Get
                Return m_AffiliateID
            End Get
            Set(ByVal Value As Integer)
                m_AffiliateID = Value
            End Set
        End Property

        Public Property RegBillingPlanId() As Integer
            Get
                Return m_RegBillingPlanId
            End Get
            Set(ByVal Value As Integer)
                m_RegBillingPlanId = Value
            End Set
        End Property

        Public Property SubBillingPlanId() As Integer
            Get
                Return m_SubBillingPlanId
            End Get
            Set(ByVal Value As Integer)
                m_SubBillingPlanId = Value
            End Set
        End Property

        Public Property BillingStartDate() As DateTime
            Get
                Return m_BillingStartDate
            End Get
            Set(ByVal Value As DateTime)
                m_BillingStartDate = Value
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

        Public Sub New(ByVal DB As Database, ByVal LLCID As Integer)
            m_DB = DB
            m_LLCID = LLCID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM LLC WHERE LLCID = " & DB.Number(LLCID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_LLCID = Convert.ToInt32(r.Item("LLCID"))
            m_LLC = Convert.ToString(r.Item("LLC"))
            If IsDBNull(r.Item("Description")) Then
                m_Description = Nothing
            Else
                m_Description = Convert.ToString(r.Item("Description"))
            End If
            m_DiscrepencyTolerance = Convert.ToDouble(r.Item("DiscrepencyTolerance"))
            m_IsActive = Convert.ToBoolean(r.Item("IsActive"))
            m_AllowExcludingVendors = Convert.ToBoolean(r.Item("AllowExcludingVendors"))
            m_Created = Convert.ToDateTime(r.Item("Created"))
            m_ExcludeFromReports = Convert.ToBoolean(r.Item("ExcludeFromReports"))
            If IsDBNull(r.Item("DefaultRebate")) Then
                m_DefaultRebate = Nothing
            Else
                m_DefaultRebate = Convert.ToDouble(r.Item("DefaultRebate"))
            End If
            m_BuilderGroup = Convert.ToString(r.Item("BuilderGroup"))
            m_OperationsManager = Convert.ToString(r.Item("OperationsManager"))
            m_NotificationEmailList = Convert.ToString(r.Item("NotificationEmailList"))
            m_AffiliateID = Convert.ToInt32(r.Item("AffiliateID"))
            m_EnableTakeOffService = Convert.ToBoolean(r.Item("EnableTakeOffService"))

            m_RegBillingPlanId = Convert.ToInt32(r.Item("RegBillingPlanId"))
            m_SubBillingPlanId = Convert.ToInt32(r.Item("SubBillingPlanId"))

            If IsDBNull(r.Item("BillingStartDate")) Then
                m_BillingStartDate = Nothing
            Else
                m_BillingStartDate = Convert.ToDateTime(r.Item("BillingStartDate")).Date
            End If

        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO LLC (" _
             & " LLC" _
             & ",Description" _
             & ",DiscrepencyTolerance" _
             & ",IsActive" _
             & ",Created" _
             & ",ExcludeFromReports" _
             & ",DefaultRebate" _
             & ",BuilderGroup" _
             & ",OperationsManager" _
             & ",NotificationEmailList" _
             & ",AllowExcludingVendors" _
             & ",AffiliateID" _
             & ",EnableTakeOffService" _
             & ",RegBillingPlanId" _
             & ",SubBillingPlanId" _
             & ",BillingStartDate" _
             & ") VALUES (" _
             & m_DB.Quote(LLC) _
             & "," & m_DB.Quote(Description) _
             & "," & m_DB.Number(DiscrepencyTolerance) _
             & "," & CInt(IsActive) _
             & "," & m_DB.NullQuote(Now) _
             & "," & CInt(ExcludeFromReports) _
             & "," & m_DB.Number(DefaultRebate) _
             & "," & m_DB.Quote(BuilderGroup) _
             & "," & m_DB.Quote(OperationsManager) _
             & "," & m_DB.Quote(NotificationEmailList) _
             & "," & CInt(AllowExcludingVendors) _
             & "," & m_DB.Number(AffiliateID) _
             & "," & CInt(EnableTakeOffService) _
             & "," & CInt(RegBillingPlanId) _
             & "," & CInt(SubBillingPlanId) _
             & "," & m_DB.NullQuote(BillingStartDate) _
             & ")"

            LLCID = m_DB.InsertSQL(SQL)

            Return LLCID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE LLC SET " _
             & " LLC = " & m_DB.Quote(LLC) _
             & ",Description = " & m_DB.Quote(Description) _
             & ",DiscrepencyTolerance = " & m_DB.Number(DiscrepencyTolerance) _
             & ",IsActive = " & CInt(IsActive) _
             & ",ExcludeFromReports = " & CInt(ExcludeFromReports) _
             & ",DefaultRebate = " & m_DB.Number(DefaultRebate) _
             & ",BuilderGroup = " & m_DB.Quote(BuilderGroup) _
             & ",OperationsManager  = " & m_DB.Quote(OperationsManager) _
             & ",NotificationEmailList  = " & m_DB.Quote(NotificationEmailList) _
             & ",AllowExcludingVendors = " & CInt(AllowExcludingVendors) _
             & ",EnableTakeOffService = " & CInt(EnableTakeOffService) _
             & ",AffiliateID  = " & m_DB.Number(AffiliateID) _
             & ",RegBillingPlanId  = " & m_DB.Number(RegBillingPlanId) _
             & ",SubBillingPlanId  = " & m_DB.Number(SubBillingPlanId) _
             & ",BillingStartDate  = " & m_DB.NullQuote(BillingStartDate) _
             & " WHERE LLCID = " & m_DB.Quote(LLCID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM LLC WHERE LLCID = " & m_DB.Number(LLCID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class LLCCollection
        Inherits GenericCollection(Of LLCRow)
    End Class

End Namespace


