<%@ Page Language="vb" AutoEventWireup="false" Inherits="order" CodeFile="~/service/order.aspx.vb" %>
<%@ Import Namespace="Components" %>
<%@ Import Namespace="DataLayer" %>
<%@ Register TagName="ShoppingCart" TagPrefix="CC" Src="~/controls/StoreShoppingCart.ascx" %>
<CT:masterpage runat="server" id="CTMain">

<asp:PlaceHolder runat="server">

	<asp:Panel runat="server" DefaultButton="btnSubmit" id="pnlTrack">

	<p>Please enter your order number and billing zip code below to track your order.</p>

	<table border="0" cellspacing="3" cellpadding="0">
		<tr>
			<td></td>
			<td class="fieldreq">&nbsp;</td>
			<td class="smaller">&nbsp;Indicates required field</td>
		</tr>
		<tr>
			<td colspan="3" style="font-size:4px;">&nbsp;</td>
		</tr>
		<tr>
			<td class="fieldlbl" runat="server" id="labeltxtOrderNo">Order Number</td>
			<td class="fieldreq" runat="server" id="bartxtOrderNo">&nbsp;</td>
			<td><asp:TextBox runat="server" id="txtOrderNo" style="width:150px;" size="20" maxlength="20" /></td>
		</tr>
		<tr>
			<td class="fieldlbl" runat="server" id="labeltxtZipcode">Billing zip code</td>
			<td class="fieldreq" runat="server" id="bartxtZipcode">&nbsp;</td>
			<td><asp:TextBox runat="server" maxlength="15" size="20" id="txtZipcode" style="width:150px;" /></td>
		</tr>
		<tr>
			<td></td>
			<td></td>
			<td><CC:OneClickButton runat="server" ID="btnSubmit" Text="Submit" CssClass="btn" /></td>
		</tr>
	</table>
	</asp:Panel>
	
	<CC:RequiredFieldValidatorFront ID="RequiredFieldValidatorFront1" runat="server" ControlToValidate="txtOrderNo" Display="none" EnableClientScript="false" ErrorMessage="Field 'Order Number' is required" />
	<CC:RequiredFieldValidatorFront ID="RequiredFieldValidatorFront2" runat="server" ControlToValidate="txtZipcode" Display="none" EnableClientScript="false" ErrorMessage="Field 'Billing Zipcode' is required" />
	
	<asp:Panel runat="server" id="pnlOrder">
		<a href="/service/order.aspx">&laquo; Check Another Order</a>
		<table width="100%" border="0" cellspacing="0" cellpadding="4" class="bdr" style="margin-top:20px;margin-bottom:10px;">
		<tr><td width="90" class="bold secondarytxtc">Order#</td><td class="primarytxtc"><%=dbOrder.OrderNo%></td></tr>
		<tr><td class="bold secondarytxtc">Order Date:</td><td class="primarytxtc"><%=FormatDateTime(dbOrder.ProcessDate,1)%></td></tr>
		<tr><td class="bold secondarytxtc">Order Status:</td><td class="primarytxtc"><%=StoreOrderStatusRow.GetRowByCode(DB, dbOrder.Status).Name%></td></tr>
		</table>

		<CC:ShoppingCart runat="server" ID="sc" />
	</asp:Panel>

</asp:PlaceHolder>

</CT:masterpage>