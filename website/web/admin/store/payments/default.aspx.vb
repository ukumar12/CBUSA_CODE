Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("STORE")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_TransactionNo.Text = Request("F_TransactionNo")
            F_OrderNo.Text = Request("F_OrderNo")
            F_Result.Text = Request("F_Result")
            F_CreateDateLbound.Text = Request("F_CreateDateLBound")
            F_CreateDateUBound.Text = Request("F_CreateDateUBound")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "CreateDate"
            If gvList.SortOrder = String.Empty Then gvList.SortOrder = "DESC"

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " coalesce((select top 1 OrderNo from StoreOrder where OrderNo = pl.OrderNo and ProcessDate is not null),'N/A') as StoreOrderOrderNo , pl.* "
        SQL = " FROM PaymentLog pl  "

        If Not F_TransactionNo.Text = String.Empty Then
            SQL = SQL & Conn & "TransactionNo LIKE " & DB.FilterQuote(F_TransactionNo.Text)
            Conn = " AND "
        End If
        If Not F_OrderNo.Text = String.Empty Then
            SQL = SQL & Conn & "OrderNo = " & DB.Quote(F_OrderNo.Text)
            Conn = " AND "
        End If
        If Not F_CreateDateLBound.Text = String.Empty Then
            SQL = SQL & Conn & "CreateDate >= " & DB.Quote(F_CreateDateLBound.Text)
            Conn = " AND "
        End If
        If Not F_CreateDateUBound.Text = String.Empty Then
            SQL = SQL & Conn & "CreateDate < " & DB.Quote(DateAdd("d", 1, F_CreateDateUBound.Text))
            Conn = " AND "
        End If
        If Not F_Result.SelectedValue = String.Empty Then
            If F_Result.SelectedValue = "0" Then
                SQL = SQL & Conn & "Result = 0"
            Else
                SQL = SQL & Conn & "Result <> 0"
            End If
            Conn = " AND "
        End If
        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub

    Protected Sub gvList_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim ltlOrder As Literal = CType(e.Row.FindControl("ltlOrder"), Literal)
            Dim ltlDescription As Literal = CType(e.Row.FindControl("ltlDescription"), Literal)
            Dim ltlResponse As Literal = CType(e.Row.FindControl("ltlResponse"), Literal)

            ltlOrder.Text = e.Row.DataItem("StoreOrderOrderNo")
            If ltlOrder.Text <> "N/A" Then ltlOrder.Text = "<a href=""/admin/store/orders/default.aspx?F_OrderNo=" & ltlOrder.Text & """>" & ltlOrder.Text & "</a>"
            ltlDescription.Text = Replace(e.Row.DataItem("Description"), vbCrLf, "<br />")
            ltlResponse.Text = Replace(e.Row.DataItem("Response"), vbCrLf, "<br />")
        End If
    End Sub
End Class
