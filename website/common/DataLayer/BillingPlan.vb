Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports Utility
Imports Components

Public Enum BILLING_PLAN_TYPE
    REGISTRATION = 1
    SUBSCRIPTION = 2
End Enum

Public Class BillingPlanRow
    Inherits BillingPlanRowBase

    Public Sub New()
        MyBase.New()
    End Sub 'New

    Public Sub New(ByVal DB As Database)
        MyBase.New(DB)
    End Sub 'New

    Public Sub New(ByVal DB As Database, ByVal BillingPlanId As Integer)
        MyBase.New(DB, BillingPlanId)
    End Sub 'New

    'Shared function to get one row
    Public Shared Function GetRow(ByVal DB As Database, ByVal BillingPlanId As Integer) As BillingPlanRow
        Dim row As BillingPlanRow

        row = New BillingPlanRow(DB, BillingPlanId)
        row.Load()

        Return row
    End Function

    Public Shared Sub RemoveRow(ByVal DB As Database, ByVal BillingPlanId As Integer)
        Dim row As BillingPlanRow

        row = New BillingPlanRow(DB, BillingPlanId)
        row.Load()

        row.Remove()
    End Sub

    Public Shared Function GetDefaultBillingPlan(ByVal DB As Database, Optional ByVal BillingPlanType As Int16 = BILLING_PLAN_TYPE.REGISTRATION) As BillingPlanRow

        Dim row As BillingPlanRow
        row = New BillingPlanRow(DB)
        row.LoadDefault(BillingPlanType)

        Return row

    End Function

    Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
        Dim SQL As String = "SELECT * FROM BillingPlan "
        If Not SortBy = String.Empty Then
            SortBy = Core.ProtectParam(SortBy)
            SortOrder = Core.ProtectParam(SortOrder)

            SQL &= " ORDER BY " & SortBy & " " & SortOrder
        End If
        Return DB.GetDataTable(SQL)
    End Function

    Public Shared Function GetActiveBillingPlans(ByVal DB As Database, Optional BillingPlanType As Int16 = BILLING_PLAN_TYPE.REGISTRATION, Optional ByVal SortBy As String = "") As DataTable
        Dim SQL As String = "SELECT * FROM BillingPlan WHERE RecordState = 1"

        If Not BillingPlanType = Nothing Then
            SQL &= " AND BillingPlanType = " & Core.ProtectParam(BillingPlanType)
        End If

        If Not SortBy = String.Empty Then
            SortBy = Core.ProtectParam(SortBy)

            SQL &= " ORDER BY " & SortBy
        End If
        Return DB.GetDataTable(SQL)
    End Function

End Class

Public MustInherit Class BillingPlanRowBase

    Private m_DB As Database

    Private m_BillingPlanId As Integer = Nothing
    Private m_DisplayValue As String = Nothing
    Private m_SortValue As String = Nothing
    Private m_Description As String = Nothing
    Private m_BillingPlanType As BILLING_PLAN_TYPE = Nothing
    Private m_ExternalSystem As String = Nothing
    Private m_ExternalKey As String = Nothing
    Private m_IsDefault As Boolean = Nothing
    Private m_RecordState As Int16 = Nothing
    Private m_CreatedOn As DateTime = Nothing
    Private m_CreatedBy As Integer = Nothing
    Private m_ModifiedOn As DateTime = Nothing
    Private m_ModifiedBy As Integer = Nothing
    Private m_RowID As Guid = Nothing

    Public Property DB() As Database
        Get
            DB = m_DB
        End Get
        Set(ByVal Value As Database)
            m_DB = Value
        End Set
    End Property

    Public Property BillingPlanId() As Integer
        Get
            Return m_BillingPlanId
        End Get
        Set(ByVal Value As Integer)
            m_BillingPlanId = Value
        End Set
    End Property

    Public Property DisplayValue() As String
        Get
            Return m_DisplayValue
        End Get
        Set(ByVal Value As String)
            m_DisplayValue = Value
        End Set
    End Property

    Public Property SortValue() As String
        Get
            Return m_SortValue
        End Get
        Set(ByVal Value As String)
            m_SortValue = Value
        End Set
    End Property

    Public Property Description() As String
        Get
            Return m_Description
        End Get
        Set(ByVal Value As String)
            m_Description = Value
        End Set
    End Property

    Public Property BillingPlanType() As BILLING_PLAN_TYPE
        Get
            Return m_BillingPlanType
        End Get
        Set(ByVal Value As BILLING_PLAN_TYPE)
            m_BillingPlanType = Value
        End Set
    End Property

    Public Property ExternalSystem() As String
        Get
            Return m_ExternalSystem
        End Get
        Set(ByVal Value As String)
            m_ExternalSystem = Value
        End Set
    End Property

    Public Property ExternalKey() As String
        Get
            Return m_ExternalKey
        End Get
        Set(ByVal Value As String)
            m_ExternalKey = Value
        End Set
    End Property

    Public Property IsDefault() As Boolean
        Get
            Return m_IsDefault
        End Get
        Set(ByVal Value As Boolean)
            m_IsDefault = Value
        End Set
    End Property

    Public Property RecordState() As Int16
        Get
            Return m_RecordState
        End Get
        Set(ByVal Value As Int16)
            m_RecordState = Value
        End Set
    End Property

    Public Property CreatedOn() As DateTime
        Get
            Return m_CreatedOn
        End Get
        Set(ByVal Value As DateTime)
            m_CreatedOn = Value
        End Set
    End Property

    Public Property CreatedBy() As Integer
        Get
            Return m_CreatedBy
        End Get
        Set(ByVal Value As Integer)
            m_CreatedBy = Value
        End Set
    End Property

    Public Property ModifiedOn() As DateTime
        Get
            Return m_ModifiedOn
        End Get
        Set(ByVal Value As DateTime)
            m_ModifiedOn = Value
        End Set
    End Property

    Public Property ModifiedBy() As Integer
        Get
            Return m_ModifiedBy
        End Get
        Set(ByVal Value As Integer)
            m_ModifiedBy = Value
        End Set
    End Property

    Public Property RowID() As Guid
        Get
            Return m_RowID
        End Get
        Set(ByVal Value As Guid)
            m_RowID = Value
        End Set
    End Property

    Public Sub New()

    End Sub 'New

    Public Sub New(ByVal database As Database)
        m_DB = database
    End Sub 'New

    Public Sub New(ByVal database As Database, ByVal BillingPlanId As Integer)
        m_DB = database
        m_BillingPlanId = BillingPlanId
    End Sub 'New

    Protected Overridable Sub Load()
        Dim r As SqlDataReader
        Dim SQL As String

        SQL = "SELECT * FROM BillingPlan WHERE BillingPlanId = " & DB.Quote(BillingPlanId)
        r = m_DB.GetReader(SQL)
        If r.Read Then
            Me.Load(r)
        End If
        r.Close()
    End Sub

    Protected Overridable Sub LoadDefault(Optional ByVal BillingPlanType As Int16 = BILLING_PLAN_TYPE.REGISTRATION)
        Dim r As SqlDataReader
        Dim SQL As String

        SQL = "SELECT * FROM BillingPlan WHERE IsDefault = 1 AND BillingPlanType = " & Core.ProtectParam(BillingPlanType)
        r = m_DB.GetReader(SQL)
        If r.Read Then
            Me.Load(r)
        End If
        r.Close()
    End Sub

    Protected Overridable Sub Load(ByVal r As SqlDataReader)
        m_BillingPlanId = Convert.ToInt32(r.Item("BillingPlanId"))

        If r.Item("DisplayValue") Is Convert.DBNull Then
            m_DisplayValue = Nothing
        Else
            m_DisplayValue = Convert.ToString(r.Item("DisplayValue"))
        End If

        If r.Item("SortValue") Is Convert.DBNull Then
            m_SortValue = Nothing
        Else
            m_SortValue = Convert.ToString(r.Item("SortValue"))
        End If

        If r.Item("Description") Is Convert.DBNull Then
            m_Description = Nothing
        Else
            m_Description = Convert.ToString(r.Item("Description"))
        End If

        If r.Item("BillingPlanType") Is Convert.DBNull Then
            m_BillingPlanType = Nothing
        Else
            m_BillingPlanType = Convert.ToInt16(r.Item("BillingPlanType"))
        End If

        If r.Item("ExternalSystem") Is Convert.DBNull Then
            m_ExternalSystem = Nothing
        Else
            m_ExternalSystem = Convert.ToString(r.Item("ExternalSystem"))
        End If

        If r.Item("ExternalKey") Is Convert.DBNull Then
            m_ExternalKey = Nothing
        Else
            m_ExternalKey = Convert.ToString(r.Item("ExternalKey"))
        End If

        If r.Item("IsDefault") Is Convert.DBNull Then
            m_IsDefault = Nothing
        Else
            m_IsDefault = Convert.ToBoolean(r.Item("IsDefault"))
        End If

        If r.Item("RecordState") Is Convert.DBNull Then
            m_RecordState = Nothing
        Else
            m_RecordState = Convert.ToInt16(r.Item("RecordState"))
        End If

    End Sub 'Load

    Public Overridable Function Insert() As Integer
        Dim SQL As String

        SQL = " INSERT INTO BillingPlan (" _
         & " DisplayValue" _
         & ",SortValue" _
         & ",Description" _
         & ",BillingPlanType" _
         & ",ExternalSystem" _
         & ",ExternalKey" _
         & ",IsDefault" _
         & ",RecordState" _
         & ",CreatedBy" _
         & ",ModifiedBy" _
         & ",RowID" _
         & ") VALUES (" _
         & m_DB.Quote(DisplayValue) _
         & "," & m_DB.Quote(SortValue) _
         & "," & m_DB.Quote(Description) _
         & "," & BillingPlanType _
         & "," & m_DB.Quote(ExternalSystem) _
         & "," & m_DB.Quote(ExternalKey) _
         & "," & m_DB.Quote(IsDefault) _
         & "," & m_DB.Quote(RecordState) _
         & "," & m_DB.Quote(CreatedBy) _
         & "," & m_DB.Quote(ModifiedBy) _
         & "," & m_DB.Quote(Guid.NewGuid().ToString()) _
         & ")"

        Return m_DB.InsertSQL(SQL)
    End Function

    Public Overridable Sub Update()
        Dim SQL As String

        SQL = " UPDATE BillingPlan SET " _
         & " DisplayValue = " & m_DB.Quote(DisplayValue) _
         & ",SortValue = " & m_DB.Quote(SortValue) _
         & ",Description = " & m_DB.Quote(Description) _
         & ",BillingPlanType = " & BillingPlanType _
         & ",ExternalSystem = " & m_DB.Quote(ExternalSystem) _
         & ",ExternalKey = " & m_DB.Quote(ExternalKey) _
         & ",IsDefault = " & CInt(IsDefault) _
         & ",RecordState = " & CInt(RecordState) _
         & ",ModifiedOn = " & m_DB.Quote(ModifiedOn) _
         & " WHERE BillingPlanId = " & m_DB.Quote(BillingPlanId)

        m_DB.ExecuteSQL(SQL)

    End Sub 'Update

    Public Sub Remove()
        'Get Default Billing Plan
        Dim r As SqlDataReader
        Dim SQLGetDefaultBP As String = ""

        Dim DefaultBillingPlanId As Int32 = 0

        If BillingPlanType = BILLING_PLAN_TYPE.REGISTRATION Then
            SQLGetDefaultBP = "SELECT BillingPlanId FROM BillingPlan WHERE BillingPlanType = " & Core.ProtectParam(BILLING_PLAN_TYPE.REGISTRATION) & " AND IsDefault = 1"
        ElseIf BillingPlanType = BILLING_PLAN_TYPE.SUBSCRIPTION Then
            SQLGetDefaultBP = "SELECT BillingPlanId FROM BillingPlan WHERE BillingPlanType = " & Core.ProtectParam(BILLING_PLAN_TYPE.SUBSCRIPTION) & " AND IsDefault = 1"
        End If

        r = m_DB.GetReader(SQLGetDefaultBP)

        If r.HasRows Then
            r.Read()
            DefaultBillingPlanId = r.GetSqlInt32(0)
        End If

        r.Close()

        Dim SQL As String

        If DefaultBillingPlanId > 0 Then

            If BillingPlanType = BILLING_PLAN_TYPE.REGISTRATION Then
                SQL = "UPDATE LLC SET RegBillingPlanId = " & DefaultBillingPlanId & " WHERE RegBillingPlanId = " & BillingPlanId
                DB.ExecuteSQL(SQL)

                SQL = "UPDATE BuilderBillingPlan SET RegBillingPlanId = " & DefaultBillingPlanId & " WHERE RegBillingPlanId = " & BillingPlanId
                DB.ExecuteSQL(SQL)

            ElseIf BillingPlanType = BILLING_PLAN_TYPE.SUBSCRIPTION Then
                SQL = "UPDATE LLC SET SubBillingPlanId = " & DefaultBillingPlanId & " WHERE SubBillingPlanId = " & BillingPlanId
                DB.ExecuteSQL(SQL)

                SQL = "UPDATE BuilderBillingPlan SET SubBillingPlanId = " & DefaultBillingPlanId & " WHERE SubBillingPlanId = " & BillingPlanId
                DB.ExecuteSQL(SQL)
            End If
        Else
            If BillingPlanType = BILLING_PLAN_TYPE.REGISTRATION Then
                SQL = "UPDATE LLC SET RegBillingPlanId = NULL WHERE RegBillingPlanId = " & BillingPlanId
                DB.ExecuteSQL(SQL)

                SQL = "UPDATE BuilderBillingPlan SET RegBillingPlanId = NULL WHERE RegBillingPlanId = " & BillingPlanId
                DB.ExecuteSQL(SQL)

            ElseIf BillingPlanType = BILLING_PLAN_TYPE.SUBSCRIPTION Then
                SQL = "UPDATE LLC SET SubBillingPlanId = NULL WHERE SubBillingPlanId = " & BillingPlanId
                DB.ExecuteSQL(SQL)

                SQL = "UPDATE BuilderBillingPlan SET SubBillingPlanId = NULL WHERE SubBillingPlanId = " & BillingPlanId
                DB.ExecuteSQL(SQL)
            End If
        End If

        'Mark the Billing Plan as "Deleted" - DO NOT Delete the actual record
        SQL = "UPDATE BillingPlan SET RecordState = 3 WHERE BillingPlanId = " & BillingPlanId
        m_DB.ExecuteSQL(SQL)

    End Sub 'Remove

End Class