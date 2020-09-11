<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Admin Message"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If AdminMessageID = 0 Then %>Add<% Else %>Edit<% End If %> Admin Message</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Subject:</td>
		<td class="field"><asp:textbox id="txtSubject" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvSubject" runat="server" Display="Dynamic" ControlToValidate="txtSubject" ErrorMessage="Field 'Subject' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Title:</td>
		<td class="field"><asp:textbox id="txtTitle" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvTitle" runat="server" Display="Dynamic" ControlToValidate="txtTitle" ErrorMessage="Field 'Title' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Start Date:</td>
		<td class="field"><CC:DatePicker ID="dtStartDate" runat="server"></CC:DatePicker></td>
		<td><CC:RequiredDateValidator Display="Dynamic" runat="server" id="rdtvStartDate" ControlToValidate="dtStartDate" ErrorMessage="Date Field 'Start Date' is blank" /><CC:DateValidator Display="Dynamic" runat="server" id="dtvStartDate" ControlToValidate="dtStartDate" ErrorMessage="Date Field 'Start Date' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">End Date:</td>
		<td class="field"><CC:DatePicker ID="dtEndDate" runat="server"></CC:DatePicker></td>
		<td><CC:DateValidator Display="Dynamic" runat="server" id="dtvEndDate" ControlToValidate="dtEndDate" ErrorMessage="Date Field 'End Date' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Message:</td>
		<td class="field"><asp:TextBox id="txtMessage" style="width: 349px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
		<td><asp:RequiredFieldValidator ID="rfvMessage" runat="server" Display="Dynamic" ControlToValidate="txtMessage" ErrorMessage="Field 'Message' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required"><b>Send Email Copy?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblSendEmailCopy" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" />
			<asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
		<td><asp:RequiredFieldValidator ID="rfvSendEmailCopy" runat="server" Display="Dynamic" ControlToValidate="rblSendEmailCopy" ErrorMessage="Field 'Send Email Copy' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required"><b>Is Active?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblIsActive" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" />
			<asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
		<td><asp:RequiredFieldValidator ID="rfvIsActive" runat="server" Display="Dynamic" ControlToValidate="rblIsActive" ErrorMessage="Field 'Is Active' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required"><b>Is Alert?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblIsAlert" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" />
			<asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
		<td><asp:RequiredFieldValidator ID="rfvIsAlert" runat="server" Display="Dynamic" ControlToValidate="rblIsAlert" ErrorMessage="Field 'Is Alert' is blank"></asp:RequiredFieldValidator></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Admin Message?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
