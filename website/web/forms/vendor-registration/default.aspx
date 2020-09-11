<%@ Page Language="VB" AutoEventWireup="false" CodeFile="default.aspx.vb" Inherits="forms_vendor_registration_default" %>
<%@ Register TagName="VendorRegistrationSteps" TagPrefix="CC" Src="~/controls/VendorRegistrationSteps.ascx" %>

<CT:MasterPage ID="CTMain" runat="server">
<script src="/includes/XmlHttpLookup.js" type="text/javascript"></script>

<asp:PlaceHolder runat="server">
<script type="text/javascript">
<!--
	function CheckAvailability() {
		var txtUsername = document.getElementById('txtUsername');
		var divAvailablity = document.getElementById('divAvailability');
		if (isEmptyField(txtUsername)) {
			divAvailablity.innerHTML = '<span style=\'color=red;font-weight:bold;\'>Username cannot be blank</span>';
			return;
		}

		var xml = getXMLHTTP();
		if(xml){
			xml.open("GET","/ajax.aspx?f=CheckAvailbility&VendorAccountId=<%=VendorAccountID %>&Username=" + txtUsername.value,true);
			xml.onreadystatechange = function() {
				if(xml.readyState==4 && xml.responseText) {
					if (xml.responseText.length > 0) {
						var sUsername = txtUsername.value
						sUsername = sUsername.replace(/</g,"&lt;");
						sUsername = sUsername.replace(/>/g,"&gt;");
						if (xml.responseText == 'OK') {
							divAvailablity.innerHTML = '<span style=\'color=green;font-weight:bold;\'>Username ' + sUsername + ' is available</span>';
						} else {
							divAvailablity.innerHTML = '<span style=\'color=#8F0407;font-weight:bold;\'>Username ' + sUsername + ' is already taken!</span>';
						}
					} else {
						divAvailablity.innerHTML = '<span style=\'color=red\'>Availability check is disabled for this version of the browser</span>';
					}
				}
			}
			xml.send(null);
		} else {
			divAvailablity.innerHTML = '<span style=\'color=red\'>Availability check is disabled for this version of the browser</span>';
		}
}
//-->
</script>
</asp:PlaceHolder>
<%--<asp:button id ="btnGoToDashBoard" text="Go to DashBoard" class="btnred" runat="server" style="margin-left: 119px;"/>--%>
<CC:VendorRegistrationSteps ID="ctlSteps" runat="server" RegistrationStep="1" />
<div class="pckggraywrpr" style="margin:20px 100px;">
<div class="pckghdgred">Account Information</div>
<table class="regform">
	<tr>
		<td>&nbsp;</td>
		<td class="fieldreq">&nbsp;</td>
		<td><span class="smaller"> indicates required field</span></td>
	</tr>
	<tr id="trUsername" runat="server" >
		<td class="fieldlbl" ><span id="labeltxtUsername" runat="server">Username:</span></td>
		<td class="fieldreq" id="bartxtUsername" runat="server">&nbsp;</td>
		<td class="field"><asp:textbox id="txtUsername" runat="server" CssClass="regtxt" maxlength="50" /><input class="btnred" type="button" value="Check Availability" onclick="CheckAvailability();"/><div id="divAvailability"></div></td>
	</tr>
	<tr>
		<td></td>
		<td class="fieldpad" colspan=2>
		<span id="psMsg" runat="server">If you want to change the password for the user, please enter data below, otherwise please leave password fields blank.</span> 
		</td>
	</tr>
	<tr id="trPassword" runat="server" >
		<td class="fieldlbl" ><span id="labeltxtPassword" runat="server">Password:</span></td>
		<td class="fieldreq" id="bartxtPassword" runat="server">&nbsp;</td>
		<td class="field"><asp:textbox id="txtPassword" TextMode="password" runat="server" CssClass="regtxt" maxlength="20" /><br /></td>
	</tr>
	<tr id="trConfirmpassword" runat="server" >
		<td class="fieldlbl" ><span id="labeltxtConfirmpassword" runat="server">Confirm Password:</span></td>
		<td class="fieldreq" id="bartxtConfirmpassword" runat="server">&nbsp;</td>
		<td class="field"><asp:textbox id="txtConfirmpassword" TextMode="password" runat="server" CssClass="regtxt" maxlength="20" />
		</td>
	</tr>
	<tr id="trFirstName" runat="server" >
		<td class="fieldlbl"><span id="labeltxtFirstName" runat="server">First Name:</span></td>
		<td class="fieldreq" id="bartxtFirstName" runat="server">&nbsp;</td>
		<td class="field"><asp:TextBox ID="txtFirstName" runat="server" MaxLength="50" cssClass="regtxt"></asp:TextBox></td>
	</tr>
	<tr id="trLastName" runat="server" >
		<td class="fieldlbl"><span id="labeltxtLastName" runat="server">Last Name:</span></td>
		<td class="fieldreq" id="bartxtLastName" runat="server">&nbsp;</td>
		<td class="field"><asp:TextBox ID="txtLastName" runat="Server" MaxLength="50" CssClass="regtxt"></asp:TextBox></td>
	</tr>
	<tr>
		<td class="fieldlbl"><span id="labeltxtTitle" runat="server">Title:</span></td>
		<td>&nbsp;</td>
		<td class="field"><asp:TextBox id="txtTitle" runat="server" maxlength="50" columns="50" cssclass="regtxt"></asp:TextBox></td>
	</tr>
	<tr>
		<td class="fieldlbl"><span id="labeltxtAccountPhone" runat="server">Phone:</span></td>
		<td class="fieldreq" id="bartxtAccountPhone" runat="server">&nbsp;</td>
		<td class="field"><asp:TextBox ID="txtAccountPhone" runat="server" MaxLength="50" CssClass="regtxt"></asp:TextBox></td>
	</tr>
	<tr>
		<td class="fieldlbl"><span id="labeltxtAccountMobile" runat="server">Mobile:</span></td>
		<td>&nbsp;</td>
		<td class="field"><asp:TextBox ID="txtAccountMobile" runat="server" MaxLength="50" CssClass="regtxt"></asp:TextBox></td>
	</tr>
	<tr>
		<td class="fieldlbl"><span id="labeltxtAccountFax" runat="server">Fax:</span></td>
		<td>&nbsp;</td>
		<td class="field"><asp:TextBox ID="txtAccountFax" runat="server" MaxLength="50" CssClass="regtxt"></asp:TextBox></td>
	</tr>
	<tr>
		<td class="fieldlbl"><span id="labeltxtEmail" runat="server">Email:</span></td>
		<td class="fieldreq" id="bartxtEmail" runat="server">&nbsp;</td>
		<td class="field"><asp:TextBox ID="txtEmail" runat="server" MaxLength="100" CssClass="regtxt" style="width: 319px;"></asp:TextBox></td>
	</tr>
	<tr>
		<td class="fieldlbl"><span id="labeltxtCompanyName" runat="server">Company Name:</span></td>
		<td class="fieldreq" id="bartxtCompanyName" runat="server">&nbsp;</td>
		<td class="field"><asp:TextBox ID="txtCompanyName" runat="server" MaxLength="50" CssClass="regtxt" ></asp:TextBox></td>
	</tr>
	<tr>
		<td class="fieldlbl"><span id="labeltxtAddress" runat="server">Address:</span></td>
		<td class="fieldreq" id="bartxtAddress" runat="server">&nbsp;</td>
		<td class="field"><asp:TextBox ID="txtAddress" runat="server" MaxLength="50" CssClass="regtxt" ></asp:TextBox></td>
	</tr>
	<tr>
		<td class="fieldlbl">Address line 2:</td>
		<td>&nbsp;</td>
		<td class="field"><asp:TextBox ID="txtAddress2" runat="server" MaxLength="50" CssClass="regtxt" ></asp:TextBox></td>
	</tr>
	<tr>
		<td class="fieldlbl"><span id="labeltxtCity" runat="server">City:</span></td>
		<td class="fieldreq" id="bartxtCity" runat="server">&nbsp;</td>
		<td class="field"><asp:TextBox ID="txtCity" runat="server" MaxLength="50" CssClass="regtxt" ></asp:TextBox></td>
	</tr>
	<tr>
		<td class="fieldlbl"><span id="labeldrpState" runat="server">State:</span></td>
		<td class="fieldreq" id="bardrpState" runat="server">&nbsp;</td>
		<td class="field"><asp:DropDownList  ID="drpState" runat="server" /></td>
	</tr>
	<tr>
		<td class="fieldlbl"><span id="labeltxtZip" runat="server">Zip Code:</span></td>
		<td class="fieldreq" id="bartxtZip" runat="server">&nbsp;</td>
		<td class="field"><asp:TextBox ID="txtZip" runat="server" MaxLength="15" CssClass="regtxtshort"></asp:TextBox></td>
	</tr>
	<tr>
		<td class="fieldlbl"><span id="labeltxtBillingAddress" runat="server">Billing Address:</span></td>
		<td class="fieldreq" id="bartxtBillingAddress" runat="server">&nbsp;</td>
		<td class="field"><asp:TextBox id="txtBillingAddress" runat="server" maxlength="50" ></asp:TextBox></td>
	</tr>
	<tr>
		<td class="fieldlbl"><span id="labeltxtBillingCity" runat="server">Billing City:</span></td>
		<td class="fieldreq" id="bartxtBillingCity" runat="server">&nbsp;</td>
		<td class="field"><asp:TextBox id="txtBillingCity" runat="server"></asp:TextBox></td>
	</tr>
	<tr>
		<td class="fieldlbl"><span id="labeldrpBillingState" runat="server">Billing State:</span></td>
		<td class="fieldreq" id="bardrpBillingState" runat="server">&nbsp;</td>
		<td class="field"><asp:DropDownList id="drpBillingState" runat="server"></asp:DropDownList></td>
	</tr>
	<tr>
		<td class="fieldlbl"><span id="labeltxtBillingZip" runat="server">Billing Zip Code:</span></td>
		<td class="fieldreq" id="bartxtBillingZip" runat="server">&nbsp;</td>
		<td class="field"><asp:TextBox id="txtBillingZip" runat="server" maxlength="15"></asp:TextBox></td>
	</tr>
	<tr>
		<td class="fieldlbl"><span id="labeltxtPhone" runat="server">Company Phone:</span></td>
		<td class="fieldreq" id="bartxtPhone" runat="server">&nbsp;</td>
		<td class="field"><asp:TextBox ID="txtPhone" runat="server" MaxLength="50" CssClass="regtxt"></asp:TextBox></td>
	</tr>
	<tr>
		<td class="fieldlbl"><span id="labeltxtMobile" runat="server">Company Mobile:</span></td>
		<td>&nbsp;</td>
		<td class="field"><asp:TextBox ID="txtMobile" runat="server" MaxLength="50" CssClass="regtxt"></asp:TextBox></td>
	</tr>
	<tr>
		<td class="fieldlbl"><span id="labeltxtFax" runat="server">Company Fax:</span></td>
		<td>&nbsp;</td>
		<td class="field"><asp:TextBox ID="txtFax" runat="server" MaxLength="50" CssClass="regtxt"></asp:TextBox></td>
	</tr>
	<tr>
		<td class="fieldlbl" colspan="3">
			<p style="text-align:center;">
				<CC:OneClickButton ID="btnContinueElectronic" runat="server" Text="Continue with Registration" CssClass="btnred"  /><br />
				<CC:OneClickButton ID="btnDashboard" runat="server" Text="Save Changes" CssClass="btnred" />
				<CC:OneClickButton ID="btnCancel" runat="server" Text="Cancel" CssClass="btnred" OnClientClick="history.go(-1);return false;" />
			</p>
		</td>
	</tr>
</table>
</div>

<CC:RequiredFieldValidatorFront ID="rfvtxtFirstName" runat="server" ControlToValidate="txtFirstName" ErrorMessage="Field 'First Name' is blank" ValidationGroup="VendorReg"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtLastName" runat="server" ControlToValidate="txtLastName" ErrorMessage="Field 'Last Name' is blank" ValidationGroup="VendorReg"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtCompanyName" runat="server" ControlToValidate="txtCompanyName" ErrorMessage="Field 'Company Name' is blank" ValidationGroup="VendorReg"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtAddress" runat="server" ControlToValidate="txtAddress" ErrorMessage="Field 'Address' is blank" ValidationGroup="VendorReg"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtCity" runat="server" ControlToValidate="txtCity" ErrorMessage="Field 'City' is blank" ValidationGroup="VendorReg"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvdrpState" runat="server" ControlToValidate="drpState" ErrorMessage="Field 'State' is blank" ValidationGroup="VendorReg"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtZip" runat="server" ControlToValidate="txtZip" ErrorMessage="Field 'Zip Code' is blank" ValidationGroup="VendorReg"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtPhone" runat="server" ControlToValidate="txtPhone" ErrorMessage="Field 'Company Phone' is blank" ValidationGroup="VendorReg"></CC:RequiredFieldValidatorFront>
<CC:PhoneValidatorFront id="pvftxtPhone" runat="server" ControlToValidate="txtPhone" ErrorMessage="Field 'Company Phone' is invalid" ValidationGroup="VendorReg"></CC:PhoneValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtAccountPhone" runat="server" ControlToValidate="txtAccountPhone" ErrorMessage="Field 'Phone' is blank" ValidationGroup="VendorReg"></CC:RequiredFieldValidatorFront>
<CC:PhoneValidatorFront id="pvftxtAccountPhone" runat="server" ControlToValidate="txtAccountPhone" ErrorMessage="Field 'Phone' is invalid" ValidationGroup="VendorReg"></CC:PhoneValidatorFront>
<CC:PhoneValidatorFront ID="pvftxtMobile" runat="server" ControlToValidate="txtModile" ErrorMessage="Field 'Mobile' is invalid" ValidationGroup="VendorReg"></CC:PhoneValidatorFront>
<CC:PhoneValidatorFront id="pvftxtPager" runat="server" ControlToValidate="txtPager" ErrorMessage="Field 'Pager' is invalid" ValidationGroup="VendorReg"></CC:PhoneValidatorFront>
<CC:PhoneValidatorFront ID="pvftxtOtherPhone" runat="server" ControlToValidate="txtOtherPhone" ErrorMessage="Field 'Other Phone' is invalid" ValidationGroup="VendorReg"></CC:PhoneValidatorFront>
<CC:PhoneValidatorFront ID="pvftxtFax" runat="server" ControlToValidate="txtFax" ErrorMessage="Field 'Fax' is invalid" ValidationGroup="VendorReg"></CC:PhoneValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Field 'Email' is blank" ValidationGroup="VendorReg"></CC:RequiredFieldValidatorFront>
<CC:EmailValidatorFront id="evftxtEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Field 'Email' is invalid" ValidationGroup="VendorReg"></CC:EmailValidatorFront>
<CC:RequiredFieldvalidatorFront ID="rqtxtUsername" runat="server" Display="none" ControlToValidate="txtUsername" ErrorMessage="You must provide a username for your membership" ValidationGroup="VendorReg" />
<CC:CustomValidatorFront id="cvftxtUsername" runat="server" Display="none" ControlToValidate="txtUsername" ErrorMessage="Entered username has been already taken" ValidationGroup="VendorReg" /> 
<CC:RequiredFieldValidatorFront ID="rfvtxtPassword" runat="server" Display='None' ControlToValidate="txtPassword" ErrorMessage="Please update your password to continue" ValidationGroup="VendorReg" ></CC:RequiredFieldValidatorFront>
<CC:PasswordValidatorFront id="pvtxtPassword" runat="server" Display="none" controltovalidate="txtPassword" ErrorMessage="Password must contain minimum 4 characters and must contain both numeric and alphabetic characters" ValidationGroup="VendorReg" />		    
<CC:comparevalidatorfront id="cvtxtPassword" runat="server" Display="none" ControlToValidate="txtPassword" ControlToCompare="txtConfirmPassword" Operator="Equal" Type="String" ErrorMessage="The passwords you entered do not match" ValidationGroup="VendorReg" /> 

<CC:RequiredFieldValidatorFront ID="rfvtxtBillingAddress" runat="server" ControlToValidate="txtBillingAddress" ErrorMessage="Field 'Billing Address' is blank" ValidationGroup="VendorReg"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtBillingCity" runat="server" ControlToValidate="txtBillingCity" ErrorMessage="Field 'Billing City' is blank" ValidationGroup="VendorReg"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvdrpBillingState" runat="server" ControlToValidate="drpBillingState" ErrorMessage="Field 'Billing State' is blank" ValidationGroup="VendorReg"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtBillingZip" runat="server" ControlToValidate="txtBillingZip" ErrorMessage="Field 'Billing Zip Code' is blank" ValidationGroup="VendorReg"></CC:RequiredFieldValidatorFront>

</CT:MasterPage>