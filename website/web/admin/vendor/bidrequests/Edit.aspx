<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="P O Quote Request"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If QuoteRequestId = 0 Then %>Add<% Else %>Edit<% End If %> P O Quote Request</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Quote Id:</td>
		<td class="field"><asp:DropDownList id="drpQuoteId" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvQuoteId" runat="server" Display="Dynamic" ControlToValidate="drpQuoteId" ErrorMessage="Field 'Quote Id' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Vendor Id:</td>
		<td class="field"><asp:DropDownList id="drpVendorId" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvVendorId" runat="server" Display="Dynamic" ControlToValidate="drpVendorId" ErrorMessage="Field 'Vendor Id' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Request Status:</td>
		<td class="field"><asp:textbox id="txtRequestStatus" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvRequestStatus" runat="server" Display="Dynamic" ControlToValidate="txtRequestStatus" ErrorMessage="Field 'Request Status' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Vendor Contact Name:</td>
		<td class="field"><asp:textbox id="txtVendorContactName" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Vendor Contact Phone:</td>
		<td class="field"><asp:textbox id="txtVendorContactPhone" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Vendor Contact Email:</td>
		<td class="field"><asp:textbox id="txtVendorContactEmail" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Quote Total:</td>
		<td class="field"><asp:textbox id="txtQuoteTotal" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><CC:FloatValidator Display="Dynamic" runat="server" id="fvQuoteTotal" ControlToValidate="txtQuoteTotal" ErrorMessage="Field 'Quote Total' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Quote Expiration Date:</td>
		<td class="field"><CC:DatePicker ID="dtQuoteExpirationDate" runat="server"></CC:DatePicker></td>
		<td><CC:DateValidator Display="Dynamic" runat="server" id="dtvQuoteExpirationDate" ControlToValidate="dtQuoteExpirationDate" ErrorMessage="Date Field 'Quote Expiration Date' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Start Date:</td>
		<td class="field"><CC:DatePicker ID="dtStartDate" runat="server"></CC:DatePicker></td>
		<td><CC:DateValidator Display="Dynamic" runat="server" id="dtvStartDate" ControlToValidate="dtStartDate" ErrorMessage="Date Field 'Start Date' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Completion Time:</td>
		<td class="field"><asp:textbox id="txtCompletionTime" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Payment Terms:</td>
		<td class="field"><asp:TextBox id="txtPaymentTerms" style="width: 349px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
		<td></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this P O Quote Request?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>