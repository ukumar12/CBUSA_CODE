Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

    Protected VendorRegistrationMemberReferenceID As Integer
    Protected VendorRegistrationID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("VENDOR_REGISTRATIONS")

        VendorRegistrationMemberReferenceID = Convert.ToInt32(Request("VendorRegistrationMemberReferenceID"))

        VendorRegistrationID = Convert.ToInt32(Request("VendorRegistrationID"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		If VendorRegistrationMemberReferenceID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbVendorRegistrationMemberReference As VendorRegistrationMemberReferenceRow = VendorRegistrationMemberReferenceRow.GetRow(DB, VendorRegistrationMemberReferenceID)
        Me.VendorRegistrationID = dbVendorRegistrationMemberReference.VendorRegistrationID
        txtFirstName.Text = dbVendorRegistrationMemberReference.FirstName
		txtLastName.Text = dbVendorRegistrationMemberReference.LastName
		txtCompanyName.Text = dbVendorRegistrationMemberReference.CompanyName
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbVendorRegistrationMemberReference As VendorRegistrationMemberReferenceRow

			If VendorRegistrationMemberReferenceID <> 0 Then
				dbVendorRegistrationMemberReference = VendorRegistrationMemberReferenceRow.GetRow(DB, VendorRegistrationMemberReferenceID)
			Else
                dbVendorRegistrationMemberReference = New VendorRegistrationMemberReferenceRow(DB)
                dbVendorRegistrationMemberReference.VendorRegistrationID = Me.vendorRegistrationId
			End If
			dbVendorRegistrationMemberReference.FirstName = txtFirstName.Text
			dbVendorRegistrationMemberReference.LastName = txtLastName.Text
			dbVendorRegistrationMemberReference.CompanyName = txtCompanyName.Text
	
			If VendorRegistrationMemberReferenceID <> 0 Then
				dbVendorRegistrationMemberReference.Update()
			Else
				VendorRegistrationMemberReferenceID = dbVendorRegistrationMemberReference.Insert
			End If
	
			DB.CommitTransaction()

	
            Response.Redirect("default.aspx?VendorRegistrationId=" & dbVendorRegistrationMemberReference.VendorRegistrationID & "&" & GetPageParams(FilterFieldType.All))

		Catch ex As SqlException
			If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
		End Try
	End Sub

	Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?VendorRegistrationId=" & VendorRegistrationID & "&" & GetPageParams(FilterFieldType.All))
	End Sub

	Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
		Response.Redirect("delete.aspx?VendorRegistrationMemberReferenceID=" & VendorRegistrationMemberReferenceID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
