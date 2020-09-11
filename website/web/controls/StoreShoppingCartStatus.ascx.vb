Imports Components
Imports DataLayer

Partial Class StoreShoppingCartStatus
    Inherits BaseControl


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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        dbOrder = StoreOrderRow.GetRow(DB, OrderId)
        MultipleShipToEnabled = SysParam.GetValue(DB, "MultipleShipToEnabled")
        BindData()
    End Sub

    Private Sub BindData()
        

        dtRecipients = ShoppingCart.GetOrderRecipients(DB, OrderId)
        dtItems = ShoppingCart.GetOrderItems(DB, OrderId)

        rptRecipients.DataSource = dtRecipients
        rptRecipients.DataBind()

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


        Dim tdQuantity As HtmlTableCell = e.Item.FindControl("tdQuantity")
        Dim trMultipleShipTo As HtmlTableRow = e.Item.FindControl("trMultipleShipTo")
        Dim trSingleShipTo As HtmlTableRow = e.Item.FindControl("trSingleShipTo")

        trMultipleShipTo.Visible = SysParam.GetValue(DB, "MultipleShipToEnabled") = 1
        trSingleShipTo.Visible = Not trMultipleShipTo.Visible

        Dim sFullname As String = Core.HTMLEncode(Core.BuildFullName(IIf(IsDBNull(e.Item.DataItem("FirstName")), String.Empty, e.Item.DataItem("FirstName")), IIf(IsDBNull(e.Item.DataItem("MiddleInitial")), String.Empty, e.Item.DataItem("MiddleInitial")), IIf(IsDBNull(e.Item.DataItem("LastName")), String.Empty, e.Item.DataItem("LastName"))))
        Dim ltlFullName As Literal = e.Item.FindControl("ltlFullName")
        Dim divCompany As HtmlGenericControl = e.Item.FindControl("divCompany")
        Dim divAddress2 As HtmlGenericControl = e.Item.FindControl("divAddress2")
        Dim divRegion As HtmlGenericControl = e.Item.FindControl("divRegion")
        Dim ltlCountry As Literal = e.Item.FindControl("ltlCountry")

        divCompany.Visible = Not IsDBNull(e.Item.DataItem("Company"))
        divAddress2.Visible = Not IsDBNull(e.Item.DataItem("Address2"))
        divRegion.Visible = Not IsDBNull(e.Item.DataItem("Region"))

        ltlFullName.Text = sFullname
        If Not IsDBNull(e.Item.DataItem("Company")) Then
            divCompany.InnerText = e.Item.DataItem("Company")
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

        Dim tdQuantity As HtmlTableCell = e.Item.FindControl("tdQuantity")

        Dim tdDetails As HtmlTableCell = e.Item.FindControl("tdDetails")
        If dtRecipients.Rows.Count = 1 AndAlso ItemCount = dtItems.Rows.Count Then
            tdDetails.Attributes.Add("class", "bdrbottom bdrleft")
            tdQuantity.Attributes.Add("class", "bdrbottom")
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
