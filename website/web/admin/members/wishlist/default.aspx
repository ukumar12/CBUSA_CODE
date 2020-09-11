<%@ Page Language="vb" AutoEventWireup="false" Inherits="member_wishlist_default" MasterPageFile="~/controls/AdminMaster.master" CodeFile="default.aspx.vb" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
<h4>View/Edit Wishlist</h4>
Wishlist for <asp:Label ID="txtMemberName" runat="server"></asp:Label>
|<a runat="server" id="lnkBack">&laquo; Go Back to Member Profile</a><br /><br />
<asp:Panel runat="server" id="pnlWishlist" defaultbutton="btnUpdate">
<table cellspacing="0" cellpadding="0" border="0" >
<tr><td colspan="5">&nbsp;</td></tr>
<tr>
<th align="left">Item</th>
<th align="left">Quantity</th>
<th align="left">Price</th>
<th align="left">Total</th>
</tr>
<asp:Repeater runat="server" id="rptWishlist">
<ItemTemplate>
<tr>
<td id="tdDetails1" runat="server" width="300">
	<table border="0">
	<tr>
	<td id="tdImage" runat="server" align="center" valign="top">
	<img id="img" src="" runat="server" alt="" /><br />
	</td>
	<td valign="top">
	<b><%#Container.DataItem("ItemName")%></b><br />
	Item no: <%#Container.DataItem("SKU")%><br /><br />
	Attributes:<br /><span id="spanAttributes" runat="server" class="smallest"></span>
	</td>
	</tr>
	</table>
</td>
<td id="tdDetails2" runat="server" width="200">
<asp:TextBox CssClass="qtybox" runat="server" id="txtQty" Text='<%#Container.DataItem("Quantity")%>' Columns="3" MaxLength="3" style="width:20px;text-align:center;" /><br />
<CC:IntegerValidator runat="server" ID="ivQuantity" Display="dynamic" ControlToValidate="txtQty" Text="Invalid Quantity" ErrorMessage="Please enter valid quantity value" />
<CC:ConfirmLinkButton CssClass="smallest" CommandName="Remove" CommandArgument='<%#Container.DataItem("WishlistItemId")%>' runat="server" id="lnkRemove" Text="Remove" Message="Are you sure you want to remove this item from your wish list" /><br />
</td>
<td id="tdItemPrice" runat="server" width="50" class="field"><%#FormatCurrency(Container.DataItem("Price"))%></td>
<td id="tdPrice" runat="server" width="50" class="field"><%#FormatCurrency(Container.DataItem("Price") * Container.DataItem("Quantity"))%></td>
</tr>    	
</ItemTemplate>
</asp:Repeater>

<tr><td colspan="5">&nbsp;</td></tr>
<tr><td colspan="5">

</td></tr>

</table>

<p></p>
<CC:OneClickButton runat="server" cssClass="btn" id="btnUpdate" Text="Update Wishlist"/>
<CC:OneClickButton runat="server" cssClass="btn" id="btnSend" Text="Send Wishlist &raquo;"/>

</asp:Panel>

<asp:Panel runat="server" id="pnlEmptyWishlist" visible="False">
<br />
There are currently no items in your wish list.
</asp:Panel>


</asp:content> 