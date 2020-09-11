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
            F_BuilderID.DataSource = BuilderRow.GetList(DB, "CompanyName")
            F_BuilderID.DataValueField = "BuilderID"
            F_BuilderID.DataTextField = "CompanyName"
            F_BuilderID.DataBind()
            F_BuilderID.Items.Insert(0, New ListItem("-- ALL --", ""))
            F_LLCID.DataSource = LLCRow.GetList(DB, "LLC")
            F_LLCID.DataValueField = "LLCID"
            F_LLCID.DataTextField = "LLC"
            F_LLCID.DataBind()
            F_LLCID.Items.Insert(0, New ListItem("--ALL --", ""))
            F_HistoricId.Text = Request("F_HistoricId")
            F_Email.Text = Request("F_Email")
            F_IsNew.Text = Request("F_IsNew")
            F_IsActive.Text = Request("F_IsActive")
            F_BuilderID.SelectedValue = Request("F_BuilderID")
            F_LLCID.SelectedValue = Request("F_LLCID")
            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            F_CreateDateLbound.Text = Request("F_CreateDateLbound")
            F_CreateDateUbound.Text = Request("F_CreateDateUbound")
            If Request("F_TotalBidsLBound") <> String.Empty Then
                F_TotalBidsLBound.Text = Request("F_TotalBidsLBound")
            Else
                F_TotalBidsLBound.Text = "1"
            End If
            F_TotalBidsUBound.Text = Request("F_TotalBidsUBound")
            F_ActiveBidsLBound.Text = Request("F_ActiveBidsLBound")
            F_ActiveBidsUBound.Text = Request("F_ActiveBidsUBound")
            F_AwardedBidsLBound.Text = Request("F_AwardedBidsLBound")
            F_AwardedBidsUBound.Text = Request("F_AwardedBidsUBound")
            F_AwardedTotalLBound.Text = Request("F_AwardedTotalLBound")
            F_AwardedTotalUBound.Text = Request("F_AwardedTotalUBound")
           
           
            If gvList.SortBy = String.Empty Then gvList.SortBy = "CompanyName"

            lnkReturn.NavigateUrl = "/admin/builders/default.aspx?" & GetPageParams(FilterFieldType.All, "BuilderId")
            BindList()
        End If
    End Sub

    Private Function BindBuilderBidData() As DataTable
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


        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & "  b.*, p.TotalBids, p.ActiveBids, llc.llc , p.AwardedBids, p.AwardedTotal "
        SQL = " From Builder b"
        SQL &= " LEFT OUTER  JOIN  LLC  on b.LLCID =LLC.LLCID "
        SQL &= " LEFT OUTER  JOIN (Select Project.BuilderId, Count(quoteid) AS TotalBids"
        SQL &= " ,Count(CASE WHEN POQuote.Status In ('New', 'Bidding In progress') THEN quoteid ELSE NULL END) AS ActiveBids"
        SQL &= " ,Count(CASE WHEN POQuote.Status In ('Awarded') THEN quoteid ELSE NULL END) AS AwardedBids"
        SQL &= " ,Sum(CASE WHEN POQuote.Status In ('Awarded') THEN AwardedTotal ELSE 0 END) AS AwardedTotal"
        SQL &= " From POQuote Inner Join Project ON POQuote.ProjectId = Project.ProjectID "
        If Not F_CreateDateLbound.Text = String.Empty Then
            SQL = SQL & Conn & "POQuote .CreateDate>= " & DB.Quote(F_CreateDateLbound.Text)
            Conn = " AND "
        End If
        If Not F_CreateDateUbound.Text = String.Empty Then
            SQL = SQL & Conn & "POQuote .CreateDate < " & DB.Quote(DateAdd("d", 1, F_CreateDateUbound.Text))
        End If
        Conn = "WHERE "
        SQL &= " group by Project.BuilderId)p"
        SQL &= " ON b.BuilderId = p.BuilderID "


        If Not F_HistoricId.Text = String.Empty Then
            SQL = SQL & Conn & "b.HistoricId = " & DB.Quote(F_HistoricId.Text)
            Conn = " AND "
        End If
        If Not F_BuilderID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "b.BuilderID = " & DB.Quote(F_BuilderID.SelectedValue)
            Conn = " AND "
        End If
        If Not F_LLCID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "b.llcid = " & DB.Quote(F_LLCID.SelectedValue)
            Conn = " AND "
        End If
        If Not F_Email.Text = String.Empty Then
            SQL = SQL & Conn & "b.Email LIKE " & DB.FilterQuote(F_Email.Text)
            Conn = " AND "
        End If
        If Not F_IsNew.SelectedValue = String.Empty Then
            SQL = SQL & Conn & " b.IsNew  = " & DB.Number(F_IsNew.SelectedValue)
            Conn = " AND "
        End If
        If Not F_IsActive.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "b.IsActive  = " & DB.Number(F_IsActive.SelectedValue)
            Conn = " AND "
        End If
        If Not F_TotalBidsLBound.Text = String.Empty Then
            SQL = SQL & Conn & "TotalBids >= " & DB.Number(F_TotalBidsLBound.Text)
            Conn = " AND "
        End If
        If Not F_TotalBidsUBound.Text = String.Empty Then
            SQL = SQL & Conn & "TotalBids <= " & DB.Number(F_TotalBidsUBound.Text)
            Conn = " AND "
        End If
        If Not F_ActiveBidsLBound.Text = String.Empty Then
            SQL = SQL & Conn & "ActiveBids >= " & DB.Number(F_ActiveBidsLBound.Text)
            Conn = " AND "
        End If
        If Not F_ActiveBidsUBound.Text = String.Empty Then
            SQL = SQL & Conn & "ActiveBids <= " & DB.Number(F_ActiveBidsUBound.Text)
            Conn = " AND "
        End If
        If Not F_AwardedBidsLBound.Text = String.Empty Then
            SQL = SQL & Conn & "AwardedBids >= " & DB.Number(F_AwardedBidsLBound.Text)
            Conn = " AND "
        End If
        If Not F_AwardedBidsUBound.Text = String.Empty Then
            SQL = SQL & Conn & "AwardedBids <= " & DB.Number(F_AwardedBidsUBound.Text)
            Conn = " AND "
        End If
        If Not F_AwardedTotalLBound.Text = String.Empty Then
            SQL = SQL & Conn & "AwardedTotal >= " & DB.Number(F_AwardedTotalLBound.Text)
            Conn = " AND "
        End If
        If Not F_AwardedTotalUBound.Text = String.Empty Then
            SQL = SQL & Conn & "AwardedTotal <= " & DB.Number(F_AwardedTotalUBound.Text)
            Conn = " AND "
        End If
        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)
        Return DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)

    End Function

    Private Sub BindList()
        Dim res As DataTable = BindBuilderBidData()
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub
        gvList.PageIndex = 0
        BindList()
    End Sub

    Protected Sub export_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Export.Click
        If Not IsValid Then Exit Sub
        ExportReport()
    End Sub
    Protected Sub gvList_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If Not (e.Row.RowType = DataControlRowType.DataRow) Then
            Exit Sub
        End If
        Dim lnktotalbids As HyperLink = e.Row.FindControl("lnktotalbids")
        lnktotalbids.NavigateUrl = "displaybids.aspx?BuilderID=" & e.Row.DataItem("BuilderID") & "&" & GetPageParams(FilterFieldType.All, "builderid;F_SortBy")

        'If e.Row.DataItem("Status") = "New" Then
        '    ltlStatus.Text = "Pending - Bid Request Not Sent"
        'Else
        '    ltlStatus.Text = e.Row.DataItem("Status")
        'End If

    End Sub
    Public Sub ExportReport()
        gvList.PageSize = 5000
        Dim res As DataTable = BindBuilderBidData()
        Dim Folder As String = "/assets/catalogs/"
        Dim FileName As String = Core.GenerateFileID & ".csv"
        Dim sw As StreamWriter = New StreamWriter(Server.MapPath(Folder & FileName), False)
        sw.WriteLine("Builder , Total Bids , Active Bids , Awarded Bids , Awarded Total(In $)")
        If res.Rows.Count > 0 Then
            For Each row As DataRow In res.Rows
                Dim CompanyName As String = row("CompanyName")
                Dim totalbids As String = String.Empty

                If Not IsDBNull(row("TotalBids")) Then
                    totalbids = row("TotalBids")
                Else
                    totalbids = "0"
                End If

                Dim ActiveBids As String = String.Empty
                If Not IsDBNull(row("ActiveBids")) Then
                    ActiveBids = row("ActiveBids")
                Else
                    ActiveBids = "0"
                End If

                Dim AwardedBids As String = String.Empty
                If Not IsDBNull(row("AwardedBids")) Then
                    AwardedBids = row("AwardedBids")
                Else
                    AwardedBids = "0"
                End If
                Dim AwardedTotal As String = String.Empty
                If Not IsDBNull(row("AwardedTotal")) Then
                    AwardedTotal = row("AwardedTotal")
                Else
                    AwardedTotal = "0"
                End If
                sw.WriteLine(Core.QuoteCSV(CompanyName) & "," & Core.QuoteCSV(totalbids) & "," & Core.QuoteCSV(ActiveBids) & "," & Core.QuoteCSV(AwardedBids) & "," & Core.QuoteCSV(AwardedTotal))
            Next
            sw.Flush()
            sw.Close()
            sw.Dispose()
            Response.Redirect(Folder & FileName)
        End If
    End Sub
End Class
