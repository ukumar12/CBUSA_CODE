Imports Components
Imports DataLayer
Imports System.Net.Mail

Partial Class _default
    Inherits SitePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureVendorAccess()

        If Not CType(Me.Page, SitePage).IsLoggedInVendor Then
            Response.Redirect("/default.aspx")
        End If

        Dim dtRoles As DataTable = VendorRoleRow.GetVendorRoles(DB, Session("VendorID"))
        Dim bValid As Boolean = True
        For Each row As DataRow In dtRoles.Rows
            If IsDBNull(row("VendorAccountID")) Then
                Session("errVendorUserRoles") = "Please select a user for every role."
                Response.Redirect("/forms/vendor-registration/users.aspx")
            End If
        Next

    End Sub

End Class
