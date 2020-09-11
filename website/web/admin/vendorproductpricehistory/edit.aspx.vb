Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected VendorProductPriceHistoryID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("VENDOR_PRODUCT_PRICE_HISTORYS")

		VendorProductPriceHistoryID = Convert.ToInt32(Request("VendorProductPriceHistoryID"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		drpProductID.Datasource = ProductRow.GetList(DB,"Product")
		drpProductID.DataValueField = "ProductID"
		drpProductID.DataTextField = "Product"
		drpProductID.Databind
		drpProductID.Items.Insert(0, New ListItem("",""))
	
		drpVendorID.Datasource = VendorRow.GetList(DB,"CompanyName")
		drpVendorID.DataValueField = "VendorID"
		drpVendorID.DataTextField = "CompanyName"
		drpVendorID.Databind
		drpVendorID.Items.Insert(0, New ListItem("",""))
	
		If VendorProductPriceHistoryID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbVendorProductPriceHistory As VendorProductPriceHistoryRow = VendorProductPriceHistoryRow.GetRow(DB, VendorProductPriceHistoryID)
		txtVendorSKU.Text = dbVendorProductPriceHistory.VendorSKU
		txtVendorPrice.Text = dbVendorProductPriceHistory.VendorPrice
		txtSubmitterVendorAccountID.Text = dbVendorProductPriceHistory.SubmitterVendorAccountID
		drpProductID.SelectedValue = dbVendorProductPriceHistory.ProductID
		drpVendorID.SelectedValue = dbVendorProductPriceHistory.VendorID
		rblIsSubstitution.SelectedValue = dbVendorProductPriceHistory.IsSubstitution
		rblIsUpload.SelectedValue = dbVendorProductPriceHistory.IsUpload
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbVendorProductPriceHistory As VendorProductPriceHistoryRow

			If VendorProductPriceHistoryID <> 0 Then
				dbVendorProductPriceHistory = VendorProductPriceHistoryRow.GetRow(DB, VendorProductPriceHistoryID)
			Else
				dbVendorProductPriceHistory = New VendorProductPriceHistoryRow(DB)
			End If
			dbVendorProductPriceHistory.VendorSKU = txtVendorSKU.Text
			dbVendorProductPriceHistory.VendorPrice = txtVendorPrice.Text
			dbVendorProductPriceHistory.SubmitterVendorAccountID = txtSubmitterVendorAccountID.Text
			dbVendorProductPriceHistory.ProductID = drpProductID.SelectedValue
			dbVendorProductPriceHistory.VendorID = drpVendorID.SelectedValue
			dbVendorProductPriceHistory.IsSubstitution = rblIsSubstitution.SelectedValue
			dbVendorProductPriceHistory.IsUpload = rblIsUpload.SelectedValue
	
			If VendorProductPriceHistoryID <> 0 Then
				dbVendorProductPriceHistory.Update()
			Else
				VendorProductPriceHistoryID = dbVendorProductPriceHistory.Insert
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
		Response.Redirect("delete.aspx?VendorProductPriceHistoryID=" & VendorProductPriceHistoryID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
