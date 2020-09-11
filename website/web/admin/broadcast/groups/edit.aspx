<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Mailing Group"%>
	
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If GroupId = 0 Then %>Add<% Else %>Edit<% End If %> Mailing Group</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan=2><span class=smallest><span class=red>red color</span> - denotes required fields</span></td></tr>
	<tr valign=top><th colspan=2>General</th></tr>
	<tr>
		<td class="required">Name:</td>
		<td class="field"><asp:textbox id="txtName" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvName" runat="server" Display="Dynamic" ControlToValidate="txtName" ErrorMessage="Field 'Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Description:</td>
		<td class="field"><asp:textbox TextMode="MultiLine" id="txtDescription" runat="server" maxlength="500" rows="4" columns="60" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Lists:</td>
		<td class="field"><CC:CheckBoxListEx ID="cblLists" runat="server" RepeatColumns="2"></CC:CheckBoxListEx></td>
	</tr>
	<tr><td colspan="2">&nbsp;</td></tr>
	<tr valign="top"><th colspan=2>Additional Search Criteria</th></tr>
	<tr>
	<td class="optional"><b>Subscription Date:</b></td>
	<td valign="top" class="field">
		<table border="0" cellpadding="0" cellspacing="0">
		<tr class="field">
			<td class="field"><span class="smallest">From:</span></td><td>&nbsp;</td><td class="field"><span class="smallest">To:</span></td>
		</tr>
		<tr class="field">
			<td><CC:DatePicker ID="dtStartDate" runat="server"></CC:DatePicker></td>
			<td>&nbsp;</td>
			<td><CC:DatePicker ID="dtEndDate" runat="server"></CC:DatePicker></td>
		</tr>
		</table>
	</td>
    <td>
    <CC:DateValidator Display="Dynamic" runat="server" id="dtvStartDate" ControlToValidate="dtStartDate" ErrorMessage="Date Field 'Start Date' is invalid" />
    <CC:DateValidator Display="Dynamic" runat="server" id="dtvEndDate" ControlToValidate="dtEndDate" ErrorMessage="Date Field 'End Date' is invalid" />
	</td>
	</tr>
</table>

<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Mailing Group?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>
</asp:content>

