Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class admin_surveys_reports_Default
    Inherits AdminPage

    Private SurveyID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        SurveyID = Request("SurveyID")
        AddControls()
    End Sub
    Private Sub AddControls()
        Dim dtQuestions As DataTable = SurveyQuestionRow.GetAllQuestionsFromSurveyID(DB, SurveyID)
        Dim dr As DataRow

        Me.plcReport.Controls.Clear()

        For Each dr In dtQuestions.Rows

            Select Case dr("QuestionTypeID")
                Case 1
                    Dim question As Survey.Report.Controls.ShortResponse = LoadControl("/admin/surveys/reports/controls/shortresponse.ascx")
                    question.QuestionId = dr("QuestionID")
                    question.SurveyId = SurveyID
                    Me.plcReport.Controls.Add(question)

                Case 2
                    Dim question As Survey.Report.Controls.LongResponse = LoadControl("/admin/surveys/reports/controls/longresponse.ascx")
                    question.QuestionId = dr("QuestionID")
                    question.SurveyId = SurveyID
                    Me.plcReport.Controls.Add(question)

                Case 3
                    Dim question As Survey.Report.Controls.SelectOne = LoadControl("/admin/surveys/reports/controls/selectone.ascx")
                    question.QuestionId = dr("QuestionID")
                    question.SurveyId = SurveyID
                    Me.plcReport.Controls.Add(question)

                Case 4
                    Dim question As Survey.Report.Controls.SelectAllThatApply = LoadControl("/admin/surveys/reports/controls/selectallthatapply.ascx")
                    question.QuestionId = dr("QuestionID")
                    question.SurveyId = SurveyID
                    Me.plcReport.Controls.Add(question)

                Case 5
                    Dim question As Survey.Report.Controls.StandardRank = LoadControl("/admin/surveys/reports/controls/standardrank.ascx")
                    question.QuestionId = dr("QuestionID")
                    question.SurveyId = SurveyID
                    Me.plcReport.Controls.Add(question)

                Case 6
                    Dim question As Survey.Report.Controls.PercentRank = LoadControl("/admin/surveys/reports/controls/percentrank.ascx")
                    question.QuestionId = dr("QuestionID")
                    question.SurveyId = SurveyID
                    Me.plcReport.Controls.Add(question)

                Case 7
                    Dim question As Survey.Report.Controls.Date = LoadControl("/admin/surveys/reports/controls/date.ascx")
                    question.QuestionId = dr("QuestionID")
                    question.SurveyId = SurveyID
                    Me.plcReport.Controls.Add(question)

                Case 8
                    Dim question As Survey.Report.Controls.Quantity = LoadControl("/admin/surveys/reports/controls/quantity.ascx")
                    question.QuestionId = dr("QuestionID")
                    question.SurveyId = SurveyID
                    Me.plcReport.Controls.Add(question)

                Case 9
                    Dim question As Survey.Report.Controls.Rate = LoadControl("/admin/surveys/reports/controls/rate.ascx")
                    question.QuestionId = dr("QuestionID")
                    question.SurveyId = SurveyID
                    Me.plcReport.Controls.Add(question)

                Case 10
                    Dim question As Survey.Report.Controls.Demographic = LoadControl("/admin/surveys/reports/controls/demographic.ascx")
                    question.QuestionId = dr("QuestionID")
                    question.SurveyId = SurveyID
                    Me.plcReport.Controls.Add(question)


            End Select

        Next


    End Sub

    Protected Sub btnRefresh_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRefresh.Click
        Response.Redirect("default.aspx?" & Request.ServerVariables("QUERY_STRING"))
    End Sub

End Class
