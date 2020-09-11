Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BROADCAST")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_Name.Text = Request("F_Name")
            F_Qty.Checked = Not Request("F_Qty") = String.Empty

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "Name"

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " and "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " *, null as HtmlMembers, null as TextMembers, null as TotalMembers"
        SQL = " FROM MailingGroup where IsPermanent = 1 "

        If Not F_Name.Text = String.Empty Then
            SQL = SQL & Conn & "Name LIKE " & DB.FilterQuote(F_Name.Text)
            Conn = " AND "
        End If
        If F_Qty.Checked Then
            gvList.Columns(4).Visible = True
            gvList.Columns(5).Visible = True
            gvList.Columns(6).Visible = True
        Else
            gvList.Columns(4).Visible = False
            gvList.Columns(5).Visible = False
            gvList.Columns(6).Visible = False
        End If
        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)

        If F_Qty.Checked Then UpdateQuantities(res)

        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
        Response.Redirect("edit.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub UpdateQuantities(ByVal res As DataTable)
        For Each row As DataRow In res.Rows
            Dim col As New NameValueCollection
            Dim Lists As String = MailingGroupRow.GetSelectedMailingLists(DB, row("GroupId"))

            col("Lists") = Lists
            col("StartDate") = IIf(IsDBNull(row("StartDate")), String.Empty, row("StartDate"))
            col("EndDate") = IIf(IsDBNull(row("EndDate")), String.Empty, row("EndDate"))

            row("HtmlMembers") = DB.ExecuteScalar(MailingHelper.GetHTMLQueryCount(DB, col, "BOTH"))
            row("TextMembers") = DB.ExecuteScalar(MailingHelper.GetTextQueryCount(DB, col, "BOTH"))
            row("TotalMembers") = row("HtmlMembers") + row("TextMembers")
        Next
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub
End Class
