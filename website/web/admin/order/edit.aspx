<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Order"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If OrderID = 0 Then %>Add<% Else %>Edit<% End If %> Order</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="optional">Historic I D:</td>
		<td class="field"><asp:textbox id="txtHistoricID" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><CC:IntegerValidator Display="Dynamic" runat="server" id="fvHistoricID" ControlToValidate="txtHistoricID" ErrorMessage="Field 'Historic I D' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Order Number:</td>
		<td class="field"><asp:textbox id="txtOrderNumber" runat="server" maxlength="20" columns="20" style="width: 139px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvOrderNumber" runat="server" Display="Dynamic" ControlToValidate="txtOrderNumber" ErrorMessage="Field 'Order Number' is blank"></asp:RequiredFieldValidator></td>
	</tr>
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
		<td class="optional">Project:</td>
		<td class="field"><asp:DropDownList id="drpProjectID" runat="server" /></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Historic Vendor I D:</td>
		<td class="field"><asp:textbox id="txtHistoricVendorID" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><CC:IntegerValidator Display="Dynamic" runat="server" id="fvHistoricVendorID" ControlToValidate="txtHistoricVendorID" ErrorMessage="Field 'Historic Vendor I D' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Historic Builder I D:</td>
		<td class="field"><asp:textbox id="txtHistoricBuilderID" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><CC:IntegerValidator Display="Dynamic" runat="server" id="fvHistoricBuilderID" ControlToValidate="txtHistoricBuilderID" ErrorMessage="Field 'Historic Builder I D' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Historic Project I D:</td>
		<td class="field"><asp:textbox id="txtHistoricProjectID" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><CC:IntegerValidator Display="Dynamic" runat="server" id="fvHistoricProjectID" ControlToValidate="txtHistoricProjectID" ErrorMessage="Field 'Historic Project I D' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Title:</td>
		<td class="field"><asp:textbox id="txtTitle" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvTitle" runat="server" Display="Dynamic" ControlToValidate="txtTitle" ErrorMessage="Field 'Title' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">P O Number:</td>
		<td class="field"><asp:textbox id="txtPONumber" runat="server" maxlength="20" columns="20" style="width: 139px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvPONumber" runat="server" Display="Dynamic" ControlToValidate="txtPONumber" ErrorMessage="Field 'P O Number' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Orderer First Name:</td>
		<td class="field"><asp:textbox id="txtOrdererFirstName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvOrdererFirstName" runat="server" Display="Dynamic" ControlToValidate="txtOrdererFirstName" ErrorMessage="Field 'Orderer First Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Orderer Last Name:</td>
		<td class="field"><asp:textbox id="txtOrdererLastName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvOrdererLastName" runat="server" Display="Dynamic" ControlToValidate="txtOrdererLastName" ErrorMessage="Field 'Orderer Last Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Orderer Email:</td>
		<td class="field"><asp:textbox id="txtOrdererEmail" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvOrdererEmail" runat="server" Display="Dynamic" ControlToValidate="txtOrdererEmail" ErrorMessage="Field 'Orderer Email' is blank"></asp:RequiredFieldValidator><CC:EmailValidator Display="Dynamic" runat="server" id="fvOrdererEmail" ControlToValidate="txtOrdererEmail" ErrorMessage="Field 'Orderer Email' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Orderer Phone:</td>
		<td class="field"><asp:textbox id="txtOrdererPhone" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvOrdererPhone" runat="server" Display="Dynamic" ControlToValidate="txtOrdererPhone" ErrorMessage="Field 'Orderer Phone' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Supervisor First Name:</td>
		<td class="field"><asp:textbox id="txtSuperFirstName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvSuperFirstName" runat="server" Display="Dynamic" ControlToValidate="txtSuperFirstName" ErrorMessage="Field 'Supervisor First Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Supervisor Last Name:</td>
		<td class="field"><asp:textbox id="txtSuperLastName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvSuperLastName" runat="server" Display="Dynamic" ControlToValidate="txtSuperLastName" ErrorMessage="Field 'Supervisor Last Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Supervisor Email:</td>
		<td class="field"><asp:textbox id="txtSuperEmail" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvSuperEmail" runat="server" Display="Dynamic" ControlToValidate="txtSuperEmail" ErrorMessage="Field 'Supervisor Email' is blank"></asp:RequiredFieldValidator><CC:EmailValidator Display="Dynamic" runat="server" id="fvSuperEmail" ControlToValidate="txtSuperEmail" ErrorMessage="Field 'Supervisor Email' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Supervisor Phone:</td>
		<td class="field"><asp:textbox id="txtSuperPhone" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvSuperPhone" runat="server" Display="Dynamic" ControlToValidate="txtSuperPhone" ErrorMessage="Field 'Supervisor Phone' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Subtotal:</td>
		<td class="field"><asp:textbox id="txtSubtotal" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvSubtotal" runat="server" Display="Dynamic" ControlToValidate="txtSubtotal" ErrorMessage="Field 'Subtotal' is blank"></asp:RequiredFieldValidator><CC:FloatValidator Display="Dynamic" runat="server" id="fvSubtotal" ControlToValidate="txtSubtotal" ErrorMessage="Field 'Subtotal' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Tax:</td>
		<td class="field"><asp:textbox id="txtTax" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvTax" runat="server" Display="Dynamic" ControlToValidate="txtTax" ErrorMessage="Field 'Tax' is blank"></asp:RequiredFieldValidator><CC:FloatValidator Display="Dynamic" runat="server" id="fvTax" ControlToValidate="txtTax" ErrorMessage="Field 'Tax' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Total:</td>
		<td class="field"><asp:textbox id="txtTotal" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvTotal" runat="server" Display="Dynamic" ControlToValidate="txtTotal" ErrorMessage="Field 'Total' is blank"></asp:RequiredFieldValidator><CC:FloatValidator Display="Dynamic" runat="server" id="fvTotal" ControlToValidate="txtTotal" ErrorMessage="Field 'Total' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Requested Delivery:</td>
		<td class="field"><CC:DatePicker ID="dtRequestedDelivery" runat="server"></CC:DatePicker></td>
		<td><CC:DateValidator Display="Dynamic" runat="server" id="dtvRequestedDelivery" ControlToValidate="dtRequestedDelivery" ErrorMessage="Date Field 'Requested Delivery' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Order Status I D:</td>
		<td class="field"><asp:DropDownList id="drpOrderStatusID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvOrderStatusID" runat="server" Display="Dynamic" ControlToValidate="drpOrderStatusID" ErrorMessage="Field 'Order Status I D' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Historic Order Status I D:</td>
		<td class="field"><asp:textbox id="txtHistoricOrderStatusID" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><CC:IntegerValidator Display="Dynamic" runat="server" id="fvHistoricOrderStatusID" ControlToValidate="txtHistoricOrderStatusID" ErrorMessage="Field 'Historic Order Status I D' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Delivery Instructions:</td>
		<td class="field"><asp:TextBox id="txtDeliveryInstructions" style="width: 319px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Notes:</td>
		<td class="field"><asp:TextBox id="txtNotes" style="width: 319px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">Remote I P:</td>
		<td class="field"><asp:textbox id="txtRemoteIP" runat="server" maxlength="16" columns="16" style="width: 115px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvRemoteIP" runat="server" Display="Dynamic" ControlToValidate="txtRemoteIP" ErrorMessage="Field 'Remote I P' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Creator Builder I D:</td>
		<td class="field"><asp:textbox id="txtCreatorBuilderID" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvCreatorBuilderID" runat="server" Display="Dynamic" ControlToValidate="txtCreatorBuilderID" ErrorMessage="Field 'Creator Builder I D' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvCreatorBuilderID" ControlToValidate="txtCreatorBuilderID" ErrorMessage="Field 'Creator Builder I D' is invalid" /></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Order?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

