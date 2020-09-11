Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls

Namespace Controls
    ''' <summary>
    ''' Provides an automatic suggestion feature for a text field by querying a web service
    ''' </summary>
    ''' <remarks><para>By setting the <see cref="AutoSuggest.ServicePath" />, 
    ''' <see cref="AutoSuggest.ServiceMethod" />, and <see cref="AutoSuggest.NumResults" /> methods, you 
    ''' can control the query which generates the list of suggestions.</para>
    ''' <para>The method signature of the method to be used by this control is 
    ''' <c>Public Function FunctionName(ByVal Text as String, ByVal NumResults as Integer) As ArrayList</c>.
    ''' </para>
    ''' <seealso cref="AutoSuggestResult" /></remarks>
    Public Class AutoSuggest
        Inherits CompositeControl
        Implements IScriptControl
        Implements IPostBackEventHandler

        Private m_divSuggest As HtmlGenericControl

        ''' <summary>
        ''' Contains the controls that pop up when you begin to type.
        ''' </summary>
        ''' <value>The <see cref="HtmlGenericControl" /> which represents a &lt;div&gt; element.</value>
        ''' <remarks>This element is used by the derived class <see cref="AssetAutoSuggest" />.</remarks>
        Protected Property divSuggest() As HtmlGenericControl
            Get
                Return m_divSuggest
            End Get
            Set(ByVal value As HtmlGenericControl)
                m_divSuggest = value
            End Set
        End Property

        Private m_txtDisplay As TextBox

        ''' <summary>
        ''' Contains the final result of the autosuggest popup.
        ''' </summary>
        ''' <value>The <see cref="TextBox" /> which is used to display the final result of the control.</value>
        ''' <remarks>This field is used by the derived class <see cref="AssetAutoSuggest" />.</remarks>
        Protected Property txtDisplay() As TextBox
            Get
                Return m_txtDisplay
            End Get
            Set(ByVal value As TextBox)
                m_txtDisplay = value
            End Set
        End Property


        Private m_txtSuggest As TextBox

        ''' <summary>
        ''' Contains what the user types in order to generate automated suggestions.
        ''' </summary>
        ''' <value>The <see cref="TextBox" /> used to type in so that the control will generate automated 
        ''' suggestions.</value>
        ''' <remarks>This field is used by the derived class <see cref="AssetAutoSuggest" />.</remarks>
        Protected Property txtSuggest() As TextBox
            Get
                Return m_txtSuggest
            End Get
            Set(ByVal value As TextBox)
                m_txtSuggest = value
            End Set
        End Property

        Private m_pnlResults As Panel

        ''' <summary>
        ''' Contains the list of automated suggestions.
        ''' </summary>
        ''' <value>The <see cref="Panel" /> which displays the values of the list of 
        ''' <see cref="AutoSuggestResult">AutoSuggestResults</see>.</value>
        ''' <remarks>This control is used by the derived class <see cref="AssetAutoSuggest" />.</remarks>
        Protected Property pnlResults() As Panel
            Get
                Return m_pnlResults
            End Get
            Set(ByVal value As Panel)
                m_pnlResults = value
            End Set
        End Property

        Private m_lnkClose As HtmlAnchor

        ''' <summary>
        ''' Closes the autosuggest popup.
        ''' </summary>
        ''' <value>The <see cref="HtmlAnchor" /> which represents an &lt;a&gt; element.</value>
        ''' <remarks>This element is used by the derived class <see cref="AssetAutoSuggest" />.</remarks>
        Protected Property lnkClose() As HtmlAnchor
            Get
                Return m_lnkClose
            End Get
            Set(ByVal value As HtmlAnchor)
                m_lnkClose = value
            End Set
        End Property

        Private m_hdnValue As HiddenField

        ''' <summary>
        ''' Contains the current value of the autosuggest control.
        ''' </summary>
        ''' <value>A <see cref="HiddenField" /> which is used to store the current value of the control.</value>
        ''' <remarks>This element is used by the derived class <see cref="AssetAutoSuggest" />.</remarks>
        Protected Property hdnValue() As HiddenField
            Get
                Return m_hdnValue
            End Get
            Set(ByVal value As HiddenField)
                m_hdnValue = value
            End Set
        End Property

        ''' <summary>
        ''' Occurs when the popup list of suggestions is closed, finalizing the current selection.
        ''' </summary>
        ''' <remarks>This event can be used to create custom functionality when a selection has been made
        ''' using this control.</remarks>
        Public Event ResultSelected As EventHandler

        ''' <summary>
        ''' Gets or sets the value field of the control.
        ''' </summary>
        ''' <value>A <see cref="String" /> representing the current value of the control.</value>
        ''' <remarks>Setting this field does not set the <see cref="Text" /> field.  However, setting the
        ''' <see cref="Text" /> field resets this field to <see langword="Nothing" />.</remarks>
        Public Property SelectedValue() As String
            Get
                EnsureChildControls()
                Return m_hdnValue.Value
            End Get
            Set(ByVal value As String)
                EnsureChildControls()
                m_hdnValue.Value = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the text value of the control.
        ''' </summary>
        ''' <value>A <see cref="String" /> which is displayed in the textboxes both inside and outside of the
        ''' popup panel.</value>
        ''' <remarks>Setting this field resets the <see cref="SelectedValue" /> field to 
        ''' <see langword="Nothing" />.  However, setting the <see cref="SelectedValue" /> field does not
        ''' affect this field.</remarks>
        Public Property Text() As String
            Get
                EnsureChildControls()
                Return m_txtSuggest.Text
            End Get
            Set(ByVal value As String)
                EnsureChildControls()
                m_txtDisplay.Text = value
                m_txtSuggest.Text = value
                m_hdnValue.Value = Nothing
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the number of results to be displayed at a time as part of the autosuggest query.
        ''' </summary>
        ''' <value>An <see cref="Integer" /> which is used by the autosuggest query to determine the number
        ''' of results to return.</value>
        ''' <remarks>This is the second parameter of the method signature of the service method being called by
        ''' this control.</remarks>
        Public Property NumResults() As Integer
            Get
                Return ViewState("NumResults")
            End Get
            Set(ByVal value As Integer)
                ViewState("NumResults") = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the URL of the web service to be used by this control
        ''' </summary>
        ''' <value>An absolute or relative path to a .asmx file.</value>
        ''' <remarks>The .asmx file being used by the Web project is located at /ajax.asmx.  
        ''' The  code-behind is located at /App_Code/ajax.vb.  To add a method to the 
        ''' service, modify ajax.vb.  The appropriate method signature is 
        ''' <c>Public Function FunctionName(ByVal Text as String, ByVal NumResults as Integer) As ArrayList</c>.
        ''' </remarks>
        Public Property ServicePath() As String
            Get
                Return ViewState("ServicePath")
            End Get
            Set(ByVal value As String)
                ViewState("ServicePath") = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the name of the method to be called on the service specified by 
        ''' <see cref="ServicePath" />.
        ''' </summary>
        ''' <value>The name of a method with the signature 
        ''' <c>Public Function FunctionName(ByVal Text as String, ByVal NumResults as Integer) As ArrayList</c>
        ''' which is found in the web service specified by the <see cref="ServicePath" /> property.</value>
        ''' <remarks>Make sure there is a method in the specified web service with the name specified by this
        ''' property.  See <see cref="ServicePath" /> for more information.</remarks>
        Public Property ServiceMethod() As String
            Get
                Return ViewState("ServiceMethod")
            End Get
            Set(ByVal value As String)
                ViewState("ServiceMethod") = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the CSS class of the popup window in which the autosuggest results are displayed.
        ''' </summary>
        ''' <value>A <see cref="String" /> which references a CSS class that is accessible from the page
        ''' this control is placed on.</value>
        ''' <remarks>This property is applied to the &lt;div&gt; element in which the autosuggest UI is located.</remarks>
        Public Property PopupClass() As String
            Get
                Return ViewState("PopupClass")
            End Get
            Set(ByVal value As String)
                ViewState("PopupClass") = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets whether the control automatically triggers a postback when a result is selected.
        ''' </summary>
        ''' <value><see langword="True" /> if selecting a result triggers a postback; <see langword="False" /> 
        ''' otherwise.</value>
        ''' <remarks>This property is used by <see cref="GetScriptDescriptors" /> in order to transfer the 
        ''' value of this property into the JavaScript code which effects the postback functionality.</remarks>
        Public Property AutoPostBack() As Boolean
            Get
                Return ViewState("AutoPostBack")
            End Get
            Set(ByVal value As Boolean)
                ViewState("AutoPostBack") = value
            End Set
        End Property

        ''' <summary>
        ''' Generates the <see cref="ScriptControlDescriptor">ScriptControlDescriptors</see> used to store the
        ''' properties on the client side that are necessary in order to make the JavaScript work.
        ''' </summary>
        ''' <returns>A single <see cref="ScriptControlDescriptor" /> which represents the 
        ''' <see cref="AutoSuggest" /> class.</returns>
        ''' <remarks>This method allows the AutoSuggest.js file the information it needs in order to 
        ''' operate the essentials of the autosuggest control.</remarks>
        Public Overridable Function GetScriptDescriptors() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptDescriptor) Implements System.Web.UI.IScriptControl.GetScriptDescriptors
            Dim s As New ScriptControlDescriptor("AmericanEagle.AutoSuggest", ClientID)

            s.AddElementProperty("divSuggest", m_divSuggest.ClientID)
            s.AddElementProperty("txtDisplay", m_txtDisplay.ClientID)
            s.AddElementProperty("txtSuggest", m_txtSuggest.ClientID)
            s.AddElementProperty("pnlResults", m_pnlResults.ClientID)
            s.AddElementProperty("lnkClose", m_lnkClose.ClientID)
            s.AddElementProperty("hdnValue", m_hdnValue.ClientID)

            'If TypeOf Page Is Components.BasePage Then
            '    s.AddProperty("SiteId", CType(Page, Components.BasePage).1)
            'End If
            s.AddProperty("ServicePath", ServicePath)
            s.AddProperty("ServiceMethod", ServiceMethod)

            If AutoPostBack Then
                s.AddProperty("PostBack", Page.ClientScript.GetPostBackEventReference(Me, Nothing))
            End If

            s.AddScriptProperty("NumResults", NumResults)

            Return New ScriptDescriptor() {s}
        End Function

        ''' <summary>
        ''' Provides the list of external JavaScript references that this control needs to operate.
        ''' </summary>
        ''' <returns>A single <see cref="ScriptReference" /> which represents the AutoSuggest.js file.</returns>
        ''' <remarks>Make sure that the path inside this method corresponds to the path in the Web project
        ''' for this particular file.</remarks>
        Public Function GetScriptReferences() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptReference) Implements System.Web.UI.IScriptControl.GetScriptReferences
            Dim s As New ScriptReference("/cms/includes/controls/AutoSuggest.js")
            Return New ScriptReference() {s}
        End Function

        ''' <summary>
        ''' Constructs the HTML of the control based on the various properties which control the appearance 
        ''' of the control.
        ''' </summary>
        ''' <remarks>This method is guaranteed to be called no later than the first time the <see cref="Text" />
        ''' or <see cref="SelectedValue" /> properties are accessed.</remarks>
        Protected Overrides Sub CreateChildControls()
            m_hdnValue = New HiddenField
            m_hdnValue.ID = "hdnValue"
            Controls.Add(m_hdnValue)

            Dim divWrapper As New HtmlGenericControl("div")
            divWrapper.Style("z-index") = "1000"
            divWrapper.Style("position") = "relative"
            Controls.Add(divWrapper)

            m_divSuggest = New HtmlGenericControl("div")
            m_divSuggest.ID = "divSuggest"
            m_divSuggest.Attributes("class") = PopupClass
            m_divSuggest.Style("position") = "absolute"
            m_divSuggest.Style("top") = "0px"
            m_divSuggest.Style("left") = "0px"
            m_divSuggest.Style("display") = "none"

            divWrapper.Controls.Add(m_divSuggest)

            m_txtSuggest = New TextBox
            m_txtSuggest.ID = "txtSuggest"
            m_txtSuggest.Style("width") = Width.ToString
            m_txtSuggest.AutoCompleteType = AutoCompleteType.None

            Dim tbl As New HtmlTable
            m_divSuggest.Controls.Add(tbl)

            Dim tr As New HtmlTableRow
            tbl.Rows.Add(tr)

            Dim td As New HtmlTableCell
            tr.Controls.Add(td)

            td.Controls.Add(m_txtSuggest)

            td = New HtmlTableCell
            tr.Controls.Add(td)

            m_lnkClose = New HtmlAnchor
            m_lnkClose.ID = "lnkClose"
            m_lnkClose.InnerHtml = "close"
            m_lnkClose.Style("cursor") = "pointer"
            td.Controls.Add(m_lnkClose)

            tr = New HtmlTableRow
            tbl.Rows.Add(tr)
            td = New HtmlTableCell
            td.ColSpan = 2
            tr.Cells.Add(td)

            m_pnlResults = New Panel
            m_pnlResults.ID = "pnlResults"
            m_pnlResults.ScrollBars = ScrollBars.Vertical
            td.Controls.Add(m_pnlResults)

            m_txtDisplay = New TextBox
            m_txtDisplay.ID = "txtDisplay"
            m_txtDisplay.Style("width") = Width.ToString
            m_txtDisplay.AutoCompleteType = AutoCompleteType.None
            Controls.Add(m_txtDisplay)
        End Sub

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
        ''' Renders the <see cref="AutoSuggest" /> control to the <paramref name="writer"/> 
        ''' object.
        ''' </summary>
        ''' <param name="writer">The <see cref="HtmlTextWriter" /> that receives the rendered output.</param>
        ''' <remarks>The <see cref="AutoSuggest" /> control renders itself via the functionality of the base
        ''' class, and then registers the script descriptors as understood by the <see cref="IScriptControl" />
        ''' interface.</remarks>
        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            MyBase.Render(writer)

            ScriptManager.GetCurrent(Page).RegisterScriptDescriptors(Me)
        End Sub

        ''' <summary>
        ''' Called as part of the page lifecycle in order to trigger the <see cref="ResultSelected" /> event.
        ''' </summary>
        ''' <param name="eventArgument">This parameter is not used by the event that is raised using this
        ''' method.</param>
        Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent
            RaiseEvent ResultSelected(Me, EventArgs.Empty)
        End Sub
    End Class

    ''' <summary>
    ''' Represents a result row in the list of automated suggestions.
    ''' </summary>
    ''' <remarks>Objects of this class are created and returned by the web service call and parsed by JavaScript
    ''' in order to provide the functionality of a particular automated suggestion.</remarks>
    <Serializable()> _
    Public Class AutoSuggestResult
        ''' <summary>
        ''' The HTML to be used when representing this particular result in the popup panel.
        ''' </summary>
        Public HTML As String

        ''' <summary>
        ''' The name of the &lt;div&gt; element which contains this particular result.
        ''' </summary>
        Public Name As String

        ''' <summary>
        ''' The value represented by this result which becomes the <see cref="AutoSuggest.SelectedValue" />
        ''' of the containing <see cref="AutoSuggest" /> object when this result is selected by the user.
        ''' </summary>
        Public Value As String
    End Class
End Namespace