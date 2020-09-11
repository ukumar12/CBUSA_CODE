Option Explicit On 

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class AdminLogRow
        Inherits AdminLogRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal LogId As Integer)
            MyBase.New(DB, LogId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal LogId As Integer) As AdminLogRow
            Dim row As AdminLogRow

            row = New AdminLogRow(DB, LogId)
            row.Load()

            Return row
        End Function

        'Custom Methods
        Public Shared Function GetLast10Logins(ByVal DB As Database, ByVal LoggedInIsInternal As Boolean) As DataTable
            Dim SQL As String = String.Empty
            If LoggedInIsInternal Then
                SQL = "SELECT TOP 10 a.FirstName + ' ' + a.LastName AS FullName, al.* from AdminLog al, Admin a where al.Succeeded = 1 and al.AdminId = a.AdminId ORDER BY al.LoginDate DESC"
            Else
                SQL = "SELECT TOP 10 a.FirstName + ' ' + a.LastName AS FullName, al.* from AdminLog al, Admin a where al.Succeeded = 1 and al.AdminId = a.AdminId and a.IsInternal = 0 ORDER BY al.LoginDate DESC"
            End If
            Dim dt As DataTable
            dt = DB.GetDataTable(SQL)

            Return dt
        End Function

    End Class

    Public MustInherit Class AdminLogRowBase
        Private m_DB As Database
        Private m_LogId As Integer = Nothing
        Private m_AdminId As Integer = Nothing
        Private m_Username As String = Nothing
        Private m_RemoteIP As String = Nothing
        Private m_LoginDate As DateTime = Nothing
        Private m_Succeeded As Boolean = False

        Public Property LogId() As Integer
            Get
                Return m_LogId
            End Get
            Set(ByVal Value As Integer)
                m_LogId = value
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

        Public Property Username() As String
            Get
                Return m_Username
            End Get
            Set(ByVal Value As String)
                m_Username = value
            End Set
        End Property

        Public Property RemoteIP() As String
            Get
                Return m_RemoteIP
            End Get
            Set(ByVal Value As String)
                m_RemoteIP = value
            End Set
        End Property

        Public Property LoginDate() As DateTime
            Get
                Return m_LoginDate
            End Get
            Set(ByVal Value As DateTime)
                m_LoginDate = value
            End Set
        End Property

        Public Property Succeeded() As Boolean
            Get
                Return m_Succeeded
            End Get
            Set(ByVal Value As Boolean)
                m_Succeeded = Value
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

        Public Sub New(ByVal database As Database)
            m_DB = database
        End Sub 'New

        Public Sub New(ByVal database As Database, ByVal LogId As Integer)
            m_DB = database
            m_LogId = LogId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM AdminLog WHERE LogId = " & DB.Quote(LogId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_AdminId = Convert.ToInt32(r.Item("AdminId"))
            m_Username = Convert.ToString(r.Item("Username"))
            m_RemoteIP = Convert.ToString(r.Item("RemoteIP"))
            m_LoginDate = Convert.ToDateTime(r.Item("LoginDate"))
            m_Succeeded = Convert.ToBoolean(r.Item("Succeeded"))
        End Sub 'Load

        Public Overridable Sub Insert()
            Dim SQL As String

            SQL = " INSERT INTO AdminLog (" _
             & " AdminId" _
             & ",Username" _
             & ",RemoteIP" _
             & ",LoginDate" _
             & ",Succeeded" _
             & ") VALUES (" _
             & m_DB.NullNumber(AdminId) _
             & "," & m_DB.Quote(Username) _
             & "," & m_DB.Quote(RemoteIP) _
             & "," & m_DB.Quote(LoginDate) _
             & "," & CInt(Succeeded) _
             & ")"

            m_DB.ExecuteSQL(SQL)
        End Sub 'Insert

        Function AutoInsert() As Integer
            Dim SQL As String = "SELECT SCOPE_IDENTITY()"

            Insert()
            Return m_DB.ExecuteScalar(SQL)
        End Function
    End Class

    Public Class AdminLogCollection
        Inherits GenericCollection(Of AdminLogRow)
    End Class

End Namespace


