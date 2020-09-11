<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Vendor Registration Business Reference"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If VendorRegistrationBusinessReferenceID = 0 Then %>Add<% Else %>Edit<% End If %> Vendor Registration Business Reference</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">First Name:</td>
		<td class="field"><asp:textbox id="txtFirstName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvFirstName" runat="server" Display="Dynamic" ControlToValidate="txtFirstName" ErrorMessage="Field 'First Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Last Name:</td>
		<td class="field"><asp:textbox id="txtLastName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvLastName" runat="server" Display="Dynamic" ControlToValidate="txtLastName" ErrorMessage="Field 'Last Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Company Name:</td>
		<td class="field"><asp:textbox id="txtCompanyName" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvCompanyName" runat="server" Display="Dynamic" ControlToValidate="txtCompanyName" ErrorMessage="Field 'Company Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">City:</td>
		<td class="field"><asp:textbox id="txtCity" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvCity" runat="server" Display="Dynamic" ControlToValidate="txtCity" ErrorMessage="Field 'City' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">State:</td>
		<td class="field"><asp:DropDownList id="drpState" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvState" runat="server" Display="Dynamic" ControlToValidate="drpState" ErrorMessage="Field 'State' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Zip:</td>
		<td class="field"><asp:textbox id="txtZip" runat="server" maxlength="15" columns="15" style="width: 109px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvZip" runat="server" Display="Dynamic" ControlToValidate="txtZip" ErrorMessage="Field 'Zip' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Phone:</td>
		<td class="field"><asp:textbox id="txtPhone" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvPhone" runat="server" Display="Dynamic" ControlToValidate="txtPhone" ErrorMessage="Field 'Phone' is blank"></asp:RequiredFieldValidator></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Vendor Registration Business Reference?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
