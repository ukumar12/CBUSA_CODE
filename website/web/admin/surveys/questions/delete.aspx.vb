Imports Components
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer

Partial Class admin_survey_question_delete
    Inherits AdminPage

    Protected SurveyId As Integer
    Protected PageId As Integer
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("SURVEY")

        Dim QuestionId As Integer = Convert.ToInt32(Request("QuestionId"))
        PageId = Convert.ToInt32(Request("PageId"))
        SurveyId = Convert.ToInt32(Request("SurveyId"))
        Try
            DB.BeginTransaction()
            SurveyQuestionRow.RemoveRow(DB, QuestionId)
            DB.CommitTransaction()

            Response.Redirect("default.aspx?SurveyId=" & SurveyId & "&PageId=" & PageId & "&" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class
