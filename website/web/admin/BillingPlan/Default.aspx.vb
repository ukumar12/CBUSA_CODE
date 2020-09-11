Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BILLING_PLANS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_BillingPlan.Text = Request("F_BillingPlan")
            F_RecordState.Text = Request("F_RecordState")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "BillingPlanID"

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " WHERE "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " *, CASE RecordState WHEN 1 THEN 'Active' WHEN 0 THEN 'Inactive' ELSE 'DELETED' END AS ActiveState  "
        SQL = " FROM BillingPlan WHERE RecordState <> 3 "

        If Not F_BillingPlan.Text = String.Empty Then
            SQL = SQL & Conn & "DisplayValue LIKE " & DB.FilterQuote(F_BillingPlan.Text)
            Conn = " AND "
        End If
        If Not F_RecordState.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "RecordState = " & DB.Number(F_RecordState.SelectedValue)
            Conn = " AND "
        End If

        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()

    End Sub

    Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
        Response.Redirect("edit.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub

    Private Sub gvList_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gvList.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Or e.Row.RowType = DataControlRowType.Header Then
            e.Row.Cells.Item(5).Visible = False
        End If

        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Cells.Item(5).Width = 0
            Dim IsDefault As String = e.Row.Cells.Item(5).Text

            If IsDefault = "True" Then
                Dim clnkDelete As ConfirmLink = e.Row.FindControl("lnkDelete")
                clnkDelete.Visible = False
            End If
        End If
    End Sub

End Class
