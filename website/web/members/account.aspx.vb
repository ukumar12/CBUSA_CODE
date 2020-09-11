Imports Components
Imports DataLayer
Imports System.Data.SqlClient

Public Class Members_Account
    Inherits SitePage

    Protected MemberId As Integer
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureSSL()
        EnsureMemberAccess()
        MemberId = Session("MemberId")
        If Not IsPostBack Then
            Dim dbMember As MemberRow = MemberRow.GetRow(DB, Session("MemberId"))
            Dim dbBilling As MemberAddressRow = MemberAddressRow.GetDefaultBillingRow(DB, Session("MemberId"))
            txtUsername.Text = dbMember.Username
            txtBillingEmail.Text = dbBilling.Email
        End If
    End Sub

    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbMember As MemberRow = MemberRow.GetRow(DB, Session("MemberId"))
            Dim dbBilling As MemberAddressRow = MemberAddressRow.GetDefaultBillingRow(DB, Session("MemberId"))

            dbMember.Username = txtUsername.Text
            If Not txtPassword.Text = String.Empty Then
                dbMember.Password = txtPassword.Text
            End If
            dbMember.Update()

            dbBilling.Email = txtBillingEmail.Text
            dbBilling.Update()

            DB.CommitTransaction()

            Response.Redirect("/members/")

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

End Class