Imports Components
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer

Partial Class admin_survey_category_delete
    Inherits AdminPage
    Protected SurveyId As Integer
    Protected PageId As Integer
    Protected QuestionId As Integer
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("SURVEY")

        SurveyId = Convert.ToInt32(Request("SurveyId"))
        PageId = Convert.ToInt32(Request("PageId"))
        QuestionId = Convert.ToInt32(Request("QuestionId"))
        Dim CategoryId As Integer = Convert.ToInt32(Request("CategoryId"))
        Try
            DB.BeginTransaction()
            SurveyQuestionCategoryRow.RemoveRow(DB, CategoryId)
            DB.CommitTransaction()

            Response.Redirect("default.aspx?SurveyId=" & SurveyId & "&PageId=" & PageId & "&QuestionId=" & QuestionId & "&" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class
