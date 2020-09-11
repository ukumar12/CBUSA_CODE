<%@ Page Language="VB" AutoEventWireup="false" CodeFile="add.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="add"  Title="Rebate Term"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4>Add Rebate Term</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Vendor:</td>
		<td class="field"><asp:DropDownList id="drpVendorID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvVendorID" runat="server" Display="Dynamic" ControlToValidate="drpVendorID" ErrorMessage="Field 'Vendor' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Start Year:</td>
		<td class="field"><asp:textbox id="txtStartYear" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvStartYear" runat="server" Display="Dynamic" ControlToValidate="txtStartYear" ErrorMessage="Field 'Start Year' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvStartYear" ControlToValidate="txtStartYear" ErrorMessage="Field 'Start Year' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Start Quarter:</td>
		<td class="field"><asp:textbox id="txtStartQuarter" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvStartQuarter" runat="server" Display="Dynamic" ControlToValidate="txtStartQuarter" ErrorMessage="Field 'Start Quarter' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvStartQuarter" ControlToValidate="txtStartQuarter" ErrorMessage="Field 'Start Quarter' is invalid" /></td>
	</tr>
		<tr>
		<td class="required">Rebate Percentage:</td>
		<td class="field"><asp:textbox id="txtRebatePercentage" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvRebatePercentage" runat="server" Display="Dynamic" ControlToValidate="txtRebatePercentage" ErrorMessage="Field 'Rebate Percentage' is blank"></asp:RequiredFieldValidator><CC:FloatValidator Display="Dynamic" runat="server" id="fvRebatePercentage" ControlToValidate="txtRebatePercentage" ErrorMessage="Field 'Rebate Percentage' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Log Msg:</td>
		<td class="field"><asp:TextBox id="txtLogMsg" style="width: 349px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
		<td></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

