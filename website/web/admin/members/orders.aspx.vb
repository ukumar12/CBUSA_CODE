Imports Components
Imports DataLayer
Imports System.Data.SqlClient

Partial Class admin_members_orders
    Inherits AdminPage

    Private PageTotal As Double = 0
    Private PageSubtotal As Double = 0
    Private PageTax As Double = 0
    Private PageDiscount As Double = 0
    Private PageShipping As Double = 0

    Private GrandTotal As Double = 0
    Private GrandSubtotal As Double = 0
    Private GrandTax As Double = 0
    Private GrandDiscount As Double = 0
    Private GrandShipping As Double = 0
    Protected MemberId As Integer = 0


    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("MEMBERS")
        MemberId = Request("MemberId")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            Dim dbMember As MemberRow = MemberRow.GetRow(DB, MemberId)
            Dim dbBilling As MemberAddressRow = MemberAddressRow.GetDefaultBillingRow(DB, MemberId)
            txtMemberName.Text = "<b>" + Core.BuildFullName(dbBilling.FirstName, dbBilling.MiddleInitial, dbBilling.LastName) + " (" + dbMember.Username + ")</b>"
            gvList.SortBy = Core.ProtectParam(Request("F_SortBy2"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder2"))
            If gvList.SortBy = String.Empty Then
                gvList.SortBy = "ProcessDate"
                gvList.SortOrder = "DESC"
            End If
            lnkBack.HRef = "/admin/members/view.aspx?MemberId=" & MemberId & "&" & GetPageParams(FilterFieldType.All)
            BindList()
        End If
        Dim SQL As String = " FROM StoreOrder o where o.ProcessDate is not null "
        If MemberId <> 0 Then
            SQL &= " and o.MemberId=" & DB.Number(MemberId)
        End If
        Dim drv As DataRowView = DB.GetDataTable("select count(*) as NumOrders, coalesce(sum(subtotal),0) TotalOrder, coalesce(avg(subtotal),0) As AvgOrder " & SQL).DefaultView(0)
        ltlSummary.Text = "<b>Number Orders</b>: " & drv("NumOrders") & " &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <b>Gross Sales</b>: " & FormatCurrency(drv("TotalOrder")) & " &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <b>Avg. Order</b>: " & FormatCurrency(drv("AvgOrder"))
    End Sub
    Private Sub BindList()
        Dim SQLFields, SQL As String

        ViewState("F_SortBy2") = gvList.SortBy
        ViewState("F_SortOrder2") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " o.OrderId, o.OrderNo, BillingLastName, o.BillingLastName + ' ' + o.BillingFirstName as FullName, o.Subtotal, o.Discount, o.Tax, o.Shipping, o.Total, o.ProcessDate, (SELECT TOP 1 Name FROM StoreOrderStatus sos WHERE sos.Code = o.Status) AS Status, (SELECT TOP 1 ShippedDate FROM StoreOrderRecipient sor WHERE o.OrderId = sor.OrderId) AS ShippedDate"

        SQL = " FROM StoreOrder o where o.ProcessDate is not null "

        If MemberId <> 0 Then
            SQL &= " and o.MemberId=" & DB.Number(MemberId)
        End If

        Dim dr As SqlDataReader = DB.GetReader("select SUM(Total) as Total, SUM(Tax) as Tax, SUM(Shipping) as Shipping, SUM(Subtotal) as Subtotal, SUM(Discount) as Discount " & SQL)
        If dr.Read Then
            GrandTotal = IIf(IsDBNull(dr("Total")), 0, dr("Total"))
            GrandTax = IIf(IsDBNull(dr("Tax")), 0, dr("Tax"))
            GrandShipping = IIf(IsDBNull(dr("Shipping")), 0, dr("Shipping"))
            GrandDiscount = IIf(IsDBNull(dr("Discount")), 0, dr("Discount"))
            GrandSubtotal = IIf(IsDBNull(dr("Subtotal")), 0, dr("Subtotal"))
        End If
        dr.Close()

        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT count(*) from (select OrderNo " & SQL & ") as tmp")

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)

        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Protected Sub gvList_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            PageSubtotal += e.Row.DataItem("Subtotal")
            PageTax += e.Row.DataItem("Tax")
            PageDiscount += e.Row.DataItem("Discount")
            PageShipping += e.Row.DataItem("Shipping")
            PageTotal += e.Row.DataItem("Total")
        End If
        If e.Row.RowType = DataControlRowType.Footer Then
            e.Row.Cells(5).Text = "Page Total:<br/>Total:"
            e.Row.Cells(6).Text = FormatCurrency(PageSubtotal) & "<br />" & FormatCurrency(GrandSubtotal)
            e.Row.Cells(7).Text = FormatCurrency(PageDiscount) & "<br />" & FormatCurrency(GrandDiscount)
            e.Row.Cells(8).Text = FormatCurrency(PageTax) & "<br />" & FormatCurrency(GrandTax)
            e.Row.Cells(9).Text = FormatCurrency(PageShipping) & "<br />" & FormatCurrency(GrandShipping)
            e.Row.Cells(10).Text = FormatCurrency(PageTotal) & "<br />" & FormatCurrency(GrandTotal)
        End If
    End Sub

End Class
