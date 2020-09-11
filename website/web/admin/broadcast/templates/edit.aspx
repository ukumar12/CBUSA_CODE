<%@ Page ValidateRequest="false" Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Mailing Template"%>
	
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If TemplateId = 0 Then %>Add<% Else %>Edit<% End If %> Mailing Template</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan=2><span class=smallest><span class=red>red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Name:</td>
		<td class="field"><asp:textbox id="txtName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvName" runat="server" Display="Dynamic" ControlToValidate="txtName" ErrorMessage="Field 'Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	
	<tr>
		<td class="required">ImageName:</td>
		<td class="field"><CC:FileUpload ID="fuImageName" Folder="/assets/broadcast/templates/" ImageDisplayFolder="/assets/broadcast/templates/" runat="server" style="width: 319px;" /></td>
	</tr>
	<tr><td colspan="2">&nbsp;</td></tr>
	<tr><td colspan="2">
	<strong>Warning:</strong><br />
    Please note that the unsubsribe links are different for Subsciber and Uploaded e-mail templates
	</td></tr>
	<tr><td colspan="2">&nbsp;</td></tr>
    <tr><th colspan="2" align="center" ><b>Template for Subscribers</b></th></tr>
	<tr>
		<td class="required">HTML:</td>
		<td class="field"><asp:TextBox id="txtHTMLMember" style="width: 600px;" Columns="55" rows="10" TextMode="Multiline" runat="server"></asp:TextBox></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">Text:</td>
		<td class="field"><asp:TextBox id="txtTextMember" style="width: 600px;" Columns="55" rows="10" TextMode="Multiline" runat="server"></asp:TextBox></td>
		<td></td>
	</tr>
	<tr><td colspan="2"><p>&nbsp;</p></td></tr>
	<tr><th colspan="2"  align="center"><b>Template for Uploaded/Imported E-mail addresses</b></th></tr>	
	<tr>
		<td class="required">HTML:</td>
		<td class="field"><asp:TextBox id="txtHTMLDynamic" style="width: 600px;" Columns="55" rows="10" TextMode="Multiline" runat="server"></asp:TextBox></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">Text:</td>
		<td class="field"><asp:TextBox id="txtTextDynamic" style="width: 600px;" Columns="55" rows="10" TextMode="Multiline" runat="server"></asp:TextBox></td>
		<td></td>
	</tr>
		<tr><td colspan="2">
	<p></p>
	<p>
	
<strong>Note:</strong> In accordance with the "CAN-SPAM" Act, a link to the unsubscribe page must be inserted at the end of the message along<br />with the physical mailing address.
	</p>
	<p></p>
	</td></tr>
</table>

	

	
<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Mailing Template?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

