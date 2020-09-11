Imports Components
Imports DataLayer
Imports System.Data.SqlClient

Public Class members_view_order
    Inherits SitePage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureSSL()
        If IsLoggedIn() Then Response.Redirect("/members/orders/")
    End Sub

    Protected Sub btnViewOrder_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnViewOrder.Click
        If Not IsValid Then Exit Sub

        Dim ViewOrderId As Integer = DB.ExecuteScalar("SELECT TOP 1 OrderId FROM StoreOrder WHERE ProcessDate IS NOT NULL AND BillingZip=" & DB.Quote(txtBillingZip.Text) & " AND OrderNo=" & DB.Quote(txtOrderNo.Text))
        If ViewOrderId > 0 Then
            Session("ViewOrderId") = ViewOrderId
            Response.Redirect("/members/orders/view.aspx")
        Else
            AddError("Sorry - the details you provided for your order history do not match our records.  Please try again or <a href='/contactus/'>contact us</a> if you still have questions about your order.")
        End If
    End Sub

    Protected Sub btnLogin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLogin.Click
        If Not IsValid Then Exit Sub

        If MemberRow.PerformMemberLogin(DB, txtUsername.Text, txtPassword.Text, chkPersist.Checked) Then
            If Not Request("redir") = String.Empty Then
                Response.Redirect(Request("redir"))
            Else
                Response.Redirect("/members/")
            End If
        Else
            AddError("The password you entered does not match the one for this account. Please try again, or go to the <a href='/members/forgot.aspx'>forgot your password</a> page to retrieve it")
        End If
    End Sub
End Class