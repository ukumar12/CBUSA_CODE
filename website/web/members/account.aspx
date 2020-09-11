<%@ Page Language="vb" AutoEventWireup="false" Inherits="members_account" CodeFile="account.aspx.vb" %>

<CT:MasterPage runat="server" ID="CTMain" DefaultButton="btnSubmit">

<asp:PlaceHolder runat="server">    
<script type="text/javascript">
<!--     
	function CheckAvailability() {
        var txtUsername = document.getElementById('<%=txtUsername.clientId %>');
        var divAvailablity = document.getElementById('divAvailability');
		if (isEmptyField(txtUsername)) {
			divAvailablity.innerHTML = '<span style=\'color=red;font-weight:bold;\'>Username cannot be blank</span>';
			return;
		}

		var xml = getXMLHTTP();
		if(xml){
			xml.open("GET","/ajax.aspx?f=CheckAvailbility&MemberId=<%=MemberId%>&Username=" + txtUsername.value,true);
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

<h2 class="hdng">Edit Account Information</h2>

<table cellspacing="0" cellpadding="0" border="0" style="margin-top:10px; margin-left: 25px;">
<tr>
<td class="fieldreq" style="padding-bottom:6px;">&nbsp;</td>
<td class="smaller" style="padding:0 0 6px 4px;">Indicates required field</td>
</tr>
</table>

<table width="100%" cellspacing="0" cellpadding="0" border="0">
<tr valign="top">
<td style="padding-left:25px;">

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

        <tr>
        <td></td>
        <td class="fieldpad">&nbsp;</td>
        </tr>			

        <tr>
        <td></td>
        <td class="fieldpad">
        If you want to change the password for the user, please enter data below, otherwise please leave password fields blank. 
        </td>
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

        <CC:OneClickButton id="btnSubmit" runat="server" Text="Save" CssClass="btn" />&nbsp;
        <input type="button" class="btn" value="Cancel" onclick="document.location.href='default.aspx'" />
</td></tr>
</table>

<CC:EmailValidatorFront ID="evtxtBillingEmail" runat="server" Display="None" ControlToValidate="txtBillingEmail" ErrorMessage="You must provide a valid email address" />
<CC:RequiredFieldvalidatorFront ID="rqtxtUsername" runat="server" Display="None" ControlToValidate="txtUsername" ErrorMessage="You must provide a username for your membership" />
<CC:CustomValidatorFront id="cvtxtUsername" runat="server" Display="none" ControlToValidate="txtUsername" ErrorMessage="Entered username has been already taken" /> 
<CC:PasswordValidatorFront id="pvtxtPassword" runat="server" Display="none" controltovalidate="txtPassword" ErrorMessage="Password must contain minimum 4 characters and must contain both numeric and alphabetic characters" />
<CC:comparevalidatorfront id="cvtxtPassword" runat="server" Display="none" ControlToValidate="txtPassword" ControlToCompare="txtConfirmPassword" Operator="Equal" Type="String" ErrorMessage="The passwords you entered do not match" /> 

</CT:MasterPage>