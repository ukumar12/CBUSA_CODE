<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Builder Registration Reference"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If BuilderRegistrationReferenceID = 0 Then %>Add<% Else %>Edit<% End If %> Builder Registration Reference</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Builder Registration ID:</td>
		<td class="field"><asp:textbox id="txtBuilderRegistrationID" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvBuilderRegistrationID" runat="server" Display="Dynamic" ControlToValidate="txtBuilderRegistrationID" ErrorMessage="Field 'Builder Registration ID' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvBuilderRegistrationID" ControlToValidate="txtBuilderRegistrationID" ErrorMessage="Field 'Builder Registration ID' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Contact First Name:</td>
		<td class="field"><asp:textbox id="txtContactFirstName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvContactFirstName" runat="server" Display="Dynamic" ControlToValidate="txtContactFirstName" ErrorMessage="Field 'Contact First Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Contact Last Name:</td>
		<td class="field"><asp:textbox id="txtContactLastName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvContactLastName" runat="server" Display="Dynamic" ControlToValidate="txtContactLastName" ErrorMessage="Field 'Contact Last Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Company:</td>
		<td class="field"><asp:textbox id="txtCompany" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvCompany" runat="server" Display="Dynamic" ControlToValidate="txtCompany" ErrorMessage="Field 'Company' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Phone:</td>
		<td class="field"><asp:textbox id="txtPhone" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvPhone" runat="server" Display="Dynamic" ControlToValidate="txtPhone" ErrorMessage="Field 'Phone' is blank"></asp:RequiredFieldValidator></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Builder Registration Reference?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
