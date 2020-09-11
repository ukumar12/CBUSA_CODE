Imports System
Imports System.Web.UI
Imports System.Text
Imports System.Collections
Imports System.Collections.Specialized
Imports System.Web
Imports System.Web.UI.WebControls
Imports System.ComponentModel
Imports System.Globalization
Imports System.Web.UI.HtmlControls
Imports System.Configuration

Namespace Controls
    <ToolboxData("<{0}:ExpDate runat=server></{0}:ExpDate>")> _
    Public Class ExpDate
        Inherits CompositeControl

        Private CtrlMonth As DropDownList
        Private CtrlYear As DropDownList

        Private m_StartYear As Integer = Year(Now)
        Private m_EndYear As Integer = Year(Now) + 20

        Public Property Value() As String
            Get
                EnsureChildControls()
                If (CtrlYear.SelectedValue = String.Empty Or CtrlMonth.SelectedValue = String.Empty) Then
                    Return String.Empty
                End If
                Value = CtrlMonth.SelectedValue & "/" & 1 & "/" & CtrlYear.SelectedValue
            End Get
            Set(ByVal value As String)
                EnsureChildControls()
                If value = "12:00:00 AM" Then value = String.Empty
                If value = String.Empty Then
                    Exit Property
                End If
                CtrlMonth.SelectedValue = Month(DateTime.Parse(value))
                CtrlYear.SelectedValue = Year(DateTime.Parse(value))
            End Set
        End Property

        Public Property StartYear() As Integer
            Get
                Return m_StartYear
            End Get
            Set(ByVal value As Integer)
                m_StartYear = value
            End Set
        End Property

        Public Property EndYear() As Integer
            Get
                Return m_EndYear
            End Get
            Set(ByVal value As Integer)
                m_EndYear = value
            End Set
        End Property

        Public Sub New()
        End Sub

        Protected Overrides Sub CreateChildControls()
            Controls.Clear()
            Controls.Add(New System.Web.UI.LiteralControl("<table cellpadding=0 cellspacing=0 border=0><tr><td nowrap>"))
            CreateMonthList()
            Controls.Add(New System.Web.UI.LiteralControl("</td><td nowrap>"))
            CreateYearList()
            Controls.Add(New System.Web.UI.LiteralControl("</td></tr></table>"))
        End Sub

        Private Sub CreateMonthList()
            CtrlMonth = New DropDownList
            CtrlMonth.ID = Me.ID.ToString() + "_MONTH"
            CtrlMonth.BorderColor = Me.BorderColor
            CtrlMonth.CssClass = Me.CssClass
            CtrlMonth.Attributes("style") = Me.Attributes("style")
            CtrlMonth.BorderStyle = Me.BorderStyle
            CtrlMonth.BorderWidth = Me.BorderWidth
            CtrlMonth.ForeColor = Me.ForeColor
            CtrlMonth.BackColor = Me.BackColor
            Controls.Remove(CtrlMonth)
            Controls.Add(CtrlMonth)

            CtrlMonth.Items.Add(New ListItem("", ""))
            For i As Integer = 0 To 11
                Dim j As Integer = i + 1
                Dim monthname As String = New DateTimeFormatInfo().MonthNames(i).ToString()
                Dim item As ListItem = New ListItem(monthname, j.ToString())
                CtrlMonth.Items.Add(item)
            Next
        End Sub

        Private Sub CreateYearList()
            CtrlYear = New DropDownList
            CtrlYear.ID = Me.ID.ToString() & "_YEAR"
            CtrlYear.BorderColor = Me.BorderColor
            CtrlYear.CssClass = Me.CssClass
            CtrlYear.Attributes("style") = Me.Attributes("style")
            CtrlYear.BorderStyle = Me.BorderStyle
            CtrlYear.BorderWidth = Me.BorderWidth
            CtrlYear.ForeColor = Me.ForeColor
            CtrlYear.BackColor = Me.BackColor
            Controls.Remove(CtrlYear)
            Controls.Add(CtrlYear)

            CtrlYear.Items.Add(New ListItem("", ""))
            For i As Integer = StartYear To EndYear
                Dim item As ListItem = New ListItem(i.ToString(), i.ToString())
                CtrlYear.Items.Add(item)
            Next
        End Sub

    End Class
End Namespace