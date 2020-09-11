
Partial Class rebates_temp_impersonate_vendor
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Request("VendorId") <> Nothing Then
            Session("VendorId") = Request("VendorId")
        Else
            'Session("VendorId") = 35631
            Session("VendorId") = 35625
        End If
        Session("VendorAccountId") = 1
        Response.Redirect("vendor-sales.aspx")
    End Sub
End Class
