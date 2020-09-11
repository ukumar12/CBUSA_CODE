<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Custom Rebate"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If CustomRebateID = 0 Then %>Add<% Else %>Edit<% End If %> Custom Rebate</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Vendor:</td>
		<td class="field"><asp:DropDownList id="drpVendorID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvVendorID" runat="server" Display="Dynamic" ControlToValidate="drpVendorID" ErrorMessage="Field 'Vendor' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Builder:</td>
		<td class="field"><asp:DropDownList id="drpBuilderID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvBuilderID" runat="server" Display="Dynamic" ControlToValidate="drpBuilderID" ErrorMessage="Field 'Builder' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Custom Rebate Program I D:</td>
		<td class="field"><asp:DropDownList id="drpCustomRebateProgramID" runat="server" /></td>
		<td></td>
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
		<td class="optional">Applicable Purchase Amount:</td>
		<td class="field"><asp:textbox id="txtApplicablePurchaseAmount" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><CC:FloatValidator Display="Dynamic" runat="server" id="fvApplicablePurchaseAmount" ControlToValidate="txtApplicablePurchaseAmount" ErrorMessage="Field 'Applicable Purchase Amount' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Rebate Amount:</td>
		<td class="field"><asp:textbox id="txtRebateAmount" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvRebateAmount" runat="server" Display="Dynamic" ControlToValidate="txtRebateAmount" ErrorMessage="Field 'Rebate Amount' is blank"></asp:RequiredFieldValidator><CC:FloatValidator Display="Dynamic" runat="server" id="fvRebateAmount" ControlToValidate="txtRebateAmount" ErrorMessage="Field 'Rebate Amount' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Details:</td>
		<td class="field"><asp:textbox id="txtDetails" runat="server" maxlength="5000" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Custom Rebate?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

