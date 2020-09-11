<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Country"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If CountryId = 0 Then %>Add<% Else %>Edit<% End If %> Country</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan=2><span class=smallest><span class=red>red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Country Code:</td>
		<td class="field"><asp:Literal id="ltlCountryCode" runat="server" /></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">Country Name:</td>
		<td class="field"><asp:Literal id="ltlCountryName" runat="server" /></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">Shipping:</td>
		<td class="field"><asp:textbox id="txtShipping" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvShipping" runat="server" Display="Dynamic" ControlToValidate="txtShipping" ErrorMessage="Field 'Shipping' is blank"></asp:RequiredFieldValidator><CC:FloatValidator Display="Dynamic" runat="server" id="fvShipping" ControlToValidate="txtShipping" ErrorMessage="Field 'Shipping' is invalid" /></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

