Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected SwatchId As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("STORE")

		SwatchId = Convert.ToInt32(Request("SwatchId"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		If SwatchId = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End If

		Dim dbLookupSwatch As LookupSwatchRow = LookupSwatchRow.GetRow(DB, SwatchId)
		txtName.Text = dbLookupSwatch.Name
		txtSKU.Text = dbLookupSwatch.SKU
		txtPrice.Text = dbLookupSwatch.Price
		txtWeight.Text = dbLookupSwatch.Weight
		fuImage.CurrentFileName = dbLookupSwatch.Image
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbLookupSwatch As LookupSwatchRow

			If SwatchId <> 0 Then
				dbLookupSwatch = LookupSwatchRow.GetRow(DB, SwatchId)
			Else
				dbLookupSwatch = New LookupSwatchRow(DB)
			End If
			dbLookupSwatch.Name = txtName.Text
			dbLookupSwatch.SKU = txtSKU.Text
			dbLookupSwatch.Price = txtPrice.Text
			dbLookupSwatch.Weight = txtWeight.Text
			If fuImage.NewFileName <> String.Empty Then
				fuImage.SaveNewFile()
				dbLookupSwatch.Image = fuImage.NewFileName
			ElseIf fuImage.MarkedToDelete Then
				dbLookupSwatch.Image = Nothing
			End If

			If SwatchId <> 0 Then
				dbLookupSwatch.Update()
			Else
				SwatchId = dbLookupSwatch.Insert
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
		Response.Redirect("delete.aspx?SwatchId=" & SwatchId & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class

