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
    Public Class Zip
        Inherits CompositeControl

        Private z4, z5 As System.Web.UI.WebControls.TextBox

        Public Property Zip5() As String
            Get
                EnsureChildControls()
                Return CType(z5.Text, String)
            End Get
            Set(ByVal Value As String)
                EnsureChildControls()
                z5.Text = Value
            End Set
        End Property

        Public Property Zip4() As String
            Get
                EnsureChildControls()
                Return CType(z4.Text, String)
            End Get
            Set(ByVal Value As String)
                EnsureChildControls()
                z4.Text = Value
            End Set
        End Property

        Public Property Value() As String
            Get
                EnsureChildControls()
                If Zip4 <> Nothing And Zip4 <> String.Empty Then
                    Return Zip5 + "-" + Zip4
                Else
                    Return Zip5
                End If
            End Get
            Set(ByVal Value As String)
                EnsureChildControls()
                Try
                    If Value.IndexOf("-") <> -1 Then
                        Dim array() As String = Value.Split("-"c)
                        Zip5 = array(0)
                        Zip4 = array(1)
                    Else
                        Zip5 = Value
                        Zip4 = String.Empty
                    End If
                Catch
                    Zip5 = String.Empty
                    Zip4 = String.Empty
                End Try
            End Set
        End Property

        Public Sub Zip()
        End Sub

        Public Overrides Property Enabled() As Boolean
            Get
                Return MyBase.Enabled
            End Get
            Set(ByVal Value As Boolean)
                EnsureChildControls()
                z5.Enabled = Value
                z4.Enabled = Value
                MyBase.Enabled = Value
            End Set
        End Property

        Protected Overrides Sub CreateChildControls()
            Controls.Clear()
            Controls.Add(New LiteralControl("<table cellpadding=0 cellspacing=0 border=0><tr><td>"))
            CreateZip5()
            Controls.Add(New LiteralControl("-</td><td>"))
            CreateZip4()
            Controls.Add(New LiteralControl("</td></tr></table>"))
        End Sub

        Private Sub CreateZip5()
            z5 = New System.Web.UI.WebControls.TextBox()
            z5.EnableViewState = True
            z5.ID = Me.ID.ToString() + "_ZIP5"
            z5.Width = Unit.Pixel(50)
            z5.MaxLength = 5
            z5.Columns = 5
            z5.BorderColor = Me.BorderColor
            z5.CssClass = Me.CssClass
            z5.BorderStyle = Me.BorderStyle
            z5.BorderWidth = Me.BorderWidth
            z5.ForeColor = Me.ForeColor
            z5.BackColor = Me.BackColor
            z5.Text = Zip5
            Controls.Remove(z5)
            Controls.Add(z5)
        End Sub

        Private Sub CreateZip4()
            z4 = New System.Web.UI.WebControls.TextBox()
            z4.EnableViewState = True
            z4.Width = Unit.Pixel(40)
            z4.MaxLength = 4
            z4.Columns = 4
            z4.ID = Me.ID.ToString() + "_ZIP4"
            z4.BorderColor = Me.BorderColor
            z4.CssClass = Me.CssClass
            z4.BorderStyle = Me.BorderStyle
            z4.BorderWidth = Me.BorderWidth
            z4.ForeColor = Me.ForeColor
            z4.BackColor = Me.BackColor
            z4.Text = Zip4
            Controls.Remove(z4)
            Controls.Add(z4)
        End Sub

        Protected Overrides Sub RecreateChildControls()
            EnsureChildControls()
        End Sub

    End Class
End Namespace