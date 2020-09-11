Imports Components
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer

Partial Class admin_survey_question_demographic_move
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
        Dim Action As String = Request("ACTION")
        Try
            DB.BeginTransaction()
            If Core.ChangeSortOrder(DB, "SurveyQuestionDemographicId", "SurveyQuestionDemographic", "SortOrder", "QuestionId = " & QuestionId, SurveyQuestionDemographicId, Action) Then
                DB.CommitTransaction()
            Else
                DB.RollbackTransaction()
            End If
            Response.Redirect("default.aspx?SurveyId=" & SurveyId & "&PageId=" & PageId & "&QuestionId=" & QuestionId & "&" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class
