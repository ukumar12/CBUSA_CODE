Imports Components
Imports DataLayer
Imports System.Data.SqlClient

Public Class Members_Default
    Inherits SitePage

    Protected dbMember As MemberRow
    Protected dbBilling As MemberAddressRow
    Protected dbShipping As MemberAddressRow

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureSSL()
        EnsureMemberAccess()

        dbMember = MemberRow.GetRow(DB, Session("MemberId"))
        dbBilling = MemberAddressRow.GetDefaultBillingRow(DB, Session("MemberId"))
        dbShipping = MemberAddressRow.GetDefaultShippingRow(DB, Session("MemberId"))

        ctrlBillingAddress.Address = dbBilling
        ctrlShippingAddress.Address = dbShipping
    End Sub

    Protected Sub btnEditBilling_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEditBilling.Click
        Response.Redirect("/members/addresses.aspx")
    End Sub

    Protected Sub btnEditAccount_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEditAccount.Click
        Response.Redirect("/members/account.aspx")
    End Sub

    Protected Sub btnEditShipping_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEditShipping.Click
        Response.Redirect("/members/addresses.aspx")
    End Sub
End Class