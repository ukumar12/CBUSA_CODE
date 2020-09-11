Imports Components
Imports Utility

Partial Class SmartBug
    Inherits System.Web.UI.UserControl
    Implements ISmartBug

    Public Property URL() As String Implements ISmartBug.URL
        Get
            Return ViewState("URL")
        End Get
        Set(ByVal value As String)
            ViewState("URL") = value
        End Set
    End Property

    Protected ReadOnly Property BugVisible() As Boolean
        Get
            Return Not CookieUtil.GetTripleDESEncryptedCookieValue("smartbug") = String.Empty And Not URL = String.Empty And Not InStr(Page.Request.Path, "/admin/", CompareMethod.Text) > 0
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If URL = String.Empty Then
            URL = "/admin/default.aspx?FrameURL=" & Server.UrlEncode("/admin/content/edit.aspx?PageURL=" & Page.Request.Path)
        End If
    End Sub
End Class
