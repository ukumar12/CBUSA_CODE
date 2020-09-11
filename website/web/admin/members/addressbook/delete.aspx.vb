Imports Components
Imports DataLayer
Partial Class admin_members_addressbook_delete
    Inherits AdminPage
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("MEMBERS")
        Dim AddressId As Integer = Convert.ToInt32(Request("AddressId"))
        Dim MemberId As Integer = Convert.ToInt32(Request("memberId"))
        If MemberAddressRow.IsMemberAddressValid(DB, MemberId, AddressId) Then
            MemberAddressRow.RemoveRow(DB, AddressId)
        End If
        Response.Redirect("/admin/members/addressbook/default.aspx?MemberId=" & MemberId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class
