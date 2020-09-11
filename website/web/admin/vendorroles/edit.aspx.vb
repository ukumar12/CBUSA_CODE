Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected VendorRoleID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("VENDOR_ROLES")

		VendorRoleID = Convert.ToInt32(Request("VendorRoleID"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		If VendorRoleID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbVendorRole As VendorRoleRow = VendorRoleRow.GetRow(DB, VendorRoleID)
		txtVendorRole.Text = dbVendorRole.VendorRole
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbVendorRole As VendorRoleRow

			If VendorRoleID <> 0 Then
				dbVendorRole = VendorRoleRow.GetRow(DB, VendorRoleID)
			Else
				dbVendorRole = New VendorRoleRow(DB)
			End If
			dbVendorRole.VendorRole = txtVendorRole.Text
	
			If VendorRoleID <> 0 Then
				dbVendorRole.Update()
			Else
				VendorRoleID = dbVendorRole.Insert
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
		Response.Redirect("delete.aspx?VendorRoleID=" & VendorRoleID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
