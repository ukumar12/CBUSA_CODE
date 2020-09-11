Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

    Protected VendorRegistrationFinancialReferenceID As Integer
    Protected VendorRegistrationID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("VENDOR_REGISTRATIONS")

        VendorRegistrationFinancialReferenceID = Convert.ToInt32(Request("VendorRegistrationFinancialReferenceID"))

        VendorRegistrationID = Convert.ToInt32(Request("VendorRegistrationID"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
        drpState.DataSource = StateRow.GetStateList(DB)
        drpState.DataValueField = "StateCode"
		drpState.DataTextField = "StateCode"
		drpState.Databind
		drpState.Items.Insert(0, New ListItem("",""))
	
		If VendorRegistrationFinancialReferenceID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbVendorRegistrationFinancialReference As VendorRegistrationFinancialReferenceRow = VendorRegistrationFinancialReferenceRow.GetRow(DB, VendorRegistrationFinancialReferenceID)
        Me.VendorRegistrationID = dbVendorRegistrationFinancialReference.VendorRegistrationID
        txtFirstName.Text = dbVendorRegistrationFinancialReference.FirstName
		txtLastName.Text = dbVendorRegistrationFinancialReference.LastName
		txtCompanyName.Text = dbVendorRegistrationFinancialReference.CompanyName
		txtCity.Text = dbVendorRegistrationFinancialReference.City
		txtZip.Text = dbVendorRegistrationFinancialReference.Zip
		txtPhone.Text = dbVendorRegistrationFinancialReference.Phone
		drpState.SelectedValue = dbVendorRegistrationFinancialReference.State
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbVendorRegistrationFinancialReference As VendorRegistrationFinancialReferenceRow

			If VendorRegistrationFinancialReferenceID <> 0 Then
				dbVendorRegistrationFinancialReference = VendorRegistrationFinancialReferenceRow.GetRow(DB, VendorRegistrationFinancialReferenceID)
			Else
                dbVendorRegistrationFinancialReference = New VendorRegistrationFinancialReferenceRow(DB)
                dbVendorRegistrationFinancialReference.VendorRegistrationID = Me.VendorRegistrationID
			End If
			dbVendorRegistrationFinancialReference.FirstName = txtFirstName.Text
			dbVendorRegistrationFinancialReference.LastName = txtLastName.Text
			dbVendorRegistrationFinancialReference.CompanyName = txtCompanyName.Text
			dbVendorRegistrationFinancialReference.City = txtCity.Text
			dbVendorRegistrationFinancialReference.Zip = txtZip.Text
			dbVendorRegistrationFinancialReference.Phone = txtPhone.Text
			dbVendorRegistrationFinancialReference.State = drpState.SelectedValue
	
			If VendorRegistrationFinancialReferenceID <> 0 Then
				dbVendorRegistrationFinancialReference.Update()
			Else
				VendorRegistrationFinancialReferenceID = dbVendorRegistrationFinancialReference.Insert
			End If
	
			DB.CommitTransaction()

	
            Response.Redirect("default.aspx?VendorRegistrationId=" & dbVendorRegistrationFinancialReference.VendorRegistrationID & "&" & GetPageParams(FilterFieldType.All))

		Catch ex As SqlException
			If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
		End Try
	End Sub

	Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?VendorRegistrationId=" & VendorRegistrationID & "&" & GetPageParams(FilterFieldType.All))
	End Sub

	Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
		Response.Redirect("delete.aspx?VendorRegistrationFinancialReferenceID=" & VendorRegistrationFinancialReferenceID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
