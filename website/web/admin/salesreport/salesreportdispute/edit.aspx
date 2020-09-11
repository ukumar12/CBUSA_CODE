<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Sales Report Dispute"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If SalesReportDisputeID = 0 Then %>Add<% Else %>Edit<% End If %> Sales Report Dispute</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Sales Report I D:</td>
		<td class="field"><asp:textbox id="txtSalesReportID" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvSalesReportID" runat="server" Display="Dynamic" ControlToValidate="txtSalesReportID" ErrorMessage="Field 'Sales Report I D' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvSalesReportID" ControlToValidate="txtSalesReportID" ErrorMessage="Field 'Sales Report I D' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Builder Total Amount:</td>
		<td class="field"><asp:textbox id="txtBuilderTotalAmount" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><CC:FloatValidator Display="Dynamic" runat="server" id="fvBuilderTotalAmount" ControlToValidate="txtBuilderTotalAmount" ErrorMessage="Field 'Builder Total Amount' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Vendor Total Amount:</td>
		<td class="field"><asp:textbox id="txtVendorTotalAmount" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><CC:FloatValidator Display="Dynamic" runat="server" id="fvVendorTotalAmount" ControlToValidate="txtVendorTotalAmount" ErrorMessage="Field 'Vendor Total Amount' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Dispute Reponse I D:</td>
		<td class="field"><asp:DropDownList id="drpDisputeResponseID" runat="server" /></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Dispute Reponse Reason I D:</td>
		<td class="field"><asp:DropDownList id="drpDisputeResponseReasonID" runat="server" /></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Resolution Amount:</td>
		<td class="field"><asp:textbox id="txtResolutionAmount" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><CC:FloatValidator Display="Dynamic" runat="server" id="fvResolutionAmount" ControlToValidate="txtResolutionAmount" ErrorMessage="Field 'Resolution Amount' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Builder Comments:</td>
		<td class="field"><asp:TextBox id="txtBuilderComments" style="width: 349px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Vendor Comments:</td>
		<td class="field"><asp:TextBox id="txtVendorComments" style="width: 349px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
		<td></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Sales Report Dispute?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
