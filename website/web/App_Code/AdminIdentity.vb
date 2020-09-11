Imports System
Imports System.Collections
Imports System.Data
Imports System.Security
Imports System.Security.Cryptography
Imports System.Text

Namespace Components

	Public Class AdminIdentity
		Implements System.Security.Principal.IIdentity

		Private m_Username As String
		Private m_FirstName As String
		Private m_LastName As String
		Private m_AdminId As Integer
		Private m_IsInternal As Boolean

		Public Property AdminId() As Integer
			Get
				Return m_AdminId
			End Get
			Set(ByVal Value As Integer)
				m_AdminId = Value
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

		Public Property Username() As String
			Get
				Return m_Username
			End Get
			Set(ByVal Value As String)
				m_Username = Value
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

		Public Sub New(ByVal AdminId As Integer, ByVal Username As String, ByVal FirstName As String, ByVal LastName As String, ByVal IsInternal As Boolean)
			Me.AdminId = AdminId
			Me.Username = Username
			Me.FirstName = FirstName
			Me.LastName = LastName
			Me.IsInternal = IsInternal
		End Sub

		' IIdentity Interface Implementation
		' ---
		Public ReadOnly Property AuthenticationType() As String _
		  Implements Principal.IIdentity.AuthenticationType
			Get
				Return "Custom Authentication"
			End Get

		End Property

		' IIdentity Interface Implementation
		' ---
		Public ReadOnly Property IsAuthenticated() As Boolean _
		  Implements Principal.IIdentity.IsAuthenticated
			Get
				' Assumption: All instances of a SiteIdentity have
				' already been authenticated. 
				Return True
			End Get

		End Property

		' IIdentity Interface Implementation
		' ---
		Public ReadOnly Property Name() As String _
		  Implements Principal.IIdentity.Name
			Get
				Return FirstName & " " & LastName
			End Get
		End Property
	End Class

End Namespace