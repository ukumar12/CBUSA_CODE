<%@ Page Language="vb" AutoEventWireup="false" Inherits="member_wishlist_default" CodeFile="default.aspx.vb" %>

<CT:masterpage runat="server" id="CTMain">
<h2 class="hdng">My Wishlist</h2>

<asp:Panel runat="server" id="pnlWishlist" defaultbutton="btnUpdate">

<table cellspacing="0" cellpadding="0" border="0" class="carttbl">
<tr><td colspan="5" class="right"><a href="/store/cart.aspx">View Shopping Cart</a></td></tr>
<tr><td colspan="5" class="bdrbottom hdng2">&nbsp;</td></tr>
<tr>
<th>Item</th>
<th>Quantity</th>
<th>Price</th>
<th>Total</th>
</tr>
<asp:Repeater runat="server" id="rptWishlist">
<ItemTemplate>
<tr>
<td id="tdDetails1" runat="server" class="bagtd bdrleft" width="300">
	<table border="0">
	<tr>
	<td id="tdImage" runat="server" align="center" valign="top">
	<a id="lnk1" runat="server" href=""><img id="img" src="" runat="server" alt="" /></a><br />
	<a id="lnk2" runat="server" href="" class="smallest">Edit Item</a>
	</td>
	<td valign="top">
	<a id="lnk3" runat="server" href=""><b><%#Container.DataItem("ItemName")%></b></a><br />
	Item no: <%#Container.DataItem("SKU")%><br /><br />
	Attributes:<br /><span id="spanAttributes" runat="server" class="smallest"></span>
	</td>
	</tr>
	</table>
</td>
<td id="tdDetails2" runat="server" class="bagtd" width="100">
<asp:TextBox CssClass="qtybox" runat="server" id="txtQty" Text='<%#Container.DataItem("Quantity")%>' Columns="3" MaxLength="3" style="width:20px;text-align:center;" /><br />
<CC:IntegerValidator runat="server" ID="ivQuantity" Display="dynamic" EnableClientScript="false" ControlToValidate="txtQty" Text="Invalid Quantity" ErrorMessage="Please enter valid quantity value" />
<CC:ConfirmLinkButton CssClass="smallest" CommandName="Remove" CommandArgument='<%#Container.DataItem("WishlistItemId")%>' runat="server" id="lnkRemove" Text="Remove" Message="Are you sure you want to remove this item from your wish list" /><br />
<asp:LinkButton CssClass="smallest" CommandName="Move" CommandArgument='<%#Container.DataItem("WishlistItemId")%>' runat="server" id="lnkMove" Text="Move to shopping cart"/><br />
</td>
<td id="tdItemPrice" runat="server" class="bagtd" align="right"><%#FormatCurrency(Container.DataItem("Price"))%></td>
<td id="tdPrice" runat="server" class="bagtd bdrright" align="right"><%#FormatCurrency(Container.DataItem("Price") * Container.DataItem("Quantity"))%></td>
</tr>    	
</ItemTemplate>
</asp:Repeater>

<tr><td colspan="5">&nbsp;</td></tr>
<tr><td colspan="5" class="baghdr bdr">

<table cellpadding="5" cellspacing="0" border="0" width="100%">
<tr>
<td><CC:OneClickButton runat="server" cssClass="btn" id="btnUpdate" Text="Update Wishlist"/></td>
</td>
<td class="right">
<CC:OneClickButton runat="server" cssClass="btn" id="btnSend" Text="Send Wishlist &raquo;"/>
</td>
</tr>
</table>

</td></tr>

<tr>
<td rowspan="5" colspan="2" valign="top">

<table cellpadding="3" cellspacing="3" style="margin-top:4px; margin-left:3px;">
<tr><td colspan="2" class="bold">CONTINUE SHOPPING</td></tr>
<tr><td>-</td><td><a href="/store/">Home</a></td></tr>
<tr id="trLastDepartmentId" visible="false" runat="server"><td>-</td><td><a id="lnkLastDepartment" runat="server"></a></td></tr>
<tr id="trLastBrandId" visible="false" runat="server"><td>-</td><td><a id="lnkLastBrand" runat="server"></a></td></tr>
<tr id="trLastItemId" visible="false" runat="server"><td>-</td><td><a id="lnkLastItem" runat="server"></a></td></tr>
</table>

</td>
</tr>
</table>
</asp:Panel>

<asp:Panel runat="server" id="pnlEmptyWishlist" visible="False">
<div class="spacer">&nbsp;</div>
<p class="bold">There are currently no items in your wish list.</p>
</asp:Panel>

</CT:masterpage>