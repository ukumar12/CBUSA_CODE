Imports Components
Imports Utility

Partial Class admin_SmartBug
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            chkSmartBug.Checked = IIf(CookieUtil.GetTripleDESEncryptedCookieValue("smartbug") = String.Empty, False, True)
        End If
    End Sub

    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
        CookieUtil.SetTripleDESEncryptedCookie("smartbug", IIf(chkSmartBug.Checked, "Y", String.Empty), Now.AddMonths(1))
        Response.Redirect("/admin/main.aspx")
    End Sub
End Class
