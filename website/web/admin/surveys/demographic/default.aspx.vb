Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class survey_question_demographic
    Inherits AdminPage

    Protected QuestionId As Integer
    Protected SurveyId As Integer
    Protected PageId As Integer

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("SURVEY")

        QuestionId = Convert.ToInt32(Request("QuestionId"))
        SurveyId = Convert.ToInt32(Request("SurveyId"))
        PageId = Convert.ToInt32(Request("PageId"))

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then

            gvList.SortBy = "SortOrder"

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " And "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) & gvList.PageSize & " SurveyQuestionDemographic.*, SurveyDemographicField.Name as DemographicNAme "
        SQL = " FROM SurveyQuestionDemographic, SurveyDemographicField   "
        SQL &= "WHERE SurveyQuestionDemographic.DemographicId = SurveyDemographicField.DemographicId And SurveyQuestionDemographic.QuestionId = " & DB.Number(QuestionId)

        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataSet = DB.GetDataSet(SQLFields & SQL & " ORDER BY SortOrder")
        gvList.DataSource = res.Tables(0).DefaultView
        gvList.DataBind()
    End Sub

    Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
        Response.Redirect("edit.aspx?SurveyId=" & SurveyId & "&PageId=" & PageId & "&QuestionId=" & QuestionId & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub BackToQuestion_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BackToQuestion.Click
        Response.Redirect("/admin/surveys/questions/edit.aspx?SurveyId=" & SurveyId & "&PageId=" & PageId & "&QuestionId=" & QuestionId)
    End Sub
End Class
