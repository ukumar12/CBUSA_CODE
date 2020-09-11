Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports System.Text
Imports MasterPages
Imports Components

Namespace Controls
    ''' <summary>
    ''' Represents a panel that can be triggered to "pop up" asynchronously as a result of certain triggers.
    ''' </summary>
    ''' <remarks>The templates <see cref="AjaxPopup.LoadingTemplate" />, 
    ''' <see cref="AjaxPopup.HeaderTemplate" />, and <see cref="AjaxPopup.BodyTemplate" /> are used to specify
    ''' the contents of the popup panel under various circumstances.
    ''' <seealso cref="Popup" />
    ''' <seealso cref="AjaxPopupTrigger" />
    ''' <seealso cref="OpenTrigger" />
    ''' <seealso cref="CloseTrigger" />
    ''' <seealso cref="AjaxPopupOpenMode" /></remarks>
    <PersistChildren(False)> _
    <ParseChildren(True)> _
    Public Class AjaxPopup
        Inherits UpdatePanel
        Implements INamingContainer

        ''' <summary>
        ''' Occurs when the popup panel is opened.
        ''' </summary>
        ''' <remarks>The <see cref="Open" /> event is raised when the <see cref="AjaxPopup" /> is opened.  
        ''' It allows for custom functionality when this occurs.</remarks>
        Public Event Open As EventHandler

        ''' <summary>
        ''' Occurs when the popup panel is closed.
        ''' </summary>
        ''' <remarks>The <see cref="Close" /> event is raised when the <see cref="AjaxPopup" /> is closed.  
        ''' It allows for custom functionality when this occurs.</remarks>
        Public Event Close As EventHandler

        ''' <summary>
        ''' Occurs when a button inside the control raises an event.
        ''' </summary>
        ''' <remarks>The <see cref="PopupCommand" /> event is raised when a button is clicked inside the 
        ''' <see cref="AjaxPopup" /> control.  It allows for custom functionality when this occurs.</remarks>
        Public Event PopupCommand As CommandEventHandler

        Private m_Popup As Popup

        ''' <summary>
        ''' Creates a new <see cref="AjaxPopup" /> control.
        ''' </summary>
        ''' <remarks>This method initializes the internal <see cref="Popup" /> object which this class is a
        ''' wrapper for.</remarks>
        Public Sub New()
            m_Popup = New Popup
            m_Popup.ID = "ctlPopup"
            AddHandler m_Popup.Open, AddressOf m_Popup_Open
            AddHandler m_Popup.Close, AddressOf m_Popup_Close
        End Sub

        ''' <summary>.
        ''' Gets or sets the Loading Template for the internal <see cref="Popup" /> object.
        ''' </summary>
        ''' <value>An <see cref="ITemplate" /> object representing the content shown when the popup is
        ''' loading.</value>
        ''' <remarks>This property exposes the <see cref="Popup.LoadingTemplate" /> property of the internal 
        ''' control.  Use this property for adding controls via markup.</remarks>
        <TemplateInstance(TemplateInstance.Single)> _
        <PersistenceMode(PersistenceMode.InnerProperty)> _
        Public Property LoadingTemplate() As ITemplate
            Get
                Return m_Popup.LoadingTemplate
            End Get
            Set(ByVal value As ITemplate)
                m_Popup.LoadingTemplate = value
            End Set
        End Property

        ''' <summary>
        ''' Gets a <see cref="Control" /> object to which you can programmatically add controls to be displayed 
        ''' while the popup control is loading.
        ''' </summary>
        ''' <value>A <see cref="Control" /> object that defines the controls visible while the 
        ''' popup control is loading.</value>
        ''' <remarks>This property exposes the <see cref="Popup.LoadingTemplateContainer" /> property of the
        ''' internal control.  Use this property for adding controls programmatically.</remarks>
        Public ReadOnly Property LoadingTemplateContainer() As Control
            Get
                Return m_Popup.LoadingTemplateContainer
            End Get
        End Property

        ''' <summary>.
        ''' Gets or sets the Header Template for the internal <see cref="Popup" /> object.
        ''' </summary>
        ''' <value>An <see cref="ITemplate" /> object representing the header of the popup control.</value>
        ''' <remarks>This property exposes the <see cref="Popup.HeaderTemplate" /> property of the internal 
        ''' control.  Use this property for adding controls via markup.</remarks>
        <TemplateInstance(TemplateInstance.Single)> _
        <PersistenceMode(PersistenceMode.InnerProperty)> _
        Public Property HeaderTemplate() As ITemplate
            Get
                Return m_Popup.HeaderTemplate
            End Get
            Set(ByVal value As ITemplate)
                m_Popup.HeaderTemplate = value
            End Set
        End Property

        ''' <summary>
        ''' Gets a <see cref="Control" /> object through which you can programmatically add controls to 
        ''' the header.
        ''' </summary>
        ''' <value>A <see cref="Control" /> object that defines the header of the popup control.</value>
        ''' <remarks>This property exposes the <see cref="Popup.HeaderTemplateContainer" /> property of the
        ''' internal control.  Use this property for adding controls programmatically.</remarks>
        Public ReadOnly Property HeaderTemplateContainer() As Control
            Get
                Return m_Popup.HeaderTemplateContainer
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the Body Template for the internal <see cref="Popup" /> object.
        ''' </summary>
        ''' <value>An <see cref="ITemplate" /> object representing the body of the popup control.</value>
        ''' <remarks>This property exposes the <see cref="Popup.BodyTemplate" /> property of the internal 
        ''' control.  Use this property for adding controls via markup.</remarks>
        <TemplateInstance(TemplateInstance.Single)> _
        <PersistenceMode(PersistenceMode.InnerProperty)> _
        Public Property BodyTemplate() As ITemplate
            Get
                Return m_Popup.BodyTemplate
            End Get
            Set(ByVal value As ITemplate)
                m_Popup.BodyTemplate = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the <see cref="Control" /> object through which you can programmatically add controls to the
        ''' body.
        ''' </summary>
        ''' <value>A <see cref="Control" /> that defines the body of the popup control.</value>
        ''' <remarks>This property exposes the <see cref="Popup.BodyTemplateContainer" /> property of the
        ''' internal control.  Use this property for adding controls programmatically.</remarks>
        Public ReadOnly Property BodyTemplateContainer() As Control
            Get
                Return m_Popup.BodyTemplateContainer
            End Get
        End Property

        ''' <summary>
        ''' This property of the base class is not used by this derived class.
        ''' </summary>
        ''' <value><see langword="Nothing" />.</value>
        ''' <remarks>Don't bother trying to set this.</remarks>
        Private Overloads Property ContentTemplate() As ITemplate
            Get
                Return Nothing
            End Get
            Set(ByVal value As ITemplate)
            End Set
        End Property

        Private m_Triggers As GenericCollection(Of AjaxPopupTrigger)

        ''' <summary>
        ''' Gets the collection of <see cref="AjaxPopupTrigger">AjaxPopupTriggers</see> which can open or 
        ''' close the control.
        ''' </summary>
        ''' <value>A collection of <see cref="AjaxPopupTrigger">AjaxPopupTriggers</see> which tell the control
        ''' under what circumstances it should open and close.</value>
        ''' <remarks>This collection should contain a set of <see cref="OpenTrigger" /> and 
        ''' <see cref="CloseTrigger" /> objects with their <see cref="AjaxPopupTrigger.ControlID" /> property
        ''' set to the controls that you want to trigger the popup to open or close.</remarks>
        <PersistenceMode(PersistenceMode.InnerProperty)> _
        Public ReadOnly Property PopupTriggers() As GenericCollection(Of AjaxPopupTrigger)
            Get
                If m_Triggers Is Nothing Then
                    m_Triggers = New GenericCollection(Of AjaxPopupTrigger)
                End If
                Return m_Triggers
            End Get
        End Property

        'Private Shadows ReadOnly Property Triggers() As UpdatePanelTriggerCollection
        '    Get
        '        Return Nothing
        '    End Get
        'End Property

        ''' <summary>
        ''' Gets the <see cref="Control.ClientID" /> property of the window opened by the popup control.
        ''' </summary>
        ''' <value>A <see cref="String" /> representing the ID on the client of the window opened by the popup
        ''' control.</value>
        ''' <remarks>This property exposes the <see cref="Control.ClientID" /> property of the internal
        ''' <see cref="Popup.Window" />.</remarks>
        Public ReadOnly Property PopupClientId() As String
            Get
                Return m_Popup.Window.ClientID
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the width of the popup window.
        ''' </summary>
        ''' <value>A <see cref="Unit" /> representing the width of the <see cref="Popup.Window" />.</value>
        ''' <remarks><para>This property exposes the <see cref="Popup.Width" /> property of the internal 
        ''' control.</para>
        ''' <para>See <see cref="Unit" /> for more information on how to set this field.</para></remarks>
        Public Property Width() As Unit
            Get
                Return m_Popup.Width
            End Get
            Set(ByVal value As Unit)
                m_Popup.Width = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the height of the popup window.
        ''' </summary>
        ''' <value>A <see cref="Unit" /> representing the height of the <see cref="Popup.Window" />.</value>
        ''' <remarks><para>This property exposes the <see cref="Popup.Height" /> property of the internal 
        ''' control.</para>
        ''' <para>See <see cref="Unit" /> for more information on how to set this field.</para></remarks>
        Public Property Height() As Unit
            Get
                Return m_Popup.Height
            End Get
            Set(ByVal value As Unit)
                m_Popup.Height = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets whether there is a postback when the popup window is opened.
        ''' </summary>
        ''' <value><see langword="True" /> if opening the popup window triggers a postback, 
        ''' <see langword="False" /> otherwise.</value>
        ''' <remarks>Default is True. This property exposes the <see cref="Popup.PostbackOnOpen" /> property of the internal
        ''' control.</remarks>
        Public Property PostbackOnOpen() As Boolean
            Get
                Return m_Popup.PostbackOnOpen
            End Get
            Set(ByVal value As Boolean)
                m_Popup.PostbackOnOpen = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets whether there is a postback when the popup window is closed.
        ''' </summary>
        ''' <value><see langword="True" /> if closing the popup window triggers a postback,
        ''' <see langword="False" /> otherwise.
        ''' </value>
        ''' <remarks>Default is True. This property exposes the <see cref="Popup.PostbackOnClose" /> property of the internal
        ''' control.</remarks>
        Public Property PostbackOnClose() As Boolean
            Get
                Return m_Popup.PostbackOnClose
            End Get
            Set(ByVal value As Boolean)
                m_Popup.PostbackOnClose = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets whether or not the popup window can be dragged around the screen.
        ''' </summary>
        ''' <value><see langword="True" /> if the popup window can be dragged around the screen; 
        ''' <see langword="False" /> otherwise.</value>
        ''' <remarks>This property exposes the <see cref="Popup.IsDraggable" /> property of the internal
        ''' control.</remarks>
        Public Property IsDraggable() As Boolean
            Get
                Return m_Popup.IsDraggable
            End Get
            Set(ByVal value As Boolean)
                m_Popup.IsDraggable = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the z-index CSS style of the popup window.
        ''' </summary>
        ''' <value>An <see cref="Integer" /> representing the value of the CSS z-index of the popup window.</value>
        ''' <remarks>This property exposes the <see cref="Popup.zIndex" /> property of the internal control.</remarks>
        Public Property zIndex() As Integer
            Get
                Return m_Popup.zIndex
            End Get
            Set(ByVal value As Integer)
                m_Popup.zIndex = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets whether or not the popup window is open.
        ''' </summary>
        ''' <value><see langword="True" /> if the popup window is currently open; <see langword="False" /> 
        ''' otherwise.</value>
        ''' <remarks>This property exposes the <see cref="Popup.IsOpen" /> property of the internal control.</remarks>
        Public Property IsOpen() As Boolean
            Get
                Return m_Popup.IsOpen
            End Get
            Set(ByVal value As Boolean)
                m_Popup.IsOpen = value
            End Set
        End Property

        Public Property VeilClass() As String
            Get
                Return m_Popup.VeilClass
            End Get
            Set(ByVal value As String)
                m_Popup.VeilClass = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets whether clicking on the veil closes the popup window.
        ''' </summary>
        ''' <value><see langword="True" /> if clicking on the veil will close the popup window; 
        ''' <see langword="False" /> otherwise.</value>
        ''' <remarks>This property exposes the <see cref="Popup.VeilCloses" /> property of the internal control.</remarks>
        Public Property VeilCloses() As Boolean
            Get
                Return m_Popup.VeilCloses
            End Get
            Set(ByVal value As Boolean)
                m_Popup.VeilCloses = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the <see cref="Global.AjaxPopupOpenMode" /> of the control.
        ''' </summary>
        ''' <value><see cref="Global.AjaxPopupOpenMode.MoveToCenter" /> if the popup window will move to the center
        ''' of the screen when opened; <see cref="Global.AjaxPopupOpenMode.MoveToClick" /> if the popup window will
        ''' move to where the mouse has been clicked when opened; <see cref="Global.AjaxPopupOpenMode.None" /> if
        ''' the popup window will not move at all.</value>
        ''' <remarks>This property exposes the <see cref="Popup.OpenMode" /> property of the internal control.</remarks>
        Public Property OpenMode() As AjaxPopupOpenMode
            Get
                Return m_Popup.OpenMode
            End Get
            Set(ByVal value As AjaxPopupOpenMode)
                m_Popup.OpenMode = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets whether the veil is visible.
        ''' </summary>
        ''' <value><see langword="True" /> if the veil is visible; <see langword="False" /> otherwise.</value>
        ''' <remarks>This property exposes the <see cref="Popup.ShowVeil" /> property of the internal control.</remarks>
        Public Property ShowVeil() As Boolean
            Get
                Return m_Popup.ShowVeil
            End Get
            Set(ByVal value As Boolean)
                m_Popup.ShowVeil = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the time in seconds before the loading screen is shown.
        ''' </summary>
        ''' <value>An <see cref="Integer" /> representing the number of seconds between the popup being triggered
        ''' to open and the loading screen beginning to show.</value>
        ''' <remarks>The default value, as set by the <see cref="Popup.LoadingPopupDelay" /> property which
        ''' this property exposes, is three seconds.</remarks>
        Public Property LoadingPopupDelay() As Integer
            Get
                Return m_Popup.LoadingPopupDelay
            End Get
            Set(ByVal value As Integer)
                m_Popup.LoadingPopupDelay = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the title of the popup window
        ''' </summary>
        ''' <value>A <see cref="String" /> representing the title of the popup window.</value>
        ''' <remarks>This property exposes the <see cref="Popup.Title" /> property of the internal control.</remarks>
        Public Property Title() As String
            Get
                Return m_Popup.Title
            End Get
            Set(ByVal value As String)
                m_Popup.Title = value
            End Set
        End Property

        ''' <overloads>Performs validation and triggers an update to the base <see cref="UpdatePanel" /> if 
        ''' necessary.</overloads>
        ''' <summary>
        ''' Validates the popup control and triggers an update to the base <see cref="UpdatePanel" /> if 
        ''' necessary.
        ''' </summary>
        ''' <returns><see langword="True" /> if the control is valid; <see langword="False" /> otherwise.</returns>
        ''' <remarks>The validation of this control is handled by the validation methods in the internal
        ''' <see cref="Popup" /> class.  Also, the base <see cref="UpdatePanel" /> updates specifically if the
        ''' control is not valid and the current postback is asynchronous.</remarks>
        Public Function Validate() As Boolean
            Dim ret As Boolean = m_Popup.Validate()
            If Not ret And ScriptManager.GetCurrent(Page).IsInAsyncPostBack Then
                Update()
            End If
            Return ret
        End Function

        ''' <summary>
        ''' Validates the <paramref name="ValidationGroup" /> and triggers an update to the 
        ''' base <see cref="UpdatePanel" /> if necessary.
        ''' </summary>
        ''' <param name="ValidationGroup">A <see cref="String" /> representing the 
        ''' <see cref="BaseValidator.ValidationGroup" /> to validate.</param>
        ''' <returns><see langword="True" /> if the <paramref name="ValidationGroup" /> is valid; 
        ''' <see langword="False" /> otherwise.</returns>
        ''' <remarks>The validation of this control is handled by the validation methods in the internal
        ''' <see cref="Popup" /> class.  Also, the base <see cref="UpdatePanel" /> updates specifically if the
        ''' control is not valid and the current postback is asynchronous.</remarks>
        Public Function Validate(ByVal ValidationGroup As String) As Boolean
            Dim ret As Boolean = m_Popup.Validate(ValidationGroup)
            If Not ret And ScriptManager.GetCurrent(Page).IsInAsyncPostBack Then
                Update()
            End If
            Return ret
        End Function

        ''' <summary>
        ''' Adds an error message to the popup window.
        ''' </summary>
        ''' <param name="Message">A <see cref="String" /> containing the error message to add.</param>
        ''' <remarks>This method is a wrapper for <see cref="Popup.AddError" />.</remarks>
        Public Sub AddError(ByVal Message As String)
            m_Popup.AddError(Message)
        End Sub

        ''' <summary>
        ''' Searches the internal <see cref="Popup" /> class for the control with the specified 
        ''' <paramref name="id" />
        ''' </summary>
        ''' <param name="id">the <see cref="Control.ID" /> of the control being searched for.</param>
        ''' <returns><see langword="Nothing" /> if the control is not found; A <see cref="Control" /> with the 
        ''' specified <paramref name="id" /> otherwise.</returns>
        ''' <remarks>This method is a wrapper for the <see cref="Popup.FindControl" /> method on the internal
        ''' control.</remarks>
        Public Overrides Function FindControl(ByVal id As String) As Control
            Return m_Popup.FindControl(m_Popup, id)
        End Function

        ' This method puts the internal Popup control into the ContentTemplateContainer of the base UpdatePanel.
        Private Sub AjaxPopup_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            ContentTemplateContainer.Controls.Add(m_Popup)
        End Sub

        ''' <summary>
        ''' Creates the <see cref="Control" /> object that acts as the container for the internal 
        ''' <see cref="Popup" /> object.
        ''' </summary>
        ''' <returns>A &lt;div&gt; element with a CSS height style of 0px.</returns>
        ''' <remarks>This method means that the <see cref="Popup" /> class will be placed inside this
        ''' particular &lt;div&gt; element.</remarks>
        Protected Overrides Function CreateContentTemplateContainer() As System.Web.UI.Control
            Dim cntr As New HtmlGenericControl("div")
            cntr.Attributes.CssStyle("height") = "0px"
            Return cntr
        End Function

        ' This method adds the OpenTriggers and CloseTriggers to the appropriate categories within the Popup
        ' class.
        Private Sub AjaxPopup_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            For Each t As AjaxPopupTrigger In PopupTriggers
                If TypeOf t Is OpenTrigger Then
                    m_Popup.OpenTriggers.Add(t)
                ElseIf TypeOf t Is CloseTrigger Then
                    m_Popup.CloseTriggers.Add(t)
                End If
            Next
        End Sub

        ' This method catches the Popup.Open event and raises it as an AjaxPopup.Open event.
        Private Sub m_Popup_Open(ByVal sender As Object, ByVal e As EventArgs)
            RaiseEvent Open(sender, e)
            Update()
        End Sub

        ' This method catches the Popup.Close event and raises it as an AjaxPopup.Close event.
        Private Sub m_Popup_Close(ByVal sender As Object, ByVal e As EventArgs)
            RaiseEvent Close(sender, e)
            Update()
        End Sub

        ''' <summary>
        ''' This method coalesces all of the events raised by buttons in the control and raises them as 
        ''' <see cref="PopupCommand" /> events
        ''' </summary>
        ''' <param name="source">The source of the event.</param>
        ''' <param name="args">An <see cref="EventArgs" /> object that contains the event data.</param>
        ''' <returns><see langword="True" /> if the event has been canceled; <see langword="False" /> otherwise.</returns>
        ''' <remarks>This method only deviates from the code written by the base class if the source of the
        ''' event is a button.  Otherwise, it will simply do whatever the base class would do.</remarks>
        Protected Overrides Function OnBubbleEvent(ByVal source As Object, ByVal args As System.EventArgs) As Boolean
            Dim name As String = String.Empty
            Dim arg As String = String.Empty
            If TypeOf source Is Button Then
                name = CType(source, Button).CommandName
                arg = CType(source, Button).CommandArgument
                RaiseEvent PopupCommand(Me, New CommandEventArgs(name, arg))
            End If
            Return MyBase.OnBubbleEvent(source, args)
        End Function
    End Class

    ''' <summary>
    ''' A helper class used in the implementation of the <see cref="AjaxPopup" /> class.
    ''' </summary>
    ''' <remarks>You should not have to use this class except insofar as it must be modified or used in order to
    ''' change the functionality of the <see cref="AjaxPopup" /> class.
    ''' <seealso cref="AjaxPopup" />
    ''' <seealso cref="AjaxPopupTrigger" />
    ''' <seealso cref="OpenTrigger" />
    ''' <seealso cref="CloseTrigger" />
    ''' <seealso cref="AjaxPopupOpenMode" /></remarks>
    Public Class Popup
        Inherits Control
        Implements IScriptControl
        Implements IPostBackDataHandler
        'Implements IPostBackEventHandler

        ''' <summary>
        ''' Occurs when the popup panel is opened.
        ''' </summary>
        ''' <remarks>The <see cref="Open" /> event is raised when the <see cref="Popup" /> is opened.  
        ''' It allows for custom functionality when this occurs.</remarks>
        Public Event Open As EventHandler

        ''' <summary>
        ''' Occurs when the popup panel is closed.
        ''' </summary>
        ''' <remarks>The <see cref="Close" /> event is raised when the <see cref="AjaxPopup" /> is closed.  
        ''' It allows for custom functionality when this occurs.</remarks>
        Public Event Close As EventHandler

        Private m_WindowDiv As HtmlGenericControl
        Private m_HeaderDiv As HtmlGenericControl
        Private m_ErrorCtl As ErrorMessage
        Private m_BodyDiv As HtmlGenericControl
        Private m_Postback As UnvalidatedPostback2
        Private m_LoadingDiv As HtmlGenericControl
        Private m_WasOpen As Boolean

        Private m_LoadingTemplate As ITemplate

        ''' <summary>.
        ''' Gets or sets the Loading Template for the <see cref="Popup" /> object.
        ''' </summary>
        ''' <value>An <see cref="ITemplate" /> object representing the content shown when the popup is
        ''' loading.</value>
        ''' <remarks>This property is manipulated indirectly through the 
        ''' <see cref="AjaxPopup.LoadingTemplate" /> property.  Use this property for adding controls via 
        ''' markup.</remarks>
        <PersistenceMode(PersistenceMode.InnerProperty)> _
        Public Property LoadingTemplate() As ITemplate
            Get
                Return m_LoadingTemplate
            End Get
            Set(ByVal value As ITemplate)
                m_LoadingTemplate = value
            End Set
        End Property

        ''' <summary>
        ''' Gets a <see cref="Control" /> object to which you can programmatically add controls to be displayed 
        ''' while the popup control is loading.
        ''' </summary>
        ''' <value>A <see cref="Control" /> object that defines the controls visible while the 
        ''' popup control is loading.</value>
        ''' <remarks>This property is manipulated indirectly through the
        ''' <see cref="AjaxPopup.LoadingTemplateContainer" /> property.  Use this property for adding 
        ''' controls programmatically.</remarks>
        Public ReadOnly Property LoadingTemplateContainer() As Control
            Get
                Return m_LoadingDiv
            End Get
        End Property

        Private m_HeaderTemplate As ITemplate

        ''' <summary>.
        ''' Gets or sets the Header Template for the internal <see cref="Popup" /> object.
        ''' </summary>
        ''' <value>An <see cref="ITemplate" /> object representing the header of the popup control.</value>
        ''' <remarks>This property is manipulated indirectly through the
        ''' <see cref="AjaxPopup.HeaderTemplate" /> property.  Use this property for adding controls via 
        ''' markup.</remarks>
        <PersistenceMode(PersistenceMode.InnerProperty)> _
        Public Property HeaderTemplate() As ITemplate
            Get
                Return m_HeaderTemplate
            End Get
            Set(ByVal value As ITemplate)
                m_HeaderTemplate = value
            End Set
        End Property

        ''' <summary>
        ''' Gets a <see cref="Control" /> object through which you can programmatically add controls to 
        ''' the header.
        ''' </summary>
        ''' <value>A <see cref="Control" /> object that defines the header of the popup control.</value>
        ''' <remarks>This property is manipulated indirectly through the
        ''' <see cref="AjaxPopup.HeaderTemplateContainer" /> property.  Use this property for adding 
        ''' controls programmatically.</remarks>
        Public ReadOnly Property HeaderTemplateContainer() As Control
            Get
                Return m_HeaderDiv
            End Get
        End Property

        Private m_BodyTemplate As ITemplate

        ''' <summary>
        ''' Gets or sets the Body Template for the internal <see cref="Popup" /> object.
        ''' </summary>
        ''' <value>An <see cref="ITemplate" /> object representing the body of the popup control.</value>
        ''' <remarks>This property is manipulated indirectly through the
        ''' <see cref="AjaxPopup.BodyTemplate" /> property.  Use this property for adding controls via 
        ''' markup.</remarks>
        <PersistenceMode(PersistenceMode.InnerProperty)> _
        Public Property BodyTemplate() As ITemplate
            Get
                Return m_BodyTemplate
            End Get
            Set(ByVal value As ITemplate)
                m_BodyTemplate = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the <see cref="Control" /> object through which you can programmatically add controls to the
        ''' body.
        ''' </summary>
        ''' <value>A <see cref="Control" /> that defines the body of the popup control.</value>
        ''' <remarks>This property is manipulated indirectly through the
        ''' <see cref="AjaxPopup.BodyTemplateContainer" /> property.  Use this property for adding 
        ''' controls programmatically.</remarks>
        Public ReadOnly Property BodyTemplateContainer() As Control
            Get
                Return m_BodyDiv
            End Get
        End Property

        Private m_OpenTriggers As Generic.List(Of AjaxPopupTrigger)

        ''' <summary>
        ''' Gets the collection of <see cref="AjaxPopupTrigger">AjaxPopupTriggers</see> which open the control.
        ''' </summary>
        ''' <value>A collection of <see cref="AjaxPopupTrigger">AjaxPopupTriggers</see> which tell the control
        ''' under what circumstances it should open.</value>
        ''' <remarks>This collection should contain a set of <see cref="OpenTrigger" /> objects with their 
        ''' <see cref="AjaxPopupTrigger.ControlID" /> property set to the controls that you want to trigger 
        ''' the popup to open.  It is populated automatically by the <see cref="AjaxPopup" /> class.</remarks>
        <PersistenceMode(PersistenceMode.InnerProperty)> _
        Public Property OpenTriggers() As Generic.List(Of AjaxPopupTrigger)
            Get
                If m_OpenTriggers Is Nothing Then
                    m_OpenTriggers = New Generic.List(Of AjaxPopupTrigger)
                End If
                Return m_OpenTriggers
            End Get
            Set(ByVal value As Generic.List(Of AjaxPopupTrigger))
                m_OpenTriggers = value
            End Set
        End Property

        Private m_CloseTriggers As Generic.List(Of AjaxPopupTrigger)

        ''' <summary>
        ''' Gets the collection of <see cref="AjaxPopupTrigger">AjaxPopupTriggers</see> which close the control.
        ''' </summary>
        ''' <value>A collection of <see cref="AjaxPopupTrigger">AjaxPopupTriggers</see> which tell the control
        ''' under what circumstances it should close.</value>
        ''' <remarks>This collection should contain a set of <see cref="CloseTrigger" /> objects with their 
        ''' <see cref="AjaxPopupTrigger.ControlID" /> property set to the controls that you want to trigger 
        ''' the popup to close.  It is populated automatically by the <see cref="AjaxPopup" /> class.</remarks>
        <PersistenceMode(PersistenceMode.InnerProperty)> _
        Public Property CloseTriggers() As Generic.List(Of AjaxPopupTrigger)
            Get
                If m_CloseTriggers Is Nothing Then
                    m_CloseTriggers = New Generic.List(Of AjaxPopupTrigger)
                End If
                Return m_CloseTriggers
            End Get
            Set(ByVal value As Generic.List(Of AjaxPopupTrigger))
                m_CloseTriggers = value
            End Set
        End Property

        Private m_IsOpen As Boolean

        ''' <summary>
        ''' Gets or sets whether or not the popup window is open.
        ''' </summary>
        ''' <value><see langword="True" /> if the popup window is currently open; <see langword="False" /> 
        ''' otherwise.</value>
        ''' <remarks>This property is manipulated indirectly through the
        ''' <see cref="AjaxPopup.IsOpen" /> property.</remarks>
        Public Property IsOpen() As Boolean
            Get
                Return m_IsOpen
            End Get
            Set(ByVal value As Boolean)
                m_IsOpen = value
            End Set
        End Property

        Private m_OpenMode As AjaxPopupOpenMode

        ''' <summary>
        ''' Gets or sets the <see cref="Global.AjaxPopupOpenMode" /> of the control.
        ''' </summary>
        ''' <value><see cref="Global.AjaxPopupOpenMode.MoveToCenter" /> if the popup window will move to the center
        ''' of the screen when opened; <see cref="Global.AjaxPopupOpenMode.MoveToClick" /> if the popup window will
        ''' move to where the mouse has been clicked when opened; <see cref="Global.AjaxPopupOpenMode.None" /> if
        ''' the popup window will not move at all.</value>
        ''' <remarks>This property is manipulated indirectly through the
        ''' <see cref="AjaxPopup.OpenMode" /> property.</remarks>
        Public Property OpenMode() As AjaxPopupOpenMode
            Get
                Return m_OpenMode
            End Get
            Set(ByVal value As AjaxPopupOpenMode)
                m_OpenMode = value
            End Set
        End Property

        Private m_VeilClass As String

        Public Property VeilClass() As String
            Get
                Return m_VeilClass
            End Get
            Set(ByVal value As String)
                m_VeilClass = value
            End Set
        End Property

        Private m_VeilCloses As Boolean

        ''' <summary>
        ''' Gets or sets whether clicking on the veil closes the popup window.
        ''' </summary>
        ''' <value><see langword="True" /> if clicking on the veil will close the popup window; 
        ''' <see langword="False" /> otherwise.</value>
        ''' <remarks>This property is manipulated indirectly through the
        ''' <see cref="AjaxPopup.VeilCloses" /> property.</remarks>
        Public Property VeilCloses() As Boolean
            Get
                Return m_VeilCloses
            End Get
            Set(ByVal value As Boolean)
                m_VeilCloses = value
            End Set
        End Property

        Private m_IsDraggable As Boolean = True

        ''' <summary>
        ''' Gets or sets whether or not the popup window can be dragged around the screen.
        ''' </summary>
        ''' <value><see langword="True" /> if the popup window can be dragged around the screen; 
        ''' <see langword="False" /> otherwise.</value>
        ''' <remarks><para>This property is manipulated indirectly through the
        ''' <see cref="AjaxPopup.IsDraggable" /> property.</para>
        ''' <para>The default value of this property is <see langword="True" />.</para></remarks>
        Public Property IsDraggable() As Boolean
            Get
                Return m_IsDraggable
            End Get
            Set(ByVal value As Boolean)
                m_IsDraggable = value
            End Set
        End Property

        Private m_PostbackOnOpen As Boolean = True

        ''' <summary>
        ''' Gets or sets whether there is a postback when the popup window is opened.
        ''' </summary>
        ''' <value><see langword="True" /> if opening the popup window triggers a postback; 
        ''' <see langword="False" /> otherwise.</value>
        ''' <remarks><para>This property is manipulated indirectly through the
        ''' <see cref="AjaxPopup.PostbackOnOpen" /> property.</para>
        ''' <para>The default value of this property is <see langword="True" />.</para></remarks>
        Public Property PostbackOnOpen() As Boolean
            Get
                Return m_PostbackOnOpen
            End Get
            Set(ByVal value As Boolean)
                m_PostbackOnOpen = value
            End Set
        End Property

        Private m_PostbackOnClose As Boolean = True

        ''' <summary>
        ''' Gets or sets whether there is a postback when the popup window is closed.
        ''' </summary>
        ''' <value><see langword="True" /> if closing the popup window triggers a postback; 
        ''' <see langword="False" /> otherwise.</value>
        ''' <remarks><para>This property is manipulated indirectly through the
        ''' <see cref="AjaxPopup.PostbackOnClose" /> property.</para>
        ''' <para>The default value of this property is <see langword="True" />.</para></remarks>
        Public Property PostbackOnClose() As Boolean
            Get
                Return m_PostbackOnClose
            End Get
            Set(ByVal value As Boolean)
                m_PostbackOnClose = value
            End Set
        End Property

        Private m_zIndex As Integer

        ''' <summary>
        ''' Gets or sets the z-index CSS style of the popup window.
        ''' </summary>
        ''' <value>An <see cref="Integer" /> representing the value of the CSS z-index of the popup window.</value>
        ''' <remarks>This property is manipulated indirectly through the
        ''' <see cref="AjaxPopup.zIndex" /> property.</remarks>
        Public Property zIndex() As Integer
            Get
                Return m_zIndex
            End Get
            Set(ByVal value As Integer)
                m_zIndex = value
            End Set
        End Property

        Private m_Top As String

        ''' <summary>
        ''' Gets or sets the Top attribute of the popup window.
        ''' </summary>
        ''' <value>A <see cref="String" /> representing the distance from the top of the browser to the top
        ''' of the popup window.</value>
        ''' <remarks>This property is set via JavaScript when the popup window is opened on the basis of the 
        ''' <see cref="Global.AjaxPopupOpenMode" /> of the control.  If the control is draggable, it can change
        ''' dynamically as a result of the actions of the user.  It is then passed back through the POST form
        ''' and persisted across postbacks.</remarks>
        Public Property Top() As String
            Get
                Return m_Top
            End Get
            Set(ByVal value As String)
                m_Top = value
            End Set
        End Property

        Private m_Left As String

        ''' <summary>
        ''' Gets or sets the Left attribute of the popup window.
        ''' </summary>
        ''' <value>A <see cref="String" /> representing the distance from the left side of the browser to the
        ''' left side of the popup window.</value>
        ''' <remarks>This property is set via JavaScript when the popup window is opened on the basis of the 
        ''' <see cref="Global.AjaxPopupOpenMode" /> of the control.  If the control is draggable, it can change
        ''' dynamically as a result of the actions of the user.  It is then passed back through the POST form
        ''' and persisted across postbacks.</remarks>
        Public Property Left() As String
            Get
                Return m_Left
            End Get
            Set(ByVal value As String)
                m_Left = value
            End Set
        End Property

        Private m_Width As Unit

        ''' <summary>
        ''' Gets or sets the width of the popup window.
        ''' </summary>
        ''' <value>A <see cref="Unit" /> representing the width of the <see cref="Popup.Window" />.</value>
        ''' <remarks><para>This property is manipulated indirectly through the <see cref="AjaxPopup.Width" /> 
        ''' property.</para>
        ''' <para>See <see cref="Unit" /> for more information on how to set this field.</para></remarks>
        Public Property Width() As Unit
            Get
                Return m_Width
            End Get
            Set(ByVal value As Unit)
                m_Width = value
            End Set
        End Property

        Private m_Height As Unit

        ''' <summary>
        ''' Gets or sets the height of the popup window.
        ''' </summary>
        ''' <value>A <see cref="Unit" /> representing the height of the <see cref="Popup.Window" />.</value>
        ''' <remarks><para>This property is manipulated indirectly through the <see cref="AjaxPopup.Height" /> 
        ''' property.</para>
        ''' <para>See <see cref="Unit" /> for more information on how to set this field.</para></remarks>
        Public Property Height() As Unit
            Get
                Return m_Height
            End Get
            Set(ByVal value As Unit)
                m_Height = value
            End Set
        End Property

        Private m_ShowVeil As Boolean = True

        ''' <summary>
        ''' Gets or sets whether the veil is visible.
        ''' </summary>
        ''' <value><see langword="True" /> if the veil is visible; <see langword="False" /> otherwise.</value>
        ''' <remarks>This property is manipulated indirectly through the
        ''' <see cref="AjaxPopup.ShowVeil" /> property.</remarks>
        Public Property ShowVeil() As Boolean
            Get
                Return m_ShowVeil
            End Get
            Set(ByVal value As Boolean)
                m_ShowVeil = value
            End Set
        End Property

        Private m_LoadingPopupDelay As Integer = 1

        ''' <summary>
        ''' Gets or sets the time in seconds before the loading screen is shown.
        ''' </summary>
        ''' <value>An <see cref="Integer" /> representing the number of seconds between the popup being triggered
        ''' to open and the loading screen beginning to show.</value>
        ''' <remarks>The default value is three seconds.  This property is manipulated indirectly through the
        ''' <see cref="AjaxPopup.LoadingPopupDelay" /> property.</remarks>
        Public Property LoadingPopupDelay() As Integer
            Get
                Return m_LoadingPopupDelay
            End Get
            Set(ByVal value As Integer)
                m_LoadingPopupDelay = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the title of the popup window
        ''' </summary>
        ''' <value>A <see cref="String" /> representing the title of the popup window.</value>
        ''' <remarks>This property is manipulated indirectly through the
        ''' <see cref="AjaxPopup.Title" /> property.</remarks>
        Public Property Title() As String
            Get
                Return ViewState("Title")
            End Get
            Set(ByVal value As String)
                ViewState("Title") = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the window which pops up when the class is triggered.
        ''' </summary>
        ''' <value>A &lt;div&gt; element which contains all the controls of the popup window.</value>
        ''' <remarks>This property is used by the <see cref="AjaxPopup" /> class to obtain the ClientID of
        ''' this &lt;div&gt; element in order to manipulate it via JavaScript.</remarks>
        Public ReadOnly Property Window() As HtmlGenericControl
            Get
                Return m_WindowDiv
            End Get
        End Property

        ''' <overloads>Performs validation on the popup window.</overloads>
        ''' <summary>
        ''' Validates all of the controls inside the popup window.
        ''' </summary>
        ''' <returns><see langword="True" /> if the controls are all valid; 
        ''' <see langword="False" /> if at least one is not.</returns>
        ''' <remarks>This method is used by the <see cref="AjaxPopup" /> class to iterate through all the
        ''' validators inside the window and tell them to validate.  It also adds the error messages to the 
        ''' popup window.</remarks>
        Public Function Validate() As Boolean
            Dim validators As New Generic.List(Of Control)
            FindValidators(Me, validators)
            Dim bValid As Boolean = True
            For Each v As BaseValidator In validators
                If v.ValidationGroup = Nothing Then
                    v.Validate()
                    If Not v.IsValid Then
                        m_ErrorCtl.AddError(v.ErrorMessage)
                        bValid = False
                    End If
                End If
            Next
            Return bValid
        End Function

        ''' <summary>
        ''' Validates the controls inside the popup window of the specified <paramref name="ValidationGroup" />
        ''' </summary>
        ''' <param name="ValidationGroup">A <see cref="String" /> representing the 
        ''' <see cref="BaseValidator.ValidationGroup" /> to validate.</param>
        ''' <returns><see langword="True" /> if the controls in the group are all valid; <see langword="False" /> 
        ''' if at least one is not.</returns>
        ''' <remarks>This method is used by the <see cref="AjaxPopup" /> class to iterate through all the
        ''' validators inside the window and tell them to validate.  It also adds the error messages to the 
        ''' popup window.</remarks>
        Public Function Validate(ByVal ValidationGroup As String) As Boolean
            Dim validators As New Generic.List(Of Control)
            FindValidators(Me, validators)
            Dim bValid As Boolean = True
            For Each v As BaseValidator In validators
                If v.ValidationGroup = ValidationGroup Then
                    v.Validate()
                    If Not v.IsValid Then
                        m_ErrorCtl.AddError(v.ErrorMessage)
                        bValid = False
                    End If
                End If
            Next
            Return bValid
        End Function

        ''' <summary>
        ''' Adds an error message to the popup window
        ''' </summary>
        ''' <param name="Message">A <see cref="String" /> containing the error message to add.</param>
        ''' <remarks>The error messages for the popup are implemented with the same <see cref="ErrorMessage" />
        ''' class as the rest of the page.</remarks>
        Public Sub AddError(ByVal Message As String)
            m_ErrorCtl.AddError(Message)
        End Sub

        ' Adds all the validators in ctl to the list provided in validators.
        Private Sub FindValidators(ByVal ctl As Control, ByRef validators As Generic.List(Of Control))
            If TypeOf ctl Is BaseValidator Then
                validators.Add(ctl)
            End If
            For Each child As Control In ctl.Controls
                FindValidators(child, validators)
            Next
        End Sub

        ''' <summary>
        ''' Generates the <see cref="ScriptControlDescriptor">ScriptControlDescriptors</see> used to store the
        ''' properties on the client side that are necessary in order to make the JavaScript work.
        ''' </summary>
        ''' <returns>A single <see cref="ScriptControlDescriptor" /> which represents the 
        ''' <see cref="AjaxPopup" /> class.</returns>
        ''' <exception cref="ArgumentException">One of the specified trigger controls could not be found.</exception>
        ''' <remarks>This method allows the AjaxPopup.js file the information it needs in order to 
        ''' operate the essentials of the popup control.</remarks>
        Public Function GetScriptDescriptors() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptDescriptor) Implements System.Web.UI.IScriptControl.GetScriptDescriptors
            Dim s As New ScriptControlDescriptor("AmericanEagle.AjaxPopup", m_WindowDiv.ClientID)

            s.AddScriptProperty("ShowVeil", ShowVeil.ToString.ToLower)
            s.AddScriptProperty("VeilCloses", VeilCloses.ToString.ToLower)
            s.AddScriptProperty("IsDraggable", IsDraggable.ToString.ToLower)
            s.AddScriptProperty("IsOpen", IsOpen.ToString.ToLower)
            s.AddScriptProperty("LoadingPopupDelay", LoadingPopupDelay)
            s.AddScriptProperty("WasOpen", m_WasOpen.ToString.ToLower)
            s.AddScriptProperty("PostbackOnOpen", PostbackOnOpen.ToString.ToLower)
            s.AddScriptProperty("PostbackOnClose", PostbackOnClose.ToString.ToLower)

            s.AddProperty("PostBack", Page.ClientScript.GetPostBackEventReference(m_Postback, "##ARGS##"))

            Select Case OpenMode
                Case AjaxPopupOpenMode.MoveToCenter
                    s.AddScriptProperty("IsMoveToCenter", "true")
                Case AjaxPopupOpenMode.MoveToClick
                    s.AddScriptProperty("IsMoveToClick", "true")
            End Select

            s.AddElementProperty("Header", m_HeaderDiv.ClientID)
            s.AddElementProperty("Body", m_BodyDiv.ClientID)
            s.AddElementProperty("Loading", m_LoadingDiv.ClientID)
            s.AddElementProperty("WindowDiv", m_WindowDiv.ClientID)

            s.AddElementProperty("hdnState", ClientID)

            Dim sOpenTriggers As New StringBuilder("[")
            For Each t As AjaxPopupTrigger In OpenTriggers
                Dim temp As Control = Me.FindControl(t.ControlID)
                If temp Is Nothing And NamingContainer IsNot Nothing Then temp = FindControl(NamingContainer, t.ControlID)
                If temp Is Nothing Then temp = FindControlOutward(t.ControlID, Me)
                If temp Is Nothing Then temp = Page.FindControl(t.ControlID)
                If temp Is Nothing Then temp = FindControl(Page, t.ControlID)
                If temp Is Nothing Then
                    Throw New ArgumentException("Could not find control '" & t.ControlID & "'")
                Else
                    sOpenTriggers.Append(IIf(sOpenTriggers.Length = 1, "", ",") & Quote(temp.ClientID))
                End If
            Next
            sOpenTriggers.Append("]")

            Dim sCloseTriggers As New StringBuilder("[")
            For Each t As AjaxPopupTrigger In CloseTriggers
                Dim temp As Control = FindControl(t.ControlID)
                If temp Is Nothing And NamingContainer IsNot Nothing Then temp = FindControl(NamingContainer, t.ControlID)
                If temp Is Nothing Then temp = FindControlOutward(t.ControlID, Me)
                If temp Is Nothing Then temp = Page.FindControl(t.ControlID)
                If temp Is Nothing Then temp = FindControl(Page, t.ControlID)
                If temp Is Nothing Then
                    Throw New ArgumentException("Could not find control '" & t.ControlID & "'")
                Else
                    sCloseTriggers.Append(IIf(sCloseTriggers.Length = 1, "", ",") & Quote(temp.ClientID))
                End If
            Next
            sCloseTriggers.Append("]")

            s.AddScriptProperty("OpenTriggers", sOpenTriggers.ToString)
            s.AddScriptProperty("CloseTriggers", sCloseTriggers.ToString)

            Return New ScriptDescriptor() {s}
        End Function

        ''' <summary>
        ''' Provides the list of external JavaScript references that this control needs to operate.
        ''' </summary>
        ''' <returns>A single <see cref="ScriptReference" /> which represents the AjaxPopup.js file.</returns>
        ''' <remarks>Make sure that the path inside this method corresponds to the path in the Web project
        ''' for this particular file.</remarks>
        Public Function GetScriptReferences() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptReference) Implements System.Web.UI.IScriptControl.GetScriptReferences
            Dim out(1) As ScriptReference
            out(0) = New ScriptReference("/cms/includes/controls/AjaxPopup.js")
            Return out
        End Function

        ' As a result of this method, child controls are created no later than the Init event.
        Private Sub UpdatePanelPopup_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            EnsureChildControls()
        End Sub

        ''' <summary>
        ''' Raises the <see cref="PreRender" /> event, registers the control with the 
        ''' <see cref="ScriptManager" />, then updates certain properties of control with their most recent 
        ''' values.
        ''' </summary>
        ''' <param name="e">An <see cref="EventArgs" /> object that contains the event data.</param>
        ''' <remarks>Don't attempt to modify the physical attributes of the popup control after this part
        ''' of the page lifecycle, because it won't do anything.</remarks>
        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)

            ScriptManager.GetCurrent(Page).RegisterScriptControl(Me)

            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(m_Postback)

            If zIndex <> Nothing Then
                m_WindowDiv.Attributes.CssStyle("z-index") = zIndex
            End If

            m_ErrorCtl.UpdateVisibility()

            m_WindowDiv.Style("display") = "none"

            If Width <> Nothing Then
                m_WindowDiv.Style("width") = Width.ToString
            End If
            If Height <> Nothing Then
                m_WindowDiv.Style("height") = Height.ToString
            End If

            If Left <> String.Empty Then
                m_WindowDiv.Style("left") = Left
            End If
            If Top <> String.Empty Then
                m_WindowDiv.Style("top") = Top
            End If

            If Title <> Nothing Then
                m_HeaderDiv.Controls.AddAt(0, New LiteralControl(Title))
            Else
                m_HeaderDiv.Controls.AddAt(0, New LiteralControl("&nbsp;"))
            End If
        End Sub

        ''' <summary>
        ''' Renders the <see cref="Popup" /> control to the <paramref name="writer"/> 
        ''' object.
        ''' </summary>
        ''' <param name="writer">The <see cref="HtmlTextWriter" /> that receives the rendered output.</param>
        ''' <remarks>The <see cref="Popup" /> control renders itself as well as a hidden field containing
        ''' the <see cref="Left" />, <see cref="Top" />, and <see cref="IsOpen" />properties of the control.</remarks>
        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            MyBase.Render(writer)

            writer.AddAttribute("id", ClientID)
            writer.AddAttribute("name", UniqueID)
            writer.AddAttribute("type", "hidden")
            writer.AddAttribute("value", Left & "|" & Top & "|" & IsOpen.ToString.ToLower)
            writer.RenderBeginTag("input")
            writer.RenderEndTag()

            ScriptManager.GetCurrent(Page).RegisterScriptDescriptors(Me)
        End Sub

        ''' <summary>
        ''' Surrounds <paramref name="str" /> with single quotes and escapes all single quotes that are inside
        ''' with \'s.
        ''' </summary>
        ''' <param name="str">The <see cref="String" /> to escape.</param>
        ''' <returns>A String that has been escaped in accordance with JavaScript string standards.</returns>
        ''' <remarks>This method should be used to escape strings that are part of JavaScript code.  Use
        ''' <see cref="Database.Quote" /> for escaping SQL code.</remarks>
        Private Function Quote(ByVal str As String) As String
            Return "'" & str.Replace("'", "\'") & "'"
        End Function

        ''' <summary>
        ''' Recursively searches the specified <paramref name="parent" /> control for the control with the specified
        ''' <paramref name="id" />
        ''' </summary>
        ''' <param name="parent">A <see cref="Control" /> to recursively search through.</param>
        ''' <param name="id">A <see cref="String" /> representing the <see cref="Control.ID" /> of the control
        ''' being searched for.</param>
        ''' <returns><see langword="Nothing" /> if the control is not found; A <see cref="Control" /> with the 
        ''' specified <paramref name="id" /> otherwise.</returns>
        ''' <remarks>This method calls <see cref="EnsureChildControls" /> in order to make sure that the control being
        ''' searched for exists.  If the control you have specified is not created within that method, and has
        ''' not already been created by the point in the page lifecycle that this method is called,
        ''' this method may fail unexpectedly.</remarks>
        Protected Friend Overloads Function FindControl(ByVal parent As Control, ByVal id As String) As Control
            EnsureChildControls()
            If parent.ID = id Then
                Return parent
            Else
                For Each child As Control In parent.Controls
                    Dim temp As Control = FindControl(child, id)
                    If temp IsNot Nothing Then Return temp
                Next
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Recursively searches outward from the specified <paramref name="Container" /> until it either finds
        ''' a control with the specified <paramref name="TargetID" />, or it reaches the root element.
        ''' </summary>
        ''' <param name="TargetID">A <see cref="String" /> representing the <see cref="Control.ID" /> being 
        ''' searched for.</param>
        ''' <param name="Container">The <see cref="Control" /> at which point the search is to begin.</param>
        ''' <returns><see langword="Nothing" /> if the control is not found; A <see cref="Control" /> with the 
        ''' specified <paramref name="TargetID" /> otherwise.</returns>
        ''' <remarks>This method will, eventually, search the entire page if it doesn't find what it's
        ''' looking for sooner.</remarks>
        Protected Function FindControlOutward(ByVal TargetID As String, ByVal Container As Control) As Control
            If Container Is Nothing Then Return Nothing
            If Container.ID = TargetID Then
                Return Container
            Else
                Dim temp As Control = Container.FindControl(TargetID)
                If temp IsNot Nothing Then
                    Return temp
                Else
                    Return FindControlOutward(TargetID, Container.NamingContainer)
                End If
            End If
        End Function

        ''' <summary>
        ''' Parses the hidden field created by <see cref="Render" /> in order to maintain certain aspects of
        ''' the state of the control.
        ''' </summary>
        ''' <param name="postDataKey">The name of the element in the <paramref name="postCollection" />
        ''' which is being loaded.</param>
        ''' <param name="postCollection">The collection of name-value pairs representing the post data
        ''' of an <see cref="IPostBackDataHandler" />.</param>
        ''' <returns><see langword="False" />.</returns>
        ''' <remarks>This method assumes that the value of the hidden field it is trying to parse will be
        ''' found in the first element of the comma-separated list of values found in the
        ''' <paramref name="postCollection" /> under the name specified by <paramref name="postDataKey" />.</remarks>
        Public Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As System.Collections.Specialized.NameValueCollection) As Boolean Implements System.Web.UI.IPostBackDataHandler.LoadPostData
            If postCollection(postDataKey) <> Nothing Then
                Dim sState As String
                If postCollection(postDataKey).Contains(",") Then
                    sState = postCollection(postDataKey).Split(",")(0)
                Else
                    sState = postCollection(postDataKey)
                End If
                Dim state As String() = sState.Split("|")
                Left = state(0)
                Top = state(1)
                IsOpen = Boolean.Parse(state(2))
                m_WasOpen = IsOpen
            End If
        End Function

        ''' <summary>
        ''' This method exists in order to implement the <see cref="IPostBackDataHandler" /> interface.  However,
        ''' it does nothing.
        ''' </summary>
        Public Sub RaisePostDataChangedEvent() Implements System.Web.UI.IPostBackDataHandler.RaisePostDataChangedEvent
        End Sub

        Private bChildControlsCreated As Boolean = False

        ''' <summary>
        ''' This method ensures that the child controls which make up the various pieces of the 
        ''' <see cref="Popup" /> control are created.
        ''' </summary>
        ''' <remarks>This method is called as a result of the <see cref="Init" /> event.  However, for 
        ''' whatever reason, it does not call <see cref="CreateChildControls" />.  CreateChildControls has, 
        ''' in fact, not been overridden and its functionality has been made part of EnsureChildControls.  Thus
        ''' it is important that you do not at any time call CreateChildControls on this object.</remarks>
        Protected Overrides Sub EnsureChildControls()
            MyBase.EnsureChildControls()

            If bChildControlsCreated Then Exit Sub

            bChildControlsCreated = True

            m_Postback = New UnvalidatedPostback2
            Controls.Add(m_Postback)
            m_Postback.ID = "postback"
            AddHandler m_Postback.Command, AddressOf postback_Command

            m_LoadingDiv = New HtmlGenericControl("div")
            m_LoadingDiv.ID = "divLoading"
            If LoadingTemplate IsNot Nothing Then
                LoadingTemplate.InstantiateIn(m_LoadingDiv)
            Else
                m_LoadingDiv.Controls.Add(New LiteralControl("<b>Loading...</b><br/><img src=""/cms/images/ajaxloading1.gif"" alt=""Loading..."" />"))
                m_LoadingDiv.Style("background-color") = "#ffe7a2"
                m_LoadingDiv.Style("background-image") = "/cms/images/admin/info_bg.gif"
                m_LoadingDiv.Style("background-repeat") = "repeat-x"
                m_LoadingDiv.Style("text-align") = "center"
                m_LoadingDiv.Style("padding") = "50px"
            End If
            m_LoadingDiv.Style("z-index") = zIndex + 1
            m_LoadingDiv.Style("display") = "none"
            m_LoadingDiv.Style("position") = "fixed"
            Controls.Add(m_LoadingDiv)

            m_WindowDiv = New HtmlGenericControl("div")
            m_WindowDiv.ID = "divWindow"
            m_WindowDiv.Style("background-color") = "transparent"
            Controls.Add(m_WindowDiv)

            Dim wrapper As New HtmlGenericControl("div")
            wrapper.Attributes("class") = "popupcontents"
            m_WindowDiv.Controls.Add(wrapper)

            m_HeaderDiv = New HtmlGenericControl("div")
            m_HeaderDiv.ID = "divHeader"
            If HeaderTemplate IsNot Nothing Then
                HeaderTemplate.InstantiateIn(m_HeaderDiv)
            Else
                m_HeaderDiv.Attributes("class") = "PopupTitle PopupTitleBorder"

                Dim divClose As New HtmlGenericControl("div")
                divClose.Attributes("class") = "closeButton"
                divClose.ID = "divClose"
                m_HeaderDiv.Controls.Add(divClose)

                'Dim a As New HtmlAnchor
                'a.ID = "lnkClose"
                'a.Attributes("class") = "closeButton"
                'a.InnerHtml = "close"
                'm_HeaderDiv.Controls.Add(a)

                Dim trigger As New CloseTrigger
                trigger.ControlID = divClose.ID
                CloseTriggers.Add(trigger)
            End If
            wrapper.Controls.Add(m_HeaderDiv)

            m_ErrorCtl = New ErrorMessage
            m_ErrorCtl.ID = "ctlErrors"
            wrapper.Controls.Add(m_ErrorCtl)

            m_BodyDiv = New HtmlGenericControl("div")
            m_BodyDiv.ID = "divBody"
            If BodyTemplate IsNot Nothing Then
                BodyTemplate.InstantiateIn(m_BodyDiv)
            End If
            wrapper.Controls.Add(m_BodyDiv)

            'shadow divs
            Dim tl, tc, tr, ml, mr, bl, bc, br As HtmlGenericControl

            tl = New HtmlGenericControl("div")
            tl.Attributes("class") = "tl popupborder"
            wrapper.Controls.Add(tl)

            tc = New HtmlGenericControl("div")
            tc.Attributes("class") = "tc popupborder"
            wrapper.Controls.Add(tc)

            tr = New HtmlGenericControl("div")
            tr.Attributes("class") = "tr popupborder"
            wrapper.Controls.Add(tr)

            ml = New HtmlGenericControl("div")
            ml.Attributes("class") = "ml popupborder"
            wrapper.Controls.Add(ml)

            mr = New HtmlGenericControl("div")
            mr.Attributes("class") = "mr popupborder"
            wrapper.Controls.Add(mr)

            bl = New HtmlGenericControl("div")
            bl.Attributes("class") = "bl popupborder"
            wrapper.Controls.Add(bl)

            bc = New HtmlGenericControl("div")
            bc.Attributes("class") = "bc popupborder"
            wrapper.Controls.Add(bc)

            br = New HtmlGenericControl("div")
            br.Attributes("class") = "br popupborder"
            wrapper.Controls.Add(br)
        End Sub

        ' This method raises the Close and Open events on the basis of e.CommandArgument
        Private Sub postback_Command(ByVal sender As Object, ByVal e As CommandEventArgs)
            Dim arg As String = e.CommandArgument
            If arg.Length > 0 Then
                Dim args As String() = arg.Split("|")
                Dim ctl As Control = Page.FindControl(args(1))
                Select Case args(0)
                    Case "Close"
                        IsOpen = False
                        RaiseEvent Close(ctl, EventArgs.Empty)
                    Case "Open"
                        IsOpen = True
                        RaiseEvent Open(ctl, EventArgs.Empty)
                End Select
            Else
                Select Case e.CommandArgument
                    Case "Close"
                        IsOpen = False
                        RaiseEvent Close(Me, EventArgs.Empty)
                    Case Else
                        IsOpen = True
                        RaiseEvent Open(Me, EventArgs.Empty)
                End Select
            End If
        End Sub
    End Class

    ''' <summary>
    ''' Provides the base class for the derived types <see cref="OpenTrigger" /> and <see cref="CloseTrigger" />.
    ''' </summary>
    ''' <remarks>This class is nothing more than a wrapper for the <see cref="AjaxPopupTrigger.ControlID" />
    ''' which determines what the trigger is that causes its derived classes to activate.
    ''' <seealso cref="AjaxPopup" />
    ''' <seealso cref="Popup" />
    ''' <seealso cref="OpenTrigger" />
    ''' <seealso cref="CloseTrigger" /></remarks>
    Public MustInherit Class AjaxPopupTrigger
        ''' <summary>
        ''' The <see cref="Control.ID" /> of the control which activates the trigger
        ''' </summary>
        Public ControlID As String
    End Class

    ''' <summary>
    ''' Specifies that this <see cref="AjaxPopupTrigger" /> opens the <see cref="AjaxPopup" /> control.
    ''' </summary>
    ''' <remarks>This is a marker class.  Only its type separates it from <see cref="CloseTrigger" />, which is
    ''' how the two are told apart by the control.
    ''' <seealso cref="AjaxPopup" />
    ''' <seealso cref="Popup" />
    ''' <seealso cref="AjaxPopupTrigger" />
    ''' <seealso cref="CloseTrigger" /></remarks>
    Public Class OpenTrigger
        Inherits AjaxPopupTrigger
    End Class

    ''' <summary>
    ''' Specifies that this <see cref="AjaxPopupTrigger" /> closes the <see cref="AjaxPopup" /> control.
    ''' </summary>
    ''' <remarks>This is a marker class.  Only its type separates it from <see cref="OpenTrigger" />, which is
    ''' how the two are told apart by the control.
    ''' <seealso cref="AjaxPopup" />
    ''' <seealso cref="Popup" />
    ''' <seealso cref="AjaxPopupTrigger" />
    ''' <seealso cref="OpenTrigger" /></remarks>
    Public Class CloseTrigger
        Inherits AjaxPopupTrigger
    End Class

    ''' <summary>
    ''' Creates the events which are transformed into <see cref="AjaxPopup.Open" /> and 
    ''' <see cref="AjaxPopup.Close" /> by the classes which utilize this class.
    ''' </summary>
    ''' <remarks>This class serves as the <see cref="IPostBackEventHandler" /> for the <see cref="AjaxPopup" />
    ''' control.
    ''' <seealso cref="Popup" /></remarks>
    Public Class UnvalidatedPostback2
        Inherits Control
        Implements IPostBackEventHandler

        ''' <summary>
        ''' Occurs when the page lifecycle triggers its 
        ''' <see cref="IPostBackEventHandler">IPostBackEventHandlers</see>.
        ''' </summary>
        ''' <remarks>This event is handled by the <see cref="Popup" /> class and transformed into opens and 
        ''' closes.</remarks>
        Public Event Command As CommandEventHandler

        ''' <summary>
        ''' Renders the control as HTML to the <paramref name="writer" />.
        ''' </summary>
        ''' <param name="writer">The <see cref="HtmlTextWriter" /> that receives the rendered output.</param>
        ''' <remarks>The HTML rendered to the screen more or less serves as a marker for the 
        ''' <see cref="IPostBackEventHandler" /> that this class represents.</remarks>
        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            writer.AddAttribute("name", UniqueID)
            writer.AddAttribute("id", ClientID)
            writer.AddAttribute("type", "button")
            writer.AddAttribute("value", "postback")
            writer.AddStyleAttribute("display", "none")
            writer.RenderBeginTag("input")
            writer.RenderEndTag()
        End Sub

        ''' <summary>
        ''' Called as part of the page lifecycle in order to trigger the <see cref="Command" /> event.
        ''' </summary>
        ''' <param name="eventArgument">A <see cref="String" /> representing the argument to be used as the
        ''' <see cref="CommandEventArgs.CommandArgument" /> of the <see cref="CommandEventArgs" /> used
        ''' by the <see cref="Command" /> event. </param>
        Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent
            If eventArgument = String.Empty Then
                eventArgument = System.Web.HttpContext.Current.Request("__EVENTARGUMENT")
            End If
            RaiseEvent Command(Me, New CommandEventArgs(String.Empty, eventArgument))
        End Sub
    End Class
End Namespace

''' <summary>
''' Represents the methodology by which the location of the <see cref="Controls.AjaxPopup" /> is determined upon 
''' opening.
''' <seealso cref="Controls.AjaxPopup" />
''' <seealso cref="Controls.Popup" />
''' </summary>
Public Enum AjaxPopupOpenMode
    ''' <summary>
    ''' Represents doing nothing to the location of the <see cref="Controls.AjaxPopup" /> upon opening.
    ''' </summary>
    None

    ''' <summary>
    ''' Represents moving the <see cref="Controls.AjaxPopup" /> to the center of the screen upon opening.
    ''' </summary>
    MoveToCenter

    ''' <summary>
    ''' Represents moving the <see cref="Controls.AjaxPopup" /> to where the user has clicked upon opening.
    ''' </summary>
    MoveToClick
End Enum

