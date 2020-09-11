Imports Components
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports System.Text
Imports DataLayer

Namespace Controls
    ''' <summary>
    ''' Provides a tab selection control for the admin backend.
    ''' </summary>
    ''' <remarks>Each <see cref="AdminTab" /> is placed inside the control and its template is rendered when
    ''' the page is loaded.  The tabs are swapped in and out via JavaScript.
    ''' <seealso cref="AdminTab" />
    ''' <seealso cref="TabControl" /></remarks>
    <PersistChildren(False)> _
    <ParseChildren(True)> _
    Public Class AdminTabs
        Inherits CompositeControl

        ''' <summary>
        ''' A hidden field used by the control to store which tab is currently active.
        ''' </summary>
        ''' <remarks>This is a 0-based index: 0 is the first tab, 1 is the second, etc.</remarks>
        Protected hdnIndex As HiddenField

        ''' <summary>
        ''' A &lt;div&gt; element which contains the tab ribbon the user clicks on to select a tab.
        ''' </summary>
        Protected divTabs As HtmlGenericControl
        'Private apHelp As AjaxPopup

        Private m_Tabs As Generic.List(Of AdminTab)

        ''' <summary>
        ''' Gets the collection of <see cref="AdminTab">AdminTabs</see>.
        ''' </summary>
        ''' <value>A list of <see cref="AdminTab" /> objects which are to be rendered as part of the control.</value>
        ''' <remarks>This property can be accessed through the inner text of the ASP.NET code.</remarks>
        <PersistenceMode(PersistenceMode.InnerProperty)> _
        Public ReadOnly Property Tabs() As Generic.List(Of AdminTab)
            Get
                If m_Tabs Is Nothing Then
                    m_Tabs = New Generic.List(Of AdminTab)
                End If
                Return m_Tabs
            End Get
        End Property

        ''' <summary>
        ''' Gets the specified <see cref="AdminTab" />.
        ''' </summary>
        ''' <param name="ID">The <see cref="AdminTab.ID" /> of the tab that is needed.</param>
        ''' <value>The tab which has the specified <paramref name="ID" />.</value>
        ''' <exception cref="ArgumentException">The tab could not be found.</exception>
        ''' <remarks>Make sure that there is a tab with the specified <paramref name="ID" />.</remarks>
        Public ReadOnly Property Tab(ByVal ID As String) As AdminTab
            Get
                For Each t As AdminTab In Tabs
                    If t.ID = ID Then
                        Return t
                    End If
                Next
                Throw New ArgumentException("Tab with ID '" & ID & "' could not be found")
            End Get
        End Property

        Private m_HelpImageUrl As String

        ''' <summary>
        ''' Gets or sets the URL of the image to be used on the help button.
        ''' </summary>
        ''' <value>A <see cref="String" /> representing the URL of the image to be used on the help button.</value>
        ''' <remarks>This property must be set before <see cref="CreateChildControls" /> is called.</remarks>
        Public Property HelpImageUrl() As String
            Get
                Return m_HelpImageUrl
            End Get
            Set(ByVal value As String)
                m_HelpImageUrl = value
            End Set
        End Property

        ' Handles the Open event of the AjaxPopup by converting the provided code into a help string and
        ' storing it in the BodyTemplateContainer.
        'Private Sub apHelp_Open(ByVal sender As Object, ByVal e As EventArgs)
        '    Dim btn As ImageButton = sender
        '    Dim code As String = btn.CommandArgument
        '    Dim dbMsg As CustomTextRow = CustomTextRow.GetRowByCode(CType(Page, BasePage).DB, code)
        '    apHelp.BodyTemplateContainer.Controls.Clear()
        '    apHelp.BodyTemplateContainer.Controls.Add(New LiteralControl("<div style=""padding: 20px;"">" & dbMsg.Value & "</div>"))
        'End Sub

        ''' <summary>
        ''' Constructs the HTML of the control from the <see cref="AdminTab" /> objects that are provided, as
        ''' well as the various properties which control the appearance of the control.
        ''' </summary>
        ''' <remarks>This is guaranteed to be called no later than the Init phase.</remarks>
        Protected Overrides Sub CreateChildControls()

            hdnIndex = New HiddenField
            hdnIndex.ID = ID & "_Index"
            Controls.Add(hdnIndex)

            'apHelp = New AjaxPopup
            'With apHelp
            '    .Title = "Help"
            '    .OpenMode = AjaxPopupOpenMode.MoveToCenter
            '    .ShowVeil = True
            '    .VeilCloses = True
            '    .UpdateMode = UpdatePanelUpdateMode.Conditional
            '    .zIndex = 110
            '    .Width = New Unit(200, UnitType.Pixel)
            'End With
            'Controls.Add(apHelp)

            Dim divWrapper As New HtmlGenericControl("div")
            divWrapper.Attributes("class") = "tabModule"
            Controls.Add(divWrapper)

            divTabs = New HtmlGenericControl("div")
            divTabs.Attributes("class") = "tabsrow"
            divTabs.ID = "mainTabs"
            divWrapper.Controls.Add(divTabs)

            Dim ulTabs As New HtmlGenericControl("ul")
            ulTabs.Attributes("class") = "tabnav"
            divTabs.Controls.Add(ulTabs)

            divTabs.Controls.Add(New LiteralControl("<div class=""spacer"">&nbsp;</div>"))

            For Each t As AdminTab In Tabs
                If t.Visible Then
                    Dim divContent As New HtmlGenericControl("div")
                    divWrapper.Controls.Add(divContent)
                    divContent.ID = t.ID
                    If t.Width <> Nothing Then
                        divContent.Style("width") = t.Width.ToString
                    End If
                    divContent.Attributes("class") = "tabdiv ui-tabs-hide"

                    t.ClientId = divContent.ClientID

                    Dim phHeader As New PlaceHolder
                    If t.HeaderTemplate IsNot Nothing Then
                        t.HeaderTemplate.InstantiateIn(phHeader)
                        If t.HelpMessageCode <> Nothing Then
                            Dim spanHelp As New HtmlGenericControl("div")
                            spanHelp.Style("float") = "right"
                            'spanHelp.Style("width") = "50px"
                            spanHelp.Style("line-height") = "5px"
                            spanHelp.Style("text-align") = "center"
                            phHeader.Controls.AddAt(0, spanHelp)

                            'Dim btnHelp As New ImageButton
                            'btnHelp.ImageUrl = IIf(HelpImageUrl <> Nothing, HelpImageUrl, "/cms/images/utility/question.gif")
                            'btnHelp.AlternateText = "Help"
                            'btnHelp.Style("border") = "0px"
                            'btnHelp.ID = t.ID & "_Help"
                            'btnHelp.CommandArgument = t.HelpMessageCode
                            'btnHelp.CausesValidation = False
                            'AddHandler apHelp.Open, AddressOf apHelp_Open

                            'Dim trigger As New Controls.OpenTrigger
                            'trigger.ControlID = btnHelp.UniqueID
                            'apHelp.PopupTriggers.Add(trigger)

                            If Context.User IsNot Nothing AndAlso Context.User.Identity.Name <> Nothing Then
                                Dim DB As Database = CType(Page, BasePage).DB

                                spanHelp.Controls.Add(New LiteralControl("<table border=""0"" cellpadding=""0"" celspacing=""0""><tr><td>"))
                                'spanHelp.Controls.Add(btnHelp)
                                Dim dbAdmin As AdminRow = AdminRow.GetRowByUsername(DB, Context.User.Identity.Name)
                                Dim PermissionList As AdminSectionCollection
                                PermissionList = dbAdmin.GetPermissionList()

                                Dim HasAccess As Boolean = False

                                For Each dbAction As AdminSectionRow In PermissionList
                                    If dbAction.Code = "HELP_MESSAGES" Then
                                        HasAccess = True
                                        Exit For
                                    End If
                                Next

                                HasAccess = HasAccess Or dbAdmin.IsInternal

                                Dim dbText As CustomTextRow = CustomTextRow.GetRowByCode(DB, t.HelpMessageCode)
                                Dim HelpText As String = String.Empty

                                If dbText.Title <> String.Empty Then
                                    HelpText = "<h3>" & dbText.Title & "</h3>"
                                End If

                                If dbText.Value = String.Empty Then
                                    HelpText &= "There is no help available for this field."
                                Else
                                    HelpText &= dbText.Value
                                End If

                                HelpText = "<div class=""adminToolTipWrpr""><div class=""adminToolTipShadow"">" & HelpText & "</div><div class=""adminToolTopShadowBottom"">&nbsp;</div></div>"

                                If HasAccess Then
                                    Dim litHelp As New LiteralControl(String.Format("<a class=""tooltip""><img src=""{0}"" alt=""Help Message"" /></a>", IIf(HelpImageUrl <> Nothing, HelpImageUrl, "/cms/images/utility/question.gif")))
                                    spanHelp.Controls.Add(litHelp)
                                    spanHelp.Controls.Add(New LiteralControl(HelpText))

                                    Dim lnkHelp As New HyperLink
                                    lnkHelp.CssClass = "smallest"

                                    Dim Redirect As String = String.Empty
                                    Try
                                        Redirect = "&RedirectUrl=" & HttpContext.Current.Request.Url.PathAndQuery.ToString()
                                    Catch ex As Exception
                                        Redirect = ""
                                    End Try

                                    If dbText.TextId <> 0 Then
                                        lnkHelp.NavigateUrl = "/admin/help/edit.aspx?TextId=" & dbText.TextId
                                        lnkHelp.ImageUrl = "/cms/images/admin/edit.gif"
                                    Else

                                        lnkHelp.NavigateUrl = "/admin/help/edit.aspx?HelpCode=" & t.HelpMessageCode
                                        lnkHelp.ImageUrl = "/cms/images/admin/Create.gif"
                                    End If

                                    lnkHelp.NavigateUrl &= Redirect

                                    spanHelp.Controls.Add(New LiteralControl("</td><td>"))
                                    spanHelp.Controls.Add(lnkHelp)
                                ElseIf dbText.Value <> String.Empty Then
                                    Dim litHelp As New LiteralControl(String.Format("<a class=""tooltip""><img src=""{0}"" alt=""Help Message"" /></a>", IIf(HelpImageUrl <> Nothing, HelpImageUrl, "/cms/images/utility/question.gif")))
                                    spanHelp.Controls.Add(litHelp)
                                    spanHelp.Controls.Add(New LiteralControl(HelpText))
                                End If

                                spanHelp.Controls.Add(New LiteralControl("</td></tr></table>"))

                            End If
                        End If
                    End If
                    divContent.Controls.Add(New LiteralControl("<div class=""AETabHeader""><div style=""margin-bottom:4px;"">"))
                    divContent.Controls.Add(phHeader)
                    divContent.Controls.Add(New LiteralControl("</div></div>"))
                    t.HeaderContainer = phHeader

                    Dim pnlContent As New Panel
                    pnlContent.Style.Add("padding", "20px")
                    If t.ContentTemplate IsNot Nothing Then
                        t.ContentTemplate.InstantiateIn(pnlContent)
                    End If
                    divContent.Controls.Add(pnlContent)
                    t.ContentContainer = pnlContent

                    Dim ltlTab As New Literal
                    ltlTab.Text = "<li id=""li-" & divContent.ClientID & """><a href=""#" & divContent.ClientID & """>" & t.Label & "</a></li>"
                    ulTabs.Controls.Add(ltlTab)
                End If
            Next
        End Sub

        ''' <summary>
        ''' Gets the appropriate <see cref="HtmlTextWriterTag" /> value for this control.
        ''' </summary>
        ''' <value><see cref="HtmlTextWriterTag.Div" />.</value>
        ''' <remarks>This property ensures that this control is rendered as a &lt;div&gt; tag by the base class.</remarks>
        Protected Overrides ReadOnly Property TagKey() As System.Web.UI.HtmlTextWriterTag
            Get
                Return HtmlTextWriterTag.Div
            End Get
        End Property

        ''' <summary>
        ''' Raises the <see cref="PreRender" /> event for this control, as well as ensuring that the appropriate 
        ''' JavaScript has been registered with the page.
        ''' </summary>
        ''' <param name="e">An <see cref="EventArgs" /> object that contains the event data.</param>
        ''' <remarks>The JavaScript generated by this method is used to make the tabs switch in and out without
        ''' requiring postbacks.</remarks>
        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)

            Dim s As New StringBuilder("$(""#" & divTabs.ClientID & """).tabs();")
            s.AppendLine("$(""#" & divTabs.ClientID & """).bind('tabsselect',function(event, ui) { $(""#" & hdnIndex.ClientID & """).val(ui.index) });")
            Dim PageIsValid As Boolean = True
            Try
                'try/catch in case Page.Validate has not been called
                PageIsValid = Page.IsValid
            Catch ex As Exception
            End Try
            If Not PageIsValid Then
                For Each t As AdminTab In Tabs
                    Dim ctl As Control = FindControl(t.ID)
                    If Not IsValid(ctl) Then
                        s.AppendLine("$(""#" & divTabs.ClientID & """).tabs().tabs('select','" & t.ClientId & "');")
                        Exit For
                    End If
                Next
            ElseIf hdnIndex.Value <> String.Empty Then
                s.AppendLine("$(""#" & divTabs.ClientID & """).tabs().tabs('select'," & hdnIndex.Value & ");")
            Else
                s.AppendLine("$(""#" & divTabs.ClientID & """).tabs().tabs('select',0);")
            End If

            For Each t As AdminTab In Tabs
                If Not t.Visible Then
                    s.AppendLine("$(""#li-" & t.ClientId & """).hide();$(""#li-" & t.ClientId & "C"").hide();")
                End If
            Next

            'Page.ClientScript.RegisterStartupScript(Me.GetType, UniqueID & "_Init", s.ToString, True)
            ScriptManager.RegisterStartupScript(Page, Me.GetType, UniqueID & "_Init", s.ToString, True)
        End Sub

        ' This method ensures that CreateChildControls is called no later than the Init event.
        Private Sub AdminTabs_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            EnsureChildControls()
        End Sub

        ''' <summary>
        ''' Writes the appropriate begin tags for the <see cref="AdminTabs" /> object.
        ''' </summary>
        ''' <param name="writer">An <see cref="HtmlTextWriter" /> that represents the output stream to render
        ''' HTML content on the client.</param>
        ''' <remarks>In addition to the &lt;div&gt; tag generated automatically by the base class, this places 
        ''' this tag inside &lt;table&gt;, &lt;tr&gt;, and &lt;td&gt; tags.</remarks>
        Public Overrides Sub RenderBeginTag(ByVal writer As System.Web.UI.HtmlTextWriter)
            writer.WriteBeginTag("table")
            writer.WriteAttribute("width", "100%")
            writer.Write(HtmlTextWriter.TagRightChar)
            writer.WriteFullBeginTag("tr")
            writer.WriteFullBeginTag("td")
            MyBase.RenderBeginTag(writer)
        End Sub

        ''' <summary>
        ''' Writes the appropriate end tags for the <see cref="AdminTabs" /> object.
        ''' </summary>
        ''' <param name="writer">An <see cref="HtmlTextWriter" /> that represents the output stream to render
        ''' HTML content on the client.</param>
        ''' <remarks>In addition to the &lt;div&gt; tag generated automatically by the base class, this places 
        ''' this tag inside &lt;table&gt;, &lt;tr&gt;, and &lt;td&gt; tags.</remarks>
        Public Overrides Sub RenderEndTag(ByVal writer As System.Web.UI.HtmlTextWriter)
            MyBase.RenderEndTag(writer)
            writer.WriteEndTag("td")
            writer.WriteEndTag("tr")
            writer.WriteEndTag("table")
        End Sub

        ' Recursively determines whether or not the control is valid in order to make certain decisions regarding
        ' scripts that are registered in the OnPreRender method.
        Private Function IsValid(ByVal ctl As Control) As Boolean
            If TypeOf ctl Is IValidator AndAlso Not CType(ctl, IValidator).IsValid Then
                Return False
            ElseIf ctl.Controls.Count > 0 Then
                For Each child As Control In ctl.Controls
                    If Not IsValid(child) Then
                        'return as soon as invalid branch found
                        Return False
                    End If
                Next
            End If
            Return True
        End Function
    End Class

    ''' <summary>
    ''' Represents a single tab within the <see cref="AdminTabs" /> object.
    ''' </summary>
    ''' <remarks>This object is templated and is only meaningful within the <see cref="AdminTabs.Tabs" /> 
    ''' property of the <see cref="AdminTabs" /> object.
    ''' <seealso cref="AdminTabs" /></remarks>
    <PersistChildren(False)> _
    <ParseChildren(True)> _
    Public Class AdminTab
        Private m_HeaderTemplate As ITemplate

        ''' <summary>
        ''' Gets or sets the Header Template which gets rendered at the top of the tab.
        ''' </summary>
        ''' <value>An <see cref="ITemplate" /> object representing the tab's header section.</value>
        ''' <remarks>This property is generated by placing a &lt;HeaderTemplate&gt; tag inside an 
        ''' <see cref="AdminTab" /> object.</remarks>
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
        ''' <see cref="AdminTab" /> object.</remarks>
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

        Private m_HeaderContainer As Control

        ''' <summary>
        ''' Gets or sets a control object through which you can programmatically add controls to the header 
        ''' section.
        ''' </summary>
        ''' <value>A <see cref="Control" /> object that defines the header of the <see cref="AdminTab" /> 
        ''' control.</value>
        ''' <remarks>The <see cref="HeaderContainer" /> property enables you to programmatically add 
        ''' controls to the header section without having to define a custom template that inherits from 
        ''' <see cref="ITemplate" />.  If you are adding content to the header declaratively, you should add
        ''' content to the <see cref="HeaderTemplate" /> property by using a &lt;HeaderTemplate&gt; 
        ''' element.</remarks>
        Public Property HeaderContainer() As Control
            Get
                Return m_HeaderContainer
            End Get
            Protected Friend Set(ByVal value As Control)
                m_HeaderContainer = value
            End Set
        End Property

        Private m_ContentContainer As Control

        ''' <summary>
        ''' Gets or sets a control object through which you can programmatically add controls to the content 
        ''' section.
        ''' </summary>
        ''' <value>A <see cref="Control" /> object that defines the content of the <see cref="AdminTab" /> 
        ''' control.</value>
        ''' <remarks>The <see cref="ContentContainer" /> property enables you to programmatically add controls
        ''' to the content section without having to define a custom template that inherits from 
        ''' <see cref="ITemplate" />.  If you are adding content declaratively, you should add to the 
        ''' <see cref="ContentTemplate" /> property by using a &lt;ContentTemplate&gt; element.</remarks>
        Public Property ContentContainer() As Control
            Get
                Return m_ContentContainer
            End Get
            Protected Friend Set(ByVal value As Control)
                m_ContentContainer = value
            End Set
        End Property

        Private m_ClientId As String
        ''' <summary>
        ''' Gets or sets the <see cref="Control.ClientID" /> property for the main container of the tab.
        ''' </summary>
        ''' <value>A <see cref="String" /> representing the ID on the client of the main container for this tab.</value>
        ''' <remarks>This is automatically set in <see cref="AdminTabs.CreateChildControls" /> and used in the 
        ''' JavaScript generated by <see cref="AdminTabs.OnPreRender" />.</remarks>
        Protected Friend Property ClientId() As String
            Get
                Return m_ClientId
            End Get
            Set(ByVal value As String)
                m_ClientId = value
            End Set
        End Property

        Private m_ID As String
        ''' <summary>
        ''' Gets or sets the <see cref="Control.ID" /> property for the main container of the tab
        ''' </summary>
        ''' <value>A <see cref="String" /> representing the <see cref="Control.ID" /> of the main container for this tab.</value>
        ''' <remarks>This property is used to name a div control, and is also used to look up this particular tab
        ''' within the others that may be within the tab control.</remarks>
        Public Property ID() As String
            Get
                Return m_ID
            End Get
            Set(ByVal value As String)
                m_ID = value
            End Set
        End Property

        Private m_Label As String
        ''' <summary>
        ''' Gets or sets the label on the tab ribbon.
        ''' </summary>
        ''' <value>A <see cref="String" /> to be displayed on the tab ribbon.</value>
        Public Property Label() As String
            Get
                Return m_Label
            End Get
            Set(ByVal value As String)
                m_Label = value
            End Set
        End Property

        Private m_Visible As Boolean = True
        ''' <summary>
        ''' Gets or sets the server-side visibility of the tab.
        ''' </summary>
        ''' <value><see langword="True" /> if the tab is visible, <see langword="False" /> otherwise.</value>
        ''' <remarks>This is a different thing from which tab is currently selected.  A tab is visible if 
        ''' it can be selected by the user, regardless of whether or not it has been.</remarks>
        Public Property Visible() As Boolean
            Get
                Return m_Visible
            End Get
            Set(ByVal value As Boolean)
                m_Visible = value
            End Set
        End Property

        Private m_HelpMessageCode As String
        ''' <summary>
        ''' Gets or sets the code used to retrieve the help message for the given tab.
        ''' </summary>
        ''' <value>A <see cref="String" /> representing a code in the CustomText table used to retrieve the
        ''' help message for the given tab.</value>
        ''' <remarks>Make sure there is a row in the CustomText table which has this string as its code.</remarks>
        Public Property HelpMessageCode() As String
            Get
                Return m_HelpMessageCode
            End Get
            Set(ByVal value As String)
                m_HelpMessageCode = value
            End Set
        End Property

        Private m_Width As Unit
        ''' <summary>
        ''' Gets or sets the width of this tab
        ''' </summary>
        ''' <value>A <see cref="Unit" /> representing the width of this tab.</value>
        ''' <remarks>See <see cref="Unit" /> for more information on how to set this field.</remarks>
        Public Property Width() As Unit
            Get
                Return m_Width
            End Get
            Set(ByVal value As Unit)
                m_Width = value
            End Set
        End Property

    End Class
End Namespace
