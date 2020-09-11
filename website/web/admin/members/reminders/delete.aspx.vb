Imports Components
Imports DataLayer
Partial Class admin_members_reminders_delete
    Inherits AdminPage
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("MEMBERS")
        Dim ReminderId As Integer = Convert.ToInt32(Request("ReminderId"))
        Dim MemberId As Integer = Convert.ToInt32(Request("memberId"))
        Dim dbReminder As MemberReminderRow = MemberReminderRow.GetRow(DB, ReminderId)
        dbReminder.Remove()
        Response.Redirect("/admin/members/reminders/default.aspx?MemberId=" & MemberId & "&" & GetPageParams(FilterFieldType.All))
    End Sub

End Class
