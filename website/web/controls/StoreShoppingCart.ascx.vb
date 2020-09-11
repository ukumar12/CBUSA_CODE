Imports Components
Imports DataLayer

Partial Class StoreShoppingCart
    Inherits BaseControl

    Public Property EditMode() As Boolean
        Get
            Return IIf(ViewState("EditMode") Is Nothing, False, ViewState("EditMode"))
        End Get
        Set(ByVal value As Boolean)
            ViewState("EditMode") = value
        End Set
    End Property

    Public Property OrderId() As Integer
        Get
            Return IIf(ViewState("OrderId") Is Nothing, 0, ViewState("OrderId"))
        End Get
        Set(ByVal value As Integer)
            ViewState("OrderId") = value
        End Set
    End Property

    Private dtItems As DataTable
    Private dtRecipients As DataTable
    Protected dbOrder As StoreOrderRow
    Private SubTotal, Total As Double
    Private ItemCount As Integer
	Private MultipleShipToEnabled As Boolean

	Protected IsFullCart As Boolean

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		IsFullCart = Request.Path.ToLower.IndexOf("/admin/") = 0 OrElse Request.Path.ToLower.IndexOf("/members/") = 0
		dbOrder = StoreOrderRow.GetRow(DB, OrderId)
		MultipleShipToEnabled = SysParam.GetValue(DB, "MultipleShipToEnabled")
		BindData()
	End Sub

    Private Sub BindData()
        If SysParam.GetValue(DB, "GiftWrappingEnabled") = 1 Then
            Dim trGiftWrapping As HtmlTableRow = FindControl("trGiftWrapping")
            Dim ltlGiftWrappingPrice As Literal = FindControl("ltlGiftWrappingPrice")

            ltlGiftWrappingPrice.Text = FormatCurrency(SysParam.GetValue(DB, "GiftWrapPrice"))
            trGiftWrapping.Visible = True
        End If

        dtRecipients = ShoppingCart.GetOrderRecipients(DB, OrderId)
        dtItems = ShoppingCart.GetOrderItems(DB, OrderId)

        rptRecipients.DataSource = dtRecipients
        rptRecipients.DataBind()

        If Not dbOrder.PromotionCode = String.Empty Then ltlPromotionMessage.Text = StorePromotionRow.GetRowByCode(DB, dbOrder.PromotionCode).Message
    End Sub

    Protected Sub rptRecipients_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptRecipients.ItemCreated
        If Not e.Item.ItemType = ListItemType.AlternatingItem AndAlso Not e.Item.ItemType = ListItemType.Item Then
            Exit Sub
        End If
        Dim rptCart As Repeater = e.Item.FindControl("rptCart")
        AddHandler rptCart.ItemDataBound, AddressOf rptCart_ItemDataBound
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
        Dim trShippingBottom As HtmlTableRow = e.Item.FindControl("trShippingBottom")
        Dim trTaxBottom As HtmlTableRow = e.Item.FindControl("trTaxBottom")
        Dim trTotalBottom As HtmlTableRow = e.Item.FindControl("trTotalBottom")

        trMultipleShipTo.Visible = SysParam.GetValue(DB, "MultipleShipToEnabled") = 1
        trSingleShipTo.Visible = Not trMultipleShipTo.Visible

        trRecipientSummary.Visible = dtRecipients.Rows.Count > 1
        trGiftWrappingBottom.Visible = dtRecipients.Rows.Count > 1
        trDiscountBottom.Visible = dtRecipients.Rows.Count > 1
        trShippingBottom.Visible = dtRecipients.Rows.Count > 1
        trTaxBottom.Visible = dtRecipients.Rows.Count > 1
        trTotalBottom.Visible = dtRecipients.Rows.Count > 1

        If e.Item.DataItem("GiftWrapping") = 0 Then
            trGiftWrappingBottom.Visible = False
        End If
        If e.Item.DataItem("Discount") = 0 Then
            trDiscountBottom.Visible = False
        End If
        tdGiftWrap.Visible = GiftWrappingEnabled
        If Not GiftWrappingEnabled Then tdQuantity.Attributes.Add("colspan", "2")

		Dim sFullname As String
		If Not IsFullCart Then sFullname = e.Item.DataItem("FirstName") Else sFullname = Core.HTMLEncode(Core.BuildFullName(IIf(IsDBNull(e.Item.DataItem("FirstName")), String.Empty, e.Item.DataItem("FirstName")), IIf(IsDBNull(e.Item.DataItem("MiddleInitial")), String.Empty, e.Item.DataItem("MiddleInitial")), IIf(IsDBNull(e.Item.DataItem("LastName")), String.Empty, e.Item.DataItem("LastName"))))
        Dim divCompany As HtmlGenericControl = e.Item.FindControl("divCompany")
        Dim divAddress2 As HtmlGenericControl = e.Item.FindControl("divAddress2")
        Dim divRegion As HtmlGenericControl = e.Item.FindControl("divRegion")
        Dim ltlCountry As Literal = e.Item.FindControl("ltlCountry")
        Dim tdGiftMessageLabel As HtmlTableCell = e.Item.FindControl("tdGiftMessageLabel")
        Dim tdGiftMessage As HtmlTableCell = e.Item.FindControl("tdGiftMessage")
        Dim btnEdit As HtmlInputButton = e.Item.FindControl("btnEdit")
        Dim ltlFullName As Literal = e.Item.FindControl("ltlFullName")

        divCompany.Visible = Not IsDBNull(e.Item.DataItem("Company"))
		divAddress2.Visible = IsFullCart AndAlso Not IsDBNull(e.Item.DataItem("Address2"))
        divRegion.Visible = Not IsDBNull(e.Item.DataItem("Region"))
        tdGiftMessageLabel.Visible = Not IsDBNull(e.Item.DataItem("GiftMessage"))
        tdGiftMessage.Visible = tdGiftMessageLabel.Visible
        ltlFullName.Text = sFullname

        If Not IsDBNull(e.Item.DataItem("Company")) Then
            divCompany.InnerText = e.Item.DataItem("Company")
        End If


        If Not IsDBNull(e.Item.DataItem("GiftMessage")) Then
            Dim ltlGiftMessage As Literal = e.Item.FindControl("ltlGiftMessage")
            ltlGiftMessage.Text = Core.Text2HTML(e.Item.DataItem("GiftMessage"))
        End If

        If MultipleShipToEnabled Then
            btnEdit.Attributes.Add("onclick", "document.location.href='shipping.aspx#R" & e.Item.DataItem("RecipientId") & "'")
        Else
            btnEdit.Attributes.Add("onclick", "document.location.href='billing.aspx'")
        End If

        ltlCountry.Text = CountryRow.GetRowByCode(DB, e.Item.DataItem("Country")).CountryName

        If Not IsDBNull(e.Item.DataItem("ShippingMethodId")) Then
            Dim ShippingMethodId As Integer = IIf(IsDBNull(e.Item.DataItem("ShippingMethodId")), 0, e.Item.DataItem("ShippingMethodId"))
            Dim dbShippingMethods As StoreShippingMethodRow = StoreShippingMethodRow.GetRow(DB, ShippingMethodId)
            Dim divShippingMethod As HtmlGenericControl = e.Item.FindControl("divShippingMethod")
            Dim sMethodTrackingShipDate As String = "<b>Shipping Method:</b> " & dbShippingMethods.Name
            If Not IsAdminDisplay Or Request("StatusEmail") = "y" Then
                If Not IsDBNull(e.Item.DataItem("Status")) Then
                    sMethodTrackingShipDate &= "<br /><b>Status:</b> " & e.Item.DataItem("StatusName")
                End If
                If Not IsDBNull(e.Item.DataItem("ShippedDate")) Then
                    sMethodTrackingShipDate &= "<br /><b>Shipped:</b> " & CType(e.Item.DataItem("ShippedDate"), Date).ToString("MM-dd-yyyy")
                End If
                If Not IsDBNull(e.Item.DataItem("TrackingNr")) Then
                    Dim TrackingURL As String = SysParam.GetValue(DB, "ShippingTrackingURL")
                    If TrackingURL <> String.Empty Then
                        sMethodTrackingShipDate &= "<br /><b>Tracking Number:</b> <a href=""" & Replace(TrackingURL, "%%TRACKINGNUMBER%%", e.Item.DataItem("TrackingNr")) & """ target=""_blank"">" & e.Item.DataItem("TrackingNr") & "</a>"
                    Else
                        sMethodTrackingShipDate &= "<br /><b>Tracking Number:</b> " & e.Item.DataItem("TrackingNr")
                    End If
                End If
            End If
            divShippingMethod.InnerHtml = sMethodTrackingShipDate
        End If

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
        If Not IsDBNull(e.Item.DataItem("DepartmentId")) Then qs.Add("DepartmentId", e.Item.DataItem("DepartmentId"))
        If Not IsDBNull(e.Item.DataItem("BrandId")) Then qs.Add("BrandId", e.Item.DataItem("BrandId"))

        lnk1.HRef = lnk & qs.ToString()
        lnk2.HRef = lnk & qs.ToString()
        lnk3.HRef = lnk & qs.ToString()

        lnk2.Visible = EditMode

        Dim GiftWrappingEnabled As Boolean = (SysParam.GetValue(DB, "GiftWrappingEnabled") = 1)
        Dim IsGiftWrap As Boolean = e.Item.DataItem("IsGiftWrap")

        Dim tdQuantity As HtmlTableCell = e.Item.FindControl("tdQuantity")
        Dim tdGiftWrap As HtmlTableCell = e.Item.FindControl("tdGiftWrap")
        Dim tdItemPrice As HtmlTableCell = e.Item.FindControl("tdItemPrice")
        Dim tdPrice As HtmlTableCell = e.Item.FindControl("tdPrice")

        tdGiftWrap.Visible = GiftWrappingEnabled

        Dim GiftQty As Integer = IIf(e.Item.DataItem("GiftQuantity") = 0, 0, e.Item.DataItem("GiftQuantity"))
        If GiftWrappingEnabled AndAlso GiftQty > 0 Then
            tdGiftWrap.InnerText = "Yes (" & GiftQty & ")"
        Else
            tdGiftWrap.InnerText = "No"
        End If

        If Not GiftWrappingEnabled Then tdQuantity.Attributes.Add("colspan", "2")

        Dim tdDetails As HtmlTableCell = e.Item.FindControl("tdDetails")
        If dtRecipients.Rows.Count = 1 AndAlso ItemCount = dtItems.Rows.Count Then
            tdDetails.Attributes.Add("class", "bdrbottom bdrleft")
            tdQuantity.Attributes.Add("class", "bdrbottom")
            tdGiftWrap.Attributes.Add("class", "bdrbottom")
            tdItemPrice.Attributes.Add("class", "bdrbottom")
            tdPrice.Attributes.Add("class", "bdrright bdrbottom")
        End If
        Dim spanAttributesWrapper As HtmlGenericControl = e.Item.FindControl("spanAttributesWrapper")
        Dim spanAttributes As HtmlGenericControl = e.Item.FindControl("spanAttributes")

        spanAttributes.InnerHtml = ShoppingCart.GetAttributeText(DB, OrderId, e.Item.DataItem("OrderItemId"))
        If spanAttributes.InnerHtml = String.Empty Then spanAttributesWrapper.Visible = False

        SubTotal += e.Item.DataItem("Price") * e.Item.DataItem("Quantity")

        Dim spanShipStatus As HtmlGenericControl = e.Item.FindControl("spanShipStatus")
        Dim sMethodTrackingShipDate As String = ""
        If Not IsAdminDisplay Or Request("StatusEmail") = "y" Then
            If Not IsDBNull(e.Item.DataItem("Status")) Then
                sMethodTrackingShipDate &= "<br /><b>Status:</b> " & e.Item.DataItem("StatusName")
            End If
            If Not IsDBNull(e.Item.DataItem("ShippedDate")) Then
                sMethodTrackingShipDate &= "<br /><b>Shipped:</b> " & CType(e.Item.DataItem("ShippedDate"), Date).ToString("MM-dd-yyyy")
            End If
            If Not IsDBNull(e.Item.DataItem("TrackingNumber")) Then
                sMethodTrackingShipDate &= "<br /><b>Tracking Number:</b> " & e.Item.DataItem("TrackingNumber")
            End If
        End If
        spanShipStatus.InnerHtml = sMethodTrackingShipDate
    End Sub

End Class
