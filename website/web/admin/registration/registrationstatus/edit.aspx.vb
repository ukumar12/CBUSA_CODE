Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected RegistrationStatusID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("REGISTRATIONS")

		RegistrationStatusID = Convert.ToInt32(Request("RegistrationStatusID"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		If RegistrationStatusID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbRegistrationStatus As RegistrationStatusRow = RegistrationStatusRow.GetRow(DB, RegistrationStatusID)
		txtRegistrationStatus.Text = dbRegistrationStatus.RegistrationStatus
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbRegistrationStatus As RegistrationStatusRow

			If RegistrationStatusID <> 0 Then
				dbRegistrationStatus = RegistrationStatusRow.GetRow(DB, RegistrationStatusID)
			Else
				dbRegistrationStatus = New RegistrationStatusRow(DB)
			End If
			dbRegistrationStatus.RegistrationStatus = txtRegistrationStatus.Text
	
			If RegistrationStatusID <> 0 Then
				dbRegistrationStatus.Update()
			Else
				RegistrationStatusID = dbRegistrationStatus.Insert
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
		Response.Redirect("delete.aspx?RegistrationStatusID=" & RegistrationStatusID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
