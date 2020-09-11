<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="PIQ Account"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If PIQAccountID = 0 Then %>Add<% Else %>Edit<% End If %> PIQ Account</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">PIQ:</td>
		<td class="field"><asp:DropDownList id="drpPIQID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvPIQID" runat="server" Display="Dynamic" ControlToValidate="drpPIQID" ErrorMessage="Field 'PIQ' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">First Name:</td>
		<td class="field"><asp:textbox id="txtFirstName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvFirstName" runat="server" Display="Dynamic" ControlToValidate="txtFirstName" ErrorMessage="Field 'First Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Last Name:</td>
		<td class="field"><asp:textbox id="txtLastName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvLastName" runat="server" Display="Dynamic" ControlToValidate="txtLastName" ErrorMessage="Field 'Last Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Username:</td>
		<td class="field"><asp:textbox id="txtUsername" runat="server" maxlength="20" columns="20" style="width: 139px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvUsername" runat="server" Display="Dynamic" ControlToValidate="txtUsername" ErrorMessage="Field 'Username' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Password:</td>
		<td class="field"><asp:textbox runat="server" id="txtPassword" style="width: 139px;" Columns="20" TextMode="Password" /></td>
		<td><asp:RequiredFieldValidator ID="rfvPassword" runat="server" Display="Dynamic" ControlToValidate="txtPasswordVerify" ErrorMessage="Field 'Password confirmation' is blank"></asp:RequiredFieldValidator><asp:RegularExpressionValidator id="fvPassword" Runat="server" ControlToValidate="txtPassword" Display="Dynamic" ValidationExpression="[A-Za-z0-9]{6,}" ErrorMessage="Password must contain minimum 6 alphanumeric characters"></asp:RegularExpressionValidator></td>
	</tr>
	<tr>
		<td class="required"><b>Password confirmation:</b></td>
		<td class="field"><asp:textbox runat="server" id="txtPasswordVerify" style="width: 139px;" Columns="20" TextMode="Password" /></td>
		<td><asp:RequiredFieldValidator ID="rfvPasswordVerify" runat="server" Display="Dynamic" ControlToValidate="txtPasswordVerify" ErrorMessage="Field 'Password confirmation' is blank"></asp:RequiredFieldValidator><asp:CompareValidator ID="rfvPasswordCompareVerify" Runat="server" ControlToCompare="txtPassword" ControlToValidate="txtPasswordVerify" Operator="Equal" Display="Dynamic" ErrorMessage="Password and Re-typed passwod don't match" /></td>
	</tr>
	<tr>
		<td class="optional">Email:</td>
		<td class="field"><asp:textbox id="txtEmail" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><CC:EmailValidator Display="Dynamic" runat="server" id="fvEmail" ControlToValidate="txtEmail" ErrorMessage="Field 'Email' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Phone:</td>
		<td class="field"><asp:textbox id="txtPhone" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Mobile:</td>
		<td class="field"><asp:textbox id="txtMobile" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Fax:</td>
		<td class="field"><asp:textbox id="txtFax" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="required"><b>Is Primary?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblIsPrimary" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" />
			<asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
		<td><asp:RequiredFieldValidator ID="rfvIsPrimary" runat="server" Display="Dynamic" ControlToValidate="rblIsPrimary" ErrorMessage="Field 'Is Primary' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required"><b>Require Password Update?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblRequirePasswordUpdate" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" />
			<asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
		<td><asp:RequiredFieldValidator ID="rfvRequirePasswordUpdate" runat="server" Display="Dynamic" ControlToValidate="rblRequirePasswordUpdate" ErrorMessage="Field 'Require Password Update' is blank"></asp:RequiredFieldValidator></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this PIQ Account?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

