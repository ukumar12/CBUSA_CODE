<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Purchases Report Vendor P O"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If PurchasesReportVendorPOID = 0 Then %>Add<% Else %>Edit<% End If %> Purchases Report Vendor P O</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Purchases Report ID:</td>
		<td class="field"><asp:textbox id="txtPurchasesReportID" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvPurchasesReportID" runat="server" Display="Dynamic" ControlToValidate="txtPurchasesReportID" ErrorMessage="Field 'Purchases Report ID' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvPurchasesReportID" ControlToValidate="txtPurchasesReportID" ErrorMessage="Field 'Purchases Report ID' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Vendor ID:</td>
		<td class="field"><asp:DropDownList id="drpVendorID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvVendorID" runat="server" Display="Dynamic" ControlToValidate="drpVendorID" ErrorMessage="Field 'Vendor ID' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">PO Amount:</td>
		<td class="field"><asp:textbox id="txtPOAmount" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvPOAmount" runat="server" Display="Dynamic" ControlToValidate="txtPOAmount" ErrorMessage="Field 'PO Amount' is blank"></asp:RequiredFieldValidator><CC:FloatValidator Display="Dynamic" runat="server" id="fvPOAmount" ControlToValidate="txtPOAmount" ErrorMessage="Field 'PO Amount' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">PO Number:</td>
		<td class="field"><asp:textbox id="txtPONumber" runat="server" maxlength="20" columns="20" style="width: 139px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">PO Date:</td>
		<td class="field"><CC:DatePicker ID="dtPODate" runat="server"></CC:DatePicker></td>
		<td><CC:DateValidator Display="Dynamic" runat="server" id="dtvPODate" ControlToValidate="dtPODate" ErrorMessage="Date Field 'PO Date' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Creator Builder Account I D:</td>
		<td class="field"><asp:textbox id="txtCreatorBuilderAccountID" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvCreatorBuilderAccountID" runat="server" Display="Dynamic" ControlToValidate="txtCreatorBuilderAccountID" ErrorMessage="Field 'Creator Builder Account I D' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvCreatorBuilderAccountID" ControlToValidate="txtCreatorBuilderAccountID" ErrorMessage="Field 'Creator Builder Account I D' is invalid" /></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Purchases Report Vendor P O?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
