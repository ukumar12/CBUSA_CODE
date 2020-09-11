Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("CUSTOM_REBATES")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_VendorID.Datasource = VendorRow.GetList(DB, "CompanyName")
            F_VendorID.DataValueField = "VendorID"
            F_VendorID.DataTextField = "CompanyName"
            F_VendorID.Databind()
            F_VendorID.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_BuilderID.Datasource = BuilderRow.GetList(DB, "CompanyName")
            F_BuilderID.DataValueField = "BuilderID"
            F_BuilderID.DataTextField = "CompanyName"
            F_BuilderID.Databind()
            F_BuilderID.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_CustomRebateProgramID.Datasource = CustomRebateProgramRow.GetList(DB, "ProgramName")
            F_CustomRebateProgramID.DataValueField = "CustomRebateProgramID"
            F_CustomRebateProgramID.DataTextField = "ProgramName"
            F_CustomRebateProgramID.Databind()
            F_CustomRebateProgramID.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_VendorID.SelectedValue = Request("F_VendorID")
            F_BuilderID.SelectedValue = Request("F_BuilderID")
            F_CustomRebateProgramID.SelectedValue = Request("F_CustomRebateProgramID")
            F_RebateYearLBound.Text = Request("F_RebateYearLBound")
            F_RebateYearUBound.Text = Request("F_RebateYearUBound")
            F_RebateQuarterLBound.Text = Request("F_RebateQuarterLBound")
            F_RebateQuarterUBound.Text = Request("F_RebateQuarterUBound")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "CustomRebateID"

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
        SQL = " FROM CustomRebate  "

        If Not F_VendorID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "VendorID = " & DB.Quote(F_VendorID.SelectedValue)
            Conn = " AND "
        End If
        If Not F_BuilderID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "BuilderID = " & DB.Quote(F_BuilderID.SelectedValue)
            Conn = " AND "
        End If
        If Not F_CustomRebateProgramID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "CustomRebateProgramID = " & DB.Quote(F_CustomRebateProgramID.SelectedValue)
            Conn = " AND "
        End If
        If Not F_RebateYearLBound.Text = String.Empty Then
            SQL = SQL & Conn & "RebateYear >= " & DB.Number(F_RebateYearLBound.Text)
            Conn = " AND "
        End If
        If Not F_RebateYearUBound.Text = String.Empty Then
            SQL = SQL & Conn & "RebateYear <= " & DB.Number(F_RebateYearUBound.Text)
            Conn = " AND "
        End If
        If Not F_RebateQuarterLBound.Text = String.Empty Then
            SQL = SQL & Conn & "RebateQuarter >= " & DB.Number(F_RebateQuarterLBound.Text)
            Conn = " AND "
        End If
        If Not F_RebateQuarterUBound.Text = String.Empty Then
            SQL = SQL & Conn & "RebateQuarter <= " & DB.Number(F_RebateQuarterUBound.Text)
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

