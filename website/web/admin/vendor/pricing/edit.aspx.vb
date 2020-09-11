Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

    Protected VendorID As Integer
    Protected ProductID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("VENDOR_PRODUCT_PRICES")

		VendorID = Convert.ToInt32(Request("VendorID"))
        ProductID = Convert.ToInt32(Request("ProductID"))
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
	
        If VendorID = 0 Or ProductID = 0 Then
            Response.Redirect("default.aspx")
            Exit Sub
        End If

        Dim dbVendorProductPrice As VendorProductPriceRow = VendorProductPriceRow.GetRow(DB, VendorID, ProductID)
		txtVendorSKU.Text = dbVendorProductPrice.VendorSKU
		txtVendorPrice.Text = dbVendorProductPrice.VendorPrice
		txtSubstituteQuantityMultiplier.Text = dbVendorProductPrice.SubstituteQuantityMultiplier
        txtNextPrice.Text = dbVendorProductPrice.NextPrice
        dtNextPriceApplies.Value = dbVendorProductPrice.NextPriceApplies
		drpProductID.SelectedValue = dbVendorProductPrice.ProductID
		rblIsSubstitution.SelectedValue = dbVendorProductPrice.IsSubstitution
		rblIsUpload.SelectedValue = dbVendorProductPrice.IsUpload
		rblIsDiscontinued.SelectedValue = dbVendorProductPrice.IsDiscontinued
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbVendorProductPrice As VendorProductPriceRow

			If VendorID <> 0 Then
                dbVendorProductPrice = VendorProductPriceRow.GetRow(DB, VendorID, ProductID)
			Else
				dbVendorProductPrice = New VendorProductPriceRow(DB)
			End If
			dbVendorProductPrice.VendorSKU = txtVendorSKU.Text
			dbVendorProductPrice.VendorPrice = txtVendorPrice.Text
			dbVendorProductPrice.SubstituteQuantityMultiplier = txtSubstituteQuantityMultiplier.Text
            dbVendorProductPrice.NextPrice = txtNextPrice.Text
            dbVendorProductPrice.NextPriceApplies = dtNextPriceApplies.Value
			dbVendorProductPrice.ProductID = drpProductID.SelectedValue
			dbVendorProductPrice.IsSubstitution = rblIsSubstitution.SelectedValue
			dbVendorProductPrice.IsUpload = rblIsUpload.SelectedValue
			dbVendorProductPrice.IsDiscontinued = rblIsDiscontinued.SelectedValue
	
			If VendorID <> 0 Then
				dbVendorProductPrice.Update()
			Else
				VendorID = dbVendorProductPrice.Insert
			End If
	
			DB.CommitTransaction()

	
            Response.Redirect("default.aspx?VendorID=" & VendorID & "&" & GetPageParams(FilterFieldType.All))

		Catch ex As SqlException
			If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
		End Try
	End Sub

	Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?VendorID=" & VendorID & "&" & GetPageParams(FilterFieldType.All))
	End Sub

	Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("delete.aspx?ProductID=" & ProductID & "&VendorID=" & VendorID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
