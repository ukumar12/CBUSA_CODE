<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Inherits="member_reminders_edit" CodeFile="edit.aspx.vb" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4><%If m_ReminderId = 0 Then%> Add <%Else%> Edit <% End If%> Reminder</h4>



<table  summary="product"> 
  <tr><td>      
		    <table cellspacing="2" cellpadding="0" border="0" summary="billing">            
		    <tr>
		    <td colspan="2"><span class="smallest"><span class="red">red color</span>-denotes required field</span></td>
		    </tr>

		    <tr valign="top">
		    <td class="required" style="width:130px;"><span id="labeltxtName" runat="server">Event Name</span></td>
		    <td class="field"><asp:textbox id="txtName" runat="server" style="width:200px;" /></td>
		    <td><asp:RequiredFieldValidator ID="rfvtxtName" runat="server" Display="Dynamic" ControlToValidate="txtName" ErrorMessage="Field 'Event name' is blank" /></td>
		    </tr>
    		
		    <tr valign="top">
		    <td class="optional" style="width:130px;">Recurring Event?</td>
		    <td class="field"><asp:checkbox id="chkIsRecurring" runat="server" /> Yes</td>
			</tr>
    		
		    <tr valign="top">
		    <td class="required" style="width:130px;"><span id="labelctrlEventDate" runat="server">Event Date</span></td>
		    <td class="field"><CC:DatePicker ID="ctrlEventDate" runat="server" /></td>
		    <td><CC:RequiredDateValidator ID="rqdvctrlEventDate" runat="server" Display="Dynamic" ControlToValidate="ctrlEventDate" ErrorMessage="Field 'Event Date' is not valid." />
		    <CC:DateValidator ID="dvctrlEventDate" runat="server" Display="Dynamic" ControlToValidate="ctrlEventDate" ErrorMessage="Field 'Event Date' is not valid." />
		    </td>
			 </tr>
    		
		    <tr valign="top">
		    <td class="optional" style="width:130px;"><nobr>When to send 1st reminder?</nobr></td>
		    <td class="field"><asp:dropdownlist id="drpFirstReminder" runat="server" /></td>
			</tr>
    		    		
		    <tr valign="top">
		    <td class="optional" style="width:130px;"><nobr>When to send 2nd reminder?</nobr></td>
		    <td class="field"><asp:dropdownlist id="drpSecondReminder" runat="server" /></td>			  
		    </tr>
    		
		    <tr valign="top">
		    <td class="required" style="width:130px;"><span id="labeltxtEmail" runat="server">Email</span></td>
		    <td class="field"><asp:textbox id="txtEmail" runat="server" style="width:200px;" /></td>
		    <td><asp:RequiredFieldValidator ID="rfvtxtEmail" runat="server" Display="Dynamic" ControlToValidate="txtEmail" ErrorMessage="Field 'Email' is blank" />
		        <CC:EmailValidator ID="evftxtEmail" runat="server" Display="Dynamic" ControlToValidate="txtEmail" ErrorMessage="Field 'Email' is not a valid email address" />
		    </td>
			</tr>
    		
		    <tr valign="top">
		    <td class="optional" style="width:130px;">Comments</td>
		    <td class="field"><asp:textbox id="txtComments" TextMode="MultiLine" runat="server" rows="5" Columns="5" style="width:200px;" /></td>
			</tr>
		    </table>    		    		    		    		    		    		
			
        <p></p>
        <CC:OneClickButton id="btnSave" runat="server" Text="Save" CssClass="btn" />
        <CC:OneClickButton id="btnDelete" CausesValidation="False" runat="server" Text="Delete" CssClass="btn" />
        <CC:OneClickButton id="btnCancel" CausesValidation="False" runat="server" Text="Cancel" CssClass="btn" />
       
    </td>
  </tr>
</table>

</asp:content>