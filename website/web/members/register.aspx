<%@ Page Language="vb" AutoEventWireup="false" Inherits="members_register" CodeFile="register.aspx.vb" %>

<CT:MasterPage runat="server" ID="CTMain" DefaultButton="btnSubmit" DefaultFocus="txtBillingFirstName">

<asp:PlaceHolder runat="server">    
<script type="text/javascript">
<!--     
    function InitializeUsername() {
        var txtBillingEmail = document.getElementById('<%=txtBillingEmail.clientId %>');
        var txtUsername = document.getElementById('<%=txtUsername.clientId %>');
        if (isEmptyField(txtUsername)) txtUsername.value = getValue(txtBillingEmail);
    }
    
	function CheckAvailability() {
        var txtUsername = document.getElementById('<%=txtUsername.clientId %>');
        var divAvailablity = document.getElementById('divAvailability');
		if (isEmptyField(txtUsername)) {
			divAvailablity.innerHTML = '<span style=\'color=red;font-weight:bold;\'>Username cannot be blank</span>';
			return;
		}

		var xml = getXMLHTTP();
		if(xml){
			xml.open("GET","/ajax.aspx?f=CheckAvailbility&Username=" + txtUsername.value,true);
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
	
	function RefreshPage() {
		var spanShippingAddress = document.getElementById('spanShippingAddress').style;
		var spanNewsletter = document.getElementById('spanNewsletter').style;
		var chkSameAsBilling = document.getElementById('<%=chkSameAsBilling.ClientId %>');
        var rbtnNewsletterYes = document.getElementById('<%=rbtnNewsletterYes.ClientId %>');
            
		if (chkSameAsBilling.checked) {
			spanShippingAddress.display="none";
		} else {
			spanShippingAddress.display="block";
		}

		if (rbtnNewsletterYes.checked) {
			spanNewsletter.display="block";
		} else {
			spanNewsletter.display="none";
		}
	}
	
	if (window.addEventListener) {
		window.addEventListener('load', RefreshPage, false);
	} else if (window.attachEvent) {
		window.attachEvent('onload', RefreshPage);
	}
//-->
</script>
</asp:PlaceHolder>    

<asp:Literal id="ltlBreadcrumb" runat="server" />

<asp:PlaceHolder runat="server">    

<h2 class="hdng">New Member Registration</h2>

	<table cellspacing="0" cellpadding="0" border="0" style="width:763px; margin:10px 0 0 15px;" summary="new member registration">
	<tr>
	<td style="width:763px;" class="loginheader">Login Information</td>
	</tr>
	<tr valign="top">
	<td style="width:763px;" class="logintext loginborders">

			<table cellspacing="0" cellpadding="0" border="0" style="width:700px; margin:15px 0 10px 20px;">
			<tr valign="top">
			<td class="fieldlbl" style="width:130px;"><span id="labeltxtBillingEmail" runat="server">E-mail</span></td>
			<td class="fieldpad">
				<table cellspacing="0" cellpadding="0" border="0">
				<tr>
				<td class="fieldreq" id="bartxtBillingEmail" runat="server">&nbsp;</td>
				<td class="field"><asp:textbox id="txtBillingEmail" runat="server" style="width:250px;" maxlength="100" onBlur="InitializeUsername();" /><br /></td>
				</tr>
				</table>
				<div class="smallest" style="color:#999999; font-weight:normal; padding-left:8px;">username@hostname.com</div>
			</td>
			</tr>
			
			<tr valign="top">
			<td class="fieldlbl" style="width:130px;"><span id="labeltxtUsername" runat="server">Username</span></td>
			<td class="fieldpad">
				<table cellspacing="0" cellpadding="0" border="0">
				<tr>
				<td class="fieldreq" id="bartxtUsername" runat="server">&nbsp;</td>
				<td class="field"><asp:textbox id="txtUsername" runat="server" style="width:250px;" maxlength="50" /><br /></td>
        	    <td valign="top" class="field"><input class="btn" type="button" value="Check Availability" onclick="CheckAvailability();"></td>
				</tr>
				</table>
			</td>
			</tr>
            
            <tr>
	        <td></td>
	        <td class="fieldpad"><div id="divAvailability"></div></td>
            </tr>			
						
			<tr valign="top">
			<td class="fieldlbl" style="width:130px;"><span id="labeltxtPassword" runat="server">Password</span></td>
			<td class="fieldpad">
				<table cellspacing="0" cellpadding="0" border="0">
				<tr>
				<td class="fieldreq" id="bartxtPassword" runat="server">&nbsp;</td>
				<td class="field"><asp:textbox id="txtPassword" TextMode="password" runat="server" style="width:150px;" maxlength="20" /><br /></td>
				</tr>
				</table>
			</td>
			</tr>
			
			<tr valign="top">
			<td class="fieldlbl" style="width:130px;"><span id="labeltxtConfirmpassword" runat="server">Confirm Password</span></td>
			<td class="fieldpad">
				<table cellspacing="0" cellpadding="0" border="0">
				<tr>
				<td class="fieldreq" id="bartxtConfirmpassword" runat="server">&nbsp;</td>
				<td class="field"><asp:textbox id="txtConfirmpassword" TextMode="password" runat="server" style="width:150px;" maxlength="20" /><br /></td>
				</tr>
				</table>
			</td>
			</tr>						
	    </table>
	</td>
</tr>
</table>

<table cellspacing="0" cellpadding="0" border="0" style="width:761px; margin:10px 0 0 15px;" summary="billing and shipping">
<tr>
<td style="width:373px;"  class="loginheader">Billing Address</td>	
<td style="width:15px;"><img src="/images/spacer.gif" width="15" height="1" alt="" /><br /></td>
<td style="width:373px;" class="loginheader">Shipping Address</td>
</tr>

<tr valign="top">
<td style="width:373px;" class="loginborders">

		<table cellspacing="2" cellpadding="0" border="0" style="margin-left:6px" summary="billing">

		<tr valign="top">
		<td style="width:130px;">&nbsp;</td>
		<td class="fieldpad" style="text-align:right;">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldreq">&nbsp;</td>
			<td class="smaller">&nbsp;Indicates required field</td>
			</tr>
			</table>
		</td>
		</tr>

		<tr >
		<td>&nbsp;</td>
		<td>&nbsp;</td>
		</tr>

		<tr valign="top">
		<td class="fieldlbl" style="width:130px;"><span id="labeltxtBillingFirstName" runat="server">First Name</span></td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldreq" id="bartxtBillingFirstName" runat="server">&nbsp;</td>
			<td class="field"><asp:textbox id="txtBillingFirstName" runat="server" style="width:200px;" maxlength="50" /><br /></td>
			</tr>
			</table>
		</td>
		</tr>

		<tr valign="top">
		<td class="fieldlbl" style="width:130px;"><span id="labeltxtBillingLastName" runat="server">Last Name</span></td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldreq" id="bartxtBillingLastName" runat="server">&nbsp;</td>
			<td class="field"><asp:textbox id="txtBillingLastName" runat="server" style="width:200px;" maxlength="50" /><br /></td>
			</tr>
			</table>
		</td>
		</tr>

		<tr valign="top">
		<td class="fieldlbl" style="width:130px;">Company/School</td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldnorm">&nbsp;</td>
			<td class="field"><asp:textbox id="txtBillingCompany" runat="server" style="width:200px;" maxlength="50" /><br /></td>
			</tr>
			</table>
		</td>
		</tr>

		<tr valign="top">
		<td class="fieldlbl" style="width:130px;"><span id="labeltxtBillingAddress1" runat="server">Address 1</span></td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldreq" id="bartxtBillingAddress1" runat="server">&nbsp;</td>
			<td class="field"><asp:textbox id="txtBillingAddress1" runat="server" style="width:200px;" maxlength="50" /><br /></td>
			</tr>
			</table>
		</td>
		</tr>

		<tr valign="top">
		<td class="fieldlbl" style="width:130px;">Address 2</td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldnorm">&nbsp;</td>
			<td class="field"><asp:textbox id="txtBillingAddress2" runat="server" style="width:200px;" maxlength="50" /><br /></td>
			</tr>
			</table>
		</td>
		</tr>

		<tr valign="top">
		<td class="fieldlbl" style="width:130px;"><span id="labeltxtBillingCity" runat="server">City</span></td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldreq" id="bartxtBillingCity" runat="server">&nbsp;</td>
			<td class="field"><asp:textbox id="txtBillingCity" runat="server" style="width:200px;" maxlength="50" /><br /></td>
			</tr>
			</table>
		</td>
		</tr>

		<tr valign="top">
		<td class="fieldlbl" style="width:130px;"><span id="labeldrpBillingState" runat="server">State</span></td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldreq" id="bardrpBillingState" runat="server">&nbsp;</td>
			<td class="field">
				<asp:dropdownlist id="drpBillingState" runat="server" />
			</td>
			</tr>
			</table>
		</td>
		</tr>

		<tr valign="top">
		<td class="fieldlbl" style="width:130px;"><span id="labeltxtBillingZip" runat="server">ZIP/Postal Code</span></td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldreq" id="bartxtBillingZip" runat="server">&nbsp;</td>
			<td class="field"><asp:textbox id="txtBillingZip" runat="server" style="width:100px;" maxlength="15" /><br /></td>
			</tr>
			</table>
		</td>
		</tr>

		<tr valign="top">
		<td class="fieldlbl" style="width:130px;">Province/Region
		<div class="smallest" style="color:#999999; font-weight:normal;">(if applicable)</div>

		</td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldnorm">&nbsp;</td>
			<td class="field"><asp:textbox id="txtBillingRegion" runat="server" style="width:200px;" maxlength="50" /><br /></td>
			</tr>
			</table>
		</td>
		</tr>

		<tr valign="top">
		<td class="fieldlbl" style="width:130px;"><span id="labeldrpBillingCountry" runat="server">Country</span></td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldreq" id="bardrpBillingCountry" runat="server">&nbsp;</td>
			<td class="field">
				<asp:dropdownlist id="drpBillingCountry" runat="server" />
			</td>
			</tr>
			</table>
		</td>
		</tr>

		<tr valign="top">
		<td class="fieldlbl" style="width:130px;"><span id="labeltxtBillingPhone" runat="server">Phone:</span></td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldreq" id="bartxtBillingPhone" runat="server">&nbsp;</td>
			<td class="field"><asp:textbox id="txtBillingPhone" runat="server" style="width:200px;" maxlength="50" /><br /></td>
			</tr>
			</table>
		</td>
		</tr>
		</table>
		
</td>
<td style="width:15px;"><img src="/images/spacer.gif" width="15" height="1" alt="" /><br /></td>
<td style="width:373px;" class="logintext loginborders">
                      
        <table cellspacing="2" cellpadding="0" border="0" style="margin-left:6px" summary="shipping">            
		<tr valign="top">
		<td colspan=2>
		  <asp:checkbox id="chkSameAsBilling" runat="server" onclick="RefreshPage();" /> <label for="<%=chkSameAsBilling.clientId %>" class='smaller'>Same as Billing Address</label>
		</td>
		</tr>
		<tr >
		<td>&nbsp;</td>
		<td>&nbsp;</td>
		</tr>           
		</table>

        <span id="spanShippingAddress">
		<table cellspacing="2" cellpadding="0" border="0" style="margin-left:6px" summary="billing">            
		<tr valign="top">
		<td class="fieldlbl" style="width:130px;"><span id="labeltxtShippingFirstName" runat="server">First Name</span></td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldreq" id="bartxtShippingFirstName" runat="server">&nbsp;</td>
			<td class="field"><asp:textbox id="txtShippingFirstName" runat="server" style="width:200px;" maxlength="50" /><br /></td>
			</tr>
			</table>
		</td>
		</tr>

		<tr valign="top">
		<td class="fieldlbl" style="width:130px;"><span id="labeltxtShippingLastName" runat="server">Last Name</span></td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldreq" id="bartxtShippingLastName" runat="server">&nbsp;</td>
			<td class="field"><asp:textbox id="txtShippingLastName" runat="server" style="width:200px;" maxlength="50"/><br /></td>
			</tr>
			</table>
		</td>
		</tr>

		<tr valign="top">
		<td class="fieldlbl" style="width:130px;">Company/School</td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldnorm">&nbsp;</td>
			<td class="field"><asp:textbox id="txtShippingCompany" runat="server" style="width:200px;" maxlength="50" /><br /></td>
			</tr>
			</table>
		</td>
		</tr>

		<tr valign="top">
		<td class="fieldlbl" style="width:130px;"><span id="labeltxtShippingAddress1" runat="server">Address 1</span></td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldreq" id="bartxtShippingAddress1" runat="server">&nbsp;</td>
			<td class="field"><asp:textbox id="txtShippingAddress1" runat="server" style="width:200px;" maxlength="50" /><br /></td>
			</tr>
			</table>
		</td>
		</tr>

		<tr valign="top">
		<td class="fieldlbl" style="width:130px;">Address 2</td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldnorm">&nbsp;</td>
			<td class="field"><asp:textbox id="txtShippingAddress2" runat="server" style="width:200px;" maxlength="50" /><br /></td>
			</tr>
			</table>
		</td>
		</tr>

		<tr valign="top">
		<td class="fieldlbl" style="width:130px;"><span id="labeltxtShippingCity" runat="server">City</span></td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldreq" id="bartxtShippingCity" runat="server">&nbsp;</td>
			<td class="field"><asp:textbox id="txtShippingCity" runat="server" style="width:200px;" maxlength="50" /><br /></td>
			</tr>
			</table>
		</td>
		</tr>

		<tr valign="top">
		<td class="fieldlbl" style="width:130px;"><span id="labeldrpShippingState" runat="server">State</span></td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldreq" id="bardrpShippingState" runat="server">&nbsp;</td>
			<td class="field">
				<asp:dropdownlist id="drpShippingState" runat="server" />
			</td>
			</tr>
			</table>
		</td>
		</tr>

		<tr valign="top">
		<td class="fieldlbl" style="width:130px;"><span id="labeltxtShippingZip" runat="server">ZIP/Postal Code</span></td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldreq" id="bartxtShippingZip" runat="server">&nbsp;</td>
			<td class="field"><asp:textbox id="txtShippingZip" runat="server" style="width:100px;" maxlength="15" /><br /></td>
			</tr>
			</table>
		</td>
		</tr>

		<tr valign="top">
		<td class="fieldlbl" style="width:130px;">Province/Region
			<div class="smallest" style="color:#999999; font-weight:normal;">(if applicable)</div>
		</td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldnorm">&nbsp;</td>
			<td class="field"><asp:textbox id="txtShippingRegion" runat="server" style="width:200px;" maxlength="50" /><br /></td>
			</tr>
			</table>
		</td>
		</tr>


		<tr valign="top">
		<td class="fieldlbl" style="width:130px;"><span id="labeldrpShippingCountry" runat="server">Country</span></td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldreq" id="bardrpShippingCountry" runat="server">&nbsp;</td>
			<td class="field">
				<asp:dropdownlist id="drpShippingCountry" runat="server" />
			</td>
			</tr>
			</table>
		</td>
		</tr>

		<tr valign="top">
		<td class="fieldlbl" style="width:130px;"><span id="labeltxtShippingPhone" runat="server">Phone:</span></td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldreq" id="bartxtShippingPhone" runat="server">&nbsp;</td>
			<td class="field"><asp:textbox id="txtShippingPhone" runat="server" style="width:200px;" maxlength="50" /><br /></td>
			</tr>
			</table>
		</td>
		</tr>          
		</table>
        </span>
</td>
</tr>
</table>

<table cellspacing="0" cellpadding="0" border="0" style="width:763px; margin:10px 0 0 15px;" summary="enewsletter">
<tr>
<td style="width:763px;" class="loginheader">Newsletter Registration</td>
</tr>
<tr valign="top">
<td style="width:763px;" class="logintext loginborders">

	<div class="smaller" style="line-height:16px; margin:0 0 4px 10px;">
		Learn about sales and updates before anyone else. Sign up for our newsletter and receive periodic updates from us.
	</div>

	<div class="smaller" style="line-height:16px; margin:0 0 4px 10px;">
		<asp:radiobutton id="rbtnNewsletterYes" runat="server" GroupName="Newsletter" onclick="RefreshPage();" />
		<label for="<%=rbtnNewsletterYes.clientId %>">Yes, I would like to receive updates and special promotions via e-mail.</label>
	</div>

	<div class="smaller" style="line-height:16px; margin:0 0 4px 10px;">
		<asp:radiobutton id="rbtnNewsletterNo" runat="server" GroupName="Newsletter" onclick="RefreshPage();" />
		<label for="<%=rbtnNewsletterNo.clientId %>">No, I would not like to receive updates and special promotions via e-mail.</label>
	</div>

    <span id="spanNewsletter" style="display:block">
	<table cellspacing="0" cellpadding="0" border="0" style="width:700px; margin:15px 0 10px 20px;">
	<tr>
	<td style="width:200px;">
		<table cellspacing="0" cellpadding="0" border="0" style="margin-left:5px;">
		<tr>
		<td class="fieldreq">&nbsp;</td>
		<td class="bold smaller">&nbsp;E-mail Format</td>
		</tr>
		</table>

		<table cellspacing="0" cellpadding="0" border="0" style="margin-top:4px;" summary="email format">
		<tr>
		<td><asp:radiobutton id="rbtnFormatHTML" runat="server" GroupName="NewsletterFormat" /><br /></td>
		<td class="smaller" style="padding-right:10px;"><label for="<%=rbtnFormatHTML.ClientId %>">HTML</label></td>
		<td><asp:radiobutton id="rbtnFormatTEXT" runat="server" GroupName="NewsletterFormat" /><br /></td>
		<td class="smaller"><label for="<%=rbtnFormatTEXT.ClientId %>">Plain Text</label></td>
		</tr>
		</table>

	</td>
	<td style="width:250px;">

		<table cellspacing="0" cellpadding="0" border="0" style="margin-left:5px;">
		<tr>
		<td class="fieldreq" id="barchklstMailingLists" runat="server">&nbsp;</td>
		<td class="bold smaller"><span id="labelchklstMailingLists" runat="server">&nbsp;Lists</span></td>
		</tr>
		</table>

		<table cellspacing="0" cellpadding="0" border="0" style="margin-top:4px; width: 250px;" summary="email format">
		<tr>
		<td width="100%">
		<CC:CheckboxlistEx id="chklstMailingLists" cellspacing="4" runat="server" TextAlign="right" RepeatDirection="Horizontal" RepeatColumns="3" />
		</td>
		</tr>
		</table>
		</td>

		<td style="width:350px;">
		</td>
	</tr>
	</table>
    </span> 
</td>
</tr>
</table>

<table style="width:763px; margin:10px 0 0 15px;"><tr><td align="right"><CC:OneClickButton CssClass="btn" id="btnSubmit" runat="server" text="Register" /></td></tr></table>

</asp:PlaceHolder>

<CC:EmailValidatorFront ID="evtxtBillingEmail" runat="server" Display="None" ControlToValidate="txtBillingEmail" ErrorMessage="You must provide a valid email address" />
<CC:RequiredFieldvalidatorFront ID="rqtxtUsername" runat="server" Display="None" ControlToValidate="txtUsername" ErrorMessage="You must provide a username for your membership" />
<CC:CustomValidatorFront id="cvtxtUsername" runat="server" Display="none" ControlToValidate="txtUsername" ErrorMessage="Entered username has been already taken" /> 
<CC:requiredfieldvalidatorfront id="rqtxtPassword" runat="server" Display="none" controltoValidate="txtPassword" ErrorMessage="You must provide a password for your membership" />
<CC:PasswordValidatorFront id="pvtxtPassword" runat="server" Display="none" controltovalidate="txtPassword" ErrorMessage="Password must contain minimum 4 characters and must contain both numeric and alphabetic characters" />
<CC:comparevalidatorfront id="cvtxtPassword" runat="server" Display="none" ControlToValidate="txtPassword" ControlToCompare="txtConfirmPassword" Operator="Equal" Type="String" ErrorMessage="The passwords you entered do not match" /> 

<CC:requiredfieldvalidatorfront id="rqtxtBillingFirstName" runat="server" Display="None" ControlToValidate="txtBillingFirstName" ErrorMessage="Billing first name is required" />
<CC:requiredfieldvalidatorfront id="rqtxtBillingLastName" runat="server" Display="None" ControlToValidate="txtBillingLastName" ErrorMessage="Billing last name is required" />
<CC:requiredfieldvalidatorfront id="rqtxtBillingAddress1" runat="server" Display="None" ControlToValidate="txtBillingAddress1" ErrorMessage="Billing address is required" />
<CC:requiredfieldvalidatorfront id="rqtxtBillingCity" runat="server" Display="None" ControlToValidate="txtBillingCity" ErrorMessage="Billing city is required" />
<CC:requiredfieldvalidatorfront id="rqtxtBillingZip" runat="server" Display="None" ControlToValidate="txtBillingZip" ErrorMessage="Billing zip is required" />
<CC:requiredfieldvalidatorfront id="rqdrpBillingState" runat="server" Display="None" ControlToValidate="drpBillingState" ErrorMessage="Billing state is required" />
<CC:requiredfieldvalidatorfront id="rqdrpBillingCountry" runat="server" Display="None" ControlToValidate="drpBillingCountry" ErrorMessage="Billing country is required" />
<CC:requiredfieldvalidatorfront id="rqtxtBillingPhone" runat="server" Display="None" ControlToValidate="txtBillingPhone" ErrorMessage="Billing phone number is required" />
<CC:requiredfieldvalidatorfront id="rqtxtBillingEmail" runat="server" Display="None" ControlToValidate="txtBillingEmail" ErrorMessage="Billing email is required" />

<CC:requiredfieldvalidatorfront id="rqtxtShippingFirstName" runat="server" Display="None" ControlToValidate="txtShippingFirstName" ErrorMessage="Shipping first name is required" />
<CC:requiredfieldvalidatorfront id="rqtxtShippingLastName" runat="server" Display="None" ControlToValidate="txtShippingLastName" ErrorMessage="Shipping last name is required" />
<CC:requiredfieldvalidatorfront id="rqtxtShippingAddress1" runat="server" Display="None" ControlToValidate="txtShippingAddress1" ErrorMessage="Shipping address is required" />
<CC:requiredfieldvalidatorfront id="rqtxtShippingCity" runat="server" Display="None" ControlToValidate="txtShippingCity" ErrorMessage="Shipping city is required" />
<CC:requiredfieldvalidatorfront id="rqtxtShippingZip" runat="server" Display="None" ControlToValidate="txtShippingZip" ErrorMessage="Shipping zip is required" />
<CC:requiredfieldvalidatorfront id="rqdrpShippingState" runat="server" Display="None" ControlToValidate="drpShippingState" ErrorMessage="Shipping state is required" />
<CC:requiredfieldvalidatorfront id="rqtxtShippingPhone" runat="server" Display="None" ControlToValidate="txtShippingPhone" ErrorMessage="Shipping phone number is required" />    
<CC:requiredfieldvalidatorfront id="rqdrpShippingCountry" runat="server" Display="None" ControlToValidate="drpShippingCountry" ErrorMessage="Shipping country is required" />

<CC:CustomValidatorFront id="cvrbtnNewsletterYes" runat="server" Display="none" ControlToValidate="rbtnNewsletterYes" ErrorMessage="You must indicate if you wish to receive the newsletter" /> 
<CC:RequiredCheckboxListValidatorFront id="cvrbtnNewsletterYesAtLeastOne" runat="server" Display="none" ControlToValidate="chklstMailingLists" ErrorMessage="You must specify at least one mailing list if you are subscribing to the newsletter" /> 

</CT:MasterPage>