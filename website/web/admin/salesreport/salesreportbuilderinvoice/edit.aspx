<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Sales Report Builder Invoice"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If SalesReportBuilderInvoiceID = 0 Then %>Add<% Else %>Edit<% End If %> Sales Report Builder Invoice</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Sales Report I D:</td>
		<td class="field"><asp:textbox id="txtSalesReportID" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvSalesReportID" runat="server" Display="Dynamic" ControlToValidate="txtSalesReportID" ErrorMessage="Field 'Sales Report I D' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvSalesReportID" ControlToValidate="txtSalesReportID" ErrorMessage="Field 'Sales Report I D' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Builder I D:</td>
		<td class="field"><asp:DropDownList id="drpBuilderID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvBuilderID" runat="server" Display="Dynamic" ControlToValidate="drpBuilderID" ErrorMessage="Field 'Builder I D' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Invoice Amount:</td>
		<td class="field"><asp:textbox id="txtInvoiceAmount" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvInvoiceAmount" runat="server" Display="Dynamic" ControlToValidate="txtInvoiceAmount" ErrorMessage="Field 'Invoice Amount' is blank"></asp:RequiredFieldValidator><CC:FloatValidator Display="Dynamic" runat="server" id="fvInvoiceAmount" ControlToValidate="txtInvoiceAmount" ErrorMessage="Field 'Invoice Amount' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Invoice Number:</td>
		<td class="field"><asp:textbox id="txtInvoiceNumber" runat="server" maxlength="20" columns="20" style="width: 139px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Invoice Date:</td>
		<td class="field"><CC:DatePicker ID="dtInvoiceDate" runat="server"></CC:DatePicker></td>
		<td><CC:DateValidator Display="Dynamic" runat="server" id="dtvInvoiceDate" ControlToValidate="dtInvoiceDate" ErrorMessage="Date Field 'Invoice Date' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Creator Vendor Account I D:</td>
		<td class="field"><asp:textbox id="txtCreatorVendorAccountID" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvCreatorVendorAccountID" runat="server" Display="Dynamic" ControlToValidate="txtCreatorVendorAccountID" ErrorMessage="Field 'Creator Vendor Account I D' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvCreatorVendorAccountID" ControlToValidate="txtCreatorVendorAccountID" ErrorMessage="Field 'Creator Vendor Account I D' is invalid" /></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Sales Report Builder Invoice?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
