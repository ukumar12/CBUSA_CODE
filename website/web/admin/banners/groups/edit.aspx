<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Banner Group"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If BannerGroupId = 0 Then %>Add<% Else %>Edit<% End If %> Banner Group</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan=2><span class=smallest><span class=red>red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Name:</td>
		<td class="field"><asp:textbox id="txtName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvName" runat="server" Display="Dynamic" ControlToValidate="txtName" ErrorMessage="Field 'Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Min Width:</td>
		<td class="field"><asp:textbox id="txtMinWidth" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvMinWidth" runat="server" Display="Dynamic" ControlToValidate="txtMinWidth" ErrorMessage="Field 'Min Width' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvMinWidth" ControlToValidate="txtMinWidth" ErrorMessage="Field 'Min Width' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Max Width:</td>
		<td class="field"><asp:textbox id="txtMaxWidth" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvMaxWidth" runat="server" Display="Dynamic" ControlToValidate="txtMaxWidth" ErrorMessage="Field 'Max Width' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvMaxWidth" ControlToValidate="txtMaxWidth" ErrorMessage="Field 'Max Width' is invalid" /></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Banner Group?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

