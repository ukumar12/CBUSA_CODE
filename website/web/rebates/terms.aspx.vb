Imports Components
Imports DataLayer
Imports System.Linq
Imports System.Data

Partial Class rebates_terms
    Inherits SitePage

    Private dbTerm As RebateTermRow
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'EnsureVendorAccess()
        If Session("VendorId") Is Nothing Or Session("VendorAccountId") Is Nothing Then
            Response.Redirect("/default.aspx")
        End If

        dbTerm = RebateTermRow.GetRowByVendor(DB, Session("VendorId"), Now.Year, Math.Ceiling(Now.Month / 3))

        If dbTerm.RebateTermsID = 0 Then
            Dim LLCRebate As Double = DB.ExecuteScalar("select DefaultRebate from LLC where LLCID = (select LLCId from LLCVendor where VendorId = " & DB.Number(Session("VendorId")) & ")")

            dbTerm = New RebateTermRow(DB)
            dbTerm.PurchaseRangeCeiling = 999999999
            dbTerm.PurchaseRangeFloor = 0
            If LLCRebate <> Nothing Then
                dbTerm.RebatePercentage = LLCRebate
            Else
                dbTerm.RebatePercentage = 1
            End If
            dbTerm.StartQuarter = Math.Ceiling(Now.Month / 3)
            dbTerm.StartYear = Now.Year
            dbTerm.IsAnnualPurchaseRange = False
            dbTerm.VendorID = Session("VendorId")
            dbTerm.CreatorVendorAccountID = Session("VendorAccountId")
            dbTerm.Insert()
        End If


        Dim dbRegistration As VendorRegistrationRow = VendorRegistrationRow.GetRowByVendor(DB, Session("VendorId"))
        If dbRegistration.CompleteDate <> Nothing Then
            ctlSteps.Visible = False
        End If
        ctlErrors.Visible = False

        PageURL = Request.Url.ToString()
        CurrentUserId = Session("VendorId")
        UserName = Session("Username")

        If Not IsPostBack Then

         Core.DataLog("Edit Rebate Terms", PageURL, CurrentUserId, "Vendor Left Menu Click", "", "", "", "", UserName)

            If Request("guid") IsNot Nothing Then
                btnContinue.Text = "Continue"
            Else
                btnContinue.Text = "Return to Dashboard"
            End If
            ltlCurrentQuarter.Text = "Q" & dbTerm.StartQuarter & " " & dbTerm.StartYear
            ltlCurrentRebate.Text = dbTerm.RebatePercentage
            txtNewRebate.Text = dbTerm.RebatePercentage
            ltlLogMsg.Text = dbTerm.LogMsg

            Dim LastYear As Integer
            Dim LastQuarter As Integer

            If Math.Ceiling(Now.Month / 3) = 1 Then
                LastYear = Now.Year - 1
                LastQuarter = 4
            Else
                LastYear = Now.Year
                LastQuarter = Math.Ceiling(Now.Month / 3) - 1
            End If

            Dim dbLastTerm As RebateTermRow = RebateTermRow.GetRowByVendor(DB, Session("VendorId"), LastYear, LastQuarter)
            If dbLastTerm.RebateTermsID > 0 Then
                ltlLastQuarter.Text = "Q" & dbLastTerm.StartQuarter & " " & dbLastTerm.StartYear
                ltlLastRebate.Text = dbLastTerm.RebatePercentage & "%"
                ltlLastLogMsg.Text = dbLastTerm.LogMsg
            Else
                ltlLastLogMsg.Text = "Not Available."
            End If

        End If

    End Sub

    Protected Sub btnContinue_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnContinue.Click
        Dim dbRegistration As VendorRegistrationRow = VendorRegistrationRow.GetRowByVendor(DB, Session("VendorId"))
        If dbRegistration.CompleteDate = Nothing Then
            Dim dbStat As RegistrationStatusRow = RegistrationStatusRow.GetStatusByStep(DB, 4)
            dbRegistration.RegistrationStatusID = dbStat.RegistrationStatusID
            dbRegistration.Update()
            Response.Redirect("/forms/vendor-registration/supplyphase.aspx?guid=" & Request("guid"))
        Else
            'log Btn Return to Dashboard Clicked
            Core.DataLog("Edit Rebate Terms", PageURL, CurrentUserId, "Btn Return to Dashboard", "", "", "", "", UserName)
            'end log
            Response.Redirect("/vendor/default.aspx")
        End If
    End Sub

    Protected Sub btnUpdateRebate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateRebate.Click
        Dim Rebate As Double = 0
        Try
            Rebate = txtNewRebate.Text
        Catch ex As Exception

        End Try
        If Rebate < 1 Or Rebate > 100 Or Rebate = dbTerm.RebatePercentage Then
            ltlMsg.Text = "<span class='errorSms'>Please enter a valid decimal value for New Rebate Percentage.</span>"
            Exit Sub
        ElseIf Rebate < dbTerm.RebatePercentage Then
            ltlMsg.Text = "<span class='errorSms'>You cannot lower you current rebate percentage.  Please contact us if you would like to do so.</span>"
            Exit Sub
        End If

        dbTerm.LogMsg = Now() & ": value updated from " & dbTerm.RebatePercentage & " to " & Rebate & " by vendor account user: " & VendorAccountRow.GetRow(DB, Session("VendorAccountId")).FirstName & " " & VendorAccountRow.GetRow(DB, Session("VendorAccountId")).LastName & "<br>" & dbTerm.LogMsg
        dbTerm.RebatePercentage = Rebate
        dbTerm.Update()
        'log Update Rebate Percentage
        Core.DataLog("Edit Rebate Terms", PageURL, CurrentUserId, "Update Rebate Percentage", "", "", "", "", UserName)
        'end log
        ltlCurrentRebate.Text = dbTerm.RebatePercentage
        txtNewRebate.Text = dbTerm.RebatePercentage
        ltlLogMsg.Text = dbTerm.LogMsg

        ltlMsg.Text = "<span class='succesSms'>Your New Rebate Percentage has been updated with success and will take effect immediately.</span>"

        Try
            Dim dbVendor As VendorRow = VendorRow.GetRow(DB, Session("VendorId"))
            Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "RebateTermsChanged")
            Dim msg As String = dbVendor.CompanyName & " has changed or deleted rebate terms.  Link to Admin: " & GlobalSecureName & "/admin/login.aspx"
            dbMsg.SendAdmin(msg)

            Dim LLCID As Integer = dbVendor.GetSelectedLLCs.Split(",")(0)
            Dim dtBuilders As DataTable = BuilderRow.GetListByLLC(DB, LLCID)
            dbMsg = AutomaticMessagesRow.GetRowByTitle(DB, "IncreasedRebate")
            For Each row As DataRow In dtBuilders.Rows
                Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, row("BuilderId"))
                dbMsg.Send(dbBuilder, "Vendor: " & vbTab & dbVendor.CompanyName & vbCrLf & "New Percentage: " & vbTab & dbTerm.RebatePercentage & "%")
            Next
        Catch ex As Exception

        End Try

    End Sub

    'Protected Sub btnGoToDashBoard_Click(sender As Object, e As System.EventArgs) Handles btnGoToDashBoard.Click
        'Response.Redirect("/vendor/default.aspx")
    'End Sub
End Class
