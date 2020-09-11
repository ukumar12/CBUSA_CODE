<%@ Page Language="vb" AutoEventWireup="false" Inherits="member_orderhistory_view" CodeFile="view.aspx.vb" %>
<%@ Register TagName="StoreShoppingCart" TagPrefix="CC" Src="~/controls/StoreShoppingCart.ascx" %>
<%@ Import Namespace="Components" %>
<%@ Import Namespace="DataLayer" %>

<CT:masterpage runat="server" id="CTMain">

<asp:Placeholder id="ph" runat="server">

<h2 class="hdng">Order History - Order Details</h2>

<asp:literal id="ltlOrderList" runat="server" />


<table width="100%" border="0" cellspacing="0" cellpadding="4" class="bdr" style="margin-top:20px;">
<tr><td class="baghdr bold" width="90">Order#</td><td><%=dbOrder.OrderNo%></td></tr>
<tr><td class="baghdr bold">Order Date:</td><td><%=FormatDateTime(dbOrder.ProcessDate,1)%></td></tr>
<tr><td class="baghdr bold">Order Status:</td><td><%=StoreOrderStatusRow.GetRowByCode(DB, dbOrder.Status).Name%></td></tr>
</table>

<CC:StoreShoppingCart ID="ctrlCart" runat="server" EditMode="False" />

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

</asp:Placeholder>

</CT:masterpage>