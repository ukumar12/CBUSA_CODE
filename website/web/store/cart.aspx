<%@ Page Language="VB" AutoEventWireup="false" CodeFile="cart.aspx.vb" Inherits="cart" %>
<%@ Import Namespace="System.Configuration.ConfigurationManager" %>

<CT:masterpage runat="server" ID="CTMain">

<h1 class="hdng">Shopping Cart</h1>

<asp:Panel runat="server" id="pnlCart" defaultbutton="btnUpdate">

<table cellspacing="0" cellpadding="0" border="0" class="carttbl">
<tr id="trGiftWrapping" runat="server" visible="False"><td colspan="5" class="right"><span class="bold">Gift Wrapping</span>: <asp:Literal id="ltlGiftWrappingPrice" runat="server"></asp:Literal> per item</td></tr>
<tr id="trWishlist" runat="server" visible="False"><td colspan="5" class="right"><a href="<%=AppSettings("GlobalSecureName") %>/members/wishlist/">View Your Wish List</a></td></tr>

<asp:Repeater runat="server" id="rptRecipients">
<ItemTemplate>
<tr id="trMultipleShipTo" runat="server"><td colspan="5" class="hdngbox">For <%#Container.DataItem("Label")%><%#IIf(IsDBNull(Container.DataItem("AddressId")), "", " (address book)")%></td></tr>
<tr id="trSingleShipTo" runat="server"><td colspan="5" class="hdngbox">&nbsp;</td></tr>
<tr>
<th style="width:50%;">Item</th>
<th id="tdQuantity" runat="server">Quantity</th>
<th id="tdGiftWrap" runat="server">Gift Wrap</th>
<th>Price</th>
<th>Total</th>
</tr>

<asp:Repeater runat="server" id="rptCart">
	<ItemTemplate>
    <tr valign="top">
    <td id="tdDetails" class="bdrbottom bdrleft" width="300" runat="server">
		<table border="0">
		<tr>
		<td id="tdImage" runat="server" align="center" valign="top">
		<a id="lnk1" runat="server" href=""><img id="img" src="" runat="server" alt="" /></a><br />
		<a id="lnk2" runat="server" href="" class="smaller">Edit Item</a>
		</td>
		<td valign="top">
		<a id="lnk3" runat="server" href=""><b><%#Container.DataItem("ItemName")%></b></a><br />
		Item no: <%#Container.DataItem("SKU")%><br /><br />
		<span id="spanAttributesWrapper" runat="server">Attributes:<br /><span id="spanAttributes" runat="server" class="smaller"></span></span>
		</td>
		</tr>
		</table>
	</td>
	<td id="tdQuantity" runat="server" class="bdrbottom" width="100">
	    <asp:TextBox CssClass="qtybox lnpad4" runat="server" id="txtQty" Text='<%#Container.DataItem("Quantity")%>' Columns="3" MaxLength="3" style="width:20px;text-align:center;" /><br />
	    <CC:IntegerValidator runat="server" ID="ivQuantity" Display="dynamic" EnableClientScript="false" ControlToValidate="txtQty" Text="Invalid Quantity" ErrorMessage="Please enter valid quantity value" />
	    <CC:ConfirmLinkButton CssClass="smaller" CommandName="Remove" CommandArgument='<%#Container.DataItem("OrderItemId")%>' runat="server" id="lnkRemove" Text="Remove" Message="Are you sure you want to remove this item from your cart" /><br />
	<asp:LinkButton CssClass="smaller" CommandName="Move" CommandArgument='<%#Container.DataItem("OrderItemId")%>' runat="server" id="lnkMove" Text="Move to wishlist"/><br />
	</td>
	<td id="tdGiftWrap" runat="server" class="bdrbottom" >
    <span id="spanGiftWrapNotApplicable" runat="server">N/A</span>
	<table border="0" cellpadding="0" cellspacing="0" id="tblGiftWrap" runat="server">
		<tr>
		    <td><asp:checkbox id="chkGiftWrap" runat="server"></asp:checkbox></td>
			<td><div id="divGiftWrap" runat="server"><asp:TextBox CssClass="qtybox" runat="server" id="txtGiftQty" Columns="3" MaxLength="3" style="width:20px; text-align:center;" /></div></td>
		</tr>
	</table>
    <CC:IntegerValidator runat="server" ID="ivGiftWrapQuantity" Display="dynamic" EnableClientScript="false" ControlToValidate="txtGiftQty" Text="Invalid Quantity" ErrorMessage="Please enter valid quantity value" />
	</td>
	<td id="tdItemPrice" runat="server" class="bdrbottom"><%#FormatCurrency(Container.DataItem("Price"))%></td>
	<td id="tdPrice" runat="server" class="bdrright bdrbottom"><%#FormatCurrency(Container.DataItem("Price") * Container.DataItem("Quantity"))%></td>
    </tr>    	
	</ItemTemplate>
</asp:Repeater>

<tr id="trRecipientSummary" runat="server">
<td id="tdBottom1" runat="server" colspan="4" class="bdrleft right">Merchandise Subtotal:</td>
<td id="tdBottom2" runat="server" class="bdrright right"><%#FormatCurrency(Container.DataItem("BaseSubtotal"))%></td>
</tr>
<tr id="trGiftWrappingBottom" runat="server">
<td colspan="4" class="bdrleft right">Gift Wrapping:</td>
<td id="tdBottom3" class="bdrright right"><%#FormatCurrency(Container.DataItem("GiftWrapping"))%></td>
</tr>
<tr id="trDiscountBottom" runat="server">
<td colspan="4" class="bdrleft right">Discount:</td>
<td class="bdrright"><%#FormatCurrency(Container.DataItem("Discount"))%></td>
</tr>
<tr id="trTotal" runat="server">
<td colspan="4" class="bdrleft bdrbottom right">Total:</td>
<td class="bdrright bdrbottom right"><%#FormatCurrency(Container.DataItem("SubTotal")+Container.DataItem("GiftWrapping"))%></td>
</tr>
</ItemTemplate>

<SeparatorTemplate>
<tr><td colspan="5">&nbsp;</td></tr>
</SeparatorTemplate>

</asp:Repeater>

<tr><td colspan="5">&nbsp;</td></tr>
<tr><td colspan="5" class="baghdr bdr">

<table cellpadding="5" cellspacing="0" border="0" width="100%">
<tr>
<td><CC:OneClickButton runat="server" cssClass="btn" id="btnUpdate" Text="Update Cart"/></td>
<td align="right">
If you have a Promotion or Source Code, please enter it here: <asp:TextBox runat="server" id="txtPromotionCode" />
<br />Please click the "Update Cart" button to recalculate all shopping cart values.
<div class="red"><asp:Literal runat="server" id="ltlPromotionMessage" /></div>
</td></tr>
</table>

</td></tr>

<tr>
<td rowspan="5" colspan="2" class="top">

<table cellpadding="0" cellspacing="0" border="0">
<tr><td colspan="2" class="bold">CONTINUE SHOPPING</td></tr>
<tr><td>&nbsp;</td><td><a href="/store/">Home</a></td></tr>
<tr id="trLastDepartmentId" visible="false" runat="server"><td>&nbsp;</td><td><a id="lnkLastDepartment" runat="server"></a></td></tr>
<tr id="trLastBrandId" visible="false" runat="server"><td>&nbsp;</td><td><a id="lnkLastBrand" runat="server"></a></td></tr>
<tr id="trLastItemId" visible="false" runat="server"><td>&nbsp;</td><td><a id="lnkLastItem" runat="server"></a></td></tr>
</table>

</td>
<td colspan="2" class="top right bold">Merchandise Subtotal:</td>
<td class="top right bold"><% =FormatCurrency(dbOrder.BaseSubtotal)%></td>
</tr>
<%  If dbOrder.GiftWrapping > 0 Then%>
<tr>
<td colspan="2" class="top right bold">Gift Wrapping:</td>
<td class="top right bold"><% =FormatCurrency(dbOrder.GiftWrapping)%></td>
</tr>
<% End If%>
<%  If dbOrder.Discount > 0 Then%>
<tr>
<td colspan="2" class="top right bold">Discount:</td>
<td class="top right bold"><% =FormatCurrency(dbOrder.Discount)%></td>
</tr>
<% End If%>
<%  If dbOrder.GiftWrapping > 0 Or dbOrder.Discount > 0 Then%>
<tr>
<td colspan="2" class="top right bold">Total:</td>
<td class="top right bold"><% =FormatCurrency(dbOrder.Subtotal + dbOrder.GiftWrapping)%></td>
</tr>
<% End If%>

<tr><td colspan="5">&nbsp;</td></tr>
<tr><td colspan="5" class="right"><CC:OneClickButton runat="server" cssClass="btncheckout" id="btnCheckout" Text="Proceed to Checkout &raquo;&raquo;" /></td></tr>
</table>
</asp:Panel>

<asp:Panel runat="server" id="pnlEmptyCart" visible="False">
<p>&nbsp;</p>
<p class="bold">There are currently no items in your shopping cart.</p>
<p class="italic">Your web browser's "cookie" feature must be enabled to use the shopping cart. If you added an item and received this message, it is possible the cookie is being blocked. Please adjust your browser's privacy settings to allow acceptance of 1st party cookies and try again.</p>
</asp:Panel>

</CT:masterpage>