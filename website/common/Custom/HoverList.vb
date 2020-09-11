Imports Components
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Collections.Specialized
Imports System.Text.RegularExpressions

Namespace Controls

    Public Class HoverList
        Inherits DataBoundControl
        Implements IScriptControl
        Implements IPostBackDataHandler
        Implements IPostBackEventHandler

        Public Event SelectedIndexChanged As EventHandler

        Private RaiseChangedEvent As Boolean

        Private m_Label As String
        Public Property Label() As String
            Get
                Return m_Label
            End Get
            Set(ByVal value As String)
                m_Label = value
            End Set
        End Property

        Private m_LabelClass As String
        Public Property LabelClass() As String
            Get
                Return m_LabelClass
            End Get
            Set(ByVal value As String)
                m_LabelClass = value
            End Set
        End Property

        Private m_MaxHeight As Integer
        Public Property MaxHeight() As Integer
            Get
                Return m_MaxHeight
            End Get
            Set(ByVal value As Integer)
                m_MaxHeight = value
            End Set
        End Property

        Private m_AutoPostback As Boolean
        Public Property AutoPostback() As Boolean
            Get
                Return m_AutoPostback
            End Get
            Set(ByVal value As Boolean)
                m_AutoPostback = value
            End Set
        End Property

        Private m_EmptyText As String
        Public Property EmptyText() As String
            Get
                Return m_EmptyText
            End Get
            Set(ByVal value As String)
                m_EmptyText = value
            End Set
        End Property

        Public Property SelectedIndex() As Integer
            Get
                If ViewState("SelectedIndex") Is Nothing Then
                    Return -1
                Else
                    Return ViewState("SelectedIndex")
                End If
            End Get
            Set(ByVal value As Integer)
                ViewState("SelectedIndex") = value
            End Set
        End Property

        Public Property SelectedValue() As String
            Get
                If SelectedIndex >= 0 Then
                    Return Items(SelectedIndex)
                Else
                    Return Nothing
                End If
            End Get
            Set(ByVal value As String)
                For i As Integer = 0 To Items.Count - 1
                    If Items(i) = value Then
                        SelectedIndex = i
                        Exit Property
                    End If
                Next
                If value = String.Empty Then
                    ViewState("SelectedIndex") = Nothing
                Else
                    Throw New ArgumentException("SelectedValue could not be found")
                End If
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

        Private m_Items As NameValueCollection
        Public ReadOnly Property Items() As NameValueCollection
            Get
                If m_Items Is Nothing Then
                    m_Items = New NameValueCollection
                End If
                Return m_Items
            End Get
        End Property

        Protected Overrides Function SaveViewState() As Object
            Dim base As Object = MyBase.SaveViewState
            Dim MyState As New Text.StringBuilder
            Dim conn As String = String.Empty
            For Each key As String In Items.Keys
                MyState.Append(conn & CustomEscape(key) & "," & CustomEscape(Items(key)))
                conn = "|"
            Next
            If base IsNot Nothing Then
                Return New Pair(base, MyState.ToString)
            Else
                Return MyState.ToString
            End If
        End Function

        Protected Overrides Sub LoadViewState(ByVal savedState As Object)
            Dim MyState As String
            If TypeOf savedState Is Pair Then
                Dim state As Pair = savedState
                MyState = state.Second
                MyBase.LoadViewState(state.First)
            Else
                MyState = savedState
            End If

            If MyState <> Nothing Then
                're looks like a mess, but just parses string of format: <name>,<value>|<name>,<value>
                Dim re As New Regex("(?:^|(?:(?!\\)\|))((?:[^,]*|(?:\\,))*),((?:[^\|]*|(?:\\\|))*)")
                Dim matches As MatchCollection = re.Matches(MyState)

                Items.Clear()
                For Each m As Match In matches
                    Items.Add(CustomUnEscape(m.Groups(1).Value), CustomUnEscape(m.Groups(2).Value))
                Next
            End If
        End Sub

        Private Function CustomEscape(ByVal str As String) As String
            Return str.Replace("\", "\\").Replace("|", "\|").Replace(",", "\,")
        End Function

        Private Function CustomUnEscape(ByVal str As String) As String
            Return str.Replace("\,", ",").Replace("\|", "|").Replace("\\", "\")
        End Function

        Protected Overrides Sub PerformSelect()
            If Not IsBoundUsingDataSourceID Then
                OnDataBinding(EventArgs.Empty)
            End If

            GetData().Select(CreateDataSourceSelectArguments(), AddressOf OnDataSourceViewSelectCallback)

            RequiresDataBinding = False
            MarkAsDataBound()

            OnDataBound(EventArgs.Empty)
        End Sub

        Private Sub OnDataSourceViewSelectCallback(ByVal data As IEnumerable)
            Items.Clear()
            For Each item As Object In data
                Items.Add(DataBinder.GetPropertyValue(item, DataTextField), DataBinder.GetPropertyValue(item, DataValueField))
            Next
        End Sub

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)
            ScriptManager.GetCurrent(Page).RegisterScriptControl(Me)
        End Sub

        Public Overrides Sub RenderBeginTag(ByVal writer As System.Web.UI.HtmlTextWriter)
            'MyBase.RenderBeginTag(writer)
        End Sub

        Public Overrides Sub RenderEndTag(ByVal writer As System.Web.UI.HtmlTextWriter)
            'MyBase.RenderEndTag(writer)
        End Sub

        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            'relative div
            writer.AddStyleAttribute(HtmlTextWriterStyle.Position, "relative")
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "hoverwrpr")
            If Width <> Nothing Then
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, Width.ToString)
            End If
            If Height <> Nothing Then
                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, Height.ToString)
            End If
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID & ClientIDSeparator & "wrapper")
            writer.RenderBeginTag(HtmlTextWriterTag.Div)

            'collapsed/expanded image
            writer.AddStyleAttribute(HtmlTextWriterStyle.BorderStyle, "none")
            writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "1px")
            writer.AddStyleAttribute("float", "left")
            writer.AddAttribute(HtmlTextWriterAttribute.Src, "/images/collapsed.gif")
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID & ClientIDSeparator & "img")
            writer.RenderBeginTag(HtmlTextWriterTag.Img)
            writer.RenderEndTag()

            'label
            If LabelClass <> Nothing Then
                writer.AddAttribute(HtmlTextWriterAttribute.Class, LabelClass)
            End If
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID & ClientIDSeparator & "lbl")
            'writer.AddStyleAttribute("float", "left")
            writer.RenderBeginTag(HtmlTextWriterTag.Span)
            writer.Write(Label)
            writer.RenderEndTag() '/label

            'writer.AddStyleAttribute("clear", "both")
            'writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "0px")
            'writer.RenderBeginTag(HtmlTextWriterTag.Div)
            'writer.Write("&nbsp;")
            'writer.RenderEndTag()

            'hidden value field
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID)
            writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID)
            If SelectedValue <> Nothing Then
                writer.AddAttribute(HtmlTextWriterAttribute.Value, SelectedValue)
            End If
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden")
            writer.RenderBeginTag(HtmlTextWriterTag.Input)
            writer.RenderEndTag()

            'absolute div
            If CssClass <> String.Empty Then
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass)
            End If
            For Each key As String In Style.Keys
                writer.AddStyleAttribute(key, Style(key))
            Next
            writer.AddStyleAttribute(HtmlTextWriterStyle.Position, "absolute")
            writer.AddStyleAttribute(HtmlTextWriterStyle.Top, "100%")
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID & ClientIDSeparator & "div")
            writer.RenderBeginTag(HtmlTextWriterTag.Div)

            If Items.Count = 0 And EmptyText <> String.Empty Then
                writer.AddStyleAttribute(HtmlTextWriterStyle.FontStyle, "italic")
                writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, "10px")
                writer.RenderBeginTag(HtmlTextWriterTag.Span)
                writer.Write(EmptyText)
                writer.RenderEndTag()
                writer.WriteBreak()
            End If

            'list
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID & ClientIDSeparator & "list")
            writer.AddStyleAttribute(HtmlTextWriterStyle.ListStyleType, "none")
            writer.RenderBeginTag(HtmlTextWriterTag.Ul)
            writer.RenderEndTag() '/list

            writer.RenderEndTag() '/absolute div
            writer.RenderEndTag() '/relative div

            ScriptManager.GetCurrent(Page).RegisterScriptDescriptors(Me)
        End Sub

        Public Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As System.Collections.Specialized.NameValueCollection) As Boolean Implements System.Web.UI.IPostBackDataHandler.LoadPostData
            If postCollection(postDataKey) <> Nothing Then
                If SelectedValue <> postCollection(postDataKey) Then
                    SelectedValue = postCollection(postDataKey)
                    RaiseChangedEvent = True
                Else
                    RaiseChangedEvent = False
                End If
                Page.RegisterRequiresRaiseEvent(Me)
            End If
        End Function

        Public Sub RaisePostDataChangedEvent() Implements System.Web.UI.IPostBackDataHandler.RaisePostDataChangedEvent
            If RaiseChangedEvent Then
                RaiseEvent SelectedIndexChanged(Me, System.EventArgs.Empty)
            End If
        End Sub

        Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent
        End Sub

        Public Function GetScriptDescriptors() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptDescriptor) Implements System.Web.UI.IScriptControl.GetScriptDescriptors
            Dim s As New ScriptControlDescriptor("AE.HoverList", ClientID)
            Dim json As New Web.Script.Serialization.JavaScriptSerializer
            Dim sItems As New Text.StringBuilder
            Dim conn As String = String.Empty
            sItems.Append("[")
            For Each key As String In Items.Keys
                sItems.Append(conn & json.Serialize(New NameValuePair(key, Items(key))))
                conn = ","
            Next
            sItems.Append("]")
            s.AddScriptProperty("items", sItems.ToString)
            If AutoPostback Then
                s.AddProperty("postback", Page.ClientScript.GetPostBackEventReference(Me, String.Empty))
            End If
            s.AddProperty("maxHeight", MaxHeight)
            Return New ScriptDescriptor() {s}
        End Function

        Public Function GetScriptReferences() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptReference) Implements System.Web.UI.IScriptControl.GetScriptReferences
            Dim s As New ScriptReference("/includes/controls/HoverList.js")
            Return New ScriptReference() {s}
        End Function
    End Class
End Namespace