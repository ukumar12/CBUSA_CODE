Imports Components
Imports DataLayer

Partial Class admin_NationalContracts_Default
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "ContractID"

            BindList()
        End If
    End Sub

    Private Function GetSearchResults() As DataTable
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
        SQL = " FROM NationalContract  "

        If Not F_Title.Text = String.Empty Then
            SQL = SQL & Conn & "Title = " & DB.Quote(F_Title.Text)
            Conn = " AND "
        End If
        If Not F_Manufacturer.Text = String.Empty Then
            SQL = SQL & Conn & "Manufacturer = " & DB.Quote(F_Manufacturer.Text)
            Conn = " AND "
        End If


        If Not F_ContractTerm.Text = String.Empty Then
            SQL = SQL & Conn & "ContractTerm Like " & DB.FilterQuote(F_ContractTerm.Text)
            Conn = " AND "
        End If

        'If Not F_StartDate.Text = String.Empty Then
        '    SQL = SQL & Conn & "StartDate >= " & DB.Quote(F_StartDate.Text)
        '    Conn = " AND "
        'End If
        'If Not F_EndDate.Text = String.Empty Then
        '    SQL = SQL & Conn & "EndDate < " & DB.Quote(DateAdd("d", 1, F_EndDate.Text))
        '    Conn = " AND "
        'End If
        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Return DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
    End Function

    Private Sub BindList()
        gvList.DataSource = GetSearchResults().DefaultView
        gvList.DataBind()
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub

    Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
        Response.Redirect("edit.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    'Protected Sub gvList_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gvList.RowDataBound
    '    If e.Row.RowType <> DataControlRowType.DataRow Then Exit Sub

    '    Dim ltlStartDate As Literal = e.Row.FindControl("ltlStartDate")
    '    Dim ltlEndDate As Literal = e.Row.FindControl("ltlEndDate")

    '    ltlStartDate.Text = Core.GetDate(e.Row.DataItem("StartDate")).ToString("M/d/yy")
    '    ltlEndDate.Text = Core.GetDate(e.Row.DataItem("EndDate")).ToString("M/d/yy")
    'End Sub

    Protected Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        Response.Clear()
        Response.ContentType = "text/csv"
        Response.AppendHeader("Content-Disposition", String.Format("attachment; filename=BuilderContracts-{0}.csv", DateTime.Now.ToString("M d yy")))
        Response.Write(BuildList)
        Context.Response.End()
    End Sub

    Protected Function BuildList() As String
        Dim ret As String = "Contract, LLC, Builder Company Name" & Environment.NewLine
        Dim SQL As String = "SELECT * FROM NationalContractBuilder ncb INNER JOIN NationalContract nc ON nc.ContractID = ncb.ContractID"

        Dim Conn As String = " where "
        If Not F_Title.Text = String.Empty Then
            SQL = SQL & Conn & "nc.Title = " & DB.Quote(F_Title.Text)
            Conn = " AND "
        End If
        If Not F_Manufacturer.Text = String.Empty Then
            SQL = SQL & Conn & "nc.Manufacturer = " & DB.Quote(F_Manufacturer.Text)
            Conn = " AND "
        End If
        'If Not F_StartDate.Text = String.Empty Then
        '    SQL = SQL & Conn & "nc.StartDate >= " & DB.Quote(F_StartDate.Text)
        '    Conn = " AND "
        'End If
        'If Not F_EndDate.Text = String.Empty Then
        '    SQL = SQL & Conn & "nc.EndDate < " & DB.Quote(DateAdd("d", 1, F_EndDate.Text))
        '    Conn = " AND "
        'End If

        For Each dr As DataRow In DB.GetDataTable(SQL).Rows
            Dim b As BuilderRow = BuilderRow.GetRow(DB, dr("BuilderID"))
            Dim contract As String = Core.GetString(dr("Title"))
            Dim LLC As String = LLCRow.GetRow(DB, b.LLCID).LLC
            Dim BuilderCompay As String = b.CompanyName

            ret &= """" & contract & """,""" & LLC & """,""" & BuilderCompay & """" & Environment.NewLine
        Next

        Return ret
    End Function

End Class
