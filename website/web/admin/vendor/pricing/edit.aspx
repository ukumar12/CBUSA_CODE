<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Vendor Product Price"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If VendorID = 0 Then %>Add<% Else %>Edit<% End If %> Vendor Product Price</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Product:</td>
		<td class="field"><asp:DropDownList id="drpProductID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvProductID" runat="server" Display="Dynamic" ControlToValidate="drpProductID" ErrorMessage="Field 'Product I D' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Vendor SKU:</td>
		<td class="field"><asp:textbox id="txtVendorSKU" runat="server" maxlength="30" columns="30" style="width: 199px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Vendor Price:</td>
		<td class="field"><asp:textbox id="txtVendorPrice" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><CC:FloatValidator Display="Dynamic" runat="server" id="fvVendorPrice" ControlToValidate="txtVendorPrice" ErrorMessage="Field 'Vendor Price' is invalid" /></td>
	</tr>
	<tr>
		<td class="required"><b>Is Substitution?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblIsSubstitution" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" />
			<asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
		<td><asp:RequiredFieldValidator ID="rfvIsSubstitution" runat="server" Display="Dynamic" ControlToValidate="rblIsSubstitution" ErrorMessage="Field 'Is Substitution' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Substitute Quantity Multiplier:</td>
		<td class="field"><asp:textbox id="txtSubstituteQuantityMultiplier" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><CC:IntegerValidator Display="Dynamic" runat="server" id="fvSubstituteQuantityMultiplier" ControlToValidate="txtSubstituteQuantityMultiplier" ErrorMessage="Field 'Substitute Quantity Multiplier' is invalid" /></td>
	</tr>
	<tr>
		<td class="required"><b>Is Upload?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblIsUpload" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" />
			<asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
		<td><asp:RequiredFieldValidator ID="rfvIsUpload" runat="server" Display="Dynamic" ControlToValidate="rblIsUpload" ErrorMessage="Field 'Is Upload' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional"><b>Is Discontinued?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblIsDiscontinued" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" />
			<asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Next Price:</td>
		<td class="field"><asp:textbox id="txtNextPrice" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><CC:FloatValidator Display="Dynamic" runat="server" id="fvNextPrice" ControlToValidate="txtNextPrice" ErrorMessage="Field 'Next Price' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Next Price Applies:</td>
		<td class="field"><CC:DatePicker ID="dtNextPriceApplies" runat="server"></CC:DatePicker></td>
		<td><CC:DateValidator Display="Dynamic" runat="server" id="dtvNextPriceApplies" ControlToValidate="dtNextPriceApplies" ErrorMessage="Date Field 'Next Price Applies' is invalid" /></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Vendor Product Price?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
