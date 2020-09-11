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
			F_HowHeard.Text = Request("F_HowHeard")
			F_IsUserInput.Text = Request("F_IsUserInput")
			F_UserInputLabel.Text = Request("F_UserInputLabel")

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

		SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
		SQL = " FROM HowHeard  "

		If Not F_HowHeard.Text = String.Empty Then
			SQL = SQL & Conn & "HowHeard LIKE " & DB.FilterQuote(F_HowHeard.Text)
			Conn = " AND "
		End If
		If Not F_UserInputLabel.Text = String.Empty Then
			SQL = SQL & Conn & "UserInputLabel LIKE " & DB.FilterQuote(F_UserInputLabel.Text)
			Conn = " AND "
		End If
		If Not F_IsUserInput.SelectedValue = String.Empty Then
			SQL = SQL & Conn & "IsUserInput  = " & DB.Number(F_IsUserInput.SelectedValue)
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