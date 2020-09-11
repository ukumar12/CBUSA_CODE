Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("PROJECTS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_BuilderID.Datasource = BuilderRow.GetList(DB, "CompanyName")
            F_BuilderID.DataValueField = "BuilderID"
            F_BuilderID.DataTextField = "CompanyName"
            F_BuilderID.Databind()
            F_BuilderID.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_State.DataSource = StateRow.GetStateList(DB)
            F_State.DataValueField = "StateCode"
            F_State.DataTextField = "StateName"
            F_State.Databind()
            F_State.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_PortfolioID.Datasource = PortfolioRow.GetList(DB, "Portfolio")
            F_PortfolioID.DataValueField = "PortfolioID"
            F_PortfolioID.DataTextField = "Portfolio"
            F_PortfolioID.Databind()
            F_PortfolioID.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_ProjectStatusID.Datasource = ProjectStatusRow.GetList(DB, "ProjectStatus")
            F_ProjectStatusID.DataValueField = "ProjectStatusID"
            F_ProjectStatusID.DataTextField = "ProjectStatus"
            F_ProjectStatusID.Databind()
            F_ProjectStatusID.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_ProjectName.Text = Request("F_ProjectName")
            F_LotNumber.Text = Request("F_LotNumber")
            F_Subdivision.Text = Request("F_Subdivision")
            F_City.Text = Request("F_City")
            F_Zip.Text = Request("F_Zip")
            F_County.Text = Request("F_County")
            F_BuilderID.SelectedValue = Request("F_BuilderID")
            F_State.SelectedValue = Request("F_State")
            F_PortfolioID.SelectedValue = Request("F_PortfolioID")
            F_ProjectStatusID.SelectedValue = Request("F_ProjectStatusID")
            F_StartDateLBound.Text = Request("F_StartDateLBound")
            F_StartDateUBound.Text = Request("F_StartDateUBound")
            F_SubmittedLBound.Text = Request("F_SubmittedLBound")
            F_SubmittedUBound.Text = Request("F_SubmittedUBound")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "ProjectID"

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " p.*, b.CompanyName as BuilderName "
        SQL = " FROM Project p left outer join Builder b on p.BuilderId=b.BuilderId  "

        If Not F_Zip.Text = String.Empty Then
            SQL = SQL & Conn & "Zip = " & DB.Quote(F_Zip.Text)
            Conn = " AND "
        End If
        If Not F_BuilderID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "BuilderID = " & DB.Quote(F_BuilderID.SelectedValue)
            Conn = " AND "
        End If
        If Not F_State.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "State = " & DB.Quote(F_State.SelectedValue)
            Conn = " AND "
        End If
        If Not F_PortfolioID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "PortfolioID = " & DB.Quote(F_PortfolioID.SelectedValue)
            Conn = " AND "
        End If
        If Not F_ProjectStatusID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "ProjectStatusID = " & DB.Quote(F_ProjectStatusID.SelectedValue)
            Conn = " AND "
        End If
        If Not F_ProjectName.Text = String.Empty Then
            SQL = SQL & Conn & "ProjectName LIKE " & DB.FilterQuote(F_ProjectName.Text)
            Conn = " AND "
        End If
        If Not F_LotNumber.Text = String.Empty Then
            SQL = SQL & Conn & "LotNumber LIKE " & DB.FilterQuote(F_LotNumber.Text)
            Conn = " AND "
        End If
        If Not F_Subdivision.Text = String.Empty Then
            SQL = SQL & Conn & "Subdivision LIKE " & DB.FilterQuote(F_Subdivision.Text)
            Conn = " AND "
        End If
        If Not F_City.Text = String.Empty Then
            SQL = SQL & Conn & "City LIKE " & DB.FilterQuote(F_City.Text)
            Conn = " AND "
        End If
        If Not F_County.Text = String.Empty Then
            SQL = SQL & Conn & "County LIKE " & DB.FilterQuote(F_County.Text)
            Conn = " AND "
        End If
        If Not F_StartDateLBound.Text = String.Empty Then
            SQL = SQL & Conn & "StartDate >= " & DB.Quote(F_StartDateLBound.Text)
            Conn = " AND "
        End If
        If Not F_StartDateUBound.Text = String.Empty Then
            SQL = SQL & Conn & "StartDate < " & DB.Quote(DateAdd("d", 1, F_StartDateUBound.Text))
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

