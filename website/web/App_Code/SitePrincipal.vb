Imports System
Imports System.Collections
Imports System.Security
Imports System.Security.Cryptography
Imports System.Text
Imports System.Runtime.InteropServices
Imports DataLayer
Imports Utility

Namespace Components

	Public Class SitePrincipal
		Implements System.Security.Principal.IPrincipal

		Private m_DB As Database

		Public Sub New(ByVal _Database As Database, ByVal MemberId As Integer)
			m_DB = _Database
        End Sub

		Public Sub New(ByVal _dataBase As Database, ByVal Username As String)
			m_DB = _dataBase
		End Sub

        Public Shared Function ValidateLogin(ByVal db As Database, ByVal Username As String, ByVal Password As String, ByVal bPersist As Boolean) As Integer
            Return Nothing
        End Function

        ' IPrincipal Interface Implementation
        ' ---
        Public ReadOnly Property Identity() As Principal.IIdentity _
          Implements Principal.IPrincipal.Identity
            Get
                Return Nothing
            End Get

        End Property

        Public Function IsInRole(ByVal role As String) As Boolean Implements System.Security.Principal.IPrincipal.IsInRole
            Return True
        End Function
    End Class

End Namespace
