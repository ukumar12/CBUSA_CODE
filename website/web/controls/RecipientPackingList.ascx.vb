Imports Components
Imports DataLayer

Public Class RecipientPackingListControl
    Inherits BaseControl

    Private m_OrderId As Integer
    Private m_RecipientId As Integer
    Private m_CartItems As String

    Protected dbOrder As StoreOrderRow
    Protected dbRecipient As StoreOrderRecipientRow
    Protected GiftWrappingEnabled As Boolean

    Public Property OrderId() As Integer
        Get
            Return m_OrderId
        End Get
        Set(ByVal value As Integer)
            m_OrderId = value
        End Set
    End Property

    Public Property RecipientId() As Integer
        Get
            Return m_RecipientId
        End Get
        Set(ByVal value As Integer)
            m_RecipientId = value
        End Set
    End Property

    Public Property CartItems() As String
        Get
            Return m_CartItems
        End Get
        Set(ByVal value As String)
            m_CartItems = value
        End Set
    End Property

    Protected Sub RecipientPackingListControl_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        dbOrder = StoreOrderRow.GetRow(DB, OrderId)
        dbRecipient = StoreOrderRecipientRow.GetRow(DB, RecipientId)
        GiftWrappingEnabled = (SysParam.GetValue(DB, "GiftWrappingEnabled") = 1)
        BindData()
    End Sub

    Private Sub BindData()
        ltlOrderNo.Text = dbOrder.OrderNo
        ltlProcessDate.Text = FormatDateTime(dbOrder.ProcessDate, DateFormat.LongDate)
        ltlShippingMethod.Text = StoreShippingMethodRow.GetRow(DB, dbRecipient.ShippingMethodId).Name

        ltlShippingCountry.Text = CountryRow.GetRowByCode(DB, dbRecipient.Country).CountryName
        ltlBillingCountry.Text = CountryRow.GetRowByCode(DB, dbOrder.BillingCountry).CountryName
        If dbOrder.BillingCountry = "US" Then
            ltlBillingStateOrRegion.Text = StateRow.GetRowByCode(DB, dbOrder.BillingState).StateName
        Else
            ltlBillingStateOrRegion.Text = dbOrder.BillingRegion
        End If
        If dbRecipient.Country = "US" Then
            ltlShippingStateOrRegion.Text = StateRow.GetRowByCode(DB, dbRecipient.State).StateName
        Else
            ltlShippingStateOrRegion.Text = dbRecipient.Region
        End If
        ltlFooterText.Text = CustomTextRow.GetRowByCode(DB, "PackingListFooter").Value

        ltlAddress.Text = SysParam.GetValue(DB, "ShippingCompany") & "<br>"
        ltlAddress.Text &= SysParam.GetValue(DB, "ShippingAddress1") & "<br />"
        If SysParam.GetValue(DB, "ShippingAddress2") <> "" Then ltlAddress.Text &= SysParam.GetValue(DB, "ShippingAddress2") & "<br />"
        ltlAddress.Text &= SysParam.GetValue(DB, "ShippingCity") & ", " & SysParam.GetValue(DB, "ShippingState") & " " & SysParam.GetValue(DB, "ShippingZip")

        rptCartItems.DataSource = dbRecipient.GetCartItemsById(DB, m_CartItems)
        rptCartItems.DataBind()
    End Sub

    Protected Sub rptCartItems_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptCartItems.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.Item Then
            Dim ltlItem As Literal = CType(e.Item.FindControl("ltlItem"), Literal)
            Dim ltlQuantity As Literal = CType(e.Item.FindControl("ltlQuantity"), Literal)
            Dim ltlGiftWrap As Literal = CType(e.Item.FindControl("ltlGiftWrap"), Literal)
            Dim ltlPrice As Literal = CType(e.Item.FindControl("ltlPrice"), Literal)
            Dim ltlTotal As Literal = CType(e.Item.FindControl("ltlTotal"), Literal)
            Dim ltlStatus As Literal = CType(e.Item.FindControl("ltlStatus"), Literal)
            Dim tdGiftWrap As HtmlTableCell = CType(e.Item.FindControl("tdGiftWrap"), HtmlTableCell)

            ltlItem.Text = Server.HtmlEncode(e.Item.DataItem("ItemName"))
            ltlItem.Text &= "<br />" & Server.HtmlEncode(e.Item.DataItem("SKU"))
            Dim sAttributes As String = ShoppingCart.GetAttributeText(DB, OrderId, e.Item.DataItem("OrderItemId"))
            If sAttributes <> "" Then ltlItem.Text &= "<br />" & sAttributes

            Dim GiftQty As Integer = IIf(e.Item.DataItem("GiftQuantity") = 0, 0, e.Item.DataItem("GiftQuantity"))
            tdGiftWrap.Visible = GiftWrappingEnabled
            If GiftWrappingEnabled AndAlso GiftQty > 0 Then
                ltlGiftWrap.Text = "Yes (" & GiftQty & ")"
            Else
                ltlGiftWrap.Text = "No"
            End If

            Dim bIsOnSale As Boolean = e.Item.DataItem("IsOnSale")
            Dim fPrice As Double = e.Item.DataItem("Price")
            Dim fSalePrice As Double = IIf(IsDBNull(e.Item.DataItem("SalePrice")), 0, e.Item.DataItem("SalePrice"))
            Dim iQuantity As Integer = e.Item.DataItem("Quantity")

            ltlQuantity.Text = iQuantity
            ltlStatus.Text = StoreOrderStatusRow.GetRowByCode(DB, e.Item.DataItem("Status")).Name
            If bIsOnSale Then ltlPrice.Text = FormatCurrency(fSalePrice) Else ltlPrice.Text = FormatCurrency(fPrice)
            If bIsOnSale Then ltlTotal.Text = FormatCurrency(fSalePrice * iQuantity) Else ltlTotal.Text = FormatCurrency(fPrice * iQuantity)
        ElseIf e.Item.ItemType = ListItemType.Header Then
            Dim thGiftWrap As HtmlTableCell = CType(e.Item.FindControl("thGiftWrap"), HtmlTableCell)
            thGiftWrap.Visible = GiftWrappingEnabled
        End If
    End Sub
End Class
