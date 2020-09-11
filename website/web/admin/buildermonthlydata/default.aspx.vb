Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.IO

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BUILDERS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_LLC.Datasource = LLCRow.GetList(DB, "LLC")
            F_LLC.DataValueField = "LLCID"
            F_LLC.DataTextField = "LLC"
            F_LLC.Databind()
            F_LLC.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_Builder.Datasource = BuilderRow.GetList(DB, "CompanyName")
            F_Builder.DataValueField = "BuilderID"
            F_Builder.DataTextField = "CompanyName"
            F_Builder.Databind()
            F_Builder.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_LLC.SelectedValue = Request("F_LLC")
            F_Builder.SelectedValue = Request("F_Builder")
            F_TimePeriodDateLBound.Text = Request("F_TimePeriodDateLBound")
            F_TimePeriodDateUBound.Text = Request("F_TimePeriodDateUBound")
            F_StartedUnitsLBound.Text = Request("F_StartedUnitsLBound")
            F_StartedUnitsUBound.Text = Request("F_StartedUnitsUBound")
            F_SoldUnitsLBound.Text = Request("F_SoldUnitsLBound")
            F_SoldUnitsUBound.Text = Request("F_SoldUnitsUBound")
            F_ClosingUnitsLBound.Text = Request("F_ClosingUnitsLBound")
            F_ClosingUnitsUBound.Text = Request("F_ClosingUnitsUBound")
            F_UnsoldUnitsLBound.Text = Request("F_UnsoldUnitsLBound")
            F_UnsoldUnitsUBound.Text = Request("F_UnsoldUnitsUBound")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            gvList.PageIndex = IIf(Request("F_pg") = String.Empty, 0, Core.ProtectParam(Request("F_pg")))

            If gvList.SortBy = String.Empty Then gvList.SortBy = "TimePeriodDate"

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder
        ViewState("F_pg") = gvList.PageIndex.ToString

        SQLFields = "SELECT * "
        SQL = " FROM BuilderMonthlyData  "

        If Not F_LLC.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "LLCID = " & DB.Quote(F_LLC.SelectedValue)
            Conn = " AND "
        End If
        If Not F_Builder.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "BuilderID = " & DB.Quote(F_Builder.SelectedValue)
            Conn = " AND "
        End If
        If Not F_TimePeriodDateLBound.Text = String.Empty Then
            SQL = SQL & Conn & "TimePeriodDate >= " & DB.Quote(F_TimePeriodDateLBound.Text)
            Conn = " AND "
        End If
        If Not F_TimePeriodDateUBound.Text = String.Empty Then
            SQL = SQL & Conn & "TimePeriodDate < " & DB.Quote(DateAdd("d", 1, F_TimePeriodDateUBound.Text))
            Conn = " AND "
        End If
        If Not F_StartedUnitsLBound.Text = String.Empty Then
            SQL = SQL & Conn & "StartedUnits >= " & DB.Number(F_StartedUnitsLBound.Text)
            Conn = " AND "
        End If
        If Not F_StartedUnitsUBound.Text = String.Empty Then
            SQL = SQL & Conn & "StartedUnits <= " & DB.Number(F_StartedUnitsUBound.Text)
            Conn = " AND "
        End If
        If Not F_SoldUnitsLBound.Text = String.Empty Then
            SQL = SQL & Conn & "SoldUnits >= " & DB.Number(F_SoldUnitsLBound.Text)
            Conn = " AND "
        End If
        If Not F_SoldUnitsUBound.Text = String.Empty Then
            SQL = SQL & Conn & "SoldUnits <= " & DB.Number(F_SoldUnitsUBound.Text)
            Conn = " AND "
        End If
        If Not F_ClosingUnitsLBound.Text = String.Empty Then
            SQL = SQL & Conn & "ClosingUnits >= " & DB.Number(F_ClosingUnitsLBound.Text)
            Conn = " AND "
        End If
        If Not F_ClosingUnitsUBound.Text = String.Empty Then
            SQL = SQL & Conn & "ClosingUnits <= " & DB.Number(F_ClosingUnitsUBound.Text)
            Conn = " AND "
        End If
        If Not F_UnsoldUnitsLBound.Text = String.Empty Then
            SQL = SQL & Conn & "UnsoldUnits >= " & DB.Number(F_UnsoldUnitsLBound.Text)
            Conn = " AND "
        End If
        If Not F_UnsoldUnitsUBound.Text = String.Empty Then
            SQL = SQL & Conn & "UnsoldUnits <= " & DB.Number(F_UnsoldUnitsUBound.Text)
            Conn = " AND "
        End If
        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub

    Private Sub btnExport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExport.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()

        Response.Clear()
        'Response.Buffer = True
        Response.ContentType = "application/vnd.ms-excel"

    End Sub
End Class

