Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

    Protected VendorRegistrationCustomerReferenceID As Integer
    Protected VendorRegistrationID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("VENDOR_REGISTRATIONS")

        VendorRegistrationCustomerReferenceID = Convert.ToInt32(Request("VendorRegistrationCustomerReferenceID"))

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
	
		If VendorRegistrationCustomerReferenceID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

        Dim dbVendorRegistrationCustomerReference As VendorRegistrationCustomerReferenceRow = VendorRegistrationCustomerReferenceRow.GetRow(DB, VendorRegistrationCustomerReferenceID)
        Me.VendorRegistrationID = dbVendorRegistrationCustomerReference.VendorRegistrationID
		txtFirstName.Text = dbVendorRegistrationCustomerReference.FirstName
		txtLastName.Text = dbVendorRegistrationCustomerReference.LastName
		txtCity.Text = dbVendorRegistrationCustomerReference.City
		txtZip.Text = dbVendorRegistrationCustomerReference.Zip
		txtPhone.Text = dbVendorRegistrationCustomerReference.Phone
		drpState.SelectedValue = dbVendorRegistrationCustomerReference.State
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbVendorRegistrationCustomerReference As VendorRegistrationCustomerReferenceRow

			If VendorRegistrationCustomerReferenceID <> 0 Then
				dbVendorRegistrationCustomerReference = VendorRegistrationCustomerReferenceRow.GetRow(DB, VendorRegistrationCustomerReferenceID)
			Else
                dbVendorRegistrationCustomerReference = New VendorRegistrationCustomerReferenceRow(DB)
                dbVendorRegistrationCustomerReference.VendorRegistrationID = Me.VendorRegistrationID
			End If
			dbVendorRegistrationCustomerReference.FirstName = txtFirstName.Text
			dbVendorRegistrationCustomerReference.LastName = txtLastName.Text
			dbVendorRegistrationCustomerReference.City = txtCity.Text
			dbVendorRegistrationCustomerReference.Zip = txtZip.Text
			dbVendorRegistrationCustomerReference.Phone = txtPhone.Text
			dbVendorRegistrationCustomerReference.State = drpState.SelectedValue
	
			If VendorRegistrationCustomerReferenceID <> 0 Then
				dbVendorRegistrationCustomerReference.Update()
			Else
				VendorRegistrationCustomerReferenceID = dbVendorRegistrationCustomerReference.Insert
			End If
	
			DB.CommitTransaction()

	
            Response.Redirect("default.aspx?VendorRegistrationId=" & dbVendorRegistrationCustomerReference.VendorRegistrationID & "&" & GetPageParams(FilterFieldType.All))

		Catch ex As SqlException
			If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
		End Try
	End Sub

	Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?VendorRegistrationId=" & VendorRegistrationID & "&" & GetPageParams(FilterFieldType.All))
	End Sub

	Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
		Response.Redirect("delete.aspx?VendorRegistrationCustomerReferenceID=" & VendorRegistrationCustomerReferenceID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
