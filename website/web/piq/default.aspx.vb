Imports Components
Imports DataLayer
Imports System.Net.Mail

Partial Class _default
    Inherits SitePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsurePIQAccess()

        If Not CType(Me.Page, SitePage).IsLoggedInPIQ Then
            Response.Redirect("/default.aspx")
        End If

    End Sub

End Class
