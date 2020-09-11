<%@ Page Language="vb" AutoEventWireup="false" Inherits="member_reminders_edit" CodeFile="edit.aspx.vb" %>

<CT:masterpage runat="server" id="CTMain" DefaultButton="btnSave">

<CC:RequiredFieldValidatorFront ID="rfvtxtName" runat="server" Display="None" EnableClientScript="False" ControlToValidate="txtName" ErrorMessage="Field 'Event name' is blank" />
<CC:RequiredFieldValidatorFront ID="rfvtxtEmail" runat="server" Display="None" EnableClientScript="False" ControlToValidate="txtEmail" ErrorMessage="Field 'Email' is blank" />
<CC:EmailValidatorFront ID="evftxtEmail" runat="server" Display="None" EnableClientScript="False" ControlToValidate="txtEmail" ErrorMessage="Field 'Email' is not a valid email address" />
<CC:RequiredDateValidatorFront ID="rqdvctrlEventDate" runat="server" Display="none" EnableClientScript="False" ControlToValidate="ctrlEventDate" ErrorMessage="Field 'Event Date' is not valid." />
<CC:DateValidatorFront ID="dvctrlEventDate" runat="server" Display="none" EnableClientScript="False" ControlToValidate="ctrlEventDate" ErrorMessage="Field 'Event Date' is not valid." />

<h2 class="hdng">Edit Reminder</h2>

<table style="width:770px; margin:20px 0 15px 20px;" cellspacing="0" cellpadding="0" border="0"  summary="product"> 
  <tr><td>      
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
		    <td class="fieldlbl" style="width:130px;"><span id="labeltxtName" runat="server">Event Name</span></td>
		    <td class="fieldpad">
			    <table cellspacing="0" cellpadding="0" border="0">
			    <tr>
			    <td class="fieldreq" id="bartxtName" runat="server">&nbsp;</td>
			    <td class="field"><asp:textbox id="txtName" runat="server" style="width:200px;" /></td>
			    </tr>
			    </table>
		    </td>
		    </tr>
    		
		    <tr valign="top">
		    <td class="fieldlbl" style="width:130px;">Recurring Event?</td>
		    <td class="fieldpad">
			    <table cellspacing="0" cellpadding="0" border="0">
			    <tr>
			    <td>&nbsp;</td>
			    <td class="field"><asp:checkbox id="chkIsRecurring" runat="server" /> Yes</td>
			    </tr>
			    </table>
		    </td>
		    </tr>
    		
		    <tr valign="top">
		    <td class="fieldlbl" style="width:130px;"><span id="labelctrlEventDate" runat="server">Event Date</span></td>
		    <td class="fieldpad">
			    <table cellspacing="0" cellpadding="0" border="0">
			    <tr>
			    <td class="fieldreq" id="barctrlEventDate" runat="server">&nbsp;</td>
			    <td class="field"><CC:DatePicker ID="ctrlEventDate" runat="server" /></td>
			    </tr>
			    </table>
		    </td>
		    </tr>
    		
		    <tr valign="top">
		    <td class="fieldlbl" style="width:130px;"><nobr>When to send 1st reminder?</nobr></td>
		    <td class="fieldpad">
			    <table cellspacing="0" cellpadding="0" border="0">
			    <tr>
			    <td>&nbsp;</td>
			    <td class="field"><asp:dropdownlist id="drpFirstReminder" runat="server" /></td>
			    </tr>
			    </table>
		    </td>
		    </tr>
    		    		
		    <tr valign="top">
		    <td class="fieldlbl" style="width:130px;"><nobr>When to send 2nd reminder?</nobr></td>
		    <td class="fieldpad">
			    <table cellspacing="0" cellpadding="0" border="0">
			    <tr>
			    <td>&nbsp;</td>
			    <td class="field"><asp:dropdownlist id="drpSecondReminder" runat="server" /></td>
			    </tr>
			    </table>
		    </td>
		    </tr>
    		
		    <tr valign="top">
		    <td class="fieldlbl" style="width:130px;"><span id="labeltxtEmail" runat="server">Email</span></td>
		    <td class="fieldpad">
			    <table cellspacing="0" cellpadding="0" border="0">
			    <tr>
			    <td class="fieldreq" id="bartxtEmail" runat="server">&nbsp;</td>
			    <td class="field"><asp:textbox id="txtEmail" runat="server" style="width:200px;" /></td>
			    </tr>
			    </table>
		    </td>
		    </tr>
    		
		    <tr valign="top">
		    <td class="fieldlbl" style="width:130px;">Comments</td>
		    <td class="fieldpad">
			    <table cellspacing="0" cellpadding="0" border="0">
			    <tr>
			    <td>&nbsp;</td>
			    <td class="field"><asp:textbox id="txtComments" TextMode="MultiLine" runat="server" cols="5" style="width:200px;" /></td>
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