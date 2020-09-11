Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class admin_surveys_surveys_default
    Inherits AdminPage

    Private SurveyId As Integer = 0
    Private qid As Integer = 0
    Private aid As String = String.Empty

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("SURVEY")

        SurveyId = Convert.ToInt32(Request("SurveyId"))
        qid = Convert.ToInt32(Request("qid"))
        aid = Convert.ToString(Request("aid"))

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then

            F_SubmitDateLbound.Text = Request("F_ProcessDateLBound")
            F_SubmitDateUbound.Text = Request("F_ProcessDateUBound")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            gvList.SortBy = "sr.ResponseId"

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " AND "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT DISTINCT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " sr.ResponseId, sr.CompleteDate, sr.RemoteIp, sr.OrderId "
        SQL = " FROM SurveyResponse sr LEFT OUTER JOIN SurveyAnswer sa ON sr.ResponseId = sa.ResponseId " & _
              "WHERE sr.SurveyId = " & SurveyId

        If Not aid = String.Empty Then
            SQL &= " AND sa.ChoiceId = " & DB.Quote(aid)
        End If

        If Not F_SubmitDateLbound.Text = String.Empty Then
            SQL = SQL & Conn & "sr.CompleteDate >= " & DB.Quote(F_SubmitDateLbound.Text)
            Conn = " AND "
        End If
        If Not F_SubmitDateUbound.Text = String.Empty Then
            SQL = SQL & Conn & "sr.CompleteDate < " & DB.Quote(DateAdd("d", 1, F_SubmitDateUbound.Text))
            Conn = " AND "
        End If

        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataSet = DB.GetDataSet(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
        gvList.DataSource = res.Tables(0).DefaultView
        gvList.DataBind()
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub
End Class
