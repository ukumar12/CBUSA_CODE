Imports Components

Partial Class logout
    Inherits BasePage
    Private PageUrl As String = ""
    Private CurrentUserId As String = ""
    Private UserName As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        FormsAuthentication.SignOut()

        Session("AdminExpiredPassword") = False
        Session("AdminIsNew") = False
        Core.LogEvent("User """ & Session("Username") & """ has logged out", Diagnostics.EventLogEntryType.Information)
        Session.Abandon()
        'log admin login
        PageUrl = Request.Url.ToString()
        CurrentUserId = Session("AdminId")
        UserName = Session("Username")
        Core.DataLog("Logout", PageUrl, CurrentUserId, "Admin Logout", "", "", "", "", UserName)
        'end log
        Response.Redirect("/includes/redirect.aspx?URL=" + Server.UrlEncode("/admin/"))
    End Sub
End Class
