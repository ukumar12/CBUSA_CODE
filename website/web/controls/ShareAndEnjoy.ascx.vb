Imports Components
Imports Utility
Imports System.Configuration.ConfigurationManager

Partial Class ShareAndEnjoy
    Inherits System.Web.UI.UserControl

    Public Property URL() As String
        Get
            Return ViewState("URL")
        End Get
        Set(ByVal value As String)
            ViewState("URL") = value
        End Set
    End Property

    Public Property Title() As String
        Get
            Return ViewState("Title")
        End Get
        Set(ByVal value As String)
            ViewState("Title") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If URL = String.Empty Then
            URL = Server.UrlEncode(AppSettings("GlobalRefererName") & Page.Request.RawUrl)
        End If
    End Sub
End Class
