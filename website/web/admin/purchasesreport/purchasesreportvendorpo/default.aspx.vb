Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
	Inherits AdminPage

	Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		CheckAccess("PURCHASES_REPORT_VENDOR_POS")

		gvList.BindList = AddressOf BindList
		If Not IsPostBack Then
			F_VendorID.Datasource = VendorRow.GetList(DB,"CompanyName")
			F_VendorID.DataValueField = "VendorID"
			F_VendorID.DataTextField = "CompanyName"
			F_VendorID.Databind
			F_VendorID.Items.Insert(0, New ListItem("-- ALL --",""))
	
			F_PONumber.Text = Request("F_PONumber")
			F_VendorID.SelectedValue = Request("F_VendorID")
			F_POAmountLBound.Text = Request("F_POAmountLBound")
			F_POAmountUBound.Text = Request("F_POAmountUBound")
			F_PODateLBound.Text = Request("F_PODateLBound")
			F_PODateUBound.Text = Request("F_PODateUBound")
			F_CreatedLBound.Text = Request("F_CreatedLBound")
			F_CreatedUBound.Text = Request("F_CreatedUBound")
	
			gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
			gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
			If gvList.SortBy = String.Empty Then gvList.SortBy = "PurchasesReportVendorPOID"

			BindList()
		End If
	End Sub

	Private Sub BindList()
		Dim SQLFields, SQL as String
		Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

		SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
		SQL = " FROM PurchasesReportVendorPO  "

		if not F_VendorID.SelectedValue = String.Empty then
			SQL = SQL & Conn & "VendorID = " & DB.Quote(F_VendorID.SelectedValue)
			Conn = " AND "
		end if
		if not F_PONumber.Text = String.Empty then
			SQL = SQL & Conn & "PONumber LIKE " & DB.FilterQuote(F_PONumber.Text)
			Conn = " AND "
		end if
		if not F_PODateLBound.Text = String.Empty then
			SQL = SQL & Conn & "PODate >= " & DB.Quote(F_PODateLBound.Text)
			Conn = " AND "
		end if
		if not F_PODateUBound.Text = String.Empty then
			SQL = SQL & Conn & "PODate < " & DB.Quote(DateAdd("d", 1, F_PODateUBound.Text))
			Conn = " AND "
		end if
		if not F_CreatedLBound.Text = String.Empty then
			SQL = SQL & Conn & "Created >= " & DB.Quote(F_CreatedLBound.Text)
			Conn = " AND "
		end if
		if not F_CreatedUBound.Text = String.Empty then
			SQL = SQL & Conn & "Created < " & DB.Quote(DateAdd("d", 1, F_CreatedUBound.Text))
			Conn = " AND "
		end if
		if not F_POAmountLBound.Text = String.Empty then
			SQL = SQL & Conn & "POAmount >= " & DB.Number(F_POAmountLBound.Text)
			Conn = " AND "
		end if
		if not F_POAmountUBound.Text = String.Empty then
			SQL = SQL & Conn & "POAmount <= " & DB.Number(F_POAmountUBound.Text)
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
