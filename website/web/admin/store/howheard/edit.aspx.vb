Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected HowHeardId As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("STORE")

		HowHeardId = Convert.ToInt32(Request("HowHeardId"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		If HowHeardId = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End If

		Dim dbHowHeard As HowHeardRow = HowHeardRow.GetRow(DB, HowHeardId)
		txtHowHeard.Text = dbHowHeard.HowHeard
		txtUserInputLabel.Text = dbHowHeard.UserInputLabel
		chkIsUserInput.Checked = dbHowHeard.IsUserInput
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbHowHeard As HowHeardRow

			If HowHeardId <> 0 Then
				dbHowHeard = HowHeardRow.GetRow(DB, HowHeardId)
			Else
				dbHowHeard = New HowHeardRow(DB)
			End If
			dbHowHeard.HowHeard = txtHowHeard.Text
			dbHowHeard.UserInputLabel = txtUserInputLabel.Text
			dbHowHeard.IsUserInput = chkIsUserInput.Checked

			If HowHeardId <> 0 Then
				dbHowHeard.Update()
			Else
				HowHeardId = dbHowHeard.Insert
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
		Response.Redirect("delete.aspx?HowHeardId=" & HowHeardId & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class