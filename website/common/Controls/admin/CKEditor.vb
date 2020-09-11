' ---------------------------------------------
' Copyright 2012 Americaneagle.com
' ---------------------------------------------
Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Collections.Specialized
Imports Components
Imports Karamasoft.WebControls.UltimateSpell
Imports System.Collections.Generic
Imports System.Text

Namespace Controls

    ''' <summary>
    ''' Functions as a wrapper for FCKeditor's default editor class, providing a mechanism for resizing the 
    ''' control and spell checking.
    ''' </summary>
    ''' <remarks>Do NOT use this control on the front end.  Only on the admin backend should a user be able
    ''' to use this control.
    ''' <seealso cref="RequiredCKValidator" /></remarks>
    <ToolboxData("<{0}:CKEditor runat=server></{0}:CKEditor>")> _
    Public Class CKEditor
        Inherits CompositeControl
        Implements IPostBackDataHandler

        ''' <summary>
        ''' The internal FCKeditor control which powers this control.
        ''' </summary>
        ''' <remarks>This field is public in order to expose all of its configuration properties without
        ''' exposing all of the properties by hand.</remarks>
        Private Editor As CKEditorWrapper.CKEditorControl

        Public Shadows Property Value() As String
            Get
                Return Editor.Text
            End Get
            Set(ByVal value As String)
                Editor.Text = value
            End Set
        End Property

        Public Shadows Property Width() As String
            Get
                If ViewState("Width") Is Nothing Then ViewState("Width") = "500"
                Return ViewState("Width")
            End Get
            Set(ByVal value As String)
                ViewState("Width") = value
                Editor.config.width = Width
            End Set
        End Property

        Public Shadows Property Height() As String
            Get
                If ViewState("Height") Is Nothing Then ViewState("Height") = "200"
                Return ViewState("Height")
            End Get
            Set(ByVal value As String)
                ViewState("Height") = value
                Editor.config.height = Height
            End Set
        End Property

        Public Property ResizeEnabled() As Boolean
            Get
                If ViewState("ResizeEnabled") Is Nothing Then ViewState("ResizeEnabled") = True
                Return ViewState("ResizeEnabled")
            End Get
            Set(ByVal value As Boolean)
                Editor.config.resize_enabled = ResizeEnabled
                ViewState("ResizeEnabled") = value
            End Set
        End Property

        ''' <summary>
        ''' Initializes a new instance of the <see cref="CKEditor" /> class.
        ''' </summary>
        Public Sub New()
            Editor = New CKEditorWrapper.CKEditorControl
        End Sub

        Private Property spell As New UltimateSpell

        ''' <summary>
        ''' Constructs the HTML of the control based on the specified dimension.
        ''' </summary>
        ''' <remarks>This method creates, in addition to the FCKeditor, a dropdown which can be used to 
        ''' select a size for the control, which is saved and restored via a cookie.</remarks>
        Protected Overrides Sub CreateChildControls()
            Controls.Clear()
            Editor.ID = "ccEditor"
            Controls.Add(Editor)

            Editor.config.width = Width
            Editor.config.height = Height
            Editor.config.resize_enabled = ResizeEnabled
            Editor.config.resize_minWidth = 200
            Editor.config.resize_minHeight = 100

            'Add UltimateSpell spellchecker
            spell.ID = "UltimateSpell"
            spell.ControlIdsToCheck = "ckeditor_" & Editor.ClientID
            spell.ShowSpellButton = False
            spell.PostBackOnOK = False

            Controls.Add(spell)
        End Sub

        ''' <summary>
        ''' Registers the postback handler with the page and raises the <see cref="Init" /> event.
        ''' </summary>
        ''' <param name="e">An <see cref="EventArgs" /> object that contains the event data.</param>
        Protected Overrides Sub OnInit(ByVal e As EventArgs)
            Page.RegisterRequiresPostBack(Me)
            MyBase.OnInit(e)
        End Sub

        ''' <summary>
        ''' This method exists in order to implement the <see cref="IPostBackDataHandler" /> interface.  However,
        ''' it does nothing.
        ''' </summary>
        Public Sub RaisePostDataChangedEvent() Implements IPostBackDataHandler.RaisePostDataChangedEvent
        End Sub

        ''' <summary>
        ''' Persists the <see cref="Width" /> and <see cref="Height" /> property of the control.
        ''' </summary>
        ''' <param name="postDataKey">Not used by this implementation.</param>
        ''' <param name="values">Not used by this implementation.</param>
        ''' <returns><see langword="False" />.</returns>
        ''' <remarks>This method completely ignores the values of <paramref name="postDataKey" /> and
        ''' <paramref name="postCollection" /> in its implementation.</remarks>
        Public Function LoadPostData(ByVal postDataKey As String, ByVal values As NameValueCollection) As Boolean Implements IPostBackDataHandler.LoadPostData
            Return False
        End Function

    End Class
End Namespace