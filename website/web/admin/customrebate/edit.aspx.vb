Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected CustomRebateID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("CUSTOM_REBATES")

        CustomRebateID = Convert.ToInt32(Request("CustomRebateID"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        drpVendorID.Datasource = VendorRow.GetList(DB, "CompanyName")
        drpVendorID.DataValueField = "VendorID"
        drpVendorID.DataTextField = "CompanyName"
        drpVendorID.Databind()
        drpVendorID.Items.Insert(0, New ListItem("", ""))

        drpBuilderID.Datasource = BuilderRow.GetList(DB, "CompanyName")
        drpBuilderID.DataValueField = "BuilderID"
        drpBuilderID.DataTextField = "CompanyName"
        drpBuilderID.Databind()
        drpBuilderID.Items.Insert(0, New ListItem("", ""))

        drpCustomRebateProgramID.Datasource = CustomRebateProgramRow.GetList(DB, "ProgramName")
        drpCustomRebateProgramID.DataValueField = "CustomRebateProgramID"
        drpCustomRebateProgramID.DataTextField = "ProgramName"
        drpCustomRebateProgramID.Databind()
        drpCustomRebateProgramID.Items.Insert(0, New ListItem("", ""))

        If CustomRebateID = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbCustomRebate As CustomRebateRow = CustomRebateRow.GetRow(DB, CustomRebateID)
        txtRebateYear.Text = dbCustomRebate.RebateYear
        txtRebateQuarter.Text = dbCustomRebate.RebateQuarter
        txtMinimumPurchase.Text = dbCustomRebate.MinimumPurchase
        txtRebatePercentage.Text = dbCustomRebate.RebatePercentage
        txtApplicablePurchaseAmount.Text = dbCustomRebate.ApplicablePurchaseAmount
        txtRebateAmount.Text = dbCustomRebate.RebateAmount
        txtDetails.Text = dbCustomRebate.Details
        drpVendorID.SelectedValue = dbCustomRebate.VendorID
        drpBuilderID.SelectedValue = dbCustomRebate.BuilderID
        drpCustomRebateProgramID.SelectedValue = dbCustomRebate.CustomRebateProgramID
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbCustomRebate As CustomRebateRow

            If CustomRebateID <> 0 Then
                dbCustomRebate = CustomRebateRow.GetRow(DB, CustomRebateID)
            Else
                dbCustomRebate = New CustomRebateRow(DB)
            End If
            dbCustomRebate.RebateYear = txtRebateYear.Text
            dbCustomRebate.RebateQuarter = txtRebateQuarter.Text
            dbCustomRebate.MinimumPurchase = txtMinimumPurchase.Text
            dbCustomRebate.RebatePercentage = txtRebatePercentage.Text
            dbCustomRebate.ApplicablePurchaseAmount = txtApplicablePurchaseAmount.Text
            dbCustomRebate.RebateAmount = txtRebateAmount.Text
            dbCustomRebate.Details = txtDetails.Text
            dbCustomRebate.VendorID = drpVendorID.SelectedValue
            dbCustomRebate.BuilderID = drpBuilderID.SelectedValue
            dbCustomRebate.CustomRebateProgramID = drpCustomRebateProgramID.SelectedValue

            If CustomRebateID <> 0 Then
                dbCustomRebate.Update()
            Else
                CustomRebateID = dbCustomRebate.Insert
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
        Response.Redirect("delete.aspx?CustomRebateID=" & CustomRebateID & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class

