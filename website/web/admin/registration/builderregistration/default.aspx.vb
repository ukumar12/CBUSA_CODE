Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
	Inherits AdminPage

	Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BUILDER_REGISTRATIONS")

		gvList.BindList = AddressOf BindList
		If Not IsPostBack Then
			F_AcceptsTerms.Text = Request("F_AcceptsTerms")
            F_YearStartedLBound.Text = Request("F_YearStartedLBound")
            F_YearStartedUBound.Text = Request("F_YearStartedUBound")
            F_EmployeesLBound.Text = Request("F_EmployeesLBound")
            F_EmployeesUBound.Text = Request("F_EmployeesUBound")
            F_HomesBuiltAndDeliveredLBound.Text = Request("F_HomesBuiltAndDeliveredLBound")
            F_HomesBuiltAndDeliveredUBound.Text = Request("F_HomesBuiltAndDeliveredUBound")
            F_HomeStartsLastYearLBound.Text = Request("F_HomeStartsLastYearLBound")
            F_HomeStartsLastYearUBound.Text = Request("F_HomeStartsLastYearUBound")
            F_HomeStartsNextYearLBound.Text = Request("F_HomeStartsNextYearLBound")
            F_HomeStartsNextYearUBound.Text = Request("F_HomeStartsNextYearUBound")
            F_AvgCostPerSqFtLBound.Text = Request("F_AvgCostPerSqFtLBound")
            F_AvgCostPerSqFtUBound.Text = Request("F_AvgCostPerSqFtUBound")
            F_RevenueLastYearLBound.Text = Request("F_RevenueLastYearLBound")
            F_RevenueLastYearUBound.Text = Request("F_RevenueLastYearUBound")
            F_RevenueNextYearLBound.Text = Request("F_RevenueNextYearLBound")
            F_RevenueNextYearUBound.Text = Request("F_RevenueNextYearUBound")
            F_SubmittedLBound.Text = Request("F_SubmittedLBound")
            F_SubmittedUBound.Text = Request("F_SubmittedUBound")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "BuilderRegistrationID"

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
        SQL = " FROM BuilderRegistration  "

        If Not F_AcceptsTerms.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "AcceptsTerms  = " & DB.Number(F_AcceptsTerms.SelectedValue)
            Conn = " AND "
        End If
        If Not F_SubmittedLBound.Text = String.Empty Then
            SQL = SQL & Conn & "Submitted >= " & DB.Quote(F_SubmittedLBound.Text)
            Conn = " AND "
        End If
        If Not F_SubmittedUBound.Text = String.Empty Then
            SQL = SQL & Conn & "Submitted < " & DB.Quote(DateAdd("d", 1, F_SubmittedUBound.Text))
            Conn = " AND "
        End If
        If Not F_YearStartedLBound.Text = String.Empty Then
            SQL = SQL & Conn & "YearStarted >= " & DB.Number(F_YearStartedLBound.Text)
            Conn = " AND "
        End If
        If Not F_YearStartedUBound.Text = String.Empty Then
            SQL = SQL & Conn & "YearStarted <= " & DB.Number(F_YearStartedUBound.Text)
            Conn = " AND "
        End If
        If Not F_EmployeesLBound.Text = String.Empty Then
            SQL = SQL & Conn & "Employees >= " & DB.Number(F_EmployeesLBound.Text)
            Conn = " AND "
        End If
        If Not F_EmployeesUBound.Text = String.Empty Then
            SQL = SQL & Conn & "Employees <= " & DB.Number(F_EmployeesUBound.Text)
            Conn = " AND "
        End If
        If Not F_HomesBuiltAndDeliveredLBound.Text = String.Empty Then
            SQL = SQL & Conn & "HomesBuiltAndDelivered >= " & DB.Number(F_HomesBuiltAndDeliveredLBound.Text)
            Conn = " AND "
        End If
        If Not F_HomesBuiltAndDeliveredUBound.Text = String.Empty Then
            SQL = SQL & Conn & "HomesBuiltAndDelivered <= " & DB.Number(F_HomesBuiltAndDeliveredUBound.Text)
            Conn = " AND "
        End If
        If Not F_HomeStartsLastYearLBound.Text = String.Empty Then
            SQL = SQL & Conn & "HomeStartsLastYear >= " & DB.Number(F_HomeStartsLastYearLBound.Text)
            Conn = " AND "
        End If
        If Not F_HomeStartsLastYearUBound.Text = String.Empty Then
            SQL = SQL & Conn & "HomeStartsLastYear <= " & DB.Number(F_HomeStartsLastYearUBound.Text)
            Conn = " AND "
        End If
        If Not F_HomeStartsNextYearLBound.Text = String.Empty Then
            SQL = SQL & Conn & "HomeStartsNextYear >= " & DB.Number(F_HomeStartsNextYearLBound.Text)
            Conn = " AND "
        End If
        If Not F_HomeStartsNextYearUBound.Text = String.Empty Then
            SQL = SQL & Conn & "HomeStartsNextYear <= " & DB.Number(F_HomeStartsNextYearUBound.Text)
            Conn = " AND "
        End If
        If Not F_AvgCostPerSqFtLBound.Text = String.Empty Then
            SQL = SQL & Conn & "AvgCostPerSqFt >= " & DB.Number(F_AvgCostPerSqFtLBound.Text)
            Conn = " AND "
        End If
        If Not F_AvgCostPerSqFtUBound.Text = String.Empty Then
            SQL = SQL & Conn & "AvgCostPerSqFt <= " & DB.Number(F_AvgCostPerSqFtUBound.Text)
            Conn = " AND "
        End If
        If Not F_RevenueLastYearLBound.Text = String.Empty Then
            SQL = SQL & Conn & "RevenueLastYear >= " & DB.Number(F_RevenueLastYearLBound.Text)
            Conn = " AND "
        End If
        If Not F_RevenueLastYearUBound.Text = String.Empty Then
            SQL = SQL & Conn & "RevenueLastYear <= " & DB.Number(F_RevenueLastYearUBound.Text)
            Conn = " AND "
        End If
        If Not F_RevenueNextYearLBound.Text = String.Empty Then
            SQL = SQL & Conn & "RevenueNextYear >= " & DB.Number(F_RevenueNextYearLBound.Text)
            Conn = " AND "
        End If
        If Not F_RevenueNextYearUBound.Text = String.Empty Then
            SQL = SQL & Conn & "RevenueNextYear <= " & DB.Number(F_RevenueNextYearUBound.Text)
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
