<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Sample Monthly Stat"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If SampleMonthlyStatID = 0 Then %>Add<% Else %>Edit<% End If %> Sample Monthly Stat</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="optional">Started Units:</td>
		<td class="field"><asp:textbox id="txtStartedUnits" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><CC:IntegerValidator Display="Dynamic" runat="server" id="fvStartedUnits" ControlToValidate="txtStartedUnits" ErrorMessage="Field 'Started Units' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Sold Units:</td>
		<td class="field"><asp:textbox id="txtSoldUnits" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><CC:IntegerValidator Display="Dynamic" runat="server" id="fvSoldUnits" ControlToValidate="txtSoldUnits" ErrorMessage="Field 'Sold Units' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Closing Units:</td>
		<td class="field"><asp:textbox id="txtClosingUnits" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><CC:IntegerValidator Display="Dynamic" runat="server" id="fvClosingUnits" ControlToValidate="txtClosingUnits" ErrorMessage="Field 'Closing Units' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Unsold Units:</td>
		<td class="field"><asp:textbox id="txtUnsoldUnits" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><CC:IntegerValidator Display="Dynamic" runat="server" id="fvUnsoldUnits" ControlToValidate="txtUnsoldUnits" ErrorMessage="Field 'Unsold Units' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Time Period Date:</td>
		<td class="field"><CC:DatePicker ID="dtTimePeriodDate" runat="server"></CC:DatePicker></td>
		<td><CC:DateValidator Display="Dynamic" runat="server" id="dtvTimePeriodDate" ControlToValidate="dtTimePeriodDate" ErrorMessage="Date Field 'Time Period Date' is invalid" /></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Sample Monthly Stat?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
