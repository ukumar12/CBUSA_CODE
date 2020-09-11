Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

    Protected RebateTermsID As Integer
    Private dbTerm As RebateTermRow

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("VENDOR_ACCOUNTS")

        RebateTermsID = Convert.ToInt32(Request("RebateTermsID"))
        dbTerm = RebateTermRow.GetRow(DB, RebateTermsID)

        If RebateTermsID = 0 Or dbTerm.RebateTermsID = 0 Then
            Response.Redirect("/admin/rebateterm/")
        End If

        If Not IsPostBack Then
            ltlCurrentQuarter.Text = "Q" & dbTerm.StartQuarter & " " & dbTerm.StartYear
            ltlCurrentRebate.Text = dbTerm.RebatePercentage
            txtNewRebate.Text = dbTerm.RebatePercentage
            ltlLogMsg.Text = dbTerm.LogMsg
        End If
	End Sub

    Protected Sub btnUpdateRebate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateRebate.Click
        Dim Rebate As Double = 0
        Try
            Rebate = txtNewRebate.Text
        Catch ex As Exception

        End Try
        If Rebate < 1 Or Rebate > 100 Or Rebate = dbTerm.RebatePercentage Then
            ltlMsg.Text = "<span class=""bold red"">Please enter a valid decimal value for New Rebate Percentage.</span>"
            Exit Sub
        End If

        dbTerm.LogMsg = Now() & ": value updated from " & dbTerm.RebatePercentage & " to " & Rebate & " by admin user: " & AdminRow.GetRow(DB, LoggedInAdminId).FirstName & " " & AdminRow.GetRow(DB, LoggedInAdminId).LastName & "<br>" & dbTerm.LogMsg
        dbTerm.RebatePercentage = Rebate
        dbTerm.Update()

        ltlCurrentRebate.Text = dbTerm.RebatePercentage
        txtNewRebate.Text = dbTerm.RebatePercentage
        ltlLogMsg.Text = dbTerm.LogMsg

        ltlMsg.Text = "<span class=""bold green"">New Rebate Percentage has been updated with success and will take effect immediately.</span>"

    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

	
End Class
