<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="State"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If StateId = 0 Then %>Add<% Else %>Edit<% End If %> State</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan=2><span class=smallest><span class=red>red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">State Code:</td>
		<td class="field"><asp:Literal id="ltlStateCode" runat="server" /></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">State Name:</td>
		<td class="field"><asp:Literal id="ltlStateName" runat="server" /></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">Tax Rate:</td>
		<td class="field"><asp:textbox id="txtTaxRate" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox>%</td>
		<td><asp:RequiredFieldValidator ID="rfvTaxRate" runat="server" Display="Dynamic" ControlToValidate="txtTaxRate" ErrorMessage="Field 'Tax Rate' is blank"></asp:RequiredFieldValidator><CC:FloatValidator Display="Dynamic" runat="server" id="fvTaxRate" ControlToValidate="txtTaxRate" ErrorMessage="Field 'Tax Rate' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional"><b>Shipping Taxable?</b></td>
		<td class="field"><asp:CheckBox runat="server" ID="chkTaxShipping" /></td>
	</tr>
	<tr>
		<td class="optional"><b>Gift Wrap Taxable?</b></td>
		<td class="field"><asp:CheckBox runat="server" ID="chkTaxGiftWrap" /></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

