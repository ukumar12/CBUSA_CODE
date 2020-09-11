Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports System.Text

Namespace Controls

    ''' <summary>
    ''' Provides a tab selection control which supports step numbers and checkmarks for completion.
    ''' </summary>
    ''' <remarks>Each <see cref="Tab" /> is placed inside the control and its template is rendered when
    ''' the page is loaded.  The tabs are swapped in and out via JavaScript.
    ''' <seealso cref="Tab" />
    ''' <seealso cref="AdminTabs" /></remarks>
    <ParseChildren(GetType(Tab))> _
    Public Class TabControl
        Inherits CompositeControl
        Implements IScriptControl
        Implements IPostBackEventHandler
        Implements IPostBackDataHandler

        ''' <summary>
        ''' Occurs when a <see cref="Tab" /> which has <see cref="Tab.AutoPostback" /> enabled triggers a 
        ''' postback.
        ''' </summary>
        ''' <remarks>This event allows server-side code to run when a tab has been selected.</remarks>
        Public Event TabSelect As EventHandler

        ''' <summary>
        ''' Gets or sets whether or not to show step numbers in the tab ribbon.
        ''' </summary>
        ''' <value><see langword="True" /> if step numbers are enabled; <see langword="False" /> otherwise.</value>
        ''' <remarks>Step numbers, unlike checks, are generated automatically based on the ordering of the tabs
        ''' in the ribbon.  They can not be manipulated directly.</remarks>
        Public Property ShowSteps() As Boolean
            Get
                Return ViewState("ShowSteps")
            End Get
            Set(ByVal value As Boolean)
                ViewState("ShowSteps") = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets whether or not to show checks for completed tabs in the tab ribbon.
        ''' </summary>
        ''' <value><see langword="True" /> if checks are enabled; <see langword="False" /> otherwise.</value>
        ''' <remarks>Checks, unlike step numbers, are manipulated manually.  In order for a tab to be checked,
        ''' you must set the <see cref="Tab.Checked" /> property to <see langword="True" />, or call the
        ''' JavaScript function CheckTab.  However, if <see cref="ShowChecks" /> is <see langword="False" />, even
        ''' checked tabs will not render checkmarks (unless you call the JavaScript function, which bypasses
        ''' this condition).</remarks>
        Public Property ShowChecks() As Boolean
            Get
                Return ViewState("ShowChecks")
            End Get
            Set(ByVal value As Boolean)
                ViewState("ShowChecks") = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the index of the currently selected <see cref="Tab" />
        ''' </summary>
        ''' <value>An <see cref="Integer" /> representing the index of the <see cref="Tab" /> which is currently 
        ''' active.</value>
        ''' <remarks>This is a 0-based index.</remarks>
        Public Property SelectedIndex() As Integer
            Get
                Return ViewState("SelectedIndex")
            End Get
            Set(ByVal value As Integer)
                ViewState("SelectedIndex") = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the CSS class name for the <see cref="TabControl" />.
        ''' </summary>
        ''' <value>A <see cref="String" /> referencing a CSS class name for the outermost &lt;div&gt; element of
        ''' the <see cref="TabControl" /></value>
        ''' <remarks>The default class is "tabModule".</remarks>
        Public Overrides Property CssClass() As String
            Get
                If MyBase.CssClass = Nothing Then
                    Return "tabModule"
                Else
                    Return MyBase.CssClass
                End If
            End Get
            Set(ByVal value As String)
                MyBase.CssClass = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the CSS class name(s) for the &lt;ul&gt; element which contains the list of 
        ''' <see cref="Tab">Tabs</see> in the tab ribbon.
        ''' </summary>
        ''' <value>A <see cref="String" /> referencing one or more CSS class names for the &lt;ul&gt; element.</value>
        ''' <remarks>The default set of classes is 
        ''' "tabnav ui-tabs-nav ui-helper-reset ui-helper-clearfix ui-widget-header ui-corner-all".</remarks>
        Public Property TabListClass() As String
            Get
                If ViewState("TabListClass") Is Nothing Then
                    Return "tabnav ui-tabs-nav ui-helper-reset ui-helper-clearfix ui-widget-header ui-corner-all"
                Else
                    Return ViewState("TabListClass")
                End If
            End Get
            Set(ByVal value As String)
                ViewState("TabListClass") = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the CSS class name(s) for the &lt;li&gt; element which contains the currently active 
        ''' <see cref="Tab" /> in the tab ribbon.
        ''' </summary>
        ''' <value>A <see cref="String" /> referencing one or more CSS class names for the active 
        ''' <see cref="Tab" /> in the tab ribbon.</value>
        ''' <remarks>The default set of classes is 
        ''' "ui-state-default ui-corner-top ui-tabs-selected ui-state-active".</remarks>
        Public Property TabOnClass() As String
            Get
                If ViewState("TabOnClass") Is Nothing Then
                    Return "ui-state-default ui-corner-top ui-tabs-selected ui-state-active"
                Else
                    Return ViewState("TabOnClass")
                End If
            End Get
            Set(ByVal value As String)
                ViewState("TabOnClass") = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the CSS class name(s) for the &lt;li&gt; elements which contain the currently inactive 
        ''' <see cref="Tab">Tabs</see> in the tab ribbon.
        ''' </summary>
        ''' <value>A <see cref="String" /> referencing one or more CSS class names for the inactive 
        ''' <see cref="Tab">Tabs</see> in the tab ribbon.</value>
        ''' <remarks>The default set of classes is "ui-state-default ui-corner-top".</remarks>
        Public Property TabOffClass() As String
            Get
                If ViewState("TabOffClass") Is Nothing Then
                    Return "ui-state-default ui-corner-top"
                Else
                    Return ViewState("TabOffClass")
                End If
            End Get
            Set(ByVal value As String)
                ViewState("TabOffClass") = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the CSS class name(s) for the &lt;li&gt; elements which contain the currently '
        ''' <see cref="Tab.Checked" /> <see cref="Tab">Tabs</see> in the tab ribbon.
        ''' </summary>
        ''' <value>A <see cref="String" /> referencing one or more CSS class names for the 
        ''' <see cref="Tab.Checked" /> <see cref="Tab">Tabs</see> in the tab ribbon.</value>
        ''' <remarks>The default class is "ui-tabs-complete".</remarks>
        Public Property TabCompleteClass() As String
            Get
                If ViewState("TabCompleteClass") Is Nothing Then
                    Return "ui-tabs-complete"
                Else
                    Return ViewState("TabCompleteClass")
                End If
            End Get
            Set(ByVal value As String)
                ViewState("TabCompleteClass") = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the <see cref="TabCollection" /> which contains all of the <see cref="Tab">Tabs</see> for this
        ''' control.
        ''' </summary>
        ''' <value>A <see cref="TabCollection" /> which corresponds to the <see cref="TabControl.Controls" />
        ''' collection.</value>
        Public ReadOnly Property Tabs() As TabCollection
            Get
                Return CType(Controls, TabCollection)
            End Get
        End Property

        ''' <summary>
        ''' Notifies the control that an element, either XML or HTML, was parsed, and adds the element to 
        ''' the control's <see cref="ControlCollection" /> object.
        ''' </summary>
        ''' <param name="obj">An <see cref="Object" /> that represents the parsed element.</param>
        ''' <exception cref="ArgumentException">The parsed object is not a <see cref="Tab" /> or
        ''' <see cref="LiteralControl" />.</exception>
        ''' <remarks>Even though literal content inside the <see cref="TabControl" /> object will not throw an
        ''' <see cref="ArgumentException" />, neither will it get added to the <see cref="ControlCollection" />.</remarks>
        Protected Overrides Sub AddParsedSubObject(ByVal obj As Object)
            If TypeOf obj Is Tab Then
                Controls.Add(obj)
            ElseIf Not TypeOf obj Is LiteralControl Then
                Throw New ArgumentException("TabControl can only have children of type Tab")
            End If
        End Sub

        ''' <summary>
        ''' Called after a <see cref="Tab" /> object has been added to the <see cref="Controls" /> collection of 
        ''' the <see cref="TabControl" /> object.
        ''' </summary>
        ''' <param name="control">The <see cref="Tab" /> that has been added.</param>
        ''' <param name="index">The index of the control in the <see cref="Controls" /> collection.</param>
        ''' <remarks>In addition to the functionality offered by the base class, the 
        ''' <see cref="Tab.SetTabControl" /> method is called on the <paramref name="control" /> with the 
        ''' <see cref="TabControl" /> as argument.</remarks>
        Protected Overrides Sub AddedControl(ByVal control As System.Web.UI.Control, ByVal index As Integer)
            CType(control, Tab).SetTabControl(Me)
            MyBase.AddedControl(control, index)
        End Sub

        ''' <summary>
        ''' Creates a new <see cref="TabCollection" /> object to hold the child controls 
        ''' (<see cref="Tab">Tabs</see>) of the <see cref="TabControl" />.
        ''' </summary>
        ''' <returns>A <see cref="TabCollection" /> object to contain the <see cref="TabControl" />'s
        ''' <see cref="Tab">Tabs</see>.</returns>
        Protected Overrides Function CreateControlCollection() As System.Web.UI.ControlCollection
            Return New TabCollection(Me)
        End Function

        ''' <summary>
        ''' Generates the <see cref="ScriptControlDescriptor">ScriptControlDescriptors</see> used to store the
        ''' properties on the client side that are necessary in order to make the JavaScript work.
        ''' </summary>
        ''' <returns>A single <see cref="ScriptControlDescriptor" /> which represents the 
        ''' <see cref="TabControl" /> class.</returns>
        ''' <remarks>This method allows the TabControl.js file the information it needs in order to 
        ''' operate the essentials of the tab control.</remarks>
        Public Function GetScriptDescriptors() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptDescriptor) Implements System.Web.UI.IScriptControl.GetScriptDescriptors
            Dim s As New ScriptControlDescriptor("AmericanEagle.TabControl", ClientID)

            Dim sTabs As New StringBuilder("[")
            Dim conn As String = ""
            For Each t As Tab In Controls
                If t.Visible Then
                    sTabs.Append(conn & "{'id':'" & t.ClientID & "','IsEnabled':" & t.Enabled.ToString.ToLower & ",'IsPostback':" & t.AutoPostback.ToString.ToLower & ",'IsChecked':" & t.Checked.ToString.ToLower & ",'OnClientClick':'" & t.OnClientClick & "'}")
                    conn = ","
                End If
            Next
            sTabs.Append("]")
            s.AddScriptProperty("tabs", sTabs.ToString)

            s.AddProperty("tabOnClass", TabOnClass)
            s.AddProperty("tabOffClass", TabOffClass)
            s.AddProperty("tabCompleteClass", TabCompleteClass)

            s.AddProperty("postback", Page.ClientScript.GetPostBackEventReference(Me, String.Empty))
            Return New ScriptDescriptor() {s}
        End Function

        ''' <summary>
        ''' Raises the <see cref="PreRender" /> event, then registers the control with the 
        ''' <see cref="ScriptManager" />.
        ''' </summary>
        ''' <param name="e">An <see cref="EventArgs" /> object that contains the event data.</param>
        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)

            ScriptManager.GetCurrent(Page).RegisterScriptControl(Me)
        End Sub

        ''' <summary>
        ''' Renders the <see cref="TabControl" /> to the <paramref name="writer"/> 
        ''' object.
        ''' </summary>
        ''' <param name="writer">The <see cref="HtmlTextWriter" /> that receives the rendered output.</param>
        ''' <remarks>The <see cref="TabControl" /> renders itself as an outer &lt;div&gt; element, containing two
        ''' things.  The first is an inner &lt;div&gt; element which in turn contains an unordered list 
        ''' representing the tab ribbon.  The second is the contents of each <see cref="Tab" /> in turn.</remarks>
        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            writer.AddAttribute("type", "hidden")
            writer.AddAttribute("id", ClientID)
            writer.AddAttribute("name", UniqueID)
            writer.AddAttribute("value", SelectedIndex.ToString)
            writer.RenderBeginTag("input")
            writer.RenderEndTag()

            writer.AddAttribute("id", ClientID)
            writer.AddAttribute("class", CssClass)
            If Width <> Nothing Then
                writer.AddStyleAttribute("width", Width.ToString)
            End If
            writer.RenderBeginTag("div")

            writer.AddStyleAttribute("height", "24px")
            writer.RenderBeginTag("div")

            writer.AddAttribute("class", TabListClass)
            writer.RenderBeginTag("ul")

            Dim stepNumber As Integer = 1
            For i As Integer = 0 To Tabs.Count - 1
                If Tabs(i).Visible Then
                    Tabs(i).RenderTab(writer, i = SelectedIndex, IIf(ShowSteps, stepNumber, 0))
                    stepNumber += 1
                End If
            Next

            writer.RenderEndTag() '</ul>

            writer.AddAttribute("class", "spacer")
            writer.RenderBeginTag("div")
            writer.Write(" ")
            writer.RenderEndTag() '</div>

            writer.RenderEndTag() '</div>

            For i As Integer = 0 To Tabs.Count - 1
                Dim tab As Tab = Tabs(i)
                tab.RenderContent(writer, i = SelectedIndex)
            Next

            writer.RenderEndTag() '</div>

            ScriptManager.GetCurrent(Page).RegisterScriptDescriptors(Me)
        End Sub

        ''' <summary>
        ''' Provides the list of external JavaScript references that this control needs to operate.
        ''' </summary>
        ''' <returns>A single <see cref="ScriptReference" /> which represents the TabControl.js file</returns>
        ''' <remarks>Make sure that the path inside this method corresponds to the path in the Web project
        ''' for this particular file.</remarks>
        Public Function GetScriptReferences() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptReference) Implements System.Web.UI.IScriptControl.GetScriptReferences
            Dim s As New ScriptReference("/cms/includes/controls/tabcontrol.js")
            Return New ScriptReference() {s}
        End Function

        ''' <summary>
        ''' Called as part of the page lifecycle in order to trigger the <see cref="TabSelect" /> event.
        ''' </summary>
        ''' <param name="eventArgument">Not used by this implementation.</param>
        Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent
            RaiseEvent TabSelect(Me, EventArgs.Empty)
        End Sub

        ''' <summary>
        ''' Manages the state of the <see cref="SelectedIndex" /> attribute.
        ''' </summary>
        ''' <param name="postDataKey">The name of the element in the <paramref name="postCollection" />
        ''' which is being loaded.</param>
        ''' <param name="postCollection">The collection of name-value pairs representing the post data
        ''' of an <see cref="IPostBackDataHandler" /></param>
        ''' <returns><see langword="True" /> if the <see cref="SelectedIndex" /> property has changed; 
        ''' <see langword="False" /> otherwise.</returns>
        ''' <remarks>This method assumes that the only numeric value in the <paramref name="postCollection" />
        ''' is the <see cref="SelectedIndex" /> for the control.</remarks>
        Public Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As System.Collections.Specialized.NameValueCollection) As Boolean Implements System.Web.UI.IPostBackDataHandler.LoadPostData
            If postCollection(postDataKey) IsNot Nothing AndAlso IsNumeric(postCollection(postDataKey)) Then
                Dim changed As Boolean = SelectedIndex <> postCollection(postDataKey)
                SelectedIndex = postCollection(postDataKey)
                Return changed
            End If
            Return False
        End Function

        ''' <summary>
        ''' This method exists in order to implement the <see cref="IPostBackDataHandler" /> interface.  However,
        ''' it does nothing.
        ''' </summary>
        Public Sub RaisePostDataChangedEvent() Implements System.Web.UI.IPostBackDataHandler.RaisePostDataChangedEvent
        End Sub
    End Class

    ''' <summary>
    ''' Represents a single tab within the <see cref="TabControl" /> object.
    ''' </summary>
    ''' <remarks>This object is templated and is only meaningful within the <see cref="TabControl" /> class's 
    ''' <see cref="TabControl.Tabs" /> (or <see cref="TabControl.Controls" />) property.
    ''' <seealso cref="TabControl" /></remarks>
    <PersistChildren(False)> _
    <ParseChildren(True)> _
    Public Class Tab
        Inherits Control

        Private m_TabControl As TabControl

        ''' <summary>
        ''' Gets or sets the label on the tab ribbon.
        ''' </summary>
        ''' <value>A <see cref="String" /> to be displayed on the tab ribbon.</value>
        Public Property Label() As String
            Get
                Return ViewState("Label")
            End Get
            Set(ByVal value As String)
                ViewState("Label") = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets whether the <see cref="Tab" /> automatically triggers a postback when it is selected.
        ''' </summary>
        ''' <value><see langword="True" /> if selecting this <see cref="Tab" /> triggers a postback; 
        ''' <see langword="False" /> otherwise.</value>
        ''' <remarks>This property is used by <see cref="TabControl.GetScriptDescriptors" /> in order to transfer the 
        ''' value of this property into the JavaScript code which effects the postback functionality.</remarks>
        Public Property AutoPostback() As Boolean
            Get
                Return ViewState("AutoPostback")
            End Get
            Set(ByVal value As Boolean)
                ViewState("AutoPostback") = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets whether the <see cref="Tab" /> is checked (i.e. complete).
        ''' </summary>
        ''' <value><see langword="True" /> if this tab is checked; <see langword="False" /> otherwise.</value>
        ''' <remarks>Unless the <see cref="TabControl.ShowChecks" /> property is enabled on the parent
        ''' <see cref="TabControl" />, a check will not be rendered even if this property is set to 
        ''' <see langword="True" />.</remarks>
        Public Property Checked() As Boolean
            Get
                Return ViewState("Checked")
            End Get
            Set(ByVal value As Boolean)
                ViewState("Checked") = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a URL to navigate to when the <see cref="Tab" /> is activated.
        ''' </summary>
        ''' <value>A <see cref="String" /> containing the URL to navigate to.</value>
        ''' <remarks>If this property is set, do not include any content inside the <see cref="Tab" />, because
        ''' it won't be used.</remarks>
        Public Property NavigateUrl() As String
            Get
                Return ViewState("NavigateUrl")
            End Get
            Set(ByVal value As String)
                ViewState("NavigateUrl") = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets whether or not the <see cref="Tab" /> has a border around its contents.
        ''' </summary>
        ''' <value><see langword="True" /> if there should be a border; <see langword="False" /> otherwise.</value>
        ''' <remarks>The default value of this property is <see langword="True" />.</remarks>
        Public Property ShowBorder() As String
            Get
                Return IIf(ViewState("ShowBorder") = Nothing, True, ViewState("ShowBorder"))
            End Get
            Set(ByVal value As String)
                ViewState("ShowBorder") = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the CSS class name(s) for the &lt;div&gt; element which contains the contents of the
        ''' <see cref="Tab" />.
        ''' </summary>
        ''' <value>A <see cref="String" /> referencing one or more CSS class names for the <see cref="Tab" />'s
        ''' contents.</value>
        ''' <remarks>The default value of this property depends on <see cref="ShowBorder" />.  If 
        ''' <see langword="True" />, the default set of classes is
        ''' "tabdiv ui-tabs-panel ui-widget-content ui-corner-bottom".  If <see langword="False" />, the default
        ''' class is "tabdivwithoutborder".</remarks>
        Public Property CssClass() As String
            Get
                If ViewState("CssClass") Is Nothing Then
                    If ShowBorder = True Then
                        Return "tabdiv ui-tabs-panel ui-widget-content ui-corner-bottom"
                    Else
                        Return "tabdivwithoutborder"
                    End If
                Else
                    Return ViewState("CssClass")
                End If
            End Get
            Set(ByVal value As String)
                ViewState("CssClass") = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets whether the <see cref="Tab" /> is enabled.
        ''' </summary>
        ''' <value><see langword="True" /> if the <see cref="Tab" /> is enabled; <see langword="False" />
        ''' otherwise.</value>
        ''' <remarks>The default value of this property is <see langword="True" />.</remarks>
        Public Property Enabled() As Boolean
            Get
                If ViewState("Enabled") Is Nothing Then
                    ViewState("Enabled") = True
                End If
                Return ViewState("Enabled")
            End Get
            Set(ByVal value As Boolean)
                ViewState("Enabled") = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the client-side JavaScript to be called when the <see cref="Tab" /> is activated.
        ''' </summary>
        ''' <value>A <see cref="String" /> containing JavaScript code to execute when the <see cref="Tab" /> is
        ''' activated.</value>
        ''' <remarks>This code will get called by the JavaScript code in TabControl.js.</remarks>
        Public Property OnClientClick() As String
            Get
                Return ViewState("OnClientClick")
            End Get
            Set(ByVal value As String)
                ViewState("OnClientClick") = value
            End Set
        End Property

        Private m_HeaderTemplate As ITemplate

        ''' <summary>
        ''' Gets or sets the Header Template which gets rendered at the top of the tab.
        ''' </summary>
        ''' <value>An <see cref="ITemplate" /> object representing the tab's header section.</value>
        ''' <remarks>This property is generated by placing a &lt;HeaderTemplate&gt; tag inside an 
        ''' <see cref="Tab" /> object.</remarks>
        <PersistenceMode(PersistenceMode.InnerProperty)> _
        <TemplateInstance(TemplateInstance.Single)> _
        Public Property HeaderTemplate() As ITemplate
            Get
                Return m_HeaderTemplate
            End Get
            Set(ByVal value As ITemplate)
                m_HeaderTemplate = value
            End Set
        End Property

        Private m_ContentTemplate As ITemplate

        ''' <summary>
        ''' Gets or sets the Content Template which gets rendered in the body of the tab.
        ''' </summary>
        ''' <value>An <see cref="ITemplate" /> object representing the tab's content section.</value>
        ''' <remarks>This property is generated by placing a &lt;ContentTemplate&gt; tag inside an 
        ''' <see cref="Tab" /> object.</remarks>
        <PersistenceMode(PersistenceMode.InnerProperty)> _
        <TemplateInstance(TemplateInstance.Single)> _
        Public Property ContentTemplate() As ITemplate
            Get
                Return m_ContentTemplate
            End Get
            Set(ByVal value As ITemplate)
                m_ContentTemplate = value
            End Set
        End Property

        Private m_Header As PlaceHolder

        ''' <summary>
        ''' Gets or sets a control object through which you can programmatically add controls to the header 
        ''' section.
        ''' </summary>
        ''' <value>A <see cref="PlaceHolder" /> object that defines the header of the <see cref="Tab" /> 
        ''' control.</value>
        ''' <remarks>The <see cref="Header" /> property enables you to programmatically add 
        ''' controls to the header section without having to define a custom template that inherits from 
        ''' <see cref="ITemplate" />.  If you are adding content to the header declaratively, you should add
        ''' content to the <see cref="HeaderTemplate" /> property by using a &lt;HeaderTemplate&gt; 
        ''' element.</remarks>
        Public Property Header() As PlaceHolder
            Get
                If m_Header Is Nothing Then
                    m_Header = New PlaceHolder
                    Controls.Add(m_Header)
                End If
                Return m_Header
            End Get
            Set(ByVal value As PlaceHolder)
                m_Header = value
            End Set
        End Property

        Private m_Content As PlaceHolder

        ''' <summary>
        ''' Gets or sets a control object through which you can programmatically add controls to the content 
        ''' section.
        ''' </summary>
        ''' <value>A <see cref="PlaceHolder" /> object that defines the content of the <see cref="Tab" /> 
        ''' control.</value>
        ''' <remarks>The <see cref="Content" /> property enables you to programmatically add controls
        ''' to the content section without having to define a custom template that inherits from 
        ''' <see cref="ITemplate" />.  If you are adding content declaratively, you should add to the 
        ''' <see cref="ContentTemplate" /> property by using a &lt;ContentTemplate&gt; element.</remarks>
        Public Property Content() As PlaceHolder
            Get
                If m_Content Is Nothing Then
                    m_Content = New PlaceHolder
                    Controls.Add(m_Content)
                End If
                Return m_Content
            End Get
            Set(ByVal value As PlaceHolder)
                m_Content = value
            End Set
        End Property

        Private m_Width As Unit

        ''' <summary>
        ''' Gets or sets the width of the control.
        ''' </summary>
        ''' <value>A <see cref="Unit" /> representing the width of the control.</value>
        ''' <remarks><para>Unlike the height of the <see cref="Tab" />, which is set dynamically based on its 
        ''' contents, you can set the width of the <see cref="Tab" /> directly.</para>
        ''' <para>See <see cref="Unit" /> for more information on how to set this field.</para></remarks>
        Public Property Width() As Unit
            Get
                Return m_Width
            End Get
            Set(ByVal value As Unit)
                m_Width = value
            End Set
        End Property

        ''' <summary>
        ''' Sets the internal reference to the parent <see cref="TabControl" /> to <paramref name="owner" />.
        ''' </summary>
        ''' <param name="owner">The <see cref="TabControl" /> which contains this <see cref="Tab" />.</param>
        ''' <remarks>This property is set automatically when a <see cref="Tab" /> is added to the 
        ''' <see cref="TabControl" />'s <see cref="TabCollection" />.</remarks>
        Protected Friend Sub SetTabControl(ByVal owner As TabControl)
            m_TabControl = owner
        End Sub

        ' instantiates the templates in their containers.
        Private Sub Tab_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            If HeaderTemplate IsNot Nothing Then
                HeaderTemplate.InstantiateIn(Header)
            End If

            If ContentTemplate IsNot Nothing Then
                ContentTemplate.InstantiateIn(Content)
            End If
        End Sub

        ''' <summary>
        ''' Renders the portion of the <see cref="Tab" /> inside the tab ribbon which is displayed even when
        ''' the tab is inactive.
        ''' </summary>
        ''' <param name="writer">The <see cref="HtmlTextWriter" /> that receives the rendered output.</param>
        ''' <param name="IsActive"><see langword="True" /> if the <see cref="Tab" /> should be rendered as active,
        ''' <see langword="False" /> otherwise.</param>
        ''' <param name="StepNumber">An <see cref="Integer" /> to render as the step number of the 
        ''' <see cref="Tab" />.  The default value of this parameter is 0.</param>
        ''' <remarks>This method is called automatically by <see cref="TabControl.Render" />.  It should not
        ''' be called elsewhere, and it should not be overridden unless you need to change the method by which the
        ''' tab ribbon is displayed.</remarks>
        Protected Friend Sub RenderTab(ByVal writer As HtmlTextWriter, ByVal IsActive As Boolean, Optional ByVal StepNumber As Integer = 0)
            Dim sClass As String

            If IsActive Then
                sClass = m_TabControl.TabOnClass
            Else
                sClass = m_TabControl.TabOffClass
            End If

            If m_TabControl.ShowChecks And Checked Then
                sClass = sClass & " " & m_TabControl.TabCompleteClass
            End If

            writer.AddAttribute("class", sClass)

            writer.RenderBeginTag("li")

            writer.AddAttribute("id", "a-" & ClientID)
            writer.AddStyleAttribute("cursor", "pointer")
            If NavigateUrl <> String.Empty And Enabled Then
                writer.AddAttribute("href", NavigateUrl)
            End If
            writer.RenderBeginTag("a")
            If StepNumber > 0 Then
                writer.Write(StepNumber & ".&nbsp;")
            End If
            writer.Write(Label)

            writer.RenderEndTag() '</a>

            writer.RenderEndTag() '</li>
        End Sub

        ''' <summary>
        ''' Renders the portion of the <see cref="Tab" /> which is displayed only when the tab is active.
        ''' </summary>
        ''' <param name="writer">The <see cref="HtmlTextWriter" /> that receives the rendered output.</param>
        ''' <param name="IsActive"><see langword="True" /> if the <see cref="Tab" /> should be rendered as active,
        ''' <see langword="False" /> otherwise.</param>
        ''' <remarks>This method is called automatically by <see cref="TabControl.Render" />.  It should not
        ''' be called elsewhere, and it should not be overridden unless you need to change the method by which the
        ''' tab contents are displayed.</remarks>
        Protected Friend Sub RenderContent(ByVal writer As HtmlTextWriter, ByVal IsActive As Boolean)
            writer.AddAttribute("id", ClientID)
            If Not IsActive Then
                writer.AddStyleAttribute("display", "none")
            End If
            If Width <> Nothing Then
                writer.AddStyleAttribute("width", Width.ToString)
            End If
            writer.AddAttribute("class", CssClass)
            writer.RenderBeginTag("div")

            If HeaderTemplate IsNot Nothing Then
                writer.AddAttribute("class", "AETabHeader")
                writer.RenderBeginTag("div")
                Header.RenderControl(writer)
                writer.RenderEndTag() '</div>
            End If

            If ShowBorder = True Then writer.AddStyleAttribute("padding", "20px")
            writer.RenderBeginTag("div")
            Content.RenderControl(writer)
            writer.RenderEndTag() '</div>

            writer.RenderEndTag() '</div>
        End Sub
    End Class

    ''' <summary>
    ''' Contains a collection of <see cref="Tab" /> objects that the <see cref="TabControl" /> uses.
    ''' </summary>
    ''' <remarks>This <see cref="ControlCollection" /> only supports <see cref="Tab" /> objects.</remarks>
    Public Class TabCollection
        Inherits ControlCollection

        ''' <summary>
        ''' Initializes a new instance of the <see cref="TabCollection" /> class for the specified 
        ''' <paramref name="owner" />.
        ''' </summary>
        ''' <param name="owner">The <see cref="TabControl" /> that the control collection is created for.</param>
        ''' <remarks>Technically, this class will not throw an exception if instantiated on something other than
        ''' a <see cref="TabControl" />, but the class you'd use instead would need to have all the same
        ''' functionality or it would be fairly useless.</remarks>
        Public Sub New(ByVal owner As Control)
            MyBase.New(owner)
        End Sub

        ''' <summary>
        ''' Adds the specified <paramref name="child" /> to the collection.
        ''' </summary>
        ''' <param name="child">The <see cref="Tab" /> to add to the collection.</param>
        ''' <exception cref="ArgumentException">The <paramref name="child" /> is not a <see cref="Tab" /> object.</exception>
        ''' <exception cref="HttpException">The <see cref="TabCollection" /> is read-only.</exception>
        ''' <remarks><para>The new control is added to the end of an ordinal index array. The control must be a 
        ''' <see cref="Tab" /> object.</para>
        ''' <para>To add a control to the collection at a specific index location, use the <see cref="AddAt" /> 
        ''' method.</para></remarks>
        Public Overrides Sub Add(ByVal child As System.Web.UI.Control)
            If Not TypeOf child Is Tab Then
                Throw New ArgumentException("TabControl can only contain children of type Tab")
            End If
            MyBase.Add(child)
        End Sub

        ''' <summary>
        ''' Adds the specified <paramref name="child" /> to the collection at the specified 
        ''' <paramref name="index" />.
        ''' </summary>
        ''' <param name="child">The <see cref="Tab" /> to add to the collection.</param>
        ''' <param name="index">The location in the array at which to add the <paramref name="child" />.</param>
        ''' <exception cref="ArgumentException">The <paramref name="child" /> is not a <see cref="Tab" /> object.</exception>
        ''' <exception cref="ArgumentOutOfRangeException">The <paramref name="index" /> parameter is less than
        ''' zero or greater than or equal to <see cref="TabCollection.Count">TabCollection.Count</see>.</exception>
        ''' <exception cref="HttpException">The <see cref="TabCollection" /> is read-only.</exception>
        ''' <remarks>The added control must be a <see cref="Tab" /> object.</remarks>
        Public Overrides Sub AddAt(ByVal index As Integer, ByVal child As System.Web.UI.Control)
            If Not TypeOf child Is Tab Then
                Throw New ArgumentException("TabControl can only contain children of type Tab")
            End If
            MyBase.AddAt(index, child)
        End Sub

        ''' <summary>
        ''' Gets a reference to the <see cref="Tab" /> at the specified <paramref name="Index" /> in the 
        ''' <see cref="TabCollection" /> object.
        ''' </summary>
        ''' <param name="Index">The location of the <see cref="Tab" /> in the <see cref="TabCollection" />.</param>
        ''' <value>The reference to the <see cref="Tab" />.</value>
        ''' <exception cref="ArgumentOutOfRangeException">The <paramref name="Index" /> parameter is less than
        ''' zero or greater than or equal to <see cref="TabCollection.Count">TabCollection.Count</see>.</exception>
        ''' <remarks>This property replaces the <see cref="Item" /> property with a strongly typed reference to 
        ''' a <see cref="Tab" /> object.</remarks>
        Default Public ReadOnly Property Tab(ByVal Index As Integer) As Tab
            Get
                Return MyBase.Item(Index)
            End Get
        End Property
    End Class
End Namespace
