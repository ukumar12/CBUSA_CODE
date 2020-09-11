Imports Components
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer

Partial Class admin_survey_choice_delete
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("SURVEY")

        Dim SurveyId As Integer = Convert.ToInt32(Request("SurveyId"))
        Dim PageId As Integer = Convert.ToInt32(Request("PageId"))
        Dim QuestionId As Integer = Convert.ToInt32(Request("QuestionId"))
        Dim ChoiceId As Integer = Convert.ToInt32(Request("ChoiceId"))

        Try
            DB.BeginTransaction()
            SurveyQuestionChoiceRow.RemoveRow(DB, ChoiceId)
            DB.CommitTransaction()

            Response.Redirect("default.aspx?SurveyId=" & SurveyId & "&PageId=" & PageId & "&QuestionId=" & QuestionId & "&" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class