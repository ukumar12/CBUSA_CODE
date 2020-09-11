Imports System.Web.UI

Namespace Components
    Public Class NameValuePair

        Private m_Name As String
        Public Property Name() As String
            Get
                Return m_Name
            End Get
            Set(ByVal value As String)
                m_Name = value
            End Set
        End Property

        Private m_Value As String
        Public Property Value() As String
            Get
                Return m_Value
            End Get
            Set(ByVal value As String)
                m_Value = value
            End Set
        End Property

        Public Sub New(ByVal Name As String, ByVal Value As String)
            m_Name = Name
            m_Value = Value
        End Sub
    End Class
End Namespace
