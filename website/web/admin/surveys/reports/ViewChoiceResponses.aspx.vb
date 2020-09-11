Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Partial Class admin_surveys_reports_ViewChoiceResponses
    Inherits AdminPage

    Private SurveyId As Integer
    Private QuestionId As Integer
    Private ChoiceId As Integer


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try
            SurveyId = Convert.ToInt32(Request("SurveyID"))
            QuestionId = Convert.ToInt32(Request("QuestionID"))
            ChoiceId = Convert.ToInt32(Request("ChoiceId"))


        Catch ex As Exception

        End Try

		gvList.BindList = AddressOf BindList
		If Not IsPostBack Then
			gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
			gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
			If gvList.SortBy = String.Empty Then gvList.SortBy = "surveyanswer.CreateDate"
			bindlist()
		End If


    End Sub

	Private Sub bindlist()
		Dim dbQuestion As SurveyQuestionRow = SurveyQuestionRow.GetRow(DB, QuestionId)

		ViewState("F_SortBy") = gvList.SortBy
		ViewState("F_SortOrder") = gvList.SortOrder

		Dim sText As String = " FROM SurveyAnswer, SurveyQuestionChoice WHERE SurveyAnswer.Response <> '' AND SurveyAnswer.ChoiceId = SurveyQuestionChoice.ChoiceId AND SurveyAnswer.ChoiceId = " & DB.Quote(ChoiceId) & " AND SurveyAnswer.QuestionId = " & DB.NullNumber(QuestionId)

		Dim dtAnswers As DataTable = DB.GetDataTable("SELECT SurveyAnswer.*, SurveyQuestionChoice.Name " & sText & " ORDER BY " & gvList.SortByAndOrder)
		gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & sText)
		gvList.DataSource = dtAnswers
		gvList.DataBind()

	End Sub


    Protected Sub btnBack_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBack.Click
        Response.Redirect("default.aspx?SurveyId=" & Request("SurveyID"))
    End Sub
End Class
