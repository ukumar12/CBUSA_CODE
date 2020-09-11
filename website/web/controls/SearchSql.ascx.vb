Imports Components
Imports System.Configuration.ConfigurationManager
Imports DataLayer
Imports NineRays.WebControls
Imports System.Linq
Imports System.Data.SqlClient

Partial Class controls_SearchSql
    Inherits BaseControl

    Public Event ResultsUpdated As EventHandler

    Public Event OnTreeNodeSelect As EventHandler

    Dim dtPhases As DataTable

    Public Enum CATALOG_TYPE
        CATALOG_TYPE_ALL = 0
        CATALOG_TYPE_MARKET = 1
        CATALOG_TYPE_VENDOR = 2
        CATALOG_TYPE_NOT_SET = -1
    End Enum

    Public Enum SEARCH_TYPE
        SEARCH_TYPE_PRODUCT = 1
        SEARCH_TYPE_PRODUCT_VENDOR = 2
        SEARCH_TYPE_SUPPLYPHASE = 3
    End Enum

#Region "Properties"
    Private cCatalogType As CATALOG_TYPE = CATALOG_TYPE.CATALOG_TYPE_NOT_SET
    Public Property CatalogType() As CATALOG_TYPE
        Get
            If cCatalogType = CATALOG_TYPE.CATALOG_TYPE_NOT_SET Then
                If Session("BuilderId") IsNot Nothing Then
                    cCatalogType = CATALOG_TYPE.CATALOG_TYPE_MARKET
                ElseIf Session("VendorId") IsNot Nothing Then
                    cCatalogType = CATALOG_TYPE.CATALOG_TYPE_VENDOR
                ElseIf Session("PIQId") IsNot Nothing Then
                    cCatalogType = CATALOG_TYPE.CATALOG_TYPE_ALL
                End If
            End If

            If ViewState("CatalogType") Is Nothing Then
                Return cCatalogType
            Else
                Return ViewState("CatalogType")
            End If
        End Get
        Set(ByVal value As CATALOG_TYPE)
            cCatalogType = value
            ViewState("CatalogType") = value

            If value = CATALOG_TYPE.CATALOG_TYPE_MARKET AndAlso rblFilter.SelectedValue = "all" Then
                rblFilter.SelectedValue = "market"
            ElseIf value = CATALOG_TYPE.CATALOG_TYPE_ALL AndAlso rblFilter.SelectedValue = "market" Then
                rblFilter.SelectedValue = "all"
            End If

        End Set
    End Property

    Private cSearchType As SEARCH_TYPE
    Public Property SearchType() As SEARCH_TYPE
        Get
            Return ViewState("SearchType")
        End Get
        Set(ByVal value As SEARCH_TYPE)
            ViewState("SearchType") = value
        End Set
    End Property

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

    Public Property PageSize() As Integer
        Get
            Return IIf(ViewState("PageSize") Is Nothing, 25, ViewState("PageSize"))
        End Get
        Set(ByVal value As Integer)
            ViewState("PageSize") = value
        End Set
    End Property

    Public Property PageNumber() As Integer
        Get
            If ViewState("pg") = Nothing Then
                ViewState.Add("pg", 1)
            End If
            Return ViewState("pg")
        End Get
        Set(ByVal value As Integer)
            If ViewState("pg") <> Nothing Then
                ViewState.Remove("pg")
            End If
            ViewState.Add("pg", value)
        End Set
    End Property

    Private m_FilterList As Generic.List(Of String)
    Public Property FilterList() As Generic.List(Of String)
        Get
            Return ViewState("FilterList")
            'Return m_FilterList
        End Get
        Set(ByVal value As Generic.List(Of String))
            ViewState("FilterList") = value
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

    Private m_SearchResults As DataTable
    Public ReadOnly Property SearchResults() As DataTable
        Get
            Return m_SearchResults
        End Get
    End Property

    Protected ReadOnly Property Keywords() As String
        Get
            Dim result As String = IIf(txtKeyword.Text = "Keyword Search", Nothing, txtKeyword.Text)
            'If result Is Nothing OrElse result = String.Empty Then result = qString("Keywords")
            If result Is Nothing Then result = String.Empty

            Return result
        End Get
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(Me)

        divTitle.Visible = ShowTitle

        If Not HidePleaseWait Then
            'RegisterScript()
        End If

        If Not IsPostBack Then
            If Session("TwoPriceLLCID") IsNot Nothing Then
                rblFilter.SelectedIndex = 0
                rblFilter.Visible = True
            ElseIf Session("BuilderId") IsNot Nothing Then
                If FilterList IsNot Nothing Then
                    rblFilter.Visible = False
                Else
                    rblFilter.SelectedIndex = 0
                End If
            ElseIf Session("VendorId") IsNot Nothing Then
                rblFilter.SelectedIndex = 1
                rblFilter.Visible = False
            ElseIf Session("PIQId") IsNot Nothing Then
                rblFilter.SelectedIndex = 1
                rblFilter.Visible = False
            End If
        End If

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

        SearchProduct(0, False, False)
        upFacets.Update()
    End Sub

    Protected Sub lnkExpand_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkExpand.Click
        For Each n As FlyTreeNode In flyTreeView.Nodes(0).ChildNodes
            n.Expand()
            If n.ChildNodes.Count > 0 Then
                ExpandChildren(n)
            End If
        Next
    End Sub

    Protected Sub lnkCollapse_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkCollapse.Click
        For Each n As FlyTreeNode In flyTreeView.Nodes(0).ChildNodes
            n.Collapse()
        Next
    End Sub

    Protected Sub rblFilter_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles rblFilter.SelectedIndexChanged

        If rblFilter.SelectedValue = "market" Then
            CatalogType = CATALOG_TYPE.CATALOG_TYPE_MARKET
            TreeViewAddNodes(flyTreeView.Nodes(0).ChildNodes, 0, flyTreeView.SelectedNode, False)
        Else
            'If Not Session("CurrentPreferredVendor") = Nothing Then Session("CurrentPreferredVendor") = Nothing

            CatalogType = CATALOG_TYPE.CATALOG_TYPE_ALL
            TreeViewAddNodes(flyTreeView.Nodes(0).ChildNodes, 0, flyTreeView.SelectedNode, True)
        End If

        ShowHideNodes(flyTreeView.Nodes(0).ChildNodes, PhaseFilter)
        SearchProduct()

        upFacets.Update()

    End Sub

#End Region

#Region "Functions"

    Private Sub Add2BreadCrumb(ByVal Text As String, ByVal link As String)
        If Not m_Breadcrumb = String.Empty Then
            m_Breadcrumb &= " <span>&gt;</span> "
        End If

        If Text = "Clear" Or Text = "Clear All" Then
            m_Breadcrumb &= "<a onclick=""javascript:ClearKeyword();"" href=""" & link & """>" & Text & "</a>"
        Else
            m_Breadcrumb &= "<a href=""" & link & """>" & Text & "</a>"
        End If

    End Sub

    Private Sub Add2BreadCrumb(ByVal Text As String)
        If Not m_Breadcrumb = String.Empty Then
            m_Breadcrumb &= " <span>&gt;</span> "
        End If
        m_Breadcrumb &= Text
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
        End If
    End Sub

    Private Sub ExpandChildren(ByVal n As FlyTreeNode)
        For Each child As FlyTreeNode In n.ChildNodes
            child.Expand()
            If child.ChildNodes.Count > 0 Then
                ExpandChildren(child)
            End If
        Next
    End Sub

#End Region

#Region "FlyTreeView"

    Protected Sub flyTreeView_PopulateNodes(ByVal sender As Object, ByVal e As NineRays.WebControls.FlyTreeNodeEventArgs)

        If CatalogType = CATALOG_TYPE.CATALOG_TYPE_ALL Then
            TreeViewAddNodes(e.Node.ChildNodes, e.Node.Value, Nothing, True)
        Else
            TreeViewAddNodes(e.Node.ChildNodes, e.Node.Value, Nothing, False)
        End If

        ShowHideNodes(e.Node.ChildNodes, PhaseFilter)

    End Sub

    Protected Sub flyTreeView_NodeSelected(ByVal sender As Object, ByVal e As NineRays.WebControls.FlyTreeNodeEventArgs)

        RaiseEvent OnTreeNodeSelect(Me, e)

        If CatalogType = CATALOG_TYPE.CATALOG_TYPE_MARKET Or CatalogType = CATALOG_TYPE.CATALOG_TYPE_VENDOR Then
            GetProductListForSupplyPhase(Convert.ToInt32(e.Node.Value))
        Else
            GetMatchingProductList(Convert.ToInt32(e.Node.Value), "")
        End If

    End Sub

    Private Sub ShowHideNodes(ByVal Nodes As FlyTreeNodeCollection, ByRef FilterList() As String)

        Dim NodeArray As New ArrayList()

        For Each node As FlyTreeNode In Nodes
            If Not FilterList Is Nothing AndAlso Not FilterList.Contains(node.Value) Then
                If Not node.Value = 0 Then
                    NodeArray.Add(node)
                End If
            End If
            ' ShowHideNodes(node.ChildNodes, FilterList)
        Next

        For Each node As FlyTreeNode In NodeArray
            node.Remove()
        Next

    End Sub

    Private Sub TreeViewAddNodes(ByVal nodes As FlyTreeNodeCollection, ByVal ParentId As Integer, ByVal SelectedNode As FlyTreeNode, Optional ByVal pAllProduct As Boolean = False, Optional ByVal pIgnoreSelectedNode As Boolean = False)

        Dim objDB As New Database
        Dim sp_name As String = ""

        objDB.Open(DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword")))
        'objDB.Open(DBConnectionString.GetConnectionString("Persist Security Info=True;Initial Catalog=CBUSA_AE;Data Source=DESKTOP-IR2TQ52;User ID=sa;Password=medullus", "", ""))

        Dim params(0) As SqlParameter

        If pAllProduct = True Then
            params(0) = New SqlParameter("@SupplyPhaseID", 0)
            sp_name = "sp_GetSupplyPhaseChainForSupplyPhase"

            If dtPhases Is Nothing Then
                dtPhases = New DataTable()
                objDB.RunProc(sp_name, params, dtPhases)
            End If
        Else
            If Session("TwoPriceLLCID") IsNot Nothing Then
                ReDim params(2)

                params(0) = New SqlParameter("@LLCID", Session("TwoPriceLLCID"))

                If pIgnoreSelectedNode = False AndAlso Not SelectedNode Is Nothing Then
                    params(1) = New SqlParameter("@SupplyPhaseID", Convert.ToInt32(SelectedNode.Value))
                Else
                    params(1) = New SqlParameter("@SupplyPhaseID", 0)
                End If

                If Not Session("CurrentPreferredVendor") Is Nothing Then
                    params(2) = New SqlParameter("@PreferredVendorID", Session("CurrentPreferredVendor"))
                Else
                    params(2) = New SqlParameter("@PreferredVendorID", 0)
                End If

                sp_name = "sp_GetSupplyPhaseChainForLLC"

            ElseIf Session("BuilderId") IsNot Nothing Then
                ReDim params(3)

                params(0) = New SqlParameter("@BuilderID", Session("BuilderId"))

                If pIgnoreSelectedNode = False AndAlso Not SelectedNode Is Nothing Then
                    params(1) = New SqlParameter("@SupplyPhaseID", Convert.ToInt32(SelectedNode.Value))
                Else
                    params(1) = New SqlParameter("@SupplyPhaseID", 0)
                End If

                If Not Session("CurrentPreferredVendor") Is Nothing Then
                    params(2) = New SqlParameter("@PreferredVendorID", Session("CurrentPreferredVendor"))
                Else
                    params(2) = New SqlParameter("@PreferredVendorID", 0)
                End If

                If FilterList IsNot Nothing Then
                    Dim strProductIDs As String = String.Join(",", FilterList.ToArray())
                    params(3) = New SqlParameter("@ProductList", strProductIDs)
                Else
                    params(3) = New SqlParameter("@ProductList", Nothing)
                End If

                sp_name = "sp_GetSupplyPhaseChainForBuilder"

            ElseIf Session("VendorId") IsNot Nothing Then
                ReDim params(2)

                params(0) = New SqlParameter("@VendorID", Session("VendorId"))

                If pIgnoreSelectedNode = False AndAlso Not SelectedNode Is Nothing Then
                    params(1) = New SqlParameter("@SupplyPhaseID", Convert.ToInt32(SelectedNode.Value))
                Else
                    params(1) = New SqlParameter("@SupplyPhaseID", 0)
                End If

                If CatalogType = CATALOG_TYPE.CATALOG_TYPE_VENDOR Then
                    params(2) = New SqlParameter("@LLCID", 0)
                ElseIf CatalogType = CATALOG_TYPE.CATALOG_TYPE_MARKET Then
                    params(2) = New SqlParameter("@LLCID", 1)
                End If

                sp_name = "sp_GetSupplyPhaseChainForVendor"
            End If

            If dtPhases Is Nothing Then
                dtPhases = New DataTable()
                objDB.RunProc(sp_name, params, dtPhases)
            End If
        End If

        objDB.Close()

        If ParentId = 0 Then
            ParentId = SupplyPhaseRow.GetRootSupplyPhase(DB).SupplyPhaseID
        End If

        dtPhases.DefaultView.RowFilter = "ParentSupplyPhaseId = " & ParentId
        dtPhases.DefaultView.Sort = "SupplyPhase"

        Dim PhaseFilterList As New Generic.List(Of String)
        For Each row As DataRowView In dtPhases.DefaultView
            Dim strSupplyPhase As String = row("SupplyPhaseID")
            If strSupplyPhase <> "ROOT" Then
                Dim split As Integer = strSupplyPhase.LastIndexOf("|")
                PhaseFilterList.Add(strSupplyPhase)
            End If
        Next
        PhaseFilter = PhaseFilterList.ToArray

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

        If pIgnoreSelectedNode = False Then
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
        End If

    End Sub

#End Region

#Region "SQL-Search"

    'New sub-routine for SQL-based product search :: by Apala (Medullus) - 4th July 2017
    '-----------------------------------------------------------------------------------

    Public Sub SearchProduct(Optional ByVal pSupplyPhaseID As Integer = 0, Optional ByVal pRenderTreeView As Boolean = True, Optional ByVal pIgnoreSelectedNode As Boolean = False)

        Dim strSearchKeyword As String = txtKeyword.Text.Trim()
        Dim IgnoreSelectedNode As Boolean = IIf(pIgnoreSelectedNode = False, False, True)

        Add2BreadCrumb("Clear All", Page.ClientScript.GetPostBackClientHyperlink(btnSearch, "clear"))
        If Not Keywords = String.Empty Then
            Dim qs As New URLParameters()
            qs.Add("f", "keywords")
            qs.Add("keywords", Keywords)
            Add2BreadCrumb("<a href=""" & Page.ClientScript.GetPostBackClientHyperlink(btnSearch, qs.ToString) & """><b>" & Keywords & "</b></a><a href=""" & Page.ClientScript.GetPostBackClientHyperlink(btnSearch, "clear") & """><sup>x</sup></a>")
        End If

        Dim selSupplyPhaseID As Integer = 0

        If Not flyTreeView.SelectedNode Is Nothing Then
            selSupplyPhaseID = flyTreeView.SelectedNode.Value
        Else
            selSupplyPhaseID = pSupplyPhaseID
        End If

        If CatalogType = CATALOG_TYPE.CATALOG_TYPE_MARKET Then
            If pRenderTreeView = True Then
                TreeViewAddNodes(flyTreeView.Nodes(0).ChildNodes, 0, flyTreeView.SelectedNode, False, IgnoreSelectedNode)
            End If

            If Not strSearchKeyword = String.Empty AndAlso strSearchKeyword <> "Keyword Search" Then
                GetProductListForSupplyPhase(0, strSearchKeyword.Replace("'", "*"))
            Else
                GetProductListForSupplyPhase(selSupplyPhaseID)
            End If
        ElseIf CatalogType = CATALOG_TYPE.CATALOG_TYPE_VENDOR Then
            If pRenderTreeView = True Then
                TreeViewAddNodes(flyTreeView.Nodes(0).ChildNodes, 0, flyTreeView.SelectedNode, False, IgnoreSelectedNode)
            End If

            If Not strSearchKeyword = String.Empty AndAlso strSearchKeyword <> "Keyword Search" Then
                GetProductListForSupplyPhase(0, strSearchKeyword.Replace("'", "*"))
            Else
                GetProductListForSupplyPhase(selSupplyPhaseID)
            End If
        Else
            If pRenderTreeView = True Then
                TreeViewAddNodes(flyTreeView.Nodes(0).ChildNodes, 0, flyTreeView.SelectedNode, True, IgnoreSelectedNode)
            End If

            If Not strSearchKeyword = String.Empty AndAlso strSearchKeyword <> "Keyword Search" Then
                GetMatchingProductList(0, strSearchKeyword.Replace("'", "*"))
            Else
                GetMatchingProductList(selSupplyPhaseID)
            End If
        End If

        ShowHideNodes(flyTreeView.Nodes(0).ChildNodes, PhaseFilter)

    End Sub

    Public Sub GetMatchingProductList(Optional ByVal pSupplyPhaseID As Integer = 0, Optional ByVal pSearchText As String = "")

        Try
            Dim objDB As New Database
            Dim sr As DataTable = Nothing
            Dim sp_name As String = "sp_GetAllProductList"

            objDB.Open(DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword")))

            Dim params(3) As SqlParameter
            params(0) = New SqlParameter("@SupplyPhaseID", pSupplyPhaseID)
            params(1) = New SqlParameter("@SearchText", IIf(pSearchText = "", DBNull.Value, pSearchText))
            params(2) = New SqlParameter("@PageSize", PageSize)
            params(3) = New SqlParameter("@PageNumber", PageNumber)

            sr = New DataTable()
            objDB.RunProc(sp_name, params, sr)

            upFacets.Update()
            m_SearchResults = sr

            objDB.Close()

            RaiseEvent ResultsUpdated(Me, EventArgs.Empty)

        Catch ex As Exception

        End Try

    End Sub

    Public Sub GetProductListForSupplyPhase(Optional ByVal pSupplyPhaseID As Integer = 0, Optional ByVal pSearchText As String = "")

        Try
            Dim objDB As New Database
            Dim sr As DataTable = Nothing
            Dim sp_name As String = ""

            objDB.Open(DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword")))

            Dim params(0) As SqlParameter


            If Session("TwoPriceLLCID") IsNot Nothing Then
                ReDim params(4)

                sp_name = "sp_GetProductListForLLC"

                params(0) = New SqlParameter("@LLCID", Session("TwoPriceLLCID"))
                params(1) = New SqlParameter("@SupplyPhaseID", pSupplyPhaseID)
                params(2) = New SqlParameter("@SearchText", IIf(pSearchText = "", DBNull.Value, pSearchText))
                params(3) = New SqlParameter("@PageSize", PageSize)
                params(4) = New SqlParameter("@PageNumber", PageNumber)

            ElseIf Session("BuilderId") IsNot Nothing Then
                ReDim params(6)

                sp_name = "sp_GetProductListForBuilder"

                params(0) = New SqlParameter("@BuilderID", Session("BuilderId"))
                params(1) = New SqlParameter("@SupplyPhaseID", pSupplyPhaseID)
                params(2) = New SqlParameter("@SearchText", IIf(pSearchText = "", DBNull.Value, pSearchText))
                params(3) = New SqlParameter("@PreferredVendorID", IIf(Session("CurrentPreferredVendor") Is Nothing, 0, Session("CurrentPreferredVendor")))
                params(4) = New SqlParameter("@PageSize", PageSize)
                params(5) = New SqlParameter("@PageNumber", PageNumber)

                If FilterList IsNot Nothing Then
                    Dim strProductIDs As String = String.Join(",", FilterList.ToArray())
                    params(6) = New SqlParameter("@ProductList", strProductIDs)
                Else
                    params(6) = New SqlParameter("@ProductList", Nothing)
                End If

            ElseIf Session("VendorId") IsNot Nothing Then
                ReDim params(5)

                sp_name = "sp_GetProductListForVendor"

                params(0) = New SqlParameter("@VendorID", Session("VendorId"))
                params(1) = New SqlParameter("@SupplyPhaseID", pSupplyPhaseID)
                params(2) = New SqlParameter("@SearchText", IIf(pSearchText = "", DBNull.Value, pSearchText))
                params(3) = New SqlParameter("@LLCID", IIf(CatalogType = CATALOG_TYPE.CATALOG_TYPE_MARKET, 1, 0))
                params(4) = New SqlParameter("@PageSize", PageSize)
                params(5) = New SqlParameter("@PageNumber", PageNumber)
            End If

            sr = New DataTable()
            objDB.RunProc(sp_name, params, sr)

            upFacets.Update()
            m_SearchResults = sr

            objDB.Close()

            RaiseEvent ResultsUpdated(Me, EventArgs.Empty)

        Catch ex As Exception

        End Try

    End Sub

    Public Sub SetCatalogType(ByVal pCatalogType As CATALOG_TYPE)
        CatalogType = pCatalogType
    End Sub

#End Region

End Class
