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
			F_Name.Text = Request("F_Name")
			F_SKU.Text = Request("F_SKU")
			F_PriceLBound.Text = Request("F_PriceLBound")
			F_PriceUBound.Text = Request("F_PriceUBound")
			F_WeightLBound.Text = Request("F_WeightLBound")
			F_WeightUBound.Text = Request("F_WeightUBound")

			gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
			gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
			If gvList.SortBy = String.Empty Then gvList.SortBy = "SwatchId"

			BindList()
		End If
	End Sub

	Private Sub BindList()
		Dim SQLFields, SQL As String
		Dim Conn As String = " where "

		ViewState("F_SortBy") = gvList.SortBy
		ViewState("F_SortOrder") = gvList.SortOrder

		SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
		SQL = " FROM LookupSwatch  "

		If Not F_Name.Text = String.Empty Then
			SQL = SQL & Conn & "Name LIKE " & DB.FilterQuote(F_Name.Text)
			Conn = " AND "
		End If
		If Not F_SKU.Text = String.Empty Then
			SQL = SQL & Conn & "SKU LIKE " & DB.FilterQuote(F_SKU.Text)
			Conn = " AND "
		End If
		If Not F_PriceLBound.Text = String.Empty Then
			SQL = SQL & Conn & "Price >= " & DB.Number(F_PriceLBound.Text)
			Conn = " AND "
		End If
		If Not F_PriceUBound.Text = String.Empty Then
			SQL = SQL & Conn & "Price <= " & DB.Number(F_PriceUBound.Text)
			Conn = " AND "
		End If
		If Not F_WeightLBound.Text = String.Empty Then
			SQL = SQL & Conn & "Weight >= " & DB.Number(F_WeightLBound.Text)
			Conn = " AND "
		End If
		If Not F_WeightUBound.Text = String.Empty Then
			SQL = SQL & Conn & "Weight <= " & DB.Number(F_WeightUBound.Text)
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
End Class

