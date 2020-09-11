<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Builder Account"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<script type="text/javascript">
    function ConfirmBlankCRMID()
    {
        var CRMID = document.getElementById("ph_txtCRMID").value;

        if (CRMID == "") {
            var response = confirm("Are you sure you want to clear the CRM ID value for this account? Doing so will unlink this account from the associated CRM record, preventing future updates from Insightly.");
            return response;
        } else {
            return true;
        }
    }
</script>

<h4><% If BuilderAccountID = 0 Then %>Add<% Else %>Edit<% End If %> Builder Account</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="optional">CRM ID:</td>
		<td class="field"><asp:textbox  id="txtCRMID" runat="server" maxlength="20" columns="20" style="width: 139px;"></asp:textbox></td>
		<td>&nbsp;</td>
	</tr>
    <tr>
		<td class="optional">Historic ID:</td>
		<td class="field"><asp:textbox id="txtHistoricID" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><CC:IntegerValidator Display="Dynamic" runat="server" id="fvHistoricID" ControlToValidate="txtHistoricID" ErrorMessage="Field 'Historic ID' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Builder:</td>
		<td class="field"><asp:DropDownList id="drpBuilderID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvBuilderID" runat="server" Display="Dynamic" ControlToValidate="drpBuilderID" ErrorMessage="Field 'Builder ID' is blank"></asp:RequiredFieldValidator></td>
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
		<td class="optional">Title:</td>
		<td class="field"><asp:textbox id="txtTitle" runat="server" maxlength="150" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
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
		<td class="required">Username:</td>
		<td class="field"><asp:textbox id="txtUsername" runat="server" maxlength="50" columns="20" style="width: 139px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvUsername" runat="server" Display="Dynamic" ControlToValidate="txtUsername" ErrorMessage="Field 'Username' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Password:</td>
		<td class="field"><asp:textbox runat="server" id="txtPassword" style="width: 139px;" Columns="20" MaxLength="20" TextMode="Password" /></td>
		<td><asp:RequiredFieldValidator ID="rfvPassword" runat="server" Enabled="false" Display="Dynamic" ControlToValidate="txtPassword" ErrorMessage="Field 'Password' is blank"></asp:RequiredFieldValidator><asp:RegularExpressionValidator id="fvPassword" Runat="server" ControlToValidate="txtPassword" Display="Dynamic" ValidationExpression="[A-Za-z0-9]{6,}" ErrorMessage="Password must contain minimum 6 alphanumeric characters"></asp:RegularExpressionValidator></td>
	</tr>
	<tr>
		<td class="required"><b>Password confirmation:</b></td>
		<td class="field"><asp:textbox runat="server" id="txtPasswordVerify" style="width: 139px;" Columns="20" maxlength="20" TextMode="Password" /></td>
		<td><asp:RequiredFieldValidator ID="rfvPasswordVerify" runat="server" Enabled="false" Display="Dynamic" ControlToValidate="txtPasswordVerify" ErrorMessage="Field 'Password confirmation' is blank"></asp:RequiredFieldValidator><asp:CompareValidator ID="rfvPasswordCompareVerify" Runat="server" ControlToCompare="txtPassword" ControlToValidate="txtPasswordVerify" Operator="Equal" Display="Dynamic" ErrorMessage="Password and Re-typed passwod don't match" /></td>
	</tr>
	<tr>
		<td class="optional">Historic Password Hash:</td>
		<td class="field"><asp:textbox id="txtHistoricPasswordHash" runat="server" maxlength="40" columns="40" style="width: 259px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Historic Password Salt:</td>
		<td class="field"><asp:textbox id="txtHistoricPasswordSalt" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Historic Password Sha 1:</td>
		<td class="field"><asp:textbox id="txtHistoricPasswordSha1" runat="server" maxlength="40" columns="40" style="width: 259px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Email:</td>
		<td class="field"><asp:textbox id="txtEmail" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><CC:EmailValidator Display="Dynamic" runat="server" id="fvEmail" ControlToValidate="txtEmail" ErrorMessage="Field 'Email' is invalid" /></td>
	</tr>
	<tr>
	<td class="optional">Role:</td>
	<td class="field"><CC:CheckBoxListEx ID="F_Role" runat="server" RepeatColumns="3"></CC:CheckBoxListEx></td>
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
		<td class="required"><b>Is Active?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblIsActive" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" />
			<asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
		<td><asp:RequiredFieldValidator ID="rfvIsActive" runat="server" Display="Dynamic" ControlToValidate="rblIsActive" ErrorMessage="Field 'Is Active' is blank"></asp:RequiredFieldValidator></td>
	</tr>
    <tr>
		<td class="required"><b>NCP Reminder?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblSendNCPReminder" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" />
			<asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
		<td><asp:RequiredFieldValidator ID="rfvSendNCPReminder" runat="server" Display="Dynamic" ControlToValidate="rblSendNCPReminder" ErrorMessage="Field 'NCP Reminder' is blank"></asp:RequiredFieldValidator></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn" OnClientClick="return ConfirmBlankCRMID();"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Builder Account?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
