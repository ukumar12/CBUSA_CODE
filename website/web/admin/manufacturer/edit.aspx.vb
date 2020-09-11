Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected ManufacturerID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("MANUFACTURERS")

		ManufacturerID = Convert.ToInt32(Request("ManufacturerID"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		If ManufacturerID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbManufacturer As ManufacturerRow = ManufacturerRow.GetRow(DB, ManufacturerID)
		txtManufacturer.Text = dbManufacturer.Manufacturer
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbManufacturer As ManufacturerRow

			If ManufacturerID <> 0 Then
				dbManufacturer = ManufacturerRow.GetRow(DB, ManufacturerID)
			Else
				dbManufacturer = New ManufacturerRow(DB)
			End If
			dbManufacturer.Manufacturer = txtManufacturer.Text
	
			If ManufacturerID <> 0 Then
				dbManufacturer.Update()
			Else
				ManufacturerID = dbManufacturer.Insert
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
		Response.Redirect("delete.aspx?ManufacturerID=" & ManufacturerID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
