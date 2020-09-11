Imports Components
Imports DataLayer
Namespace Survey.Report.QuestionType
    Public Class Demographic
        Inherits BaseSurveyControl



        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                LoadFromDB()
            End If
        End Sub
        Private Sub LoadFromDB()

            Dim dbSurveyQuestion As SurveyQuestionRow = SurveyQuestionRow.GetRow(DB, QuestionId)
            Me.ltlQuestionText.Text = dbSurveyQuestion.Name

        End Sub

        Protected Sub btnViewResponses_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnViewResponses.Click
            Response.Redirect("/admin/surveys/reports/ViewDemographics.aspx?SurveyId=" & SurveyId & "&QuestionId=" & QuestionId)

        End Sub
    End Class

End Namespace
