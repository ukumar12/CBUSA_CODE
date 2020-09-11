Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
	Inherits AdminPage

	Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		CheckAccess("SAMPLE_MONTHLY_STATS")

		gvList.BindList = AddressOf BindList
		If Not IsPostBack Then
			F_YearLBound.Text = Request("F_YearLBound")
			F_YearUBound.Text = Request("F_YearUBound")
			F_MonthLBound.Text = Request("F_MonthLBound")
			F_MonthUBound.Text = Request("F_MonthUBound")
			F_TimePeriodDateLBound.Text = Request("F_TimePeriodDateLBound")
			F_TimePeriodDateUBound.Text = Request("F_TimePeriodDateUBound")
	
			gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
			gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
			If gvList.SortBy = String.Empty Then gvList.SortBy = "SampleMonthlyStatID"

			BindList()
		End If
	End Sub

	Private Sub BindList()
		Dim SQLFields, SQL as String
		Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

		SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
		SQL = " FROM SampleMonthlyStat  "

		if not F_TimePeriodDateLBound.Text = String.Empty then
			SQL = SQL & Conn & "TimePeriodDate >= " & DB.Quote(F_TimePeriodDateLBound.Text)
			Conn = " AND "
		end if
		if not F_TimePeriodDateUBound.Text = String.Empty then
			SQL = SQL & Conn & "TimePeriodDate < " & DB.Quote(DateAdd("d", 1, F_TimePeriodDateUBound.Text))
			Conn = " AND "
		end if
		if not F_YearLBound.Text = String.Empty then
			SQL = SQL & Conn & "Year >= " & DB.Number(F_YearLBound.Text)
			Conn = " AND "
		end if
		if not F_YearUBound.Text = String.Empty then
			SQL = SQL & Conn & "Year <= " & DB.Number(F_YearUBound.Text)
			Conn = " AND "
		end if
		if not F_MonthLBound.Text = String.Empty then
			SQL = SQL & Conn & "Month >= " & DB.Number(F_MonthLBound.Text)
			Conn = " AND "
		end if
		if not F_MonthUBound.Text = String.Empty then
			SQL = SQL & Conn & "Month <= " & DB.Number(F_MonthUBound.Text)
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
