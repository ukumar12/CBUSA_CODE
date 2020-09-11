<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Builder Registration Phase Expenditure"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If BuilderRegistrationPhaseExpenditureID = 0 Then %>Add<% Else %>Edit<% End If %> Builder Registration Phase Expenditure</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Builder Registration ID:</td>
		<td class="field"><asp:textbox id="txtBuilderRegistrationID" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvBuilderRegistrationID" runat="server" Display="Dynamic" ControlToValidate="txtBuilderRegistrationID" ErrorMessage="Field 'Builder Registration ID' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvBuilderRegistrationID" ControlToValidate="txtBuilderRegistrationID" ErrorMessage="Field 'Builder Registration ID' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Supply Phase ID:</td>
		<td class="field"><asp:DropDownList id="drpSupplyPhaseID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvSupplyPhaseID" runat="server" Display="Dynamic" ControlToValidate="drpSupplyPhaseID" ErrorMessage="Field 'Supply Phase ID' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Amount Spent Last Year:</td>
		<td class="field"><asp:textbox id="txtAmountSpentLastYear" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvAmountSpentLastYear" runat="server" Display="Dynamic" ControlToValidate="txtAmountSpentLastYear" ErrorMessage="Field 'Amount Spent Last Year' is blank"></asp:RequiredFieldValidator><CC:FloatValidator Display="Dynamic" runat="server" id="fvAmountSpentLastYear" ControlToValidate="txtAmountSpentLastYear" ErrorMessage="Field 'Amount Spent Last Year' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Preferred Vendor ID:</td>
		<td class="field"><asp:DropDownList id="drpPreferredVendorID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvPreferredVendorID" runat="server" Display="Dynamic" ControlToValidate="drpPreferredVendorID" ErrorMessage="Field 'Preferred Vendor ID' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Other Preferred Vendor:</td>
		<td class="field"><asp:textbox id="txtOtherPreferredVendor" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Builder Registration Phase Expenditure?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
