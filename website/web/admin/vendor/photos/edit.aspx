<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Vendor Photo"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If PhotoId = 0 Then %>Add<% Else %>Edit<% End If %> Vendor Photo</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Vendor:</td>
		<td class="field"><asp:DropDownList id="drpVendorId" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvVendorId" runat="server" Display="Dynamic" ControlToValidate="drpVendorId" ErrorMessage="Field 'Vendor' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Photo:</td>
		<td class="field"><CC:FileUpload ID="fuPhoto" Folder="/assets/vendorphoto" ImageDisplayFolder="/assets/vendorphoto" DisplayImage="True" runat="server" style="width: 319px;" /></td>
		<td><CC:RequiredFileUploadValidator ID="rfvPhoto" runat="server" Display="Dynamic" ControlToValidate="fuPhoto" ErrorMessage="Field 'Photo' is required"></CC:RequiredFileUploadValidator><CC:FileUploadExtensionValidator Extensions="jpg,jpeg,gif,bmp" ID="fePhoto" runat="server" Display="Dynamic" ControlToValidate="fuPhoto" ErrorMessage="Invalid file extension"></CC:FileUploadExtensionValidator></td>
	</tr>
	<tr>
		<td class="optional">Caption:</td>
		<td class="field"><asp:textbox id="txtCaption" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Alt Text:</td>
		<td class="field"><asp:textbox id="txtAltText" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Vendor Photo?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

