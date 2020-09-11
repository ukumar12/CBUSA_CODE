<%@ Page Language="VB" ValidateRequest="False" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Store Custom Text"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If TextId = 0 Then %>Add<% Else %>Edit<% End If %> Help Message Text</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Code:</td>
		<td class="field"><asp:textbox id="txtCode" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox><asp:label runat="server" id="lblCode"></asp:label></td>
		<td><asp:RequiredFieldValidator ID="rfvCode" runat="server" Display="Dynamic" ControlToValidate="txtCode" ErrorMessage="Field 'Code' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Title:</td>
		<td class="field"><asp:textbox id="txtTitle" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvTitle" runat="server" Display="Dynamic" ControlToValidate="txtTitle" ErrorMessage="Field 'Title' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Value:</td>
		<td class="field"><CC:CKEditor id="txtValue" runat="server" Width="600" Height="300" /></td>
		<td></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Store Custom Text?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

