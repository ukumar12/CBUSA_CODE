Imports Components
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports System.Text.RegularExpressions

Namespace Controls
    Public Class MultiSelect
        Inherits BaseDataBoundControl

        Public Property SelectedValues() As String
            Get
                Return ViewState(Me.UniqueID & "_Selected")
            End Get
            Set(ByVal value As String)
                ViewState(Me.UniqueID & "_Selected") = value
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

        Private m_Height As Unit
        Public Overrides Property Height() As Unit
            Get
                Return m_Height
            End Get
            Set(ByVal value As Unit)
                m_Height = value
            End Set
        End Property

        Private m_OnClass As String
        Public Property OnClass() As String
            Get
                Return m_OnClass
            End Get
            Set(ByVal value As String)
                m_OnClass = value
            End Set
        End Property

        Private m_OffClass As String
        Public Property OffClass() As String
            Get
                Return m_OffClass
            End Get
            Set(ByVal value As String)
                m_OffClass = value
            End Set
        End Property

        Private m_Items As GenericCollection(Of ListItem)
        Public ReadOnly Property Items() As GenericCollection(Of ListItem)
            Get
                If m_Items Is Nothing Then
                    m_Items = New GenericCollection(Of ListItem)
                End If
                Return m_Items
            End Get
        End Property

        Protected Overrides Function SaveViewState() As Object
            Dim baseState As Object = MyBase.SaveViewState()
            Dim myState As New Text.StringBuilder()
            For Each item As ListItem In Items
                Dim str As String = CustomEscape(item.Text) & "," & CustomEscape(item.Value) & "," & CustomEscape(Convert.ToInt32(item.Selected))
                If myState.Length > 0 Then
                    myState.Append("|" & str)
                Else
                    myState.Append(str)
                End If
            Next
            If baseState Is Nothing Then
                Return myState.ToString
            Else
                Return New Pair(baseState, myState.ToString)
            End If
        End Function

        Protected Overrides Sub LoadViewState(ByVal savedState As Object)
            Dim myState As String
            If TypeOf savedState Is Pair Then
                Dim p As Pair = DirectCast(savedState, Pair)
                MyBase.LoadViewState(p.First)
                myState = p.Second
            Else
                myState = savedState
            End If

            If myState = String.Empty Then Exit Sub

            Items.Clear()
            Dim itemStrings As MatchCollection = Regex.Matches(myState, ".+(?=(?!\\)\||$)")
            For Each m As Match In itemStrings
                Dim props As MatchCollection = Regex.Matches(m.Value, ".+(?=(?!\\),|$)")
                If props.Count <> 3 Then
                    Logger.Error("Error loading viewstate: " & Me.ID & " (" & myState & ")")
                Else
                    Dim i As New ListItem(props(0).Value, props(1).Value)
                    i.Selected = Convert.ToBoolean(props(2).Value)
                    Items.Add(i)
                End If
            Next
        End Sub

        Private Function CustomEscape(ByVal str As String) As String
            Return str.Replace("\", "\\").Replace(",", "\,").Replace("|", "\|")
        End Function

        Private Sub RegisterScript()
            If Not Page.ClientScript.IsClientScriptBlockRegistered("MultiSelect") Then
                Dim s As String = _
                      "function MultiSelectClick(e,onClass,offClass) {" _
                    & " var target=e.target?e.target:e.srcElement;" _
                    & " var hdn=$get(target.parentNode.id + '_hdn');" _
                    & " var re = new RegExp('(?:^|,)'+ target.value +'($|,)');" _
                    & " if(re.test(hdn.value)) {" _
                    & "     target.className=offClass;" _
                    & "     hdn.value = hdn.value.replace(re,'');" _
                    & " } else {" _
                    & "     target.className=onClass;" _
                    & "     hdn.value = hdn.value =='' ? target.value : hdn.value + ',' + target.value;" _
                    & " }" _
                    & "}"

                Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "MultiSelect", s, True)
            End If
        End Sub

        Public Overrides Sub RenderBeginTag(ByVal writer As System.Web.UI.HtmlTextWriter)

        End Sub

        Public Overrides Sub RenderEndTag(ByVal writer As System.Web.UI.HtmlTextWriter)

        End Sub

        Protected Overrides Sub CreateChildControls()
            Dim ul As New HtmlGenericControl("ul")
            ul.ID = Me.ID & "_list"
            If Height <> Nothing Then ul.Attributes.CssStyle.Add("height", Height.ToString)
            ul.Attributes.CssStyle.Add("overflow", "auto")
            Controls.Add(ul)
            For Each item As ListItem In Items
                Dim li As New HtmlGenericControl("li")
                li.InnerHtml = item.Text
                li.Attributes.Add("value", item.Value)
                If item.Selected Then
                    li.Attributes.Add("class", OnClass)
                Else
                    li.Attributes.Add("class", OffClass)
                End If
                li.Attributes.Add("onclick", "MultiSelectClick(event,'" & OnClass & "','" & OffClass & "')")
                ul.Controls.Add(li)
            Next
        End Sub

        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            MyBase.Render(writer)
            writer.AddAttribute("type", "hidden")
            writer.AddAttribute("name", Me.UniqueID)
            writer.AddAttribute("id", Me.ClientID)
            writer.AddAttribute("value", SelectedValues)
            writer.RenderBeginTag("input")
        End Sub

        Protected Overrides Sub PerformSelect()
            Dim num As IEnumerable = Nothing
            If TypeOf DataSource Is IEnumerable Or TypeOf DataSource Is IDataSource Then
                num = DataSource
            ElseIf TypeOf DataSource Is ComponentModel.IListSource Then
                num = CType(DataSource, ComponentModel.IListSource).GetList
            End If
            If num IsNot Nothing Then
                For Each rec As Object In num
                    Dim item As New ListItem()
                    item.Text = DataBinder.GetPropertyValue(rec, DataTextField)
                    item.Value = DataBinder.GetPropertyValue(rec, DataValueField)
                    Items.Add(item)
                Next
            Else
                Throw New ArgumentException("Could not bind to DataSource")
            End If
        End Sub

        Protected Overrides Sub ValidateDataSource(ByVal dataSource As Object)
            If Not TypeOf dataSource Is IDataSource And Not TypeOf dataSource Is IEnumerable And Not TypeOf dataSource Is System.ComponentModel.IListSource Then
                Throw New ArgumentException("Invalid DataSource")
            End If
        End Sub
    End Class
End Namespace