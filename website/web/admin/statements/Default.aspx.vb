Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.IO
Imports System.Net

Partial Class Index
    Inherits AdminPage

    Private path As String

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BUILDERS")

        path = SysParam.GetValue(DB, "StatementsFilePath")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_BuilderID.DataSource = BuilderRow.GetList(DB, "CompanyName")
            F_BuilderID.DataValueField = "HistoricID"
            F_BuilderID.DataTextField = "CompanyName"
            F_BuilderID.DataBind()
            F_BuilderID.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_BuilderID.SelectedValue = Request("F_StatementId")
            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            F_CreateDateLbound.Text = Request("F_CreateDateLbound")
            F_CreateDateUbound.Text = Request("F_CreateDateUbound")



            If gvList.SortBy = String.Empty Then
                gvList.SortBy = "StatementDate "
                gvList.SortOrder = "DESC "
            End If


            lnkReturn.NavigateUrl = "/admin/builders/default.aspx?" & GetPageParams(FilterFieldType.All, "BuilderId")
            BindList()
        End If
    End Sub

    Private Function BindBuilderStatement() As DataTable
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        'SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & "  Builder.* "
        'SQLFields &= ",(Select Count(QuoteId) From POQuote Inner Join Project  ON POQuote.ProjectId = Project.ProjectID Where Project.BuilderId = Builder.BuilderID ) As TotalBids "
        'SQLFields &= ",(Select Count(QuoteId) From POQuote Inner Join Project  ON POQuote.ProjectId = Project.ProjectID Where POQuote.Status In ('New', 'Bidding In progress') And Project.BuilderId = Builder.BuilderID) As ActiveBids "
        'SQLFields &= ",(Select Count(QuoteId) From POQuote Inner Join Project  ON POQuote.ProjectId = Project.ProjectID Where POQuote.Status In ('Awarded') And Project.BuilderId = Builder.BuilderID) As AwardedBids "
        'SQLFields &= ",(Select Sum(AwardedTotal) From POQuote Inner Join Project  ON POQuote.ProjectId = Project.ProjectID Where POQuote.Status In ('Awarded') And Project.BuilderId = Builder.BuilderID) As AwardedTotal"
        'SQL = " From Builder"


        SQLFields = "Select * "
        SQL = ""
        SQL &= " From Statement Inner Join Builder ON Statement.HistoricID = Builder.HistoricID "
        If Not F_CreateDateLbound.Text = String.Empty Then
            SQL = SQL & Conn & "StatementDate>= " & DB.Quote(F_CreateDateLbound.Text)
            Conn = " AND "
        End If
        If Not F_CreateDateUbound.Text = String.Empty Then
            SQL = SQL & Conn & "StatementDate < " & DB.Quote(DateAdd("d", 1, F_CreateDateUbound.Text))
            Conn = " AND "
        End If

        If Not F_BuilderID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "Builder.HistoricID = " & DB.Quote(F_BuilderID.SelectedValue)
            Conn = " AND "
        End If


        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)
        Return DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)

    End Function

    Private Sub BindList()
        Dim res As DataTable = BindBuilderStatement()
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub
        gvList.PageIndex = 0
        BindList()
    End Sub


    Protected Sub gvList_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If Not (e.Row.RowType = DataControlRowType.DataRow) Then
            Exit Sub
        End If
        'Dim lnktotalbids As HyperLink = e.Row.FindControl("lnktotalbids")
        'lnktotalbids.NavigateUrl = "/file.aspx?Id=" & e.Row.DataItem("StatementId") & "&" & GetPageParams(FilterFieldType.All, "builderid;F_SortBy")

        Dim sdate As DateTime = DataBinder.Eval(e.Row.DataItem, "StatementDate")

        Dim ltlMonth As Literal = e.Row.FindControl("ltlMonth")

        ltlMonth.Text = sdate.Month
        Dim ltlYear As Literal = e.Row.FindControl("ltlYear")
        ltlYear.Text = sdate.Year

        Dim lnktostatement As HyperLink = e.Row.FindControl("lnktostatement")
        lnktostatement.NavigateUrl = "/admin/file.aspx?Id=" & e.Row.DataItem("StatementId")
        lnktostatement.Text = e.Row.DataItem("FileName")
        'If e.Row.DataItem("Status") = "New" Then
        '    ltlStatus.Text = "Pending - Bid Request Not Sent"
        'Else
        '    ltlStatus.Text = e.Row.DataItem("Status")
        'End If

    End Sub


   
End Class
