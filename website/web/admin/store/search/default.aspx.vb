Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class admin_search_default
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("REPORTS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then
                If Me.chkIsDetails.Checked = True Then
                    gvList.SortBy = "CreateDate"
                    gvList.SortOrder = "DESC"
                Else
                    gvList.SortBy = "NumberOfSearches"
                    gvList.SortOrder = "DESC"
                End If
                
            End If

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        If Not Me.chkIsDetails.Checked Then
            SQLFields = " Term, AVG(NumberResults) AS AverageResults, count(*) AS NumberOfSearches, '' as CreateDate, '' as RemoteIP, 0 as NumberResults "

            Me.gvList.Columns.Item(0).Visible = True
            Me.gvList.Columns.Item(2).Visible = False
            Me.gvList.Columns.Item(3).Visible = False
            Me.gvList.Columns.Item(4).Visible = False
            Me.gvList.Columns.Item(5).Visible = True
            Me.gvList.Columns.Item(6).Visible = True
            Me.gvList.Columns.Item(7).Visible = False
            Me.gvList.Columns.Item(8).Visible = False
        Else
            SQLFields = " *, 0 as AverageResults, 0 as NumberOfSearches,( select OrderNo from StoreOrder o where o.ProcessDate Is not null and o.OrderId=SearchTerm.OrderId) as OrderNo,(select Firstname + ' ' + LastName from MemberAddress ma where ma.MemberId = SearchTerm.MemberId and AddressType='Billing') as MemberName  "
            Me.gvList.Columns.Item(0).Visible = False
            Me.gvList.Columns.Item(2).Visible = True
            Me.gvList.Columns.Item(3).Visible = True
            Me.gvList.Columns.Item(4).Visible = True
            Me.gvList.Columns.Item(5).Visible = False
            Me.gvList.Columns.Item(6).Visible = False
            Me.gvList.Columns.Item(7).Visible = True
            Me.gvList.Columns.Item(8).Visible = True
        End If

        SQL = " FROM SearchTerm  "

        If Not Me.dpStartDate.Text = String.Empty Then
            SQL = SQL & Conn & " CreateDate >=" & DB.Quote(Me.dpStartDate.Text & " 12:00:01 AM")
            Conn = " AND "
        End If
        If Not Me.dpEndDate.Text = String.Empty Then
            SQL = SQL & Conn & " CreateDate <= " & DB.Quote(Me.dpEndDate.Text & " 11:59:00 PM")
            Conn = " AND "
        End If
        If Not Me.txtTerm.Text = String.Empty Then
            SQL = SQL & Conn & " Term = " & DB.Quote(Me.txtTerm.Text)
            Conn = " AND "
        End If

        If Not Me.chkIsDetails.Checked Then
            SQL += " GROUP BY SearchTerm.Term "
        End If

        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) FROM (SELECT " & SQLFields & SQL & ") A")

        SQL += " ORDER BY " & gvList.SortByAndOrder

        Dim res As DataTable = DB.GetDataTable("SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " " & SQLFields & SQL)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Protected Sub btnFilter_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFilter.Click
        gvList.PageIndex = 0
        BindList()
    End Sub

    Protected Sub gvList_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvList.RowCommand
        If e.CommandName = "Details" Then
            Me.txtTerm.Text = e.CommandArgument
            Me.chkIsDetails.Checked = True
            BindList()
        End If
    End Sub

    Protected Sub gvList_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound

        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim btn As Button = e.Row.FindControl("btnDetails")
            btn.CommandArgument = e.Row.DataItem("Term")
            btn.CommandName = "Details"
            If chkIsDetails.Checked = True Then
                Dim ltlOrderNo As Literal = e.Row.FindControl("ltlOrderNo")
                Dim ltlMemberName As Literal = e.Row.FindControl("ltlMemberName")
                If Not IsDBNull(e.Row.DataItem("OrderNo")) Then ltlOrderNo.Text = "<a href=""/admin/store/orders/default.aspx?F_OrderNo=" & e.Row.DataItem("OrderNo") & """ >" & e.Row.DataItem("OrderNo") & "</a>"
                If Not IsDBNull(e.Row.DataItem("MemberName")) Then ltlMemberName.Text = "<a href=""/admin/members/view.aspx?MemberId=" & e.Row.DataItem("MemberId") & """ >" & e.Row.DataItem("MemberName") & "</a>"
            End If
        End If
    End Sub
    
End Class

