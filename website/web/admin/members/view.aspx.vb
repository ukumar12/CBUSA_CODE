Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected MemberId As Integer
    Protected dbMember As MemberRow
    Protected dbBilling As MemberAddressRow
    Protected dbShipping As MemberAddressRow

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("MEMBERS")
        MemberId = Convert.ToInt32(Request("MemberId"))
        dbMember = MemberRow.GetRow(DB, MemberId)
        dbBilling = MemberAddressRow.GetDefaultBillingRow(DB, MemberId)
        dbShipping = MemberAddressRow.GetDefaultShippingRow(DB, MemberId)

        ctrlBillingAddress.Address = dbBilling
        ctrlShippingAddress.Address = dbShipping
        BuildLinks()
        gvListOrderNotes.BindList = AddressOf BindList
        If gvListOrderNotes.SortBy = String.Empty Then
            gvListOrderNotes.SortBy = "son.NoteDate"
            gvListOrderNotes.SortOrder = "Desc"
        End If
        BindList()

        If MemberId = 0 Then
            ctrlBillingAddress.Visible = False
            ctrlShippingAddress.Visible = False
            tblAccount.Visible = False
            gvListOrderNotes.Visible = False
            btnEditShipping.Text = "Add"
            btnEditBilling.Text = "Add"
            btnEditAccount.Text = "Add"
        End If
    End Sub
    Private Sub BindList()
        If MemberId = 0 Then Exit Sub
        Dim dtOrderNotes As DataTable = StoreOrderNoteRow.GetOrderNotesByMember(DB, MemberId, gvListOrderNotes.SortByAndOrder, (gvListOrderNotes.PageIndex + 1) * gvListOrderNotes.PageSize)
        gvListOrderNotes.Pager.NofRecords = StoreOrderNoteRow.GetOrderNotesByMemberCount(DB, MemberId)
        gvListOrderNotes.DataSource = dtOrderNotes.DefaultView
        gvListOrderNotes.DataBind()
    End Sub

    Protected Sub btnEditBilling_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEditBilling.Click
        Response.Redirect("address.aspx?MemberId=" & MemberId & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnEditAccount_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEditAccount.Click
        Response.Redirect("account.aspx?memberId=" & MemberId & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnEditShipping_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEditShipping.Click
        Response.Redirect("address.aspx?memberId=" & MemberId & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub BuildLinks()
        lnkOrder.HRef = "/admin/members/orders.aspx?MemberId=" & MemberId & "&" & GetPageParams(FilterFieldType.All)
        lnkAddressBook.HRef = "/admin/members/addressbook/default.aspx?MemberId=" & MemberId & "&" & GetPageParams(FilterFieldType.All)
        lnkWishList.HRef = "/admin/members/wishlist/default.aspx?MemberId=" & MemberId & "&" & GetPageParams(FilterFieldType.All)
        lnkReminder.HRef = "/admin/members/reminders/default.aspx?MemberId=" & MemberId & "&" & GetPageParams(FilterFieldType.All)
        lnkEmailPref.HRef = "email.aspx?MemberId=" & MemberId & "&" & GetPageParams(FilterFieldType.All)
        lnkAccount.HRef = "account.aspx?MemberId=" & MemberId & "&" & GetPageParams(FilterFieldType.All)
    End Sub

    Protected Sub gvListOrderNotes_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvListOrderNotes.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim ltlOrderNo As Literal = e.Row.FindControl("ltlOrderNo")
            If Not IsDBNull(e.Row.DataItem("OrderNo")) Then ltlOrderNo.Text = "<a href=""/admin/store/orders/default.aspx?F_OrderNo=" & e.Row.DataItem("OrderNo") & """ >" & e.Row.DataItem("OrderNo") & "</a>"
            Dim ltlSubmittedby As Literal = e.Row.FindControl("ltlSubmittedBy")
            If Not e.Row.DataItem("AdminId") = 0 Then ltlSubmittedby.Text = e.Row.DataItem("Submittedby")
        End If
    End Sub

    Protected Sub btnSendPassword_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSendPassword.Click
        Try
            Dim sMsg As String

            sMsg = "Dear " & Core.BuildFullName(dbBilling.FirstName, String.Empty, dbBilling.LastName) & vbCrLf & vbCrLf
            sMsg = sMsg & "Welcome to the " & SysParam.GetValue(DB, "SiteName") & " Member section, below you will find your password." & vbCrLf
            sMsg = sMsg & "Please keep this password in a safe place." & vbCrLf & vbCrLf
            sMsg = sMsg & "Password : " & dbMember.Password & vbCrLf & vbCrLf
            sMsg = sMsg & "Your password is case sensitive, be sure to enter exactly how you see it above." & vbCrLf
            sMsg = sMsg & "Additionally, you can change your User ID and password by clicking on the 'Update my membership information' link after logging in." & vbCrLf
            sMsg = sMsg & vbCrLf
            sMsg = sMsg & "Sincerely," & vbCrLf
            sMsg = sMsg & SysParam.GetValue(DB, "SiteName") & " Administrator" & vbCrLf

            Call Core.SendSimpleMail(SysParam.GetValue(DB, "ContactUsEmail"), SysParam.GetValue(DB, "ContactUsName"), dbBilling.Email, dbBilling.Email, SysParam.GetValue(DB, "Sitename") & " password", sMsg)

            Response.Redirect(Request.Url.PathAndQuery)

        Catch ex As Exception
            AddError("The username you entered could not be found in our system. Please try again, or you may also create a new account")
        End Try
    End Sub

End Class
