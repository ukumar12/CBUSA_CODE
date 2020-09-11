Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
	Inherits AdminPage

	Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		CheckAccess("PURCHASES_REPORTS")

		gvList.BindList = AddressOf BindList
		If Not IsPostBack Then
			F_BuilderID.Datasource = BuilderRow.GetList(DB,"CompanyName")
			F_BuilderID.DataValueField = "BuilderID"
			F_BuilderID.DataTextField = "CompanyName"
			F_BuilderID.Databind
			F_BuilderID.Items.Insert(0, New ListItem("-- ALL --",""))
	
			F_BuilderID.SelectedValue = Request("F_BuilderID")
			F_PeriodYearLBound.Text = Request("F_PeriodYearLBound")
			F_PeriodYearUBound.Text = Request("F_PeriodYearUBound")
			F_PeriodQuarterLBound.Text = Request("F_PeriodQuarterLBound")
			F_PeriodQuarterUBound.Text = Request("F_PeriodQuarterUBound")
	
			gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
			gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "PurchasesReportID"

			BindList()
		End If
	End Sub

	Private Sub BindList()
		Dim SQLFields, SQL as String
		Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

		SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
		SQL = " FROM PurchasesReport  "

		if not F_BuilderID.SelectedValue = String.Empty then
			SQL = SQL & Conn & "BuilderID = " & DB.Quote(F_BuilderID.SelectedValue)
			Conn = " AND "
		end if
		if not F_PeriodYearLBound.Text = String.Empty then
			SQL = SQL & Conn & "PeriodYear >= " & DB.Number(F_PeriodYearLBound.Text)
			Conn = " AND "
		end if
		if not F_PeriodYearUBound.Text = String.Empty then
			SQL = SQL & Conn & "PeriodYear <= " & DB.Number(F_PeriodYearUBound.Text)
			Conn = " AND "
		end if
		if not F_PeriodQuarterLBound.Text = String.Empty then
			SQL = SQL & Conn & "PeriodQuarter >= " & DB.Number(F_PeriodQuarterLBound.Text)
			Conn = " AND "
		end if
		if not F_PeriodQuarterUBound.Text = String.Empty then
			SQL = SQL & Conn & "PeriodQuarter <= " & DB.Number(F_PeriodQuarterUBound.Text)
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
