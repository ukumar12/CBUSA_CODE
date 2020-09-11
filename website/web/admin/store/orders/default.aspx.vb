Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.IO
Imports System.Data.SqlClient

Partial Class Index
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

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("ORDERS")

        Dim drv As DataRowView = DB.GetDataTable("select count(*) as NumOrders, coalesce(sum(subtotal),0) TotalOrder, coalesce(avg(subtotal),0) As AvgOrder " & BuildQuery()).DefaultView(0)
        ltlSummary.Text = "<b>Number Orders</b>: " & drv("NumOrders") & " &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <b>Gross Sales</b>: " & FormatCurrency(drv("TotalOrder")) & " &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <b>Avg. Order</b>: " & FormatCurrency(drv("AvgOrder"))

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_Status.DataSource = StoreOrderStatusRow.GetOrderStatusesWithSummary(DB)
            F_Status.DataValueField = "code"
            F_Status.DataTextField = "Name"
            F_Status.DataBind()
            F_Status.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_State.DataSource = StateRow.GetStateList(DB)
            F_State.DataValueField = "StateCode"
            F_State.DataTextField = "StateName"
            F_State.DataBind()
            F_State.Items.Insert(0, New ListItem("-- Please Select --", ""))


            F_OrderNo.Text = Request("F_OrderNo")
            F_ProcessDateLbound.Text = Request("F_ProcessDateLBound")
            F_ProcessDateUbound.Text = Request("F_ProcessDateUBound")
            F_Name.Text = Request("F_Name")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then
                gvList.SortBy = "ProcessDate"
                gvList.SortOrder = "DESC"
            End If

            BindList()
        End If
    End Sub

    Private Function BuildQuery() As String
        Dim Conn As String = " and "

        Dim SQL As String = " FROM StoreOrder o Left Outer Join State on o.BillingState = State.StateCode where o.ProcessDate is not null "

        If Not F_ProcessDateLbound.Text = String.Empty Then
            SQL = SQL & Conn & "o.ProcessDate >= " & DB.Quote(F_ProcessDateLbound.Text)
            Conn = " AND "
        End If
        If Not F_ProcessDateUbound.Text = String.Empty Then
            SQL = SQL & Conn & "o.ProcessDate < " & DB.Quote(DateAdd("d", 1, F_ProcessDateUbound.Text))
            Conn = " AND "
        End If
        If Not F_Status.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "o.Status=" & DB.Quote(F_Status.SelectedValue)
            Conn = " AND "
        End If
        If Not F_OrderNo.Text = String.Empty Then
            SQL = SQL & Conn & "o.OrderNo like " & DB.StartsWith(F_OrderNo.Text)
            Conn = " AND "
        End If
        If Not F_Name.Text = String.Empty Then
            SQL = SQL & Conn & "o.BillingLastName like " & DB.StartsWith(F_Name.Text)
            Conn = " AND "
        End If
        If Not F_PromotionCode.Text = String.Empty Then
            SQL = SQL & Conn & "o.PromotionCode like " & DB.StartsWith(F_PromotionCode.Text)
            Conn = " AND "
        End If
        If Not F_State.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "o.BillingState=" & DB.Quote(F_State.SelectedValue)
            Conn = " AND "
        End If
        Return SQL
    End Function

    Private Sub ExportList()
        Dim SQLFields, SQL As String

        SQLFields = "SELECT o.OrderId, cast(o.OrderNo as float) as OrderNo, o.BillingLastName, o.BillingLastName + ' ' + BillingFirstName as FullName, o.Subtotal, o.Discount, o.Tax, o.Shipping, o.Total, o.ProcessDate, (SELECT TOP 1 Name FROM StoreOrderStatus sos WHERE sos.Code = o.Status) AS Status, (SELECT TOP 1 ShippedDate FROM StoreOrderRecipient sor WHERE o.OrderId = sor.OrderId) AS ShippedDate,o.PromotionCode,StateName "
        SQL = BuildQuery()

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)

        Dim Folder As String = "/assets/orders/"
        Dim i As Integer = 0
        Dim FileName As String = Core.GenerateFileID & ".csv"

        Dim sw As StreamWriter = New StreamWriter(Server.MapPath(Folder & FileName), False)
        sw.WriteLine("Web Orders Report")
        sw.WriteLine("Report generated on " & DateTime.Now.ToString("d"))
        sw.WriteLine(String.Empty)
        sw.WriteLine("Search Criteria")
        sw.WriteLine("Order Date From:," & F_ProcessDateLbound.Text)
        sw.WriteLine("Order Date To:," & F_ProcessDateUbound.Text)
        sw.WriteLine("Status:," & F_Status.SelectedItem.Text)
        sw.WriteLine("Customer:," & F_Name.Text)
        sw.WriteLine("Order#:," & F_OrderNo.Text)
        sw.WriteLine("Promotion Code:," & F_PromotionCode.Text)
        sw.WriteLine("State:," & F_State.SelectedItem.Text)
        sw.WriteLine(String.Empty)
        sw.WriteLine(String.Empty)

        sw.WriteLine("Order#,Full Name,Process Date,Shipped Date,Subtotal,Discount,Tax,Shipping,Total,Status,Promotion Code, State")
        For Each dr As DataRow In res.Rows
            Dim OrderNo As String = IIf(IsDBNull(dr("OrderNo")), String.Empty, dr("OrderNo"))
            Dim Status As String = IIf(IsDBNull(dr("Status")), String.Empty, dr("Status"))
            Dim FullName As String = IIf(IsDBNull(dr("FullName")), String.Empty, dr("FullName"))
            Dim Total As String = IIf(IsDBNull(dr("Total")), String.Empty, dr("Total"))
            Dim Discount As String = IIf(IsDBNull(dr("Discount")), String.Empty, dr("Discount"))
            Dim Subtotal As String = IIf(IsDBNull(dr("Subtotal")), String.Empty, dr("Subtotal"))
            Dim Shipping As String = IIf(IsDBNull(dr("Shipping")), String.Empty, dr("Shipping"))
            Dim Tax As String = IIf(IsDBNull(dr("Tax")), String.Empty, dr("Tax"))

            Dim ProcessDate As String = IIf(IsDBNull(dr("ProcessDate")), String.Empty, dr("ProcessDate"))
            Dim ShippedDate As String = IIf(IsDBNull(dr("ShippedDate")), String.Empty, dr("ShippedDate"))
            Dim PromotionCode As String = IIf(IsDBNull(dr("PromotionCode")), String.Empty, dr("PromotionCode"))
            Dim State As String = IIf(IsDBNull(dr("StateName")), String.Empty, dr("StateName"))
            sw.WriteLine(Core.QuoteCSV(OrderNo) & "," & Core.QuoteCSV(FullName) & _
                         "," & Core.QuoteCSV(ProcessDate) & "," & Core.QuoteCSV(ShippedDate) & _
                         "," & Core.QuoteCSV(Subtotal) & "," & Core.QuoteCSV(Discount) & "," & Core.QuoteCSV(Tax) & "," & Core.QuoteCSV(Shipping) & "," & Core.QuoteCSV(Total) & "," & Core.QuoteCSV(Status) & "," & Core.QuoteCSV(PromotionCode) & "," & Core.QuoteCSV(State))
        Next
        sw.Flush()
        sw.Close()

        lnkDownload.NavigateUrl = Folder & FileName
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " o.OrderId, o.OrderNo, BillingLastName, o.BillingLastName + ' ' + o.BillingFirstName as FullName,o.MemberId, o.Subtotal, o.Discount, o.Tax, o.Shipping, o.Total, o.ProcessDate, (SELECT TOP 1 Name FROM StoreOrderStatus sos WHERE sos.Code = o.Status) AS Status, (SELECT TOP 1 ShippedDate FROM StoreOrderRecipient sor WHERE o.OrderId = sor.OrderId) AS ShippedDate, (Select IsHighRisk from PaymentLog pl, StoreOrder sor where sor.OrderNo=pl.OrderNo and pl.OrderNo =o.OrderNo) as IsHighRisk,o.PromotionCode,StateName"

        SQL = BuildQuery()

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

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0

        If F_OutputAs.SelectedValue = "Excel" Then
            divDownload.Visible = True
            gvList.Visible = False
            ExportList()
        Else
            divDownload.Visible = False
            gvList.Visible = True
            BindList()
        End If
    End Sub

    Protected Sub gvList_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            PageSubtotal += e.Row.DataItem("Subtotal")
            PageTax += e.Row.DataItem("Tax")
            PageDiscount += e.Row.DataItem("Discount")
            PageShipping += e.Row.DataItem("Shipping")
            PageTotal += e.Row.DataItem("Total")
            Dim lblOrderId As Label = e.Row.FindControl("lblOrderId")
            If Not IsDBNull(e.Row.DataItem("IsHighRisk")) Then
                If Convert.ToBoolean(e.Row.DataItem("IsHighRisk")) = True Then
                    lblOrderId.Text = "<div style=""background-color:#FF0000; color: #FFFFFF"">" & e.Row.DataItem("OrderNo") & "</div>"
                Else
                    lblOrderId.Text = e.Row.DataItem("OrderNo")
                End If
            End If
            Dim ltlCustomerName As Literal = e.Row.FindControl("ltlCustomerName")
            If Not IsDBNull(e.Row.DataItem("MemberId")) Then
                ltlCustomerName.Text = "<a href=/admin/members/view.aspx?MemberId=" & e.Row.DataItem("MemberId") & ">" & e.Row.DataItem("FullName") & "</a>"
            Else
                ltlCustomerName.Text = e.Row.DataItem("FullName")
            End If
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

