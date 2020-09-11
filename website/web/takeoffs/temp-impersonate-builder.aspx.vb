Imports Components
Imports DataLayer

Partial Class takeoffs_temp_set_takeoff_for
    Inherits SitePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Request("id") Is Nothing Then
            Session("BuilderId") = 23180
        Else
            Session("BuilderId") = Request("id")
        End If
        Session("BuilderAccountId") = 1
        Response.Redirect("default.aspx")
    End Sub
End Class
