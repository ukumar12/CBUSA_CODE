Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports Utility
Imports Components

Public Class BuilderBillingPlanRow
    Inherits BuilderBillingPlanRowBase

    Public Sub New()
        MyBase.New()
    End Sub 'New

    Public Sub New(ByVal DB As Database)
        MyBase.New(DB)
    End Sub 'New

    Public Sub New(ByVal DB As Database, ByVal BuilderId As Integer)
        MyBase.New(DB, BuilderId)
    End Sub 'New

    'Shared function to get one row
    Public Shared Function GetRow(ByVal DB As Database, ByVal BuilderId As Integer) As BuilderBillingPlanRowBase
        Dim row As BuilderBillingPlanRow

        row = New BuilderBillingPlanRow(DB, BuilderId)
        row.Load()

        Return row
    End Function

    Public Shared Sub RemoveRow(ByVal DB As Database, ByVal BuilderId As Integer)
        Dim row As BuilderBillingPlanRow

        row = New BuilderBillingPlanRow(DB, BuilderId)
        row.Remove()
    End Sub

End Class

Public MustInherit Class BuilderBillingPlanRowBase

    Private m_DB As Database

    Private m_BuilderBillingPlanId As Integer = Nothing
    Private m_BuilderId As Integer = Nothing
    Private m_RegBillingPlanId As Integer = Nothing
    Private m_SubBillingPlanId As Integer = Nothing
    Private m_FreeTrialPeriod As Integer = Nothing
    Private m_BillingStartDate As DateTime = Nothing
    Private m_SubscriptionStartDate As DateTime = Nothing
    Private m_BillingLastSuccess As DateTime = Nothing
    Private m_BillingSubscriptionAutobill As String = Nothing
    Private m_BillingMembershipAutobill As String = Nothing
    Private m_BillingProcessorId As String = Nothing

    Public Property DB() As Database
        Get
            DB = m_DB
        End Get
        Set(ByVal Value As Database)
            m_DB = Value
        End Set
    End Property

    Public Property BuilderBillingPlanId() As Integer
        Get
            Return m_BuilderBillingPlanId
        End Get
        Set(ByVal Value As Integer)
            m_BuilderBillingPlanId = Value
        End Set
    End Property

    Public Property BuilderId() As Integer
        Get
            Return m_BuilderId
        End Get
        Set(ByVal Value As Integer)
            m_BuilderId = Value
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

    Public Property FreeTrialPeriod() As Integer
        Get
            Return m_FreeTrialPeriod
        End Get
        Set(ByVal Value As Integer)
            m_FreeTrialPeriod = Value
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

    Public Property SubscriptionStartDate() As DateTime
        Get
            Return m_SubscriptionStartDate
        End Get
        Set(ByVal Value As DateTime)
            m_SubscriptionStartDate = Value
        End Set
    End Property

    Public Property BillingLastSuccess() As DateTime
        Get
            Return m_BillingLastSuccess
        End Get
        Set(ByVal Value As DateTime)
            m_BillingLastSuccess = Value
        End Set
    End Property

    Public Property BillingSubscriptionAutobill() As String
        Get
            Return m_BillingSubscriptionAutobill
        End Get
        Set(ByVal Value As String)
            m_BillingSubscriptionAutobill = Value
        End Set
    End Property

    Public Property BillingMembershipAutobill() As String
        Get
            Return m_BillingMembershipAutobill
        End Get
        Set(ByVal Value As String)
            m_BillingMembershipAutobill = Value
        End Set
    End Property

    Public Property BillingProcessorId() As String
        Get
            Return m_BillingProcessorId
        End Get
        Set(ByVal Value As String)
            m_BillingProcessorId = Value
        End Set
    End Property

    Public Sub New()

    End Sub 'New

    Public Sub New(ByVal database As Database)
        m_DB = database
    End Sub 'New

    Public Sub New(ByVal database As Database, ByVal BuilderId As Integer)
        m_DB = database
        m_BuilderId = BuilderId
    End Sub 'New

    Protected Overridable Sub Load()
        Dim r As SqlDataReader
        Dim SQL As String

        SQL = "SELECT * FROM BuilderBillingPlan WHERE BuilderId = " & DB.Quote(BuilderId)
        r = m_DB.GetReader(SQL)
        If r.Read Then
            Me.Load(r)
        End If
        r.Close()
    End Sub

    Protected Overridable Sub Load(ByVal r As SqlDataReader)
        m_BuilderBillingPlanId = Convert.ToInt32(r.Item("BuilderBillingPlanId"))
        m_BuilderId = Convert.ToInt32(r.Item("BuilderId"))
        m_RegBillingPlanId = Convert.ToInt32(r.Item("RegBillingPlanId"))
        m_SubBillingPlanId = Convert.ToInt32(r.Item("SubBillingPlanId"))

        If r.Item("FreeTrialPeriod") Is Convert.DBNull Then
            m_FreeTrialPeriod = Nothing
        Else
            m_FreeTrialPeriod = Convert.ToInt32(r.Item("FreeTrialPeriod"))
        End If

        If r.Item("BillingStartDate") Is Convert.DBNull Then
            m_BillingStartDate = Nothing
        Else
            m_BillingStartDate = Convert.ToDateTime(r.Item("BillingStartDate"))
        End If

        If r.Item("SubscriptionStartDate") Is Convert.DBNull Then
            m_SubscriptionStartDate = Nothing
        Else
            m_SubscriptionStartDate = Convert.ToDateTime(r.Item("SubscriptionStartDate"))
        End If

        If r.Item("BillingLastSuccess") Is Convert.DBNull Then
            m_BillingLastSuccess = Nothing
        Else
            m_BillingLastSuccess = Convert.ToDateTime(r.Item("BillingLastSuccess"))
        End If

        If r.Item("BillingSubscriptionAutobill") Is Convert.DBNull Then
            m_BillingSubscriptionAutobill = Nothing
        Else
            m_BillingSubscriptionAutobill = Convert.ToString(r.Item("BillingSubscriptionAutobill"))
        End If

        If r.Item("BillingMembershipAutobill") Is Convert.DBNull Then
            m_BillingMembershipAutobill = Nothing
        Else
            m_BillingMembershipAutobill = Convert.ToString(r.Item("BillingMembershipAutobill"))
        End If

        If r.Item("BillingProcessorId") Is Convert.DBNull Then
            m_BillingProcessorId = Nothing
        Else
            m_BillingProcessorId = Convert.ToString(r.Item("BillingProcessorId"))
        End If

    End Sub 'Load

    Public Overridable Function Insert() As Integer
        Dim SQL As String

        SQL = " INSERT INTO BuilderBillingPlan (" _
         & " BuilderId" _
         & ",RegBillingPlanId" _
         & ",SubBillingPlanId" _
         & ",FreeTrialPeriod" _
         & ",BillingStartDate" _
         & ",SubscriptionStartDate" _
         & ",BillingLastSuccess" _
         & ",BillingSubscriptionAutobill" _
         & ",BillingMembershipAutobill" _
         & ",BillingProcessorId" _
         & ") VALUES (" _
         & m_DB.Quote(BuilderId) _
         & "," & m_DB.Quote(RegBillingPlanId) _
         & "," & m_DB.Quote(SubBillingPlanId) _
         & "," & m_DB.Quote(FreeTrialPeriod) _
         & "," & m_DB.Quote(BillingStartDate) _
         & "," & m_DB.Quote(SubscriptionStartDate) _
         & "," & m_DB.Quote(BillingLastSuccess) _
         & "," & m_DB.Quote(BillingSubscriptionAutobill) _
         & "," & m_DB.Quote(BillingMembershipAutobill) _
         & "," & m_DB.Quote(BillingProcessorId) _
         & ")"

        Return m_DB.InsertSQL(SQL)
    End Function

    Public Overridable Sub Update()
        Dim SQL As String

        SQL = " UPDATE BuilderBillingPlan SET " _
         & " RegBillingPlanId = " & m_DB.Quote(RegBillingPlanId) _
         & ",SubBillingPlanId = " & m_DB.Quote(SubBillingPlanId) _
         & ",FreeTrialPeriod = " & m_DB.Quote(FreeTrialPeriod) _
         & ",BillingStartDate = " & m_DB.Quote(BillingStartDate) _
         & ",SubscriptionStartDate = " & m_DB.Quote(SubscriptionStartDate) _
         & ",BillingLastSuccess = " & m_DB.Quote(BillingLastSuccess) _
         & ",BillingSubscriptionAutobill = " & m_DB.Quote(BillingSubscriptionAutobill) _
         & ",BillingMembershipAutobill = " & m_DB.Quote(BillingMembershipAutobill) _
         & ",BillingProcessorId = " & m_DB.Quote(BillingProcessorId) _
         & " WHERE BuilderId = " & m_DB.Quote(BuilderId)

        m_DB.ExecuteSQL(SQL)

    End Sub 'Update

    Public Sub Remove()
        Dim SQL As String

        SQL = "DELETE FROM BuilderBillingPlan WHERE BuilderId = " & BuilderId
        m_DB.ExecuteSQL(SQL)

    End Sub 'Remove

End Class
