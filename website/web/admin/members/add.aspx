<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" CodeFile="add.aspx.vb" Inherits="admin_members_add" %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
<asp:PlaceHolder ID="PlaceHolder1" runat="server">    
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
		var spanNewsletter = document.getElementById('spanNewsLetter').style;
		var chkSameAsBilling = document.getElementById('<%=chkSameAsBilling.ClientId %>');
        var rbtnNewsletterYes = document.getElementById('<%=rbtnNewsletterYes.ClientId %>');
        var rqtxtShippingCity = document.getElementById('<%=rqtxtShippingCity.ClientId %>');
        var rqtxtShippingZip = document.getElementById('<%=rqtxtShippingZip.ClientId %>');
        var rqdrpShippingState = document.getElementById('<%=rqdrpShippingState.ClientId %>');
        var rqtxtShippingAddress1 = document.getElementById('<%=rqtxtShippingAddress1.ClientId %>');
        var rqdrpShippingCountry = document.getElementById('<%=rqdrpShippingCountry.ClientId %>');
        var rqtxtShippingFirstName = document.getElementById('<%=rqtxtShippingFirstName.ClientId %>');
        var rqtxtShippingLastName = document.getElementById('<%=rqtxtShippingLastName.ClientId %>');
        var rqtxtShippingPhone = document.getElementById('<%=rqtxtShippingPhone.ClientId %>');
        
            
		if (chkSameAsBilling.checked) {
			spanShippingAddress.display="none";
			rqtxtShippingAddress1.enabled = false;
			rqtxtShippingCity.enabled = false; 
			rqtxtShippingZip.enabled = false;
			rqdrpShippingCountry.enabled = false;
			rqdrpShippingState.enabled = false;
			rqtxtShippingFirstName.enabled = false;
			rqtxtShippingLastName.enabled = false;
			rqtxtShippingPhone.enabled = false;
		} else {
			spanShippingAddress.display="block";
			rqtxtShippingAddress1.enabled = true;
			rqtxtShippingCity.enabled = true; 
			rqtxtShippingZip.enabled = true;
			rqdrpShippingCountry.enabled = true;
			rqdrpShippingState.enabled = true;
			rqtxtShippingFirstName.enabled = true;
			rqtxtShippingLastName.enabled = true;
			rqtxtShippingPhone.enabled = true;
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

<asp:PlaceHolder ID="PlaceHolder2" runat="server">    

<h4> Add New Member </h4>
	<table>
	<tr>
    	<td colspan="2"><font color="red">Red color </font>- indicates required field</td>
	</tr>
	
	<tr>
        <td>&nbsp;</td>
		<td>&nbsp;</td>
	</tr>    
	
	
	<tr>
	<td>Login Information</td>
	<td></td>
	</tr>
	
	<tr>
        <td>&nbsp;</td>
		<td>&nbsp;</td>
	</tr>   
	
    <tr>
		<td class="required" style="width:130px;"><span id="labeltxtBillingEmail" runat="server">E-mail</span></td>
		<td class="field"><asp:textbox id="txtBillingEmail" runat="server" style="width:250px;" maxlength="100" onBlur="InitializeUsername();" /><div class="smallest" style="color:#999999; font-weight:normal; padding-left:8px;">username@hostname.com</div></td>
		<td>
		    <CC:EmailValidatorFront ID="evtxtBillingEmail" runat="server" Display="Dynamic" ControlToValidate="txtBillingEmail" ErrorMessage="You must provide a valid email address" />
		    <asp:requiredfieldvalidator id="rqtxtBillingEmail" runat="server" Display="Dynamic" ControlToValidate="txtBillingEmail" ErrorMessage="Email is required for membership" />    
		</td>
	</tr>
			
	<tr>
		<td class="required" style="width:130px;"><span id="labeltxtUsername" runat="server">Username</span></td>
		<td class="field"><asp:textbox id="txtUsername" runat="server" style="width:250px;" maxlength="50" /><input class="btn" type="button" value="Check Availability" onclick="CheckAvailability();"/><div id="divAvailability"></div></td>
		<td>
		    <asp:RequiredFieldvalidator ID="rqtxtUsername" runat="server" Display="Dynamic" ControlToValidate="txtUsername" ErrorMessage="You must provide a username for your membership" />
		    <CC:CustomValidatorFront id="cvtxtUsername" runat="server" Display="Dynamic" ControlToValidate="txtUsername" ErrorMessage="Entered username has been already taken" /> 
		</td>
	</tr>
			
     					
	<tr>
	    <td class="required" style="width:130px;"><span id="labeltxtPassword" runat="server">Password</span></td>
		<td class="field"><asp:textbox id="txtPassword" TextMode="password" runat="server" style="width:150px;" maxlength="20" /><br /></td>
		<td><asp:requiredfieldvalidator id="rqtxtPassword" runat="server" Display="Dynamic" controltoValidate="txtPassword" ErrorMessage="You must provide a password for your membership" />
		    <CC:PasswordValidatorFront id="pvtxtPassword" runat="server" Display="Dynamic" controltovalidate="txtPassword" ErrorMessage="Password must contain minimum 4 characters and must contain both numeric and alphabetic characters" />
		</td>
	</tr>
			
	<tr>
		<td class="required" style="width:130px;"><span id="labeltxtConfirmpassword" runat="server">Confirm Password</span></td>
		<td class="field"><asp:textbox id="txtConfirmpassword" TextMode="password" runat="server" style="width:150px;" maxlength="20" />
		       <CC:comparevalidatorfront id="cvtxtPassword" runat="server" Display="none" ControlToValidate="txtPassword" ControlToCompare="txtConfirmPassword" Operator="Equal" Type="String" ErrorMessage="The passwords you entered do not match" /> 
		</td>
	</tr>						
	    
	 
	<tr>
        <td>&nbsp;</td>
		<td>&nbsp;</td>
	</tr>    
	</table>

	<table>
    <tr>
        <td>Billing Address</td>	
        <td></td>
    </tr>
    
    <tr>
        <td>&nbsp;</td>
		<td>&nbsp;</td>
	</tr>    

  	<tr>
		<td class="required" style="width:130px;"><span id="labeltxtBillingFirstName" runat="server">First Name</span></td>
		<td class="field"><asp:textbox id="txtBillingFirstName" runat="server" style="width:200px;" maxlength="50" /><br /></td>
		<td><asp:requiredfieldvalidator id="rqtxtBillingFirstName" Display="Dynamic" runat="server" ControlToValidate="txtBillingFirstName" ErrorMessage="Billing first name is required" /></td>
	</tr>

	<tr>
		<td class="required" style="width:130px;"><span id="labeltxtBillingLastName" runat="server">Last Name</span></td>
		<td class="field"><asp:textbox id="txtBillingLastName" runat="server" style="width:200px;" maxlength="50" /><br /></td>
		<td><asp:requiredfieldvalidator id="rqtxtBillingLastName" runat="server" Display="Dynamic" ControlToValidate="txtBillingLastName" ErrorMessage="Billing last name is required" />
        </td>
	</tr>

	<tr>
		<td class="optional" style="width:130px;">Company/School</td>
		<td class="field"><asp:textbox id="txtBillingCompany" runat="server" style="width:200px;" maxlength="50" /><br /></td>
		<td></td>
	</tr>

	<tr>
		<td class="required" style="width:130px;"><span id="labeltxtBillingAddress1" runat="server">Address 1</span></td>
		<td class="field"><asp:textbox id="txtBillingAddress1" runat="server" style="width:200px;" maxlength="50" /><br /></td>
		<td><asp:requiredfieldvalidator id="rqtxtBillingAddress1" runat="server" Display="Dynamic" ControlToValidate="txtBillingAddress1" ErrorMessage="Billing address is required" /></td>
	</tr>

	<tr>
		<td class="optional" style="width:130px;">Address 2</td>
		<td class="field"><asp:textbox id="txtBillingAddress2" runat="server" style="width:200px;" maxlength="50" /><br /></td>
	</tr>
    
	<tr>
		<td class="required" style="width:130px;"><span id="labeltxtBillingCity" runat="server">City</span></td>
	    <td class="field"><asp:textbox id="txtBillingCity" runat="server" style="width:200px;" maxlength="50" /><br /></td>
	    <td><asp:requiredfieldvalidator id="rqtxtBillingCity" runat="server" Display="Dynamic" ControlToValidate="txtBillingCity" Enabled="false"  ErrorMessage="Billing city is required" /></td>
	</tr>

	<tr>
		<td class="required" style="width:130px;"><span id="labeldrpBillingState" runat="server">State</span></td>
		<td class="field">
        	<asp:dropdownlist id="drpBillingState" runat="server" />
		</td>
		<td><asp:requiredfieldvalidator id="rqdrpBillingState" runat="server" Display="Dynamic" ControlToValidate="drpBillingState" ErrorMessage="Billing state is required" /></td>
	</tr>

	<tr>
		<td class="required" style="width:130px;"><span id="labeltxtBillingZip" runat="server">ZIP/Postal Code</span></td>
		<td class="field"><asp:textbox id="txtBillingZip" runat="server" style="width:100px;" maxlength="15" /><br /></td>
		<td><asp:requiredfieldvalidator id="rqtxtBillingZip" runat="server" Display="Dynamic" ControlToValidate="txtBillingZip" ErrorMessage="Billing zip is required" /></td>
	</tr>

	<tr>
		<td class="optional" style="width:130px;">Province/Region
		<div class="smallest" style="color:#999999; font-weight:normal;">(if applicable)</div>
		</td>
		<td class="field"><asp:textbox id="txtBillingRegion" runat="server" style="width:200px;" maxlength="50" /><br /></td>
	</tr>

	<tr>
		<td class="required" style="width:130px;"><span id="labeldrpBillingCountry" runat="server">Country</span></td>
		<td class="field">
			<asp:dropdownlist id="drpBillingCountry" runat="server" />
		</td>
		<td><asp:requiredfieldvalidator id="rqdrpBillingCountry" runat="server" Display="Dynamic" ControlToValidate="drpBillingCountry" ErrorMessage="Billing country is required" /></td>
	</tr>

	<tr>
	    <td class="required" style="width:130px;"><span id="labeltxtBillingPhone" runat="server">Phone:</span></td>
		<td class="field"><asp:textbox id="txtBillingPhone" runat="server" style="width:200px;" maxlength="50" /><br /></td>
		<td><asp:requiredfieldvalidator id="rqtxtBillingPhone" runat="server" Display="Dynamic" ControlToValidate="txtBillingPhone" ErrorMessage="Billing phone number is required" /></td>
	</tr>
	
	<tr>
        <td>&nbsp;</td>
		<td>&nbsp;</td>
	</tr>    
		
	<tr >
	    <td>Shipping Address</td>
		<td>&nbsp;</td>
	</tr>        
	
	<tr>
        <td>&nbsp;</td>
		<td>&nbsp;</td>
	</tr>    
	
	<tr>
		<td colspan="2">
		  <asp:checkbox id="chkSameAsBilling" runat="server" onclick="RefreshPage();" /> <label for="<%=chkSameAsBilling.clientId %>" class='smaller'>Same as Billing Address</label>
		</td>
     </tr>
     
     </table>	
     
    <table id="spanShippingAddress">
     <tr>
	    <td class="required" style="width:130px;"><span id="labeltxtShippingFirstName" runat="server">First Name</span></td>
		<td class="field"><asp:textbox id="txtShippingFirstName" runat="server" style="width:200px;" maxlength="50" /><br /></td>
		<td><asp:requiredfieldvalidator id="rqtxtShippingFirstName" runat="server" Display="Dynamic" ControlToValidate="txtShippingFirstName" Enabled="false" ErrorMessage="Shipping first name is required" /></td>
	</tr>

	<tr>
		<td class="required" style="width:130px;"><span id="labeltxtShippingLastName" runat="server">Last Name</span></td>
		<td class="field"><asp:textbox id="txtShippingLastName" runat="server" style="width:200px;" maxlength="50"/><br /></td>
		<td><asp:requiredfieldvalidator id="rqtxtShippingLastName" runat="server" Display="Dynamic" ControlToValidate="txtShippingLastName" Enabled="false" ErrorMessage="Shipping last name is required" />
        </td>
	</tr>

	<tr>
		<td class="optional" style="width:130px;">Company/School</td>
		<td class="field"><asp:textbox id="txtShippingCompany" runat="server" style="width:200px;" maxlength="50" /><br /></td>
		<td></td>
    </tr>

    <tr>
		<td class="required" style="width:130px;"><span id="labeltxtShippingAddress1" runat="server">Address 1</span></td>
		<td class="field"><asp:textbox id="txtShippingAddress1" runat="server" style="width:200px;" maxlength="50" /><br /></td>
		<td><asp:requiredfieldvalidator id="rqtxtShippingAddress1" runat="server" Display="Dynamic" ControlToValidate="txtShippingAddress1" Enabled="false" ErrorMessage="Shipping address is required" /></td>
	</tr>

	<tr>
		<td class="optional" style="width:130px;">Address 2</td>
		<td class="field"><asp:textbox id="txtShippingAddress2" runat="server" style="width:200px;" maxlength="50" /><br /></td>
        <td></td>
	</tr>

	<tr>
		<td class="required" style="width:130px;"><span id="labeltxtShippingCity" runat="server">City</span></td>
		<td class="field"><asp:textbox id="txtShippingCity" runat="server" style="width:200px;" maxlength="50" /><br /></td>
		<td><asp:requiredfieldvalidator id="rqtxtShippingCity" runat="server" Display="Dynamic" Enabled="False" ControlToValidate="txtShippingCity"  ErrorMessage="Shipping city is required" /></td>
	</tr>

	<tr>
		<td class="required" style="width:130px;"><span id="labeldrpShippingState" runat="server">State</span></td>
		<td class="field">
			<asp:dropdownlist id="drpShippingState" runat="server" />
		</td>
		<td><asp:requiredfieldvalidator id="rqdrpShippingState" runat="server" Enabled="false" Display="Dynamic" ControlToValidate="drpShippingState" ErrorMessage="Shipping state is required" /></td>
	</tr>

	<tr>
		<td class="required" style="width:130px;"><span id="labeltxtShippingZip" runat="server">ZIP/Postal Code</span></td>
		<td class="field"><asp:textbox id="txtShippingZip" runat="server" style="width:100px;" maxlength="15" /><br /></td>
		<td><asp:requiredfieldvalidator id="rqtxtShippingZip" runat="server" Enabled="false" Display="Dynamic" ControlToValidate="txtShippingZip" ErrorMessage="Shipping zip is required" /></td>
	</tr>

	<tr>
		<td class="optional" style="width:130px;">Province/Region
			<div class="smallest" style="color:#999999; font-weight:normal;">(if applicable)</div>
		</td>
		<td class="field"><asp:textbox id="txtShippingRegion" runat="server" style="width:200px;" maxlength="50" /><br /></td>		
	</tr>


	<tr valign="top">
		<td class="required" style="width:130px;"><span id="labeldrpShippingCountry" runat="server">Country</span></td>
		<td class="field">
			<asp:dropdownlist id="drpShippingCountry" runat="server" />
		</td>
		<td><asp:requiredfieldvalidator id="rqdrpShippingCountry" Enabled="false" runat="server" Display="Dynamic" ControlToValidate="drpShippingCountry" ErrorMessage="Shipping country is required" /></td>
	</tr>

	<tr valign="top">
	    <td class="required" style="width:130px;"><span id="labeltxtShippingPhone" runat="server">Phone:</span></td>
		<td class="field"><asp:textbox id="txtShippingPhone" runat="server" style="width:200px;" maxlength="50" /><br /></td>
		<td><asp:requiredfieldvalidator id="rqtxtShippingPhone" runat="server" Display="Dynamic" Enabled="false" ControlToValidate="txtShippingPhone" ErrorMessage="Shipping phone number is required" /> </td>
    </tr>          
	
     </table>
     <table>
       
        <tr>
            <td>&nbsp;</td>
		    <td>&nbsp;</td>
	    </tr>   

        <tr>
            <td>Newsletter Registration</td>
            <td></td>
        </tr>
        
        <tr>
            <td>&nbsp;</td>
		    <td>&nbsp;</td>
	    </tr>   
        
        <tr>
            <td class="optional smaller" style="width:300px;">
            	   Learn about sales and updates before anyone else.<br />Sign up for our newsletter and receive periodic updates from us.
	        </td>
	        <td class="field" style="width:300px;">      
               	<div class="smaller" style="line-height:16px; margin:0 0 4px 10px;">
		            <asp:radiobutton id="rbtnNewsletterYes" runat="server" GroupName="Newsletter" onclick="RefreshPage();" />
		            <label for="<%=rbtnNewsletterYes.clientId %>">Yes, I would like to receive updates and special promotions via e-mail.</label>
	            </div>
	            <div class="smaller" style="line-height:16px; margin:0 0 4px 10px;">
		            <asp:radiobutton id="rbtnNewsletterNo" runat="server" GroupName="Newsletter" onclick="RefreshPage();" />
		            <label for="<%=rbtnNewsletterNo.clientId %>">No, I would not like to receive updates and special promotions via e-mail.</label>
	            </div>	            
             </td>	  
             <td><CC:CustomValidatorFront id="cvrbtnNewsletterYes" runat="server" Display="Dynamic" ControlToValidate="rbtnNewsletterYes" ErrorMessage="You must indicate if you wish to receive the newsletter" /> </td>       
	     </tr>
	     
	     </table>
	         
	    <table id="spanNewsLetter" style="display:block;">          
          <tr>
             <td class="required" style="width:300px;">&nbsp;E-mail Format</td>
             <td class="field" style="width:300px;"><asp:radiobutton id="rbtnFormatHTML" runat="server" GroupName="NewsletterFormat" /><label for="<%=rbtnFormatHTML.ClientId %>">HTML</label><br />
             <asp:radiobutton id="rbtnFormatTEXT" runat="server" GroupName="NewsletterFormat" /><label for="<%=rbtnFormatTEXT.ClientId %>">Plain Text</label>
             </td>
		 </tr>
		
		  <tr>
		      <td class="required" style="width:300px;"><span id="labelchklstMailingLists" runat="server">&nbsp;Lists</span></td>
		      <td class="field" style="width:300px;"><CC:CheckboxlistEx id="chklstMailingLists" cellspacing="4" runat="server" TextAlign="right" RepeatDirection="Horizontal" RepeatColumns="3" /></td>
		      <td><CC:RequiredCheckboxListValidatorFront id="cvrbtnNewsletterYesAtLeastOne" Display="Dynamic" runat="server" ControlToValidate="chklstMailingLists" ErrorMessage="You must specify at least one mailing list if you are subscribing to the newsletter" /> </td>
		  </tr>
		
		
		<tr>
		<td></td>
		<td></td>
		</tr>
	
</table>

<p></p>
<CC:OneClickButton CssClass="btn" id="btnSubmit" runat="server" text="Save" />
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:PlaceHolder>
</asp:content>