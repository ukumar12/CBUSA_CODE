Option Strict Off

Imports Components
Imports DataLayer

Partial Class Content
    Inherits ModuleControl

    Private m_ContentId As String

    Public Overrides Property Args() As String
        Get
            Return ContentId
        End Get
        Set(ByVal Value As String)
            ContentId = Value
        End Set
    End Property

    Public Overrides ReadOnly Property EditMode() As Boolean
        Get
            Return True
        End Get
    End Property

    Protected Property ContentId() As String
        Get
            Return m_ContentId
        End Get
        Set(ByVal Value As String)
            m_ContentId = Value
        End Set
    End Property

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        HTMLContent = ContentToolContentRow.GetRow(DB, ContentId).Content
    End Sub
End Class
