Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Admin_Survey_Question_Default
    Inherits AdminPage

    Protected PageId As Integer
    Protected SurveyId As Integer

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("SURVEY")

        PageId = Convert.ToInt32(Request("PageId"))
        SurveyId = Convert.ToInt32(Request("SurveyId"))

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
        SQL = "      FROM SurveyQuestion  "
        SQL &= "     WHERE PageId = " & PageId

        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataSet = DB.GetDataSet(SQLFields & SQL & " ORDER BY SortOrder")
        gvList.DataSource = res.Tables(0).DefaultView
        gvList.DataBind()
    End Sub

    Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
        Response.Redirect("edit.aspx?SurveyId=" & SurveyId & "&PageId=" & PageId & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    
    Protected Sub BackToPageList_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BackToPageList.Click
        Response.Redirect("/admin/surveys/page/default.aspx?SurveyId=" & SurveyId)


    End Sub
End Class
