Imports Components
Imports DataLayer
Imports System.Data.SqlClient

Public Class Member_Wishlist_Default
    Inherits AdminPage

    Protected MemberId As Integer
    Private OrderId As Integer
    Private dtItems As DataTable
    Private ItemCount As Integer

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("MEMBERS")
        MemberId = Convert.ToInt32(Request.QueryString("MemberId"))
        Dim dbMember As MemberRow = MemberRow.GetRow(DB, MemberId)
        Dim dbMemberBilling As MemberAddressRow = MemberAddressRow.GetDefaultBillingRow(DB, MemberId)
        txtMemberName.Text = "<b>" + Core.BuildFullName(dbMemberBilling.FirstName, dbMemberBilling.MiddleInitial, dbMemberBilling.LastName) + " (" + dbMember.Username + ")</b>"
        lnkBack.HRef = "/admin/members/view.aspx?MemberId=" & MemberId & "&" & GetPageParams(FilterFieldType.All)
        dtItems = Wishlist.GetWishlistItems(DB, MemberId)
        If dtItems.Rows.Count = 0 Then
            pnlEmptyWishlist.Visible = True
            pnlWishlist.Visible = False
            Exit Sub
        End If
        If Not IsPostBack Then
            BindData()
        End If
    End Sub

    Private Sub BindData()
        rptWishlist.DataSource = dtItems
        rptWishlist.DataBind()
    End Sub

    Private Sub rptWishlist_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptWishlist.ItemDataBound
        If Not e.Item.ItemType = ListItemType.AlternatingItem AndAlso Not e.Item.ItemType = ListItemType.Item Then
            Exit Sub
        End If
        ItemCount += 1

        Dim img As HtmlImage = e.Item.FindControl("img")
        img.Alt = e.Item.DataItem("ItemName")
        If IsDBNull(e.Item.DataItem("Image")) Then
            img.Src = "/images/spacer.gif"
        Else
            img.Src = "/assets/item/cart/" & e.Item.DataItem("Image")
        End If
        img.Width = SysParam.GetValue(DB, "StoreItemImageCartWidth")
        img.Height = SysParam.GetValue(DB, "StoreItemImageCartHeight")
        Dim tdImage As HtmlTableCell = e.Item.FindControl("tdImage")
        tdImage.Width = img.Width

        Dim tdDetails1 As HtmlTableCell = e.Item.FindControl("tdDetails1")
        Dim tdDetails2 As HtmlTableCell = e.Item.FindControl("tdDetails2")
        Dim tdItemPrice As HtmlTableCell = e.Item.FindControl("tdItemPrice")
        Dim tdPrice As HtmlTableCell = e.Item.FindControl("tdPrice")

        If ItemCount = dtItems.Rows.Count Then
            tdDetails1.Attributes.Add("class", "bdrbottom bdrleft")
            tdDetails2.Attributes.Add("class", "bdrbottom")
            tdItemPrice.Attributes.Add("class", "bdrbottom")
            tdPrice.Attributes.Add("class", "bdrbottom bdrright")
        End If
        Dim txtQty As TextBox = e.Item.FindControl("txtQty")

        Dim spanAttributes As HtmlGenericControl = e.Item.FindControl("spanAttributes")
        spanAttributes.InnerHtml = Wishlist.GetAttributeText(DB, e.Item.DataItem("WishlistItemId"))
    End Sub

    Private Sub rptCart_ItemCommand(ByVal sender As Object, ByVal e As RepeaterCommandEventArgs) Handles rptWishlist.ItemCommand
        Select Case e.CommandName
            Case "Remove"
                Try
                    DB.BeginTransaction()
                    Wishlist.DeleteWishlistItem(DB, MemberId, e.CommandArgument)
                    DB.CommitTransaction()

                    Response.Redirect("/admin/members/wishlist/default.aspx?MemberId=" & MemberId)
                Catch ex As SqlException
                    If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
                    AddError(ErrHandler.ErrorText(ex))
                Catch ex As ApplicationException
                    If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
                    AddError(ex.Message)
                End Try
        End Select
    End Sub

    Private Sub btnUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnUpdate.Click
        If Not IsValid Then Exit Sub
        If UpdateWishlist() Then Response.Redirect("/admin/members/wishlist/default.aspx?MemberId=" & MemberId & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnSend_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSend.Click
        If Not IsValid Then Exit Sub
        If UpdateWishlist() Then Response.Redirect("/admin/members/wishlist/send.aspx?MemberId=" & MemberId & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Private Function UpdateWishlist() As Boolean
        Try
            DB.BeginTransaction()
            For Each item As RepeaterItem In rptWishlist.Items
                Dim txtQty As TextBox = item.FindControl("txtQty")
                Dim WishlistItemId As Integer = CType(item.FindControl("lnkRemove"), LinkButton).CommandArgument

                Dim Quantity As Integer = txtQty.Text
                If Quantity = 0 Then
                    Wishlist.DeleteWishlistItem(DB, MemberId, WishlistItemId)
                Else
                    Wishlist.UpdateQuantity(DB, MemberId, WishlistItemId, Quantity)
                End If
            Next
            DB.CommitTransaction()
            Return True
        Catch ex As ApplicationException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ex.Message)
        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Function
End Class