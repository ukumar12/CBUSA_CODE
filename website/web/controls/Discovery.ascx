<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Discovery.ascx.vb" Inherits="DiscoveryDropDown" %>
<table border="0" cellpadding="0" cellspacing="0">
<tr>
	<td><asp:DropDownList ID="drpHowHeardList" runat="server"></asp:DropDownList></td>
</tr>
<tr id="trOther" style="display:none;" runat="server">
	<td style="padding-top:3px;"><div id="divUserInputLabel" runat="server" /><asp:TextBox ID="txtDiscoveryOther" columns="30" maxlength="50" runat="server" CssClass="text" /></td>
</tr>
</table>

<CC:CustomValidatorFront ID="cvfDiscovery" runat="server" ErrorMessage="Please input another source."></CC:CustomValidatorFront>