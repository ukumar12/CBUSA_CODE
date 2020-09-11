Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("MARKETING_TOOLS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_ClickDateLBound.Text = Request("F_ClickDateLBound")
            F_ClickDateUBound.Text = Request("F_ClickDateUBound")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "ClickId"

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQL As String
        Dim WhereClick As String = String.Empty, WhereOrder As String = String.Empty

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        If Not F_ClickDateLbound.Text = String.Empty Then
            WhereClick &= " and ClickDate >= " & DB.Quote(F_ClickDateLbound.Text)
            WhereOrder &= " and ProcessDate >= " & DB.Quote(F_ClickDateLbound.Text)
        End If
        If Not F_ClickDateUbound.Text = String.Empty Then
            WhereClick &= " and ClickDate < " & DB.Quote(DateAdd("d", 1, F_ClickDateUbound.Text))
            WhereOrder &= " and ProcessDate < " & DB.Quote(DateAdd("d", 1, F_ClickDateUbound.Text))
        End If

        SQL = " SELECT *, CASE WHEN HOWMANY = 0 THEN 0 ELSE ROUND(NofOrders * 100 / Howmany,2) END AS Conversion FROM (" _
          & " SELECT r.*," _
          & " COALESCE((SELECT COUNT(*) FROM ReferralClick WHERE Code = r.Code " & WhereClick & "),0) AS Howmany," _
          & " COALESCE((SELECT COUNT(*) FROM StoreOrder so WHERE ProcessDate IS NOT NULL AND so.ReferralCode = r.Code " & WhereOrder & "),0) AS NofOrders," _
          & " COALESCE((SELECT SUM(Total) FROM StoreOrder so WHERE ProcessDate IS NOT NULL AND so.ReferralCode = r.Code " & WhereOrder & "),0) AS Total," _
          & " COALESCE((SELECT AVG(Total) FROM StoreOrder so WHERE ProcessDate IS NOT NULL AND so.ReferralCode = r.Code " & WhereOrder & "),0) AS AvgOrder " _
          & " FROM Referral r ) as tmp ORDER BY Code ASC"

        Dim res As DataTable = DB.GetDataTable(SQL)
        gvList.Pager.NofRecords = res.Rows.Count
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
End Class

