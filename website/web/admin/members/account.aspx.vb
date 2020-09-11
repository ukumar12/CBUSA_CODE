Imports Components
Imports DataLayer
Imports System.Data.SqlClient
Partial Class admin_members_account
    Inherits AdminPage

    Protected MemberId As Integer

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("MEMBERS")
        MemberId = Convert.ToInt32(Request("MemberId"))

        If Not IsPostBack Then
            Dim dbMember As MemberRow = MemberRow.GetRow(DB, MemberId)
            Dim dbBilling As MemberAddressRow = MemberAddressRow.GetDefaultBillingRow(DB, MemberId)
            txtMemberName.Text = "<b>" + Core.BuildFullName(dbBilling.FirstName, dbBilling.MiddleInitial, dbBilling.LastName) + " (" + dbMember.Username + ")</b>"
            txtUsername.Text = dbMember.Username
            txtBillingEmail.Text = dbBilling.Email
			chkIsActive.Checked = dbMember.IsActive
			lnkBack.HRef = "/admin/members/view.aspx?MemberId=" & MemberId & "&" & GetPageParams(FilterFieldType.All)
        End If
    End Sub

    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbMember As MemberRow = MemberRow.GetRow(DB, MemberId)
            Dim dbBilling As MemberAddressRow = MemberAddressRow.GetDefaultBillingRow(DB, MemberId)

            If Not txtUsername.Text = dbMember.Username Then
                Core.LogEvent("Username for member """ & dbMember.Username & """ was modified by user """ & Session("Username") & """", Diagnostics.EventLogEntryType.Information)
            End If

            dbMember.Username = txtUsername.Text
            If Not txtPassword.Text = String.Empty Then
                dbMember.Password = txtPassword.Text
                Core.LogEvent("Password for member """ & dbMember.Username & """ was modified by user """ & Session("Username") & """", Diagnostics.EventLogEntryType.Information)
            End If
			dbMember.IsActive = chkIsActive.Checked

            dbMember.ModifyDate = Date.Now
            dbMember.Update()

            dbBilling.Email = txtBillingEmail.Text
            dbBilling.Update()

            DB.CommitTransaction()

            Response.Redirect("view.aspx?Memberid=" & MemberId & "&" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
            'AddError(ex.Message)
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("/admin/members/view.aspx?MemberId=" & MemberId & "&" & GetPageParams(FilterFieldType.All))
    End Sub

  

End Class
