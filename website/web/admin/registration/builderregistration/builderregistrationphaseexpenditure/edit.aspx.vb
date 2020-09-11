Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected BuilderRegistrationPhaseExpenditureID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("BUILDER_REGISTRATIONS")

		BuilderRegistrationPhaseExpenditureID = Convert.ToInt32(Request("BuilderRegistrationPhaseExpenditureID"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		drpSupplyPhaseID.Datasource = SupplyPhaseRow.GetList(DB)
		drpSupplyPhaseID.DataValueField = "SupplyPhaseID"
		drpSupplyPhaseID.DataTextField = "SupplyPhase"
		drpSupplyPhaseID.Databind
		drpSupplyPhaseID.Items.Insert(0, New ListItem("",""))
	
        'drpPreferredVendorID.Datasource = VendorRow.GetList(DB)
        'drpPreferredVendorID.DataValueField = "VendorID"
        'drpPreferredVendorID.DataTextField = "CompanyName"
        'drpPreferredVendorID.Databind
        'drpPreferredVendorID.Items.Insert(0, New ListItem("",""))
	
		If BuilderRegistrationPhaseExpenditureID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbBuilderRegistrationPhaseExpenditure As BuilderRegistrationPhaseExpenditureRow = BuilderRegistrationPhaseExpenditureRow.GetRow(DB, BuilderRegistrationPhaseExpenditureID)
		txtBuilderRegistrationID.Text = dbBuilderRegistrationPhaseExpenditure.BuilderRegistrationID
		txtAmountSpentLastYear.Text = dbBuilderRegistrationPhaseExpenditure.AmountSpentLastYear
		txtOtherPreferredVendor.Text = dbBuilderRegistrationPhaseExpenditure.OtherPreferredVendor
		drpSupplyPhaseID.SelectedValue = dbBuilderRegistrationPhaseExpenditure.SupplyPhaseID
		drpPreferredVendorID.SelectedValue = dbBuilderRegistrationPhaseExpenditure.PreferredVendorID
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbBuilderRegistrationPhaseExpenditure As BuilderRegistrationPhaseExpenditureRow

			If BuilderRegistrationPhaseExpenditureID <> 0 Then
				dbBuilderRegistrationPhaseExpenditure = BuilderRegistrationPhaseExpenditureRow.GetRow(DB, BuilderRegistrationPhaseExpenditureID)
			Else
				dbBuilderRegistrationPhaseExpenditure = New BuilderRegistrationPhaseExpenditureRow(DB)
			End If
			dbBuilderRegistrationPhaseExpenditure.BuilderRegistrationID = txtBuilderRegistrationID.Text
			dbBuilderRegistrationPhaseExpenditure.AmountSpentLastYear = txtAmountSpentLastYear.Text
			dbBuilderRegistrationPhaseExpenditure.OtherPreferredVendor = txtOtherPreferredVendor.Text
			dbBuilderRegistrationPhaseExpenditure.SupplyPhaseID = drpSupplyPhaseID.SelectedValue
			dbBuilderRegistrationPhaseExpenditure.PreferredVendorID = drpPreferredVendorID.SelectedValue
	
			If BuilderRegistrationPhaseExpenditureID <> 0 Then
				dbBuilderRegistrationPhaseExpenditure.Update()
			Else
				BuilderRegistrationPhaseExpenditureID = dbBuilderRegistrationPhaseExpenditure.Insert
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
		Response.Redirect("delete.aspx?BuilderRegistrationPhaseExpenditureID=" & BuilderRegistrationPhaseExpenditureID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
