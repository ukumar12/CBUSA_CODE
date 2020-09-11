Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class admin_survey_category_Edit
    Inherits AdminPage

    Protected CategoryId As Integer
    Protected SurveyId As Integer
    Protected PageId As Integer
    Protected QuestionId As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("SURVEY")

        CategoryId = Convert.ToInt32(Request("CategoryId"))
        SurveyId = Convert.ToInt32(Request("SurveyId"))
        PageId = Convert.ToInt32(Request("PageId"))
        QuestionId = Convert.ToInt32(Request("QuestionId"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If CategoryId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbSurveyQuestionCategory As SurveyQuestionCategoryRow = SurveyQuestionCategoryRow.GetRow(DB, CategoryId)
        txtName.Text = dbSurveyQuestionCategory.Name
        txtDisplayText.Value = dbSurveyQuestionCategory.DisplayText
        chkShowComments.Checked = dbSurveyQuestionCategory.ShowComments
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbSurveyQuestionCategory As SurveyQuestionCategoryRow

            If CategoryId <> 0 Then
                dbSurveyQuestionCategory = SurveyQuestionCategoryRow.GetRow(DB, CategoryId)
            Else
                dbSurveyQuestionCategory = New SurveyQuestionCategoryRow(DB)
            End If
            dbSurveyQuestionCategory.QuestionId = QuestionId
            dbSurveyQuestionCategory.Name = txtName.Text
            dbSurveyQuestionCategory.DisplayText = txtDisplayText.Value
            dbSurveyQuestionCategory.ShowComments = chkShowComments.Checked

            If CategoryId <> 0 Then
                dbSurveyQuestionCategory.Update()
            Else
                CategoryId = dbSurveyQuestionCategory.Insert
            End If

            DB.CommitTransaction()


            Response.Redirect("default.aspx?SurveyId=" & SurveyId & "&PageId=" & PageId & "&QuestionId=" & QuestionId & "&" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?SurveyId=" & SurveyId & "&PageId=" & PageId & "&QuestionId=" & QuestionId & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("delete.aspx?SurveyId=" & SurveyId & "&PageId=" & PageId & "&QuestionId=" & QuestionId & "&CategoryId=" & CategoryId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class
