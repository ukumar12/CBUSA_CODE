<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="PIQ Ad"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If PIQAdID = 0 Then %>Add<% Else %>Edit<% End If %> PIQ Ad/Call Out</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">PIQ:</td>
		<td class="field"><asp:Literal ID="ltrPIQ" runat="server" ></asp:Literal></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">Ad/Call Out:</td>
		<td class="field"><CC:FileUpload ID="fuAdFile" Folder="/assets/piq/ads" ImageDisplayFolder="/assets/piq/ads" DisplayImage="True" runat="server" style="width: 319px;" /></td>
		<td><CC:RequiredFileUploadValidator ID="rfvAdFile" runat="server" Display="Dynamic" ControlToValidate="fuAdFile" ErrorMessage="Field 'Ad/Call Out' is required"></CC:RequiredFileUploadValidator><CC:FileUploadExtensionValidator Extensions="jpg,jpeg,gif,bmp" ID="feAdFile" runat="server" Display="Dynamic" ControlToValidate="fuAdFile" ErrorMessage="Invalid file extension"></CC:FileUploadExtensionValidator></td>
	</tr>
	<tr>
		<td class="required">Alternate Text:</td>
		<td class="field"><asp:textbox id="txtAltText" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvAltText" runat="server" Display="Dynamic" ControlToValidate="txtAltText" ErrorMessage="Field 'Alternate Text' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Ad Link:</td>
		<td class="field"><asp:textbox id="txtLinkURL" runat="server" maxlength="500" columns="50" style="width: 319px;"></asp:textbox><br/><span class="smaller">http:// or https:// are required</span></td>
		<td><asp:RequiredFieldValidator ID="rfvLinkURL" runat="server" Display="Dynamic" ControlToValidate="txtLinkURL" ErrorMessage="Field 'Ad Link' is blank"></asp:RequiredFieldValidator><CC:URLValidator Display="Dynamic" runat="server" id="lnkvLinkURL" ControlToValidate="txtLinkURL" ErrorMessage="Link 'Ad Link' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Start Date:</td>
		<td class="field"><CC:DatePicker ID="dtStartDate" runat="server"></CC:DatePicker></td>
		<td><CC:DateValidator Display="Dynamic" runat="server" id="dtvStartDate" ControlToValidate="dtStartDate" ErrorMessage="Date Field 'Start Date' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">End Date:</td>
		<td class="field"><CC:DatePicker ID="dtEndDate" runat="server"></CC:DatePicker></td>
		<td><CC:DateValidator Display="Dynamic" runat="server" id="dtvEndDate" ControlToValidate="dtEndDate" ErrorMessage="Date Field 'End Date' is invalid" /></td>
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
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this PIQ Ad?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

