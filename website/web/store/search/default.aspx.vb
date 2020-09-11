Imports Components
Imports DataLayer

Partial Class Search
    Inherits SitePage

    Private Counter As Integer = 0
    Private ItemCounter As Integer = 0
    Private ItemsCollectionCount As Integer = 0
    Private ItemsCollection As StoreItemCollection
    Private ItemsPerRow As Integer = 3
    Private filter As DepartmentFilterField
    Private PerColumn As Integer = 0
    Private Keyword As String
    Dim qs As URLParameters
    Dim m_Match As String

    Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            txtKeyword.Text = Request("keyword")
        End If
        If txtKeyword.Text = "Enter Keyword" Then txtKeyword.Text = String.Empty

        LoadData()
        If Not IsPostBack Then LogSearch()
    End Sub

    Private Sub LoadData()
        Dim keyword As String = txtKeyword.Text

        If Not keyword = String.Empty Then

            Dim sFilteredWord As String = ExcludedSearchWordsRow.CleanInput(DB, keyword)
            If sFilteredWord = "" Then
                AddError("Your search keyword contained only special phrases/words.  Please try a more specific search.")
                Exit Sub
            End If
            keyword = sFilteredWord

            If keyword = String.Empty Then
                'do nothing
            ElseIf Match = "EXACT" Then
                rbMatchExact.Checked = True
                keyword = Core.DblQuote(keyword)
            ElseIf Match = "OR" Then
                rbMatchOr.Checked = True
                keyword = Core.SplitSearchOR(keyword)
            Else
                rbMatchAnd.Checked = True
                keyword = Core.SplitSearchAND(keyword)
            End If
        End If

        filter = New DepartmentFilterField
        filter.Keyword = keyword
		filter.RawKeyword = Trim(txtKeyword.Text)
        filter.SortBy = Request("sort")
        filter.SortOrder = Request("dir")
        filter.MaxPerPage = IIf(Request("perpage") = String.Empty, IIf(Request("F_All") = String.Empty, -1, 12), Request("perpage"))
        filter.pg = IIf(Request("pg") = String.Empty, 1, Request("pg"))

        divItems.Visible = True
        BindItems()
    End Sub

    Private Sub BindItems()
        ltlHeaderTitle.Text = "<div class=""largest bold center"" style=""padding:20px 15px 10px 15px;"">Search Results</div>"

        If txtKeyword.Text = String.Empty Then
            ItemsCollectionCount = 0
            ItemsCollection = Nothing
        Else
            Try
                ItemsCollectionCount = StoreItemRow.GetActiveItemsCount(DB, filter)
                ItemsCollection = StoreItemRow.GetActiveItems(DB, filter)
                If ItemsCollectionCount = 1 Then
                    LogSearch()
                    Response.Redirect("/store/item.aspx?itemid=" & ItemsCollection.Item(0).ItemId & "&keyword=" & txtKeyword.Text)
                End If
                NavigatorTop.NofRecords = ItemsCollectionCount
                NavigatorBottom.NofRecords = ItemsCollectionCount
                NavigatorTop.MaxPerPage = filter.MaxPerPage
                NavigatorBottom.MaxPerPage = filter.MaxPerPage
                NavigatorTop.Pg = Request("pg")
                NavigatorBottom.Pg = Request("pg")
                NavigatorTop.Sort = Request("sort")
                NavigatorBottom.Sort = Request("sort")
                NavigatorTop.URL = Core.GetURLOnly(Request.RawUrl)
                NavigatorBottom.URL = Core.GetURLOnly(Request.RawUrl)
                rptItems.DataSource = ItemsCollection
                rptItems.DataBind()
            Catch ex As Exception
                'Throw ex
                AddError("A clause of the query contained only ignored words. Please change the keyword and try again.")
            End Try
        End If
    End Sub

    Sub rptItems_DataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptItems.ItemDataBound
        If Not e.Item.ItemType = ListItemType.Item And Not e.Item.ItemType = ListItemType.AlternatingItem Then
            Exit Sub
        End If

        ItemCounter += 1
        If ItemCounter Mod ItemsPerRow = 0 Then
            Dim ltlEnd As Literal = CType(e.Item.FindControl("ltlEnd"), Literal)
            ltlEnd.Text = "</tr>"
            If ItemCounter <= ItemsCollection.Count Then
                Dim trDivider As HtmlTableRow = CType(e.Item.FindControl("trDivider"), HtmlTableRow)
                trDivider.Visible = True

                Dim tdColspan As HtmlTableCell = CType(e.Item.FindControl("tdColspan"), HtmlTableCell)
                tdColspan.ColSpan = ItemsPerRow
            End If
        End If
        If ItemCounter Mod ItemsPerRow = 1 Then
            Dim ltlStart As Literal = CType(e.Item.FindControl("ltlStart"), Literal)
            ltlStart.Text = "<tr valign=""bottom"">"
        End If
        If ItemCounter = ItemsCollection.Count Then
            Dim ltlEnd As Literal = CType(e.Item.FindControl("ltlEnd"), Literal)
            ltlEnd.Text = "</tr>"
        End If
        Dim ctrlSearchItem As SearchItem = CType(e.Item.FindControl("ctrlSearchItem"), SearchItem)
        ctrlSearchItem.Item = CType(e.Item.DataItem, StoreItemRow)
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If ItemsCollectionCount = 0 Then
            divNoRecords.Visible = True
        End If
    End Sub

    Private Property Match() As String
        Get
            If rbMatchExact.Checked = True Then
                m_Match = "EXACT"
            ElseIf rbMatchOr.Checked = True Then
                m_Match = "OR"
            ElseIf rbMatchAnd.Checked = True Then
                m_Match = "AND"
            ElseIf Not Request("match") = Nothing Then
                m_Match = Request("match")
            Else
                m_Match = "AND"
            End If
            Return m_Match
        End Get
        Set(ByVal value As String)

        End Set
    End Property

    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        qs = New URLParameters(Request.QueryString, "pg;keyword;match;f1;f2;grade;sort;dir;perpage;BrandId")
        qs.Add("keyword", txtKeyword.Text)
        qs.Add("match", Match)
        qs.Add("perpage", "12")
        Response.Redirect("/store/search/default.aspx" & qs.ToString())
    End Sub

    Private Sub LogSearch()
        If Not txtKeyword.Text = String.Empty Then
            'Log search term in the database
            Dim OrderId As Integer = IIf(Session("OrderId") Is Nothing, 0, Session("OrderId"))
            Dim MemberId As Integer = IIf(Session("MemberId") Is Nothing, 0, Session("MemberId"))
            Dim SearchTermRowId As Integer = SearchTermRow.InsertSearchTerm(DB, txtKeyword.Text, Request.ServerVariables("REMOTE_ADDR"), OrderId, MemberId, ItemsCollectionCount)
            If OrderId = 0 Then
                Dim Conn As String = IIf(Session("SearchOrderTracking") Is Nothing, String.Empty, ",")
                Session("SearchOrderTracking") = Session("SearchOrderTracking") & Conn & SearchTermRowId
            End If
            If MemberId = 0 Then
                Dim Conn As String = IIf(Session("SearchMemberTracking") Is Nothing, String.Empty, ",")
                Session("SearchMemberTracking") = Session("SearchMemberTracking") & Conn & SearchTermRowId
            End If
        End If
    End Sub
End Class
