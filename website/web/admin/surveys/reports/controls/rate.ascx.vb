Imports Components
Imports DataLayer
Namespace Survey.Report.QuestionType
    Public Class Rate
        Inherits BaseSurveyControl

        Protected TotalResponses As Integer

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                LoadFromDB()
            End If
        End Sub
        Private Sub LoadFromDB()

            TotalResponses = SurveyResponseRow.getTotalResponses(DB, SurveyId)

            Dim dbSurveyQuestion As SurveyQuestionRow = SurveyQuestionRow.GetRow(DB, QuestionId)
            Me.ltlQuestionText.Text = dbSurveyQuestion.Name

            Me.rptChoices.DataSource = DataLayer.SurveyAnswerRow.GetChoiceCounts(DB, QuestionId)
            Me.rptChoices.DataBind()

        End Sub

        Protected Sub rptChoices_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptChoices.ItemDataBound
            If e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.Item Then
                If TotalResponses > 0 Then
                    Dim img As HtmlImage = e.Item.FindControl("img")
                    Dim iWidth As Integer = (e.Item.DataItem("AnswerCount") / TotalResponses) * 400
                    img.Width = iWidth
                End If
            End If
        End Sub
    End Class

End Namespace
