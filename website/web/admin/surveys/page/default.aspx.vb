Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Admin_Survey_Page_Default
    Inherits AdminPage

    Protected SurveyId As Integer

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("SURVEY")

        SurveyId = Convert.ToInt32(Request("SurveyId"))

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "SortOrder"

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
        SQL = " FROM SurveyPage  "
        SQL &= " WHERE SurveyID = " & SurveyId

        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataSet = DB.GetDataSet(SQLFields & SQL & " ORDER BY SortOrder ")
        gvList.DataSource = res.Tables(0).DefaultView
        gvList.DataBind()
    End Sub

    Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
        Response.Redirect("edit.aspx?SurveyId=" & SurveyId & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub BackToSurveys_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BackToSurveys.Click
        Response.Redirect("/admin/surveys/")

    End Sub
End Class
