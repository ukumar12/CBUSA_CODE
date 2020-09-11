Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Partial Class Edit
    Inherits AdminPage

    Protected BillingPlanId As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("BILLING_PLANS")

        BillingPlanId = Convert.ToInt32(Request("BillingPlanId"))

        If Not IsPostBack Then
            PopulateBillingPlans()

            If BillingPlanId > 0 Then
                LoadFromDB()
            End If
        End If

    End Sub

    Private Sub LoadFromDB()

        Dim dbBillingPlan As BillingPlanRow = BillingPlanRow.GetRow(DB, BillingPlanId)
        txtBillingPlan.Text = dbBillingPlan.DisplayValue
        txtSortValue.Text = dbBillingPlan.SortValue
        rblBillingPlanType.SelectedValue = dbBillingPlan.BillingPlanType
        txtDescription.Text = dbBillingPlan.Description
        txtExternalSystem.Text = dbBillingPlan.ExternalSystem

        rblIsDefault.SelectedValue = dbBillingPlan.IsDefault
        rblRecordState.SelectedValue = dbBillingPlan.RecordState

        If dbBillingPlan.IsDefault = True Then
            rblRecordState.Enabled = False
            rblBillingPlanType.Enabled = False

            btnDelete.Enabled = False
            btnDelete.Style.Add("display", "none")
        End If

        PopulateBillingPlans()
        ddlBillingPlans.SelectedValue = dbBillingPlan.ExternalKey

    End Sub

    Private Sub PopulateBillingPlans()

        Dim VinProcessor As VindiciaPaymentProcessor
        VinProcessor = New VindiciaPaymentProcessor(DB)
        VinProcessor.IsTestMode = SysParam.GetValue(DB, "TestMode")

        Dim VinBillingPlans As Vindicia.BillingPlan() = VinProcessor.GetBillingPlansForAdmin()

        ddlBillingPlans.Items.Clear()

        Dim i As Integer
        For i = 0 To VinBillingPlans.Length - 1
            If VinBillingPlans(i).nameValues Is Nothing Then Continue For

            If VinBillingPlans(i).status = Vindicia.BillingPlanStatus.Active AndAlso VinBillingPlans(i).merchantBillingPlanId.StartsWith("VIN-") Then

                If rblBillingPlanType.SelectedValue = BILLING_PLAN_TYPE.REGISTRATION Then
                    If VinBillingPlans(i).merchantBillingPlanId.Contains("-REG-") Then
                        Dim BillingPlan As New ListItem()
                        BillingPlan.Text = VinBillingPlans(i).merchantBillingPlanId
                        BillingPlan.Value = VinBillingPlans(i).merchantBillingPlanId

                        ddlBillingPlans.Items.Add(BillingPlan)
                    End If
                Else
                    If VinBillingPlans(i).merchantBillingPlanId.Contains("-SUB-") Then
                        Dim BillingPlan As New ListItem()
                        BillingPlan.Text = VinBillingPlans(i).merchantBillingPlanId
                        BillingPlan.Value = VinBillingPlans(i).merchantBillingPlanId

                        ddlBillingPlans.Items.Add(BillingPlan)
                    End If
                End If
            End If
        Next

    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub

        Try
            DB.BeginTransaction()

            Dim dbBillingPlan As BillingPlanRow

            If BillingPlanId <> 0 Then
                dbBillingPlan = BillingPlanRow.GetRow(DB, BillingPlanId)
            Else
                dbBillingPlan = New BillingPlanRow(DB)
            End If

            dbBillingPlan.DisplayValue = txtBillingPlan.Text
            dbBillingPlan.SortValue = txtSortValue.Text
            dbBillingPlan.BillingPlanType = rblBillingPlanType.SelectedValue
            dbBillingPlan.Description = txtDescription.Text
            dbBillingPlan.ExternalSystem = txtExternalSystem.Text
            dbBillingPlan.ExternalKey = ddlBillingPlans.Text
            dbBillingPlan.RecordState = rblRecordState.SelectedValue
            dbBillingPlan.IsDefault = rblIsDefault.SelectedValue

            If BillingPlanId <> 0 Then
                dbBillingPlan.ModifiedOn = Now.Date()
                dbBillingPlan.ModifiedBy = Convert.ToUInt32(Session("AdminId"))

                dbBillingPlan.Update()
            Else
                dbBillingPlan.CreatedOn = Now.Date()
                dbBillingPlan.CreatedBy = Convert.ToUInt32(Session("AdminId"))
                dbBillingPlan.ModifiedBy = Convert.ToUInt32(Session("AdminId"))
                dbBillingPlan.RowID = Guid.NewGuid()

                BillingPlanId = dbBillingPlan.Insert()
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
        Response.Redirect("delete.aspx?BillingPlanId=" & BillingPlanId & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Private Sub rblBillingPlanType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblBillingPlanType.SelectedIndexChanged
        PopulateBillingPlans()
    End Sub

End Class
