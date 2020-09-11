Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected UnitOfMeasureID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("UNITS_OF_MEASURE")

		UnitOfMeasureID = Convert.ToInt32(Request("UnitOfMeasureID"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		If UnitOfMeasureID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbUnitOfMeasure As UnitOfMeasureRow = UnitOfMeasureRow.GetRow(DB, UnitOfMeasureID)
		txtUnitOfMeasure.Text = dbUnitOfMeasure.UnitOfMeasure
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbUnitOfMeasure As UnitOfMeasureRow

			If UnitOfMeasureID <> 0 Then
				dbUnitOfMeasure = UnitOfMeasureRow.GetRow(DB, UnitOfMeasureID)
			Else
				dbUnitOfMeasure = New UnitOfMeasureRow(DB)
			End If
			dbUnitOfMeasure.UnitOfMeasure = txtUnitOfMeasure.Text
	
			If UnitOfMeasureID <> 0 Then
				dbUnitOfMeasure.Update()
			Else
				UnitOfMeasureID = dbUnitOfMeasure.Insert
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
		Response.Redirect("delete.aspx?UnitOfMeasureID=" & UnitOfMeasureID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
