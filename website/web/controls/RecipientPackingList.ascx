<%@ Control Language="VB" AutoEventWireup="false" CodeFile="RecipientPackingList.ascx.vb" Inherits="RecipientPackingListControl" %>
<center><h2>SHIPPING RECEIPT</h2></center>

<table border=0 cellspacing=2 cellpadding=3 width="100%">
<tr><td valign=top nowrap width="50%">

<table border=0 cellspacing=2 cellpadding=3>
<tr>
	<td class=optional><b>Order#</b></td>
	<td><asp:Literal ID="ltlOrderNo" runat="server" /></td>
</tr><tr>
	<td class=optional><b>Date:</b></td>
	<td nowrap><asp:Literal ID="ltlProcessDate" runat="server" /></td>
</tr><tr>
	<td class=optional nowrap><b>Shipping Method</b></td>
	<td><asp:Literal ID="ltlShippingMethod" runat="server" /></td>
</tr>
</table>

</td><td valign=top align="right" width="50%">

<table align=right>
<tr><td>
<b class=largest>
<asp:Literal ID="ltlAddress" runat="server" />
</b>

</td></tr>
</table>

</td>
</tr></table>

<!-- Begin Address Details -->
<p>
	<table width="100%">
	<tr><td valign=top width="50%" align="center">
	<p><span style="font-size:14px; font-weight:bold;">BILLING ADDRESS</span>
	
	<table>
	<tr>
		<td width=70><b>Name:</b></td>
		<td><%= Server.HTMLEncode(dbOrder.BillingFirstName & " " & dbOrder.BillingLastName) %></td>
	</tr>
	<% If Not dbOrder.BillingCompany = Nothing Then%>
		<tr>
			<td width=70><b>Company:</b></td>
			<td><%= Server.HTMLEncode(dbOrder.BillingCompany) %></td>
		</tr>
	<%	end if %>
	<tr>
		<td><b>Address:</b></td>
		<td><%= Server.HTMLEncode(dbOrder.BillingAddress1) %></td>
	</tr>
	<%	if not dbOrder.BillingAddress2 = Nothing then %>
		<tr>
			<td><b>&#160;</b></td>
			<td><%= Server.HTMLEncode(dbOrder.BillingCity) %></td>
		</tr>
	<%	end if %>
	<tr>
		<td><b></b></td>
		<td><%=Server.HtmlEncode(dbOrder.BillingCity)%>, 
		 <asp:Literal ID="ltlBillingStateOrRegion" runat="server" />, 
		 <%=Server.HtmlEncode(dbOrder.BillingZip)%></td>
	</tr>
	<tr>
		<td><b>Country:</b></td>
		<td><asp:Literal ID="ltlBillingCountry" runat="server" /></td>
	</tr><tr>
		<td><b>Phone:</b></td>
		<td><%=Server.HtmlEncode(dbOrder.BillingPhone)%></td>
	</tr>	
	</table>
	
	</td><td valign="top" width="50%" align="center">
	
	<p><span style="font-size:14px; font-weight:bold;">SHIPPING ADDRESS</span>
	<table>
	<tr>
		<td width=60><b>Name:</b></td>
		<td><%= Server.HtmlEncode(dbRecipient.FirstName & " " & dbRecipient.LastName)%></td>
	</tr>
	<% If Not dbRecipient.Company = Nothing Then%>
		<tr>
			<td width=60><b>Company:</b></td>
			<td><%=Server.HtmlEncode(dbRecipient.Company)%></td>
		</tr>
	<%	end if %>
		<tr>
			<td width=60><b>Address:</b></td>
			<td><%=Server.HtmlEncode(dbRecipient.Address1)%></td>
		</tr>
	<% If Not dbRecipient.Address2 = Nothing Then%>
		<tr>
			<td width=60><b>&#160;</b></td>
			<td><%=Server.HtmlEncode(dbRecipient.Address2)%></td>
		</tr>
	<%	end if %>
	<tr>
		<td><b></b></td>
		<td><%=Server.HtmlEncode(dbRecipient.City)%>, 
		 <asp:Literal ID="ltlShippingStateOrRegion" runat="server" />, 
		 <%=Server.HTMLEncode(dbRecipient.Zip)%></td>
	</tr>
	<tr>
		<td><b>Country:</b></td>
		<td><asp:Literal ID="ltlShippingCountry" runat="server" /></td>
	</tr><tr>
		<td><b>Phone:</b></td>
		<td><%=Server.HtmlEncode(dbRecipient.Phone)%></td>
	</tr>
	</table>

	</td></tr>
	</table>
</p>
<p>
<asp:Repeater ID="rptCartItems" runat="server">
  <HeaderTemplate>
    <table width="100%" cellspacing="0" cellpadding="2">
      <tr>
      <th class="bdrleft bdrtop bdrbottom">Item</th>
      <th class="bdrleft bdrtop bdrbottom" runat="server">Quantity</th>
      <th class="bdrleft bdrtop bdrbottom" id="thGiftWrap" runat="server">Gift Wrap</th>
      <th class="bdrleft bdrtop bdrbottom">Status</th>
      <th class="bdrleft bdrtop bdrbottom">Price</th>
      <th class="bdr" >Total</th>  
      </tr>
  </HeaderTemplate>
  <ItemTemplate>
    <tr>
    <td class="bdrleft bdrbottom" align="center"><asp:Literal ID="ltlItem" runat="server" /></td>
    <td class="bdrleft bdrbottom" align="center"><asp:Literal ID="ltlQuantity" runat="server" /></td>
    <td class="bdrleft bdrbottom" id="tdGiftWrap" runat="server" align="center"><asp:Literal ID="ltlGiftWrap" runat="server" /></td>
    <td class="bdrleft bdrbottom" align="center"><asp:Literal ID="ltlStatus" runat="server" /></td>
    <td class="bdrleft bdrbottom" align="center"><asp:Literal ID="ltlPrice" runat="server" /></td>
    <td class="bdrleft bdrright bdrbottom" align="center"><asp:Literal ID="ltlTotal" runat="server" /></td>
    </tr>
  </ItemTemplate>
  <FooterTemplate>
    </table>  
  </FooterTemplate>
</asp:Repeater>
</p>
<!-- End Address Details -->
<p style="page-break-after: always;"><asp:Literal ID="ltlFooterText" runat="server" /></p>
	
