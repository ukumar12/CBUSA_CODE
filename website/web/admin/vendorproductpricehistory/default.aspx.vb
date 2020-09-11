Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
	Inherits AdminPage

	Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		CheckAccess("VENDOR_PRODUCT_PRICE_HISTORYS")

		gvList.BindList = AddressOf BindList
		If Not IsPostBack Then
			F_ProductID.Datasource = ProductRow.GetList(DB,"Product")
			F_ProductID.DataValueField = "ProductID"
			F_ProductID.DataTextField = "Product"
			F_ProductID.Databind
			F_ProductID.Items.Insert(0, New ListItem("-- ALL --",""))
	
			F_VendorID.Datasource = VendorRow.GetList(DB,"CompanyName")
			F_VendorID.DataValueField = "VendorID"
			F_VendorID.DataTextField = "CompanyName"
			F_VendorID.Databind
			F_VendorID.Items.Insert(0, New ListItem("-- ALL --",""))
	
			F_VendorSKU.Text = Request("F_VendorSKU")
			F_ProductID.SelectedValue = Request("F_ProductID")
			F_VendorID.SelectedValue = Request("F_VendorID")
			F_SubmittedLBound.Text = Request("F_SubmittedLBound")
			F_SubmittedUBound.Text = Request("F_SubmittedUBound")
	
			gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
			gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
			If gvList.SortBy = String.Empty Then gvList.SortBy = "VendorProductPriceHistoryID"

			BindList()
		End If
	End Sub

	Private Sub BindList()
		Dim SQLFields, SQL as String
		Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

		SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
		SQL = " FROM VendorProductPriceHistory  "

		if not F_ProductID.SelectedValue = String.Empty then
			SQL = SQL & Conn & "ProductID = " & DB.Quote(F_ProductID.SelectedValue)
			Conn = " AND "
		end if
		if not F_VendorID.SelectedValue = String.Empty then
			SQL = SQL & Conn & "VendorID = " & DB.Quote(F_VendorID.SelectedValue)
			Conn = " AND "
		end if
		if not F_VendorSKU.Text = String.Empty then
			SQL = SQL & Conn & "VendorSKU LIKE " & DB.FilterQuote(F_VendorSKU.Text)
			Conn = " AND "
		end if
		if not F_SubmittedLBound.Text = String.Empty then
			SQL = SQL & Conn & "Submitted >= " & DB.Quote(F_SubmittedLBound.Text)
			Conn = " AND "
		end if
		if not F_SubmittedUBound.Text = String.Empty then
			SQL = SQL & Conn & "Submitted < " & DB.Quote(DateAdd("d", 1, F_SubmittedUBound.Text))
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
