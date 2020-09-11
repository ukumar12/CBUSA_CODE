Imports Components
Imports DataLayer
Imports System.Data.SqlClient
Partial Class admin_members_email
    Inherits AdminPage
    Protected MemberId As Integer
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("MEMBERS")
        MemberId = Convert.ToInt32(Request("MemberId"))

        If Not IsPostBack Then
            LoadMailingList()

            Dim dbMember As MemberRow = MemberRow.GetRow(DB, MemberId)

            Dim dbBilling As MemberAddressRow = MemberAddressRow.GetDefaultBillingRow(DB, MemberId)
            Dim dbMailingMember As MailingMemberRow = MailingMemberRow.GetRowByEmail(DB, dbBilling.Email)

            txtMemberName.Text = "<b>" + Core.BuildFullName(dbBilling.FirstName, dbBilling.MiddleInitial, dbBilling.LastName) + " (" + dbMember.Username + ")</b>"

            ltlEmail.Text = dbBilling.Email

            rbtnNewsletterYes.Checked = (dbMailingMember.Status = "ACTIVE" And Not dbMailingMember.SubscribedLists = String.Empty)
            rbtnNewsletterNo.Checked = Not rbtnNewsletterYes.Checked
            If dbMailingMember.MemberId = 0 Then
                rbtnFormatHTML.Checked = True
                For Each ol As ListItem In chklstMailingLists.Items
                    ol.Selected = True
                Next
            Else
                rbtnFormatHTML.Checked = (dbMailingMember.MimeType = "HTML")
                rbtnFormatTEXT.Checked = Not rbtnFormatHTML.Checked
                chklstMailingLists.SelectedValues = dbMailingMember.SubscribedLists
            End If
            lnkBack.HRef = "/admin/members/view.aspx?MemberId=" & MemberId & "&" & GetPageParams(FilterFieldType.All)
        End If
        cvrbtnNewsletterYesAtLeastOne.Enabled = rbtnNewsletterYes.Checked
    End Sub

    Private Sub LoadMailingList()
        chklstMailingLists.DataSource = MailingListRow.GetPermanentLists(DB)
        chklstMailingLists.DataValueField = "ListId"
        chklstMailingLists.DataTextField = "Name"
        chklstMailingLists.DataBind()
    End Sub

    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
        If Not IsValid Then Exit Sub

        Try
            DB.BeginTransaction()

            Dim dbMember As MemberRow = MemberRow.GetRow(DB, MemberId)
            Dim dbBilling As MemberAddressRow = MemberAddressRow.GetDefaultBillingRow(DB, MemberId)

            'Update MailingMember, but only if user subscribed for the newsletter
            Dim dbMailingMember As MailingMemberRow = MailingMemberRow.GetRowByEmail(DB, dbBilling.Email)
            If rbtnFormatHTML.Checked = True Then dbMailingMember.MimeType = "HTML" Else dbMailingMember.MimeType = "TEXT"
            dbMailingMember.Email = dbBilling.Email
            dbMailingMember.Name = Core.BuildFullName(dbBilling.FirstName, String.Empty, dbBilling.LastName)
            dbMailingMember.Status = "ACTIVE"
            If dbMailingMember.MemberId <> 0 Then
                dbMailingMember.Update()
                dbMember.ModifyDate = Date.Now
                dbMember.Update()
            Else
                dbMailingMember.Insert()
            End If
            dbMailingMember.DeleteFromAllPermanentLists()
            If rbtnNewsletterYes.Checked Then
                dbMailingMember.InsertToLists(chklstMailingLists.SelectedValues)
            End If
            DB.CommitTransaction()

            Response.Redirect("/admin/members/view.aspx?MemberId=" & MemberId & "&" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("/admin/members/view.aspx?MemberId=" & MemberId & "&" & GetPageParams(FilterFieldType.All))
    End Sub


End Class
