Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Collections.Specialized
Imports Components

Namespace Controls
	<ToolboxData("<{0}:FCKEditor runat=server></{0}:FCKEditor>")> _
	Public Class FCKEditor
		Inherits CompositeControl
		Implements IPostBackDataHandler

        Public Editor As FredCK.FCKeditorV2.FCKeditor
        Private WithEvents CtrlDimensions As DropDownList
        Private Dimensions As New DimensionCollection
        Private Save As Button

        Private Class Dimension
            Public Width As Unit
            Public Height As Unit
            Public ReadOnly Property Value() As String
                Get
                    Return Me.Width.Value.ToString & IIf(Me.Width.Type = UnitType.Percentage, "%", "") & "x" & Me.Height.Value.ToString
                End Get
            End Property
        End Class
        Private Class DimensionCollection
            Inherits GenericCollection(Of Dimension)
        End Class

        Public Sub New()
            Editor = New FredCK.FCKeditorV2.FCKeditor

            Dim d As Dimension

            d = New Dimension
            d.Width = New Unit("0")
            d.Height = New Unit("0")
            Dimensions.Add(d)

            d = New Dimension
            d.Width = New Unit("600")
            d.Height = New Unit("300")
            Dimensions.Add(d)

            d = New Dimension
            d.Width = New Unit("600")
            d.Height = New Unit("450")
            Dimensions.Add(d)

            d = New Dimension
            d.Width = New Unit("800")
            d.Height = New Unit("600")
            Dimensions.Add(d)

            d = New Dimension
            d.Width = New Unit("1024")
            d.Height = New Unit("768")
            Dimensions.Add(d)

            d = New Dimension
            d.Width = New Unit("100%")
            d.Height = New Unit("550")
            Dimensions.Add(d)

            d = New Dimension
            d.Width = New Unit("100%")
            d.Height = New Unit("768")
            Dimensions.Add(d)
        End Sub

        Public Shadows Property Width() As Unit
            Get
                Return Editor.Width
            End Get
            Set(ByVal value As Unit)
                Editor.Width = value
            End Set
        End Property

        Public Shadows Property Height() As Unit
            Get
                Return Editor.Height
            End Get
            Set(ByVal value As Unit)
                Editor.Height = value
            End Set
        End Property

        Public Shadows Property Value() As String
            Get
                Return Editor.Value
            End Get
            Set(ByVal value As String)
                Editor.Value = value
            End Set
        End Property

        Protected Overrides Sub CreateChildControls()
            Controls.Clear()

            Controls.Add(New LiteralControl("<table border=""0""><tr><td class=""bold smaller"">Select Window Size:</td><td>"))
            CtrlDimensions = New DropDownList
            CtrlDimensions.ID = "CtrlDimensions"
            CtrlDimensions.CssClass = "smaller"
            For Each d As Dimension In Dimensions
                CtrlDimensions.Items.Add(New ListItem(IIf(d.Width.Value = 0 AndAlso d.Height.Value = 0, "Hide", d.Value.Replace("x", " x ")), d.Value.Replace(" ", "")))
            Next
            CtrlDimensions.SelectedValue = Me.Width.Value.ToString & IIf(Me.Width.Type = UnitType.Percentage, "%", "") & "x" & Me.Height.Value.ToString

            Dim Context As System.Web.HttpContext = System.Web.HttpContext.Current
            Dim CookieName As String = Context.Server.UrlEncode(Context.Request.Path) & "_" & Me.ID
            If Not Page.IsPostBack Then
                For i As Integer = 0 To Context.Request.Cookies.Count - 1
                    If Context.Request.Cookies(i).Name = CookieName Then
                        CtrlDimensions.SelectedValue = Context.Request.Cookies(i).Value
                        Dim a() As String = CtrlDimensions.SelectedValue.Split("x")
                        Width = New Unit(a(0))
                        Height = New Unit(a(1))
                    End If
                Next
            End If
            Controls.Add(CtrlDimensions)

            Controls.Add(New LiteralControl("</td><td>"))

            Save = New Button
            Save.ID = "btnSave"
            Save.CssClass = "btn"
            Save.Style.Add("font-size", "11px")
            Save.Text = "Save this Setting"
            Save.Attributes.Add("onclick", "createCookie('" & CookieName & "',document.getElementById('" & CtrlDimensions.ClientID & "').value);if(readCookie('" & CookieName & "'))alert('Settings Saved!');return false;")

            Controls.Add(Save)

            Controls.Add(New LiteralControl("</td></tr></table>"))

            Editor.Width = Me.Width
            Editor.Height = Me.Height
            Editor.ID = "ccEditor"
            Controls.Add(Editor)

            CtrlDimensions.Attributes.Add("onchange", "var editor = document.getElementById('" & Editor.ClientID & "___Frame');var arr = this.value.split('x'); editor.style.width = arr[0]; editor.style.height = arr[1];")
        End Sub

		Protected Overrides Sub OnInit(ByVal e As EventArgs)
			Page.RegisterRequiresPostBack(Me)
			MyBase.OnInit(e)
		End Sub

		Public Sub RaisePostDataChangedEvent() Implements IPostBackDataHandler.RaisePostDataChangedEvent
		End Sub

		Public Function LoadPostData(ByVal postDataKey As String, ByVal values As NameValueCollection) As Boolean Implements IPostBackDataHandler.LoadPostData
			Dim drp As DropDownList = FindControl("CtrlDimensions")

			Dim a() As String = drp.SelectedValue.Split("x")
			Width = New Unit(a(0))
			Height = New Unit(a(1))

			Return False
		End Function
	End Class
End Namespace