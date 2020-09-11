Imports System
Imports System.Collections
Imports System.Security
Imports System.Security.Cryptography
Imports System.Text
Imports DataLayer

Namespace Components

	Public Class AdminPrincipal
		Implements System.Security.Principal.IPrincipal

		Private m_Identity As Principal.IIdentity
		Private m_PermissionList As AdminSectionCollection
		Private m_Username As String
		Private m_IsInternal As Boolean
		Private m_DB As Database

        Public Sub New(ByVal DB As Database, ByVal AdminId As Integer)
            m_DB = DB
            Dim admin As AdminRow = AdminRow.GetRow(DB, AdminId)

            m_PermissionList = admin.GetPermissionList()
            m_Identity = New AdminIdentity(AdminId, admin.Username, admin.FirstName, admin.LastName, admin.IsInternal)
            m_IsInternal = admin.IsInternal
            m_Username = admin.Username
        End Sub

        Public Sub New(ByVal DB As Database, ByVal username As String)
            m_DB = DB

            Dim admin As AdminRow = AdminRow.GetRowByUsername(DB, username)
            m_PermissionList = admin.GetPermissionList()
            m_Identity = New AdminIdentity(admin.AdminId, admin.Username, admin.FirstName, admin.LastName, admin.IsInternal)
            m_IsInternal = admin.IsInternal
            m_Username = admin.Username
        End Sub

		Public Function HasPermission(ByVal action As String) As Boolean
			Dim section As AdminSectionRow

			For Each privilege As Object In m_PermissionList
				section = CType(privilege, AdminSectionRow)
				If section.Code = action Then
					Return True
				End If
			Next
			Return False
		End Function

		Public Property IsInternal() As Boolean
			Get
				Return m_IsInternal
			End Get
			Set(ByVal Value As Boolean)
				m_IsInternal = Value
			End Set
		End Property

		Public Property Username() As String
			Get
				Return m_Username
			End Get
			Set(ByVal Value As String)
				m_Username = Value
			End Set
		End Property

        Public Shared Function ValidateLogin(ByVal db As Database, ByVal username As String, ByVal password As String, ByVal Groups As String, ByRef response As ValidateCredentialsResponse) As AdminPrincipal
            Dim AdminId As Integer = 0

            'check database users first
            AdminId = AdminRow.ValidateAdminCredentials(db, username, password, response)
            If AdminId > 0 Then
                Return New AdminPrincipal(db, AdminId)
            End If


            Return Nothing
        End Function

		' IPrincipal Interface Implementation
		' ---
		Public ReadOnly Property Identity() As Principal.IIdentity _
		  Implements Principal.IPrincipal.Identity
			Get
				Return m_Identity
			End Get

		End Property

		Public Function IsInRole(ByVal role As String) As Boolean Implements System.Security.Principal.IPrincipal.IsInRole
			Return True
		End Function
	End Class

End Namespace
