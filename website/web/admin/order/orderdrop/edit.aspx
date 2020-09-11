<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Order Drop"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If OrderDropID = 0 Then %>Add<% Else %>Edit<% End If %> Order Drop</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Order ID:</td>
		<td class="field"><asp:textbox id="txtOrderID" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvOrderID" runat="server" Display="Dynamic" ControlToValidate="txtOrderID" ErrorMessage="Field 'Order ID' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvOrderID" ControlToValidate="txtOrderID" ErrorMessage="Field 'Order ID' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Drop Name:</td>
		<td class="field"><asp:textbox id="txtDropName" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvDropName" runat="server" Display="Dynamic" ControlToValidate="txtDropName" ErrorMessage="Field 'Drop Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Requested Delivery:</td>
		<td class="field"><CC:DatePicker ID="dtRequestedDelivery" runat="server"></CC:DatePicker></td>
		<td><CC:DateValidator Display="Dynamic" runat="server" id="dtvRequestedDelivery" ControlToValidate="dtRequestedDelivery" ErrorMessage="Date Field 'Requested Delivery' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Ship To Address:</td>
		<td class="field"><asp:textbox id="txtShipToAddress" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvShipToAddress" runat="server" Display="Dynamic" ControlToValidate="txtShipToAddress" ErrorMessage="Field 'Ship To Address' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Ship To Address 2:</td>
		<td class="field"><asp:textbox id="txtShipToAddress2" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">City:</td>
		<td class="field"><asp:textbox id="txtCity" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvCity" runat="server" Display="Dynamic" ControlToValidate="txtCity" ErrorMessage="Field 'City' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">State:</td>
		<td class="field"><asp:DropDownList id="drpState" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvState" runat="server" Display="Dynamic" ControlToValidate="drpState" ErrorMessage="Field 'State' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Zip:</td>
		<td class="field"><CC:Zip id="ctrlZip" runat="server" /></td>
		<td><CC:RequiredZipValidator ID="rfvZip" runat="server" Display="Dynamic" ControlToValidate="ctrlZip" ErrorMessage="Field 'Zip' is blank"></CC:RequiredZipValidator><CC:ZipValidator Display="Dynamic" runat="server" id="fvZip" ControlToValidate="ctrlZip" ErrorMessage="Zip 'Zip' is invalid" /></td>
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
		<td class="required">Creator Builder I D:</td>
		<td class="field"><asp:textbox id="txtCreatorBuilderID" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvCreatorBuilderID" runat="server" Display="Dynamic" ControlToValidate="txtCreatorBuilderID" ErrorMessage="Field 'Creator Builder I D' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvCreatorBuilderID" ControlToValidate="txtCreatorBuilderID" ErrorMessage="Field 'Creator Builder I D' is invalid" /></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Order Drop?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
