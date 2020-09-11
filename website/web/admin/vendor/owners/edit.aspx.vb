Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected VendorOwnerID As Integer
    Protected VendorId As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("VENDORS")

		VendorOwnerID = Convert.ToInt32(Request("VendorOwnerID"))
        VendorId = Convert.ToInt32(Request("VendorId"))

		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		If VendorOwnerID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbVendorOwners As VendorOwnersRow = VendorOwnersRow.GetRow(DB, VendorOwnerID)
		txtFirstName.Text = dbVendorOwners.FirstName
		txtLastName.Text = dbVendorOwners.LastName
		txtPhone.Text = dbVendorOwners.Phone
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbVendorOwners As VendorOwnersRow

			If VendorOwnerID <> 0 Then
				dbVendorOwners = VendorOwnersRow.GetRow(DB, VendorOwnerID)
			Else
				dbVendorOwners = New VendorOwnersRow(DB)
			dbVendorOwners.VendorID = VendorId
			End If
			dbVendorOwners.FirstName = txtFirstName.Text
			dbVendorOwners.LastName = txtLastName.Text
			dbVendorOwners.Phone = txtPhone.Text
	
			If VendorOwnerID <> 0 Then
				dbVendorOwners.Update()
			Else
				VendorOwnerID = dbVendorOwners.Insert
			End If
	
			DB.CommitTransaction()

	
			Response.Redirect("default.aspx?VendorId=" & dbVendorOwners.VendorID & "&" & GetPageParams(FilterFieldType.All))

		Catch ex As SqlException
			If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
		End Try
	End Sub

	Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
		Response.Redirect("default.aspx?VendorId=" & VendorID & "&" & GetPageParams(FilterFieldType.All))
	End Sub

	Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
		Response.Redirect("delete.aspx?VendorOwnerID=" & VendorOwnerID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
