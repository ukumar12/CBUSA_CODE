Imports Components
Partial Class Gotodashboard
    Inherits BasePage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("BuilderId") IsNot Nothing Then
            Response.Redirect("/builder/default.aspx")
        Else
            Response.Redirect("/Vendor/default.aspx")
        End If
    End Sub
End Class
