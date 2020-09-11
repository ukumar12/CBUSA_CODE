Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls

Namespace Controls
    ''' <summary>
    ''' Provides the panel which contains the search functionality as well as the "Add New" link.
    ''' </summary>
    ''' <remarks><see cref="AdminSearchTable.AddNewText" /> and <see cref="AdminSearchTable.AddNewUrl" /> 
    ''' expose the properties of <see cref="AdminSearchTable.lnkAddNew"/> which are necessary to specify in 
    ''' order to customize this control.  The controls which are to be placed inside the search panel should 
    ''' be added to the <see cref="AdminSearchTable.Controls" /> property.</remarks>
    Public Class AdminSearchTable
        Inherits Control

        Private m_Panel As Panel
        Private m_HiddenField As HiddenField

        ''' <summary>
        ''' Gets or sets the text of the "Add New" link.
        ''' </summary>
        ''' <value>A <see cref="String" /> representing the text to set <see cref="lnkAddNew" />'s 
        ''' <see cref="HyperLink.Text" /> property to.</value>
        ''' <remarks>This property directly exposes a property of <see cref="lnkAddNew" />.</remarks>
        Public Property AddNewText() As String
            Get
                Return lnkAddNew.Text
            End Get
            Set(ByVal value As String)
                lnkAddNew.Text = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the URL of the "Add New" link.
        ''' </summary>
        ''' <value>A <see cref="String" /> representing the text to set <see cref="lnkAddNew" />'s 
        ''' <see cref="HyperLink.NavigateUrl" /> property to.</value>
        ''' <remarks>This property directly exposes a property of <see cref="lnkAddNew" />.</remarks>
        Public Property AddNewUrl() As String
            Get
                Return lnkAddNew.NavigateUrl
            End Get
            Set(ByVal value As String)
                lnkAddNew.NavigateUrl = value
            End Set
        End Property

        Private m_lnkAddNew As HyperLink

        ''' <summary>
        ''' Gets or sets the <see cref="HyperLink" /> that is to be clicked when the user wishes to add a new
        ''' record.
        ''' </summary>
        ''' <value>A <see cref="HyperLink" /> which is to be used as the link that is pressed when
        ''' the user wishes to add a new record on this particular page.</value>
        ''' <remarks>Unless a specific property needs to be set on this HyperLink, it is probably not necessary
        ''' to use this property at all.  It should suffice to set the <see cref="AddNewText" /> 
        ''' and <see cref="AddNewUrl" /> properties.</remarks>
        Public Property lnkAddNew() As HyperLink
            Get
                If m_lnkAddNew Is Nothing Then
                    m_lnkAddNew = New HyperLink
                End If
                Return m_lnkAddNew
            End Get
            Set(ByVal value As HyperLink)
                m_lnkAddNew = value
            End Set
        End Property

        Private m_DefaultButton As String
        ''' <summary>
        ''' Gets or sets the <see cref="Control.ID" /> of the default button which is contained in the search panel.
        ''' </summary>
        ''' <value>A <see cref="String" /> representing the <see cref="Control.ID" /> of a button inside this control.</value>
        ''' <remarks>This property, if it is used, should be set before the <see cref="Init" /> event is 
        ''' fired.</remarks>
        Public Property DefaultButton() As String
            Get
                Return m_DefaultButton
            End Get
            Set(ByVal value As String)
                m_DefaultButton = value
            End Set
        End Property

        Private m_Width As Unit
        ''' <summary>
        ''' Gets or sets the width of the search panel.
        ''' </summary>
        ''' <value>A <see cref="Unit" /> representing the width of the popup panel that appears when the
        ''' Search link is pressed.</value>
        ''' <remarks>See <see cref="Unit" /> for more information on how to set this field.</remarks>
        Public Property Width() As Unit
            Get
                Return m_Width
            End Get
            Set(ByVal value As Unit)
                m_Width = value
            End Set
        End Property

        ' This method sets up lnkAddNew, m_Panel, and m_HiddenField with the appropriate values and then
        ' makes sure that the appropriate JavaScript functions are registered in order to allow the Panel to
        ' open and close appropriately.
        Private Sub AdminSearchPanel_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            lnkAddNew.ID = "lnkAddNew"
            Controls.Add(lnkAddNew)

            m_Panel = New Panel
            m_Panel.DefaultButton = DefaultButton

            m_HiddenField = New HiddenField
            m_HiddenField.ID = "hdn"
            Controls.Add(m_HiddenField)
            Dim s As String
            If Not Page.ClientScript.IsClientScriptBlockRegistered("ToggleSearch") Then
                s = _
                    "function toggleSearch(id) {" & _
                    "   var obj = $('#'+ id); " & _
                    "   obj.slideToggle(75,function() { " & _
                    "       $('#" & m_HiddenField.ClientID & "').val(obj.is("":hidden""));" & _
                    "       createCookie('SearchTableState',obj.is("":hidden"") ? 'Y' : null);" & _
                    "   })" & _
                    "}"

                Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "ToggleSearch", s, True)
            End If

            s = _
                "function onload_" & UniqueID & "() {" & _
                "   var c = readCookie('SearchTableState');" & _
                "   var obj = $('#" & ClientID & "_AdminSearchContainer');" & _
                "   if(c && c == 'Y') {" & _
                "       obj.hide();" & _
                "   } else {" & _
                "       obj.show();" & _
                "   }" & _
                "   $('#" & m_HiddenField.ClientID & "').val(obj.is(""hidden""));" & _
                "}" & _
                "Sys.Application.add_load(onload_" & UniqueID & ");"
            Page.ClientScript.RegisterStartupScript(Me.GetType, "InitSearch", s, True)
        End Sub

        ''' <summary>
        ''' Renders the <see cref="AdminSearchTable" /> control to the <paramref name="writer"/> 
        ''' object.
        ''' </summary>
        ''' <param name="writer">The <see cref="HtmlTextWriter" /> that receives the rendered output.</param>
        ''' <remarks>The Render method for this control combines the various pieces of the control into the 
        ''' standard HTML that AmericanEagle uses to render the pane which manages searching and adding new
        ''' records.</remarks>
        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            m_HiddenField.RenderControl(writer)
            writer.Write("<div style=""position:relative; height:16px;"" class=""darkgrey""><ul id=""pagenav""><li><a href=""javascript:void(0);"" onclick=""toggleSearch('" & ClientID & "_AdminSearchContainer');"">Search <img src=""/cms/images/admin/pop_arrow.gif"" align=""absmiddle"" border=""0""/></a></li><li>")
            lnkAddNew.RenderControl(writer)
            writer.Write("</ul></div>")

            Dim sStyle As String = String.Empty
            If m_HiddenField.Value <> Nothing AndAlso m_HiddenField.Value.Split(",")(0).ToLower = "true" Then
                sStyle &= "display:none;"
            End If

            If Width <> Nothing Then
                sStyle &= "width:" & Width.ToString & ";"
            End If

            writer.Write("<div id=""" & ClientID & "_AdminSearchContainer"" style=""background:#fff;margin:10px 0 10px 0;overflow: hidden;" & sStyle & """>")

            m_Panel.Attributes("class") = "AdminSearchFieldContainer"
            m_Panel.RenderBeginTag(writer)

            writer.Write("<div id=""AdminClose""><div style=""float: left; font-weight: bold;"">Search</div><a href=""javascript:void(0);"" onclick=""toggleSearch('" & ClientID & "_AdminSearchContainer');"">Close</a></div>")
            For Each ctl As Control In Controls
                If ctl IsNot lnkAddNew And ctl IsNot m_HiddenField Then
                    ctl.RenderControl(writer)
                End If
            Next
            m_Panel.RenderEndTag(writer)

            writer.Write("</div>")
        End Sub
    End Class
End Namespace