<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Supply Phase"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If SupplyPhaseID = 0 Then %>Add<% Else %>Edit<% End If %> Supply Phase</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Supply Phase:</td>
		<td class="field"><asp:textbox id="txtSupplyPhase" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvSupplyPhase" runat="server" Display="Dynamic" ControlToValidate="txtSupplyPhase" ErrorMessage="Field 'Supply Phase' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Parent Supply Phase:</td>
		<td class="field"><asp:DropDownList id="drpParentSupplyPhaseID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvParentSupplyPhase" runat="server" Display="Dynamic" ControlToValidate="drpParentSupplyPhaseID" ErrorMessage="Field 'Parent Supply Phase' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Price Lock Days:</td>
		<td class="field"><asp:textbox id="txtPriceLockDays" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvPriceLockDays" runat="server" Display="Dynamic" ControlToValidate="txtPriceLockDays" ErrorMessage="Field 'Price Lock Days' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvPriceLockDays" ControlToValidate="txtPriceLockDays" ErrorMessage="Field 'Price Lock Days' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Exclude from LLCs:</td>
		<td class="field"><CC:CheckBoxListEx ID="cblExcludedLLC" runat="server" RepeatColumns="3"></CC:CheckBoxListEx></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Supply Phase?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
