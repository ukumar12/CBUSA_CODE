Imports Components
Imports DataLayer
Imports System.Data.SqlClient

Public Class Member_Reminders_Default
    Inherits AdminPage
    Protected Memberid As Integer

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("MEMBERS")
        Memberid = Convert.ToInt32(Request.QueryString("MemberId"))
        Dim dbMember As MemberRow = MemberRow.GetRow(DB, Memberid)
        Dim dbMemberBilling As MemberAddressRow = MemberAddressRow.GetDefaultBillingRow(DB, Memberid)
        txtMemberName.Text = "<b>" + Core.BuildFullName(dbMemberBilling.FirstName, dbMemberBilling.MiddleInitial, dbMemberBilling.LastName) + " (" + dbMember.Username + ")</b>"
        lnkBack.HRef = "/admin/members/view.aspx?MemberId=" & Memberid & "&" & GetPageParams(FilterFieldType.All)
        Dim ds As DataTable = dbMember.GetReminders()
        gvReminders.DataSource = ds
        gvReminders.Pager.NofRecords = ds.Rows.Count()
        gvReminders.DataBind()
    End Sub

    Protected Sub gvReminders_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvReminders.RowDataBound
        If e.Row.RowType = ListItemType.AlternatingItem Or e.Row.RowType = ListItemType.Item Then
            Dim ltlName As Literal = CType(e.Row.FindControl("ltlName"), Literal)
            Dim ltlRecurs As Literal = CType(e.Row.FindControl("ltlRecurs"), Literal)
            Dim ltlDate As Literal = CType(e.Row.FindControl("ltlDate"), Literal)
            Dim ltlFirstReminder As Literal = CType(e.Row.FindControl("ltlFirstReminder"), Literal)
            Dim ltlSecondReminder As Literal = CType(e.Row.FindControl("ltlSecondReminder"), Literal)

            ltlDate.Text = FormatDateTime(e.Row.DataItem("EventDate"), DateFormat.LongDate)
            ltlName.Text = e.Row.DataItem("Name")
            If Convert.ToBoolean(e.Row.DataItem("IsRecurrent")) = True Then ltlRecurs.Text = "<span style='color: green;'>Yes</span>" Else ltlRecurs.Text = "<span style='color: red;'>no</span>"
            ltlFirstReminder.Text = DecodeWhenToSend(e.Row.DataItem("DaysBefore1"))
            ltlSecondReminder.Text = DecodeWhenToSend(e.Row.DataItem("DaysBefore2"))
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
        Response.Redirect("/admin/members/reminders/edit.aspx?MemberId=" & Memberid & "&" & GetPageParams(FilterFieldType.All))
    End Sub

  
End Class