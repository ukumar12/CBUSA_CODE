Imports Components
Imports System.Web.UI.WebControls
Imports System.Web.UI
Imports System.Text.RegularExpressions

Namespace Controls
#Region "CustomTreeView"
    Public Class CustomTreeView
        Inherits DataBoundControl
        Implements IPostBackDataHandler

        Public Delegate Sub NodeSelectedHandler(ByVal sender As Object, ByVal args As CustomTreeViewEventArgs)
        Public Event OnNodeSelected As NodeSelectedHandler

        Private m_Tree As TreeView
        Private m_Nodes As Hashtable
        Private m_RebuildControls As Boolean

        'Private Property m_Nodes() As Hashtable
        '    Get
        '        If ViewState("NodesTree") Is Nothing Then
        '            ViewState("NodesTree") = New Hashtable()
        '        End If
        '        Return ViewState("NodesTree")
        '    End Get
        '    Set(ByVal value As Hashtable)
        '        ViewState("NodesTree") = value
        '    End Set
        'End Property

        Public Sub New()
            MyBase.New()
            m_Tree = New TreeView
            m_Tree.ID = Me.ID
            Controls.Add(m_Tree)
            AddHandler m_Tree.SelectedNodeChanged, AddressOf OnSelectedNodeChanged

            m_Nodes = New Hashtable()
        End Sub

        Protected Sub OnSelectedNodeChanged(ByVal sender As Object, ByVal args As System.EventArgs)
            RaiseEvent OnNodeSelected(Me, New CustomTreeViewEventArgs(m_Tree.SelectedNode))
        End Sub

        Private m_NodeList As TreeNodeCollection
        Public ReadOnly Property Nodes() As TreeNodeCollection
            Get
                If m_NodeList Is Nothing Then
                    m_NodeList = New TreeNodeCollection
                End If
                Return m_NodeList
            End Get
        End Property

        Public ReadOnly Property SelectedNodes() As GenericCollection(Of CustomTreeNode)
            Get
                EnsureDataBound()
                Dim ret As New GenericCollection(Of CustomTreeNode)
                For Each node As CustomTreeNode In Nodes
                    If Value <> Nothing And Regex.IsMatch(Value, "(^|,)" & node.Value & "(,|$)") Then
                        ret.Add(node)
                    End If
                Next
                Return ret
            End Get
        End Property

        Public Property Type() As CustomTreeViewNodeType
            Get
                Return ViewState("Type")
            End Get
            Set(ByVal value As CustomTreeViewNodeType)
                ViewState("Type") = value
            End Set
        End Property

        Public Property RootId() As Integer
            Get
                Return ViewState("RootId")
            End Get
            Set(ByVal value As Integer)
                ViewState("RootId") = value
            End Set
        End Property

        Public Property ParentFieldName() As String
            Get
                Return ViewState("ParentFieldName")
            End Get
            Set(ByVal value As String)
                ViewState("ParentFieldName") = value
            End Set
        End Property

        Public Property DataValueName() As String
            Get
                Return ViewState("DataValueName")
            End Get
            Set(ByVal value As String)
                ViewState("DataValueName") = value
            End Set
        End Property

        Public Property DataTextName() As String
            Get
                Return ViewState("DataTextName")
            End Get
            Set(ByVal value As String)
                ViewState("DataTextName") = value
            End Set
        End Property

        Private m_OnClientNodeSelect As String
        Public Property OnClientNodeSelect() As String
            Get
                Return m_OnClientNodeSelect
            End Get
            Set(ByVal value As String)
                m_OnClientNodeSelect = value
            End Set
        End Property

        Public Property Value() As String
            Get
                Return ViewState("Value")
            End Get
            Set(ByVal value As String)
                ViewState("Value") = value
            End Set
        End Property

        Private Function NewNode(ByVal Text As String, ByVal Value As String) As CustomTreeNode
            Select Case Type
                Case CustomTreeViewNodeType.Checkbox
                    Return New CheckboxTreeNode(m_Tree, Text, Value, Me.UniqueID)
                Case CustomTreeViewNodeType.Radio
                    Return New RadioButtonTreeNode(m_Tree, Text, Value, Me.UniqueID)
                Case CustomTreeViewNodeType.Link
                    Return New LinkButtonTreeNode(m_Tree, Text, Value)
                Case Else
                    Return Nothing
            End Select
        End Function

        Protected Overrides Sub PerformDataBinding(ByVal data As System.Collections.IEnumerable)
            MyBase.PerformDataBinding(data)
            If data IsNot Nothing Then
                For Each item As Object In data
                    Dim n As CustomTreeNode = NewNode(DataBinder.GetPropertyValue(item, DataTextName), DataBinder.GetPropertyValue(item, DataValueName))
                    Dim parentId As Object = DataBinder.GetPropertyValue(item, ParentFieldName)

                    If IsDBNull(parentId) Then
                        parentId = "0"
                    Else
                        parentId = Convert.ToString(parentId)
                    End If

                    If Not m_Nodes.ContainsKey(parentId) Then
                        Dim al As New ArrayList
                        m_Nodes.Add(parentId, al)
                    End If

                    n.OnClientSelect = OnClientNodeSelect
                    m_Nodes(parentId).Add(n)
                    Nodes.Add(n)
                Next
                m_RebuildControls = True
            Else
                m_RebuildControls = False
            End If
        End Sub

        Protected Overrides Sub CreateChildControls()
			If m_RebuildControls Then
				MyBase.CreateChildControls()
				If Not m_Nodes.ContainsKey("0") Then
					Throw New ArgumentException("No root node could be found in the dataset")
				End If

				m_Tree.Nodes.Clear()
				Dim nodeList As ArrayList = m_Nodes("0")
				If nodeList.Count <> 1 Then
					Throw New ArgumentException("More than one root was found in the dataset")
				End If
				Dim n As CustomTreeNode = nodeList(0)
				m_Tree.Nodes.Add(n)
				If Value <> Nothing Then
					If (Type = CustomTreeViewNodeType.Radio Or Type = CustomTreeViewNodeType.Link) And n.Value = Value Then
						n.IsChecked = True
					ElseIf Type = CustomTreeViewNodeType.Checkbox And Regex.IsMatch(Value, "(?<=^|,)" & n.Value & "(?=,|$)") Then
						n.IsChecked = True
					End If
				End If
				n.Expanded = CreateChildren(n)
			End If
        End Sub

        Private Function CreateChildren(ByRef parent As TreeNode) As Boolean
            If Not m_Nodes.ContainsKey(parent.Value) Then Return False
            Dim nodeList As ArrayList = m_Nodes(parent.Value)
            Dim parentOpen As Boolean = False
            For Each n As CustomTreeNode In nodeList.OfType(Of CustomTreeNode).OrderBy(Of String)(Function(node) node.Text)
                n.Expanded = False
                If Value <> Nothing Then
                    If Type = CustomTreeViewNodeType.Radio And n.Value = Value Then
                        parentOpen = True
                        n.IsChecked = True
                    ElseIf Type = CustomTreeViewNodeType.Checkbox And Regex.IsMatch(Value, "(?<=^|,)" & n.Value & "(?=,|$)") Then
                        parentOpen = True
                        n.IsChecked = True
                    End If
                End If
                parent.ChildNodes.Add(n)
                If CreateChildren(n) Then
                    n.Expanded = True
                    parentOpen = True
                End If
            Next
            Return parentOpen
        End Function

        Public Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As System.Collections.Specialized.NameValueCollection) As Boolean Implements System.Web.UI.IPostBackDataHandler.LoadPostData
            If postCollection(postDataKey) <> Nothing Then
                Value = postCollection(postDataKey)
                Return True
            End If
            Return False
        End Function

        Public Sub RaisePostDataChangedEvent() Implements System.Web.UI.IPostBackDataHandler.RaisePostDataChangedEvent
        End Sub
    End Class

    <Serializable()> _
    Public MustInherit Class CustomTreeNode
        Inherits TreeNode

        Public Sub New(ByVal TreeView As TreeView, ByVal Text As String, ByVal Value As String)
            MyBase.New(Text, Value)
            m_TreeView = TreeView
        End Sub

        Private m_OnClientSelect As String
        Public Property OnClientSelect() As String
            Get
                Return m_OnClientSelect
            End Get
            Set(ByVal value As String)
                m_OnClientSelect = value
            End Set
        End Property

        Private m_TreeView As TreeView
        Public ReadOnly Property TreeView() As TreeView
            Get
                Return m_TreeView
            End Get
        End Property

        Public MustOverride Property IsChecked() As Boolean

        Public MustOverride Property GroupName() As String
    End Class

    Public Class CustomTreeViewEventArgs
        Public Sub New(ByVal SelectedNode As TreeNode)
            m_SelectedNode = SelectedNode
        End Sub

        Private m_SelectedNode As TreeNode
        Public Property SelectedNode() As TreeNode
            Get
                Return m_SelectedNode
            End Get
            Set(ByVal value As TreeNode)
                m_SelectedNode = value
            End Set
        End Property
    End Class

    <Serializable()> _
    Public Class LinkButtonTreeNode
        Inherits CustomTreeNode

        Public Sub New(ByVal TreeView As TreeView, ByVal Text As String, ByVal Value As String)
            MyBase.New(TreeView, Text, Value)
        End Sub

        'Abstract prop CustomTreeNode.GroupName unused
        Public Overrides Property GroupName() As String
            Get
                Return String.Empty
            End Get
            Set(ByVal value As String)
            End Set
        End Property

        Private m_IsChecked As Boolean
        Public Overrides Property IsChecked() As Boolean
            Get
                Return m_IsChecked
            End Get
            Set(ByVal value As Boolean)
                m_IsChecked = value
            End Set
        End Property
    End Class

    Public Class RadioButtonTreeNode
        Inherits CustomTreeNode

        Public Sub New(ByVal TreeView As TreeView, ByVal Text As String, ByVal Value As String, ByVal GroupName As String)
            MyBase.New(TreeView, Text, Value)
            m_GroupName = GroupName
        End Sub

        Private m_GroupName As String
        Public Overrides Property GroupName() As String
            Get
                Return m_GroupName
            End Get
            Set(ByVal value As String)
                m_GroupName = value
            End Set
        End Property

        Private m_IsChecked As Boolean
        Public Overrides Property IsChecked() As Boolean
            Get
                Return m_IsChecked
            End Get
            Set(ByVal value As Boolean)
                m_IsChecked = value
            End Set
        End Property

        Protected Overrides Sub RenderPreText(ByVal writer As System.Web.UI.HtmlTextWriter)
            MyBase.RenderPreText(writer)
        End Sub

        Protected Overrides Sub RenderPostText(ByVal writer As System.Web.UI.HtmlTextWriter)
            MyBase.RenderPostText(writer)
            writer.AddAttribute("type", "radio")
            writer.AddAttribute("value", Me.Value)
            writer.AddAttribute("name", GroupName)
            If IsChecked Then
                writer.AddAttribute("checked", "checked")
            End If
            If OnClientSelect <> Nothing Then
                writer.AddAttribute("onclick", OnClientSelect)
            End If
            writer.RenderBeginTag(HtmlTextWriterTag.Input)
            writer.RenderEndTag()
        End Sub
    End Class

    Public Class CheckboxTreeNode
        Inherits CustomTreeNode

        Public Sub New(ByVal TreeView As TreeView, ByVal Text As String, ByVal Value As String, ByVal GroupName As String)
            MyBase.New(TreeView, Text, Value)
            Me.GroupName = GroupName
        End Sub

        Private m_IsChecked As Boolean
        Public Overrides Property IsChecked() As Boolean
            Get
                Return m_IsChecked
            End Get
            Set(ByVal value As Boolean)
                m_IsChecked = value
            End Set
        End Property

        Private m_GroupName As String
        Public Overrides Property GroupName() As String
            Get
                Return m_GroupName
            End Get
            Set(ByVal value As String)
                m_GroupName = value
            End Set
        End Property

        Protected Overrides Sub RenderPreText(ByVal writer As System.Web.UI.HtmlTextWriter)
            MyBase.RenderPreText(writer)
        End Sub

        Protected Overrides Sub RenderPostText(ByVal writer As System.Web.UI.HtmlTextWriter)
            MyBase.RenderPostText(writer)
            writer.AddAttribute("type", "checkbox")
            writer.AddAttribute("value", Me.Value)
            writer.AddAttribute("name", GroupName)
            If IsChecked Then
                writer.AddAttribute("checked", "checked")
            End If
            If OnClientSelect <> Nothing Then
                writer.AddAttribute("onclick", OnClientSelect)
            End If
            writer.RenderBeginTag(HtmlTextWriterTag.Input)
            writer.RenderEndTag()
        End Sub
    End Class

#End Region

End Namespace

'enum outside of Controls NS to avoid strange compiler conflict
Public Enum CustomTreeViewNodeType
    Checkbox
    Radio
    Link
End Enum

