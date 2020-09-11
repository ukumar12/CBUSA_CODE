<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Manufacturer"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If ManufacturerID = 0 Then %>Add<% Else %>Edit<% End If %> Manufacturer</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Manufacturer:</td>
		<td class="field"><asp:textbox id="txtManufacturer" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvManufacturer" runat="server" Display="Dynamic" ControlToValidate="txtManufacturer" ErrorMessage="Field 'Manufacturer' is blank"></asp:RequiredFieldValidator></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Manufacturer?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
