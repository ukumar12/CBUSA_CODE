Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class NotificationRow
        Inherits NotificationRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal NotificationId As Integer)
            MyBase.New(DB, NotificationId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal NotificationId As Integer) As NotificationRow
            Dim row As NotificationRow

            row = New NotificationRow(DB, NotificationId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal NotificationId As Integer)
            Dim row As NotificationRow

            row = New NotificationRow(DB, NotificationId)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from Notification"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        Public Shared Function GetActiveListByAccountId(ByVal DB As Database, ByVal IdValue As Integer, ByVal IdField As String) As DataTable
            Dim SQL As String = "select * from Notification where " & Core.ProtectParam(IdField) & " = " & DB.Number(IdValue) & " and ProcessDate is null order by CreateDate desc"
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Function Add(ByVal DB As Database, ByVal IdValue As Integer, ByVal IdField As String, ByVal Message As String) As Integer
            Dim dbNotification As New NotificationRow(DB)
            Select Case IdField
                Case "AdminId"
                    dbNotification.AdminId = IdValue
                Case "VendorAccountId"
                    dbNotification.VendorAccountId = IdValue
                Case "BuilderAccountId"
                    dbNotification.BuilderAccountId = IdValue
                Case "PIQAccountId"
                    dbNotification.PIQAccountId = IdValue
            End Select
            dbNotification.Message = Message
            Return dbNotification.Insert
        End Function

    End Class

    Public MustInherit Class NotificationRowBase
        Private m_DB As Database
        Private m_NotificationId As Integer = Nothing
        Private m_AdminId As Integer = Nothing
        Private m_VendorAccountId As Integer = Nothing
        Private m_BuilderAccountId As Integer = Nothing
        Private m_PIQAccountId As Integer = Nothing
        Private m_CreateDate As DateTime = Nothing
        Private m_Message As String = Nothing
        Private m_ProcessDate As DateTime = Nothing


        Public Property NotificationId() As Integer
            Get
                Return m_NotificationId
            End Get
            Set(ByVal Value As Integer)
                m_NotificationId = value
            End Set
        End Property

        Public Property AdminId() As Integer
            Get
                Return m_AdminId
            End Get
            Set(ByVal Value As Integer)
                m_AdminId = value
            End Set
        End Property

        Public Property VendorAccountId() As Integer
            Get
                Return m_VendorAccountId
            End Get
            Set(ByVal Value As Integer)
                m_VendorAccountId = Value
            End Set
        End Property

        Public Property BuilderAccountId() As Integer
            Get
                Return m_BuilderAccountId
            End Get
            Set(ByVal Value As Integer)
                m_BuilderAccountId = Value
            End Set
        End Property

        Public Property PIQAccountId() As Integer
            Get
                Return m_PIQAccountId
            End Get
            Set(ByVal Value As Integer)
                m_PIQAccountId = Value
            End Set
        End Property

        Public Property Message() As String
            Get
                Return m_Message
            End Get
            Set(ByVal Value As String)
                m_Message = Value
            End Set
        End Property

        Public Property ProcessDate() As DateTime
            Get
                Return m_ProcessDate
            End Get
            Set(ByVal Value As DateTime)
                m_ProcessDate = value
            End Set
        End Property

        Public ReadOnly Property CreateDate() As DateTime
            Get
                Return m_CreateDate
            End Get
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

        Public Sub New(ByVal DB As Database, ByVal NotificationId As Integer)
            m_DB = DB
            m_NotificationId = NotificationId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM Notification WHERE NotificationId = " & DB.Number(NotificationId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_NotificationId = Convert.ToInt32(r.Item("NotificationId"))
            If IsDBNull(r.Item("AdminId")) Then
                m_AdminId = Nothing
            Else
                m_AdminId = Convert.ToInt32(r.Item("AdminId"))
            End If
            If IsDBNull(r.Item("VendorAccountID")) Then
                m_VendorAccountId = Nothing
            Else
                m_VendorAccountId = Convert.ToInt32(r.Item("VendorAccountID"))
            End If
            If IsDBNull(r.Item("BuilderAccountID")) Then
                m_BuilderAccountId = Nothing
            Else
                m_BuilderAccountId = Convert.ToInt32(r.Item("BuilderAccountID"))
            End If
            If IsDBNull(r.Item("PIQAccountID")) Then
                m_PIQAccountId = Nothing
            Else
                m_PIQAccountId = Convert.ToInt32(r.Item("PIQAccountID"))
            End If
            m_CreateDate = Convert.ToDateTime(r.Item("CreateDate"))
            m_Message = Convert.ToString(r.Item("Message"))
            If IsDBNull(r.Item("ProcessDate")) Then
                m_ProcessDate = Nothing
            Else
                m_ProcessDate = Convert.ToDateTime(r.Item("ProcessDate"))
            End If
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO Notification (" _
             & " AdminId" _
             & ",VendorAccountId" _
             & ",BuilderAccountId" _
             & ",PIQAccountId" _
             & ",CreateDate" _
             & ",Message" _
             & ",ProcessDate" _
             & ") VALUES (" _
             & m_DB.NullNumber(AdminId) _
             & "," & m_DB.NullNumber(VendorAccountId) _
             & "," & m_DB.NullNumber(BuilderAccountId) _
             & "," & m_DB.NullNumber(PIQAccountId) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.Quote(Message) _
             & "," & m_DB.NullQuote(ProcessDate) _
             & ")"

            NotificationId = m_DB.InsertSQL(SQL)

            Return NotificationId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE Notification SET " _
             & " AdminId = " & m_DB.NullNumber(AdminId) _
             & ",VendorAccountId = " & m_DB.NullNumber(VendorAccountId) _
             & ",BuilderAccountId = " & m_DB.NullNumber(BuilderAccountId) _
             & ",PIQAccountId = " & m_DB.NullNumber(PIQAccountId) _
             & ",Message = " & m_DB.Quote(Message) _
             & ",ProcessDate = " & m_DB.NullQuote(ProcessDate) _
             & " WHERE NotificationId = " & m_DB.Quote(NotificationId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM Notification WHERE NotificationId = " & m_DB.Number(NotificationId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class NotificationCollection
        Inherits GenericCollection(Of NotificationRow)
    End Class

End Namespace