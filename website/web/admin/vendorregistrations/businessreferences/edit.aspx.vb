Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

    Protected VendorRegistrationBusinessReferenceID As Integer
    Protected VendorRegistrationID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("VENDOR_REGISTRATIONS")

        VendorRegistrationBusinessReferenceID = Convert.ToInt32(Request("VendorRegistrationBusinessReferenceID"))

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
	
		If VendorRegistrationBusinessReferenceID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbVendorRegistrationBusinessReference As VendorRegistrationBusinessReferenceRow = VendorRegistrationBusinessReferenceRow.GetRow(DB, VendorRegistrationBusinessReferenceID)
        Me.VendorRegistrationID = dbVendorRegistrationBusinessReference.VendorRegistrationID
        txtFirstName.Text = dbVendorRegistrationBusinessReference.FirstName
		txtLastName.Text = dbVendorRegistrationBusinessReference.LastName
		txtCompanyName.Text = dbVendorRegistrationBusinessReference.CompanyName
		txtCity.Text = dbVendorRegistrationBusinessReference.City
		txtZip.Text = dbVendorRegistrationBusinessReference.Zip
		txtPhone.Text = dbVendorRegistrationBusinessReference.Phone
		drpState.SelectedValue = dbVendorRegistrationBusinessReference.State
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbVendorRegistrationBusinessReference As VendorRegistrationBusinessReferenceRow

			If VendorRegistrationBusinessReferenceID <> 0 Then
				dbVendorRegistrationBusinessReference = VendorRegistrationBusinessReferenceRow.GetRow(DB, VendorRegistrationBusinessReferenceID)
			Else
                dbVendorRegistrationBusinessReference = New VendorRegistrationBusinessReferenceRow(DB)
                dbVendorRegistrationBusinessReference.VendorRegistrationID = Me.VendorRegistrationID
			End If
			dbVendorRegistrationBusinessReference.FirstName = txtFirstName.Text
			dbVendorRegistrationBusinessReference.LastName = txtLastName.Text
			dbVendorRegistrationBusinessReference.CompanyName = txtCompanyName.Text
			dbVendorRegistrationBusinessReference.City = txtCity.Text
			dbVendorRegistrationBusinessReference.Zip = txtZip.Text
			dbVendorRegistrationBusinessReference.Phone = txtPhone.Text
			dbVendorRegistrationBusinessReference.State = drpState.SelectedValue
	
			If VendorRegistrationBusinessReferenceID <> 0 Then
				dbVendorRegistrationBusinessReference.Update()
			Else
				VendorRegistrationBusinessReferenceID = dbVendorRegistrationBusinessReference.Insert
			End If
	
			DB.CommitTransaction()

	
            Response.Redirect("default.aspx?VendorRegistrationId=" & dbVendorRegistrationBusinessReference.VendorRegistrationID & "&" & GetPageParams(FilterFieldType.All))

		Catch ex As SqlException
			If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
		End Try
	End Sub

	Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?VendorRegistrationId=" & VendorRegistrationID & "&" & GetPageParams(FilterFieldType.All))
	End Sub

	Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
		Response.Redirect("delete.aspx?VendorRegistrationBusinessReferenceID=" & VendorRegistrationBusinessReferenceID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
