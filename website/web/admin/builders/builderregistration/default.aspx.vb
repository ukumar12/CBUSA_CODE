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
            F_BuilderID.Datasource = BuilderRow.GetList(DB, "CompanyName")
            F_BuilderID.DataValueField = "BuilderID"
            F_BuilderID.DataTextField = "CompanyName"
            F_BuilderID.Databind()
            F_BuilderID.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_RegistrationStatusID.Datasource = RegistrationStatusRow.GetList(DB, "RegistrationStatus")
            F_RegistrationStatusID.DataValueField = "RegistrationStatusID"
            F_RegistrationStatusID.DataTextField = "RegistrationStatus"
            F_RegistrationStatusID.Databind()
            F_RegistrationStatusID.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_BuilderID.SelectedValue = Request("F_BuilderID")
            F_RegistrationStatusID.SelectedValue = Request("F_RegistrationStatusID")
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

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " r.*, b.CompanyName, s.RegistrationStatus "
        SQL = " FROM BuilderRegistration r inner join Builder b on r.BuilderID=b.BuilderID " _
            & "     left outer join RegistrationStatus s on r.RegistrationStatusID=s.RegistrationStatusID"

        If Not F_BuilderID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "r.BuilderID = " & DB.Quote(F_BuilderID.SelectedValue)
            Conn = " AND "
        End If
        If Not F_RegistrationStatusID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "r.RegistrationStatusID = " & DB.Quote(F_RegistrationStatusID.SelectedValue)
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

