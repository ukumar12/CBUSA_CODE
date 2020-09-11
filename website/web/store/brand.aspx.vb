Imports Components
Imports DataLayer

Partial Class store_brand
    Inherits SitePage

    Private Filter As New DepartmentFilterField
    Private BrandCounter As Integer = 0
    Private ItemsCollectionCount As Integer = 0
    Private ItemsCollection As StoreItemCollection
	Private BrandId As Integer = 0
    Dim dbStoreBrand As StoreBrandRow

    Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
		If Not Request("pg") = String.Empty Then MetaRobots = "<meta name=""robots"" content=""noindex,follow"">"

		BrandId = IIf(IsNumeric(Request("BrandId")), Request("BrandId"), 0)
		dbStoreBrand = StoreBrandRow.GetRow(DB, BrandId)

        hTitle.InnerText = dbStoreBrand.Name

        Session("LastBrandId") = dbStoreBrand.BrandId

        BindBrands()
        BindItems()

        ltlBreadcrumb.Text = StoreBrandRow.DisplayBreadCrumb(DB, dbStoreBrand.BrandId, False)
    End Sub

    Private Sub BindBrands()
        If Not dbStoreBrand.BrandId = 0 Then
            Exit Sub
        End If
        Dim dtBrands As DataTable = StoreBrandRow.GetActiveBrands(DB)
        BrandCounter = dtBrands.Rows.Count
        pnlBrands.Visible = (BrandCounter > 0)
        rptBrands.DataSource = dtBrands
        rptBrands.DataBind()
    End Sub

    Private Sub rptBrands_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptBrands.ItemDataBound
        If Not e.Item.ItemType = ListItemType.Item AndAlso Not e.Item.ItemType = ListItemType.AlternatingItem Then
            Exit Sub
        End If
        Dim lnkBrand As HtmlAnchor = e.Item.FindControl("lnkBrand")
        If IsDBNull(e.Item.DataItem("CustomURL")) Then
            lnkBrand.HRef = "/store/brand.aspx?BrandId=" & e.Item.DataItem("BrandId")
        Else
            lnkBrand.HRef = e.Item.DataItem("CustomURL")
        End If

        Dim imgBrand As HtmlImage = e.Item.FindControl("imgBrand")
        If IsDBNull(e.Item.DataItem("Logo")) Then
            imgBrand.Src = "/images/spacer.gif"
            imgBrand.Width = SysParam.GetValue(DB, "StoreBrandLogoThumbnailWidth")
            imgBrand.Height = SysParam.GetValue(DB, "StoreBrandLogoThumbnailHeight")
        Else
            imgBrand.Src = "/assets/brand/thumbnail/" & e.Item.DataItem("Logo")
            imgBrand.Width = e.Item.DataItem("ThumbnailWidth")
            imgBrand.Height = e.Item.DataItem("ThumbnailHeight")
            imgBrand.Style.Item("margin-top") = CInt((SysParam.GetValue(DB, "StoreBrandLogoThumbnailHeight") - e.Item.DataItem("ThumbnailHeight")) / 2) & "px"
            imgBrand.Style.Item("margin-bottom") = imgBrand.Style.Item("margin-top")
        End If

        Dim divSpacer As HtmlGenericControl = CType(e.Item.FindControl("divSpacer"), HtmlGenericControl)
        divSpacer.Visible = (e.Item.ItemIndex Mod 4 = 3)
        If e.Item.ItemIndex = BrandCounter - 1 Then divSpacer.Visible = True
    End Sub

    Public Sub BindItems()
		If BrandId <= 0 Then
            Exit Sub
        End If

        Filter.SortBy = Request("sort")
        Filter.SortOrder = Request("dir")
		Filter.MaxPerPage = IIf(IsNumeric(Request("perpage")), Request("perpage"), IIf(Not Request("F_All") = String.Empty, -1, 12))
		Filter.pg = IIf(IsNumeric(Request("pg")), Request("pg"), 1)
        Filter.IncludeItemsFromSubdepartments = False
        Filter.BrandId = dbStoreBrand.BrandId

        NavigatorTop.NofRecords = ItemsCollectionCount
        NavigatorBottom.NofRecords = ItemsCollectionCount
        NavigatorTop.MaxPerPage = Filter.MaxPerPage
        NavigatorBottom.MaxPerPage = Filter.MaxPerPage
		NavigatorTop.Pg = Filter.pg
		NavigatorBottom.Pg = Filter.pg
        NavigatorTop.URL = Core.GetURLOnly(Request.RawUrl)
        NavigatorBottom.URL = Core.GetURLOnly(Request.RawUrl)
        If Not dbStoreBrand.CustomURL = String.Empty Then
            NavigatorTop.Exclude = "brandid"
            NavigatorBottom.Exclude = "brandid"
        End If
        ItemsCollection = StoreItemRow.GetActiveItems(DB, Filter)
        ItemsCollectionCount = StoreItemRow.GetActiveItemsCount(DB, Filter)
        pnlItems.Visible = (ItemsCollectionCount > 0)
        rptItems.DataSource = ItemsCollection
        rptItems.DataBind()
    End Sub

    Private Sub rptItems_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptItems.ItemDataBound
        If Not e.Item.ItemType = ListItemType.Item AndAlso Not e.Item.ItemType = ListItemType.AlternatingItem Then
            Exit Sub
        End If
        Dim dbItem As StoreItemRow = CType(e.Item.DataItem, StoreItemRow)
        Dim lnkItem As HtmlAnchor = e.Item.FindControl("lnkItem")
        If dbItem.CustomURL = String.Empty Then
            lnkItem.HRef = "/store/item.aspx?BrandId=" & dbStoreBrand.BrandId & "&ItemId=" & dbItem.ItemId & "&" & GetPageParams()
        Else
            lnkItem.HRef = dbItem.CustomURL & "?BrandId=" & dbStoreBrand.BrandId
        End If

        Dim imgItem As HtmlImage = e.Item.FindControl("imgItem")
        If dbItem.Image = String.Empty Then
            imgItem.Src = "/images/spacer.gif"
            imgItem.Width = SysParam.GetValue(DB, "StoreItemImageThumbnailWidth")
            imgItem.Height = SysParam.GetValue(DB, "StoreItemImageThumbnailHeight")
        Else
            imgItem.Src = "/assets/item/thumbnail/" & dbItem.Image
            imgItem.Width = dbItem.ThumbnailWidth
            imgItem.Height = dbItem.ThumbnailHeight
            imgItem.Style.Item("margin-top") = CInt((SysParam.GetValue(DB, "StoreItemImageThumbnailHeight") - dbItem.ThumbnailHeight) / 2) & "px"
            imgItem.Style.Item("margin-bottom") = imgItem.Style.Item("margin-top")
        End If

        Dim ltlPrice As HtmlGenericControl = e.Item.FindControl("ltlPrice")
        If dbItem.IsOnSale Then
            ltlPrice.InnerHtml = FormatCurrency(dbItem.Price)
        Else
            ltlPrice.InnerHtml = "<strike>" & FormatCurrency(dbItem.Price) & "</strike> <span class=""red"">" & FormatCurrency(dbItem.SalePrice) & "</span>"
        End If

        Dim divSpacer As HtmlGenericControl = CType(e.Item.FindControl("divSpacer"), HtmlGenericControl)
        divSpacer.Visible = (e.Item.ItemIndex Mod 4 = 3)
        If e.Item.ItemIndex = ItemsCollectionCount - 1 Then divSpacer.Visible = True
    End Sub

End Class
