<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Swatches"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If SwatchId = 0 Then %>Add<% Else %>Edit<% End If %> Swatch</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Name:</td>
		<td class="field"><asp:textbox id="txtName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvName" runat="server" Display="Dynamic" ControlToValidate="txtName" ErrorMessage="Field 'Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">SKU:</td>
		<td class="field"><asp:textbox id="txtSKU" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Price:</td>
		<td class="field"><asp:textbox id="txtPrice" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><CC:FloatValidator Display="Dynamic" runat="server" id="fvPrice" ControlToValidate="txtPrice" ErrorMessage="Field 'Price' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Weight:</td>
		<td class="field"><asp:textbox id="txtWeight" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><CC:FloatValidator Display="Dynamic" runat="server" id="fvWeight" ControlToValidate="txtWeight" ErrorMessage="Field 'Weight' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Image:</td>
		<td class="field"><CC:FileUpload ID="fuImage" Folder="/assets/item/swatch" ImageDisplayFolder="/assets/item/swatch" DisplayImage="True" runat="server" style="width: 319px;" /></td>
		<td><CC:FileUploadExtensionValidator Extensions="jpg,jpeg,gif,bmp,png" ID="feImage" runat="server" Display="Dynamic" ControlToValidate="fuImage" ErrorMessage="Invalid file extension"></CC:FileUploadExtensionValidator></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Swatch?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

