<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Custom Rebate Program"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If CustomRebateProgramID = 0 Then %>Add<% Else %>Edit<% End If %> Custom Rebate Program</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Vendor I D:</td>
		<td class="field"><asp:DropDownList id="drpVendorID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvVendorID" runat="server" Display="Dynamic" ControlToValidate="drpVendorID" ErrorMessage="Field 'Vendor I D' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Program Name:</td>
		<td class="field"><asp:textbox id="txtProgramName" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvProgramName" runat="server" Display="Dynamic" ControlToValidate="txtProgramName" ErrorMessage="Field 'Program Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Rebate Year:</td>
		<td class="field"><asp:textbox id="txtRebateYear" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvRebateYear" runat="server" Display="Dynamic" ControlToValidate="txtRebateYear" ErrorMessage="Field 'Rebate Year' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvRebateYear" ControlToValidate="txtRebateYear" ErrorMessage="Field 'Rebate Year' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Rebate Quarter:</td>
		<td class="field"><asp:textbox id="txtRebateQuarter" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvRebateQuarter" runat="server" Display="Dynamic" ControlToValidate="txtRebateQuarter" ErrorMessage="Field 'Rebate Quarter' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvRebateQuarter" ControlToValidate="txtRebateQuarter" ErrorMessage="Field 'Rebate Quarter' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Minimum Purchase:</td>
		<td class="field"><asp:textbox id="txtMinimumPurchase" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><CC:FloatValidator Display="Dynamic" runat="server" id="fvMinimumPurchase" ControlToValidate="txtMinimumPurchase" ErrorMessage="Field 'Minimum Purchase' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Rebate Percentage:</td>
		<td class="field"><asp:textbox id="txtRebatePercentage" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><CC:IntegerValidator Display="Dynamic" runat="server" id="fvRebatePercentage" ControlToValidate="txtRebatePercentage" ErrorMessage="Field 'Rebate Percentage' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Details:</td>
		<td class="field"><asp:TextBox id="txtDetails" style="width: 319px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
		<td></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Custom Rebate Program?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
