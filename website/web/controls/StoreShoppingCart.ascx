<%@ Control Language="VB" EnableViewState="False" AutoEventWireup="false" CodeFile="StoreShoppingCart.ascx.vb" Inherits="StoreShoppingCart" %>
<%@ Import Namespace="Components" %>

<table cellspacing="0" cellpadding="0" border="0" class="carttbl">
<tr id="trGiftWrapping" runat="server" visible="False"><td colspan="5" align="right"><b>Gift Wrapping</b>: <asp:Literal id="ltlGiftWrappingPrice" runat="server"></asp:Literal> per item</td></tr>

<asp:Repeater runat="server" id="rptRecipients">
<ItemTemplate>
<tr id="trMultipleShipTo" runat="server"><td colspan="5" class="bdrbottom hdng2">For <%#Container.DataItem("Label")%><%#IIf(IsDBNull(Container.DataItem("AddressId")), "", " (address book)")%></td></tr>
<tr id="trSingleShipTo" runat="server"><td colspan="5" class="bdrbottom hdng2">&nbsp;</td></tr>
<tr><td colspan="5" class="bdrleft bdrright bdrbottom">
	<table border="0">
	<tr><td>
	<b>Ship To:&nbsp;&nbsp;&nbsp;</b>
	</td><td>
	<div><asp:Literal ID="ltlFullName" runat="server" EnableViewState="False" /></div>
	<div id="divCompany" runat="server"><%#Core.HTMLEncode(Container.DataItem("Company"))%></div>
	<%If IsFullCart Then%><div><%#Core.HTMLEncode(Container.DataItem("Address1"))%></div><%End If%>
	<div id="divAddress2" runat="server"><%#Core.HTmlEncode(Container.DataItem("Address2"))%></div>
	<div><%#Core.HTMLEncode(Container.DataItem("City"))%>, <%#Core.HTMLEncode(Container.DataItem("State"))%>&nbsp;<%#Core.HTMLEncode(Container.DataItem("Zip"))%></div>
	<div id="divRegion" runat="server"><%#Core.HTMLEncode(Container.DataItem("Region"))%></div>
	<div><asp:Literal ID="ltlCountry" runat="server" EnableViewState="False" /></div>
	<%If IsFullCart Then%><div>Phone: <%#Core.HTMLEncode(Container.DataItem("PHONE"))%></div><%End If%>

	</td><td style="vertical-align: bottom;">&nbsp;&nbsp;&nbsp;</td><td style="vertical-align: bottom;">

	<div runat="server" id="divShippingMethod"></div>

	<% If EditMode Then%>
	<div style="margin-top:10px;"><input type="button" class="btn" id="btnEdit" value="Edit Shipping" runat="server"></div>
	<% end if %>

	</td>
	<td>&nbsp;&nbsp;&nbsp;</td>
	<td id="tdGiftMessageLabel" runat="server">
	<b>Gift Message:&nbsp;&nbsp;&nbsp;</b>
	</td>
	<td id="tdGiftMessage" runat="server">
		<div><asp:Literal Id="ltlGiftMessage" runat="server" /></div>
    </td></tr>
	</table>
</td></tr>
<tr>
<th>Item</th>
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
		<a id="lnk1" runat="server" href=""><img id="img" src="" runat="server" alt="" border="0" /></a><br />
		<a id="lnk2" runat="server" href="" class="smallest">Edit Item</a>
		</td>
		<td valign="top">
		<a id="lnk3" runat="server" href=""><b><%#Container.DataItem("ItemName")%></b></a><br />
		Item no: <%#Container.DataItem("SKU")%><br /><br />
		<span id="spanAttributesWrapper" runat="server">Attributes:<br /><span id="spanAttributes" runat="server" class="smallest"></span></span>
		<span id="spanShipStatus" runat="server"></span>
		</td>
		</tr>
		</table>
	</td>
	<td id="tdQuantity" runat="server" class="bdrbottom"><%#Container.DataItem("Quantity")%></td>
	<td id="tdGiftWrap" runat="server" class="bdrbottom">&nbsp;</td>
	<td id="tdItemPrice" runat="server" class="bdrbottom"><%#FormatCurrency(Container.DataItem("Price"))%></td>
	<td id="tdPrice" runat="server" class="bdrright bdrbottom"><%#FormatCurrency(Container.DataItem("Price") * Container.DataItem("Quantity"))%></td>
    </tr>    	
	</ItemTemplate>
</asp:Repeater>

<tr id="trRecipientSummary" runat="server">
<td id="Td1" runat="server" colspan="4" class="bdrleft right">Merchandise Subtotal:</td>
<td id="Td2" runat="server" class="bdrright right"><%#FormatCurrency(Container.DataItem("BaseSubtotal"))%></td>
</tr>
<tr id="trGiftWrappingBottom" runat="server">
<td colspan="4" class="bdrleft right">Gift Wrapping:</td>
<td id="tdBottom3" class="bdrright right"><%#FormatCurrency(Container.DataItem("GiftWrapping"))%></td>
</tr>
<tr id="trDiscountBottom" runat="server">
<td colspan="4" class="bdrleft right">Discount:</td>
<td class="bdrright right"><%#FormatCurrency(Container.DataItem("Discount"))%></td>
</tr>
<tr id="trShippingBottom" runat="server">
<td colspan="4" class="bdrleft right">Shipping:</td>
<td class="bdrright right"><%#FormatCurrency(Container.DataItem("Shipping"))%></td>
</tr>
<tr id="trTaxBottom" runat="server">
<td colspan="4" class="bdrleft right">Tax:</td>
<td class="bdrright right"><%#FormatCurrency(Container.DataItem("Tax"))%></td>
</tr>
<tr id="trTotalBottom" runat="server">
<td colspan="4" class="bdrleft right bdrbottom">Total:</td>
<td class="bdrright right bdrbottom"><%#FormatCurrency(Container.DataItem("Total"))%></td>
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
<td>TOTAL</td>
<td align="right">
<div class="red"><asp:Literal runat="server" id="ltlPromotionMessage" /></div>
</td></tr>
</table>

</td></tr>

<tr>
<td rowspan="5" colspan="2" valign="top">
&nbsp;
</td>
<td colspan="2" class="top right bold">Merchandise Subtotal:</td>
<td align="right" valign="top"><% =FormatCurrency(dbOrder.BaseSubtotal)%></td>
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
<tr>
<td colspan="2" class="top right bold">Shipping:</td>
<td class="top right bold"><% =FormatCurrency(dbOrder.Shipping)%></td>
</tr>
<tr>
<td colspan="2" class="top right bold">Tax:</td>
<td class="top right bold"><% =FormatCurrency(dbOrder.Tax)%></td>
</tr>
<tr>
<td colspan="2" class="top right bold">Total:</td>
<td class="top right bold"><% =FormatCurrency(dbOrder.Total)%></td>
</tr>
</table>
