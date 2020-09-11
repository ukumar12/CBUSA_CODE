Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Partial Class admin_surveys_reports_ViewDemographics
    Inherits AdminPage

    Private SurveyId As Integer
    Private QuestionId As Integer
    Private dvDemographics As DataView



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try
            SurveyId = Convert.ToInt32(Request("SurveyID"))
            QuestionId = Convert.ToInt32(Request("QuestionID"))

        Catch ex As Exception

        End Try
        dvDemographics = DataLayer.SurveyResponseDemographicRow.GetAllSurveyDemographics(DB, SurveyId).DefaultView

        Dim dtResponses As DataTable = SurveyResponseRow.GetResponses(DB, SurveyId)
        Me.rptList.DataSource = dtResponses
        rptList.DataBind()


    End Sub

    Protected Sub btnBack_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBack.Click
        Response.Redirect("default.aspx?SurveyId=" & Request("SurveyID"))
    End Sub

    Protected Sub rptList_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptList.ItemDataBound
        If e.Item.ItemType = ListItemType.Header Then
            Dim ltl As Literal = e.Item.FindControl("ltlHeader")
            Dim sText As String = ""
            Dim dr As DataRow, dtFields As DataTable = DataLayer.SurveyQuestionDemographicRow.GetQuestionDemographics(DB, QuestionId)

            For Each dr In dtFields.Rows
                sText &= "<th>" & dr("Name") & "</th>"
            Next
            ltl.Text = sText
        End If
        If e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.Item Then
            Dim ltl As Literal = e.Item.FindControl("ltlBody")
            Dim sText As String = ""
            Dim drv As DataRowView

            dvDemographics.RowFilter = "ResponseId=" & e.Item.DataItem("ResponseId")

            For Each drv In dvDemographics
                sText &= "<td>" & drv("Value") & "</td>"
            Next
            ltl.Text = sText
        End If
    End Sub
End Class
