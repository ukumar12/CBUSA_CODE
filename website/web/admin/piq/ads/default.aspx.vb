Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage
    Public PIQID As Integer

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("PIQ")

        PIQID = Convert.ToInt32(Request("PIQID"))

        If IsDBNull(PIQID) OrElse PIQID = 0 Then Response.Redirect("/admin/piq/default.aspx?" & GetPageParams(FilterFieldType.All))

        ltrPIQ.Text = PIQRow.GetRow(DB, PIQID).CompanyName

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "PIQAdID"

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " and "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
        SQL = " FROM PIQAd WHERE PIQID = " & DB.Number(PIQID)

        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
        Response.Redirect("edit.aspx?PIQID=" & PIQID & "&" & GetPageParams(FilterFieldType.All))
    End Sub
    Private Sub btnBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBack.Click
        Response.Redirect("/admin/piq/default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

End Class

