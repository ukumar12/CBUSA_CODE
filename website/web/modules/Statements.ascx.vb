Option Strict Off

Imports Components
Imports DataLayer
Imports Controls
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationManager
Imports System.Linq

Partial Class _default
    Inherits ModuleControl
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""

      Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        gvList.BindList = AddressOf BindList

        PageURL = Request.Url.ToString()
        CurrentUserId = Session("BuilderId")
        UserName = Session("Username")

        If Not IsPostBack Then

        Core.DataLog("Rebate Distribution Report", PageURL, CurrentUserId, "Builder Rebates Left Menu Click", "", "", "", "", UserName)

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))



            If gvList.SortBy = String.Empty Then
                gvList.SortBy = "StatementDate "
                gvList.SortOrder = "DESC "
            End If
            If gvList.SortOrder = String.Empty Then gvList.SortOrder = "ASC"

            Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))
            Dim Sql As String = ""
            Sql &= " Select  max(YEAR(StatementDate)) AS maximum, min(YEAR(StatementDate))as minimum from Statement "
            Sql &= " Inner Join Builder ON Statement.HistoricID = Builder.HistoricID  "
            Sql &= " where Builder.HistoricID = " & DB.Quote(dbBuilder.HistoricID)
            Dim Minmaxdb As DataTable = DB.GetDataTable(Sql)
            If Not IsDBNull(Minmaxdb.Rows.Item(0).Item("maximum")) AndAlso Not IsDBNull(Minmaxdb.Rows.Item(0).Item("minimum")) Then
                Dim maxyear As Integer = CInt(Minmaxdb.Rows.Item(0).Item("maximum"))
                Dim minyear As Integer = CInt(Minmaxdb.Rows.Item(0).Item("minimum"))
                If maxyear <> minyear Then
                    For value As Integer = minyear To maxyear Step 1
                        F_drpYear.Items.Insert(0, New ListItem(value, value))
                    Next
                    F_drpYear.Items.Insert(0, New ListItem("-- please select -- ", ""))
                Else
                    F_drpYear.Items.Insert(0, New ListItem(minyear, minyear))
                    F_drpYear.Items.Insert(0, New ListItem("-- please select -- ", ""))
                End If
            End If

            F_drpYear.SelectedValue = Request("F_drpYear")
            If Request("F_drpYear") = String.Empty Then
                F_drpYear.SelectedValue = Now.Year
            End If
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
        If Not F_drpYear.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "YEAR(StatementDate) = " & DB.Quote(F_drpYear.SelectedValue)
            Conn = " AND "
        End If
        

        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))
        SQL = SQL & Conn & "Builder.HistoricID = " & DB.Quote(dbBuilder.HistoricID)
        Conn = " AND "



        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)
        Return DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)

    End Function

    Private Sub BindList()
        Dim res As DataTable = BindBuilderStatement()
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

 


    Protected Sub gvList_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If Not (e.Row.RowType = DataControlRowType.DataRow) Then
            Exit Sub
        End If
        Dim lnktostatement As HyperLink = e.Row.FindControl("lnktostatement")
        lnktostatement.NavigateUrl = "/file.aspx?Id=" & e.Row.DataItem("StatementId")
        Dim sdate As DateTime = DataBinder.Eval(e.Row.DataItem, "StatementDate")
        lnktostatement.Text = sdate.ToString("MMMM") & " " & sdate.ToString("yyyy")


       
        'If e.Row.DataItem("Status") = "New" Then
        '    ltlStatus.Text = "Pending - Bid Request Not Sent"
        'Else
        '    ltlStatus.Text = e.Row.DataItem("Status")
        'End If

    End Sub

    Protected Sub F_drpYear_SelectedIndexChanged(sender As Object, e As EventArgs) Handles F_drpYear.SelectedIndexChanged
        gvList.PageIndex = 0
        BindList()
    End Sub

   ' Protected Sub btnDashBoard_Click(sender As Object, e As System.EventArgs) Handles btnDashBoard.Click
       ' Response.Redirect("/builder/default.aspx")
   ' End Sub

End Class
