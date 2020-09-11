Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected VendorCategoryID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("VENDOR_CATEGORYS")

		VendorCategoryID = Convert.ToInt32(Request("VendorCategoryID"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		If VendorCategoryID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbVendorCategory As VendorCategoryRow = VendorCategoryRow.GetRow(DB, VendorCategoryID)
        txtCategory.Text = dbVendorCategory.Category
        rblIsPlansOnline.SelectedValue = dbVendorCategory.IsPlansOnline
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbVendorCategory As VendorCategoryRow

			If VendorCategoryID <> 0 Then
				dbVendorCategory = VendorCategoryRow.GetRow(DB, VendorCategoryID)
			Else
				dbVendorCategory = New VendorCategoryRow(DB)
			End If
			dbVendorCategory.Category = txtCategory.Text
            dbVendorCategory.IsPlansOnline = rblIsPlansOnline.SelectedValue
			If VendorCategoryID <> 0 Then
				dbVendorCategory.Update()
			Else
				VendorCategoryID = dbVendorCategory.Insert
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
		Response.Redirect("delete.aspx?VendorCategoryID=" & VendorCategoryID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
