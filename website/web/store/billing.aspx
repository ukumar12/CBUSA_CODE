<%@ Page Language="VB" AutoEventWireup="false" CodeFile="billing.aspx.vb" Inherits="store_billing" %>
<%@ Register TagName="StoreCheckoutSteps" TagPrefix="CC" Src="~/controls/StoreCheckoutSteps.ascx" %>
<%@ Register TagName="LimitTextBox" TagPrefix="CC" Src="~/controls/LimitTextBox.ascx" %>
<%@ Import Namespace="System.Configuration.ConfigurationManager" %>

<CT:masterpage runat="server" id="CTMain">

<div class="stepswrpr"><CC:StoreCheckoutSteps ID="Steps" CurrentStep="Billing" runat="server" /></div>

<p>Please enter your billing address in the form below. Your name and billing address must be entered exactly as they appear on your credit card statement to avoid any delay in the authorization process. If you are not paying with a credit card, this address will allow us to contact you if any issues arise with your order.</p>

<p runat="server">Your personal information is safe with us. <a href="<%=AppSettings("GlobalRefererName")%>/service/privacy.aspx">Read our Privacy Policy.</a></p>

<asp:PlaceHolder runat="server">    
<script type="text/javascript">
<!--     
    function InitializeUsername() {
        var txtBillingEmail = document.getElementById('<%=txtBillingEmail.clientId %>');
        var txtUsername = document.getElementById('<%=txtUsername.clientId %>');
        if(!txtUsername) return;
        if (isEmptyField(txtUsername)) txtUsername.value = getValue(txtBillingEmail);
    }
    
    function CopyGiftMessage(ctrl){
      	var i = ctrl.selectedIndex;
		var j = ctrl.value;
		var Field;
	    Field = document.getElementById('txtGiftMessage_ctrl');if (i==0) Field.value = ''; else Field.value = j;
	    Field.focus();
     }
     
    function CaptureGoogleAutofillChange() {    
      if (document.getElementById('<%= drpShippingCountry.ClientId %>')) {
          var sBillingCountry = document.getElementById('<%= hdnBillingCountry.ClientId %>').value;
          var sShippingCountry = document.getElementById('<%= hdnShippingCountry.ClientId %>').value;
          var sDrpBillingCountry = document.getElementById('<%= drpBillingCountry.ClientId %>')[document.getElementById('<%= drpBillingCountry.ClientId %>').selectedIndex].value;
          var sDrpShippingCountry = document.getElementById('<%= drpShippingCountry.ClientId %>')[document.getElementById('<%= drpShippingCountry.ClientId %>').selectedIndex].value;
               
          document.getElementById('<%= hdnShippingCountry.ClientId %>').value = sDrpShippingCountry;
          document.getElementById('<%= hdnBillingCountry.ClientId %>').value = sDrpBillingCountry;
          
          if (sBillingCountry != sDrpBillingCountry || sShippingCountry != sDrpShippingCountry) {
            __doPostBack('<%= chkSameAsBilling.ClientId %>','');
          }
          
          setTimeout("CaptureGoogleAutofillChange()", 500);      
      }
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
        <% if Not IsLoggedIn() Then %>
		var spanNewsletter = document.getElementById('spanNewsletter').style;
		var spanAccount = document.getElementById('spanAccount').style;
        <% end if %>
		var chkSameAsBilling = document.getElementById('<%=chkSameAsBilling.ClientId %>');
        var rbtnNewsletterYes = document.getElementById('<%=rbtnNewsletterYes.ClientId %>');
        var rbtnCreateAccountYes = document.getElementById('<%=rbtnCreateAccountYes.ClientId %>');
            
        <% if not MultipleShipToEnabled Then %>
		var spanShippingAddress = document.getElementById('spanShippingAddress').style;
		if (chkSameAsBilling.checked) {
			spanShippingAddress.display="none";
		} else {
			spanShippingAddress.display="block";
		}
        <% End if %>
        
        <% if Not IsLoggedIn() Then %>
		if (rbtnNewsletterYes.checked) {
			spanNewsletter.display="block";
		} else {
			spanNewsletter.display="none";
		}
		if (rbtnCreateAccountYes.checked) {
			spanAccount.display="block";
		} else {
			spanAccount.display="none";
		}
        <% End if %>
	}
	
	if (window.addEventListener) {
		window.addEventListener('load', RefreshPage, false);
		window.addEventListener('load', CaptureGoogleAutofillChange, false);
	} else if (window.attachEvent) {
		window.attachEvent('onload', RefreshPage);
		window.attachEvent("onload",CaptureGoogleAutofillChange);
	}
	
//-->
</script>
</asp:PlaceHolder>    

<asp:PlaceHolder runat="server">    

<asp:HiddenField runat="server" id="hdnBillingCountry" />
<asp:HiddenField runat="server" id="hdnShippingCountry" />

<table cellspacing="0" cellpadding="0" border="0" style="width:100%; margin-bottom:15px;">
<tr>
<td class="bdr vtop" style="width:<% if not MultipleShipToEnabled then %>50%;<%else%>100%;<%end if %>" id="tdBilling" runat="server">
	<h2 class="hdngbox">Billing Address</h2>
	<div class="cblock10">

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

		<tr>
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
		
		<tr runat="server" id="trSaveBilling">
		<td class="fieldlbl" style="width:130px;"></td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td runat="server">&nbsp;</td>
			<td><asp:checkbox id="chkSaveAsBilling" runat="server" /> <span class="smaller">Save this address as the default billing address.</span></td>
			</tr>
			</table>			
		</td>
		</tr>
		
		</table>


	</div>
</td>
<% if not MultipleShipToEnabled then %>
<td style="width:15px;">
	<div class="spacer" style="padding:7px;">&nbsp;</div>
</td>
<td class="bdr vtop" style="width:50%;" id="tdShipping" runat="server">
	<h2 class="hdngbox">Shipping Address</h2>
	<div class="cblock10">

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

        <span id="spanShippingAddress" runat="server">
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
		       
        <table runat="server" id="tblGiftMessage" >
    	<tr valign="top">
		<td class="fieldlbl" style="width:130px;"><span id="Span1" runat="server">Gift Message:</span></td>
        <td class="fieldpad">
            <table cellspacing="4" cellpadding="0" border="0" >
	        <tr>
		        <td>Select a predefined gift message<br /><asp:DropDownList runat="server" onchange="CopyGiftMessage(this)" columns="50" maxlength="100" width="310" id="drpGiftMessage" /></td>
	        </tr>
	        <tr>
		        <td>Enter/Customize your own gift message<br /><CC:LimitTextBox  runat="server" Limit="100" columns="50" Rows="3" maxlength="100" Width="310" id="txtGiftMessage" TextMode="MultiLine"/></td>
	        </tr>
            </table>
		</td>
        </tr>
		</table>


	</div>
</td>
<% end if %>
</tr>
</table>

<% if not MultipleShipToEnabled then %>
<table cellspacing="0" cellpadding="0" border="0" style="width:728px; margin-bottom: 20px;" summary="for layout only">
<tr>
<td class="bdr vtop" style="width:50%;" id="td1" runat="server">
	<h2 class="hdngbox">Shipping Method</h2>
	<div class="cblock10">
	<div style="margin:10px;">
		<table cellspacing="0" cellpadding="0" border="0">
		<tr>
		<td style="padding-right:10px;">
		    <asp:ScriptManager id="ScriptManager1" runat="server" />
		    <asp:UpdatePanel id="UpdatePanelShippingMethod" runat="server" UpdateMode="Conditional">
		        <ContentTemplate>
    			    <asp:dropdownlist id="drpShippingMethod" runat="server" />
			    </ContentTemplate>
			    <Triggers>
			      <asp:AsyncPostbackTrigger ControlId="chkSameAsBilling" EventName="CheckedChanged" />
			      <asp:AsyncPostbackTrigger ControlId="drpBillingCountry" EventName="SelectedIndexChanged" />
			      <asp:AsyncPostbackTrigger ControlId="drpShippingCountry" EventName="SelectedIndexChanged" />
			    </Triggers>
			</asp:UpdatePanel>
		</td>
		</tr>
		</table>

	</div>
</td>
</tr>
</table>
<% End if %>


<div class="bdr" style="margin-bottom:15px;" id="divRegister" runat="server">
	<h2 class="hdngbox">Why Register</h2>
	<div class="cblock10">


			<p>Registering for an account is fast and easy. There are many benefits to signing up for an account.</p>
			<p class="bold">My Account Advantages</p>
				
				<ul class="benefits">
				<li>Faster Check Out</li>
				<li>Address Book</li>
				<li>Order Tracking</li>
				<li>Product News and Exclusive Offers</li>
			</ul>
	
			<p class="bold">Setup Your Account (optional)</p>
			
			<p>Would you like to create an account?</p>
			
			<p class="bold"><asp:RadioButton runat="server" id="rbtnCreateAccountYes" GroupName="CreateAccount" Text="Yes" onclick="RefreshPage();" /> &nbsp;&nbsp;<asp:RadioButton runat="server" id="rbtnCreateAccountNo" GroupName="CreateAccount" Text="No" onclick="RefreshPage();" /></p>

            <span id="spanAccount" style="display:block">
			    <p>If you would like to create an account, please create a password below. We will use the email address you provided in the billing section above.</p>
    	
			    <table cellspacing="0" cellpadding="0" border="0" style="width:700px; margin:15px 0 10px 20px;">
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
	    </span> 


	</div>
</div>



<div class="bdr" style="margin-bottom:15px;" id="tblNewsletter" runat="server">
	<h2 class="hdngbox">Newsletter Registration</h2>
	<div class="cblock10">


	<div style="line-height:16px; margin:0 0 4px 10px;">
		<p>Learn about sales and updates before anyone else. Sign up for our newsletter and receive periodic updates from us.</p>
	</div>

	<div style="line-height:16px; margin:0 0 4px 10px;">
		<asp:radiobutton id="rbtnNewsletterYes" runat="server" GroupName="Newsletter" onclick="RefreshPage();" />
		<label for="<%=rbtnNewsletterYes.clientId %>">Yes, I would like to receive updates and special promotions via e-mail.</label>
	</div>

	<div style="line-height:16px; margin:0 0 4px 10px;">
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
		<td style="padding-right:10px;"><label for="<%=rbtnFormatHTML.ClientId %>">HTML</label></td>
		<td><asp:radiobutton id="rbtnFormatTEXT" runat="server" GroupName="NewsletterFormat" /><br /></td>
		<td><label for="<%=rbtnFormatTEXT.ClientId %>">Plain Text</label></td>
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


	</div>
</div>

<div class="right" style="padding:15px;">
    <CC:OneClickButton CssClass="btn" id="btnSubmit" runat="server" text="Continue &raquo;" /><br />
</div>

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
<CC:requiredfieldvalidatorfront id="rqdrpShippingMethod" runat="server" Display="None" ControlToValidate="drpShippingMethod" ErrorMessage="Shipping method is required" />

<CC:CustomValidatorFront id="cvrbtnNewsletterYes" runat="server" Display="none" ControlToValidate="rbtnNewsletterYes" ErrorMessage="You must indicate if you wish to receive the newsletter" /> 
<CC:RequiredCheckboxListValidatorFront id="cvrbtnNewsletterYesAtLeastOne" runat="server" Display="none" ControlToValidate="chklstMailingLists" ErrorMessage="You must specify at least one mailing list if you are subscribing to the newsletter" /> 

</CT:masterpage>