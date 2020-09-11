<%@ Page Language="VB" AutoEventWireup="false" CodeFile="confirm.aspx.vb" Inherits="store_confirm" %>
<%@ Register TagName="StoreCheckoutSteps" TagPrefix="CC" Src="~/controls/StoreCheckoutSteps.ascx" %>
<%@ Register TagName="StoreShoppingCart" TagPrefix="CC" Src="~/controls/StoreShoppingCart.ascx" %>
<%@ Import Namespace="Components" %>

<CT:masterpage runat="server" id="CTMain">

<asp:Placeholder id="ph" runat="server">

<%  If Not IsPrint Then%>
<div class="stepswrpr"><CC:StoreCheckoutSteps ID="Steps" CurrentStep="Confirmation" runat="server" /></div>
<span style="font-size:14px;" class="bold">Your order has been successfully processed.</span>
<% end if %>

<p class="bold red">Order Number: <%=dbOrder.OrderNo%></p>

<%  If Not IsPrint Then%>
<p />
<span class="green">
Please print this page for your records.<br />
You will also receive a confirmation email shortly from us regarding your order.
</span>

<table border="0" cellpadding="4" style="margin-top:10px;">
<tr><td class="bdr">
<b class="largest"><a href="/store/confirm.aspx?print=y" target="_blank">Print This Page</a></b>
</td></tr>
</table>

<% end if %>

<CC:StoreShoppingCart ID="ctrlCart" runat="server" />

<table border="0" cellpadding="0" cellspacing="0" style="margin-top:10px;" width="100%">
<tr valign="top">
	<td width="50%" class="bdrtop bdrleft bdrright bdrbottom">
		<table border="0" cellpadding="3" cellspacing="0" width="100%">
		<tr valign="top">
			<td class="loginheader">Address Summary</td>
		</tr>
		<tr valign="top">
			<td class="logintext" style="padding:10px;">
				<p class="bold" style="font-size:15px;">Billing Address</p>

	            <div><asp:Literal ID="ltlFullName" runat="server" EnableViewState="False" /></div>
	            <div id="divCompany" runat="server"><%=Core.HTMLEncode(dbOrder.BillingCompany)%></div>
	            <div><%=Core.HTMLEncode(dbOrder.BillingAddress1)%></div>
	            <div id="divAddress2" runat="server"><%=Core.HTMLEncode(dbOrder.BillingAddress2)%></div>
	            <div><%=Core.HTMLEncode(dbOrder.BillingCity)%>, <%=Core.HTMLEncode(dbOrder.BillingState)%>&nbsp;<%=Core.HTMLEncode(dbOrder.BillingZip)%></div>
	            <div id="divRegion" runat="server"><%=Core.HTMLEncode(dbOrder.BillingRegion)%></div>
	            <div ><asp:Literal ID="ltlCountry" runat="server" EnableViewState="False" /></div>
	            <div>Phone: <%=Core.HTMLEncode(dbOrder.BillingPhone)%></div>
			</td>
		</tr>
		</table>
	</td>
	<td width="7"><div style="width:7px;height:7px;"></div></td>
	<td width="50%" class="bdrtop bdrleft bdrright bdrbottom">
		<table border="0" cellpadding="0" cellspacing="0" width="100%">
		<tr valign="top">
			<td class="loginheader">Payment Information</td>
		</tr>
		<tr valign="top">
			<td style="padding:10px;">

   				<table border="0" cellpadding="0" cellspacing="3">
				<tr>
					<td class="fieldtext bold">Card Type</td>
					<td><asp:Literal id="ltlCardType" runat="server" /></td>
				</tr>
				<tr>
				    <td class="fieldtext bold">Cardholder Name</td>
					<td><asp:Literal id="ltlCardHolderName" runat="server" /></td>
				</tr>
				<tr>
				    <td class="fieldtext bold">Card Number</td>
                    <td><asp:Literal id="ltlCardNumber" runat="server" /></td>
				</tr>
				<tr>
					<td class="fieldtext bold">Expiration Date</td>
					<td><asp:Literal id="ltlExpirationDate" runat="server" /></td>
				</tr>
				<tr>
					<td class="fieldtext bold">Security Code</td>
					<td><asp:Literal id="ltlCID" runat="server" /></td>
				</tr>
				<tr>
					<td class="fieldtext">&nbsp;</td>
					<td class="smaller">&nbsp;</td>
				</tr>
				<tr id="trHowHeard" runat="server">
					<td class="fieldtext bold">How did you hear about us?</td>
					<td><asp:Literal id="ltlHowHeard" runat="server" /></td>
				</tr>
				<tr>
					<td class="fieldtext">&nbsp;</td>
					<td class="smaller">&nbsp;</td>
				</tr>
				<tr>
					<td class="fieldtext bold">Comments</td>
					<td></td>
				</tr>
				<tr>
					<td colspan="2"><asp:Literal id="ltlComments" runat="server" /></td>
				</tr>	
				</table>
			</td>
		</tr>
		</table>
	</td>
</tr>
</table>

<br />
<%=dbCustomText.Value%>

</asp:Placeholder>

</CT:masterpage>