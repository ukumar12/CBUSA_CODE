<%@ Control Language="VB" AutoEventWireup="false" CodeFile="StoreRecipientsDropDown.ascx.vb" Inherits="StoreRecipientsDropDown" %>
<table border="0" cellpadding="0" cellspacing="0">
<tr>
	<td>
	<asp:DropDownList ID="drpRecipient" runat="server" style="width:200px;"></asp:DropDownList>
</td>
</tr>
<tr id="trNew" style="display:none;" runat="server">
	<td><asp:TextBox ID="txtNew" columns="30" maxlength="50" runat="server" style="width:200px;" /><br />
		<span class="smaller">Enter full name. We'll ask for delivery information at checkout.</span>
	</td>
</tr>
</table>

<CC:CustomValidatorFront ID="cvLabel" runat="server" ErrorMessage="Please enter Recipient name"></CC:CustomValidatorFront>