<%@ Page Language="VB" AutoEventWireup="false" CodeFile="payment.aspx.vb" Inherits="store_payment" %>
<%@ Register TagName="StoreCheckoutSteps" TagPrefix="CC" Src="~/controls/StoreCheckoutSteps.ascx" %>
<%@ Register TagName="StoreShoppingCart" TagPrefix="CC" Src="~/controls/StoreShoppingCart.ascx" %>
<%@ Register TagName="StoreCreditCardImages" TagPrefix="CC" Src="~/controls/StoreCreditCardImages.ascx" %>
<%@ Register TagName="Discovery" TagPrefix="CC" Src="~/controls/Discovery.ascx" %>
<%@ Import Namespace="Components" %>
<%@ Import Namespace="System.Configuration.ConfigurationManager" %>

<CT:masterpage runat="server" id="CTMain">

<div class="stepswrpr"><CC:StoreCheckoutSteps ID="Steps" CurrentStep="Payment" runat="server" /></div>

<CC:StoreShoppingCart ID="ctrlCart" runat="server" />

<asp:Placeholder id="ph" runat="server">
<table border="0" cellpadding="0" cellspacing="0" style="margin-top:10px;" width="100%">
<tr valign="top">
	<td width="50%" class="bdrtop bdrleft bdrright bdrbottom">
		<table border="0" cellpadding="3" cellspacing="0" width="100%">
		<tr valign="top">
			<td class="loginheader">Address Summary</td>
		</tr>
		<tr valign="top">
			<td class="logintext" style="padding:10px;">
				<p class="bold" style="font-size:15px;">Billing Address</p>

	            <div><asp:Literal ID="ltlFullName" runat="server" EnableViewState="False" /></div>
	            <div id="divCompany" runat="server"><%=Core.HTMLEncode(dbOrder.BillingCompany)%></div>
	            <div><%=Core.HTMLEncode(dbOrder.BillingAddress1)%></div>
	            <div id="divAddress2" runat="server"><%=Core.HTMLEncode(dbOrder.BillingAddress2)%></div>
	            <div><%=Core.HTMLEncode(dbOrder.BillingCity)%>, <%=Core.HTMLEncode(dbOrder.BillingState)%>&nbsp;<%=Core.HTMLEncode(dbOrder.BillingZip)%></div>
	            <div id="divRegion" runat="server"><%=Core.HTMLEncode(dbOrder.BillingRegion)%></div>
	            <div><asp:Literal ID="ltlCountry" runat="server" EnableViewState="False" /></div>
	            <div>Phone: <%=Core.HTMLEncode(dbOrder.BillingPhone)%></div>

				<div style="text-align:right;padding-right:7px;"><input type="button" class="btn" value="Edit Billing" onclick="document.location.href='billing.aspx'" /></div>
			</td>
		</tr>
		</table>
	</td>
	<td width="7"><div style="width:7px;height:7px;"></div></td>
	<td width="50%" class="bdrtop bdrleft bdrright bdrbottom">
		<table border="0" cellpadding="0" cellspacing="0" width="100%">
		<tr valign="top">
			<td class="loginheader">Payment Information</td>
		</tr>
		<tr valign="top">
			<td style="padding:10px;">

   				<table border="0" cellpadding="0" cellspacing="3">
				<tr>
				<td>
			        <table cellspacing="0" cellpadding="0" border="0">
			        <tr>
			        <td class="fieldreq">&nbsp;</td>
			        <td class="smaller">&nbsp;Indicates required field</td>
			        </tr>
			        </table>
				</td>
				</tr>
				<tr>
					<td class="fieldtext"></td>
					<td width="4" style="padding-top:3px;padding-bottom:3px;"></td>
					<td><CC:StoreCreditCardImages id="ctrlStoreCreditCardImages" runat="server" /></td>
				</tr>
				<tr>
				<td class="fieldtext"><label for="cardnum" id="labeltxtCardholderName" runat="server">Cardholder Name</label></td>
					<td width="4" style="padding-top:3px;padding-bottom:3px;"><div class="fieldreq" style="height:22px;" id="bartxtCardholderName" runat="server">&nbsp;</div></td>
					<td><asp:textbox id="txtCardholderName" runat="server" CssClass="logininput" style="width:200px;" columns="10" maxlength="100" /></td>
				</tr>
				<tr>
				<td class="fieldtext"><label for="cardnum" id="labeltxtCardNumber" runat="server">Card Number</label></td>
					<td width="4" style="padding-top:3px;padding-bottom:3px;"><div class="fieldreq" style="height:22px;" id="bartxtCardNumber" runat="server">&nbsp;</div></td>
					<td><asp:textbox id="txtCardNumber" runat="server" autocomplete="off" CssClass="logininput" style="width:200px;" columns="10" maxlength="20" /></td>
				</tr>
				<tr>
					<td class="fieldtext"><label for="exp" id="labelctrlExpDate" runat="server">Expiration</label></td>
					<td width="4" style="padding-top:3px;padding-bottom:3px;"><div class="fieldreq" style="height:22px;" id="barctrlExpDate" runat="server">&nbsp;</div></td>
					<td><CC:ExpDate id="ctrlExpDate" runat="server" /></td>
				</tr>
				<tr>
					<td class="fieldtext"><label for="code" id="labeltxtCID" runat="server">Security Code</label></td>
					<td width="4" style="padding-top:3px;padding-bottom:3px;"><div class="fieldreq" style="height:22px;" id="bartxtCID" runat="server">&nbsp;</div></td>
					<td class="smaller"><asp:textbox id="txtCID" autocomplete="off" runat="server" style="width:45px;" columns="10" maxlength="4" /><span class="smaller" style="padding-left:10px;"><a href="#" onClick="NewWindow('<%=AppSettings("GlobalRefererName")%>/store/security-info.aspx', 'SecurityInfo', '600', '400', 'no'); return false;">what's this?</a></span></td>
				</tr>
				<tr>
					<td class="fieldtext"><label for="cctype" id="labeldrpCardType" runat="server">Card Type</label></td>
					<td width="4" style="padding-top:3px;padding-bottom:3px;"><div class="fieldreq" style="height:22px;" id="bardrpCardType" runat="server">&nbsp;</div></td>
					<td><asp:dropdownlist id="drpCardType" runat="server" style="width:175px;height:20px;" /></td>
				<tr>
					<td class="fieldtext"><label for="howheard" id="labeldrpDiscovery" runat="server">How did you hear about us?</label></td>
					<td width="4" style="padding-top:3px;padding-bottom:3px;"><div class="fieldnorm" style="height:22px;" id="bardrpDiscovery" runat="server">&nbsp;</div></td>
					<td><CC:Discovery ID="drpDiscovery" runat="server" style="width:175px;"/></td>
				</tr>
				</table>
				
				<div class="fieldtext" style="margin-top:10px;"><label for="instructions">Special Instructions (this may delay your order)</label></div>
				<asp:textbox id="txtComments" runat="server" columns="25" rows="5" style="width:312px;height:85px;" TextMode="Multiline" /><br />

	            <img src="/images/utility/verisignseal.gif" style="width:100px; height:72px;" alt="VeriSign Secured" /><br />
			</td>
		</tr>
		</table>
	</td>
</tr>
</table>

<div class="right" style="padding:10px;">
    <strong>Please make sure you have fully reviewed your order. Clicking Place Order will finalize your transaction.</strong><br /><br />
    <CC:OneClickButton CssClass="btncheckout" id="btnProcess" runat="server" text="Place Order &raquo;" />
</div>

</asp:Placeholder>

<CC:CreditCardValidatorFront ID="cvtxtCardNumber" runat="server" ControlToValidate="txtCardNumber" EnableClientScript="False" Display="None" ErrorMessage="The credit card number you provided is not valid" />
<CC:CreditCardTypeValidatorFront ID="cvdrpCardType" runat="server" ControlToValidate="drpCardType" CreditCardNumberControl="txtCardNumber" EnableClientScript="False" Display="None" ErrorMessage="The credit card type doesn't match the credit card number" />
<CC:RequiredFieldValidatorFront ID="rqtxtCardholderName" runat="server" ControlToValidate="txtCardholderName" EnableClientScript="False" Display="None" ErrorMessage="You must enter your credit cardholder name" />
<CC:RequiredFieldValidatorFront ID="rqtxtCardNumber" runat="server" ControlToValidate="txtCardNumber" EnableClientScript="False" Display="None" ErrorMessage="You must enter your credit card number" />
<CC:RequiredFieldValidatorFront ID="rqtxtCID" runat="server" ControlToValidate="txtCID" EnableClientScript="False" Display="None" ErrorMessage="You must enter your card security code" />
<CC:RequiredExpDateValidatorFront ID="rqctrlExpDate" runat="server" ControlToValidate="ctrlExpDate" EnableClientScript="False" Display="None" ErrorMessage="You must provide expiration date" />
<CC:ExpDateValidatorFront ID="valctrlExpDate" runat="server" ControlToValidate="ctrlExpDate" EnableClientScript="False" Display="None" ErrorMessage="You must provide a valid expiration date" />
<CC:RequiredFieldValidatorFront ID="rqdrpCardType" runat="server" ControlToValidate="drpCardType" EnableClientScript="False" Display="None" ErrorMessage="You must enter your credit card type" />

</CT:masterpage>