Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected SupplyPhaseCategoryId As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("SUPPLY_PHASE_CATEGORYS")

		SupplyPhaseCategoryId = Convert.ToInt32(Request("SupplyPhaseCategoryId"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		If SupplyPhaseCategoryId = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbSupplyPhaseCategory As SupplyPhaseCategoryRow = SupplyPhaseCategoryRow.GetRow(DB, SupplyPhaseCategoryId)
		txtSupplyPhaseCategory.Text = dbSupplyPhaseCategory.SupplyPhaseCategory
		txtDescription.Text = dbSupplyPhaseCategory.Description
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbSupplyPhaseCategory As SupplyPhaseCategoryRow

			If SupplyPhaseCategoryId <> 0 Then
				dbSupplyPhaseCategory = SupplyPhaseCategoryRow.GetRow(DB, SupplyPhaseCategoryId)
			Else
				dbSupplyPhaseCategory = New SupplyPhaseCategoryRow(DB)
			End If
			dbSupplyPhaseCategory.SupplyPhaseCategory = txtSupplyPhaseCategory.Text
			dbSupplyPhaseCategory.Description = txtDescription.Text
	
			If SupplyPhaseCategoryId <> 0 Then
				dbSupplyPhaseCategory.Update()
			Else
				SupplyPhaseCategoryId = dbSupplyPhaseCategory.Insert
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
		Response.Redirect("delete.aspx?SupplyPhaseCategoryId=" & SupplyPhaseCategoryId & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
