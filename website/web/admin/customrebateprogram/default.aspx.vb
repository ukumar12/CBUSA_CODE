Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
	Inherits AdminPage

	Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		CheckAccess("CUSTOM_REBATE_PROGRAMS")

		gvList.BindList = AddressOf BindList
		If Not IsPostBack Then
			F_VendorID.Datasource = VendorRow.GetList(DB,"CompanyName")
			F_VendorID.DataValueField = "VendorID"
			F_VendorID.DataTextField = "CompanyName"
			F_VendorID.Databind
			F_VendorID.Items.Insert(0, New ListItem("-- ALL --",""))
	
			F_ProgramName.Text = Request("F_ProgramName")
			F_VendorID.SelectedValue = Request("F_VendorID")
			F_RebateYearLBound.Text = Request("F_RebateYearLBound")
			F_RebateYearUBound.Text = Request("F_RebateYearUBound")
			F_RebateQuarterLBound.Text = Request("F_RebateQuarterLBound")
			F_RebateQuarterUBound.Text = Request("F_RebateQuarterUBound")
	
			gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
			gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
			If gvList.SortBy = String.Empty Then gvList.SortBy = "CustomRebateProgramID"

			BindList()
		End If
	End Sub

	Private Sub BindList()
		Dim SQLFields, SQL as String
		Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

		SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
		SQL = " FROM CustomRebateProgram  "

		if not F_VendorID.SelectedValue = String.Empty then
			SQL = SQL & Conn & "VendorID = " & DB.Quote(F_VendorID.SelectedValue)
			Conn = " AND "
		end if
		if not F_ProgramName.Text = String.Empty then
			SQL = SQL & Conn & "ProgramName LIKE " & DB.FilterQuote(F_ProgramName.Text)
			Conn = " AND "
		end if
		if not F_RebateYearLBound.Text = String.Empty then
			SQL = SQL & Conn & "RebateYear >= " & DB.Number(F_RebateYearLBound.Text)
			Conn = " AND "
		end if
		if not F_RebateYearUBound.Text = String.Empty then
			SQL = SQL & Conn & "RebateYear <= " & DB.Number(F_RebateYearUBound.Text)
			Conn = " AND "
		end if
		if not F_RebateQuarterLBound.Text = String.Empty then
			SQL = SQL & Conn & "RebateQuarter >= " & DB.Number(F_RebateQuarterLBound.Text)
			Conn = " AND "
		end if
		if not F_RebateQuarterUBound.Text = String.Empty then
			SQL = SQL & Conn & "RebateQuarter <= " & DB.Number(F_RebateQuarterUBound.Text)
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
