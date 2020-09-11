Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected ProjectStatusID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("PROJECT_STATUS")

		ProjectStatusID = Convert.ToInt32(Request("ProjectStatusID"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		If ProjectStatusID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbProjectStatus As ProjectStatusRow = ProjectStatusRow.GetRow(DB, ProjectStatusID)
		txtProjectStatus.Text = dbProjectStatus.ProjectStatus
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbProjectStatus As ProjectStatusRow

			If ProjectStatusID <> 0 Then
				dbProjectStatus = ProjectStatusRow.GetRow(DB, ProjectStatusID)
			Else
				dbProjectStatus = New ProjectStatusRow(DB)
			End If
			dbProjectStatus.ProjectStatus = txtProjectStatus.Text
	
			If ProjectStatusID <> 0 Then
				dbProjectStatus.Update()
			Else
				ProjectStatusID = dbProjectStatus.Insert
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
		Response.Redirect("delete.aspx?ProjectStatusID=" & ProjectStatusID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
