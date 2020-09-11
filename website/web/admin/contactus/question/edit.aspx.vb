Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected QuestionId As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("CONTACT_US")

		QuestionId = Convert.ToInt32(Request("QuestionId"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		If QuestionId = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End If

		Dim dbContactUsQuestion As ContactUsQuestionRow = ContactUsQuestionRow.GetRow(DB, QuestionId)
		txtQuestion.Text = dbContactUsQuestion.Question
		txtEmailAddress.Text = dbContactUsQuestion.EmailAddress
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbContactUsQuestion As ContactUsQuestionRow

			If QuestionId <> 0 Then
				dbContactUsQuestion = ContactUsQuestionRow.GetRow(DB, QuestionId)
			Else
				dbContactUsQuestion = New ContactUsQuestionRow(DB)
			End If
			dbContactUsQuestion.Question = txtQuestion.Text
			dbContactUsQuestion.EmailAddress = txtEmailAddress.Text

			If QuestionId <> 0 Then
				dbContactUsQuestion.Update()
			Else
				QuestionId = dbContactUsQuestion.Insert
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
		Response.Redirect("delete.aspx?QuestionId=" & QuestionId & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class

