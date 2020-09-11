Imports Components
Imports DataLayer

Partial Class _404
    Inherits SitePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.StatusCode = 404
        Response.Status = "404 Not Found"
    End Sub
End Class
