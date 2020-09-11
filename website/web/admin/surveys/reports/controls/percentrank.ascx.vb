Imports Components
Imports DataLayer
Namespace Survey.Report.QuestionType
    Public Class PercentRank
        Inherits BaseSurveyControl

        Protected TotalResponses As Integer
        Private dtPercentRankCounts As DataTable
        Private dtChoices As DataTable

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then

                LoadFromDB()
            End If
        End Sub
        Private Sub LoadFromDB()

            TotalResponses = SurveyResponseRow.getTotalResponses(DB, SurveyId)

            Dim dbSurveyQuestion As SurveyQuestionRow = SurveyQuestionRow.GetRow(DB, QuestionId)
            Me.ltlQuestionText.Text = dbSurveyQuestion.Name

            dtPercentRankCounts = DataLayer.SurveyAnswerRow.GetPercentRankCounts(DB, QuestionId)
            Me.rptChoices.DataSource = dtPercentRankCounts
            Me.rptChoices.DataBind()


        End Sub

        Protected Sub rptChoices_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptChoices.ItemDataBound


            If e.Item.ItemType = ListItemType.Header Then
               
            End If

            If TotalResponses > 0 And (e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.Item) Then

                Dim lbl As Label = e.Item.FindControl("lblTotals")
                If Convert.ToInt32(e.Item.DataItem("ChoiceSum")) > 0 Then
                    lbl.Text = Math.Round(Convert.ToInt32(e.Item.DataItem("ChoiceSum")) / TotalResponses, 1)
                Else
                    lbl.Text = "0"
                End If
                If e.Item.DataItem("ShowResponseField") Then
                    CType(e.Item.FindControl("lnkView"), HtmlAnchor).Visible = True
                    CType(e.Item.FindControl("lnkView"), HtmlAnchor).HRef = "../ViewChoiceResponses.aspx?SurveyId=" & SurveyId & "&ChoiceId=" & e.Item.DataItem("ChoiceId") & "&QuestionId=" & QuestionId
                End If
            End If
        End Sub
    End Class

End Namespace
