<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Vendor Account"%>

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

<h4><% If VendorAccountID = 0 Then %>Add<% Else %>Edit<% End If %> Vendor Account</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="optional">CRM ID:</td>
		<td class="field"><asp:textbox id="txtCRMID" runat="server" maxlength="20" columns="20" style="width: 139px;"></asp:textbox></td>
		<td>&nbsp;</td>
	</tr>
    <tr>
		<td class="optional">Historic ID:</td>
		<td class="field"><asp:textbox id="txtHistoricID" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><CC:IntegerValidator Display="Dynamic" runat="server" id="fvHistoricID" ControlToValidate="txtHistoricID" ErrorMessage="Field 'Historic I D' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Vendor:</td>
		<td class="field"><asp:DropDownList id="drpVendorID" runat="server" AutoPostBack="true" /></td>
		<td><asp:RequiredFieldValidator ID="rfvVendorID" runat="server" Display="Dynamic" ControlToValidate="drpVendorID" ErrorMessage="Field 'Vendor I D' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Vendor Branch Office:</td>
		<td class="field"><asp:DropDownList id="drpVendorBranchOfficeID" runat="server" /></td>
		<td></td>
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
		<td class="required">Email:</td>
		<td class="field"><asp:textbox id="txtEmail" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvEmail" runat="server" Display="Dynamic" ControlToValidate="txtEmail" ErrorMessage="Field 'Email' is blank"></asp:RequiredFieldValidator></td>
                <CC:EmailValidatorFront id="evftxtEmailAddress" runat="server" Display="Dynamic" ControlToValidate="txtEmail" ErrorMessage="Field 'Email Address' is invalid" />
	</tr>
	<tr>
		<td class="required">Username:</td>
		<td class="field"><asp:textbox id="txtUsername" runat="server" maxlength="50" columns="50" style="width: 139px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvUsername" runat="server" Display="Dynamic" ControlToValidate="txtUsername" ErrorMessage="Field 'Username' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Password:</td>
		<td class="field"><asp:textbox runat="server" id="txtPassword" style="width: 319px;" Columns="20" TextMode="Password" /></td>
		<td><asp:RequiredFieldValidator ID="rfvPassword" runat="server" Display="Dynamic" ControlToValidate="txtPassword" ErrorMessage="Field 'Password confirmation' is blank"></asp:RequiredFieldValidator><asp:RegularExpressionValidator id="fvPassword" Runat="server" ControlToValidate="txtPassword" Display="Dynamic" ValidationExpression="[A-Za-z0-9]{6,}" ErrorMessage="Password must contain minimum 6 alphanumeric characters"></asp:RegularExpressionValidator></td>
	</tr>
	<tr>
		<td class="required"><b>Password confirmation:</b></td>
		<td class="field"><asp:textbox runat="server" id="txtPasswordVerify" style="width: 319px;" Columns="20" TextMode="Password" /></td>
		<td><asp:RequiredFieldValidator ID="rfvPasswordVerify" runat="server" Display="Dynamic" ControlToValidate="txtPasswordVerify" ErrorMessage="Field 'Password confirmation' is blank"></asp:RequiredFieldValidator><asp:CompareValidator ID="rfvPasswordCompareVerify" Runat="server" ControlToCompare="txtPassword" ControlToValidate="txtPasswordVerify" Operator="Equal" Display="Dynamic" ErrorMessage="Password and Re-typed passwod don't match" /></td>
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
		<td class="required"><b>Require Password Update?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblRequirePasswordUpdate" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" />
			<asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
		<td><asp:RequiredFieldValidator ID="rfvRequirePasswordUpdate" runat="server" Display="Dynamic" ControlToValidate="rblRequirePasswordUpdate" ErrorMessage="Field 'Require Password Update' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Created:</td>
		<td class="field"><asp:Literal ID="ltlCreate" runat="server"></asp:Literal></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Updated:</td>
		<td class="field"><asp:Literal ID="ltlUpdate" runat="server"></asp:Literal></td>
		<td></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn" OnClientClick="return ConfirmBlankCRMID();"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Vendor Account?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

