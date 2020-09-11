<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Mailing Template Slot"%>
	
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If SlotId = 0 Then %>Add<% Else %>Edit<% End If %> Mailing Template Slot</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan=2><span class="red">red color</span> - denotes required fields</td></tr>
	<tr>
		<td class="required">Template:</td>
		<td class="field"><%=dbTemplate.Name%>
		</td>
	</tr>
	<tr>
		<td class="required">Slot Name:</td>
		<td class="field"><asp:textbox id="txtSlotName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvSlotName" runat="server" Display="Dynamic" ControlToValidate="txtSlotName" ErrorMessage="Field 'Slot Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">ImageName:</td>
		<td class="field">
		    <CC:FileUpload ID="fuImageName" Folder="/assets/broadcast/templates/slots/" ImageDisplayFolder="/assets/broadcast/templates/slots/" runat="server" style="width: 319px;" />
		</td>
		<td><CC:RequiredFileUploadValidator ControlToValidate="fuImageName" ID="rvImage" runat="server" ErrorMessage="File name is blank"></CC:RequiredFileUploadValidator></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Mailing Template Slot?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
