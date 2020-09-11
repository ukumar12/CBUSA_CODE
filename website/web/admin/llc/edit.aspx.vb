Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected LLCID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("LLCs")

        LLCID = Convert.ToInt32(Request("LLCID"))
        If Not IsPostBack Then
            F_RegBillingPlan.DataSource = BillingPlanRow.GetActiveBillingPlans(DB, BILLING_PLAN_TYPE.REGISTRATION, "SortValue")
            F_RegBillingPlan.DataValueField = "BillingPlanId"
            F_RegBillingPlan.DataTextField = "DisplayValue"
            F_RegBillingPlan.DataBind()
            F_RegBillingPlan.Items.Insert(0, New ListItem("-- SELECT --", ""))

            F_SubBillingPlan.DataSource = BillingPlanRow.GetActiveBillingPlans(DB, BILLING_PLAN_TYPE.SUBSCRIPTION, "SortValue")
            F_SubBillingPlan.DataValueField = "BillingPlanId"
            F_SubBillingPlan.DataTextField = "DisplayValue"
            F_SubBillingPlan.DataBind()
            F_SubBillingPlan.Items.Insert(0, New ListItem("-- SELECT --", ""))

            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If LLCID = 0 Then
            Dim affliateID As Integer = DB.ExecuteScalar("SELECT MAX(AffiliateID) +1 FROM LLC")
            txtAffliateID.Text = affliateID.ToString("D3")
            btnDelete.Visible = False
            txtAffliateID.Enabled = False

            dtpBillingStartDate.Value = Now.Date        'Set current date as default Billing Start Date

            Exit Sub
        End If

        Dim dbLLC As LLCRow = LLCRow.GetRow(DB, LLCID)
        txtLLC.Text = dbLLC.LLC
        txtDescription.Text = dbLLC.Description
        txtDiscrepencyTolerance.Text = dbLLC.DiscrepencyTolerance
        rblIsActive.SelectedValue = dbLLC.IsActive
        rblExcludeFromReporting.SelectedValue = dbLLC.ExcludeFromReports
        txtDefaultRebate.Text = dbLLC.DefaultRebate
        txtBuilderGroup.Text = dbLLC.BuilderGroup
        txtOperationsManager.Text = dbLLC.OperationsManager
        txtEmailNotification.Text = dbLLC.NotificationEmailList
        rblAllowExcludingVendors.SelectedValue = dbLLC.AllowExcludingVendors
        txtAffliateID.Text = dbLLC.AffiliateID.ToString("D3")
        rblTakeOffserivce.SelectedValue = dbLLC.EnableTakeOffService

        F_RegBillingPlan.SelectedValue = dbLLC.RegBillingPlanId
        F_SubBillingPlan.SelectedValue = dbLLC.SubBillingPlanId

        dtpBillingStartDate.Value = dbLLC.BillingStartDate

    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbLLC As LLCRow

            If LLCID <> 0 Then
                dbLLC = LLCRow.GetRow(DB, LLCID)
            Else
                dbLLC = New LLCRow(DB)
            End If
            dbLLC.LLC = txtLLC.Text
            dbLLC.Description = txtDescription.Text
            dbLLC.DiscrepencyTolerance = txtDiscrepencyTolerance.Text
            dbLLC.IsActive = rblIsActive.SelectedValue
            dbLLC.ExcludeFromReports = rblExcludeFromReporting.SelectedValue
            dbLLC.BuilderGroup = txtBuilderGroup.Text
            dbLLC.OperationsManager = txtOperationsManager.Text
            dbLLC.NotificationEmailList = txtEmailNotification.Text
            dbLLC.AffiliateID = txtAffliateID.Text
            dbLLC.EnableTakeOffService = rblTakeOffserivce.SelectedValue
            If txtDefaultRebate.Text = Nothing Then
                dbLLC.DefaultRebate = 0
            Else
                dbLLC.DefaultRebate = txtDefaultRebate.Text
            End If
            dbLLC.AllowExcludingVendors = rblAllowExcludingVendors.SelectedValue

            If F_RegBillingPlan.SelectedValue = "" Then
                dbLLC.RegBillingPlanId = BillingPlanRow.GetDefaultBillingPlan(DB, BILLING_PLAN_TYPE.REGISTRATION).BillingPlanId
            Else
                dbLLC.RegBillingPlanId = CInt(F_RegBillingPlan.SelectedValue)
            End If

            If F_SubBillingPlan.SelectedValue = "" Then
                dbLLC.SubBillingPlanId = BillingPlanRow.GetDefaultBillingPlan(DB, BILLING_PLAN_TYPE.SUBSCRIPTION).BillingPlanId
            Else
                dbLLC.SubBillingPlanId = CInt(F_SubBillingPlan.SelectedValue)
            End If

            If dtpBillingStartDate.Value.Date = New Date(1, 1, 1) Then
                dbLLC.BillingStartDate = Now.Date
            Else
                dbLLC.BillingStartDate = dtpBillingStartDate.Value
            End If

            If LLCID <> 0 Then
                dbLLC.Update()
            Else
                LLCID = dbLLC.Insert
            End If

            DB.CommitTransaction()

            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("delete.aspx?LLCID=" & LLCID & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class

