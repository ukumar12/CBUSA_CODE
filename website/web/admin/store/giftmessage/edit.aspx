<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Gift Message"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If GiftMessageId = 0 Then %>Add<% Else %>Edit<% End If %> Gift Message</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Gift Message Label:</td>
		<td class="field"><asp:textbox id="txtGiftMessageLabel" runat="server" maxlength="25" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvGiftMessageLabel" runat="server" Display="Dynamic" ControlToValidate="txtGiftMessageLabel" ErrorMessage="Field 'Gift Message Label' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
	    <td class="required">Gift Message:</td>
		<td class="field"><asp:textbox id="txtGiftMessage" runat="server" TextMode="MultiLine"  maxlength="100" columns="50" style="width: 319px;" rows="4"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvGiftMessage" runat="server" Display="Dynamic" ControlToValidate="txtGiftMessage" ErrorMessage="Field 'Gift Message' is blank"></asp:RequiredFieldValidator></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Gift Message?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>


