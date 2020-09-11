Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected SpecialOrderProductID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("SPECIAL_ORDER_PRODUCTS")

		SpecialOrderProductID = Convert.ToInt32(Request("SpecialOrderProductID"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		drpBuilderID.Datasource = BuilderRow.GetList(DB,"CompanyName")
		drpBuilderID.DataValueField = "BuilderID"
		drpBuilderID.DataTextField = "CompanyName"
		drpBuilderID.Databind
		drpBuilderID.Items.Insert(0, New ListItem("",""))
	
		drpUnitOfMeasureID.Datasource = UnitOfMeasureRow.GetList(DB,"UnitOfMeasure")
		drpUnitOfMeasureID.DataValueField = "UnitOfMeasureID"
		drpUnitOfMeasureID.DataTextField = "UnitOfMeasure"
		drpUnitOfMeasureID.Databind
		drpUnitOfMeasureID.Items.Insert(0, New ListItem("",""))
	
		If SpecialOrderProductID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbSpecialOrderProduct As SpecialOrderProductRow = SpecialOrderProductRow.GetRow(DB, SpecialOrderProductID)
		txtSpecialOrderProduct.Text = dbSpecialOrderProduct.SpecialOrderProduct
		txtDescription.Text = dbSpecialOrderProduct.Description
		drpBuilderID.SelectedValue = dbSpecialOrderProduct.BuilderID
		drpUnitOfMeasureID.SelectedValue = dbSpecialOrderProduct.UnitOfMeasureID
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbSpecialOrderProduct As SpecialOrderProductRow

			If SpecialOrderProductID <> 0 Then
				dbSpecialOrderProduct = SpecialOrderProductRow.GetRow(DB, SpecialOrderProductID)
			Else
				dbSpecialOrderProduct = New SpecialOrderProductRow(DB)
			End If
			dbSpecialOrderProduct.SpecialOrderProduct = txtSpecialOrderProduct.Text
			dbSpecialOrderProduct.Description = txtDescription.Text
			dbSpecialOrderProduct.BuilderID = drpBuilderID.SelectedValue
			dbSpecialOrderProduct.UnitOfMeasureID = drpUnitOfMeasureID.SelectedValue
	
			If SpecialOrderProductID <> 0 Then
				dbSpecialOrderProduct.Update()
			Else
				SpecialOrderProductID = dbSpecialOrderProduct.Insert
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
		Response.Redirect("delete.aspx?SpecialOrderProductID=" & SpecialOrderProductID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
