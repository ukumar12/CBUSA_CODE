Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
	Inherits AdminPage

	Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		CheckAccess("FAQ")

		gvList.BindList = AddressOf BindList
		If Not IsPostBack Then
			F_AdminId.Datasource = AdminRow.GetAllAdmins(DB)
			F_AdminId.DataValueField = "AdminId"
			F_AdminId.DataTextField = "Username"
			F_AdminId.Databind()
			F_AdminId.Items.Insert(0, New ListItem("-- ALL --", ""))

			F_IsActive.Text = Request("F_IsActive")
			F_CategoryName.Text = Request("F_CategoryName")
			F_AdminId.SelectedValue = Request("F_AdminId")

			gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
			gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
			If gvList.SortBy = String.Empty Then gvList.SortBy = "SortOrder"

			BindList()
		End If
	End Sub

	Private Sub BindList()
		Dim SQLFields, SQL As String
		Dim Conn As String = " where "

		ViewState("F_SortBy") = gvList.SortBy
		ViewState("F_SortOrder") = gvList.SortOrder

		SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " f.*, a.Username "
		SQL = " FROM FaqCategory f left outer join admin a on f.adminid = a.adminid "

		If Not F_AdminId.SelectedValue = String.Empty Then
			SQL = SQL & Conn & "f.AdminId = " & DB.Quote(F_AdminId.SelectedValue)
			Conn = " AND "
		End If
		If Not F_CategoryName.Text = String.Empty Then
			SQL = SQL & Conn & "f.CategoryName LIKE " & DB.FilterQuote(F_CategoryName.Text)
			Conn = " AND "
		End If
		If Not F_IsActive.SelectedValue = String.Empty Then
			SQL = SQL & Conn & "f.IsActive  = " & DB.Number(F_IsActive.SelectedValue)
			Conn = " AND "
		End If
		gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

		Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
		gvList.DataSource = res.DefaultView
		gvList.DataBind()
	End Sub

	Protected Sub lnkAddNew_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles AddNew.Click
		Response.Redirect("edit.aspx?" & GetPageParams(FilterFieldType.All))
	End Sub

	Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
		If Not IsValid Then Exit Sub

		gvList.PageIndex = 0
		BindList()
	End Sub
End Class

