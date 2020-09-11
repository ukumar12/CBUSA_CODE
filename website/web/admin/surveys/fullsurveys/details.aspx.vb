Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class admin_surveys_fullsurveys_details
    Inherits AdminPage

    Private ResponseId As Integer = 0

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("SURVEY")

        ResponseId = Convert.ToInt32(Request("ResponseId"))

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "SurveyResponseId"

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " and "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " sq.name as question, coalesce(sa.response, sqc.name) as answer "
        SQL = " FROM surveyanswer sa LEFT OUTER JOIN surveyquestion sq ON sa.questionid = sq.questionid LEFT OUTER JOIN surveyquestionchoice sqc ON sa.choiceid = sqc.choiceid WHERE sa.responseid = " & ResponseId & " order by sa.questionid "

        
        'gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataSet = DB.GetDataSet(SQLFields & SQL)
        gvList.DataSource = res.Tables(0).DefaultView
        gvList.DataBind()
    End Sub
End Class
