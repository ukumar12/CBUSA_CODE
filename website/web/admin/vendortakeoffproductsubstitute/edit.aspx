<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Vendor Take Off Product Substitute"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If VendorID = 0 Then %>Add<% Else %>Edit<% End If %> Vendor Take Off Product Substitute</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Take Off Product I D:</td>
		<td class="field"><asp:DropDownList id="drpTakeOffProductID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvTakeOffProductID" runat="server" Display="Dynamic" ControlToValidate="drpTakeOffProductID" ErrorMessage="Field 'Take Off Product I D' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Substitute Product:</td>
		<td class="field"><asp:DropDownList id="drpSubstituteProductID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvSubstituteProductID" runat="server" Display="Dynamic" ControlToValidate="drpSubstituteProductID" ErrorMessage="Field 'Substitute Product' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Recommended Quantity:</td>
		<td class="field"><asp:textbox id="txtRecommendedQuantity" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvRecommendedQuantity" runat="server" Display="Dynamic" ControlToValidate="txtRecommendedQuantity" ErrorMessage="Field 'Recommended Quantity' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvRecommendedQuantity" ControlToValidate="txtRecommendedQuantity" ErrorMessage="Field 'Recommended Quantity' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Creator Vendor Account I D:</td>
		<td class="field"><asp:DropDownList id="drpCreatorVendorAccountID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvCreatorVendorAccountID" runat="server" Display="Dynamic" ControlToValidate="drpCreatorVendorAccountID" ErrorMessage="Field 'Creator Vendor Account I D' is blank"></asp:RequiredFieldValidator></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Vendor Take Off Product Substitute?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
