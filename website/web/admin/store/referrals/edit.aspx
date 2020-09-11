<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Referral"%>
	
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If ReferralId = 0 Then %>Add<% Else %>Edit<% End If %> Referral</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class=smallest><span class=red>red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Name:</td>
		<td class="field"><asp:textbox id="txtName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvName" runat="server" Display="Dynamic" ControlToValidate="txtName" ErrorMessage="Field 'Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Code:</td>
		<td class="field"><asp:textbox id="txtCode" runat="server" maxlength="50" columns="50" style="width: 119px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvCode" runat="server" Display="Dynamic" ControlToValidate="txtCode" ErrorMessage="Field 'Code' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Promotion:</td>
		<td class="field"><asp:DropDownList ID="drpPromotion" runat="server" /></td>
		<td>&nbsp;</td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Referral?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

