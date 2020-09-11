Imports System
Imports System.Web.UI
Imports System.Web
Imports System.Web.UI.WebControls
Imports System.ComponentModel
Imports System.Globalization
Imports System.Web.UI.HtmlControls
Imports System.Collections
Imports System.Collections.Specialized

Namespace Controls
    <ToolboxData("<{0}:Phone runat=server></{0}:Phone>")> _
    Public Class Phone
        Inherits CompositeControl

        Private phone1 As TextBox, phone2 As TextBox, phone3 As TextBox

        Public Property Phone_1() As String
            Get
                EnsureChildControls()
                Return CType(phone1.Text, String)
            End Get
            Set(ByVal value As String)
                EnsureChildControls()
                phone1.Text = value
            End Set
        End Property

        Public Property Phone_2() As String
            Get
                EnsureChildControls()
                Return CType(phone2.Text, String)
            End Get
            Set(ByVal value As String)
                EnsureChildControls()
                phone2.Text = value
            End Set
        End Property

        Public Property Phone_3() As String
            Get
                EnsureChildControls()
                Return CType(phone3.Text, String)
            End Get
            Set(ByVal value As String)
                EnsureChildControls()
                phone3.Text = value
            End Set
        End Property

        Public Property Value() As String
            Get
                EnsureChildControls()
                Dim retval As String = Phone_1 & "-" & Phone_2 & "-" & Phone_3
                If retval.Replace("-", "") = String.Empty Then
                    Return String.Empty
                Else
                    Return Phone_1 & "-" & Phone_2 & "-" & Phone_3
                End If
            End Get
            Set(ByVal Value As String)
                EnsureChildControls()
                Try
                    Dim array() As String = Value.Split("-"c)
                    Phone_1 = array(0)
                    Phone_2 = array(1)
                    Phone_3 = array(2)
                Catch
                    Phone_1 = String.Empty
                    Phone_2 = String.Empty
                    Phone_3 = String.Empty
                End Try
            End Set
        End Property

        Public Sub New()
        End Sub

        Protected Overrides Sub CreateChildControls()
            Controls.Clear()
            Controls.Add(New LiteralControl("<table cellpadding=0 cellspacing=0 border=0><tr><td nowrap valign=top>"))
            CreatePhone1()
            Controls.Add(New LiteralControl("&nbsp;</td><td nowrap valign=top>"))
            CreatePhone2()
            Controls.Add(New LiteralControl("-</td><td nowrap valign=top>"))
            CreatePhone3()
            Controls.Add(New LiteralControl("&nbsp;</td></tr></table>"))
        End Sub

        Private Sub CreatePhone1()
            phone1 = New TextBox()
            phone1.ID = Me.ID.ToString() + "_PHONE1"
            phone1.Width = Unit.Pixel(30)
            phone1.MaxLength = 3
            phone1.Columns = 3
            phone1.BorderColor = Me.BorderColor
            phone1.CssClass = Me.CssClass
            phone1.BorderStyle = Me.BorderStyle
            phone1.BorderWidth = Me.BorderWidth
            phone1.ForeColor = Me.ForeColor
            phone1.BackColor = Me.BackColor
            phone1.Text = Phone_1
            Controls.Remove(phone1)
            Controls.Add(phone1)
        End Sub

        Private Sub CreatePhone2()
            phone2 = New TextBox()
            phone2.ID = Me.ID.ToString() + "_PHONE2"
            phone2.Width = Unit.Pixel(30)
            phone2.MaxLength = 3
            phone2.Columns = 3
            phone2.BorderColor = Me.BorderColor
            phone2.CssClass = Me.CssClass
            phone2.BorderStyle = Me.BorderStyle
            phone2.BorderWidth = Me.BorderWidth
            phone2.ForeColor = Me.ForeColor
            phone2.BackColor = Me.BackColor
            phone2.Text = Phone_2
            Controls.Remove(phone2)
            Controls.Add(phone2)
        End Sub

        Private Sub CreatePhone3()
            phone3 = New TextBox()
            phone3.ID = Me.ID.ToString() + "_PHONE3"
            phone3.Width = Unit.Pixel(40)
            phone3.MaxLength = 4
            phone3.Columns = 4
            phone3.BorderColor = Me.BorderColor
            phone3.CssClass = Me.CssClass
            phone3.BorderStyle = Me.BorderStyle
            phone3.BorderWidth = Me.BorderWidth
            phone3.ForeColor = Me.ForeColor
            phone3.BackColor = Me.BackColor
            phone3.Text = Phone_3
            Controls.Remove(phone3)
            Controls.Add(phone3)
        End Sub
    End Class
End Namespace