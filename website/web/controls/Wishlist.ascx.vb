Imports Components
Imports DataLayer
Imports System.Data.SqlClient

Partial Class WishlistCtrl
    Inherits BaseControl

    Public Property EditMode() As Boolean
        Get
            Return IIf(ViewState("EditMode") Is Nothing, False, ViewState("EditMode"))
        End Get
        Set(ByVal value As Boolean)
            ViewState("EditMode") = value
        End Set
    End Property

    Public Property MemberId() As Integer
        Get
            Return IIf(ViewState("MemberId") Is Nothing, 0, ViewState("MemberId"))
        End Get
        Set(ByVal value As Integer)
            ViewState("MemberId") = value
        End Set
    End Property

    Private ItemCount As Integer
    Private dtItems As DataTable

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
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

        Dim lnk1 As HtmlAnchor = e.Item.FindControl("lnk1")
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
        qs.Add("b", e.Item.DataItem("WishlistItemId"))

        lnk1.HRef = lnk & qs.ToString()
        lnk3.HRef = lnk & qs.ToString()

        Dim tdBuy As HtmlTableCell = e.Item.FindControl("tdBuy")
        Dim tdDetails1 As HtmlTableCell = e.Item.FindControl("tdDetails1")
        Dim tdDetails2 As HtmlTableCell = e.Item.FindControl("tdDetails2")
        Dim tdItemPrice As HtmlTableCell = e.Item.FindControl("tdItemPrice")
        Dim tdPrice As HtmlTableCell = e.Item.FindControl("tdPrice")

        If ItemCount = dtItems.Rows.Count Then
            tdBuy.Attributes.Add("style", "border-bottom:1px solid #999999;")
            tdDetails1.Attributes.Add("style", "border-bottom:1px solid #999999;")
            tdDetails2.Attributes.Add("style", "border-bottom:1px solid #999999;")
            tdItemPrice.Attributes.Add("style", "border-bottom:1px solid #999999;")
            tdPrice.Attributes.Add("style", "border-bottom:1px solid #999999;")
        End If
        Dim txtQty As TextBox = e.Item.FindControl("txtQty")

        Dim spanAttributes As HtmlGenericControl = e.Item.FindControl("spanAttributes")
        spanAttributes.InnerHtml = Wishlist.GetAttributeText(DB, e.Item.DataItem("WishlistItemId"))
    End Sub

    Protected Sub rptWishlist_ItemCommand(ByVal sender As Object, ByVal e As RepeaterCommandEventArgs) Handles rptWishlist.ItemCommand
        Dim dbWishlistItem As MemberWishlistItemRow = MemberWishlistItemRow.GetRow(DB, e.CommandArgument)
        If dbWishlistItem.MemberId <> MemberId Then
            Exit Sub
        End If
        Dim dbItem As StoreItemRow = StoreItemRow.GetRow(DB, dbWishlistItem.ItemId)
        Dim lnk As String = String.Empty
        If Not dbItem.CustomURL = String.Empty Then
            lnk = dbItem.CustomURL
        Else
            lnk = "/store/item.aspx"
        End If
        lnk &= "?b=" & dbWishlistItem.WishlistItemId
        Response.Redirect(lnk)
    End Sub
End Class
