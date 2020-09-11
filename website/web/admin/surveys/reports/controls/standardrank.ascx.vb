Imports Components
Imports DataLayer
Namespace Survey.Report.QuestionType
    Public Class StandardRank
        Inherits BaseSurveyControl

        Private TotalResponses As Integer
        Private dvStandardRankCounts As DataView
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

            dvStandardRankCounts = DataLayer.SurveyAnswerRow.GetStandardRankCounts(DB, QuestionId).DefaultView

            dtChoices = DataLayer.SurveyAnswerRow.GetChoiceCounts(DB, QuestionId)
            Me.rptChoices.DataSource = dtChoices
            Me.rptChoices.DataBind()


        End Sub

        Protected Sub rptChoices_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptChoices.ItemDataBound
            Dim table As HtmlTable, tr As HtmlTableRow, td As HtmlTableCell
            Dim plc As PlaceHolder = e.Item.FindControl("plcTotals")
            Dim bShowResponseField As Boolean = False

            If e.Item.ItemType = ListItemType.Header Then
                plc = e.Item.FindControl("plcHeader")

                table = New HtmlTable
                table.CellPadding = "0"
                table.CellSpacing = "0"
                table.Border = "0"
                table.Style.Add("width", "100%")
                table.Attributes.Add("rules", "all")
                plc.Controls.Add(table)

                tr = New HtmlTableRow
                tr.Style.Add("height", "30px")
                table.Controls.Add(tr)

                Dim iIndex As Integer
                For iIndex = 1 To dtChoices.Rows.Count

                    td = New HtmlTableCell
                    td.Style.Add("height", "30px")
                    td.Style.Add("text-align", "center")
                    td.Style.Add("vertical-align", "middle")
                    td.InnerText = iIndex.ToString
                    tr.Controls.Add(td)

                Next
            End If

            If e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.Item Then

                plc = e.Item.FindControl("plcTotals")

                table = New HtmlTable
                table.CellPadding = "0"
                table.CellSpacing = "0"
                table.Border = "0"
                table.Attributes.Add("rules", "all")
                table.Style.Add("width", "100%")
                plc.Controls.Add(table)

                tr = New HtmlTableRow
                tr.Style.Add("height", "30px")
                table.Controls.Add(tr)

                Dim iIndex As Integer
                For iIndex = 1 To dtChoices.Rows.Count

                    td = New HtmlTableCell
                    td.Style.Add("height", "30px")
                    td.Style.Add("text-align", "center")
                    td.Style.Add("vertical-align", "middle")

                    dvStandardRankCounts.RowFilter = "ChoiceId = " & e.Item.DataItem("ChoiceId") & " AND Value = " & CStr(iIndex)
                    If dvStandardRankCounts.Count > 0 Then
                        td.InnerText = dvStandardRankCounts(0)("RankCount")
                    Else
                        td.InnerText = "0"
                    End If

                    tr.Controls.Add(td)

                Next
                If e.Item.DataItem("ShowResponseField") Then
                    CType(e.Item.FindControl("lnkView"), HtmlAnchor).Visible = True
                    CType(e.Item.FindControl("lnkView"), HtmlAnchor).HRef = "../ViewChoiceResponses.aspx?SurveyId=" & SurveyId & "&ChoiceId=" & e.Item.DataItem("ChoiceId") & "&QuestionId=" & QuestionId
                End If
            End If
        End Sub
    End Class

End Namespace
