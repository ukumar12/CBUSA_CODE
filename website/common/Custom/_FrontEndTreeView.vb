Imports Components
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Collections.Specialized
Imports System.Text.RegularExpressions
Imports System.Threading

Namespace Controls
    Public Class FrontEndTreeView
        Inherits DataBoundControl
        Implements IPostBackEventHandler

        Private Const CacheRefresh As Integer = 5

        Public Event SelectedIndexChanged As EventHandler
        Public Event TreeStateChanged As eventhandler

        Private m_AsyncTrigger As UnvalidatedPostback
        Public ReadOnly Property AsyncTrigger() As UnvalidatedPostback
            Get
                Return m_AsyncTrigger
            End Get
        End Property

        Private m_SyncTrigger As UnvalidatedPostback
        Public ReadOnly Property SyncTrigger() As UnvalidatedPostback
            Get
                Return m_SyncTrigger
            End Get
        End Property

        Public ReadOnly Property IsCached() As Boolean
            Get
                'caching was too much trouble too close to deadline; should improve performance if fixed though
                'Return NodesDict.Count > 0
                Return False
            End Get
        End Property

        Public Property RootLabel() As String
            Get
                Return ViewState("RootLabel")
            End Get
            Set(ByVal value As String)
                ViewState("RootLabel") = value
            End Set
        End Property

        Public Property RootParentId() As Integer
            Get
                If ViewState("RootParentId") Is Nothing Then
                    ViewState("RootParentId") = -1
                End If
                Return ViewState("RootParentId")
            End Get
            Set(ByVal value As Integer)
                ViewState("RootParentId") = value
            End Set
        End Property

        Private Shared LockObject As New Object

        Private m_RootNode As FrontEndTreeNode
        Public ReadOnly Property RootNode() As FrontEndTreeNode
            Get
                Return m_RootNode
                'If NodesDict.ContainsKey(RootParentId) Then
                '    Return NodesDict.Item(RootParentId).Item(0)
                'Else
                '    Return Nothing
                'End If
            End Get
        End Property

        Private m_NodesDict As Generic.Dictionary(Of Integer, Generic.List(Of FrontEndTreeNode))
        Private ReadOnly Property NodesDict() As Generic.Dictionary(Of Integer, Generic.List(Of FrontEndTreeNode))
            Get
                'm_NodesDict = HttpContext.Current.Session("SupplyPhaseTreeNodes" & UniqueID)
                'If m_NodesDict Is Nothing Then
                '    m_NodesDict = New Generic.Dictionary(Of Integer, Generic.List(Of FrontEndTreeNode))
                '    HttpContext.Current.Session("SupplyPhaseTreeNodes" & UniqueID) = m_NodesDict
                '    'HttpContext.Current.Items.Add("SupplyPhaseTreeNodes" & UniqueID, m_NodesDict)
                '    'HttpContext.Current.Cache.Add("SupplyPhaseTreeNodes" & UniqueID, m_NodesDict, Nothing, nothing, New TimeSpan(0,5, Caching.CacheItemPriority.Default, Nothing)
                'End If
                If m_NodesDict Is Nothing Then
                    m_NodesDict = New Generic.Dictionary(Of Integer, Generic.List(Of FrontEndTreeNode))
                End If
                Return m_NodesDict
            End Get
        End Property

        Private m_DataKeyField As String
        Public Property DataKeyField() As String
            Get
                Return IIf(m_DataKeyField = Nothing, DataValueField, m_DataKeyField)
            End Get
            Set(ByVal value As String)
                m_datakeyfield = value
            End Set
        End Property

        Private m_DataParentField As String
        Public Property DataParentField() As String
            Get
                Return m_DataParentField
            End Get
            Set(ByVal value As String)
                m_DataParentField = value
            End Set
        End Property

        Private m_DataTextField As String
        Public Property DataTextField() As String
            Get
                Return m_DataTextField
            End Get
            Set(ByVal value As String)
                m_DataTextField = value
            End Set
        End Property

        Private m_DataValueField As String
        Public Property DataValueField() As String
            Get
                Return m_DataValueField
            End Get
            Set(ByVal value As String)
                m_DataValueField = value
            End Set
        End Property

        Public ReadOnly Property Value() As String
            Get
                If CurrentNode Is Nothing Then
                    Return Nothing
                Else
                    Return CurrentNode.Value
                End If
            End Get
        End Property

        Public Property UseFilter() As Boolean
            Get
                Return ViewState("UseFilter")
            End Get
            Set(ByVal value As Boolean)
                ViewState("UseFilter") = value
            End Set
        End Property

        Private m_CurrentNode As FrontEndTreeNode
        Public ReadOnly Property CurrentNode() As FrontEndTreeNode
            Get
                Return m_CurrentNode
            End Get
        End Property

        Public Property ExpandList() As Generic.List(Of String)
            Get
                If ViewState("ExpandList") Is Nothing Then
                    ViewState("ExpandList") = New Generic.List(Of String)
                End If
                Return ViewState("ExpandList")
            End Get
            Set(ByVal value As Generic.List(Of String))
                ViewState("ExpandList") = value
            End Set
        End Property

        Public Property FilterList() As Generic.List(Of String)
            Get
                If ViewState("FilterList") Is Nothing Then
                    ViewState("FilterList") = New Generic.List(Of String)
                End If
                Return ViewState("FilterList")
            End Get
            Set(ByVal value As Generic.List(Of String))
                ViewState("FilterList") = value
            End Set
        End Property

        Public Sub ExpandAll()
            ExpandList.Clear()
            For Each list As Generic.List(Of FrontEndTreeNode) In NodesDict.Values
                For Each n As FrontEndTreeNode In list
                    ExpandList.Add(n.Key)
                Next
            Next
        End Sub

        Public Sub CollapseAll()
            ExpandList.Clear()
        End Sub

        Public Function FindNodeByKey(ByVal Key As Integer) As FrontEndTreeNode
            Return FindNodeByKey(Key, RootNode)

            'For Each list As Generic.List(Of FrontEndTreeNode) In NodesDict.Values
            '    For Each node As FrontEndTreeNode In list
            '        If node.Key = Key Then
            '            Return node
            '        End If
            '    Next
            'Next
            'Return Nothing
        End Function

        Private Function FindNodeByKey(ByVal Key As Integer, ByVal Parent As FrontEndTreeNode) As FrontEndTreeNode
            If Parent.Key = Key Then
                Return Parent
            Else
                For Each child As FrontEndTreeNode In Parent.Children
                    Dim temp As FrontEndTreeNode = FindNodeByKey(Key, child)
                    If temp IsNot Nothing Then
                        Return temp
                    End If
                Next
            End If
            Return Nothing
        End Function

        Private Sub BuildState(ByVal Node As FrontEndTreeNode, ByRef State As Text.StringBuilder)
            Dim parentKey As Integer
            If Node.Parent Is Nothing Then
                parentKey = -1
            Else
                parentKey = Node.Parent.Key
            End If
            State.Append(CustomEscape(parentKey) & "," & CustomEscape(Node.Key) & "," & CustomEscape(Node.Name) & "," & CustomEscape(Node.Value) & "|")
            For Each child As FrontEndTreeNode In Node.Children
                BuildState(child, State)
            Next
        End Sub

        'Protected Overrides Function SaveViewState() As Object
        '    Dim base As Object = MyBase.SaveViewState
        '    Dim mystate As New Text.StringBuilder
        '    Dim conn As String = String.Empty

        '    'For Each n As FrontEndTreeNode In AllNodes
        '    '    Dim parentKey As Integer
        '    '    If n.Parent Is Nothing Then
        '    '        parentKey = -1
        '    '    Else
        '    '        parentKey = n.Parent.Key
        '    '    End If
        '    '    mystate.Append(conn & CustomEscape(parentKey) & "," & CustomEscape(n.Key) & "," & CustomEscape(n.Name) & "," & CustomEscape(n.Value) & "," & Convert.ToInt32(n.Expanded))
        '    '    conn = "|"
        '    'Next

        '    'If base Is Nothing Then
        '    'Return mystate.ToString
        '    'Else
        '    BuildState(RootNode, mystate)

        '    Return New Pair(base, mystate.ToString)
        '    'End If
        'End Function

        Protected Overrides Sub LoadViewState(ByVal savedState As Object)
            'Dim state As Pair = savedState
            'Dim mystate As String = state.Second
            'MyBase.LoadViewState(state.First)

            'Dim re As New Regex("(?:^|(?:(?!\\)\|))(?<ParentKey>(?:[^,]*|(?:\\,))*),(?<Key>(?:[^,\\]*|(?:\\,)|(?:(?<!,)\\))*),(?<Name>(?:[^,\\]*|(?:\\,)|(?:(?<!,)\\))*),(?<Value>(?:[^,\\\|]*|(?:\\,)|(?:\\\|)|(?:(?<!,)\\)|(?:(?<!\|)\\))*)")
            'Dim matches As MatchCollection = re.Matches(mystate)
            'NodesDict.Clear()
            'Try
            '    For Each m As Match In matches
            '        Dim node As New FrontEndTreeNode(CustomUnEscape(m.Groups("Key").Value), CustomUnEscape(m.Groups("Name").Value), CustomUnEscape(m.Groups("Value").Value))
            '        Dim parentKey As String = CustomUnEscape(m.Groups("ParentKey").Value)
            '        If Not NodesDict.ContainsKey(parentKey) Then
            '            NodesDict.Add(parentKey, New Generic.List(Of FrontEndTreeNode))
            '        End If
            '        NodesDict(parentKey).Add(node)
            '        If (RootParentId = Nothing And parentKey = -1) Or (RootParentId <> Nothing And parentKey = RootParentId) Then
            '            m_RootNode = node
            '        End If
            '    Next

            'Catch ex As Exception
            '    Logger.Error(Logger.GetErrorMessage(ex))
            'End Try
            'If RootNode Is Nothing Then
            '    Throw New ApplicationException("Root node not found in loaded ViewState")
            'Else
            '    AddChildren(RootNode)
            'End If

            MyBase.LoadViewState(savedState)
            m_RootNode = HttpContext.Current.Application("SupplyPhaseRoot")
        End Sub

        Private Function CustomEscape(ByVal str As String) As String
            Return str.Replace("\", "\\").Replace(",", "\,").Replace("|", "\|")
        End Function

        Private Function CustomUnEscape(ByVal str As String) As String
            Return str.Replace("\|", "|").Replace("\,", ",").Replace("\\", "\")
        End Function

        Protected Overrides Sub PerformSelect()
            Monitor.Enter(LockObject)
            Try
                If HttpContext.Current.Application("SupplyPhaseRoot") IsNot Nothing Then
                    m_RootNode = HttpContext.Current.Application("SupplyPhaseRoot")
                    RequiresDataBinding = False
                    MarkAsDataBound()
                    Exit Sub
                End If

                If Not IsBoundUsingDataSourceID Then
                    OnDataBinding(EventArgs.Empty)
                End If

                GetData().Select(CreateDataSourceSelectArguments(), AddressOf OnDataSourceViewSelectCallback)

                RequiresDataBinding = False
                MarkAsDataBound()
            Catch ex As Exception
                Logger.Error(Logger.GetErrorMessage(ex))
            Finally
                Monitor.Exit(LockObject)
            End Try

            OnDataBound(EventArgs.Empty)
        End Sub

        Private Sub OnDataSourceViewSelectCallback(ByVal data As IEnumerable)
            If data IsNot Nothing Then
                NodesDict.Clear()
                For Each item As Object In data
                    Dim node As New FrontEndTreeNode(DataBinder.GetPropertyValue(item, DataKeyField), DataBinder.GetPropertyValue(item, DataTextField), DataBinder.GetPropertyValue(item, DataValueField))
                    Dim parentId As Object = DataBinder.GetPropertyValue(item, DataParentField)
                    Dim ParentKey As Integer = IIf(IsDBNull(parentId), -1, parentId)
                    node.ParentKey = ParentKey
                    If Not NodesDict.ContainsKey(ParentKey) Then
                        NodesDict.Add(ParentKey, New Generic.List(Of FrontEndTreeNode))
                    End If
                    NodesDict(ParentKey).Add(node)
                    If RootParentId = ParentKey Then
                        m_RootNode = node
                    End If
                Next
                If RootNode Is Nothing Then
                    Throw New ApplicationException("Root Node could not be found in Data Source")
                Else
                    AddChildren(RootNode)
                End If

                HttpContext.Current.Application.Lock()
                HttpContext.Current.Application("SupplyPhaseRoot") = RootNode
                HttpContext.Current.Application.UnLock()
            End If
        End Sub

        Private Sub AddChildren(ByVal parent As FrontEndTreeNode)
            If NodesDict.ContainsKey(parent.Key) Then
                For Each child As FrontEndTreeNode In NodesDict.Item(parent.Key)
                    child.Parent = parent
                    parent.Children.Add(child)
                    AddChildren(child)
                Next
            End If
        End Sub

        Public Overrides Sub RenderBeginTag(ByVal writer As System.Web.UI.HtmlTextWriter)
            'MyBase.RenderBeginTag(writer)
        End Sub

        Public Overrides Sub RenderEndTag(ByVal writer As System.Web.UI.HtmlTextWriter)
            'MyBase.RenderEndTag(writer)
        End Sub

        Private childWriter As HtmlTextWriter
        Private childStream As IO.StringWriter
        Private childBuilder As Text.StringBuilder

        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            'writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID)
            'writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID)
            'writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden")
            'writer.AddAttribute(HtmlTextWriterAttribute.Value, "empty")
            'writer.RenderBeginTag("input")
            'writer.RenderEndTag()

            MyBase.Render(writer)

            Dim h As Integer = Height.Value
            If h = Nothing And Style.Item("height") <> Nothing Then
                h = Regex.Replace(Style.Item("height"), "[\D]", "")
            End If

            If h <> Nothing Then
                writer.AddStyleAttribute("height", CStr(h) & "px")
                writer.AddStyleAttribute("overflow", "auto")
            End If
            writer.AddAttribute(HtmlTextWriterAttribute.Id, "treelist")
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "treelist")
            writer.RenderBeginTag(HtmlTextWriterTag.Ul)

            childBuilder = New Text.StringBuilder()
            childStream = New IO.StringWriter(childBuilder)
            childWriter = New HtmlTextWriter(childStream)

            If RootLabel <> Nothing Then
                If Not ExpandList.Contains(RootNode.Key) Then
                    ExpandList.Add(RootNode.Key)
                End If
                If Not FilterList.Contains(RootNode.Key) Then
                    FilterList.Add(RootNode.Key)
                End If
                Dim ChildHTML As String = String.Empty
                RenderNode(RootNode, ChildHTML, New Boolean, New Boolean)
                If ChildHTML <> String.Empty Then
                    writer.Write(ChildHTML)
                End If
            Else
                For Each child As FrontEndTreeNode In RootNode.Children.OrderBy(Of String)(Function(n) n.Name)
                    Dim ChildHTML As String = String.Empty
                    RenderNode(child, ChildHTML, New Boolean, New Boolean)
                    If ChildHTML <> String.Empty Then
                        writer.Write(ChildHTML)
                    End If
                Next
            End If

            childWriter.Close()
            childWriter = Nothing
            childStream.Close()
            childStream = Nothing
            childBuilder.Length = 0
            childBuilder = Nothing

            writer.RenderEndTag() '/ul
        End Sub

        Private Sub RenderNode(ByVal node As FrontEndTreeNode, ByRef SubHTML As String, ByRef IsExpanded As Boolean, ByRef IsUnfiltered As Boolean)
            Dim ChildHTML As New Text.StringBuilder

            Dim childrenUnfiltered As Boolean = False
            IsUnfiltered = FilterList.Count = 1 OrElse FilterList.Contains(node.Value)
            IsExpanded = ExpandList.Contains(node.Key)

            If node.Children.Count > 0 Then

                For Each child As FrontEndTreeNode In node.Children.OrderBy(Of String)(Function(n) n.Name)
                    Dim TempHTML As String = String.Empty
                    Dim ChildExpanded As Boolean = False
                    Dim ChildUnfiltered As Boolean = False
                    RenderNode(child, TempHTML, ChildExpanded, ChildUnfiltered)
                    'IsExpanded = IsExpanded Or ChildExpanded
                    childrenUnfiltered = childrenUnfiltered Or ChildUnfiltered
                    If ChildUnfiltered Then
                        ChildHTML.Append(TempHTML)
                    End If
                Next
            End If

            IsUnfiltered = IsUnfiltered Or childrenUnfiltered Or (Not UseFilter)

            If Not IsUnfiltered Then
                SubHTML = String.Empty
                Exit Sub
            Else
                'Dim out As New Text.StringBuilder()
                'Dim sw As New IO.StringWriter(out)
                'Dim writer As New HtmlTextWriter(sw)
                childBuilder.Length = 0

                childWriter.RenderBeginTag(HtmlTextWriterTag.Li)

                If childrenUnfiltered Then
                    If IsExpanded Then
                        childWriter.AddAttribute(HtmlTextWriterAttribute.Src, "/images/global/tree/minus.gif")
                    Else
                        childWriter.AddAttribute(HtmlTextWriterAttribute.Src, "/images/global/tree/plus.gif")
                    End If
                    'childWriter.AddAttribute(HtmlTextWriterAttribute.Onclick, Page.ClientScript.GetPostBackEventReference(Me, False & "|" & node.Key))
                    childWriter.AddAttribute(HtmlTextWriterAttribute.Onclick, Page.ClientScript.GetPostBackEventReference(AsyncTrigger, False & "|" & node.Key))
                Else
                    childWriter.AddAttribute(HtmlTextWriterAttribute.Src, "/images/spacer.gif")
                    childWriter.AddStyleAttribute(HtmlTextWriterStyle.Width, "18px")
                End If
                childWriter.RenderBeginTag(HtmlTextWriterTag.Img)
                childWriter.RenderEndTag()

                'writer.AddAttribute(HtmlTextWriterAttribute.Onclick, Page.ClientScript.GetPostBackEventReference(Me, node.Key))
                'childWriter.AddAttribute(HtmlTextWriterAttribute.Href, Page.ClientScript.GetPostBackClientHyperlink(Me, True & "|" & node.Key))
                childWriter.AddAttribute(HtmlTextWriterAttribute.Href, Page.ClientScript.GetPostBackClientHyperlink(SyncTrigger, True & "|" & node.Key))
                childWriter.AddAttribute(HtmlTextWriterAttribute.Id, ClientID & ClientIDSeparator & node.Key)
                childWriter.RenderBeginTag(HtmlTextWriterTag.A)
                childWriter.Write(IIf(node Is RootNode, RootLabel, node.Name))
                childWriter.RenderEndTag() '/a

                If IsExpanded Then
                    childWriter.RenderBeginTag(HtmlTextWriterTag.Ul)

                    childWriter.Write(ChildHTML.ToString)

                    childWriter.RenderEndTag() '/ul
                End If
                childWriter.RenderEndTag() '/li

                'writer.Close()
                'sw.Close()

                SubHTML = childBuilder.ToString
            End If
        End Sub

        Public Sub HandlePostback(ByVal sender As Object, ByVal e As System.EventArgs)
            RaisePostBackEvent(HttpContext.Current.Request("__EVENTARGUMENT"))
        End Sub

        Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent
            'If HttpContext.Current.Request("__EVENTTARGET") <> UniqueID Then
            '    'extremely hacky, but .NET handling of postback handlers is ridiculous
            '    Dim ctl As IPostBackEventHandler = TryCast(Page.FindControl(HttpContext.Current.Request("__EVENTTARGET")), IPostBackEventHandler)
            '    If ctl IsNot Nothing Then
            '        If eventArgument = String.Empty And HttpContext.Current.Request("__EVENTARGUMENT") IsNot Nothing Then
            '            eventArgument = HttpContext.Current.Request("__EVENTARGUMENT")
            '        End If
            '        ctl.RaisePostBackEvent(eventArgument)
            '    End If
            '    Exit Sub
            'End If
            If eventArgument = Nothing Then
                eventArgument = HttpContext.Current.Request("__EVENTARGUMENT")
            End If
            Dim split As Integer = eventArgument.IndexOf("|")

            Dim doRaise As Boolean = eventArgument.Substring(0, split)
            Dim key As String = eventArgument.Substring(split + 1, eventArgument.Length - split - 1)

            If ExpandList.Contains(key) Then
                If Not doRaise Then
                    ExpandList.Remove(key)
                End If
            Else
                ExpandList.Add(key)
            End If
            'If Not ExpandList.Remove(key) Then
            '    ExpandList.Add(key)
            'End If
            If doRaise Then
                Dim n As FrontEndTreeNode = FindNodeByKey(key)
                m_CurrentNode = n
                RaiseEvent SelectedIndexChanged(Me, EventArgs.Empty)
            Else
                RaiseEvent TreeStateChanged(Me, EventArgs.Empty)
            End If
        End Sub

        Private Sub FrontEndTreeView_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            m_AsyncTrigger = New UnvalidatedPostback()
            m_AsyncTrigger.ID = "btnAsyncTrigger"
            Controls.Add(m_AsyncTrigger)
            Dim sm As ScriptManager = ScriptManager.GetCurrent(Page)
            If sm IsNot Nothing Then
                sm.RegisterAsyncPostBackControl(m_AsyncTrigger)
            End If
            AddHandler m_AsyncTrigger.Postback, AddressOf HandlePostback

            m_SyncTrigger = New UnvalidatedPostback()
            m_SyncTrigger.ID = "btnSyncTrigger"
            Controls.Add(m_SyncTrigger)
            If sm IsNot Nothing Then
                sm.RegisterPostBackControl(m_SyncTrigger)
            End If
            AddHandler m_SyncTrigger.Postback, AddressOf HandlePostback
        End Sub

        Private Sub FrontEndTreeView_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Page.ClientScript.IsClientScriptIncludeRegistered("folder-tree-static") Then
                'Page.ClientScript.RegisterClientScriptInclude("folder-tree-static", ResolveClientUrl("/includes/folder-tree-static.js"))
            End If
        End Sub
    End Class

    Public Class FrontEndTreeNode
        Public Sub New(ByVal Key As Integer, ByVal Name As String, ByVal Value As String)
            Me.Key = Key
            Me.Name = Name
            Me.Value = Value

            Children = New Generic.List(Of FrontEndTreeNode)
        End Sub

        Public Key As Integer
        Public Name As String
        Public Value As String

        Private m_ParentKey As Integer
        Public Property ParentKey() As Integer
            Get
                If Parent Is Nothing Then
                    Return m_ParentKey
                Else
                    Return Parent.Value
                End If
            End Get
            Set(ByVal value As Integer)
                m_ParentKey = value
            End Set
        End Property
        Public Parent As FrontEndTreeNode
        Public Children As Generic.List(Of FrontEndTreeNode)
    End Class
End Namespace