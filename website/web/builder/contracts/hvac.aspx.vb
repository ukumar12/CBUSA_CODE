Imports Components
Imports DataLayer
Imports System.Net.Mail

Partial Class _default
    Inherits SitePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureBuilderAccess()

    End Sub

End Class
