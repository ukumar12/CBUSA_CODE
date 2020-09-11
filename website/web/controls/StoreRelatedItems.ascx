<%@ Control Language="VB" AutoEventWireup="false" CodeFile="StoreRelatedItems.ascx.vb" Inherits="StoreRelatedItems" %>
<div class="largest bold">Related Items:</div>
<div class="reltdwrpr">
<asp:Repeater runat="server" id="rptRelated">
	<ItemTemplate>
		<a href="" runat="server" id="lnkItem">
			<img src='/assets/item/related/<%#Container.DataItem("Image")%>' alt="<%# Server.HTMLEncode(Container.DataItem("ItemName")) %>" /><br />
			<span class="itemname"><%#Container.DataItem("ItemName")%><br />
			<strong id="ltlPrice" runat="server"></strong>
		</a>
	</ItemTemplate>
</asp:Repeater>
</div>
<div style="clear:both;"></div>