<%@ Page Language="vb" AutoEventWireup="false" Inherits="members_register" CodeFile="addresses.aspx.vb" %>

<CT:MasterPage runat="server" ID="CTMain" DefaultButton="btnSubmit" DefaultFocus="txtBillingFirstName">

<asp:PlaceHolder runat="server">    
<script type="text/javascript">
<!--     
	function RefreshPage() {
		var spanShippingAddress = document.getElementById('spanShippingAddress').style;
		var chkSameAsBilling = document.getElementById('<%=chkSameAsBilling.ClientId %>');
            
		if (chkSameAsBilling.checked) {
			spanShippingAddress.display="none";
		} else {
			spanShippingAddress.display="block";
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

<asp:PlaceHolder runat="server">    

<h2 class="hdng">Edit Default Billing and Shipping Address</h2>

<table cellspacing="0" cellpadding="0" border="0" style="width:761px; margin:10px 0 0 15px;" summary="billing and shipping">
<tr>
<td style="width:373px;"  class="loginheader">Billing Address</td>	
<td style="width:15px;"><img src="/images/spacer.gif" width="15" height="1" alt="" /><br /></td>
<td style="width:373px;" class="loginheader">Default Shipping Address</td>
</tr>

<tr valign="top">
<td style="width:373px;" class="loginborders">

		<table cellspacing="2" cellpadding="0" border="0" style="margin-left:6px" summary="shipping">

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
                      
        <table cellspacing="2" cellpadding="0" border="0" style="margin-left:6px" summary="billing">            
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

<table cellspacing="0" cellpadding="0" border="0" style="width:761px; margin:10px 0 0 15px;" summary="billing and shipping">
<tr>
<td>
        <CC:OneClickButton CssClass="btn" id="btnSubmit" runat="server" text="Save" />&nbsp;
        <input type="button" class="btn" value="Cancel" onclick="document.location.href='default.aspx'" />
</td>
</tr>
</table>

</asp:PlaceHolder>

<CC:requiredfieldvalidatorfront id="rqtxtBillingFirstName" runat="server" Display="None" ControlToValidate="txtBillingFirstName" ErrorMessage="Billing first name is required" />
<CC:requiredfieldvalidatorfront id="rqtxtBillingLastName" runat="server" Display="None" ControlToValidate="txtBillingLastName" ErrorMessage="Billing last name is required" />
<CC:requiredfieldvalidatorfront id="rqtxtBillingAddress1" runat="server" Display="None" ControlToValidate="txtBillingAddress1" ErrorMessage="Billing address is required" />
<CC:requiredfieldvalidatorfront id="rqtxtBillingCity" runat="server" Display="None" ControlToValidate="txtBillingCity" ErrorMessage="Billing city is required" />
<CC:requiredfieldvalidatorfront id="rqtxtBillingZip" runat="server" Display="None" ControlToValidate="txtBillingZip" ErrorMessage="Billing zip is required" />
<CC:requiredfieldvalidatorfront id="rqdrpBillingState" runat="server" Display="None" ControlToValidate="drpBillingState" ErrorMessage="Billing state is required" />
<CC:requiredfieldvalidatorfront id="rqdrpBillingCountry" runat="server" Display="None" ControlToValidate="drpBillingCountry" ErrorMessage="Billing country is required" />
<CC:requiredfieldvalidatorfront id="rqtxtBillingPhone" runat="server" Display="None" ControlToValidate="txtBillingPhone" ErrorMessage="Billing phone number is required" />

<CC:requiredfieldvalidatorfront id="rqtxtShippingFirstName" runat="server" Display="None" ControlToValidate="txtShippingFirstName" ErrorMessage="Shipping first name is required" />
<CC:requiredfieldvalidatorfront id="rqtxtShippingLastName" runat="server" Display="None" ControlToValidate="txtShippingLastName" ErrorMessage="Shipping last name is required" />
<CC:requiredfieldvalidatorfront id="rqtxtShippingAddress1" runat="server" Display="None" ControlToValidate="txtShippingAddress1" ErrorMessage="Shipping address is required" />
<CC:requiredfieldvalidatorfront id="rqtxtShippingCity" runat="server" Display="None" ControlToValidate="txtShippingCity" ErrorMessage="Shipping city is required" />
<CC:requiredfieldvalidatorfront id="rqtxtShippingZip" runat="server" Display="None" ControlToValidate="txtShippingZip" ErrorMessage="Shipping zip is required" />
<CC:requiredfieldvalidatorfront id="rqdrpShippingState" runat="server" Display="None" ControlToValidate="drpShippingState" ErrorMessage="Shipping state is required" />
<CC:requiredfieldvalidatorfront id="rqtxtShippingPhone" runat="server" Display="None" ControlToValidate="txtShippingPhone" ErrorMessage="Shipping phone number is required" />    
<CC:requiredfieldvalidatorfront id="rqdrpShippingCountry" runat="server" Display="None" ControlToValidate="drpShippingCountry" ErrorMessage="Shipping country is required" />

</CT:MasterPage>