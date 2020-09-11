Imports Components
Imports DataLayer
Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationManager

Partial Class cart
	Inherits SitePage

    Private OrderId As Integer
    Private MemberId As Integer
    Private SubTotal, Total As Double
    Private dtItems As DataTable
    Private dtRecipients As DataTable
    Private ItemCount As Integer

    Protected dbOrder As StoreOrderRow

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        OrderId = IIf(Session("OrderId") Is Nothing, 0, Session("OrderId"))
        MemberId = IIf(Session("MemberId") Is Nothing, 0, Session("MemberId"))
        Try
            DB.BeginTransaction()
            Dim ValidateErrorMessage As String = ShoppingCart.ValidateOrderItems(DB, OrderId)
            If Not ValidateErrorMessage = String.Empty Then
                AddError(ValidateErrorMessage)
            End If
            DB.CommitTransaction()
        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        Catch ex As ApplicationException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ex.Message)
        End Try

        If ShoppingCart.GetRecipients(DB, OrderId).Rows.Count = 0 Then
            pnlEmptyCart.Visible = True
            pnlCart.Visible = False
            Exit Sub
        End If

        dbOrder = StoreOrderRow.GetRow(DB, OrderId)
        If Not IsPostBack Then
            BindData()
        End If
    End Sub

    Private Sub BindData()

        If SysParam.GetValue(DB, "GiftWrappingEnabled") = 1 Then
            Dim trGiftWrapping As HtmlTableRow = FindControl("trGiftWrapping")
            Dim ltlGiftWrappingPrice As Literal = FindControl("ltlGiftWrappingPrice")

            ltlGiftWrappingPrice.Text = FormatCurrency(SysParam.GetValue(DB, "GiftWrapPrice"))
            trGiftWrapping.Visible = True
        End If

        If Not Session("MemberId") Is Nothing Then
            Dim trWishlist As HtmlTableRow = FindControl("trWishlist")
            trWishlist.Visible = True
        End If

        dtRecipients = ShoppingCart.GetOrderRecipients(DB, OrderId)
        dtItems = ShoppingCart.GetOrderItems(DB, OrderId)

        rptRecipients.DataSource = dtRecipients
        rptRecipients.DataBind()

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
                lnkLastItem.HRef = "/store/item.aspx?ItemId=" & dbItem.ItemId
            Else
                lnkLastItem.HRef = dbItem.CustomURL
            End If
        End If
        txtPromotionCode.Text = dbOrder.PromotionCode
        If Not dbOrder.PromotionCode = String.Empty Then ltlPromotionMessage.Text = StorePromotionRow.GetRowByCode(DB, dbOrder.PromotionCode).Message
    End Sub

    Protected Sub rptRecipients_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptRecipients.ItemCreated
        If Not e.Item.ItemType = ListItemType.AlternatingItem AndAlso Not e.Item.ItemType = ListItemType.Item Then
            Exit Sub
        End If
        Dim rptCart As Repeater = e.Item.FindControl("rptCart")
        AddHandler rptCart.ItemDataBound, AddressOf rptCart_ItemDataBound
        AddHandler rptCart.ItemCreated, AddressOf rptCart_ItemCreated
        AddHandler rptCart.ItemCommand, AddressOf rptCart_ItemCommand
    End Sub

    Protected Sub rptCart_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        If Not e.Item.ItemType = ListItemType.AlternatingItem AndAlso Not e.Item.ItemType = ListItemType.Item Then
            Exit Sub
        End If
        If IsPostBack Then
            Dim chkGiftWrap As CheckBox = e.Item.FindControl("chkGiftWrap")
            Dim divGiftWrap As HtmlGenericControl = e.Item.FindControl("divGiftWrap")
            divGiftWrap.Attributes.Add("style", IIf(chkGiftWrap.Checked, "display:block", "display:none"))
        End If
    End Sub

    Protected Sub rptRecipients_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptRecipients.ItemDataBound
        If Not e.Item.ItemType = ListItemType.AlternatingItem AndAlso Not e.Item.ItemType = ListItemType.Item Then
            Exit Sub
        End If

        Dim GiftWrappingEnabled As Boolean = (SysParam.GetValue(DB, "GiftWrappingEnabled") = 1)

        Dim tdQuantity As HtmlTableCell = e.Item.FindControl("tdQuantity")
        Dim tdGiftWrap As HtmlTableCell = e.Item.FindControl("tdGiftWrap")
        Dim trMultipleShipTo As HtmlTableRow = e.Item.FindControl("trMultipleShipTo")
        Dim trSingleShipTo As HtmlTableRow = e.Item.FindControl("trSingleShipTo")
        Dim trRecipientSummary As HtmlTableRow = e.Item.FindControl("trRecipientSummary")
        Dim trGiftWrappingBottom As HtmlTableRow = e.Item.FindControl("trGiftWrappingBottom")
        Dim trDiscountBottom As HtmlTableRow = e.Item.FindControl("trDiscountBottom")
        Dim trTotal As HtmlTableRow = e.Item.FindControl("trTotal")

        trMultipleShipTo.Visible = SysParam.GetValue(DB, "MultipleShipToEnabled") = 1
        trSingleShipTo.Visible = Not trMultipleShipTo.Visible

        trRecipientSummary.Visible = dtRecipients.Rows.Count > 1
        trGiftWrappingBottom.Visible = dtRecipients.Rows.Count > 1
        trDiscountBottom.Visible = dtRecipients.Rows.Count > 1
        trTotal.Visible = dtRecipients.Rows.Count > 1

        If e.Item.DataItem("GiftWrapping") = 0 Then
            trGiftWrappingBottom.Visible = False
        End If
        If e.Item.DataItem("Discount") = 0 Then
            trDiscountBottom.Visible = False
        End If
        If e.Item.DataItem("GiftWrapping") = 0 And e.Item.DataItem("Discount") = 0 Then
            trTotal.Visible = False
            Dim tdBottom1 As HtmlTableCell = e.Item.FindControl("tdBottom1")
            Dim tdBottom2 As HtmlTableCell = e.Item.FindControl("tdBottom2")

            tdBottom1.Attributes("class") = "bdrleft bdrbottom"
            tdBottom2.Attributes("class") = "bdrright bdrbottom"
        End If

        tdGiftWrap.Visible = GiftWrappingEnabled
        If Not GiftWrappingEnabled Then tdQuantity.Attributes.Add("colspan", "2")

        Dim rptCart As Repeater = e.Item.FindControl("rptCart")
        dtItems.DefaultView.RowFilter = "RecipientId = " & e.Item.DataItem("RecipientId")
        rptCart.DataSource = dtItems.DefaultView
        rptCart.DataBind()
    End Sub

    Private Sub rptCart_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs)
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
        qs.Add("OrderItemId", e.Item.DataItem("OrderItemId"))
        If Not IsDBNull(e.Item.DataItem("DepartmentId")) Then qs.Add("DepartmentId", e.Item.DataItem("DepartmentId"))
        If Not IsDBNull(e.Item.DataItem("BrandId")) Then qs.Add("BrandId", e.Item.DataItem("BrandId"))

        lnk1.HRef = lnk & qs.ToString()
        lnk2.HRef = lnk & qs.ToString()
        lnk3.HRef = lnk & qs.ToString()

        Dim GiftWrappingEnabled As Boolean = (SysParam.GetValue(DB, "GiftWrappingEnabled") = 1)
        Dim IsGiftWrap As Boolean = e.Item.DataItem("IsGiftWrap")

        Dim tdQuantity As HtmlTableCell = e.Item.FindControl("tdQuantity")
        Dim tdGiftWrap As HtmlTableCell = e.Item.FindControl("tdGiftWrap")
        Dim tdItemPrice As HtmlTableCell = e.Item.FindControl("tdItemPrice")
        Dim tdPrice As HtmlTableCell = e.Item.FindControl("tdPrice")

        tdGiftWrap.Visible = GiftWrappingEnabled

        If Not GiftWrappingEnabled Then tdQuantity.Attributes.Add("colspan", "2")

        Dim tdDetails As HtmlTableCell = e.Item.FindControl("tdDetails")
        If dtRecipients.Rows.Count = 1 AndAlso ItemCount = dtItems.Rows.Count Then
            tdDetails.Attributes.Add("class", "bdrbottom bdrleft")
            tdQuantity.Attributes.Add("class", "bdrbottom")
            tdGiftWrap.Attributes.Add("class", "bdrbottom")
            tdItemPrice.Attributes.Add("class", "bdrbottom")
            tdPrice.Attributes.Add("class", "bdrright bdrbottom")
        End If

        Dim chkGiftWrap As CheckBox = e.Item.FindControl("chkGiftWrap")
        Dim tblGiftWrap As HtmlTable = e.Item.FindControl("tblGiftWrap")
        Dim divGiftWrap As HtmlGenericControl = e.Item.FindControl("divGiftWrap")
        Dim txtGiftQty As TextBox = e.Item.FindControl("txtGiftQty")
        Dim txtQty As TextBox = e.Item.FindControl("txtQty")
        Dim spanGiftWrapNotApplicable As HtmlGenericControl = e.Item.FindControl("spanGiftWrapNotApplicable")

        tblGiftWrap.Visible = IsGiftWrap
        spanGiftWrapNotApplicable.Visible = Not IsGiftWrap
        txtGiftQty.Text = IIf(e.Item.DataItem("GiftQuantity") = 0, String.Empty, e.Item.DataItem("GiftQuantity"))
        chkGiftWrap.Checked = (e.Item.DataItem("GiftQuantity") <> 0)
        divGiftWrap.Attributes.Add("style", IIf(chkGiftWrap.Checked, "display:block", "display:none"))
        chkGiftWrap.Attributes.Add("onclick", "var el = document.getElementById('" & divGiftWrap.ClientID & "'); if (!this.checked) document.getElementById('" & txtGiftQty.ClientID & "').value = ''; if (this.checked) { document.getElementById('" & txtGiftQty.ClientID & "').value = document.getElementById('" & txtQty.ClientID & "').value;} if (this.checked) {el.style.display = 'block';} else {el.style.display = 'none';}")

        Dim spanAttributes As HtmlGenericControl = e.Item.FindControl("spanAttributes")
        Dim spanAttributesWrapper As HtmlGenericControl = e.Item.FindControl("spanAttributesWrapper")

        spanAttributes.InnerHtml = ShoppingCart.GetAttributeText(DB, OrderId, e.Item.DataItem("OrderItemId"))
        If spanAttributes.InnerHtml = String.Empty Then spanAttributesWrapper.Visible = False

        SubTotal += e.Item.DataItem("Price") * e.Item.DataItem("Quantity")
    End Sub

    Private Sub rptCart_ItemCommand(ByVal sender As Object, ByVal e As RepeaterCommandEventArgs)
        Select Case e.CommandName
            Case "Remove"
                Try
                    DB.BeginTransaction()
                    ShoppingCart.DeleteOrderItem(DB, OrderId, e.CommandArgument)
                    ShoppingCart.RecalculateShoppingCart(DB, OrderId, False)
                    DB.CommitTransaction()

                    Response.Redirect("/store/cart.aspx")

                Catch ex As SqlException
                    If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
                    AddError(ErrHandler.ErrorText(ex))
                Catch ex As ApplicationException
                    If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
                    AddError(ex.Message)
                End Try
            Case "Move"

                If Not IsLoggedIn() Then
                    Response.Redirect(AppSettings("GlobalSecureName") & "/members/login.aspx?redir=" & Server.UrlEncode(Request.RawUrl))
                End If

                Try
                    DB.BeginTransaction()
                    Wishlist.Add2WishlistFromCart(DB, MemberId, OrderId, e.CommandArgument)
                    ShoppingCart.DeleteOrderItem(DB, OrderId, e.CommandArgument)
                    ShoppingCart.RecalculateShoppingCart(DB, OrderId, False)
                    DB.CommitTransaction()

                    Response.Redirect(AppSettings("GlobalSecureName") & "/members/wishlist/default.aspx")

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
        If UpdateShoppingCart() Then Response.Redirect("/store/cart.aspx")
    End Sub

    Protected Sub btnCheckout_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCheckout.Click
        If Not IsValid Then Exit Sub
        If UpdateShoppingCart() Then Response.Redirect(AppSettings("GlobalSecureName") & "/store/checkout.aspx")
    End Sub

    Private Function UpdateShoppingCart() As Boolean
        Try
            DB.BeginTransaction()

            For Each recipient As RepeaterItem In rptRecipients.Items
                Dim rptCart As Repeater = recipient.FindControl("rptCart")
                For Each item As RepeaterItem In rptCart.Items
                    Dim txtQty As TextBox = item.FindControl("txtQty")
                    Dim chkGiftWrap As CheckBox = item.FindControl("chkGiftWrap")
                    Dim txtGiftQty As TextBox = item.FindControl("txtGiftQty")
                    Dim OrderItemId As Integer = CType(item.FindControl("lnkRemove"), LinkButton).CommandArgument

                    Dim Quantity As Integer = txtQty.Text
                    Dim GiftQuantity As Integer = IIf(txtGiftQty.Text = String.Empty, 0, txtGiftQty.Text)
                    If Not chkGiftWrap.Checked Then GiftQuantity = 0
                    If GiftQuantity > Quantity Then GiftQuantity = Quantity

                    If Quantity = 0 Then
                        ShoppingCart.DeleteOrderItem(DB, OrderId, OrderItemId)
                    Else
                        ShoppingCart.UpdateQuantities(DB, OrderId, OrderItemId, Quantity, GiftQuantity)
                    End If
                Next
            Next
            ShoppingCart.UpdatePromotionCode(DB, OrderId, txtPromotionCode.Text)
            ShoppingCart.RecalculateShoppingCart(DB, OrderId)

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
