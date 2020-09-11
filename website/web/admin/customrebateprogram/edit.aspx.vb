Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected CustomRebateProgramID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("CUSTOM_REBATE_PROGRAMS")

		CustomRebateProgramID = Convert.ToInt32(Request("CustomRebateProgramID"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		drpVendorID.Datasource = VendorRow.GetList(DB,"CompanyName")
		drpVendorID.DataValueField = "VendorID"
		drpVendorID.DataTextField = "CompanyName"
		drpVendorID.Databind
		drpVendorID.Items.Insert(0, New ListItem("",""))
	
		If CustomRebateProgramID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbCustomRebateProgram As CustomRebateProgramRow = CustomRebateProgramRow.GetRow(DB, CustomRebateProgramID)
		txtProgramName.Text = dbCustomRebateProgram.ProgramName
		txtRebateYear.Text = dbCustomRebateProgram.RebateYear
		txtRebateQuarter.Text = dbCustomRebateProgram.RebateQuarter
		txtMinimumPurchase.Text = dbCustomRebateProgram.MinimumPurchase
		txtRebatePercentage.Text = dbCustomRebateProgram.RebatePercentage
		txtDetails.Text = dbCustomRebateProgram.Details
		drpVendorID.SelectedValue = dbCustomRebateProgram.VendorID
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbCustomRebateProgram As CustomRebateProgramRow

			If CustomRebateProgramID <> 0 Then
				dbCustomRebateProgram = CustomRebateProgramRow.GetRow(DB, CustomRebateProgramID)
			Else
				dbCustomRebateProgram = New CustomRebateProgramRow(DB)
			End If
			dbCustomRebateProgram.ProgramName = txtProgramName.Text
			dbCustomRebateProgram.RebateYear = txtRebateYear.Text
			dbCustomRebateProgram.RebateQuarter = txtRebateQuarter.Text
			dbCustomRebateProgram.MinimumPurchase = txtMinimumPurchase.Text
			dbCustomRebateProgram.RebatePercentage = txtRebatePercentage.Text
			dbCustomRebateProgram.Details = txtDetails.Text
			dbCustomRebateProgram.VendorID = drpVendorID.SelectedValue
	
			If CustomRebateProgramID <> 0 Then
				dbCustomRebateProgram.Update()
			Else
				CustomRebateProgramID = dbCustomRebateProgram.Insert
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
		Response.Redirect("delete.aspx?CustomRebateProgramID=" & CustomRebateProgramID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
