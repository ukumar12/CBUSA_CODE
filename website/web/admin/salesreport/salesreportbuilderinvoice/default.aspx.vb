Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
	Inherits AdminPage

	Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		CheckAccess("SALES_REPORTS")

		gvList.BindList = AddressOf BindList
		If Not IsPostBack Then
			F_InvoiceNumber.Text = Request("F_InvoiceNumber")
			F_InvoiceAmountLBound.Text = Request("F_InvoiceAmountLBound")
			F_InvoiceAmountUBound.Text = Request("F_InvoiceAmountUBound")
			F_InvoiceDateLBound.Text = Request("F_InvoiceDateLBound")
			F_InvoiceDateUBound.Text = Request("F_InvoiceDateUBound")
	
			gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
			gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
			If gvList.SortBy = String.Empty Then gvList.SortBy = "SalesReportBuilderInvoiceID"

			BindList()
		End If
	End Sub

	Private Sub BindList()
		Dim SQLFields, SQL as String
		Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

		SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
		SQL = " FROM SalesReportBuilderInvoice  "

		if not F_InvoiceNumber.Text = String.Empty then
			SQL = SQL & Conn & "InvoiceNumber LIKE " & DB.FilterQuote(F_InvoiceNumber.Text)
			Conn = " AND "
		end if
		if not F_InvoiceDateLBound.Text = String.Empty then
			SQL = SQL & Conn & "InvoiceDate >= " & DB.Quote(F_InvoiceDateLBound.Text)
			Conn = " AND "
		end if
		if not F_InvoiceDateUBound.Text = String.Empty then
			SQL = SQL & Conn & "InvoiceDate < " & DB.Quote(DateAdd("d", 1, F_InvoiceDateUBound.Text))
			Conn = " AND "
		end if
		if not F_InvoiceAmountLBound.Text = String.Empty then
			SQL = SQL & Conn & "InvoiceAmount >= " & DB.Number(F_InvoiceAmountLBound.Text)
			Conn = " AND "
		end if
		if not F_InvoiceAmountUBound.Text = String.Empty then
			SQL = SQL & Conn & "InvoiceAmount <= " & DB.Number(F_InvoiceAmountUBound.Text)
			Conn = " AND "
		end if
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
End Class
