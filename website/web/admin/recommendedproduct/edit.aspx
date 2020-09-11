<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Recommended Product"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If RecommendedProductID = 0 Then %>Add<% Else %>Edit<% End If %> Recommended Product</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Vendor ID:</td>
		<td class="field"><asp:textbox id="txtVendorID" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvVendorID" runat="server" Display="Dynamic" ControlToValidate="txtVendorID" ErrorMessage="Field 'Vendor ID' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvVendorID" ControlToValidate="txtVendorID" ErrorMessage="Field 'Vendor ID' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Recommended Product:</td>
		<td class="field"><asp:textbox id="txtRecommendedProduct" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvRecommendedProduct" runat="server" Display="Dynamic" ControlToValidate="txtRecommendedProduct" ErrorMessage="Field 'Recommended Product' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Description:</td>
		<td class="field"><asp:textbox id="txtDescription" runat="server" maxlength="1000" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Manufacturer ID:</td>
		<td class="field"><asp:textbox id="txtManufacturerID" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><CC:IntegerValidator Display="Dynamic" runat="server" id="fvManufacturerID" ControlToValidate="txtManufacturerID" ErrorMessage="Field 'Manufacturer ID' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Size:</td>
		<td class="field"><asp:textbox id="txtSize" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Unit Of Measure ID:</td>
		<td class="field"><asp:DropDownList id="drpUnitOfMeasureID" runat="server" /></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Grade:</td>
		<td class="field"><asp:textbox id="txtGrade" runat="server" maxlength="20" columns="20" style="width: 139px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">Product Type ID:</td>
		<td class="field"><asp:DropDownList id="drpProductTypeID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvProductTypeID" runat="server" Display="Dynamic" ControlToValidate="drpProductTypeID" ErrorMessage="Field 'Product Type ID' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Recommended Product Status I D:</td>
		<td class="field"><asp:DropDownList id="drpRecommendedProductStatusID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvRecommendedProductStatusID" runat="server" Display="Dynamic" ControlToValidate="drpRecommendedProductStatusID" ErrorMessage="Field 'Recommended Product Status I D' is blank"></asp:RequiredFieldValidator></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Recommended Product?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
