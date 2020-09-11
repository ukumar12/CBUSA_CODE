Imports Components
Imports DataLayer
Imports System.Web
Imports System.Web.UI
Imports System.Text.RegularExpressions

Namespace Controls
    Public Class MultiBox
        Inherits WebControls.BaseDataBoundControl
        Implements IPostBackDataHandler

        Public Event VendorsChanged As EventHandler

        Private LBox As WebControls.ListBox
        Private RBox As WebControls.ListBox

        Private m_ListWidth As Integer
        Public Property ListWidth() As Integer
            Get
                Return m_ListWidth
            End Get
            Set(ByVal value As Integer)
                m_ListWidth = value
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

        Private m_Items As GenericCollection(Of WebControls.ListItem)
        Public ReadOnly Property Items() As GenericCollection(Of WebControls.ListItem)
            Get
                If m_Items Is Nothing Then
                    m_Items = New GenericCollection(Of WebControls.ListItem)
                End If
                Return m_Items
            End Get
        End Property

        Public Property SelectedValues() As String
            Get
                If Items.Count = 0 Then
                    Return String.Empty
                Else
                    Dim query = From item As WebControls.ListItem In Items Where item.Selected Select item.Value
                    Dim out As New Text.StringBuilder
                    Dim conn As String = String.Empty
                    For Each value As String In query
                        out.Append(conn & value)
                        conn = ","
                    Next
                    Return out.ToString
                End If
            End Get
            Set(ByVal value As String)
                For Each item As WebControls.ListItem In Items
                    item.Selected = False
                Next
                If value = String.Empty Then
                    Exit Property
                Else
                    Dim vals As String() = value.Split(",")
                    For i As Integer = LBound(vals) To UBound(vals)
                        Dim tmp As String = vals(i)
                        Dim l As WebControls.ListItem = (From item As WebControls.ListItem In Items Where item.Value = tmp Select item).First
                        If l Is Nothing Then
                            Throw New ArgumentException("Invalid value in property SelectedValues")
                            Return
                        Else
                            l.Selected = True
                        End If
                    Next
                End If
            End Set
        End Property

        Protected Overrides Function SaveViewState() As Object
            Dim base As Object = MyBase.SaveViewState
            Dim myState(Items.Count - 1) As String
            For i As Integer = 0 To Items.Count - 1
                Dim state As String = Items(i).Text.Replace("|", "\|")
                state &= "|" & Items(i).Value.Replace("|", "\|")
                state &= "|" & Items(i).Selected.ToString
                myState(i) = state
            Next
            If base Is Nothing Then
                Return myState
            Else
                Return New Pair(base, myState)
            End If
        End Function

        Private Sub RegisterScript()
            If Not Page.ClientScript.IsClientScriptBlockRegistered("MultiBoxScript") Then
                Dim s As String = _
                      "function MultiBoxItemSelected(e) {" _
                    & " var target = e.target ? e.target : e.srcElement;" _
                    & " if(target.id.indexOf('_lbox') >= 0) {" _
                    & "     var opt = target.options[target.selectedIndex];" _
                    & "     var rbox = $get(target.id.replace('_lbox','_rbox'));" _
                    & "     target.remove(target.selectedIndex);" _
                    & "     rbox.add(opt);" _
                    & "     var hdn=$get(target.id.replace('_lbox',''));" _
                    & "     hdn.value = hdn.value + (hdn.value == '' ? '' : ',') + opt.value;" _
                    & " } else {" _
                    & "     var opt = target.options[target.selectedIndex];" _
                    & "     var lbox = $get(target.id.replace('_rbox','_lbox'));" _
                    & "     target.remove(target.selectedIndex);" _
                    & "     lbox.add(opt);" _
                    & "     var hdn=$get(target.id.replace('_rbox',''));" _
                    & "     hdn.value = hdn.value.replace(new RegExp('(,' + opt.value +'(?:,|$))|((?:^|,)'+ opt.value +',)|(^'+opt.value+'$)','g'),'');" _
                    & " }" _
                    & "}"

                Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "MultiBoxScript", s, True)
            End If
        End Sub

        Protected Overrides Sub LoadViewState(ByVal savedState As Object)
            Dim base As Object = Nothing
            Dim myState As String() = Nothing

            If TypeOf savedState Is Pair Then
                base = TryCast(savedState, Pair).First
                myState = TryCast(savedState, Pair).Second
            Else
                myState = TryCast(savedState, String())
            End If
            If myState IsNot Nothing Then
                Dim re As New Regex("[^\|$]+")
                Items.Clear()
                For Each item As String In myState
                    Dim matches As MatchCollection = re.Matches(item)
                    If matches.Count <> 3 Then
                        Throw New ArgumentException("Error loading ViewState")
                    Else
                        Dim l As New WebControls.ListItem(matches(0).Value, matches(1).Value)
                        l.Selected = Boolean.Parse(matches(2).Value)
                        m_Items.Add(l)
                    End If
                Next
            End If
            If base IsNot Nothing Then
                MyBase.LoadViewState(base)
            End If
        End Sub

        Private Sub MultiBox_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            RegisterScript()
        End Sub

        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            writer.AddAttribute("id", Me.ClientID)
            writer.AddAttribute("name", Me.UniqueID)
            writer.AddAttribute("type", "hidden")
            writer.AddAttribute("value", SelectedValues)
            writer.RenderBeginTag("input")

            writer.AddAttribute("border", "0")
            writer.AddAttribute("cellpadding", "0")
            writer.AddAttribute("cellspacing", "0")
            If Width <> Nothing Then writer.AddAttribute("width", Width.Value.ToString)
            writer.RenderBeginTag("table")
            writer.AddAttribute("valign", "top")
            writer.RenderBeginTag("tr")
            writer.RenderBeginTag("td")

            writer.AddAttribute("id", Me.ClientID & Me.ClientIDSeparator & "lbox")
            writer.AddAttribute("multiple", "true")
            writer.AddAttribute("onchange", "MultiBoxItemSelected(event)")
            If ListWidth <> Nothing Then writer.AddStyleAttribute("width", ListWidth & "px")
            writer.RenderBeginTag("select")
            Dim e As IEnumerable = From i As WebControls.ListItem In Items Where i.Selected = False Select i
            For Each item As WebControls.ListItem In e
                writer.AddAttribute("value", item.Value)
                writer.AddAttribute("checked", "false")
                writer.RenderBeginTag("option")
                writer.Write(item.Text)
                writer.RenderEndTag()
            Next
            writer.RenderEndTag() 'select
            writer.RenderEndTag() 'td

            writer.RenderBeginTag("td")

            writer.AddAttribute("id", Me.ClientID & ClientIDSeparator & "rbox")
            writer.AddAttribute("multiple", "true")
            writer.AddAttribute("onchange", "MultiBoxItemSelected(event)")
            If ListWidth <> Nothing Then writer.AddStyleAttribute("width", ListWidth & "px")
            writer.RenderBeginTag("select")

            e = From i As WebControls.ListItem In Items Where i.Selected = True Select i
            For Each item As WebControls.ListItem In e
                writer.AddAttribute("value", item.Value)
                writer.AddAttribute("checked", "false")
                writer.RenderBeginTag("option")
                writer.Write(item.Text)
                writer.RenderEndTag()
            Next
            writer.RenderEndTag() 'select
            writer.RenderEndTag() 'td
            writer.RenderEndTag() 'tr
            writer.RenderEndTag() 'table
        End Sub

        Public Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As System.Collections.Specialized.NameValueCollection) As Boolean Implements System.Web.UI.IPostBackDataHandler.LoadPostData
            Dim bRaise As Boolean = False
            If SelectedValues <> postCollection(postDataKey) Then
                bRaise = True
            End If
            SelectedValues = postCollection(postDataKey)
            If bRaise Then RaiseEvent VendorsChanged(Me, EventArgs.Empty)
        End Function

        Public Sub RaisePostDataChangedEvent() Implements System.Web.UI.IPostBackDataHandler.RaisePostDataChangedEvent

        End Sub

        Protected Overrides Sub PerformSelect()
            If DataSource IsNot Nothing Then
                Dim data As Object = DataSource
                Dim ls As ComponentModel.IListSource = TryCast(DataSource, ComponentModel.IListSource)
                If ls IsNot Nothing Then
                    data = ls.GetList
                End If
                Items.Clear()
                For Each item As Object In data
                    Dim l As New WebControls.ListItem()
                    l.Text = DataBinder.GetPropertyValue(item, DataTextField)
                    l.Value = DataBinder.GetPropertyValue(item, DataValueField)
                    Items.Add(l)
                Next
            End If
        End Sub

        Protected Overrides Sub ValidateDataSource(ByVal dataSource As Object)
            If Not (TypeOf dataSource Is IEnumerable Or TypeOf dataSource Is ComponentModel.IListSource Or TypeOf dataSource Is IDataSource) Then
                Throw New ArgumentException("Invalid DataSource")
            End If
        End Sub
    End Class
End Namespace