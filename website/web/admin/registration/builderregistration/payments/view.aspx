<%@ Page Language="VB" AutoEventWireup="false" CodeFile="view.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Builder Registration Payment"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If BuilderRegistrationPaymentID = 0 Then %>Add<% Else %>Edit<% End If %> Builder Registration Payment</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Cardholder Name:</td>
		<td class="field"><asp:literal id="txtCardholderName" runat="server"></asp:literal></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">Card Type:</td>
		<td class="field"><asp:Literal ID="ltlCardType" runat="server"></asp:Literal></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">Card Number:</td>
		<td class="field"><asp:literal id="txtCardNumber" runat="server"></asp:literal></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">Expiration Date:</td>
		<td class="field"><asp:literal id="txtExpirationDate" runat="server"></asp:literal></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Reference Number:</td>
		<td class="field"><asp:literal id="txtReferenceNumber" runat="server"></asp:literal></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">Payment Status:</td>
		<td class="field"><asp:Literal ID="ltlPaymentStatus" runat="server"></asp:Literal></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">Submitted:</td>
		<td class="field"><asp:Literal ID="ltlSubmitted" runat="server"></asp:Literal></td>
		<td></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnCancel" runat="server" Text="Return" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

