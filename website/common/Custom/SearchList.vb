Imports Components
Imports Controls
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls

Namespace Controls
    <ValidationPropertyAttribute("Value")> _
    Public Class SearchList
        Inherits ScriptControl
        Implements ISubFormScriptControl
        Implements IPostBackEventHandler
        Implements IPostBackDataHandler

        Public Event ValueChanged As EventHandler

        Public Property Table() As String
            Get
                Return ViewState("Table")
            End Get
            Set(ByVal value As String)
                ViewState("Table") = value
            End Set
        End Property

        Public Property TextField() As String
            Get
                Return ViewState("Field")
            End Get
            Set(ByVal value As String)
                ViewState("Field") = value
            End Set
        End Property

        Public Property ValueField() As String
            Get
                Return ViewState("ValueField")
            End Get
            Set(ByVal value As String)
                ViewState("ValueField") = value
            End Set
        End Property

        Public Property WhereClause() As String
            Get
                Return ViewState("WhereClause")
            End Get
            Set(ByVal value As String)
                ViewState("WhereClause") = value
            End Set
        End Property

        Public Property MinLength() As Integer
            Get
                Return ViewState("MinLength")
            End Get
            Set(ByVal value As Integer)
                ViewState("MinLength") = value
            End Set
        End Property

        Public Property ViewAllLength() As Integer
            Get
                Return ViewState("ViewAllLength")
            End Get
            Set(ByVal value As Integer)
                ViewState("ViewAllLength") = value
            End Set
        End Property

        Public Property OnClientValueUpdated() As String
            Get
                Return ViewState("OnClientValueUpdated")
            End Get
            Set(ByVal value As String)
                ViewState("OnClientValueUpdated") = value
            End Set
        End Property

        Public Property OnClientTextChanged() As String
            Get
                Return ViewState("OnClientTextChanged")
            End Get
            Set(ByVal value As String)
                ViewState("OnClientTextChanged") = value
            End Set
        End Property

        Public Property AllowNew() As Boolean
            Get
                Return ViewState("AllowNew")
            End Get
            Set(ByVal value As Boolean)
                ViewState("AllowNew") = value
            End Set
        End Property

        Public Property SearchFunction() As String
            Get
                Return ViewState("SearchFunction")
            End Get
            Set(ByVal value As String)
                ViewState("SearchFunction") = value
            End Set
        End Property

        Public Property AutoPostback() As Boolean
            Get
                Return ViewState("AutoPostback")
            End Get
            Set(ByVal value As Boolean)
                ViewState("AutoPostback") = value
            End Set
        End Property

        Public Property Text() As String
            Get
                Return ViewState("Text")
            End Get
            Set(ByVal value As String)
                ViewState("Text") = value
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

        Protected Overrides Function GetScriptDescriptors() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptDescriptor)
            Dim s As New ModifiedControlDescriptor("AE.SearchList", ClientID)
            s.AddProperty("table", Table)
            s.AddProperty("textField", TextField)
            s.AddProperty("valueField", ValueField)
            s.AddProperty("minLength", MinLength)
            s.AddProperty("allowNew", AllowNew)
            s.AddProperty("viewAllLength", ViewAllLength)
            If WhereClause <> Nothing Then
                s.AddProperty("whereClause", WhereClause)
            End If
            If AutoPostback Then
                s.AddScriptProperty("autopostback", "function() {" & Page.ClientScript.GetPostBackEventReference(Me, String.Empty) & "}")
            End If
            If SearchFunction <> Nothing Then
                s.AddProperty("searchFunction", SearchFunction)
            End If
            'If CssClass <> Nothing Then
            '    s.AddProperty("className", CssClass)
            'End If
            If OnClientValueUpdated <> String.Empty Then
                s.AddProperty("onClientUpdate", OnClientValueUpdated)
            End If
            If OnClientTextChanged <> String.Empty Then
                s.AddProperty("onTextChanged", OnClientTextChanged)
            End If

            s.AddElementProperty("hdn", Me.ClientID & Me.ClientIDSeparator & "hdn")
            s.AddElementProperty("wrapper", Me.ClientID & Me.ClientIDSeparator & "wrapper")
            s.AddElementProperty("list", Me.ClientID & Me.ClientIDSeparator & "list")
            s.AddElementProperty("div", Me.ClientID & Me.ClientIDSeparator & "div")

            's.AddProperty("name", UniqueID)
            Return New ScriptDescriptor() {s}
        End Function

        Public Overrides Sub RenderBeginTag(ByVal writer As System.Web.UI.HtmlTextWriter)
            'MyBase.RenderBeginTag(writer)
        End Sub

        Public Overrides Sub RenderEndTag(ByVal writer As System.Web.UI.HtmlTextWriter)
            'MyBase.RenderEndTag(writer)
        End Sub

        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            If RenderScript And ScriptManager.GetCurrent(Page) IsNot Nothing Then
                ScriptManager.GetCurrent(Page).RegisterScriptDescriptors(Me)
            End If

            writer.AddAttribute("id", ClientID & ClientIDSeparator & "wrapper")
            writer.AddStyleAttribute("position", "relative")
            writer.RenderBeginTag("div")

            writer.AddAttribute("id", ClientID)
            writer.AddAttribute("name", UniqueID)
            writer.AddAttribute("type", "text")
            writer.AddAttribute("value", Text)
            writer.AddAttribute("autocomplete", "off")
            If Width <> Nothing Then
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, Width.ToString)
            ElseIf Style("width") IsNot Nothing Then
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, Style("width"))
            End If
            writer.AddStyleAttribute("position", "relative")
            writer.AddStyleAttribute("top", "0px")
            writer.RenderBeginTag("input")
            writer.RenderEndTag()

            writer.AddAttribute("id", ClientID & ClientIDSeparator & "hdn")
            writer.AddAttribute("name", UniqueID & IdSeparator & "hdn")
            writer.AddAttribute("type", "hidden")
            writer.AddAttribute("value", Value)
            writer.RenderBeginTag("input")
            writer.RenderEndTag()

            If CssClass <> String.Empty Then
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass)
            End If
            For Each key As String In Style.Keys
                writer.AddStyleAttribute(key, Style(key))
            Next
            writer.AddStyleAttribute(HtmlTextWriterStyle.Position, "absolute")
            writer.AddStyleAttribute(HtmlTextWriterStyle.Top, "100%")
            writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none")
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID & ClientIDSeparator & "div")
            writer.RenderBeginTag(HtmlTextWriterTag.Div)

            'list
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID & ClientIDSeparator & "list")
            writer.AddStyleAttribute(HtmlTextWriterStyle.ListStyleType, "none")
            writer.RenderBeginTag(HtmlTextWriterTag.Ul)
            writer.RenderEndTag() '/list

            writer.RenderEndTag() '/absolute div
            writer.RenderEndTag() '/relative div
        End Sub

        Protected Overrides Function GetScriptReferences() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptReference)
            Dim a As New Generic.List(Of ScriptReference)
            a.Add(New ScriptReference("/includes/controls/SearchList.js"))
            'a.Add(New ScriptReference("/includes/jquery-1.2.6.min.js"))
            Return a.ToArray
        End Function

        Public Function GetScript() As String Implements ISubFormScriptControl.GetScript
            Dim s As ScriptDescriptor() = GetScriptDescriptors()
            Dim out As String = String.Empty
            Dim conn As String = ""
            For Each sd As ModifiedControlDescriptor In s
                out &= conn & sd.GetScriptPublic()
                conn = ";"
            Next
            Return out
        End Function

        Private m_RenderScript As Boolean = True
        Public Property RenderScript() As Boolean Implements ISubFormScriptControl.RenderScript
            Get
                Return m_RenderScript
            End Get
            Set(ByVal value As Boolean)
                m_RenderScript = value
            End Set
        End Property

        Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent
            RaiseEvent ValueChanged(Me, EventArgs.Empty)
        End Sub

        Public Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As System.Collections.Specialized.NameValueCollection) As Boolean Implements System.Web.UI.IPostBackDataHandler.LoadPostData
            If postCollection.GetValues(postDataKey).Length > 0 Then
                If postCollection("__EVENTTARGET") = Me.UniqueID Then
                    'Page.RegisterRequiresRaiseEvent(Me)
                    Page.RegisterRequiresPostBack(Me)
                End If
                If postCollection(postDataKey) <> Text Or postCollection(postDataKey & IdSeparator & "hdn") <> Value Then
                    Text = postCollection(postDataKey)
                    Value = postCollection(postDataKey & IdSeparator & "hdn")
                    RaisePostDataChangedEvent()
                End If
            End If
        End Function

        Public Sub RaisePostDataChangedEvent() Implements System.Web.UI.IPostBackDataHandler.RaisePostDataChangedEvent

        End Sub
    End Class
End Namespace