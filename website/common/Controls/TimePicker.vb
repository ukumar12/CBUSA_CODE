Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.web
Imports System.Text
Imports System.Collections.Specialized

Namespace Controls
    Public Class TimePicker
        Inherits WebControl
        Implements INamingContainer
        Implements IPostBackDataHandler

        Private m_Hour As String = String.Empty
        Private m_Minute As String = String.Empty
        Private m_AMPM As String = String.Empty
        Private m_Text As String = String.Empty

        Public Sub RaisePostDataChangedEvent() Implements IPostBackDataHandler.RaisePostDataChangedEvent
        End Sub

        Public Property Text() As String
            Get
                Return Time
            End Get
            Set(ByVal value As String)
                m_Text = value
                If Not m_Text = String.Empty Then
                    m_Minute = Minute(value)
                    If Hour(value) > 12 Then m_Hour = Hour(value) - 12 Else m_Hour = Hour(value)
                    If Hour(value) >= 12 Then m_AMPM = "PM" Else m_AMPM = "AM"
                Else
                    m_Minute = String.Empty
                    m_Hour = String.Empty
                    m_AMPM = String.Empty
                End If
            End Set
        End Property

        Private ReadOnly Property Time() As String
            Get
                If m_Hour = String.Empty Then Return String.Empty
                If m_Minute = String.Empty Then Return String.Empty
                If m_AMPM = String.Empty Then Return String.Empty

                Try
                    Dim dt As DateTime = CDate(m_Hour & ":" & m_Minute & " " & m_AMPM)
                Catch ex As Exception
                    Return String.Empty
                End Try
                Return CDate(m_Hour & ":" & m_Minute & " " & m_AMPM)
            End Get
        End Property

        Public Function LoadPostData(ByVal postDataKey As String, ByVal values As NameValueCollection) As Boolean Implements IPostBackDataHandler.LoadPostData
            Dim context As HttpContext = HttpContext.Current
            Dim HoursCtrl As DropDownList = FindControl("H")
            Dim MinutesCtrl As DropDownList = FindControl("M")
            Dim AMPMCtrl As DropDownList = FindControl("AMPM")

            m_Hour = HoursCtrl.SelectedValue
            m_Minute = MinutesCtrl.SelectedValue
            m_AMPM = AMPMCtrl.SelectedValue

            Return False
        End Function

        Protected Overrides Sub OnInit(ByVal e As EventArgs)
            Page.RegisterRequiresPostBack(Me)
            MyBase.OnInit(e)
        End Sub

        Public Sub New()
        End Sub

        Protected Overrides Sub CreateChildControls()
            Controls.Clear()
            Controls.Add(New LiteralControl("<table cellpadding=0 cellspacing=0 border=0><tr><td>"))
            CreateHoursList()
            Controls.Add(New LiteralControl("/</td><td>"))
            CreateMinutesList()
            Controls.Add(New LiteralControl("/</td><td>"))
            CreateAMPMList()
            Controls.Add(New LiteralControl("</td></tr></table>"))
        End Sub

        Private Sub CopyProperties(ByVal ctrl As WebControl)
            ctrl.EnableViewState = False
            ctrl.BorderColor = Me.BorderColor
            ctrl.CssClass = Me.CssClass
            ctrl.BorderStyle = Me.BorderStyle
            ctrl.BorderWidth = Me.BorderWidth
            ctrl.ForeColor = Me.ForeColor
            ctrl.BackColor = Me.BackColor
        End Sub

        Private Sub CreateHoursList()
            Dim hours As DropDownList = New DropDownList()
            CopyProperties(hours)
            hours.ID = "H"
            Controls.Add(hours)
            hours.Items.Add(New ListItem("", ""))
            Dim i As Integer
            For i = 1 To 12
                Dim text As String = i
                If Len(text) = 1 Then text = "0" & text
                Dim item As ListItem = New ListItem(text, i)
                If Not m_Hour = String.Empty AndAlso m_Hour = i Then
                    item.Selected = True
                End If
                hours.Items.Add(item)
            Next
        End Sub

        Private Sub CreateMinutesList()
            Dim minutes As DropDownList = New DropDownList()
            CopyProperties(minutes)
            minutes.ID = "M"
            Controls.Remove(minutes)
            Controls.Add(minutes)
            minutes.Items.Add(New ListItem("", ""))
            Dim i As Integer
            For i = 0 To 59
                Dim text As String = i
                If Len(text) = 1 Then text = "0" & text
                Dim item As ListItem = New ListItem(text, i)
                If Not m_Minute = String.Empty AndAlso m_Minute = i Then
                    item.Selected = True
                End If
                minutes.Items.Add(item)
            Next
        End Sub

        Private Sub CreateAMPMList()
            Dim ampm As DropDownList = New DropDownList()
            Dim item As ListItem

            CopyProperties(ampm)
            ampm.ID = "AMPM"
            Controls.Remove(ampm)
            Controls.Add(ampm)
            ampm.Items.Add(New ListItem("", ""))
            item = New ListItem("AM", "AM")
            If m_AMPM = "AM" Then item.Selected = True
            ampm.Items.Add(item)
            item = New ListItem("PM", "PM")
            If m_AMPM = "PM" Then item.Selected = True
            ampm.Items.Add(item)
        End Sub

    End Class
End Namespace
