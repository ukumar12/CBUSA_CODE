Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
	Inherits AdminPage

	Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		CheckAccess("VENDOR_REGISTRATIONS")

		gvList.BindList = AddressOf BindList
		If Not IsPostBack Then
			F_VendorID.Datasource = VendorRow.GetList(DB,"CompanyName")
			F_VendorID.DataValueField = "VendorID"
			F_VendorID.DataTextField = "CompanyName"
			F_VendorID.Databind
			F_VendorID.Items.Insert(0, New ListItem("-- ALL --",""))
	
			F_RegistrationStatusID.Datasource = RegistrationStatusRow.GetList(DB,"RegistrationStatus")
			F_RegistrationStatusID.DataValueField = "RegistrationStatusID"
			F_RegistrationStatusID.DataTextField = "RegistrationStatus"
			F_RegistrationStatusID.Databind
			F_RegistrationStatusID.Items.Insert(0, New ListItem("-- ALL --",""))
	
			F_VendorID.SelectedValue = Request("F_VendorID")
			F_RegistrationStatusID.SelectedValue = Request("F_RegistrationStatusID")
            F_YearStartedLBound.Text = Request("F_YearStartedLBound")
            F_YearStartedUBound.Text = Request("F_YearStartedUBound")
            F_EmployeesLBound.Text = Request("F_EmployeesLBound")
            F_EmployeesUBound.Text = Request("F_EmployeesUBound")
            F_ApprovedLBound.Text = Request("F_ApprovedLBound")
            F_ApprovedUBound.Text = Request("F_ApprovedUBound")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "VendorRegistrationID"

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " vr.*, v.CompanyName, rs.RegistrationStatus "
        SQL = " FROM VendorRegistration vr JOIN Vendor v ON v.VendorId = vr.VendorId JOIN RegistrationStatus rs ON vr.RegistrationStatusID = rs.RegistrationStatusID "

        If Not F_VendorID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "vr.VendorID = " & DB.Quote(F_VendorID.SelectedValue)
            Conn = " AND "
        End If
        If Not F_RegistrationStatusID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "rs.RegistrationStatusID = " & DB.Quote(F_RegistrationStatusID.SelectedValue)
            Conn = " AND "
        End If
        If Not F_ApprovedLBound.Text = String.Empty Then
            SQL = SQL & Conn & "vr.Approved >= " & DB.Quote(F_ApprovedLbound.Text)
            Conn = " AND "
        End If
        If Not F_ApprovedUBound.Text = String.Empty Then
            SQL = SQL & Conn & "vr.Approved < " & DB.Quote(DateAdd("d", 1, F_ApprovedUbound.Text))
            Conn = " AND "
        End If
        If Not F_YearStartedLBound.Text = String.Empty Then
            SQL = SQL & Conn & "vr.YearStarted >= " & DB.Number(F_YearStartedLBound.Text)
            Conn = " AND "
        End If
        If Not F_YearStartedUBound.Text = String.Empty Then
            SQL = SQL & Conn & "vr.YearStarted <= " & DB.Number(F_YearStartedUBound.Text)
            Conn = " AND "
        End If
        If Not F_EmployeesLBound.Text = String.Empty Then
            SQL = SQL & Conn & "vr.Employees >= " & DB.Number(F_EmployeesLBound.Text)
            Conn = " AND "
        End If
        If Not F_EmployeesUBound.Text = String.Empty Then
            SQL = SQL & Conn & "vr.Employees <= " & DB.Number(F_EmployeesUBound.Text)
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
