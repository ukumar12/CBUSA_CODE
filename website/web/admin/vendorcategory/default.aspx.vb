Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
	Inherits AdminPage

	Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		CheckAccess("VENDOR_CATEGORYS")

		gvList.BindList = AddressOf BindList
		If Not IsPostBack Then
			F_Category.Text = Request("F_Category")
	
            'gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            'gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            'If gvList.SortBy = String.Empty Then gvList.SortBy = "SortOrder"

			BindList()
		End If
	End Sub

	Private Sub BindList()
		Dim SQLFields, SQL as String
		Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

		SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
		SQL = " FROM VendorCategory  "

		if not F_Category.Text = String.Empty then
			SQL = SQL & Conn & "Category LIKE " & DB.FilterQuote(F_Category.Text)
			Conn = " AND "
		end if
		gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)
				
        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY Category")
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
