Imports Components
Imports DataLayer
Imports System.Data.SqlClient

Partial Class store_checkout
    Inherits SitePage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureSSL()
        If IsLoggedIn() Then
            Response.Redirect("billing.aspx")
        End If
    End Sub

    Protected Sub btnGuest_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGuest.Click
        Response.Redirect("billing.aspx")
    End Sub

    Protected Sub btnLogin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLogin.Click
        If Not IsValid Then Exit Sub

        Dim MemberId As Integer = MemberRow.ValidateMemberCredentials(DB, txtUsername.Text, txtPassword.Text)
        If MemberId > 0 Then
            Session("MemberId") = MemberId
            If chkPersist.Checked = True Then Utility.CookieUtil.SetTripleDESEncryptedCookie("MemberId", Session("MemberId").ToString, Today.AddDays(15))
            Response.Redirect("billing.aspx")
        Else
            AddError("The password you entered does not match the one for this account. Please try again, or go to the <a href='/members/forgot.aspx'>forgot your password</a> page to retrieve it")
        End If
    End Sub

End Class
