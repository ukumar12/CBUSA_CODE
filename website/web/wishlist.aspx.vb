Imports Components
Imports System.IO
Imports DataLayer

Public Class Wishlist
    Inherits SitePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim Guid As String = Request("w")
        Dim dbMember As MemberRow = MemberRow.GetRowByGuid(DB, Guid)
        ctrlWishlist.MemberId = dbMember.MemberId
    End Sub
End Class
