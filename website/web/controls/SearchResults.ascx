<%@ Control Language="VB" AutoEventWireup="false" CodeFile="SearchResults.ascx.vb" Inherits="SearchItem" %>
<%@ Import Namespace="DataLayer" %>
<%@ Import Namespace="Components" %>
<table style="width:194px;" cellspacing="0" cellpadding="0" border="0" summary="item">
<tr>
<td colspan="2" class="bold center" style="width:194px; height:50px; padding:4px 10px 4px 10px;">
	<a href="<%=URL() %>"><%=Item.ItemName%></a>
</td>
</tr>
<tr>
<td colspan="2" class="center" style="width:194px; height:150px;">
<%  If Core.FileExists(Server.MapPath("/assets/item/thumbnail/" & Item.Image)) Then%>
	<a href="<%=URL() %>"><img src="/assets/item/thumbnail/<%=Item.Image %>" style="border-style:none" alt="" /></a><br />
<% Else %>
	<a href="<%=URL() %>"><img src="/assets/item/thumbnail/empty.jpg" style="border-style:none" alt="" /></a><br />
<% End if %>
</td>
</tr>
<tr>
<td colspan="2" class="center" style="width:194px; height:30px;">
    <asp:Literal ID="ltlPrice" runat="server" />
</td>
</tr>
<tr>
<td style="width:97px; text-align:right; padding:8px 4px 12px 0;">
	<a href="<%=URL() %>">Details</a><br />
</td>
<td style="width:97px; padding:8px 0 12px 4px;">
    <asp:Panel ID="pnlInStock" runat="server">
	<asp:ImageButton ID="btnAdd2Cart" runat="server" ImageUrl="/images/global/btn-add2cart.gif" alternatext="Add to Cart" />
	</asp:Panel>
	<asp:Panel ID="pnlNotInStock" runat="server">
	<asp:Literal ID="ltlStockMessage" runat="server" />
	</asp:Panel>
</td>
</tr>
</table>
