Imports Utility

Namespace Components

    Public Class DBConnectionString

        Private m_ConnectionString As String
        Private m_Username As String
        Private m_Password As String

        Sub New(ByVal cs As String, ByVal Username As String, ByVal Password As String)
            m_ConnectionString = cs

            EncryptedUsername = Username
            EncryptedPassword = Password
        End Sub

        Public Shared Function GetConnectionString(ByVal cs, ByVal Username, ByVal Password) As String
            Dim ConnString As DBConnectionString = New DBConnectionString(cs, Username, Password)
            Return ConnString.ConnectionString
        End Function

        Protected ReadOnly Property ConnectionString() As String
            Get
                Return String.Format(m_ConnectionString, Username, Password)
            End Get
        End Property

        Protected WriteOnly Property EncryptedUsername() As String
            Set(ByVal value As String)
                m_Username = value
            End Set
        End Property

        Protected ReadOnly Property Username() As String
            Get
                Return Crypt.DecryptTripleDES(m_Username)
            End Get
        End Property

        Protected WriteOnly Property EncryptedPassword() As String
            Set(ByVal value As String)
                m_Password = value
            End Set
        End Property

        Protected ReadOnly Property Password() As String
            Get
                Return Crypt.DecryptTripleDES(m_Password)
            End Get
        End Property
    End Class

End Namespace