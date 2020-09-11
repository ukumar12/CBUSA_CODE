<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Admin_Survey_Page_Edit"  Title="Survey Page"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If PageId = 0 Then %>Add<% Else %>Edit<% End If %> Survey Page</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan=2><span class=smallest><span class=red>red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Name:</td>
		<td class="field"><asp:textbox id="txtName" runat="server" maxlength="50" columns="50" style="width: 125px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvName" runat="server" Display="Dynamic" ControlToValidate="txtName" ErrorMessage="Field 'Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Display Name:</td>
		<td class="field"><asp:textbox id="txtDisplayName" runat="server" maxlength="255" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Survey Page?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
