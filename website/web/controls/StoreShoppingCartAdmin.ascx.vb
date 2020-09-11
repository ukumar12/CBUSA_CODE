Imports Components
Imports DataLayer
Imports System.Configuration.ConfigurationManager

Partial Class StoreShoppingCartAdmin
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
    Private dtOrderStatus As DataTable
    Private SubTotal, Total As Double
    Private ItemCount As Integer
    Private MultipleShipToEnabled As Boolean
    Private NewRow As Boolean = False

    Private bHasDisplayedLineItems As Boolean = False, bMustDisplayChildren As Boolean = False
    Private aryRecipientId() As String
    Private aryOrderItemId() As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        dbOrder = StoreOrderRow.GetRow(DB, OrderId)
        MultipleShipToEnabled = SysParam.GetValue(DB, "MultipleShipToEnabled")

        If Request("StatusEmail") = "y" Then
            Me.tdTotal.Visible = False
            Me.trOrderTotal.Visible = False
            Me.trOrderTax.Visible = False
            Me.trOrderSubTotal.Visible = False
            Me.trOrderShipping.Visible = False
            Me.trOrderGiftWrapping.Visible = False
            If Request("RecipientId") <> String.Empty Then
                aryRecipientId = Split(Request("RecipientId"), ",")
            End If
            If Request("OrderItemId") <> String.Empty Then
                aryOrderItemId = Split(Request("OrderItemId"), ",")
            End If
        End If

        If Not IsPostBack Then BindData()

        If IsAdminDisplay And Request("StatusEmail") = String.Empty Then
            Me.pnlAdminScripts.Visible = True
            Me.tblOrderSTatus.Visible = True
            Me.trSendStatus.Visible = True
        Else
            Me.pnlAdminScripts.Visible = False
            Me.tblOrderSTatus.Visible = False
            Me.trSendStatus.Visible = False
        End If

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
        dtOrderStatus = StoreOrderStatusRow.GetAllStoreOrderStatuss(DB)

        rptRecipients.DataSource = dtRecipients
        rptRecipients.DataBind()

        If Not dbOrder.PromotionCode = String.Empty Then ltlPromotionMessage.Text = StorePromotionRow.GetRowByCode(DB, dbOrder.PromotionCode).Message

        Me.radOrderStatus.DataSource = dtOrderStatus
        Me.radOrderStatus.DataTextField = "Name"
        Me.radOrderStatus.DataValueField = "Code"
        Me.radOrderStatus.DataBind()
        Me.radOrderStatus.SelectedValue = dbOrder.Status
        For Each dr As DataRow In dtOrderStatus.Rows
            radOrderStatus.Items.FindByValue(dr("Code")).Attributes.Add("IsFinalAction", IIf(dr("IsFinalAction"), "1", "0"))
        Next

        If SysParam.GetValue(DB, "IsPackingListPDF") = "0" Then
            ltlPackingListPrint.Text = " | <a href=""javascript:document.getElementById('ifrmPackingList').contentWindow.focus(); document.getElementById('ifrmPackingList').contentWindow.print();"" class=""smaller"">Print</a>"
        Else
            ltlPackingListPrint.Text = ""
        End If
    End Sub


    Protected Sub rptRecipients_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptRecipients.ItemCommand
        Response.Redirect(e.CommandArgument)
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
        Dim AjaxManager As ScriptManager = Me.Parent.FindControl("AjaxManager")

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
        Dim tdShipping As HtmlTableCell = e.Item.FindControl("tdShipping")
        Dim trAdminShippingStatus As HtmlTableRow = e.Item.FindControl("trAdminShippingStatus")
        Dim drpIsShippingIndividually As DropDownList = e.Item.FindControl("drpIsShippingIndividually")
        Dim dpRecipientShippedDate As Controls.DatePicker = e.Item.FindControl("dpRecipientShippedDate")

        trAdminShippingStatus.Attributes.Add("RecipientId", e.Item.DataItem("RecipientId"))

        'determind multiple vs. single ship to
        trMultipleShipTo.Visible = SysParam.GetValue(DB, "MultipleShipToEnabled") = 1
        trSingleShipTo.Visible = Not trMultipleShipTo.Visible

        ' make appropriate order fields visible
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

        Dim sFullname As String = Core.HTMLEncode(Core.BuildFullName(IIf(IsDBNull(e.Item.DataItem("FirstName")), String.Empty, e.Item.DataItem("FirstName")), IIf(IsDBNull(e.Item.DataItem("MiddleInitial")), String.Empty, e.Item.DataItem("MiddleInitial")), IIf(IsDBNull(e.Item.DataItem("LastName")), String.Empty, e.Item.DataItem("LastName"))))
        Dim ltlFullName As Literal = e.Item.FindControl("ltlFullName")
        Dim divCompany As HtmlGenericControl = e.Item.FindControl("divCompany")
        Dim divAddress2 As HtmlGenericControl = e.Item.FindControl("divAddress2")
        Dim divRegion As HtmlGenericControl = e.Item.FindControl("divRegion")
        Dim ltlCountry As Literal = e.Item.FindControl("ltlCountry")
        Dim tdGiftMessageLabel As HtmlTableCell = e.Item.FindControl("tdGiftMessageLabel")
        Dim tdGiftMessage As HtmlTableCell = e.Item.FindControl("tdGiftMessage")
        Dim btnEdit As HtmlInputButton = e.Item.FindControl("btnEdit")

        divCompany.Visible = Not IsDBNull(e.Item.DataItem("Company"))
        divAddress2.Visible = Not IsDBNull(e.Item.DataItem("Address2"))
        divRegion.Visible = Not IsDBNull(e.Item.DataItem("Region"))
        tdGiftMessageLabel.Visible = Not IsDBNull(e.Item.DataItem("GiftMessage"))
        tdGiftMessage.Visible = tdGiftMessageLabel.Visible

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
        ltlFullName.Text = sFullname
        If Not IsDBNull(e.Item.DataItem("Company")) Then
            divCompany.InnerText = e.Item.DataItem("Company")
        End If

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


        'get recipient level status control
        'set recipientid, OriginalValue and bind status
        'Set type of dropdown IsRecipientStatus (referenced in javascript functionality)
        ' set isfinalaction (referenced in javacsript)
        Dim drpShipmentStatus As DropDownList = e.Item.FindControl("drpShipmentStatus")
        drpShipmentStatus.AutoPostBack = True
        AddHandler drpShipmentStatus.SelectedIndexChanged, AddressOf drpRecipientStatus_SelectedIndexChanged
        drpShipmentStatus.Attributes.Add("RecipientId", e.Item.DataItem("RecipientId"))
        drpShipmentStatus.DataSource = dtOrderStatus
        drpShipmentStatus.DataTextField = "Name"
        drpShipmentStatus.DataValueField = "Code"
        drpShipmentStatus.DataBind()
        If Not IsDBNull(e.Item.DataItem("Status")) And Not IsPostBack Then drpShipmentStatus.SelectedValue = e.Item.DataItem("Status")
        For Each dr As DataRow In dtOrderStatus.Rows
            drpShipmentStatus.Items.FindByValue(dr("Code")).Attributes.Add("IsFinalAction", IIf(dr("IsFinalAction"), "1", "0"))
        Next


        e.Item.FindControl("tdRecipientShippedDate").Visible = drpShipmentStatus.SelectedValue = "S"
        e.Item.FindControl("tdRecipientTrackingNumber").Visible = drpShipmentStatus.SelectedValue = "S"


        ' Recipient Shipped Date  tdRecipientTrackingNumber
        CType(e.Item.FindControl("tdRecipientShippedDate"), HtmlTableCell).Attributes.Add("RecipientId", e.Item.DataItem("RecipientId"))
        CType(e.Item.FindControl("tdRecipientTrackingNumber"), HtmlTableCell).Attributes.Add("RecipientId", e.Item.DataItem("RecipientId"))
        dpRecipientShippedDate.Attributes.Add("RecipientId", e.Item.DataItem("RecipientId"))
        If Not IsDBNull(e.Item.DataItem("ShippedDate")) Then
            dpRecipientShippedDate.Value = e.Item.DataItem("ShippedDate")
        End If


        'Recipient Tracking Number
        Dim txtRecipientTrackingNumber As TextBox = e.Item.FindControl("txtRecipientTrackingNumber")
        txtRecipientTrackingNumber.Attributes.Add("RecipientId", e.Item.DataItem("RecipientId"))
        If Not IsDBNull(e.Item.DataItem("TrackingNr")) Then
            txtRecipientTrackingNumber.Text = e.Item.DataItem("TrackingNr")
        End If

        If IsAdminDisplay And Request("StatusEmail") = String.Empty Then
            trAdminShippingStatus.Visible = True
            Dim tblRecipientStatusFields As HtmlTable = e.Item.FindControl("tblRecipientStatusFields")
            tblRecipientStatusFields.ID = "tblRecipientStatusFields" & e.Item.DataItem("RecipientId")
            If e.Item.DataItem("IsShippingIndividually") Then
                tblRecipientStatusFields.Visible = False
            Else
                tblRecipientStatusFields.Visible = True
            End If
        End If

        If e.Item.DataItem("IsShippingIndividually") Then
            drpIsShippingIndividually.SelectedValue = "true"
        Else
            drpIsShippingIndividually.SelectedValue = "false"
        End If
        drpIsShippingIndividually.Attributes.Add("recipientid", e.Item.DataItem("RecipientId"))
        drpIsShippingIndividually.Attributes.Add("RecipientLabel", Replace(e.Item.DataItem("Label"), """", ""))
        drpIsShippingIndividually.AutoPostBack = True
        AddHandler drpIsShippingIndividually.SelectedIndexChanged, AddressOf drpIsShippingIndividually_SelectedIndexChanged



        'Select all Items in recipient Item List (used for select items for sending status update.)
        Dim chk As HtmlControls.HtmlInputCheckBox = e.Item.FindControl("chkSelectAllRecipientItems")
        'chk.Attributes.Add("IsItem", "0")
        chk.Attributes.Add("RecipientId", e.Item.DataItem("RecipientId"))
        chk.Attributes.Add("onclick", "if(document.getElementById('" & chk.ClientID & "').checked){selectAllRecipientItems(" & e.Item.DataItem("RecipientId") & ",1);}else {selectAllRecipientItems(" & e.Item.DataItem("RecipientId") & ",0);}")

        bHasDisplayedLineItems = False
        bMustDisplayChildren = False
        If Request("StatusEmail") = "y" Then
            e.Item.Visible = False
            If Not aryRecipientId Is Nothing Then
                If Array.IndexOf(aryRecipientId, e.Item.DataItem("RecipientId").ToString) > -1 Then
                    e.Item.Visible = True
                    bMustDisplayChildren = False
                End If
            End If
        End If

        'Bind Recipient ITems
        Dim rptCart As Repeater = e.Item.FindControl("rptCart")
        dtItems.DefaultView.RowFilter = "RecipientId = " & e.Item.DataItem("RecipientId")
        rptCart.DataSource = dtItems.DefaultView
        rptCart.DataBind()

        e.Item.Visible = Request("StatusEmail") = String.Empty OrElse (bHasDisplayedLineItems And Request("STatusEmail") = "y")

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
        lnk = AppSettings("GlobalRefererName") & lnk
        qs.Add("OrderItemId", e.Item.DataItem("OrderItemId"))
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


        SubTotal += e.Item.DataItem("Price") * e.Item.DataItem("Quantity")

        Dim drpItemStatus As DropDownList = e.Item.FindControl("drpItemStatus")
        drpItemStatus.AutoPostBack = True
        drpItemStatus.Attributes.Add("RecipientId", e.Item.DataItem("RecipientId"))
        drpItemStatus.Attributes.Add("OrderItemId", e.Item.DataItem("OrderItemId"))
        drpItemStatus.Attributes.Add("ItemName", Replace(e.Item.DataItem("ItemName"), """", """"""))
        drpItemStatus.DataSource = dtOrderStatus
        drpItemStatus.DataTextField = "Name"
        drpItemStatus.DataValueField = "Code"
        drpItemStatus.DataBind()
        If Not IsDBNull(e.Item.DataItem("Status")) Then drpItemStatus.SelectedValue = e.Item.DataItem("Status")
        For Each dr As DataRow In dtOrderStatus.Rows
            drpItemStatus.Items.FindByValue(dr("Code")).Attributes.Add("IsFinalAction", IIf(dr("IsFinalAction"), "1", "0"))
        Next

        e.Item.FindControl("tdShippedDate").Visible = drpItemStatus.SelectedValue = "S"
        e.Item.FindControl("tdItemTrackingNumber").Visible = drpItemStatus.SelectedValue = "S"

        Dim chk As HtmlControls.HtmlInputCheckBox = e.Item.FindControl("chkStatusItem")
        chk.Attributes.Add("OrderItemId", e.Item.DataItem("OrderItemId"))
        chk.Attributes.Add("RecipientId", e.Item.DataItem("RecipientId"))
        chk.Attributes.Add("onclick", "deselectSelectAllCheckbox(" & e.Item.DataItem("RecipientId") & ")")


        Dim dtShippedDate As Controls.DatePicker = e.Item.FindControl("dpShippedDate")
        dtShippedDate.Attributes.Add("OrderItemId", e.Item.DataItem("OrderItemId"))
        dtShippedDate.Attributes.Add("RecipientId", e.Item.DataItem("RecipientId"))
        If Not IsDBNull(e.Item.DataItem("ShippedDate")) Then
            dtShippedDate.Attributes.Add("OriginalValue", CType(e.Item.DataItem("ShippedDate"), Date).ToString("MM-dd-yyyy"))
            dtShippedDate.Value = e.Item.DataItem("ShippedDate")
        Else
            dtShippedDate.Attributes.Add("OriginalValue", "")
        End If


        Dim txtTrackingNumber As TextBox = e.Item.FindControl("txtTrackingNumber")
        txtTrackingNumber.Attributes.Add("OrderItemId", e.Item.DataItem("OrderItemId"))
        txtTrackingNumber.Attributes.Add("RecipientId", e.Item.DataItem("RecipientId"))
        If Not IsDBNull(e.Item.DataItem("TrackingNumber")) Then
            txtTrackingNumber.Attributes.Add("OriginalValue", e.Item.DataItem("TrackingNumber"))
            txtTrackingNumber.Text = e.Item.DataItem("TrackingNumber")
        Else
            txtTrackingNumber.Attributes.Add("OriginalValue", "")
        End If

        If IsAdminDisplay And Request("StatusEmail") = String.Empty Then
            Dim trAdminShippingStatus As HtmlTableRow = e.Item.FindControl("trAdminShippingStatus")
            trAdminShippingStatus.Visible = True
            Dim tblItemStatusFields As HtmlTable = e.Item.FindControl("tblItemStatusFields")
            tblItemStatusFields.ID = "tblItemStatusFields" & e.Item.DataItem("OrderItemId")
            If e.Item.DataItem("IsShippingIndividually") Then
                tblItemStatusFields.Visible = True
            Else
                tblItemStatusFields.Visible = False
            End If
        End If


        If Request("statusemail") = "y" Then
            e.Item.Visible = False
            If bMustDisplayChildren Then
                e.Item.Visible = True
            Else
                If Not aryOrderItemId Is Nothing Then
                    If Array.IndexOf(aryOrderItemId, CStr(e.Item.DataItem("OrderItemId"))) > -1 Then
                        e.Item.Visible = True
                        bHasDisplayedLineItems = True
                    End If
                End If
            End If
        End If

    End Sub
    Public Function ValidateForm() As Boolean
        Dim bError As Boolean = True

        For Each lRecipient As RepeaterItem In Me.rptRecipients.Items

            Dim drpRecipientStatus As DropDownList = lRecipient.FindControl("drpShipmentStatus")
            Dim dpRecipientShippedDate As Controls.DatePicker = lRecipient.FindControl("dpRecipientShippedDate")
            Dim tdRecipientShippedDate As HtmlControls.HtmlTableCell = lRecipient.FindControl("tdRecipientShippedDate")

            Dim drpIsShippingIndividually As DropDownList = lRecipient.FindControl("drpIsShippingIndividually")
            Dim rptItems As Repeater = lRecipient.FindControl("rptCart")

            If drpIsShippingIndividually.SelectedValue = "false" Then
                'if the status is shipped, make the ship date required
                If dpRecipientShippedDate.Text <> String.Empty Then
                    If Not IsDate(dpRecipientShippedDate.Text) Then
                        tdRecipientShippedDate.Style.Add("background-color", "#FF6464")
                        AddError("Recipient Shipped Date is invalid : " & drpIsShippingIndividually.Attributes("RecipientLabel"))
                        bError = False
                    End If
                Else
                    'Ship date is empty
                    'if status is set to shipped, required a ship date
                    'throw error.
                    If drpRecipientStatus.SelectedValue = "S" Then
                        tdRecipientShippedDate.Style.Add("background-color", "#FF6464")
                        AddError("Recipient Shipped Date is required : " & drpIsShippingIndividually.Attributes("RecipientLabel"))
                        bError = False
                    End If
                End If
            Else

                For Each litem As RepeaterItem In rptItems.Items
                    Dim dpItemShippedDate As Controls.DatePicker = litem.FindControl("dpShippedDate")
                    Dim tdShippedDate As HtmlTableCell = litem.FindControl("tdShippedDate")
                    Dim drpItemStatus As DropDownList = litem.FindControl("drpItemStatus")
                    If dpItemShippedDate.Text <> String.Empty Then
                        If Not IsDate(dpItemShippedDate.Text) Then
                            tdShippedDate.Style.Add("background-color", "#FF6464")
                            AddError("Shipped Date is invalid : " & drpIsShippingIndividually.Attributes("RecipientLabel") & " - " & drpItemStatus.Attributes("ItemName"))
                            bError = False
                        End If
                    Else
                        'Ship date is empty
                        'if status is set to shipped, required a ship date
                        'throw error.
                        If drpItemStatus.SelectedValue = "S" Then
                            tdShippedDate.Style.Add("background-color", "#FF6464")
                            AddError("Line item shipped date is required : " & drpIsShippingIndividually.Attributes("RecipientLabel") & " - " & drpItemStatus.Attributes("ItemName"))
                            bError = False
                        End If
                    End If
                Next

            End If


        Next



        Return bError

    End Function


    Public Function SaveStatusInfo() As String
        Dim sNotes As String = GetUpdatesForNotes()
        SaveStatus()
        Return sNotes
    End Function

    Public Function SaveStatus() As Boolean
        For Each lRecipient As RepeaterItem In Me.rptRecipients.Items

            Dim drpRecipientStatus As DropDownList = lRecipient.FindControl("drpShipmentStatus")
            Dim txtRecipientTracking As TextBox = lRecipient.FindControl("txtRecipientTrackingNumber")
            Dim dpRecipientShippedDate As Controls.DatePicker = lRecipient.FindControl("dpRecipientShippedDate")
            Dim RecipientId As Integer = drpRecipientStatus.Attributes("RecipientId")
            Dim drpIsShippingIndividually As DropDownList = lRecipient.FindControl("drpIsShippingIndividually")
            Dim rptItems As Repeater = lRecipient.FindControl("rptCart")

            Dim dbStoreOrderRecipient As StoreOrderRecipientRow = StoreOrderRecipientRow.GetRow(DB, RecipientId)
            dbStoreOrderRecipient.Status = drpRecipientStatus.SelectedValue
            If drpIsShippingIndividually.SelectedValue = "false" Then
                dbStoreOrderRecipient.IsShippingIndividually = False
                If dpRecipientShippedDate.Text <> String.Empty Then
                    dbStoreOrderRecipient.ShippedDate = dpRecipientShippedDate.Value
                Else
                    dbStoreOrderRecipient.ShippedDate = Nothing
                End If
                dbStoreOrderRecipient.TrackingNr = Trim(txtRecipientTracking.Text)
            Else
                dbStoreOrderRecipient.IsShippingIndividually = True
                dbStoreOrderRecipient.ShippedDate = Nothing
                dbStoreOrderRecipient.TrackingNr = ""
            End If
            dbStoreOrderRecipient.Update()

            For Each litem As RepeaterItem In rptItems.Items
                Dim drpItemStatus As DropDownList = litem.FindControl("drpItemSTatus")
                Dim txtItemTracking As TextBox = litem.FindControl("txtTrackingNumber")
                Dim dpItemShippedDate As Controls.DatePicker = litem.FindControl("dpShippedDate")

                Dim OrderItemId As Integer = drpItemStatus.Attributes("OrderItemId")
                Dim dbStoreOrderItem As StoreOrderItemRow = StoreOrderItemRow.GetRow(DB, OrderItemId)

                dbStoreOrderItem.Status = drpItemStatus.SelectedValue
                If dpItemShippedDate.Text <> String.Empty Then
                    dbStoreOrderItem.ShippedDate = dpItemShippedDate.Value
                Else
                    dbStoreOrderItem.ShippedDate = Nothing
                End If
                dbStoreOrderItem.TrackingNumber = txtItemTracking.Text
                dbStoreOrderItem.Update()
            Next
        Next

        dbOrder.Status = Me.radOrderStatus.SelectedValue
        dbOrder.Update()

    End Function

    Public Function GetUpdatesForNotes() As String
        Dim sNotes As New StringBuilder

        If dbOrder.Status <> Me.radOrderStatus.SelectedValue Then
            'not working
            Dim FromOrderStatusName As String = DB.ExecuteScalar("SELECT Name FROM StoreOrderStatus WHERE Code = " & DB.Quote(dbOrder.Status))
            Dim ToOrderStatusName As String = DB.ExecuteScalar("SELECT Name FROM StoreOrderStatus WHERE Code = " & DB.Quote(radOrderStatus.SelectedValue))
            sNotes.AppendLine("Order Status changed from : '" & FromOrderStatusName & "' to '" & ToOrderStatusName & "'<br />")
        End If

        Dim dvRecipients As DataView = StoreOrderRecipientRow.GetOrderRecipients(DB, OrderId).DefaultView
        Dim dvOrderItems As DataView = DB.GetDataTable("SELECT * FROM StoreOrderItem WHERE OrderId = " & DB.Number(OrderId)).DefaultView
        For Each lRecipient As RepeaterItem In Me.rptRecipients.Items
            Dim RecipientId As Integer = Nothing
            Dim drpIsShippingIndividually As DropDownList = lRecipient.FindControl("drpIsShippingIndividually")
            RecipientId = drpIsShippingIndividually.Attributes("RecipientId")
            dvRecipients.RowFilter = "RecipientId = " & RecipientId
            dvOrderItems.RowFilter = "RecipientId = " & RecipientId

            If dvRecipients.Count > 0 Then

                If Not dvRecipients(0)("IsShippingIndividually") And drpIsShippingIndividually.SelectedValue = "true" Then
                    sNotes.AppendLine("Shipment Type: changed from 'All items shipped together' to 'individually shipped'<br />")
                End If

                If dvRecipients(0)("IsShippingIndividually") And drpIsShippingIndividually.SelectedValue = "false" Then
                    sNotes.AppendLine("Shipment Type: changed from 'individually shipped' to 'All items shipped together'<br />")
                End If

                If CType(lRecipient.FindControl("drpShipmentStatus"), DropDownList).SelectedValue <> dvRecipients(0)("Status") Then
                    sNotes.AppendLine("Recipient '" & dvRecipients(0)("Label") & "' Shipment Status : changed from '" & dvRecipients(0)("StatusName") & "' to '" & CType(lRecipient.FindControl("drpShipmentStatus"), DropDownList).SelectedItem.Text & "'<br />")
                End If

                Dim dShipDate As Date = CType(lRecipient.FindControl("dpRecipientShippedDate"), Controls.DatePicker).Value
                If CType(lRecipient.FindControl("dpRecipientShippedDate"), Controls.DatePicker).Text <> String.Empty Then
                    If IsDBNull(dvRecipients(0)("ShippedDate")) Then
                        sNotes.AppendLine("Recipient '" & dvRecipients(0)("Label") & "' Shipped Date : changed from '' to '" & dShipDate.ToString("MM-dd-yyyy") & "'<br />")
                    Else
                        If dShipDate.ToString("MM-dd-yyyy") <> CType(dvRecipients(0)("ShippedDate"), Date).ToString("MM-dd-yyyy") Then
                            sNotes.AppendLine("Recipient '" & dvRecipients(0)("Label") & "' Shipped Date : changed from '" & CType(dvRecipients(0)("ShippedDate"), Date).ToString("MM-dd-yyyy") & "' to '" & dShipDate.ToString("MM-dd-yyyy") & "'<br />")
                        End If
                    End If
                Else
                    If Not IsDBNull(dvRecipients(0)("ShippedDate")) Then
                        sNotes.AppendLine("Recipient '" & dvRecipients(0)("Label") & "' Shipped Date : changed from '" & CType(dvRecipients(0)("ShippedDate"), Date).ToString("MM-dd-yyyy") & "' to ''<br />")
                    End If
                End If

                Dim sTrackingNumber As String = Trim(CType(lRecipient.FindControl("txtRecipientTrackingNumber"), TextBox).Text)
                If sTrackingNumber <> String.Empty Then
                    If IsDBNull(dvRecipients(0)("TrackingNr")) Then
                        sNotes.AppendLine("Recipient '" & dvRecipients(0)("Label") & "' Tracking# : changed from '' to '" & sTrackingNumber & "'<br />")
                    Else
                        If sTrackingNumber <> Trim(dvRecipients(0)("TrackingNr")) Then
                            sNotes.AppendLine("Recipient '" & dvRecipients(0)("Label") & "' Tracking# : changed from '" & dvRecipients(0)("TrackingNr") & "' to '" & sTrackingNumber & "'<br />")
                        End If
                    End If
                Else
                    If Not IsDBNull(dvRecipients(0)("TrackingNr")) Then
                        sNotes.AppendLine("Recipient '" & dvRecipients(0)("Label") & "' Tracking# : changed from '" & dvRecipients(0)("TrackingNr") & "' to ''<br />")
                    End If
                End If

                Dim rptItems As Repeater = lRecipient.FindControl("rptCart")
                For Each lItem As RepeaterItem In rptItems.Items

                    Dim drpItemStatus As DropDownList = lItem.FindControl("drpItemStatus")
                    Dim dpItemShippedDate As Controls.DatePicker = lItem.FindControl("dpShippedDate")
                    Dim txtItemTrackingNumber As TextBox = lItem.FindControl("txtTrackingNumber")
                    Dim OrderItemId As Integer = drpItemStatus.Attributes("OrderItemId")

                    dvOrderItems.RowFilter = "OrderItemId = " & OrderItemId
                    Dim drv As DataRowView = dvOrderItems(0)


                    If drpItemStatus.SelectedValue <> drv("Status") Then
                        sNotes.AppendLine("Recipient '" & dvRecipients(0)("Label") & "' : " & drv("ItemNAme") & " Shipment Status : changed from '" & drv("Status") & "' to '" & drpItemStatus.SelectedValue & "'<br />")
                    End If

                    Dim dItemShipDate As Date = dpItemShippedDate.Value
                    If dpItemShippedDate.Text <> String.Empty Then
                        If IsDBNull(drv("ShippedDate")) Then
                            sNotes.AppendLine("Recipient '" & dvRecipients(0)("Label") & "' : " & drv("ItemNAme") & " Shipped Date : changed from '' to '" & dItemShipDate.ToString("MM-dd-yyyy") & "'<br />")
                        Else
                            If dItemShipDate.ToString("MM-dd-yyyy") <> CType(drv("ShippedDate"), Date).ToString("MM-dd-yyyy") Then
                                sNotes.AppendLine("Recipient '" & dvRecipients(0)("Label") & "' : " & drv("ItemNAme") & " Shipped Date : changed from '" & CType(drv("ShippedDate"), Date).ToString("MM-dd-yyyy") & "' to '" & dItemShipDate.ToString("MM-dd-yyyy") & "'<br />")
                            End If
                        End If
                    Else
                        If Not IsDBNull(drv("ShippedDate")) Then
                            sNotes.AppendLine("Recipient '" & dvRecipients(0)("Label") & "' : " & drv("ItemName") & " Shipped Date : changed from '" & CType(drv("ShippedDate"), Date).ToString("MM-dd-yyyy") & "' to ''<br />")
                        End If
                    End If

                    If txtItemTrackingNumber.Text <> String.Empty Then
                        If IsDBNull(drv("TrackingNumber")) Then
                            sNotes.AppendLine("Recipient '" & dvRecipients(0)("Label") & "' : " & drv("ItemName") & " Tracking# : changed from '' to '" & txtItemTrackingNumber.Text & "'<br />")
                        Else
                            If txtItemTrackingNumber.Text <> Trim(drv("TrackingNumber")) Then
                                sNotes.AppendLine("Recipient '" & dvRecipients(0)("Label") & "' : " & drv("ItemName") & " Tracking# : changed from '" & drv("TrackingNumber") & "' to '" & txtItemTrackingNumber.Text & "'<br />")
                            End If
                        End If
                    Else
                        If Not IsDBNull(drv("TrackingNumber")) Then
                            sNotes.AppendLine("Recipient '" & dvRecipients(0)("Label") & "' : " & drv("ItemName") & " Tracking# : changed from '" & drv("TrackingNumber") & "' to ''<br />")
                        End If
                    End If


                Next
            End If
        Next


        Return sNotes.ToString

    End Function

    Private Function ValidateAndSaveForm() As Boolean
        ValidateAndSaveForm = False
        If Not ValidateForm() Then
            Exit Function
        End If
        Dim CartUpdates As String = SaveStatusInfo()
        Dim sNotes As New StringBuilder
        Dim bUpdate As Boolean = False
        If CartUpdates <> String.Empty Then
            sNotes.AppendLine(CartUpdates)
            bUpdate = True
        End If
        If bUpdate Then
            Dim dbNotes As New StoreOrderNoteRow(DB)
            dbNotes.Note = sNotes.ToString
            dbNotes.AdminId = CType(Me.Page, AdminPage).LoggedInAdminId
            dbNotes.OrderId = OrderId
            dbNotes.Insert()
        End If
        ValidateAndSaveForm = True
    End Function

    Protected Sub btnPrintPacking_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPrintPacking.Click
        If Not ValidateAndSaveForm() Then Exit Sub
        Dim RecipientId As String = "", OrderItemId As String = ""

        For Each objRepeaterItem As RepeaterItem In rptRecipients.Items
            If objRepeaterItem.ItemType = ListItemType.Item Or objRepeaterItem.ItemType = ListItemType.AlternatingItem Then
                Dim rptCart As Repeater = CType(objRepeaterItem.FindControl("rptCart"), Repeater)
                For Each objCartItem As RepeaterItem In rptCart.Items
                    If objCartItem.ItemType = ListItemType.AlternatingItem Or objCartItem.ItemType = ListItemType.Item Then
                        Dim chkStatusItem As HtmlInputCheckBox = CType(objCartItem.FindControl("chkStatusItem"), HtmlInputCheckBox)
                        If chkStatusItem.Checked Then
                            If OrderItemId = "" Then OrderItemId = chkStatusItem.Attributes("OrderItemId") Else OrderItemId &= "," & chkStatusItem.Attributes("OrderItemId")
                        End If
                    End If
                Next
            End If
        Next

        Dim OrderGuid As String = StoreOrderRow.GetRow(DB, OrderId).Guid
        Dim AjaxManager As ScriptManager = Me.Parent.FindControl("AjaxManager")
        Dim URL As String

        If SysParam.GetValue(DB, "IsPackingListPDF") = "1" Then
            URL = AppSettings("PDFServer") & "/print.asp?OrderId=" & Server.UrlEncode(OrderId) & "&Items=" & Server.UrlEncode(OrderItemId) & "&OrderGuid=" & Server.UrlEncode(OrderGuid)
        Else
            URL = "/admin/store/orders/PrintPackingList.aspx?OrderId=" & Server.UrlEncode(OrderId) & "&Items=" & Server.UrlEncode(OrderItemId) & "&OrderGuid=" & Server.UrlEncode(OrderGuid)
        End If

        AjaxManager.RegisterClientScriptBlock(Me.Page, Me.GetType, "printpackinglist", "PrintPackingList('" & URL & "')", True)
    End Sub


    Protected Sub btnSendStatus_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSendStatus.Click
        IF NOT ValidateAndSaveForm() Then Exit Sub
        Dim AjaxManager As ScriptManager = Me.Parent.FindControl("AjaxManager")
        AjaxManager.RegisterClientScriptBlock(Me.Page, Me.GetType, "sendstatus", "popSendStatus()", True)
    End Sub


    Protected Sub drpIsShippingIndividually_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim RecipientId As Integer = sender.Attributes("RecipientId")

        For Each ritem As RepeaterItem In Me.rptRecipients.Items
            Dim trAdminShippingStatus As HtmlTableRow = ritem.FindControl("trAdminShippingStatus")
            If CInt(trAdminShippingStatus.Attributes("RecipientId")) = RecipientId Then

                ritem.FindControl("tblRecipientStatusFields").Visible = (sender.SelectedValue = "false")

                Dim rptCart As Repeater = ritem.FindControl("rptCart")
                For Each cartItem As RepeaterItem In rptCart.Items
                    cartItem.FindControl("tblItemStatusFields").Visible = (sender.selectedValue = "true")
                Next

                Exit For
            End If '
        Next
        SetRecipientLevelShipmentDateTrackingNumber()
    End Sub
    Protected Sub drpRecipientStatus_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)

        SetRecipientLevelShipmentDateTrackingNumber()
        SetStatus()

    End Sub

    Private Sub SetRecipientLevelShipmentDateTrackingNumber()
        For Each ritem As RepeaterItem In Me.rptRecipients.Items
            Dim drpIsShippingIndividually As DropDownList = ritem.FindControl("drpIsShippingIndividually")
            Dim tdRecipientTrackingNumber As HtmlTableCell = ritem.FindControl("tdRecipientTrackingNumber")
            Dim drpShipmentStatus As DropDownList = ritem.FindControl("drpShipmentStatus")
            If drpIsShippingIndividually.SelectedValue = "true" Then
                tdRecipientTrackingNumber.Visible = False
                ritem.FindControl("tdRecipientShippedDate").Visible = False
            Else
                tdRecipientTrackingNumber.Visible = drpShipmentStatus.SelectedValue = "S"
                ritem.FindControl("tdRecipientShippedDate").Visible = drpShipmentStatus.SelectedValue = "S"
            End If

            If drpShipmentStatus.SelectedValue <> "S" Then
                CType(ritem.FindControl("dpRecipientShippedDate"), Controls.DatePicker).Value = Nothing
                CType(ritem.FindControl("txtRecipientTrackingNumber"), TextBox).Text = ""
            End If

        Next
    End Sub

    Protected Sub drpItemStatus_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim RecipientId As Integer = sender.Attributes("RecipientId")
        Dim OrderItemId As Integer = sender.Attributes("OrderItemId")
        For Each ritem As RepeaterItem In Me.rptRecipients.Items
            Dim tdRecipientTrackingNumber As HtmlTableCell = ritem.FindControl("tdRecipientTrackingNumber")
            If CInt(tdRecipientTrackingNumber.Attributes("RecipientId")) = RecipientId Then
                ritem.FindControl("tblRecipientStatusFields").Visible = (sender.SelectedValue = "false")

                Dim rptCart As Repeater = ritem.FindControl("rptCart")
                For Each cartItem As RepeaterItem In rptCart.Items
                    Dim drpItemStatus As DropDownList = cartItem.FindControl("drpItemStatus")
                    If CInt(drpItemStatus.Attributes("OrderItemId")) = OrderItemId Then
                        cartItem.FindControl("tdShippedDate").Visible = (sender.SelectedValue = "S")
                        cartItem.FindControl("tdItemTrackingNumber").Visible = (sender.SelectedValue = "S")
                    End If
                    If sender.selectedValue <> "S" Then
                        CType(sender.FindControl("dpShippedDate"), Controls.DatePicker).Value = Nothing
                        CType(sender.FindControl("txtTrackingNumber"), TextBox).Text = ""
                    End If
                Next
                Exit For
            End If '
        Next

        SetStatus()

    End Sub

    Private Sub SetStatus()
        Dim ItemStatus As String = String.Empty, RecipientStatus As String = String.Empty
        Dim NumberOfItemsProcessing As Integer = 0, NumberOfRecipientsProcessing As Integer = 0
        Dim NumberOfItemsCompleted As Integer = 0, NumberOfRecipientsCompleted As Integer = 0
        Dim NumberOfItems As Integer = 0, NumberOfRecipients As Integer = 0
        Dim IsItemFirstItem As Boolean = True, IsRecipientFirstItem As Boolean = True
        Dim dvStatus As DataView = StoreOrderStatusRow.GetAllStoreOrderStatuss(DB).DefaultView

        NumberOfRecipients = Me.rptRecipients.Items.Count

        For Each ritem As RepeaterItem In Me.rptRecipients.Items
            Dim drpShipmentStatus As DropDownList = ritem.FindControl("drpShipmentStatus")
            Dim drpIsShippingIndividually As DropDownList = ritem.FindControl("drpIsShippingIndividually")
            Dim rptCart As Repeater = ritem.FindControl("rptCart")
            NumberOfItems = rptCart.Items.Count
            IsItemFirstItem = True
            NumberOfItemsCompleted = 0
            NumberOfItemsProcessing = 0
            ItemStatus = String.Empty

            If drpIsShippingIndividually.SelectedValue = "true" Then
                'set recipient level status, 
                For Each cartItem As RepeaterItem In rptCart.Items
                    Dim drpItemStatus As DropDownList = cartItem.FindControl("drpItemStatus")
                    dvStatus.RowFilter = "Code = " & DB.Quote(drpItemStatus.SelectedValue)

                    If IsItemFirstItem Then
                        ItemStatus = drpItemStatus.SelectedValue
                        IsItemFirstItem = False
                    Else
                        If ItemStatus <> drpItemStatus.SelectedValue Then
                            ItemStatus = String.Empty
                        End If
                    End If

                    If dvStatus(0)("IsFinalAction") Then
                        NumberOfItemsCompleted += 1
                    Else
                        NumberOfItemsProcessing += 1
                    End If
                Next
                If NumberOfItemsCompleted = 0 Then
                    If ItemStatus = "" Then
                        ItemStatus = "P"
                    End If
                Else
                    If NumberOfItems = NumberOfItemsCompleted Then
                        If ItemStatus = String.Empty Then
                            ItemStatus = "X"
                        End If
                    Else
                        ItemStatus = "P"
                    End If
                End If
                drpShipmentStatus.SelectedValue = ItemStatus
                CType(ritem.FindControl("txtRecipientTrackingNumber"), TextBox).Text = ""
                CType(ritem.FindControl("dpRecipientShippedDate"), Controls.DatePicker).Value = Nothing
            Else
                ItemStatus = drpShipmentStatus.SelectedValue
                For Each cartItem As RepeaterItem In rptCart.Items
                    Dim drpItemStatus As DropDownList = cartItem.FindControl("drpItemStatus")
                    Dim dpShippedDate As Controls.DatePicker = cartItem.FindControl("dpShippedDate")
                    Dim txtTrackingNumber As TextBox = cartItem.FindControl("txtTRackingNumber")

                    drpItemStatus.SelectedValue = drpShipmentStatus.SelectedValue

                    If drpShipmentStatus.SelectedValue = "S" Then
                        txtTrackingNumber.Text = CType(ritem.FindControl("txtRecipientTrackingNumber"), TextBox).Text
                        dpShippedDate.Value = CType(ritem.FindControl("dpRecipientShippedDate"), Controls.DatePicker).Value
                    Else
                        txtTrackingNumber.Text = ""
                        dpShippedDate.Value = Nothing
                    End If
                Next
            End If
            dvStatus.RowFilter = "Code = " & DB.Quote(drpShipmentStatus.SelectedValue)
            If IsRecipientFirstItem Then
                RecipientStatus = drpShipmentStatus.SelectedValue
                IsRecipientFirstItem = False
            Else
                If RecipientStatus <> drpShipmentStatus.SelectedValue Then
                    RecipientStatus = String.Empty
                End If
            End If

            If dvStatus(0)("IsFinalAction") Then
                NumberOfRecipientsCompleted += 1
            Else
                NumberOfRecipientsProcessing += 1
            End If
        Next
        If NumberOfRecipientsCompleted = 0 Then
            If RecipientStatus = String.Empty Then
                RecipientStatus = "P"
            End If
        Else
            If NumberOfRecipients = NumberOfRecipientsCompleted Then
                If RecipientStatus = String.Empty Then
                    RecipientStatus = "X"
                End If
            Else
                RecipientStatus = "P"
            End If
        End If
        Me.radOrderStatus.SelectedValue = RecipientStatus
    End Sub
End Class
