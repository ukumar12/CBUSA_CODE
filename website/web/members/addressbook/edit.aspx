<%@ Page Language="vb" AutoEventWireup="false" Inherits="member_addressbook_edit" CodeFile="edit.aspx.vb" %>

<CT:masterpage runat="server" id="CTMain" DefaultButton="btnSave">

    <CC:RequiredFieldValidatorFront ID="rfvtxtLabel" runat="server" Display="None" EnableClientScript="False" ControlToValidate="txtLabel" ErrorMessage="Field 'Label' is blank" />
    <CC:RequiredFieldValidatorFront ID="rfvFirstName" runat="server" Display="None" EnableClientScript="False" ControlToValidate="txtFirstName" ErrorMessage="Field 'First Name' is blank" />
    <CC:RequiredFieldValidatorFront ID="rfvLastName" runat="server" Display="None" EnableClientScript="False" ControlToValidate="txtLastName" ErrorMessage="Field 'Last Name' is blank" />
    <CC:RequiredFieldValidatorFront ID="rfvAddress1" runat="server" Display="None" EnableClientScript="False" ControlToValidate="txtAddress1" ErrorMessage="Field 'Address Ln 1' is blank" />
    <CC:RequiredFieldValidatorFront ID="rfvCity" runat="server" Display="None" EnableClientScript="False" ControlToValidate="txtCity" ErrorMessage="Field 'City' is blank" />
    <CC:RequiredFieldValidatorFront ID="rfvState" runat="server" Display="None" EnableClientScript="False" ControlToValidate="drpState" ErrorMessage="Field 'State' is blank" />		        
    <CC:RequiredFieldValidatorFront ID="rfvRegion" runat="server" Display="None" EnableClientScript="False" ControlToValidate="txtRegion" ErrorMessage="Field 'Region' is blank" />		        
    <CC:RequiredFieldValidatorFront ID="rfvCountry" runat="server" Display="None" EnableClientScript="False" ControlToValidate="drpCountry" ErrorMessage="Field 'Country' is blank" />		        
    <CC:RequiredFieldValidatorFront ID="rfvZip" runat="server" Display="None" EnableClientScript="False" ControlToValidate="txtZip" ErrorMessage="Field 'Zip' is blank" />		        
    <CC:RequiredFieldValidatorFront ID="rfvPhone" runat="server" Display="None" EnableClientScript="False" ControlToValidate="txtPhone" ErrorMessage="Field 'Phone' is blank" />		        
    
    <h2 class="hdng">Address Book - Add/Edit Entry</h2>

    <table cellspacing="0" cellpadding="0" border="0"> 
      <tr><td>      
			    <table cellspacing="2" cellpadding="0" border="0" style="margin-left:6px" summary="billing" class="bdr">            
			    <tr valign="top" class="loginheader"><td colspan="2">Address Details</td></tr>
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
			    <td class="fieldlbl" style="width:130px;"><span id="labeltxtLabel" runat="server">Label</span></td>
			    <td class="fieldpad">
				    <table cellspacing="0" cellpadding="0" border="0">
				    <tr>
				    <td class="fieldreq" id="bartxtLabel" runat="server">&nbsp;</td>
				    <td class="field"><asp:textbox id="txtLabel" runat="server" style="width:200px;" /></td>
				    </tr>
				    </table>
				    <div class="smallest" style="color:#999999; font-weight:normal;">(specify a short name for quick reference)</div>
			    </td>
    						
			    <tr valign="top">
			    <td class="fieldlbl" style="width:130px;"><span id="labeltxtFirstName" runat="server">First Name</span></td>
			    <td class="fieldpad">
				    <table cellspacing="0" cellpadding="0" border="0">
				    <tr>
				    <td class="fieldreq" id="bartxtFirstName" runat="server">&nbsp;</td>
				    <td class="field"><asp:textbox id="txtFirstName" runat="server" style="width:200px;" /><br /></td>
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
				    <td class="field"><asp:textbox id="txtLastName" runat="server" style="width:200px;" /><br /></td>
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
				    <td class="field"><asp:textbox id="txtCompany" runat="server" style="width:200px;" /><br /></td>
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
				    <td class="field"><asp:textbox id="txtAddress1" maxlength="30" runat="server" style="width:200px;" /><br /></td>
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
				    <td class="field"><asp:textbox id="txtAddress2" maxlength="30" runat="server" style="width:200px;" /><br /></td>
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
				    <td class="field"><asp:textbox id="txtCity" runat="server" style="width:200px;" /><br /></td>
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
				    <td class="field"><asp:textbox id="txtZip" runat="server" style="width:100px;" /><br /></td>
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
				    <td class="field"><asp:textbox id="txtRegion" runat="server" style="width:200px;" /><br /></td>
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
					    <asp:dropdownlist id="drpCountry" runat="server" />
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
				    <td class="field"><asp:textbox id="txtPhone" runat="server" style="width:200px;" /><br /></td>
				    </tr>
				    </table>
			    </td>
			    </tr>          
			    </table>
    			
            <p></p>
            <CC:OneClickButton id="btnSave" runat="server" Text="Save" CssClass="btn" />
            <input type="button" class="btn" value="Cancel" onclick="document.location.href='default.aspx'" />
        </td>
      </tr>
    </table>

</CT:masterpage>