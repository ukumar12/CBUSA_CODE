' ---------------------------------------------
' Copyright 2012 Americaneagle.com
' ---------------------------------------------
Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Collections.Specialized
Imports Components

Namespace Controls
    <ToolboxData("<{0}:CKHelper runat=server></{0}:CKHelper>")> _
 Public Class CKHelper
        Inherits PlaceHolder
        Implements IPostBackDataHandler
        Implements INamingContainer

        Private m_Width As Unit
        Private m_Height As Unit
        Private m_Rows As Integer
        Private m_Columns As Integer
        Private m_Style As String

        Public ReadOnly Property TextBox() As TextBox
            Get
                Return FindControl("txtCKHelper__" & Me.ID)
            End Get
        End Property

        Public Property Style() As String
            Get
                Return m_Style
            End Get
            Set(ByVal value As String)
                m_Style = value
            End Set
        End Property

        Public Property Width() As Unit
            Get
                Return m_Width
            End Get
            Set(ByVal value As Unit)
                m_Width = value
            End Set
        End Property

        Public Property Height() As Unit
            Get
                Return m_Height
            End Get
            Set(ByVal value As Unit)
                m_Height = value
            End Set
        End Property

        Public Property Columns() As Integer
            Get
                Return m_Columns
            End Get
            Set(ByVal value As Integer)
                m_Columns = value
            End Set
        End Property

        Public Property Rows() As Integer
            Get
                Return m_Rows
            End Get
            Set(ByVal value As Integer)
                m_Rows = value
            End Set
        End Property

        Public Property Value() As String
            Get
                Dim txt As TextBox = FindControl("txtCKHelper__" & Me.ID)
                If txt Is Nothing Then
                    Return String.Empty
                End If
                Return txt.Text
            End Get
            Set(ByVal value As String)
                Dim txt As TextBox = FindControl("txtCKHelper__" & Me.ID)
                If txt Is Nothing Then Exit Property
                txt.Text = value
            End Set
        End Property

        Protected Overrides Sub CreateChildControls()
            Controls.Clear()

            Dim txt As New TextBox
            txt.ID = "txtCKHelper__" & Me.ID
            txt.TextMode = TextBoxMode.MultiLine
            txt.Text = Value
            txt.Rows = Rows
            txt.Columns = Columns
            txt.Width = Width
            txt.Height = Height
            If Not Style = Nothing Then txt.Attributes.Add("style", Style)

            Dim btn As New Button
            btn.ID = "btn__" & Me.ID
            btn.Text = "Launch HTML Editor"
            btn.CssClass = "btn"

            Controls.Add(New LiteralControl("<div style=""margin-bottom:5px;"">"))
            Controls.Add(btn)
            Controls.Add(New LiteralControl("</div>"))
            Controls.Add(txt)

            btn.Attributes.Add("onclick", "NewWindow('/admin/CKEditor.aspx?ClientID=" & txt.ClientID & "', 'CKHelper_" & ClientID & "', 816, 700, 'yes', 'yes');return false;")
        End Sub

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            Page.RegisterRequiresPostBack(Me)
            MyBase.OnInit(e)
        End Sub

        Public Sub RaisePostDataChangedEvent() Implements IPostBackDataHandler.RaisePostDataChangedEvent
        End Sub

        Public Function LoadPostData(ByVal postDataKey As String, ByVal values As NameValueCollection) As Boolean Implements IPostBackDataHandler.LoadPostData
            Value = CType(FindControl("txtCKHelper__" & Me.ID), System.Web.UI.WebControls.TextBox).Text
            Return False
        End Function
    End Class
End Namespace
