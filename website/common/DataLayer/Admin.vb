Option Explicit On 

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports Utility
Imports Components

Namespace DataLayer

    Public Class AdminRow
        Inherits AdminRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal AdminId As Integer)
            MyBase.New(DB, AdminId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal AdminId As Integer) As AdminRow
            Dim row As AdminRow

            row = New AdminRow(DB, AdminId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal AdminId As Integer)
            Dim row As AdminRow

            row = New AdminRow(DB, AdminId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Function GetPermissionList() As AdminSectionCollection
            Return AdminSectionRow.GetPermissionList(DB, AdminId)
        End Function

        Public Shared Function GetRowByUsername(ByVal DB As Database, ByVal UserName As String) As AdminRow
            Dim SQL As String = "SELECT * FROM Admin WHERE Username = " & DB.Quote(username)
            Dim r As SqlDataReader

            Dim row As AdminRow = New AdminRow(DB)
            r = DB.GetReader(SQL)
            If r.Read Then
                row.Load(r)
            End If
            r.Close()

            Return row
		End Function

		Public Shared Function GetAllAdmins(ByVal DB As Database) As DataTable
			Dim dt As DataTable = DB.GetDataTable("select adminid, firstname + ' ' + lastname + ' (' + email + ')' as username from Admin order by Username")
			Return dt
		End Function

        'This function returns AdminId and Error Message when Password is not set or expired
        Public Shared Function ValidateAdminCredentials(ByVal DB As Database, ByVal UserName As String, ByVal Password As String, ByRef Response As ValidateCredentialsResponse) As Integer
            ' CHECK IF USER EXISTS AND HAS ADMIN PROVILEGES
            Dim dbAdmin As AdminRow = AdminRow.GetRowByUsername(DB, UserName)
            If dbAdmin.AdminId = 0 Then
                Response = ValidateCredentialsResponse.WrongUsername
                Return 0
            End If
            If dbAdmin.IsLocked Then
                Response = ValidateCredentialsResponse.LockedUser
                Return 0
            End If
            If dbAdmin.Password <> Password Then
                Response = ValidateCredentialsResponse.WrongPassword
                Return 0
            End If
            If dbAdmin.PasswordDate = Nothing Then
                Response = ValidateCredentialsResponse.NewUser
                Return dbAdmin.AdminId
            End If
            If Not dbAdmin.PasswordDate = Nothing AndAlso DateDiff("d", dbAdmin.PasswordDate, Now()) > 90 Then
                Response = ValidateCredentialsResponse.ExpiredPassword
                Return dbAdmin.AdminId
            End If
            Response = ValidateCredentialsResponse.Valid
            Return dbAdmin.AdminId
        End Function

        Public Shared Function ValidateSecondaryCredentials(ByVal DB As Database, ByVal UserName As String, ByVal Password As String) As Integer
            Dim dbAdmin As AdminRow = AdminRow.GetRowByUsername(DB, UserName)
            If dbAdmin.PasswordEx <> Password Then
                Return 0
            End If
            Return dbAdmin.AdminId
        End Function

        Public Shared Sub InsertAdminLog(ByVal DB As Database, ByVal AdminId As Integer, ByVal SessionId As String, ByVal Username As String, ByVal RemoteAddr As String, ByVal Succeeded As Boolean)
            Dim dbAdminLog As AdminLogRow

            dbAdminLog = New AdminLogRow(DB)
            dbAdminLog.AdminId = AdminId
            dbAdminLog.Username = Username
            dbAdminLog.RemoteIP = RemoteAddr
            dbAdminLog.LoginDate = Now()
            dbAdminLog.Succeeded = Succeeded
            dbAdminLog.Insert()
        End Sub

        Public Shared Sub LockUser(ByVal DB As Database, ByVal Username As String)
            Dim SQL As String = "Update Admin set IsLocked = 1 WHERE Username = " & DB.Quote(Username)
            DB.ExecuteSQL(SQL)
        End Sub

    End Class

    Public MustInherit Class AdminRowBase
        Private m_DB As Database
        Private m_AdminId As Integer = Nothing
        Private m_Username As String = Nothing
        Private m_Password As String = Nothing
        Private m_PasswordEx As String = Nothing
        Private m_Email As String = Nothing
        Private m_FirstName As String = Nothing
        Private m_LastName As String = Nothing
        Private m_IsInternal As Boolean = Nothing
        Private m_IsLocked As Boolean = Nothing
        Private m_PasswordDate As DateTime = Nothing

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

        Public Property Password() As String
            Get
                Return m_Password
            End Get
            Set(ByVal Value As String)
                m_Password = value
            End Set
        End Property

        Public Property PasswordEx() As String
            Get
                Return m_PasswordEx
            End Get
            Set(ByVal Value As String)
                m_PasswordEx = Value
            End Set
        End Property

        Public ReadOnly Property EncryptedPassword() As String
            Get
                If m_Password = String.Empty Then
                    Return String.Empty
                End If
                Return Crypt.EncryptTripleDES(Password)
            End Get
        End Property

        Public ReadOnly Property EncryptedPasswordEx() As String
            Get
                If m_PasswordEx = String.Empty Then
                    Return String.Empty
                End If
                Return Crypt.EncryptTripleDes(PasswordEx)
            End Get
        End Property

        Public Property Email() As String
            Get
                Return m_Email
            End Get
            Set(ByVal Value As String)
                m_Email = Value
            End Set
        End Property

        Public Property FirstName() As String
            Get
                Return m_FirstName
            End Get
            Set(ByVal Value As String)
                m_FirstName = Value
            End Set
        End Property

        Public Property LastName() As String
            Get
                Return m_LastName
            End Get
            Set(ByVal Value As String)
                m_LastName = Value
            End Set
        End Property

        Public Property IsInternal() As Boolean
            Get
                Return m_IsInternal
            End Get
            Set(ByVal Value As Boolean)
                m_IsInternal = Value
            End Set
        End Property

        Public Property IsLocked() As Boolean
            Get
                Return m_IsLocked
            End Get
            Set(ByVal Value As Boolean)
                m_IsLocked = Value
            End Set
        End Property

        Public Property PasswordDate() As DateTime
            Get
                Return m_PasswordDate
            End Get
            Set(ByVal Value As DateTime)
                m_PasswordDate = Value
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

        Public Sub New(ByVal database As Database, ByVal AdminId As Integer)
            m_DB = database
            m_AdminId = AdminId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM Admin WHERE AdminId = " & DB.Quote(AdminId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_AdminId = Convert.ToInt32(r.Item("AdminId"))
            m_Username = Convert.ToString(r.Item("Username"))
            m_Password = Crypt.DecryptTripleDES(IIf(IsDBNull(r.Item("Password")), String.Empty, r.Item("Password")))
            m_PasswordEx = Crypt.DecryptTripleDes(IIf(IsDBNull(r.Item("PasswordEx")), String.Empty, r.Item("PasswordEx")))
            If r.Item("Email") Is Convert.DBNull Then
                m_Email = Nothing
            Else
                m_Email = Convert.ToString(r.Item("Email"))
            End If
            If r.Item("FirstName") Is Convert.DBNull Then
                m_FirstName = Nothing
            Else
                m_FirstName = Convert.ToString(r.Item("FirstName"))
            End If
            If r.Item("LastName") Is Convert.DBNull Then
                m_LastName = Nothing
            Else
                m_LastName = Convert.ToString(r.Item("LastName"))
            End If
            m_IsInternal = Convert.ToBoolean(r.Item("IsInternal"))
            m_IsLocked = Convert.ToBoolean(r.Item("IsLocked"))
            If r.Item("PasswordDate") Is Convert.DBNull Then
                m_PasswordDate = Nothing
            Else
                m_PasswordDate = Convert.ToDateTime(r.Item("PasswordDate"))
            End If
        End Sub 'Load

        Public Overridable Function AutoInsert() As Integer
            Dim SQL As String

            SQL = " INSERT INTO Admin (" _
             & " Username" _
             & ",Password" _
             & ",PasswordEx" _
             & ",Email" _
             & ",FirstName" _
             & ",LastName" _
             & ") VALUES (" _
             & m_DB.Quote(Username) _
             & "," & m_DB.Quote(EncryptedPassword) _
             & "," & m_DB.Quote(EncryptedPasswordEx) _
             & "," & m_DB.Quote(Email) _
             & "," & m_DB.Quote(FirstName) _
             & "," & m_DB.Quote(LastName) _
             & ")"

            Return m_DB.InsertSQL(SQL)
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE Admin SET " _
             & " Username = " & m_DB.Quote(Username) _
             & ",Password = " & m_DB.Quote(EncryptedPassword) _
             & ",PasswordEx = " & m_DB.Quote(EncryptedPasswordEx) _
             & ",Email = " & m_DB.Quote(Email) _
             & ",FirstName = " & m_DB.Quote(FirstName) _
             & ",LastName = " & m_DB.Quote(LastName) _
             & ",IsInternal = " & CInt(IsInternal) _
             & ",IsLocked = " & CInt(IsLocked) _
             & ",PasswordDate = " & m_DB.Quote(PasswordDate) _
             & " WHERE AdminId = " & m_DB.Quote(AdminId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "Delete from AdminAdminGroup where AdminId = " & AdminId
            DB.ExecuteSQL(SQL)

            SQL = "DELETE FROM Admin WHERE AdminId = " & AdminId
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class AdminCollection
        Inherits GenericCollection(Of AdminRow)
    End Class

    Public Enum ValidateCredentialsResponse
        Valid = 1
        WrongUsername = 2
        WrongPassword = 3
        NewUser = 4
        ExpiredPassword = 5
        LockedUser = 6
    End Enum

End Namespace
