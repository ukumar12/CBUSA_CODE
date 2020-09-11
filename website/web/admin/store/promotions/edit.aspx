<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Store Promotion"%>
	
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If PromotionId = 0 Then %>Add<% Else %>Edit<% End If %> Store Promotion</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan=2><span class=smallest><span class=red>red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Promotion Name:</td>
		<td class="field"><asp:textbox id="txtPromotionName" runat="server" maxlength="255" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvPromotionName" runat="server" Display="Dynamic" ControlToValidate="txtPromotionName" ErrorMessage="Field 'Promotion Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Promotion Code:</td>
		<td class="field"><asp:textbox id="txtPromotionCode" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvPromotionCode" runat="server" Display="Dynamic" ControlToValidate="txtPromotionCode" ErrorMessage="Field 'Promotion Code' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Promotion Type:</td>
		<td class="field">
		  <asp:DropDownList ID="drpPromotionType" runat="server">
		  <asp:ListItem Value=""></asp:ListItem>
		  <asp:ListItem Value="Percentage">Percent Off</asp:ListItem>
		  <asp:ListItem Value="Monetary">Dollar Off</asp:ListItem>
		  </asp:DropDownList>
		</td>
		<td><asp:RequiredFieldValidator ID="rfvdrpPromotionType" runat="server" Display="Dynamic" ControlToValidate="drpPromotionType" ErrorMessage="Field 'Promotion Type' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Message:</td>
		<td class="field"><asp:textbox id="txtMessage" runat="server" maxlength="255" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvMessage" runat="server" Display="Dynamic" ControlToValidate="txtMessage" ErrorMessage="Field 'Message' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Discount:</td>
		<td class="field"><asp:textbox id="txtDiscount" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvDiscount" runat="server" Display="Dynamic" ControlToValidate="txtDiscount" ErrorMessage="Field 'Discount' is blank"></asp:RequiredFieldValidator>
            <CC:FloatValidator ID="floatvDiscount" runat="server" ControlToValidate="txtDiscount" Display="Dynamic" ErrorMessage="Field 'Discount' is Invalid."></CC:FloatValidator></td>
	</tr>
	<tr>
		<td class="required">Start Date:</td>
		<td class="field"><CC:DatePicker ID="dtStartDate" runat="server"></CC:DatePicker></td>
		<td><CC:RequiredDateValidator Display="Dynamic" runat="server" id="rdtvStartDate" ControlToValidate="dtStartDate" ErrorMessage="Date Field 'Start Date' is blank" /><CC:DateValidator Display="Dynamic" runat="server" id="dtvStartDate" ControlToValidate="dtStartDate" ErrorMessage="Date Field 'Start Date' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">End Date:</td>
		<td class="field"><CC:DatePicker ID="dtEndDate" runat="server"></CC:DatePicker></td>
		<td><CC:RequiredDateValidator Display="Dynamic" runat="server" id="rdtvEndDate" ControlToValidate="dtEndDate" ErrorMessage="Date Field 'End Date' is blank" /><CC:DateValidator Display="Dynamic" runat="server" id="dtvEndDate" ControlToValidate="dtEndDate" ErrorMessage="Date Field 'End Date' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Minimum Purchase:</td>
		<td class="field"><asp:textbox id="txtMinimumPurchase" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><CC:FloatValidator Display="Dynamic" ControlToValidate="txtMinimumPurchase" runat="server" ID="floatvMinPurchase" ErrorMessage="Field 'Minimum Purchase' is invalid"></CC:FloatValidator> </td>
	</tr>
	<tr>
		<td class="optional">Maximum Purchase:</td>
		<td class="field"><asp:textbox id="txtMaximumPurchase" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><CC:FloatValidator Display="Dynamic" ControlToValidate="txtMaximumPurchase" runat="server" ID="floatvMaxPurchase" ErrorMessage="Field 'Maximum Purchase' is invalid"></CC:FloatValidator></td>
	</tr>
	<tr>
		<td class="optional"><b>Is Item Specific?</b></td>
		<td class="field"><asp:CheckBox runat="server" ID="chkIsItemSpecific" /></td>
	</tr>
	<tr>
		<td class="optional"><b>Is Free Shipping?</b></td>
		<td class="field"><asp:CheckBox runat="server" ID="chkIsFreeShipping" /></td>
	</tr>
	<tr>
		<td class="optional">Number Sent:</td>
		<td class="field"><asp:TextBox id="txtNumberSent" runat="server" maxlength="6" columns="50" style="width: 125px;"></asp:TextBox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Delivery Method:</td>
		<td class="field"><asp:TextBox id="txtDeliveryMethod" runat="server" maxlength="50" style="width: 200px;"></asp:TextBox> </td>
		<td></td>
	</tr>
	<tr>
		<td class="optional"><b>Is Active?</b></td>
		<td class="field"><asp:CheckBox runat="server" ID="chkIsActive" /></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Store Promotion?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
