Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected VendorBranchOfficeID As Integer
    Protected VendorId As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("VENDORS")

		VendorBranchOfficeID = Convert.ToInt32(Request("VendorBranchOfficeID"))
        VendorId = Convert.ToInt32(Request("VendorId"))

		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		drpState.Datasource = StateRow.GetStateList(DB)
		drpState.DataValueField = "StateCode"
		drpState.DataTextField = "StateCode"
		drpState.Databind
		drpState.Items.Insert(0, New ListItem("",""))
	
		If VendorBranchOfficeID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbVendorBranchOffice As VendorBranchOfficeRow = VendorBranchOfficeRow.GetRow(DB, VendorBranchOfficeID)
		txtAddress.Text = dbVendorBranchOffice.Address
		txtAddress2.Text = dbVendorBranchOffice.Address2
		txtCity.Text = dbVendorBranchOffice.City
		txtZip.Text = dbVendorBranchOffice.Zip
		drpState.SelectedValue = dbVendorBranchOffice.State
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbVendorBranchOffice As VendorBranchOfficeRow

			If VendorBranchOfficeID <> 0 Then
				dbVendorBranchOffice = VendorBranchOfficeRow.GetRow(DB, VendorBranchOfficeID)
			Else
				dbVendorBranchOffice = New VendorBranchOfficeRow(DB)
			    dbVendorBranchOffice.VendorID = VendorID
			End If
			dbVendorBranchOffice.Address = txtAddress.Text
			dbVendorBranchOffice.Address2 = txtAddress2.Text
			dbVendorBranchOffice.City = txtCity.Text
			dbVendorBranchOffice.Zip = txtZip.Text
			dbVendorBranchOffice.State = drpState.SelectedValue
	
			If VendorBranchOfficeID <> 0 Then
				dbVendorBranchOffice.Update()
			Else
				VendorBranchOfficeID = dbVendorBranchOffice.Insert
			End If
	
			DB.CommitTransaction()

	
			Response.Redirect("default.aspx?VendorId=" & dbVendorBranchOffice.VendorID & "&" & GetPageParams(FilterFieldType.All))

		Catch ex As SqlException
			If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
		End Try
	End Sub

	Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
		Response.Redirect("default.aspx?VendorId=" & VendorID & "&" & GetPageParams(FilterFieldType.All))
	End Sub

	Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
		Response.Redirect("delete.aspx?VendorBranchOfficeID=" & VendorBranchOfficeID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
