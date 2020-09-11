Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected RecommendedProductID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("RECOMMENDED_PRODUCTS")

		RecommendedProductID = Convert.ToInt32(Request("RecommendedProductID"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		drpUnitOfMeasureID.Datasource = UnitOfMeasureRow.GetList(DB,"UnitOfMeasure")
		drpUnitOfMeasureID.DataValueField = "UnitOfMeasureID"
		drpUnitOfMeasureID.DataTextField = "UnitOfMeasure"
		drpUnitOfMeasureID.Databind
		drpUnitOfMeasureID.Items.Insert(0, New ListItem("",""))
	
		drpProductTypeID.Datasource = ProductTypeRow.GetList(DB,"ProductType")
		drpProductTypeID.DataValueField = "ProductTypeID"
		drpProductTypeID.DataTextField = "ProductType"
		drpProductTypeID.Databind
		drpProductTypeID.Items.Insert(0, New ListItem("",""))
	
		drpRecommendedProductStatusID.Datasource = RecommendedProductStatusRow.GetList(DB,"RecommendedProductStatus")
		drpRecommendedProductStatusID.DataValueField = "RecommendedProductStatusID"
		drpRecommendedProductStatusID.DataTextField = "RecommendedProductStatus"
		drpRecommendedProductStatusID.Databind
		drpRecommendedProductStatusID.Items.Insert(0, New ListItem("",""))
	
		If RecommendedProductID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbRecommendedProduct As RecommendedProductRow = RecommendedProductRow.GetRow(DB, RecommendedProductID)
		txtVendorID.Text = dbRecommendedProduct.VendorID
		txtRecommendedProduct.Text = dbRecommendedProduct.RecommendedProduct
		txtDescription.Text = dbRecommendedProduct.Description
		txtManufacturerID.Text = dbRecommendedProduct.ManufacturerID
		txtSize.Text = dbRecommendedProduct.Size
		txtGrade.Text = dbRecommendedProduct.Grade
		drpUnitOfMeasureID.SelectedValue = dbRecommendedProduct.UnitOfMeasureID
		drpProductTypeID.SelectedValue = dbRecommendedProduct.ProductTypeID
		drpRecommendedProductStatusID.SelectedValue = dbRecommendedProduct.RecommendedProductStatusID
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbRecommendedProduct As RecommendedProductRow

			If RecommendedProductID <> 0 Then
				dbRecommendedProduct = RecommendedProductRow.GetRow(DB, RecommendedProductID)
			Else
				dbRecommendedProduct = New RecommendedProductRow(DB)
			End If
			dbRecommendedProduct.VendorID = txtVendorID.Text
			dbRecommendedProduct.RecommendedProduct = txtRecommendedProduct.Text
			dbRecommendedProduct.Description = txtDescription.Text
			dbRecommendedProduct.ManufacturerID = txtManufacturerID.Text
			dbRecommendedProduct.Size = txtSize.Text
			dbRecommendedProduct.Grade = txtGrade.Text
			dbRecommendedProduct.UnitOfMeasureID = drpUnitOfMeasureID.SelectedValue
			dbRecommendedProduct.ProductTypeID = drpProductTypeID.SelectedValue
			dbRecommendedProduct.RecommendedProductStatusID = drpRecommendedProductStatusID.SelectedValue
	
			If RecommendedProductID <> 0 Then
				dbRecommendedProduct.Update()
			Else
				RecommendedProductID = dbRecommendedProduct.Insert
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
		Response.Redirect("delete.aspx?RecommendedProductID=" & RecommendedProductID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
