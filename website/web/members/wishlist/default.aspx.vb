Imports Components
Imports DataLayer
Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationManager

Public Class Member_Wishlist_Default
    Inherits SitePage

    Private MemberId As Integer
    Private OrderId As Integer
    Private dtItems As DataTable
    Private ItemCount As Integer

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureMemberAccess()
        MemberId = IIf(Session("MemberId") Is Nothing, 0, Session("MemberId"))
        OrderId = ShoppingCart.GenerateOrRetrieveOrder(DB)

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
        If Not Session("LastDepartmentId") = Nothing Then
            trLastDepartmentId.Visible = True
            Dim dbDepartment As StoreDepartmentRow = StoreDepartmentRow.GetRow(DB, Session("LastDepartmentId"))
            lnkLastDepartment.InnerHtml = dbDepartment.Name
            If dbDepartment.CustomURL = String.Empty Then
                Dim PageName As String = "default.aspx"
                If dbDepartment.ParentId = StoreDepartmentRow.GetDefaultDepartmentId(DB) Then
                    PageName = "main.aspx"
                End If
                lnkLastDepartment.HRef = "/store/" & PageName & "?DepartmentId=" & dbDepartment.DepartmentId
            Else
                lnkLastDepartment.HRef = dbDepartment.CustomURL
            End If
        End If
        If Not Session("LastBrandId") = Nothing Then
            trLastBrandId.Visible = True
            Dim dbBrand As StoreBrandRow = StoreBrandRow.GetRow(DB, Session("LastBrandId"))
            lnkLastBrand.InnerHtml = dbBrand.Name
            If dbBrand.CustomURL = String.Empty Then
                lnkLastBrand.HRef = "/store/brand.aspx?BrandId=" & dbBrand.BrandId
            Else
                lnkLastBrand.HRef = dbBrand.CustomURL
            End If
        End If
        If Not Session("LastItemId") = Nothing Then
            trLastItemId.Visible = True
            Dim dbItem As StoreItemRow = StoreItemRow.GetRow(DB, Session("LastItemId"))
            lnkLastItem.InnerHtml = dbItem.ItemName
            If dbItem.CustomURL = String.Empty Then
                lnkLastItem.HRef = "/store/item.aspx?ItemId=" & dbItem.BrandId
            Else
                lnkLastItem.HRef = dbItem.CustomURL
            End If
        End If
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

        Dim lnk1 As HtmlAnchor = e.Item.FindControl("lnk1")
        Dim lnk2 As HtmlAnchor = e.Item.FindControl("lnk2")
        Dim lnk3 As HtmlAnchor = e.Item.FindControl("lnk3")
        Dim CustomURL As String = IIf(IsDBNull(e.Item.DataItem("CustomURL")), String.Empty, e.Item.DataItem("CustomURL"))
        Dim lnk As String = String.Empty
        Dim qs As URLParameters = New URLParameters()
        If Not CustomURL = String.Empty Then
            lnk = CustomURL
        Else
            lnk = "/store/item.aspx"
            qs.Add("ItemId", e.Item.DataItem("ItemId"))
        End If
        lnk = AppSettings("GlobalRefererName") & lnk
        qs.Add("WishlistItemId", e.Item.DataItem("WishlistItemId"))
        If Not IsDBNull(e.Item.DataItem("BrandId")) Then qs.Add("BrandId", e.Item.DataItem("BrandId"))

        lnk1.HRef = lnk & qs.ToString()
        lnk2.HRef = lnk & qs.ToString()
        lnk3.HRef = lnk & qs.ToString()

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

                    Response.Redirect("/members/wishlist/default.aspx")
                Catch ex As SqlException
                    If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
                    AddError(ErrHandler.ErrorText(ex))
                Catch ex As ApplicationException
                    If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
                    AddError(ex.Message)
                End Try
            Case "Move"
                Try
                    DB.BeginTransaction()
                    ShoppingCart.Add2CartFromWislist(DB, OrderId, MemberId, e.CommandArgument)
                    Wishlist.DeleteWishlistItem(DB, MemberId, e.CommandArgument)
                    DB.CommitTransaction()

                    Response.Redirect(AppSettings("GlobalRefererName") & "/store/cart.aspx")

                Catch ex As ApplicationException
                    If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
                    AddError(ex.Message)
                Catch ex As SqlException
                    If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
                    AddError(ErrHandler.ErrorText(ex))
                End Try
        End Select
    End Sub

    Private Sub btnUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnUpdate.Click
        If Not IsValid Then Exit Sub
        If UpdateWishlist() Then Response.Redirect("/members/wishlist/default.aspx")
    End Sub

    Protected Sub btnSend_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSend.Click
        If Not IsValid Then Exit Sub
        If UpdateWishlist() Then Response.Redirect("/members/wishlist/send.aspx")
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