Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected ProductTypeID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("PRODUCT_TYPES")

		ProductTypeID = Convert.ToInt32(Request("ProductTypeID"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		If ProductTypeID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbProductType As ProductTypeRow = ProductTypeRow.GetRow(DB, ProductTypeID)
		txtProductType.Text = dbProductType.ProductType
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbProductType As ProductTypeRow

			If ProductTypeID <> 0 Then
				dbProductType = ProductTypeRow.GetRow(DB, ProductTypeID)
			Else
				dbProductType = New ProductTypeRow(DB)
			End If
			dbProductType.ProductType = txtProductType.Text
	
			If ProductTypeID <> 0 Then
				dbProductType.Update()
			Else
				ProductTypeID = dbProductType.Insert
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
		Response.Redirect("delete.aspx?ProductTypeID=" & ProductTypeID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
