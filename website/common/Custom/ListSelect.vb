Imports Components
Imports DataLayer
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Text.RegularExpressions

Namespace Controls
    Public Class ListSelect
        Inherits System.Web.UI.WebControls.BaseDataBoundControl
        Implements IScriptControl
        Implements IPostBackEventHandler
        Implements IPostBackDataHandler

        Public Event SelectedChanged As EventHandler

        Private m_AutoPostback As Boolean = False
        Public Property AutoPostback() As Boolean
            Get
                Return m_AutoPostback
            End Get
            Set(ByVal value As Boolean)
                m_AutoPostback = value
            End Set
        End Property

        Public Property SelectedValues() As String
            Get
                Dim out As New Text.StringBuilder()
                Dim conn As String = String.Empty
                For Each item As ListItem In Items
                    If item.Selected Then
                        out.Append(conn & item.Value)
                        conn = ","
                    End If
                Next
                Return out.ToString
            End Get
            Set(ByVal value As String)
                For Each item As ListItem In Items
                    item.Selected = False
                Next
                If value <> String.Empty Then
                    Dim values As String() = value.Split(",")

                    For Each v As String In values
                        Dim tmp As String = v
                        Dim item As ListItem = (From i As ListItem In Items Select i Where i.Value = tmp).FirstOrDefault
                        If item IsNot Nothing Then
                            item.Selected = True
                        End If
                    Next
                End If
            End Set
        End Property

        Private m_SelectLimit As Integer
        Public Property SelectLimit() As Integer
            Get
                Return m_SelectLimit
            End Get
            Set(ByVal value As Integer)
                m_SelectLimit = value
            End Set
        End Property

        Private m_DeleteImageUrl As String
        Public Property DeleteImageUrl() As String
            Get
                Return m_DeleteImageUrl
            End Get
            Set(ByVal value As String)
                m_DeleteImageUrl = value
            End Set
        End Property

        Private m_AddImageUrl As String
        Public Property AddImageUrl() As String
            Get
                Return m_AddImageUrl
            End Get
            Set(ByVal value As String)
                m_AddImageUrl = value
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

        Private m_Items As Generic.List(Of ListItem)
        Public ReadOnly Property Items() As Generic.List(Of ListItem)
            Get
                If m_Items Is Nothing Then
                    m_Items = New Generic.List(Of ListItem)
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
            Dim itemStrings As MatchCollection = Regex.Matches(myState, "(?<=(?!\\)\||^).+?(?=(?!\\)\||$)")
            For Each m As Match In itemStrings
                'Dim props As MatchCollection = Regex.Matches(m.Value, "(?<=(?<=\\),|^).+?(?=(?<=\\),|$)")
                'Dim props As String() = Regex.Split(m.Value, "((?:[^\\,\|])+|(?:\\,)|(?:\\\\)|(?:\\\|))+")
                Dim props As MatchCollection = Regex.Matches(m.Value, "((?:[^\\,\|])+|(?:\\,)|(?:\\\\)|(?:\\\|))+")
                If props.Count <> 3 Then
                    Logger.Error("Error loading viewstate: " & Me.ID & " (" & myState & ")")
                Else
                    Try
                        Dim i As New ListItem(CustomUnEscape(props(0).Value), CustomUnEscape(props(1).Value))
                        i.Selected = Convert.ToInt32(props(2).Value)
                        Items.Add(i)
                    Catch ex As Exception
                    End Try
                End If
            Next
        End Sub

        Private Function CustomEscape(ByVal str As String) As String
            Return str.Replace("\", "\\").Replace(",", "\,").Replace("|", "\|")
        End Function

        Private Function CustomUnEscape(ByVal str As String) As String
            Return str.Replace("\,", ",").Replace("\|", "|").Replace("\\", "\")
        End Function

        Protected Overrides Sub PerformSelect()
            Dim e As IEnumerable
            If TypeOf DataSource Is ComponentModel.IListSource Then
                e = CType(DataSource, ComponentModel.IListSource).GetList
            Else
                e = DataSource
            End If
            Items.Clear()
            For Each item As Object In e
                Dim l As New ListItem(DataBinder.GetPropertyValue(item, DataTextField), DataBinder.GetPropertyValue(item, DataValueField))
                If SelectedValues <> Nothing AndAlso Text.RegularExpressions.Regex.IsMatch(SelectedValues, "(^|,)" & l.Value & "(,|$)") Then
                    l.Selected = True
                End If
                Items.Add(l)
            Next
        End Sub

        Protected Overrides Sub ValidateDataSource(ByVal dataSource As Object)
            If Not (TypeOf dataSource Is IEnumerable Or TypeOf dataSource Is IDataSource Or TypeOf dataSource Is ComponentModel.IListSource) Then
                Throw New ArgumentException("Datasource is an invalid or unrecognized type.")
            End If
        End Sub

        Public Function GetScriptDescriptors() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptDescriptor) Implements System.Web.UI.IScriptControl.GetScriptDescriptors
            Dim s As New ScriptControlDescriptor("AE.ListSelect", Me.ClientID)
            If SelectLimit <> Nothing Then
                s.AddProperty("selectLimit", SelectLimit)
            End If
            If AutoPostback Then
                s.AddProperty("pbReference", Page.ClientScript.GetPostBackEventReference(Me, String.Empty))
            End If
            If AddImageUrl <> String.Empty Then
                s.AddProperty("addImageUrl", AddImageUrl)
            End If
            If DeleteImageUrl <> String.Empty Then
                s.AddProperty("deleteImageUrl", DeleteImageUrl)
            End If
            Return New ScriptDescriptor() {s}
        End Function

        Public Function GetScriptReferences() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptReference) Implements System.Web.UI.IScriptControl.GetScriptReferences
            Return New ScriptReference() {New ScriptReference("/includes/controls/ListSelect.js")}
        End Function

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)

            ScriptManager.GetCurrent(Page).RegisterScriptControl(Me)
        End Sub

        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            'MyBase.Render(writer)

            ScriptManager.GetCurrent(Page).RegisterScriptDescriptors(Me)

            writer.AddAttribute("type", "hidden")
            writer.AddAttribute("name", Me.UniqueID)
            writer.AddAttribute("id", Me.ClientID)
            writer.AddAttribute("value", SelectedValues)
            writer.RenderBeginTag("input")

            writer.AddAttribute("class", Me.CssClass)
            If Me.Width <> Nothing Then
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, Me.Width.ToString)
            End If

            'writer.RenderBeginTag("div")
            writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0")
            writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "1")
            writer.AddAttribute(HtmlTextWriterAttribute.Border, "0")
            For Each key As String In Attributes.Keys
                writer.AddAttribute(key, Attributes(key))
            Next
            For Each key As String In Style.Keys
                writer.AddStyleAttribute(key, Style(key))
            Next
            If Style("width") = Nothing And Width = Nothing Then
                writer.AddStyleAttribute("width", "100%")
            End If
            writer.AddStyleAttribute("table-layout", "fixed")
            writer.RenderBeginTag("table")
            writer.RenderBeginTag("tr")

            writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "50%")
            writer.RenderBeginTag("td")

            writer.Write("<b class=""smaller"">Available</b>")
            If Me.Width <> Nothing Then
                writer.AddStyleAttribute("width", Math.Floor(Me.Width.Value / 2) & Me.Width.Type.ToString)
            End If
            writer.AddAttribute("id", Me.ClientID & "_offList")
            writer.AddStyleAttribute("background-color", "#fff")
            writer.AddStyleAttribute("border", "1px solid #666")
            writer.AddStyleAttribute("list-style-type", "none")
            writer.AddStyleAttribute("margin", "3px")
            If Me.Height <> Nothing Then
                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, Me.Height.ToString)
                writer.AddStyleAttribute(HtmlTextWriterStyle.OverflowY, "auto")
            End If
            writer.RenderBeginTag("ul")
            Dim e As IEnumerator = (From item As ListItem In Items Select item Where Not item.Selected).GetEnumerator
            If e IsNot Nothing Then
                While e.MoveNext
                    Dim tmp As ListItem = DirectCast(e.Current, ListItem)
                    writer.AddAttribute("value", tmp.Value)
                    writer.AddAttribute("class", "lsitem")
                    writer.RenderBeginTag("li")
                    'writer.AddStyleAttribute("cursor", "pointer")
                    writer.RenderBeginTag("a")
                    If AddImageUrl <> Nothing Then
                        writer.AddAttribute("src", AddImageUrl)
                        writer.AddAttribute("alt", tmp.Text)
                        writer.AddStyleAttribute("float", "right")
                        writer.AddStyleAttribute("margin-left", "5px")
                        writer.RenderBeginTag("img")
                        writer.RenderEndTag()
                    End If
                    writer.Write(tmp.Text)

                    writer.RenderEndTag() 'a
                    writer.RenderEndTag() 'li
                End While
            End If
            writer.RenderEndTag() 'ul

            writer.RenderEndTag() 'td

            If Me.Height <> Nothing Then
                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, Me.Height.ToString)
            End If
            writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "50%")
            writer.AddStyleAttribute(HtmlTextWriterStyle.OverflowY, "auto")
            writer.RenderBeginTag("td")

            writer.Write("<b class=""smaller"">Selected</b>")
            If Me.Width <> Nothing Then
                writer.AddStyleAttribute("width", Math.Floor(Me.Width.Value / 2) & Me.Width.Type.ToString)
            End If
            writer.AddAttribute("id", Me.ClientID & "_onList")
            writer.AddStyleAttribute("background-color", "#fff")
            writer.AddStyleAttribute("border", "1px solid #666")
            writer.AddStyleAttribute("margin", "3px")
            writer.AddStyleAttribute("list-style-type", "none")
            If Me.Height <> Nothing Then
                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, Me.Height.ToString)
                writer.AddStyleAttribute(HtmlTextWriterStyle.OverflowY, "auto")
            End If
            writer.RenderBeginTag("ul")
            e = (From item As ListItem In Items Select item Where item.Selected).GetEnumerator
            If e IsNot Nothing Then
                While e.MoveNext
                    Dim tmp As ListItem = DirectCast(e.Current, ListItem)
                    writer.AddAttribute("value", tmp.Value)
                    writer.AddAttribute("class", "lsitem")
                    writer.RenderBeginTag("li")
                    'writer.AddStyleAttribute("cursor", "pointer")
                    writer.RenderBeginTag("a")

                    If DeleteImageUrl <> Nothing Then
                        writer.AddAttribute("src", DeleteImageUrl)
                        writer.AddAttribute("alt", tmp.Text)
                        writer.AddStyleAttribute("float", "right")
                        writer.AddStyleAttribute("margin-left", "5px")
                        writer.RenderBeginTag("img")
                        writer.RenderEndTag()
                    End If

                    writer.Write(tmp.Text)

                    writer.RenderEndTag() 'a
                    writer.RenderEndTag() 'li
                End While
            End If
            writer.RenderEndTag() 'ul

            writer.RenderEndTag() 'td
            writer.RenderEndTag() 'tr
            writer.RenderEndTag() 'table

            'writer.AddStyleAttribute("clear", "both")
            'writer.RenderBeginTag("div")
            'writer.Write("&nbsp;")
            'writer.RenderEndTag() 'div
        End Sub

        Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent
            RaiseEvent SelectedChanged(Me, System.EventArgs.Empty)
        End Sub

        Public Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As System.Collections.Specialized.NameValueCollection) As Boolean Implements System.Web.UI.IPostBackDataHandler.LoadPostData
            If postCollection(postDataKey) <> Nothing Then
                'If SelectedValues <> Nothing Then
                '    If SelectedValues.Split(",").Intersect(postCollection(postDataKey).Split(",")).Count <> SelectedValues.Count Then
                '        Page.RegisterRequiresRaiseEvent(Me)
                '    End If
                'End If
                SelectedValues = postCollection(postDataKey)
            Else
                SelectedValues = String.Empty
            End If
            Return True
        End Function

        Public Sub RaisePostDataChangedEvent() Implements System.Web.UI.IPostBackDataHandler.RaisePostDataChangedEvent
        End Sub
    End Class
End Namespace
