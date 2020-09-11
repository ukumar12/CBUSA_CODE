Imports Components
Imports DataLayer
Imports System.Data.SqlClient

Public Class Member_Reminders_Edit
    Inherits SitePage

    Private m_ReminderId As Integer

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureMemberAccess()
        Try
            m_ReminderId = Request.QueryString("ReminderId")
            Dim dbMember As MemberRow = MemberRow.GetRow(DB, Session("MemberId"))
        Catch ex As Exception
            Response.Redirect("/members/reminders/")
        End Try
        If Not IsPostBack Then
            LoadDropdowns()
            If m_ReminderId > 0 Then LoadFormData()
        End If
    End Sub

    Private Sub LoadFormData()
        Dim dbMemberReminder As MemberReminderRow
        If m_ReminderId > 0 Then dbMemberReminder = MemberReminderRow.GetRow(DB, m_ReminderId) Else Exit Sub
        If Session("MemberId") <> dbMemberReminder.MemberId Then
            Response.Redirect("/members/reminders/", False)
        End If
        txtName.Text = dbMemberReminder.Name
        chkIsRecurring.Checked = dbMemberReminder.IsRecurrent
        ctrlEventDate.Value = dbMemberReminder.EventDate
        drpFirstReminder.SelectedValue = dbMemberReminder.DaysBefore1
        drpSecondReminder.SelectedValue = dbMemberReminder.DaysBefore2
        txtEmail.Text = dbMemberReminder.Email
        txtComments.Text = dbMemberReminder.Body
    End Sub

    Private Sub LoadDropdowns()
        drpFirstReminder.Items.Insert(0, New ListItem("do not send", ""))
        drpFirstReminder.Items.Insert(1, New ListItem("day of event", "0"))
        drpFirstReminder.Items.Insert(2, New ListItem("1 day prior to event", "1"))
        drpFirstReminder.Items.Insert(3, New ListItem("1 week prior to event", "7"))
        drpFirstReminder.Items.Insert(4, New ListItem("2 weeks prior to event", "14"))
        drpFirstReminder.Items.Insert(5, New ListItem("3 weeks prior to event", "21"))

        drpSecondReminder.Items.Insert(0, New ListItem("do not send", ""))
        drpSecondReminder.Items.Insert(1, New ListItem("day of event", "0"))
        drpSecondReminder.Items.Insert(2, New ListItem("1 day prior to event", "1"))
        drpSecondReminder.Items.Insert(3, New ListItem("1 week prior to event", "7"))
        drpSecondReminder.Items.Insert(4, New ListItem("2 weeks prior to event", "14"))
        drpSecondReminder.Items.Insert(5, New ListItem("3 weeks prior to event", "21"))

    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()
            Dim dbMemberReminder As MemberReminderRow
            If m_ReminderId > 0 Then dbMemberReminder = MemberReminderRow.GetRow(DB, m_ReminderId) Else dbMemberReminder = New MemberReminderRow(DB)

            dbMemberReminder.MemberId = Session("MemberId")
            dbMemberReminder.Name = txtName.Text
            dbMemberReminder.IsRecurrent = chkIsRecurring.Checked
            dbMemberReminder.EventDate = ctrlEventDate.Value
            dbMemberReminder.DaysBefore1 = drpFirstReminder.SelectedValue
            dbMemberReminder.DaysBefore2 = drpSecondReminder.SelectedValue
            dbMemberReminder.Email = txtEmail.Text
            dbMemberReminder.Body = txtComments.Text

            If m_ReminderId > 0 Then dbMemberReminder.Update() Else dbMemberReminder.Insert()
            DB.CommitTransaction()

            Response.Redirect("/members/reminders/")

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class