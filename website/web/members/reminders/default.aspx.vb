Imports Components
Imports DataLayer
Imports System.Data.SqlClient

Public Class Member_Reminders_Default
    Inherits SitePage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureMemberAccess()
        Dim dbMember As MemberRow = MemberRow.GetRow(DB, Session("memberId"))

        Dim ds As DataTable = dbMember.GetReminders()
        rptReminders.DataSource = ds
        rptReminders.DataBind()
        If ds.DefaultView.Count = 0 Then
            divItems.Visible = False
            divNoItems.Visible = True
            ltlNoItems.Text = "<table style=""width:770px; margin:20px 0 15px 20px;"" cellspacing=""0"" cellpadding=""0"" border=""0""  summary=""product""><tr><Td>There are currently no reminders setup for your account.</td></tr></table>"
        Else
            divItems.Visible = True
            divNoItems.Visible = False
        End If
    End Sub

    Protected Sub rptReminders_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptReminders.ItemCommand
        If e.CommandName = "Remove" Then
            Dim dbReminder As MemberReminderRow = MemberReminderRow.GetRow(DB, e.CommandArgument)
            dbReminder.Remove()
            DB.Close()
            Response.Redirect("/members/reminders/")
        ElseIf e.CommandName = "Edit" Then
            Dim dbReminder As MemberReminderRow = MemberReminderRow.GetRow(DB, e.CommandArgument)
            DB.Close()
            Response.Redirect("/members/reminders/edit.aspx?ReminderId=" & e.CommandArgument)
        End If
    End Sub

    Protected Sub rptReminders_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptReminders.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.Item Then
            Dim btnEdit As Button = CType(e.Item.FindControl("btnEdit"), Button)
            Dim btnDelete As Button = CType(e.Item.FindControl("btnDelete"), Button)
            Dim ltlName As Literal = CType(e.Item.FindControl("ltlName"), Literal)
            Dim ltlRecurs As Literal = CType(e.Item.FindControl("ltlRecurs"), Literal)
            Dim ltlDate As Literal = CType(e.Item.FindControl("ltlDate"), Literal)
            Dim ltlFirstReminder As Literal = CType(e.Item.FindControl("ltlFirstReminder"), Literal)
            Dim ltlSecondReminder As Literal = CType(e.Item.FindControl("ltlSecondReminder"), Literal)

            btnDelete.CommandArgument = e.Item.DataItem("ReminderId")
            btnEdit.CommandArgument = e.Item.DataItem("ReminderId")

            ltlDate.Text = FormatDateTime(e.Item.DataItem("EventDate"), DateFormat.LongDate)
            ltlName.Text = e.Item.DataItem("Name")
            If Convert.ToBoolean(e.Item.DataItem("IsRecurrent")) = True Then ltlRecurs.Text = "<span style='color: green;'>Yes</span>" Else ltlRecurs.Text = "<span style='color: red;'>no</span>"
            ltlFirstReminder.Text = DecodeWhenToSend(e.Item.DataItem("DaysBefore1"))
            ltlSecondReminder.Text = DecodeWhenToSend(e.Item.DataItem("DaysBefore2"))
        End If
    End Sub

    Function DecodeWhenToSend(ByVal DaysBefore As String) As String
        Select Case Trim(DaysBefore)
            Case ""
                DaysBefore = "do not send"
            Case "0"
                DaysBefore = "day of event"
            Case "1"
                DaysBefore = "1 day prior to event"
            Case "7"
                DaysBefore = "1 week prior to event"
            Case "14"
                DaysBefore = "2 weeks prior to event"
            Case "21"
                DaysBefore = "3 weeks prior to event"
        End Select
        Return DaysBefore
    End Function

    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        DB.Close()
        Response.Redirect("/members/reminders/edit.aspx")
    End Sub
End Class