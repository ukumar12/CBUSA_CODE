<%@ Page Language="vb" AutoEventWireup="false" Inherits="members_default" CodeFile="default.aspx.vb" %>
<%@ Register Src="~/controls/MemberAddress.ascx" TagName="MemberAddress" TagPrefix="CC" %>

<CT:masterpage runat="server" id="CTMain">

<h2 class="hdng">My Account Details</h2>


<div runat="server" class="bdr" style="margin-bottom:15px;">
	<h2 class="hdngbox">Account Details</h2>
	<div class="cblock10">
        
        <table cellspacing="0" cellpadding="4">
        <tr><td style="width:100px;"><b>Username:</b></td><td><%= Server.HtmlEncode(dbMember.Username)%></td></tr>
        <tr><td><b>Email Address:</b></td><td><%=Server.HtmlEncode(dbBilling.Email)%></td></tr>
        <tr><td><b>Member Since:</b></td><td><%=dbMember.CreateDate.ToLongDateString%></td></tr>
        </table>
        
        <div style="padding:10px;"><CC:OneClickButton id="btnEditAccount" Text="Edit" runat="server" CssClass="btn" /></div>

	</div>
</div>

<table cellspacing="0" cellpadding="0" border="0" style="width:100%; margin-bottom:15px;">
<tr>
<td class="bdr vtop" style="width:50%;">
	<h2 class="hdngbox">Billing Address</h2>
	<div class="cblock10">
        
        <CC:MemberAddress id="ctrlBillingAddress" runat="server"></CC:MemberAddress>
        <div style="padding:10px;"><CC:OneClickButton id="btnEditBilling" runat="server" Text="Edit" CssClass="btn" /></div>
	
	</div>
</td>
<td style="width:15px;">
	<div class="spacer" style="padding:7px;">&nbsp;</div>
</td>
<td class="bdr vtop" style="width:50%;">
	<h2 class="hdngbox">Default Shipping Address</h2>
	<div class="cblock10">

       <CC:MemberAddress id="ctrlShippingAddress" runat="server"></CC:MemberAddress>
       <div style="padding:10px;"><CC:OneClickButton id="btnEditShipping" runat="server" Text="Edit" CssClass="btn" /></div>

	</div>
</td>
</tr>
</table>


</CT:masterpage>