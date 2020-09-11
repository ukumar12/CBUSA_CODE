<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Content Items"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If ContentItemsID = 0 Then %>Add<% Else %>Edit<% End If %> Content Items</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	 
	<tr>
		<td class="optional">Title:</td>
		<td class="field"><asp:textbox id="txtTitle" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvTitle" runat="server" Display="Dynamic" ControlToValidate="txtTitle" ErrorMessage="Field 'Title' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">File Name:</td>
		<td class="field"><CC:FileUpload ID="fuFileName" Folder="/assets/contentitems" runat="server" style="width: 319px;" /></td>
		<td><CC:RequiredFileUploadValidator ID="rfvFileName" runat="server" Display="Dynamic" ControlToValidate="fuFileName" ErrorMessage="Field 'File Name' is required"></CC:RequiredFileUploadValidator><CC:FileUploadExtensionValidator Extensions="txt,csv,doc,pdf,jpg,jpeg,png,docx,xls,xlsx" ID="feFileName" runat="server" Display="Dynamic" ControlToValidate="fuFileName" ErrorMessage="Invalid file extension"></CC:FileUploadExtensionValidator></td>
	</tr>
 
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Content Items?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
