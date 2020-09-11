<%@ Control Language="VB" EnableViewState="False" AutoEventWireup="false" CodeFile="StoreShoppingCartStatus.ascx.vb" Inherits="StoreShoppingCartStatus" %>
<%@ Import Namespace="Components" %>

<table cellspacing="0" cellpadding="0" border="0" class="carttbl" style="width:600px;">
<asp:Repeater runat="server" id="rptRecipients">
<ItemTemplate>
<tr id="trMultipleShipTo" runat="server"><td colspan="2" class="bdrbottom hdng2">For <%#Container.DataItem("Label")%><%#IIf(IsDBNull(Container.DataItem("AddressId")), "", " (address book)")%></td></tr>
<tr id="trSingleShipTo" runat="server"><td colspan="2" class="bdrbottom hdng2">&nbsp;</td></tr>
<tr><td colspan="2" class="bdrleft bdrright bdrbottom">
	<table border="0" width="100%">
	<tr><td>
	<b>Ship To:&nbsp;&nbsp;&nbsp;</b>
	</td><td>
	<div><asp:Literal ID="ltlFullName" runat="server" EnableViewState="False" /></div>
	<div id="divCompany" runat="server"><%#Core.HTMLEncode(Container.DataItem("Company"))%></div>
	<div><%#Core.HTMLEncode(Container.DataItem("Address1"))%></div>
	<div id="divAddress2" runat="server"><%#Core.HTmlEncode(Container.DataItem("Address2"))%></div>
	<div><%#Core.HTMLEncode(Container.DataItem("City"))%>, <%#Core.HTMLEncode(Container.DataItem("State"))%>&nbsp;<%#Core.HTMLEncode(Container.DataItem("Zip"))%></div>
	<div id="divRegion" runat="server"><%#Core.HTMLEncode(Container.DataItem("Region"))%></div>
	<div><asp:Literal ID="ltlCountry" runat="server" EnableViewState="False" /></div>
	<div>Phone: <%#Core.HTMLEncode(Container.DataItem("PHONE"))%></div>

	</td><td style="vertical-align: bottom;">&nbsp;&nbsp;&nbsp;</td><td style="vertical-align: bottom;">

	<div runat="server" id="divShippingMethod"></div>
	</td>
	</tr>
	</table>
</td></tr>
<tr>
<th>Item</th>
<th id="tdQuantity" runat="server" style="width:100px;">Quantity</th>
</tr>
<asp:Repeater runat="server" id="rptCart">
	<ItemTemplate>
    <tr valign="top">
    <td id="tdDetails" class="bdrbottom bdrleft" runat="server">
		<table border="0">
		<tr>
		<td id="tdImage" runat="server" align="center" valign="top">
		<img id="img" src="" runat="server" alt="" border="0" /><br />
		</td>
		<td valign="top">
		<b><%#Container.DataItem("ItemName")%></b><br />
		Item no: <%#Container.DataItem("SKU")%><br /><br />
		<span id="spanAttributesWrapper" runat="server">Attributes:<br /><span id="spanAttributes" runat="server" class="smallest"></span></span>
		<span id="spanShipStatus" runat="server"></span>
		</td>
		</tr>
		</table>
	</td>
	<td id="tdQuantity" runat="server" class="bdrbottom"  style="width:100px;"><%#Container.DataItem("Quantity")%></td>
    </tr>    	
	</ItemTemplate>
</asp:Repeater>

</ItemTemplate>

<SeparatorTemplate>
<tr><td colspan="2">&nbsp;</td></tr>
</SeparatorTemplate>

</asp:Repeater>

</table>
