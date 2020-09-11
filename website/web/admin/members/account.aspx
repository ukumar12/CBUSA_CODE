<%@ Page Language="VB" AutoEventWireup="false" CodeFile="account.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="admin_members_account" %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
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

<h4>Edit Account Information</h4>

Account Information for <asp:Label ID="txtMemberName" runat="server"></asp:Label>
| <a runat="server" id="lnkBack">&laquo; Go Back to Member Profile</a><br /><br />

<span class=smallest><span class=red>red color-</span> indicates required field</span>

<table>
		<tr valign="top">
		<td class="required" style="width:130px;">E-mail</td>
		<td class="field"><asp:textbox id="txtBillingEmail" runat="server" style="width:250px;" maxlength="100" />
		<div class="smallest" style="color:#999999; font-weight:normal; padding-left:8px;">username@hostname.com</div></td>
		<td><CC:EmailValidator ID="evtxtBillingEmail" runat="server" Display="Dynamic" ControlToValidate="txtBillingEmail" ErrorMessage="You must provide a valid email address" />
            <asp:RequiredFieldvalidator ID="rqtxtBillingEmail" runat="server" Display="Dynamic" ControlToValidate="txtBillingEmail" ErrorMessage="You must provide an email address" /></td>
		</tr>

		<tr valign="top">
		<td class="required" style="width:130px;">Username</td>
		<td class="field"><asp:textbox id="txtUsername" runat="server" style="width:250px;" maxlength="50" /><input class="btn" type="button" value="Check Availability" onclick="CheckAvailability();" /><div id="divAvailability"></div></td>
		<td><asp:RequiredFieldvalidator ID="rqtxtUsername" runat="server" Display="Dynamic" ControlToValidate="txtUsername" ErrorMessage="You must provide a username for your membership" /></td>            
        </tr>

 		<tr valign="top">
		<td class="required">Is Active?</td>
		<td class="field"><asp:CheckBox runat="server" ID="chkIsActive" /></td>
		<td></td>
		</tr>

        <tr>
        <td colspan="2">
        If you want to change the password for the user, please enter data below, otherwise please leave password fields blank. 
        </td>
        </tr>			

		<tr valign="top">
		<td class="required" style="width:130px;"><span id="labeltxtPassword" runat="server">Password</span></td>
		<td class="field"><asp:textbox id="txtPassword" TextMode="password" runat="server" style="width:150px;" maxlength="20" /><br /></td>
		<td><CC:PasswordValidatorFront id="pvtxtPassword" runat="server" Display="None" EnableClientScript="False" controltovalidate="txtPassword" ErrorMessage="Password must contain minimum 4 characters and must contain both numeric and alphabetic characters" />
        </td>
		</tr>
		
		<tr valign="top">
		<td class="required" style="width:130px;"><span id="labeltxtConfirmpassword" runat="server">Confirm Password</span></td>
		<td class="field"><asp:textbox id="txtConfirmpassword" TextMode="password" runat="server" style="width:150px;" maxlength="20" /><br /></td>
		<td><CC:comparevalidatorfront id="cvtxtPassword" runat="server" Display="None" EnableClientScript="False" ControlToValidate="txtPassword" ControlToCompare="txtConfirmPassword" Operator="Equal" Type="String" ErrorMessage="The passwords you entered do not match" /></td>
		</tr>      

</table>
<p></p>
<CC:OneClickButton id="btnSubmit" runat="server" Text="Save" CssClass="btn" />&nbsp;
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" CssClass="btn" />





</asp:content>
