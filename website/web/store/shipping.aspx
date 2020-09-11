<%@ Page Language="VB" AutoEventWireup="false" CodeFile="shipping.aspx.vb" Inherits="store_shipping" %>
<%@ Register TagName="StoreCheckoutSteps" TagPrefix="CC" Src="~/controls/StoreCheckoutSteps.ascx" %>
<%@ Register TagName="LimitTextBox" TagPrefix="CC" Src="~/controls/LimitTextBox.ascx" %>
<%@ Import Namespace="Components" %>

<CT:masterpage runat="server" id="CTMain">

<script type="text/javascript">
<!--
	function CopyShippingAddress(ctrl) {
		var id = ctrl.id.replace(/_drpAddresses/i,'');
		var i = ctrl.selectedIndex;
		var Field;
		
		Field = document.getElementById(id + '_txtFirstName'); if (i==0) Field.value = ''; else Field.value = saa[i].FirstName;
		Field = document.getElementById(id + '_txtLastName'); if (i==0) Field.value = ''; else Field.value = saa[i].LastName;
		Field = document.getElementById(id + '_txtCompany'); if (i==0) Field.value = ''; else Field.value = saa[i].Company;
		Field = document.getElementById(id + '_txtAddress1'); if (i==0) Field.value = ''; else Field.value = saa[i].Address1;
		Field = document.getElementById(id + '_txtAddress2'); if (i==0) Field.value = ''; else Field.value = saa[i].Address2;
		Field = document.getElementById(id + '_txtCity'); if (i==0) Field.value = ''; else Field.value = saa[i].City;
		Field = document.getElementById(id + '_drpState'); if (i==0) Field.value = ''; else Field.value = saa[i].State;
		Field = document.getElementById(id + '_txtZip'); if (i==0) Field.value = ''; else Field.value = saa[i].Zip;
		Field = document.getElementById(id + '_txtPhone'); if (i==0) Field.value = ''; else Field.value = saa[i].Phone;
		Field = document.getElementById(id + '_txtRegion'); if (i==0) Field.value = ''; else Field.value = saa[i].Region;
		Field = document.getElementById(id + '_drpCountry'); if (i==0) Field.value = ''; else Field.value = saa[i].Country;		
		__doPostBack(id + '_drpCountry','');
	}
	
    function CaptureGoogleAutofillChange() { 
       <asp:literal id="ltlGoogleAutoFillCapture" runat="server" />   
        setTimeout("CaptureGoogleAutofillChange()", 500);      
     } 	
	
	if (window.addEventListener) {
		window.addEventListener('load', CaptureGoogleAutofillChange, false);
	} else if (window.attachEvent) {
		window.attachEvent("onload",CaptureGoogleAutofillChange);
	}	
	
	function CopyGiftMessage(ctrl){
        var id = ctrl.id.replace(/_drpGiftMessage/i,'');
		var i = ctrl.selectedIndex;
		var j = ctrl.value;
		var Field;
	    Field = document.getElementById(id + '_txtGiftMessage_ctrl');if (i==0) Field.value = ''; else Field.value = j;
	    Field.focus();
     }
    
    var saa = new Array();
<asp:Repeater id="rptArray" runat="server">
<ItemTemplate>
	saa[<%#Container.ItemIndex+1%>] = new Object();
    saa[<%#Container.ItemIndex+1%>].FirstName = <%#Core.Escape(Container.DataItem("FirstName"))%>;
	saa[<%#Container.ItemIndex+1%>].LastName = <%#Core.Escape(Container.DataItem("LastName"))%>;
	saa[<%#Container.ItemIndex+1%>].Company = <%#Core.Escape(IIf(IsDBNull(Container.DataItem("Company")), String.Empty, Container.DataItem("Company")))%>;
    saa[<%#Container.ItemIndex+1%>].Address1 = <%#Core.Escape(Container.DataItem("Address1"))%>;
	saa[<%#Container.ItemIndex+1%>].Address2 = <%#Core.Escape(IIf(IsDBNull(Container.DataItem("Address2")), String.Empty, Container.DataItem("Address2")))%>;
    saa[<%#Container.ItemIndex+1%>].City = <%#Core.Escape(Container.DataItem("City"))%>;
	saa[<%#Container.ItemIndex+1%>].State = <%#Core.Escape(IIf(IsDBNull(Container.DataItem("State")), String.Empty, Container.DataItem("State")))%>;
	saa[<%#Container.ItemIndex+1%>].Zip = <%#Core.Escape(Container.DataItem("Zip"))%>;
	saa[<%#Container.ItemIndex+1%>].Phone = <%#Core.Escape(Container.DataItem("Phone"))%>;
    saa[<%#Container.ItemIndex+1%>].Region = <%#Core.Escape(IIf(IsDBNull(Container.DataItem("Region")), String.Empty, Container.DataItem("Region")))%>;
    saa[<%#Container.ItemIndex+1%>].Country = <%#Core.Escape(Container.DataItem("Country"))%>;     
</ItemTemplate>
</asp:Repeater>
//-->
</script>

<div class="stepswrpr"><CC:StoreCheckoutSteps ID="Steps" CurrentStep="Shipping" runat="server" /></div>
<asp:ScriptManager id="ScriptManager1" runat="server" />

<asp:Repeater runat="server" id="rptRecipients">
<ItemTemplate>

<table border="0" width="100%" cellpadding="5" cellspacing="0" class="bdrleft bdrtop bdrright bdrbottom">
<tr><td valign="top"><h3 class="hdng2">For <%#Container.DataItem("Label")%><%#IIf(IsDBNull(Container.DataItem("AddressId")), "", " (address book)")%></h3>
<tr><td valign="top">

        <a name="R<%#Container.DataItem("RecipientId")%>"></a>
        <input type="hidden" id="hdnRecipient" runat="server" />
        
        <table cellpadding="0" cellspacing="0">
        <tr><td valign="top">
        
        <table cellspacing="2" cellpadding="0" border="0" style="margin-left:6px" summary="shipping">
		<tr valign="top">
		<td>&nbsp;</td>
		<td class="fieldpad" style="text-align:right;">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldreq">&nbsp;</td>
			<td class="smaller">&nbsp;Indicates required field</td>
			</tr>
			</table>
		</td>
		</tr>

		<tr valign="top">
		<td class="fieldlbl">Select Address</td>
		<td>
		<asp:Dropdownlist id="drpAddresses" runat="server" onchange="CopyShippingAddress(this);" style="width:200px;" /><br />
        <span style="font-size:10px;">Select a delivery address from your address book or enter<br />a new delivery address below.</span>
		</td>
		</tr>
		<tr >
		<td>&nbsp;</td>
		<td>&nbsp;</td>
		</tr>           

		<tr valign="top">
		<td class="fieldlbl" style="width:130px;"><span id="labeltxtFirstName" runat="server">First Name</span></td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldreq" id="bartxtFirstName" runat="server">&nbsp;</td>
			<td class="field"><asp:textbox id="txtFirstName" runat="server" style="width:200px;" maxlength="50" /><br /></td>
			</tr>
			</table>
		</td>
		</tr>

		<tr valign="top">
		<td class="fieldlbl" style="width:130px;"><span id="labeltxtLastName" runat="server">Last Name</span></td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldreq" id="bartxtLastName" runat="server">&nbsp;</td>
			<td class="field"><asp:textbox id="txtLastName" runat="server" style="width:200px;" maxlength="50"/><br /></td>
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
			<td class="field"><asp:textbox id="txtCompany" runat="server" style="width:200px;" maxlength="50" /><br /></td>
			</tr>
			</table>
		</td>
		</tr>

		<tr valign="top">
		<td class="fieldlbl" style="width:130px;"><span id="labeltxtAddress1" runat="server">Address 1</span></td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldreq" id="bartxtAddress1" runat="server">&nbsp;</td>
			<td class="field"><asp:textbox id="txtAddress1" runat="server" style="width:200px;" maxlength="50" /><br /></td>
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
			<td class="field"><asp:textbox id="txtAddress2" runat="server" style="width:200px;" maxlength="50" /><br /></td>
			</tr>
			</table>
		</td>
		</tr>

		<tr valign="top">
		<td class="fieldlbl" style="width:130px;"><span id="labeltxtCity" runat="server">City</span></td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldreq" id="bartxtCity" runat="server">&nbsp;</td>
			<td class="field"><asp:textbox id="txtCity" runat="server" style="width:200px;" maxlength="50" /><br /></td>
			</tr>
			</table>
		</td>
		</tr>

		<tr valign="top">
		<td class="fieldlbl" style="width:130px;"><span id="labeldrpState" runat="server">State</span></td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldreq" id="bardrpState" runat="server">&nbsp;</td>
			<td class="field">
				<asp:dropdownlist id="drpState" runat="server" />
			</td>
			</tr>
			</table>
		</td>
		</tr>

		<tr valign="top">
		<td class="fieldlbl" style="width:130px;"><span id="labeltxtZip" runat="server">ZIP/Postal Code</span></td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldreq" id="bartxtZip" runat="server">&nbsp;</td>
			<td class="field"><asp:textbox id="txtZip" runat="server" style="width:100px;" maxlength="15" /><br /></td>
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
			<td class="field"><asp:textbox id="txtRegion" runat="server" style="width:200px;" maxlength="50" /><br /></td>
			</tr>
			</table>
		</td>
		</tr>


		<tr valign="top">
		<td class="fieldlbl" style="width:130px;"><span id="labeldrpCountry" runat="server">Country</span></td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldreq" id="bardrpCountry" runat="server">&nbsp;</td>
			<td class="field">
				<asp:dropdownlist id="drpCountry" AutoPostback="True" OnSelectedIndexChanged="drpCountry_SelectedIndexChanged" runat="server" />
			</td>
			</tr>
			</table>
		</td>
		</tr>

		<tr valign="top">
		<td class="fieldlbl" style="width:130px;"><span id="labeltxtPhone" runat="server">Phone:</span></td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldreq" id="bartxtPhone" runat="server">&nbsp;</td>
			<td class="field"><asp:textbox id="txtPhone" runat="server" style="width:200px;" maxlength="50" /><br /></td>
			</tr>
			</table>
		</td>
		</tr>          

    	<tr valign="top">
		<td class="fieldlbl" style="width:130px;"><span id="labeldrpShippingMethod" runat="server">Shipping Method:</span></td>
        <td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldreq" id="bardrpShippingMethod" runat="server">&nbsp;</td>
			<td class="field">
		        <asp:UpdatePanel id="UpdatePanelShippingMethod" runat="server" UpdateMode="Conditional">
		            <ContentTemplate>
		                <asp:HiddenField id="hdnCountry" runat="server" />
    			        <asp:dropdownlist id="drpShippingMethod" runat="server" />
			        </ContentTemplate>
			        <Triggers>
			          <asp:AsyncPostbackTrigger ControlId="drpCountry" EventName="SelectedIndexChanged" />
			        </Triggers>
			    </asp:UpdatePanel>
			</td>
			</tr>
			</table>
		</td>
        </tr>
        
        <tr valign="top" runat="server" id="trchkSaveShipping" >
		<td class="fieldlbl" style="width:130px;"></td>
        <td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td></td>
			<td class="field"><asp:checkbox id="chkSaveToAddressBook" runat="server" /> <span class="smaller">Save this address for <%#Container.DataItem("Label")%>.</span></td>
			</tr>
			</table>
		</td>
        </tr>
        </table>			
		
		</td>
        
        <td valign="top" width="80">&nbsp;</td><td valign="top" style="padding-top:50px;" id="tdGiftMessage" runat="server">
		
        <b>Gift Message:</b>

        <table cellspacing="4" cellpadding="0" border="0" style="margin-top:20px;">
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

</td></tr>
</table>

<CC:requiredfieldvalidatorfront id="rqtxtFirstName" runat="server" Display="None" ControlToValidate="txtFirstName" ErrorMessage="First name is required" />
<CC:requiredfieldvalidatorfront id="rqtxtLastName" runat="server" Display="None" ControlToValidate="txtLastName" ErrorMessage="Last name is required" />
<CC:requiredfieldvalidatorfront id="rqtxtAddress1" runat="server" Display="None" ControlToValidate="txtAddress1" ErrorMessage="Address is required" />
<CC:requiredfieldvalidatorfront id="rqtxtCity" runat="server" Display="None" ControlToValidate="txtCity" ErrorMessage="City is required" />
<CC:requiredfieldvalidatorfront id="rqtxtZip" runat="server" Display="None" ControlToValidate="txtZip" ErrorMessage="Zip is required" />
<CC:requiredfieldvalidatorfront id="rqdrpState" runat="server" Display="None" ControlToValidate="drpState" ErrorMessage="State is required" />
<CC:requiredfieldvalidatorfront id="rqtxtPhone" runat="server" Display="None" ControlToValidate="txtPhone" ErrorMessage="Phone number is required" />    
<CC:requiredfieldvalidatorfront id="rqdrpCountry" runat="server" Display="None" ControlToValidate="drpCountry" ErrorMessage="Country is required" />
<CC:requiredfieldvalidatorfront id="rqdrpShippingMethod" runat="server" Display="None" ControlToValidate="drpShippingMethod" ErrorMessage="Shipping method is required" />

</ItemTemplate>
<SeparatorTemplate>
<br />
</SeparatorTemplate>
</asp:Repeater>

<table style="width:763px; margin:10px 0 0 15px;"><tr><td align="right"><CC:OneClickButton CssClass="btn" id="btnSubmit" runat="server" text="Continue &raquo;" /></td></tr></table>

</CT:masterpage>