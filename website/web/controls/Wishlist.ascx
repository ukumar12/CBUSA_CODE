<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Wishlist.ascx.vb" Inherits="WishlistCtrl" %>
<%@ Import Namespace="Components" %>

<asp:Panel runat="server" id="pnlWishlist">

<table width="100%" cellspacing="0" cellpadding="4" border="0" style="margin-top:4px; margin-left:3px;">
<tr><td colspan="5" class="bdrbottom hdng2">&nbsp;</td></tr>
<tr>
<td class="baghdr bdrleft">&nbsp;</td>
<td class="baghdr"><b>Item</b></td>
<td class="baghdr"><b>Quantity</b></td>
<td class="baghdr" align="right"><b>Price</b></td>
<td class="baghdr bdrright" align="right"><b>Total</b></td>
</tr>
<asp:Repeater runat="server" id="rptWishlist">
<ItemTemplate>
<tr valign="top">
<td id="tdBuy" runat="server" class="bagtd bdrleft" width="50" align="center">
<CC:OneClickButton CssClass="btn" runat="server" id="btnBuy" CommandName="Buy" CommandArgument='<%#Container.DataItem("WishlistItemId")%>' Text="Buy" />
</td>
<td id="tdDetails1" runat="server" class="bagtd" width="300">
	<table border="0">
	<tr>
	<td id="tdImage" runat="server" align="center" valign="top">
	<a id="lnk1" runat="server" href=""><img id="img" src="" runat="server" alt="" /></a><br />
	</td>
	<td valign="top">
	<a id="lnk3" runat="server" href=""><b><%#Container.DataItem("ItemName")%></b></a><br />
	Item no: <%#Container.DataItem("SKU")%><br /><br />
	Attributes:<br /><span id="spanAttributes" runat="server" class="smallest"></span>
	</td>
	</tr>
	</table>
</td>
<td id="tdDetails2" runat="server" class="bagtd" width="100"><%#Container.DataItem("Quantity")%></td>
<td id="tdItemPrice" runat="server" class="bagtd" align="right"><%#FormatCurrency(Container.DataItem("Price"))%></td>
<td id="tdPrice" runat="server" class="bagtd bdrright" align="right"><%#FormatCurrency(Container.DataItem("Price") * Container.DataItem("Quantity"))%></td>
</tr>    	
</ItemTemplate>
</asp:Repeater>
<tr><td colspan="5">&nbsp;</td></tr>
</table>

</asp:Panel>

<asp:Panel runat="server" id="pnlEmptyWishlist" visible="False">
<div class="spacer">&nbsp;</div>
<p class="bold">There are currently no items in your wish list.</p>
</asp:Panel>
