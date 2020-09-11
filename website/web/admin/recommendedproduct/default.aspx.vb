Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
	Inherits AdminPage

	Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		CheckAccess("RECOMMENDED_PRODUCTS")

		gvList.BindList = AddressOf BindList
		If Not IsPostBack Then
			F_ProductTypeID.Datasource = ProductTypeRow.GetList(DB,"ProductType")
			F_ProductTypeID.DataValueField = "ProductTypeID"
			F_ProductTypeID.DataTextField = "ProductType"
			F_ProductTypeID.Databind
			F_ProductTypeID.Items.Insert(0, New ListItem("-- ALL --",""))
	
			F_RecommendedProduct.Text = Request("F_RecommendedProduct")
			F_ProductTypeID.SelectedValue = Request("F_ProductTypeID")
			F_ManufacturerIDLBound.Text = Request("F_ManufacturerIDLBound")
			F_ManufacturerIDUBound.Text = Request("F_ManufacturerIDUBound")
	
			gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
			gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
			If gvList.SortBy = String.Empty Then gvList.SortBy = "RecommendedProductID"

			BindList()
		End If
	End Sub

	Private Sub BindList()
		Dim SQLFields, SQL as String
		Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

		SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
		SQL = " FROM RecommendedProduct  "

		if not F_ProductTypeID.SelectedValue = String.Empty then
			SQL = SQL & Conn & "ProductTypeID = " & DB.Quote(F_ProductTypeID.SelectedValue)
			Conn = " AND "
		end if
		if not F_RecommendedProduct.Text = String.Empty then
			SQL = SQL & Conn & "RecommendedProduct LIKE " & DB.FilterQuote(F_RecommendedProduct.Text)
			Conn = " AND "
		end if
		if not F_ManufacturerIDLBound.Text = String.Empty then
			SQL = SQL & Conn & "ManufacturerID >= " & DB.Number(F_ManufacturerIDLBound.Text)
			Conn = " AND "
		end if
		if not F_ManufacturerIDUBound.Text = String.Empty then
			SQL = SQL & Conn & "ManufacturerID <= " & DB.Number(F_ManufacturerIDUBound.Text)
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
