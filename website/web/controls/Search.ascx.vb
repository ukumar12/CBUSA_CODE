Imports Components
Imports IDevSearch
Imports System.Configuration.ConfigurationManager
Imports DataLayer
Imports NineRays.WebControls
Imports System.Linq


Partial Class controls_Search
    Inherits BaseControl

    Public Event ResultsUpdated As EventHandler

    Public Event OnTreeNodeSelect As eventhandler

    Dim dtPhases As DataTable

    Protected Property PhaseFilter() As String()
        Get
            Return ViewState("PhaseFilter")
        End Get
        Set(ByVal value As String())
            ViewState("PhaseFilter") = value
        End Set
    End Property

    Public Property ShowTitle() As Boolean
        Get
            Return IIf(ViewState("ShowTitle") Is Nothing, True, False)
        End Get
        Set(ByVal value As Boolean)
            ViewState("ShowTitle") = value
        End Set
    End Property

    Public Property HidePleaseWait() As Boolean
        Get
            Return ViewState("HidePleaseWait")
        End Get
        Set(ByVal value As Boolean)
            ViewState("HidePleaseWait") = value
        End Set
    End Property

    Public Property PageSize() As Integer
        Get
            Return IIf(ViewState("PageSize") Is Nothing, 25, ViewState("PageSize"))
        End Get
        Set(ByVal value As Integer)
            ViewState("PageSize") = value
        End Set
    End Property

    Public Property MaxDocs() As Integer
        Get
            Return IIf(ViewState("MaxDocs") Is Nothing, 1000, ViewState("MaxDocs"))
        End Get
        Set(ByVal value As Integer)
            ViewState("MaxDocs") = value
        End Set
    End Property

    Private m_FilterList As Generic.List(Of String)
    Public Property FilterList() As Generic.List(Of String)
        Get
            Return m_FilterList
        End Get
        Set(ByVal value As Generic.List(Of String))
            m_FilterList = value
        End Set
    End Property

    Private m_FilterListCallback As FilterListDelegate
    Public Property FilterListCallback() As FilterListDelegate
        Get
            Return m_FilterListCallback
        End Get
        Set(ByVal value As FilterListDelegate)
            m_FilterListCallback = value
        End Set
    End Property

    Private m_FilterCacheKey As String
    Public Property FilterCacheKey() As String
        Get
            Return m_FilterCacheKey
        End Get
        Set(ByVal value As String)
            m_FilterCacheKey = value
        End Set
    End Property

    Public Property ForceFilter() As Boolean
        Get
            Return ViewState("ForceFilter")
        End Get
        Set(ByVal value As Boolean)
            ViewState("ForceFilter") = value
        End Set
    End Property

    Private m_TreeOnlyFilter As Generic.List(Of String)
    Public Property TreeOnlyFilter() As Generic.List(Of String)
        Get
            'Return m_TreeOnlyFilter
            Return ViewState("TreeOnlyFilter")
        End Get
        Set(ByVal value As Generic.List(Of String))
            'm_TreeOnlyFilter = value
            ViewState("TreeOnlyFilter") = value
        End Set
    End Property

    Public Property FilterLLCID() As Integer
        Get
            Return ViewState("FilterLLCID")
        End Get
        Set(ByVal value As Integer)
            ViewState("FilterLLCID") = value
        End Set
    End Property

#Region "IDev"
    Private guid As String
    Private TrackingId As Integer

    Private ReadOnly Property txtKeyword() As TextBox
        Get
            Dim ctl As Control = Nothing
            If NamingContainer IsNot Nothing Then
                ctl = NamingContainer.FindControl(KeywordsTextboxId)
            End If
            If ctl Is Nothing Then
                ctl = Page.FindControl(KeywordsTextboxId)
            End If
            Return ctl
        End Get
    End Property

    Public Property KeywordsTextboxId() As String
        Get
            Return ViewState("KeywordsTextboxId")
        End Get
        Set(ByVal value As String)
            ViewState("KeywordsTextboxId") = value
        End Set
    End Property

    Private Overloads Function FindControl(ByVal id As String, ByVal parent As Control) As Control
        If parent.ID = id Then
            Return parent
        Else
            For Each child As Control In parent.Controls
                Dim ctl As Control = FindControl(id, child)
                If ctl IsNot Nothing Then
                    Return ctl
                End If
            Next
        End If
        Return Nothing
    End Function

    Private m_Breadcrumb As String
    Public ReadOnly Property Breadcrumbs() As String
        Get
            Return m_Breadcrumb
        End Get
    End Property

    Private m_SearchResults As SearchResult
    Public ReadOnly Property SearchResults() As SearchResult
        Get
            Return m_SearchResults
        End Get
    End Property

    Public Property PageNumber() As Integer
        Get
            If qString("pg") = Nothing Then
                qString.Add("pg", 1)
            End If
            Return qString("pg")
        End Get
        Set(ByVal value As Integer)
            If qString("pg") <> Nothing Then
                qString.Remove("pg")
            End If
            qString.Add("pg", value)
        End Set
    End Property

    Protected Property qString() As URLParameters
        Get
            If Not IsPostBack Or Session("SearchQueryString" & UniqueID) Is Nothing Then
                Session("SearchQueryString" & UniqueID) = New URLParameters()
            End If
            Return Session("SearchQueryString" & UniqueID)
        End Get
        Set(ByVal value As URLParameters)
            Session("SearchQueryString" & UniqueID) = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(Me)

        divTitle.Visible = ShowTitle
        'ray
        'ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(tvSupplyPhases)
        If Not HidePleaseWait Then
            RegisterScript()
        End If
        If Not IsPostBack Then
            qString = New URLParameters

            'If Not tvSupplyPhases.IsCached Then
            '    tvSupplyPhases.DataSource = SupplyPhaseRow.GetList(DB)
            '    tvSupplyPhases.DataTextField = "SupplyPhase"
            '    tvSupplyPhases.DataValueField = "SupplyPhaseId"
            '    tvSupplyPhases.DataKeyField = "SupplyPhaseId"
            '    tvSupplyPhases.DataParentField = "ParentSupplyPhaseId"
            '    tvSupplyPhases.RootLabel = "All Supply Phases"
            '    If TreeOnlyFilter IsNot Nothing Then
            '        tvSupplyPhases.FilterList = TreeOnlyFilter
            '        tvSupplyPhases.UseFilter = True
            '    ElseIf txtKeyword.Text = Nothing And Not ForceFilter Then
            '        tvSupplyPhases.UseFilter = False
            '    Else
            '        tvSupplyPhases.UseFilter = True
            '    End If
            '    tvSupplyPhases.DataBind()
            'End If
        End If
    End Sub

    Private Sub RegisterScript()
        If Not Page.ClientScript.IsClientScriptBlockRegistered("PleaseWait") Then
            Dim s As String = _
                  " function ShowSearching(sender,args) {" _
                & "     var frm = $get('" & frmLoading.ClientID & "').control;" _
                & "     frm._doMoveToCenter();" _
                & "     frm.Open();" _
                & "     Sys.Application.add_load(HideSearching);" _
                & " }" _
                & " function HideSearching(sender,args) {" _
                & "     Sys.Application.remove_load(HideSearching);" _
                & "     var frm = $get('" & frmLoading.ClientID & "').control;" _
                & "     frm.Close();" _
                & " }"

            Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "PleaseWait", s, True)
            'Page.ClientScript.RegisterStartupScript(Me.GetType, "InitPleaseWait", " Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(ShowSearching);", True)
            'Else
            '    frmLoading.Visible = False
        End If
    End Sub

    Private Function RemovePipe(ByVal s As String) As String
        Dim pipe As String = InStrRev(s, "|")
        Return Right(s, Len(s) - pipe)
    End Function

    Private Sub Add2BreadCrumb(ByVal Text As String, ByVal link As String)
        If Not m_Breadcrumb = String.Empty Then
            m_Breadcrumb &= " <span>&gt;</span> "
        End If
        m_Breadcrumb &= "<a href=""" & link & """>" & Text & "</a>"
    End Sub

    Private Sub Add2BreadCrumb(ByVal Text As String)
        If Not m_Breadcrumb = String.Empty Then
            m_Breadcrumb &= " <span>&gt;</span> "
        End If
        m_Breadcrumb &= Text
    End Sub

    Private Function GetBreadcrumb(ByVal Text As String, ByVal Facet As String, ByVal qs As URLParameters) As String
        Return GetBreadcrumb(Text, Facet, qs, String.Empty)
    End Function

    Private Function GetBreadcrumb(ByVal Text As String, ByVal Facet As String, ByVal qs As URLParameters, ByVal Children As String) As String
        'Remove facets
        Dim f As String = qString("f")
        Dim ChildrenArray() As String = Children.Split(",")
        For Each childfacet As String In ChildrenArray
            If Not childfacet = String.Empty Then
                qs.Remove(childfacet)
                f = SearchIndex.RemoveFacet(f, childfacet)
            End If
        Next
        f = SearchIndex.RemoveFacet(f, Facet)
        'Add (x) to remove facet
        Dim xlnk As String = "<a href=""" & Page.ClientScript.GetPostBackClientHyperlink(btnSearch, qs.ToString("f", f)) & """><sup>(x)</sup></a>"

        'Remove facets
        Dim facets As String = RemoveFacets(Facet, qs)
        qs.Add("f", facets)
        qs.Add(Facet, qString(Facet))

        Text = RemovePipe(Text)
        Return "<a href=""" & Page.ClientScript.GetPostBackClientHyperlink(btnSearch, qs.ToString) & """>" & Text & "</a>" & xlnk
    End Function

    Private Function RemoveFacets(ByVal facet As String, ByRef qs As URLParameters) As String
        If qString("f") = String.Empty Then
            Return String.Empty
        End If
        Dim Facets() As String = qString("f").ToLower.Split(",")
        Dim j As Integer = Facets.Length
        Dim Result As String = String.Empty
        Dim Conn As String = String.Empty
        facet = facet.ToLower
        For i As Integer = 0 To Facets.Length - 1
            If Facets(i) = facet Then
                j = i
            End If
            If i > j Then
                qs.Remove(Facets(i))
                Facets(i) = String.Empty
            End If
            If Not Facets(i) = String.Empty Then
                Result &= Conn & Facets(i)
                Conn = ","
            End If
        Next
        Return Result
    End Function

    Private Sub AddFacet(ByRef bc As Hashtable, ByRef facets As GenericCollection(Of Facet), ByRef facetcache As GenericCollection(Of Facet), ByVal name As String, ByVal field As String, ByVal param As String)
        Dim f As New Facet
        f.Name = name
        f.Field = field
        f.MaxDocs = 0
        f.Narrow = qString(param)
        f.ZerosIncluded = False
        facets.Add(f)
        facetcache.Add(f)
        If Not qString(param) = String.Empty Then
            Dim qs As New URLParameters(qString.Items, param & ";f;pg")
            bc(param) = GetBreadcrumb(qString(param), param, qs, "")
        End If
    End Sub

    Public Sub Search(Optional ByVal ClearFacets As Boolean = False)
        txtKeyword.Text = Keywords

        If ClearFacets Then
            qString = New URLParameters()
            qString.Add("keywords", Keywords)
            qString.Add("f", "keywords")
        End If
         
        Dim bc As New Hashtable
        guid = qString("guid")
        If guid = String.Empty Then guid = Core.GenerateFileID

        Dim index As New SearchIndex
        index.Directory = AppSettings("SearchIndexDirectory")
        index.IndexName = AppSettings("SearchIndexName")
        If qString("operator") = "OR" Then
            index.DefaultOperator = "OR"
        End If

        Dim facets As New GenericCollection(Of Facet)
        Dim facetcache As New GenericCollection(Of Facet)
        Dim query As New IndexQuery
        query.Keywords = Keywords
        Dim aKeywords As String() = query.Keywords.Split(" ")
        If aKeywords.Length = 1 AndAlso aKeywords(0) <> "" Then
            query.Keywords = "*" & query.Keywords & "*"
        End If
        query.MaxDocs = MaxDocs
        query.Facets = facets
        query.FacetCache = facetcache
        query.SortBy = String.Empty
        query.SortReverse = False
        query.BestFragmentField = "Description"
        query.PageNo = IIf(qString("pg") = String.Empty, 1, qString("pg"))
        query.MaxPerPage = PageSize
        query.ForceRefresh = False
        query.FacetCacheDuration = 60 * 5 'five minutes
        query.MaxHitsForCache = 10

        If FilterList IsNot Nothing Then
            'query.FilterListCallback = FilterListCallback
            'query.FilterCacheKey = FilterCacheKey
            query.FilterList = FilterList
            If Session("InitialllcFilter") IsNot Nothing Then
                rblFilter.SelectedValue = "market"
            End If

        ElseIf qString("LLCFilterId") IsNot Nothing Then
            'query.FilterListCallback = AddressOf GetFilterList
            'query.FilterCacheKey = qString("LLCFilterId")
            Dim list As New Generic.List(Of String)
            GetFilterList(list)
            query.FilterList = list
        ElseIf Session("InitialllcFilter") IsNot Nothing Then
            Dim list As New Generic.List(Of String)
            GetFilterListBySession(list)
            query.FilterList = list
        End If

        Dim sort As String = qString("sort")
        Select Case sort
            Case "priceasc"
                query.SortBy = "SalePrice"
                query.SortReverse = False
            Case "pricedesc"
                query.SortBy = "SalePrice"
                query.SortReverse = True
            Case "score", ""
                query.SortBy = String.Empty
                query.SortReverse = False
        End Select

        AddFacet(bc, facets, facetcache, "ProductType", "ProductType", "producttype")
        AddFacet(bc, facets, facetcache, "Manufacturer", "Manufacturer", "manufacturer")
        AddFacet(bc, facets, facetcache, "UnitOfMeasure", "UnitOfMeasure", "unitofmeasure")

        Dim phase As New Facet
        phase.Name = "SupplyPhase"
        phase.Field = "SupplyPhase"
        phase.MaxDocs = 0
        phase.Narrow = IIf(qString("supplyphase") = Nothing, "ROOT", Server.HtmlDecode(Server.UrlDecode(qString("supplyphase"))))
        'phase.Narrow = Nothing
        phase.ZerosIncluded = False
        facets.Add(phase)
        facetcache.Add(phase)
        If Not qString("supplyphase") = String.Empty Then
            Dim qs As New URLParameters(qString.Items, "supplyphase;f;pg")
            bc("supplyphase") = GetBreadcrumb(Server.UrlDecode(qString("supplyphase")), "supplyphase", qs, "")
        End If

        Dim start As DateTime = Now
        Dim sr As SearchResult = Nothing
        sr = index.Search(query)

        'If more than one word and no results, then redirect to search
        'page with OR operator
        If Not Keywords = String.Empty Then
            Dim Words() As String = Keywords.Split(" "c)
            If Words.Length > 1 Then
                Dim qs As New URLParameters(qString.Items, "operator;pg;guid;s")
                qs.Add("guid", guid)
                If sr.Count = 0 AndAlso qString("operator") <> "OR" Then
                    qString.Remove("operator")
                    qString.Add("operator", "OR")
                    Search()
                    Exit Sub
                End If
            End If
        End If

        If Not qString("producttype") = String.Empty AndAlso sr.ds.Tables("producttype").Rows.Count > 0 Then
            AddAllRow(sr.ds.Tables("producttype"))
        End If
        rptProductType.DataSource = sr.ds.Tables("producttype")
        rptProductType.Visible = rptProductType.DataSource.Rows.Count > 1
        rptProductType.DataBind()

        If Not qString("manufacturer") = String.Empty AndAlso sr.ds.Tables("manufacturer").Rows.Count > 0 Then
            AddAllRow(sr.ds.Tables("manufacturer"))
        End If
        rptManufacturer.DataSource = sr.ds.Tables("manufacturer")
        rptManufacturer.Visible = rptManufacturer.DataSource.Rows.Count > 1
        rptManufacturer.DataBind()

        If Not qString("unitofmeasure") = String.Empty AndAlso sr.ds.Tables("unitofmeasure").Rows.Count > 0 Then
            AddAllRow(sr.ds.Tables("unitofmeasure"))
        End If
        rptUnitOfMeasure.DataSource = sr.ds.Tables("unitofmeasure")
        rptUnitOfMeasure.Visible = rptUnitOfMeasure.DataSource.Rows.Count > 1
        rptUnitOfMeasure.DataBind()


        Dim PhaseFilterList As New Generic.List(Of String)
        For Each row As DataRow In sr.ds.Tables("SupplyPhase").Rows
            Dim str As String = Core.GetString(row("Value"))
            If str <> "ROOT" Then
                Dim split As Integer = str.LastIndexOf("|")
                PhaseFilterList.Add(str.Substring(0, split))
            End If
        Next
        PhaseFilter = PhaseFilterList.ToArray
        TreeViewAddNodes(flyTreeView.Nodes, 0, flyTreeView.SelectedNode, PhaseFilter)

        'Display breadcrumb trail
        Add2BreadCrumb("Clear All", Page.ClientScript.GetPostBackClientHyperlink(btnSearch, "clear"))
        If Not Keywords = String.Empty Then
            'Add2BreadCrumb("<b>" & Keywords & "</b>", Page.ClientScript.GetPostBackClientHyperlink(btnSearch, "keywords=" & Keywords))

            'Add2BreadCrumb(GetBreadcrumb(Keywords, "keywords", qString, String.Empty))
            Dim qs As New URLParameters()
            qs.Add("f", "keywords")
            qs.Add("keywords", Keywords)
            Add2BreadCrumb("<a href=""" & Page.ClientScript.GetPostBackClientHyperlink(btnSearch, qs.ToString) & """><b>" & Keywords & "</b></a><a href=""" & Page.ClientScript.GetPostBackClientHyperlink(btnSearch, "clear") & """><sup>x</sup></a>")

        End If
        If Not qString("f") = String.Empty Then
            Dim f() As String = qString("f").Split(",")
            For j As Integer = 0 To f.Length - 1
                Add2BreadCrumb(bc(LCase(f(j))))
            Next
        End If

        If FilterLLCID = Nothing Then
            liLLCFilter.Visible = False
        Else

            If qString("LLCFilterId") = Nothing AndAlso Session("InitialllcFilter") = Nothing Then
                rblFilter.SelectedValue = "all"
                Session("InitialllcFilter") = Nothing
            Else
                rblFilter.SelectedValue = "market"
            End If
        End If

        'Generate user guid for current session
        If Session("UserGuid") Is Nothing Then
            Session("UserGuid") = Core.GenerateFileID
        End If

        upFacets.Update()
        m_SearchResults = sr
        RaiseEvent ResultsUpdated(Me, EventArgs.Empty)
    End Sub

    Protected ReadOnly Property Keywords() As String
        Get
            Dim result As String = IIf(txtKeyword.Text = "Keyword Search", Nothing, txtKeyword.Text)
            If result Is Nothing OrElse result = String.Empty Then result = qString("Keywords")
            If result Is Nothing Then result = String.Empty

            Return result
        End Get
    End Property

    Private Function GetLabelOrValueText(ByVal field As String, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs, ByVal Skip As String, ByVal fieldvalue As String) As String
        Dim qs As New URLParameters(qString.Items, field & ";" & Skip & ";f;pg;guid;s")
        qs.Add("guid", guid)
        Dim CountText As String = String.Empty
        Dim SkipArray() As String = (Skip & "").Split(";")
        If e.Item.DataItem("count") >= 0 Then
            CountText = "(" & e.Item.DataItem("count") & ")"
        End If
        Dim f As String = Request("f")
        For Each item As String In SkipArray
            If Not item = String.Empty Then f = SearchIndex.RemoveFacet(f, item)
        Next
        If e.Item.DataItem("value") = String.Empty Then
            qs.Add("f", SearchIndex.RemoveFacet(f, field))
        Else
            qs.Add("f", SearchIndex.ReplaceFacet(f, field))
        End If
        If qString(field) = e.Item.DataItem("value") Then
            Dim value As String = RemovePipe(e.Item.DataItem(fieldvalue))
            Return value & " " & CountText
        ElseIf e.Item.DataItem("count") = -1 Then
            Return "<a href=""" & Page.ClientScript.GetPostBackClientHyperlink(btnSearch, qs.ToString(field, e.Item.DataItem("value"))) & """>" & e.Item.DataItem("label") & "</a> " & CountText
        Else
            Dim value As String = RemovePipe(e.Item.DataItem(fieldvalue))
            Return "<a href=""" & Page.ClientScript.GetPostBackClientHyperlink(btnSearch, qs.ToString(field, e.Item.DataItem("value"))) & """>" & value & "</a> " & CountText
        End If
    End Function

    Private Function GetLabelText(ByVal field As String, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs, ByVal skip As String) As String
        Return GetLabelOrValueText(field, e, skip, "label")
    End Function

    Private Function GetLabelText(ByVal field As String, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) As String
        Return GetLabelOrValueText(field, e, String.Empty, "label")
    End Function

    Private Function GetValueText(ByVal field As String, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs, ByVal skip As String) As String
        Return GetLabelOrValueText(field, e, skip, "value")
    End Function

    Private Function GetValueText(ByVal field As String, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) As String
        Return GetLabelOrValueText(field, e, String.Empty, "value")
    End Function

    Private Sub AddAllRow(ByRef dt As DataTable)
        Dim row As DataRow = dt.NewRow()
        row("Name") = "All"
        row("Label") = "All"
        row("Value") = String.Empty
        row("Count") = -1
        dt.Rows.InsertAt(row, 0)
    End Sub

    Protected Sub rptManufacturer_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptManufacturer.ItemDataBound
        If Not e.Item.ItemType = ListItemType.AlternatingItem AndAlso Not e.Item.ItemType = ListItemType.Item Then
            Exit Sub
        End If
        Dim Label As Literal = e.Item.FindControl("ltlLabel")
        Label.Text = GetLabelText("manufacturer", e, "")
    End Sub

    Protected Sub rptProductType_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptProductType.ItemDataBound
        If Not e.Item.ItemType = ListItemType.AlternatingItem AndAlso Not e.Item.ItemType = ListItemType.Item Then
            Exit Sub
        End If
        Dim Label As Literal = e.Item.FindControl("ltlLabel")
        Label.Text = GetLabelText("producttype", e, "")
    End Sub

    Protected Sub rptUnitOfMeasure_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptUnitOfMeasure.ItemDataBound
        If Not e.Item.ItemType = ListItemType.AlternatingItem AndAlso Not e.Item.ItemType = ListItemType.Item Then
            Exit Sub
        End If
        Dim Label As Literal = e.Item.FindControl("ltlLabel")
        Label.Text = GetLabelText("unitofmeasure", e, "")
    End Sub

    Private Function ParamsFromString(ByVal queryString As String) As URLParameters
        Dim out As New URLParameters
        If queryString.Length > 0 Then
            For Each item As String In queryString.Split("&")
                Dim pair As String() = item.Split("=")
                If pair(0)(0) = "?" Then
                    pair(0) = Server.UrlDecode(pair(0).Substring(1, pair(0).Length - 1))
                End If
                out.Add(pair(0), Server.UrlDecode(pair(1)))
            Next
        End If
        Return out
    End Function
#End Region

    Protected Sub tvSupplyPhases_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tvSupplyPhases.SelectedIndexChanged
        Dim qs As New URLParameters(qString.Items, "supplyphase;f;pg;guid;s")
        Dim f As String = qString("f")
        qs.Add("f", SearchIndex.ReplaceFacet(f, "supplyphase"))
        qs.Add("supplyphase", tvSupplyPhases.CurrentNode.Value & "|" & tvSupplyPhases.CurrentNode.Name)
        qString = qs
        Search()
        upFacets.Update()
    End Sub

    Protected Sub btnSearch_Postback(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Postback
        Dim arg As String = Request("__EVENTARGUMENT")
        If arg <> Nothing Then
            If arg.ToLower = "clear" Then
                qString = New URLParameters
                txtKeyword.Text = String.Empty
            ElseIf Left(arg, 9) = "keywords=" Then
                qString = New URLParameters()
                qString.Add("keywords", Right(arg, arg.Length - 9))
            Else
                Dim qTemp As URLParameters = ParamsFromString(arg)
                If qTemp("LLCFilterID") <> qString("LLCFilterID") Then
                    TreeOnlyFilter = Nothing
                End If
                qString = qTemp
            End If
        Else
            qString = New URLParameters()
            qString.Add("keywords", txtKeyword.Text)
        End If
        Search()
        upFacets.Update()
    End Sub
    
    Protected Sub lnkExpand_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkExpand.Click
        For Each n As FlyTreeNode In flyTreeView.Nodes
            n.Expand()
            If n.ChildNodes.Count > 0 Then
                ExpandChildren(n)
            End If
        Next
    End Sub

    Private Sub ExpandChildren(ByVal n As FlyTreeNode)
        For Each child As FlyTreeNode In n.ChildNodes
            child.Expand()
            If child.ChildNodes.Count > 0 Then
                ExpandChildren(child)
            End If
        Next
    End Sub

    Protected Sub lnkCollapse_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkCollapse.Click
        For Each n As FlyTreeNode In flyTreeView.Nodes
            n.Collapse()
        Next
    End Sub

    Private Sub CollapseChildren(ByVal n As FlyTreeNode)
        For Each child As FlyTreeNode In n.ChildNodes
            child.Collapse()
            If child.ChildNodes.Count > 0 Then
                CollapseChildren(child)
            End If
        Next
    End Sub

    Protected Sub GetFilterList(ByRef list As Generic.List(Of String))
        list.AddRange(LLCRow.GetPricedProductsListForSearch(DB, qString("LLCFilterId")))
    End Sub
    Protected Sub GetFilterListBySession(ByRef list As Generic.List(Of String))
        list.AddRange(LLCRow.GetPricedProductsListForSearch(DB, Session("InitialllcFilter")))
    End Sub
    Protected Sub tvSupplyPhases_TreeStateChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tvSupplyPhases.TreeStateChanged
        upFacets.Update()
    End Sub


#Region "FlyTreeView"
    Protected Sub flyTreeView_PopulateNodes(ByVal sender As Object, ByVal e As NineRays.WebControls.FlyTreeNodeEventArgs)
        TreeViewAddNodes(e.Node.ChildNodes, e.Node.Value, Nothing, PhaseFilter)
        ShowHideNodes(e.Node.ChildNodes, PhaseFilter)
    End Sub

    Protected Sub flyTreeView_NodeSelected(ByVal sender As Object, ByVal e As NineRays.WebControls.FlyTreeNodeEventArgs)
        Dim qs As New URLParameters(qString.Items, "supplyphase;f;pg;guid;s")
        Dim f As String = qString("f")
        qs.Add("f", SearchIndex.ReplaceFacet(f, "supplyphase"))
        If e.Node.Value <> 0 Then
            qs.Add("supplyphase", e.Node.Value & "|" & Server.UrlEncode(e.Node.Text))
        Else
            'collaspe all nodes
            For Each node As FlyTreeNode In flyTreeView.Nodes
                node.Collapse()
            Next
        End If
        qString = qs

        RaiseEvent OnTreeNodeSelect(Me, e)

        Search()
        'upFacets.Update()
        'upProducts.Update()
    End Sub


    Private Sub ShowHideNodes(ByVal Nodes As FlyTreeNodeCollection, ByRef FilterList() As String)
        Dim NodeArray As New ArrayList()
        For Each node As FlyTreeNode In Nodes
            If Not FilterList Is Nothing AndAlso Not FilterList.Contains(node.Value) Then
                If Not node.Value = 0 Then
                    NodeArray.Add(node)
                End If
            End If
            ShowHideNodes(node.ChildNodes, FilterList)
        Next
        For Each node As FlyTreeNode In NodeArray
            node.Remove()
        Next
    End Sub

    Private Sub TreeViewAddNodes(ByVal nodes As FlyTreeNodeCollection, ByVal ParentId As Integer, ByVal SelectedNode As FlyTreeNode, Optional ByVal Filter() As String = Nothing)
        If dtPhases Is Nothing Then dtPhases = SupplyPhaseRow.GetList(DB)
        If ParentId = 0 Then

            Dim ftn As FlyTreeNode = New FlyTreeNode()
            ftn.Text = HttpUtility.HtmlEncode("All products")
            ftn.Value = 0
            ftn.Expanded = True

            If nodes.FindByValue(ftn.Value, True) Is Nothing Then
                nodes.Add(ftn)
            End If

            ParentId = SupplyPhaseRow.GetRootSupplyPhase(DB).SupplyPhaseID
        End If

        dtPhases.DefaultView.RowFilter = "ParentSupplyPhaseId = " & ParentId
        dtPhases.DefaultView.Sort = "SupplyPhase"
        For Each row As DataRowView In dtPhases.DefaultView
            Dim ftn As FlyTreeNode = New FlyTreeNode()
            ftn.Text = HttpUtility.HtmlEncode(row("SupplyPhase"))
            ftn.Value = row("SupplyPhaseId")

            If nodes.FindByValue(ftn.Value, True) Is Nothing Then
                nodes.Add(ftn)
            End If
            Try
                ftn.PopulateNodesOnDemand = dtPhases.Select("ParentSupplyPhaseId = " & row("SupplyPhaseId")).Count > 0
            Catch ex As Exception
            End Try
        Next

        If Not SelectedNode Is Nothing Then
            dtPhases.DefaultView.RowFilter = "ParentSupplyPhaseId = " & SelectedNode.Value
            dtPhases.DefaultView.Sort = "SupplyPhase"
            For Each row As DataRowView In dtPhases.DefaultView
                Dim ftn As FlyTreeNode = New FlyTreeNode()
                ftn.Text = HttpUtility.HtmlEncode(row("SupplyPhase"))
                ftn.Value = row("SupplyPhaseId")

                If SelectedNode.ChildNodes.FindByValue(ftn.Value, True) Is Nothing Then
                    SelectedNode.ChildNodes.Add(ftn)
                End If
                Try
                    ftn.PopulateNodesOnDemand = dtPhases.Select("ParentSupplyPhaseId = " & row("SupplyPhaseId")).Count > 0
                Catch ex As Exception
                End Try
            Next
        End If


    End Sub

#End Region

    Protected Sub Page_PreRender1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        ShowHideNodes(flyTreeView.Nodes, PhaseFilter)
    End Sub

    Protected Sub rblFilter_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles rblFilter.SelectedIndexChanged
        If rblFilter.SelectedValue = "market" Then
            If FilterLLCID = Nothing Then
                liLLCFilter.Visible = False
            Else

                'btnSearch_Postback(Me, qString.ToString())
                If qString("LLCFilterId") Is Nothing Then
                    'Dim qs As New URLParameters(qString.Items)
                    'qs.Remove("LLCFilterId")
                    Session("InitialllcFilter") = FilterLLCID
                    'qString = New URLParameters()
                    qString.Add("LLCFilterId", FilterLLCID)
                    Dim qTemp As URLParameters = ParamsFromString(qString.ToString())
                    If qTemp("LLCFilterId") <> qString("LLCFilterId") Then
                        TreeOnlyFilter = Nothing
                    End If
                    qString = qTemp

                End If
            End If
        Else
            'Dim qs As New URLParameters(qString.Items)
            qString.Remove("LLCFilterId")
            Session("InitialllcFilter") = Nothing
        End If

        Search()
        upFacets.Update()
    End Sub
End Class
