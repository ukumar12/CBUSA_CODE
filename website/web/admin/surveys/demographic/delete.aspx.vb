Imports Components
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer

Partial Class admin_survey_question_demographic_delete
    Inherits AdminPage
    Protected QuestionId As Integer
    Protected SurveyId As Integer
    Protected PageId As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("SURVEY")

        QuestionId = Convert.ToInt32(Request("QuestionId"))
        SurveyId = Convert.ToInt32(Request("SurveyId"))
        PageId = Convert.ToInt32(Request("PageId"))

        Dim SurveyQuestionDemographicId As Integer = Convert.ToInt32(Request("SurveyQuestionDemographicId"))
        Try
            DB.BeginTransaction()
            SurveyQuestionDemographicRow.RemoveRow(DB, SurveyQuestionDemographicId)
            DB.CommitTransaction()

            Response.Redirect("default.aspx?SurveyId=" & SurveyId & "&PageId=" & PageId & "&QuestionId=" & QuestionId & "&" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class
