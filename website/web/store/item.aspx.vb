Imports Components
Imports DataLayer
Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationManager

Partial Class store_item
    Inherits SitePage

    Protected OrderId As Integer
    Protected MemberId As Integer
    Protected OrderItemId As Integer
    Protected dbItem As StoreItemRow
    Protected DepartmentId As Integer
    Protected BrandId As Integer
    Protected ItemId As Integer
    Protected WishlistItemIdAdd As Integer
    Protected WishlistItemId As Integer
	Protected EnableInventoryManagement As Boolean
	Protected EnableAttributeInventoryManagement As Boolean
	Protected AttributeDisplayMode As String
    Protected IsAttributes As Boolean = False
	Protected dbDepartment As StoreDepartmentRow
	Protected dtAlternates As DataTable
	Const AlternateImagePageSize As Integer = 4
	Protected AlternateImageCount As Integer

	Protected ReadOnly Property RecipientDropDown() As Object
		Get
			If AttributeDisplayMode = "AdminDriven" Then
				Return ctrlRecipientsDropDown
			Else
				Return ctrlRecipientsDropDownTable
			End If
		End Get
	End Property

	Public Property AltImagePage() As Integer
		Get
			If ViewState("AltImagePage") = Nothing Then ViewState("AltImagePage") = 1
			Return ViewState("AltImagePage")
		End Get
		Set(ByVal value As Integer)
			ViewState("AltImagePage") = value
		End Set
	End Property

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		DepartmentId = IIf(IsNumeric(Request("DepartmentId")), Request("DepartmentId"), 0)
		BrandId = IIf(IsNumeric(Request("BrandId")), Request("BrandId"), 0)
		OrderItemId = IIf(IsNumeric(Request("OrderItemId")), Request("OrderItemId"), 0)
		ItemId = IIf(IsNumeric(Request("ItemId")), Request("ItemId"), 0)
		OrderId = IIf(Session("OrderId") Is Nothing, 0, Session("OrderId"))
		MemberId = IIf(Session("MemberId") Is Nothing, 0, Session("MemberId"))
		WishlistItemId = IIf(IsNumeric(Request("WishlistItemId")), Request("WishlistItemId"), 0)
		WishlistItemIdAdd = IIf(IsNumeric(Request("b")), Request("b"), 0)

		EnableInventoryManagement = SysParam.GetValue(DB, "EnableInventoryManagement")
		EnableAttributeInventoryManagement = SysParam.GetValue(DB, "EnableAttributeInventoryManagement")

		If SysParam.GetValue(DB, "MultipleShipToEnabled") = False Then
			trShipTo.Visible = False
		End If

		ctrlItemAttributes.ltlPrice = ltlPrice
		ctrlItemAttributes.ltlPriceSale = ltlPriceSale
		ctrlItemAttributes.ltlSalePrice = ltlSalePrice
		ctrlItemAttributes.ProductImage = imgProduct
		ctrlItemAttributes.ProductImageEnlarge = imgEnlargedImage
		ctrlItemAttributes.mvAddToCart = mvAddToCart
		ctrlItemAttributes.ltlBackorder = ltlBackorder
		ctrlItemAttributes.EmailMe = ctrlEmailMeAtt

		If Not IsPostBack Then ctrlItemAttributes.Values = Nothing
		If OrderItemId > 0 Then
			'If item is being updated, then initialize fields
			Dim dbOrderItem As StoreOrderItemRow = StoreOrderItemRow.GetRow(DB, OrderItemId)

			'make sure parameters are correct for the order.
			If OrderId > 0 And dbOrderItem.OrderId <> OrderId Then
				Response.Redirect("/store/cart.aspx", False)
			End If

			ItemId = dbOrderItem.ItemId
			DepartmentId = dbOrderItem.DepartmentId
			BrandId = dbOrderItem.BrandId

			If Not IsPostBack Then
				btnAdd2Cart.CommandName = "Update"
				txtQty.Text = dbOrderItem.Quantity
				ctrlItemAttributes.Values = dbOrderItem.GetItemAttributeCollection
				ctrlRecipientsDropDown.RecipientId = dbOrderItem.RecipientId
				ctrlRecipientsDropDownTable.RecipientId = dbOrderItem.RecipientId
			End If
		ElseIf WishlistItemId > 0 Then

			'If item is being updated, then initialize fields
			Dim dbWishlistItem As MemberWishlistItemRow = MemberWishlistItemRow.GetRow(DB, WishlistItemId)

			'Make sure parameters are correct for member
			If MemberId <> 0 And dbWishlistItem.MemberId <> MemberId Then
				Response.Redirect("/members/wishlist/", False)
			End If

			ItemId = dbWishlistItem.ItemId
			If Not IsPostBack Then
				btnAdd2Cart.CommandName = "Add"
				btnAdd2Wishlist.CommandName = "Update"
				txtQty.Text = dbWishlistItem.Quantity
				ctrlItemAttributes.Values = dbWishlistItem.GetItemAttributeCollection
			End If

		ElseIf WishlistItemIdAdd > 0 Then
			'If item is being updated, then initialize fields
			Dim dbWishlistItem As MemberWishlistItemRow = MemberWishlistItemRow.GetRow(DB, WishlistItemIdAdd)

			'Make sure parameters are correct for member
			If MemberId <> 0 And dbWishlistItem.MemberId <> MemberId Then
				Response.Redirect("/members/wishlist/", False)
			End If

			ItemId = dbWishlistItem.ItemId
			If Not IsPostBack Then
				btnAdd2Cart.CommandName = "Add"
				btnAdd2Wishlist.CommandName = "Add"
				txtQty.Text = dbWishlistItem.Quantity
				ctrlItemAttributes.Values = dbWishlistItem.GetItemAttributeCollection
			End If
		Else
            btnAdd2Cart.CommandName = "Add"
            If Not IsPostBack Then txtQty.Text = "1"
		End If
		dbItem = StoreItemRow.GetRow(DB, ItemId)
		dbItem.InsertRecentlyViewedItem(DepartmentId, BrandId, MemberId, Session.SessionID)
		IsAttributes = DB.ExecuteScalar("SELECT TOP 1 IsAttributes FROM StoreItemTemplate WHERE TemplateId = " & dbItem.TemplateId)

		AlternateImageCount = StoreItemRow.GetAlternateImagesCount(DB, ItemId)
		CreateEnlargeImageControls()

		dbDepartment = StoreDepartmentRow.GetRow(DB, DepartmentId)
		BindPager()

		ctrlEmailMe.ItemId = ItemId

		If dbItem.DisplayMode = "TableLayout" OrElse (dbItem.DisplayMode = Nothing AndAlso StoreItemTemplateRow.GetRow(DB, dbItem.TemplateId).DisplayMode = "TableLayout") Then
			rvQty.Enabled = False
			AttributeDisplayMode = "TableLayout"
			ctrlItemAttributesTable.ItemId = dbItem.ItemId
		Else
			rvQty.Enabled = True
			AttributeDisplayMode = "AdminDriven"
			ctrlItemAttributes.ItemId = dbItem.ItemId
		End If

		If Not EnableInventoryManagement OrElse dbItem.InventoryQty > dbItem.InventoryActionThreshold OrElse dbItem.InventoryAction = "Backorder" OrElse IsAttributes Then
			mvInventory.SetActiveView(vAddToCart)
			If EnableInventoryManagement AndAlso dbItem.InventoryQty <= dbItem.InventoryActionThreshold AndAlso dbItem.InventoryAction = "Backorder" Then
				If Not EnableInventoryManagement OrElse dbItem.InventoryQty > dbItem.InventoryActionThreshold OrElse dbItem.InventoryAction = "Backorder" OrElse ctrlItemAttributes.Visible OrElse ctrlItemAttributesTable.Visible Then
					ltlBackorder.Text = "<p><strong>Backorder</strong>" & IIf(Not dbItem.BackorderDate = Nothing, "<br /><span class=""smaller"">Estimated Ship Date: " & dbItem.BackorderDate.ToShortDateString & "</span>", "") & "</p>"
				End If
			End If
			If mvAddToCart.GetActiveView Is Nothing Then mvAddToCart.SetActiveView(vQty)
		Else
			mvInventory.SetActiveView(vInventory)
		End If

		If IsPostBack Then
			Exit Sub
		End If

		If IsNumeric(Request("ItemNotifyId")) Then
			Dim dbNotify As StoreItemNotifyRow = StoreItemNotifyRow.GetRow(DB, Request("ItemNotifyId"))
			If dbNotify.ViewDate = Nothing Then
				dbNotify.ViewDate = Now
				dbNotify.Update()
			End If
			If Not dbNotify.ItemAttributeId = Nothing Then ctrlItemAttributes.Values = dbNotify.GetItemAttributeCollection(dbItem.ItemId, dbItem.TemplateId)
		End If

		If BrandId > 0 Then
			ltlBreadcrumb.Text = StoreBrandRow.DisplayBreadCrumb(DB, BrandId, False, dbItem.ItemName)
		Else
			ltlBreadcrumb.Text = StoreDepartmentRow.DisplayBreadCrumb(DB, DepartmentId, True, dbItem.ItemName)
		End If

		'If item is not longer active, then redirect to home page
		If Not dbItem.IsActive Then
			Response.Redirect("/")
		End If

		ltlPrice.Text = FormatCurrency(dbItem.Price)
		ltlPriceSale.Text = FormatCurrency(dbItem.Price)
		ltlSalePrice.Text = FormatCurrency(dbItem.SalePrice)

		'Overwrite smartbug URL
		SmartBugUrl = "/admin/default.aspx?FrameURL=" & Server.UrlEncode("/admin/store/items/edit.aspx?ItemId=" & dbItem.ItemId)

		'Overwrite default PageTitle and meta tags
		PageTitle = dbItem.PageTitle
		If Not dbItem.MetaKeywords = String.Empty Then MetaKeywords = dbItem.MetaKeywords
		If Not dbItem.MetaDescription = String.Empty Then MetaDescription = dbItem.MetaDescription

		ctrlShareAndEnjoy.Title = PageTitle

		'Save last item, department and brand (used in the view cart page)
		Session("LastItemId") = ItemId
		Session("LastDepartmentId") = DepartmentId
		Session("LastBrandId") = BrandId

		ctrlRecipientsDropDown.MemberId = IIf(Session("MemberId") Is Nothing, 0, Session("MemberId"))
		ctrlRecipientsDropDownTable.MemberId = IIf(Session("MemberId") Is Nothing, 0, Session("MemberId"))
		ctrlRecipientsDropDown.OrderId = IIf(Session("OrderId") Is Nothing, 0, Session("OrderId"))
		ctrlRecipientsDropDownTable.OrderId = IIf(Session("OrderId") Is Nothing, 0, Session("OrderId"))
		ctrlRelatedItems.ItemId = ItemId
		ctrlItemAttributes.ItemId = ItemId

		If Not ctrlItemAttributes.ItemId = 0 Then imgProduct.Src = "/assets/item/regular/" & dbItem.Image
	End Sub

    Protected Sub btnAdd2Cart_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnAdd2Cart.Click, btnAdd2CartTop.Click, btnAdd2CartBottom.Click
        Page.Validate()

        If Not IsValid Then Exit Sub

        'Retrieve attribute values
        Dim col As ItemAttributeCollection
        If AttributeDisplayMode = "AdminDriven" Then
            If Not IsNumeric(txtQty.Text) OrElse CInt(txtQty.Text) <= 0 Then
                AddError("Quantity field value must be greater than 0")
                Exit Sub
            End If

            col = ctrlItemAttributes.Values
        ElseIf AttributeDisplayMode = "TableLayout" Then
            If Not IsValidTable() Then Exit Sub
        End If

        'Generate unique OrderId if nothing (keep outside of the transaction)
        OrderId = ShoppingCart.GenerateOrRetrieveOrder(DB)
        Try
            DB.BeginTransaction()

            If btnAdd2Cart.CommandName = "Update" Then
                ShoppingCart.DeleteOnlyOrderItem(DB, OrderId, OrderItemId)
            End If

            If AttributeDisplayMode = "AdminDriven" Then
                OrderItemId = ShoppingCart.Add2Cart(DB, OrderId, MemberId, dbItem.ItemId, txtQty.Text, DepartmentId, BrandId, ctrlRecipientsDropDown.SelectedValue, col)
            Else
                AddItems()
            End If

            ShoppingCart.DeleteNotUsedRecipients(DB, OrderId)
            ShoppingCart.RecalculateShoppingCart(DB, OrderId, False)

            DB.CommitTransaction()

            Dim dbOrderItem As StoreOrderItemRow = StoreOrderItemRow.GetRow(DB, OrderItemId)
            Dim dbOrderSummary As StoreOrderSummary = StoreOrderRow.GetOrderSummary(DB, OrderId)
            Dim strHtml As String = "<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%""><tr><td>"
            strHtml &= "<div class=""addedCartHdr"">This item has been added to your cart</div>"
            strHtml &= "</td></tr></table>"
            strHtml &= "<table cellspacing=""0"" cellpadding=""0"" border=""0"" style=""width:325px; margin:8px 0;"">"
            strHtml &= "<tr>"
            strHtml &= "<td style=""vertical-align:top;padding-right:8px;"">"
            strHtml &= "	<img src=""/assets/item/cart/" & IIf(dbOrderItem.Image = String.Empty, "na.jpg", dbOrderItem.Image) & """ /><br />"
            strHtml &= "</td>"
            strHtml &= "<td class=""aligntop"" style=""width:100%"">"
            strHtml &= "	<div style=""padding-left:6px;"">"
            strHtml &= "		<div class=""bold"">" & dbOrderItem.ItemName & "</div>"
            If Not col Is Nothing AndAlso col.Count > 0 Then
                strHtml &= "<div class=""smaller"">"
                For i As Integer = 0 To col.Count - 1
                    strHtml &= col(i).AttributeName & "=" & col(i).AttributeValue & "<br />"
                Next
                strHtml &= "</div>"
            End If
            strHtml &= "	</div>"
            strHtml &= "	<div class=""smaller"">"
            strHtml &= "		<div style=""padding:4px 6px;"">Quantity <span style=""padding:2px; background-color:#f4f4f4;"">" & txtQty.Text & "</span> x " & IIf(dbOrderItem.IsOnSale, FormatCurrency(dbOrderItem.SalePrice), FormatCurrency(dbOrderItem.Price)) & "</div>"
            strHtml &= "		<div style=""padding:4px 6px;""><span class=""bold"">Total</span> " & IIf(dbOrderItem.IsOnSale, FormatCurrency(dbOrderItem.SalePrice * txtQty.Text), FormatCurrency(dbOrderItem.Price * txtQty.Text)) & "</div>"
            strHtml &= "		<div style=""padding:4px 6px;""><span class=""bold"">Shopping Cart Total</span> " & FormatCurrency(dbOrderSummary.SubTotal) & "</div>"
            strHtml &= "	</div>"
            strHtml &= "</td>"
            strHtml &= "</tr>"
            strHtml &= "</table>"

            If AttributeDisplayMode = "TableLayout" Then
                ScriptManager.RegisterStartupScript(Me, Me.GetType, "ItemAdded", "window.location='/store/cart.aspx'", True)
            Else
                ScriptManager.RegisterStartupScript(Me, Me.GetType, "ItemAdded", "document.getElementById('spanCartQuantity').innerHTML='" & dbOrderSummary.Quantity & "';document.getElementById('spanCartSubTotal').innerHTML='" & FormatCurrency(dbOrderSummary.SubTotal) & "';document.getElementById('divAddedToCart').innerHTML = '" & strHtml.Replace("'", "\'") & "';ypSlideOutCart.showCart('cart');", True)
            End If

            'Response.Redirect("/store/cart.aspx")

        Catch ex As ApplicationException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ex.Message)
        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

	Protected Sub btnAdd2Wishlist_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnAdd2Wishlist.Click, btnAdd2WishlistTop.Click, btnAdd2WishlistBottom.Click
		Page.Validate()

		If Not IsValid Then Exit Sub

		'Retrieve attribute values
		Dim sAttributes As String = String.Empty
		Dim col As ItemAttributeCollection
		If AttributeDisplayMode = "AdminDriven" Then
			If Not IsNumeric(txtQty.Text) OrElse CInt(txtQty.Text) <= 0 Then
				AddError("Quantity field value must be greater than 0")
				Exit Sub
			End If

			col = ctrlItemAttributes.Values
			For Each a As ItemAttribute In ctrlItemAttributes.Values
				sAttributes &= IIf(sAttributes = String.Empty, "", ",") & a.ItemAttributeId
			Next
		ElseIf AttributeDisplayMode = "TableLayout" Then
			If Not IsValidTable() Then Exit Sub
		End If

		If Not IsLoggedIn() Then
			Response.Redirect(AppSettings("GlobalSecureName") & "/members/login.aspx?redir=" & Server.UrlEncode(Request.RawUrl) & "&ItemId=" & dbItem.ItemId & "&Qty=" & txtQty.Text & "&ItemAttributeIds=" & sAttributes)
		End If

		Try
			DB.BeginTransaction()
			If btnAdd2Wishlist.CommandName = "Update" Then
				Wishlist.DeleteWishlistItem(DB, MemberId, WishlistItemId)
			End If

			If AttributeDisplayMode = "AdminDriven" Then
				Wishlist.Add2Wishlist(DB, Session("MemberId"), dbItem.ItemId, txtQty.Text, col)
			Else
				AddItems(True)
			End If

			DB.CommitTransaction()
			Response.Redirect("/members/wishlist/default.aspx")
		Catch ex As ApplicationException
			If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
			AddError(ex.Message)
		Catch ex As SqlException
			If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
			AddError(ErrHandler.ErrorText(ex))
		End Try
	End Sub

	Private Function IsValidTable() As Boolean
		Dim HasOne As Boolean = False

		For Each r As RepeaterItem In ctrlItemAttributesTable.AttributeRepeater.Items
			Dim txtQuantity As TextBox = r.FindControl("txtQty")
			If Not Trim(txtQuantity.Text) = String.Empty AndAlso (Not IsNumeric(txtQuantity.Text) OrElse CInt(txtQuantity.Text) <= 0) Then
				AddError("Please enter valid quantities.")
				Return False
			ElseIf IsNumeric(txtQuantity.Text) AndAlso CInt(txtQuantity.Text) > 0 Then
				HasOne = True
			End If
		Next

		If Not HasOne Then AddError("Please choose one or more items to add to your cart.")

		Return HasOne
	End Function

	Private Sub AddItems(Optional ByVal IsWishlist As Boolean = False)
		Dim dv As DataView = DB.GetDataTable("exec sp_GetAttributeTree " & dbItem.ItemId).DefaultView
		For Each r As RepeaterItem In ctrlItemAttributesTable.AttributeRepeater.Items
			Dim txtQuantity As TextBox = r.FindControl("txtQty")

			If Not Trim(txtQuantity.Text) = String.Empty Then
				Dim lblIds As Label = r.FindControl("lblIds")
				Dim Ids() As String = lblIds.Text.Split(",")

				Dim col As ItemAttributeCollection = New ItemAttributeCollection
				For Each s As String In Ids
					dv.RowFilter = "ItemAttributeId = " & DB.Number(s)
					If dv.Count > 0 Then
						Dim dr As DataRowView = dv(0)
						Dim attr As New ItemAttribute
						attr.AttributeType = IIf(IsDBNull(dr("AttributeType")), String.Empty, dr("AttributeType"))
						attr.AttributeName = IIf(IsDBNull(dr("AttributeName")), String.Empty, dr("AttributeName"))
						attr.AttributeValue = IIf(IsDBNull(dr("AttributeValue")), String.Empty, dr("AttributeValue"))
						attr.ImageName = IIf(IsDBNull(dr("ImageName")), String.Empty, dr("ImageName"))
						attr.ImageAlt = IIf(IsDBNull(dr("ImageAlt")), String.Empty, dr("ImageAlt"))
						attr.ParentAttributeId = IIf(IsDBNull(dr("ParentAttributeId")), Nothing, dr("ParentAttributeId"))
						attr.ProductAlt = IIf(IsDBNull(dr("ProductAlt")), String.Empty, dr("ProductAlt"))
						attr.ProductImage = IIf(IsDBNull(dr("ProductImage")), String.Empty, dr("ProductImage"))
						attr.Weight = IIf(IsDBNull(dr("Weight")), 0, dr("Weight"))
						attr.ItemAttributeId = IIf(IsDBNull(dr("ItemAttributeId")), 0, dr("ItemAttributeId"))
						attr.ItemId = dbItem.ItemId
						attr.Price = IIf(IsDBNull(dr("Price")), 0, dr("Price"))
						attr.SKU = IIf(IsDBNull(dr("SKU")), String.Empty, dr("SKU"))
						attr.SortOrder = IIf(IsDBNull(dr("SortOrder")), 0, dr("SortOrder"))
						attr.TemplateAttributeId = IIf(IsDBNull(dr("TemplateAttributeId")), 0, dr("TemplateAttributeId"))
						col.Add(attr)
					End If
				Next

				If IsWishlist Then
					Wishlist.Add2Wishlist(DB, Session("MemberId"), dbItem.ItemId, txtQuantity.Text, col)
				Else
					ShoppingCart.Add2Cart(DB, OrderId, MemberId, dbItem.ItemId, txtQuantity.Text, DepartmentId, BrandId, ctrlRecipientsDropDownTable.SelectedValue, col)
				End If
			End If
		Next
    End Sub

    Private Sub BindPager()
        Dim PrevURL As String = "", NextURL As String = "", ItemsCollectionCount As Integer, CurrentIndex As Integer
        GetNextPrevData(dbItem, dbDepartment, PrevURL, NextURL, ItemsCollectionCount, CurrentIndex)

        If ItemsCollectionCount > 0 And CurrentIndex > 0 Then
            ltlPaging.Text = "<table cellspacing=""0"" cellpadding=""0"" border=""0"" summary=""pagination""><tr><td class=""smallest blue"" style=""padding:0 2px 0 2px;"">"
            If PrevURL <> "" Then ltlPaging.Text &= "<a href=""" & PrevURL & """><img src=""/images/global/btn-arrow-left.gif"" width=""12"" height=""12"" style=""border-style:none"" alt=""Back"" /></a>"
            ltlPaging.Text &= "</td><td class=""smallest blue"" style=""padding:1px 20px 2px 0;"">"
            If PrevURL <> "" Then ltlPaging.Text &= "<a href=""" & PrevURL & """ class=""noul"">back</a>"
            ltlPaging.Text &= "</td><td class=""smallest blue bold"" style=""padding:1px 2px 2px 2px; background-color:#eef7e0;"">" & CurrentIndex & "</td>"
            ltlPaging.Text &= "<td class=""smallest blue"" style=""padding:1px 2px 2px 4px;"">of</td>"
            ltlPaging.Text &= "<td class=""smallest blue"" style=""padding:1px 2px 2px 2px;"">" & ItemsCollectionCount & "</td>"
            ltlPaging.Text &= "<td class=""smallest blue"" style=""padding:1px 0 2px 20px;"">"
            If NextURL <> "" Then ltlPaging.Text &= "<a href=""" & NextURL & """ class=""noul"">next</a>"
            ltlPaging.Text &= "</td><td class=""smallest blue"" style=""padding:0 2px 0 2px;"">"
            If NextURL <> "" Then ltlPaging.Text &= "<a href=""" & NextURL & """><img src=""/images/global/btn-arrow-right.gif"" width=""12"" height=""12"" style=""border-style:none"" alt=""Next"" /></a>"
            ltlPaging.Text &= "</td></tr></table>"
        Else
            ltlPaging.Text = ""
        End If
    End Sub

    Public Sub GetNextPrevData(ByVal dbCurrentItem As StoreItemRow, ByVal dbCurrentDepartment As StoreDepartmentRow, ByRef PrevURL As String, ByRef NextURL As String, ByRef ItemsCollectionCount As Integer, ByRef CurrentIndex As Integer)
        Dim dbMainLevelDepartment As StoreDepartmentRow = dbCurrentDepartment.GetMainLevelDepartment()
        Dim ItemsCollection As StoreItemCollection
        Dim iTmp As Integer = 0
        'Dim iCustomUrl As String = String.Empty
        Dim Filter As DepartmentFilterField = New DepartmentFilterField()
        Filter.DepartmentId = dbCurrentDepartment.DepartmentId
        Filter.MaxPerPage = -1

        Dim keyword As String = Request("keyword")
        If Not keyword = String.Empty Then
            Dim match As String = Request("match")
            If keyword = String.Empty Then
                'do nothing
            ElseIf match = "AND" Then
                keyword = Core.SplitSearchAND(keyword)
            ElseIf match = "OR" Then
                keyword = Core.SplitSearchOR(keyword)
            Else
                keyword = Core.DblQuote(keyword)
            End If
        End If

        Filter.Keyword = keyword
		Filter.BrandId = BrandId

        ItemsCollectionCount = StoreItemRow.GetActiveItemsCount(DB, Filter)
        ItemsCollection = StoreItemRow.GetActiveItems(DB, Filter)



        Dim qs As URLParameters = New URLParameters(System.Web.HttpContext.Current.Request.QueryString, "ItemId;isbn")

        For iLoop As Integer = 0 To ItemsCollection.Count - 1
            iTmp = ItemsCollection.Item(iLoop).ItemId
            If iTmp = dbCurrentItem.ItemId Then
                CurrentIndex = iLoop + 1
                If iLoop > 0 Then
                    If ItemsCollection.Item(iLoop - 1).CustomURL = String.Empty Then
                        qs.Add("ItemId", ItemsCollection.Item(iLoop - 1).ItemId)
                        PrevURL = GlobalRefererName & "/store/item.aspx" & qs.ToString
                    Else
                        PrevURL = ItemsCollection.Item(iLoop - 1).CustomURL & qs.ToString
                    End If
                    qs = New URLParameters(System.Web.HttpContext.Current.Request.QueryString, "ItemId;isbn")
                End If
                If iLoop < ItemsCollection.Count - 1 Then
                    If ItemsCollection.Item(iLoop + 1).CustomURL = String.Empty Then
                        qs.Add("ItemId", ItemsCollection.Item(iLoop + 1).ItemId)
                        NextURL = GlobalRefererName & "/store/item.aspx" & qs.ToString
                    Else
                        NextURL = ItemsCollection.Item(iLoop + 1).CustomURL & qs.ToString
                    End If
                    qs = New URLParameters(System.Web.HttpContext.Current.Request.QueryString, "ItemId;isbn")
                End If
            End If
        Next
    End Sub

#Region "Enlage Image Functionality"
	Private Sub lnkEnlarge_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkEnlarge.Click
		Dim Screen As HtmlGenericControl = Page.FindControl("divScreen")
		If Not Screen Is Nothing Then Screen.Visible = True
		divLarger.Visible = True
		BindAlternateImages()
		imgEnlargedImage.Src = imgProduct.Src.Replace("/regular/", "/large/")
	End Sub

	Private Sub lnkCloseWindow_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkCloseWindow.Click
		Dim Screen As HtmlGenericControl = Page.FindControl("divScreen")
		If Not Screen Is Nothing Then Screen.Visible = False
		divLarger.Visible = False
	End Sub

	Private Sub lnkViewDefault_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkViewDefault.Click
		imgEnlargedImage.Src = imgProduct.Src.Replace("/regular/", "/large/")
	End Sub

	Private Sub CreateEnlargeImageControls()
		Dim lb, lbPrev, lbNext As LinkButton
		Dim ltl As Literal
		Dim img As HtmlImage
		Dim iPages As Integer = Math.Ceiling(AlternateImageCount / AlternateImagePageSize)

		For i As Integer = 1 To AlternateImagePageSize
			lb = New LinkButton
			lb.ID = "ui" & i
			lb.CommandName = "UpdateImage"
			AddHandler lb.Command, AddressOf lb_Command

			img = New HtmlImage
			img.ID = "img" & i
			img.Style.Add("margin-bottom", "25px")
            img.Style.Add("border", "solid 1px #e6e6e6;")
            If AlternateImageCount = 0 Then img.Visible = False
			lb.Controls.Add(img)

			divAlternates.Controls.Add(lb)
			divAlternates.Controls.Add(New LiteralControl("<br />"))
		Next

		If iPages > 1 Then
			divAlternates.Controls.Add(New LiteralControl("<div class=""smaller"">"))
			For i As Integer = 1 To iPages
				ltl = New Literal
				ltl.ID = "ltl" & i
				ltl.Text = " <b>" & i & "</b> "
				divAlternates.Controls.Add(ltl)

				lb = New LinkButton
				lb.ID = "pg" & i
				lb.CommandName = "Page"
				lb.CommandArgument = i
				lb.Text = i
				AddHandler lb.Command, AddressOf lb_Command
				divAlternates.Controls.Add(lb)
			Next
			divAlternates.Controls.Add(New LiteralControl("</div>"))

			divAlternates.Controls.Add(New LiteralControl("<div class=""smallest"">"))
			ltl = New Literal
			ltl.ID = "ltlprev"
			ltl.Text = "<span style=""color:#999"">&laquo; Prev</span>"
			divAlternates.Controls.Add(ltl)

			lbPrev = New LinkButton
			lbPrev.ID = "lbprev"
			lbPrev.CommandName = "Page"
			lbPrev.Text = "&laquo; Prev"
			AddHandler lbPrev.Command, AddressOf lb_Command
			divAlternates.Controls.Add(lbPrev)
			divAlternates.Controls.Add(New LiteralControl(" "))

			ltl = New Literal
			ltl.ID = "ltlnext"
			ltl.Text = "<span style=""color:#999"">Next &raquo;</span>"
			divAlternates.Controls.Add(ltl)

			lbNext = New LinkButton
			lbNext.ID = "lbnext"
			lbNext.CommandName = "Page"
			lbNext.Text = "Next &raquo;"
			AddHandler lbNext.Command, AddressOf lb_Command
			divAlternates.Controls.Add(New LiteralControl(" "))
			divAlternates.Controls.Add(lbNext)

			divAlternates.Controls.Add(New LiteralControl("</div>"))
		End If
	End Sub

	Private Sub BindAlternateImages()
		dtAlternates = StoreItemRow.GetAlternateImages(DB, ItemId, AltImagePage * AlternateImagePageSize)

		Dim iPages As Integer = Math.Ceiling(AlternateImageCount / AlternateImagePageSize)
		Dim r As DataRow
		Dim lb, lbPrev, lbNext As LinkButton
		Dim ltl, ltlPrev, ltlNext As Literal
		Dim img As HtmlImage
		Dim ctr As Integer = 1

		For i As Integer = 1 To AlternateImagePageSize
			img = divAlternates.FindControl("img" & i)
			img.Visible = False
		Next

		If dtAlternates.Rows.Count > 0 Then
			For i As Integer = (AltImagePage - 1) * AlternateImagePageSize To dtAlternates.Rows.Count - 1
				r = dtAlternates.Rows(i)

				lb = divAlternates.FindControl("ui" & ctr)
				lb.CommandArgument = r("Image")

				img = lb.FindControl("img" & ctr)
				img.Src = "/assets/item/alternate/" & r("Image")
				img.Alt = Convert.ToString(r("ImageAltTag"))
				img.Visible = True

				ctr += 1
			Next
		Else
			divAlternates.InnerHtml = "<div class=""smallest"" style=""margin-top:10px;font-style:italic;width:100px;"">Sorry, no alternate images are available for this product</div>"
			Exit Sub
		End If

		If iPages > 1 Then
			lbPrev = divAlternates.FindControl("lbprev")
			lbNext = divAlternates.FindControl("lbnext")
			ltlPrev = divAlternates.FindControl("ltlprev")
			ltlNext = divAlternates.FindControl("ltlnext")

			ltlPrev.Visible = True
			ltlNext.Visible = True
			lbPrev.Visible = False
			lbNext.Visible = False
			For i As Integer = 1 To iPages
				lb = divAlternates.FindControl("pg" & i)
				ltl = divAlternates.FindControl("ltl" & i)
				If i = AltImagePage Then
					lb.Visible = False
					ltl.Visible = True
				Else
					lb.Visible = True
					ltl.Visible = False
				End If

				If AltImagePage = i - 1 Then
					lbNext.CommandArgument = i
					lbNext.Visible = True
					ltlNext.Visible = False
				ElseIf AltImagePage = i + 1 Then
					lbPrev.CommandArgument = i
					lbPrev.Visible = True
					ltlPrev.Visible = False
				End If
			Next
		End If
	End Sub

	Private Sub lb_Command(ByVal sender As Object, ByVal e As CommandEventArgs)
		Select Case e.CommandName
			Case "UpdateImage"
				imgEnlargedImage.Src = "/assets/item/alternate/large/" & e.CommandArgument
			Case "Page"
				AltImagePage = e.CommandArgument
				BindAlternateImages()
		End Select
	End Sub
#End Region

End Class
