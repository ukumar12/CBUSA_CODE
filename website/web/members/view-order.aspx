<%@ Page Language="vb" AutoEventWireup="false" Inherits="members_view_order" CodeFile="view-order.aspx.vb" %>
<CT:masterpage runat="server" id="CTMain" DefaultButton="btnLogin">

<h2 class="hdng">My Account Login</h2>


<table cellspacing="0" cellpadding="0" border="0" style="width:680px; margin:15px auto;" summary="account selection">
<tr>
<td class="bdr vtop" style="width:310px;">
	<h2 class="hdngbox">Yes, I have an account</h2>
	<div class="cblock10">

      <div class="smaller" style="line-height:16px; padding:6px 10px 20px 10px;">
	      If you've shopped with us before, please enter your e-mail address and password.
     </div>

        <table cellspacing="0" cellpadding="0" border="0" style="margin:6px 0 10px 10px;" summary="login">
        <tr valign="top">
        <td>&nbsp;</td>
        <td style="text-align:right; padding-bottom:15px;">
	        <table cellspacing="0" cellpadding="0" border="0">
	        <tr>
	        <td class="fieldreq">&nbsp;</td>
	        <td class="smaller field">indicates required field</td>
	        </tr>
	        </table>
        </td>
        </tr>

        <tr valign="top">
        <td class="fieldlbl"><span id="labeltxtUsername" runat="server">Username:</span></td>
        <td style="padding-bottom:10px;">
	        <table cellspacing="0" cellpadding="0" border="0">
	        <tr>
	        <td id="bartxtUsername" class="fieldreq" runat="server">&nbsp;</td>
	        <td class="field"><asp:textbox id="txtUsername" style="width:200px;" runat="server" maxlength="100" /></td>
	        </tr>
	        </table>
        </td>
        </tr>

        <tr valign="top">
        <td class="fieldlbl"><span id="labeltxtPassword" runat="server">Password:</span></td>
        <td class="fieldpad">
	        <table cellspacing="0" cellpadding="0" border="0">
	        <tr>
	        <td id="bartxtPassword" class="fieldreq" runat="server">&nbsp;</td>
	        <td class="field"><asp:textbox id="txtPassword" TextMode="Password" style="width:200px;" runat="server" maxlength="20" /></td>
	        </tr>
	        </table>
        </td>
        </tr>

        <tr valign="top">
        <td>&nbsp;</td>
        <td id="Td1" runat="server">
          <asp:checkbox id="chkPersist" runat="server" /><label for="<%=chkPersist.clientId %>">Save my login information</label>
        </td>
        </tr>

        <tr valign="top">
        <td>&nbsp;</td>
        <td style="text-align:right; padding:15px 0 10px 0;">
            <CC:OneClickButton id="btnLogin" runat="server" Cssclass="btn" Text="Login" ValidationGroup="LoginCheckout" />
        </td>
        </tr>

        <tr valign="top">
        <td>&nbsp;</td>
        <td style="text-align:right; padding:15px 0 10px 0;">
        <a href="/members/forgot.aspx">forgot your password?</a>
        </td>
        </tr>
        </table>

	</div>
</td>
<td style="width:60px;">
    <img alt="Or" src="/images/global/lbl-or.gif" style="width:60px; height:60px;" /><br />
</td>
<td class="bdr vtop" style="width:310px;">
	<h2 class="hdngbox">No, I do not have an account</h2>
	<div class="cblock10">

        <div class="smaller" style="padding-left: 5px; padding-right: 5px; padding-top: 5px;">
            Please enter your Order # and the billing zip code you used with your order.<br />

        <table cellspacing="0" cellpadding="0" border="0" style="margin:6px 0 10px 10px;" summary="login">
        <tr valign="top">
        <td>&nbsp;</td>
        <td style="text-align:right; padding-bottom:15px;">
	        <table cellspacing="0" cellpadding="0" border="0">
	        <tr>
	        <td class="fieldreq">&nbsp;</td>
	        <td class="smaller field">indicates required field</td>
	        </tr>
	        </table>
        </td>
        </tr>

        <tr valign="top">
        <td class="fieldlbl"><span id="labeltxtOrderNo" runat="server">Order #:</span></td>
        <td style="padding-bottom:10px;">
	        <table cellspacing="0" cellpadding="0" border="0">
	        <tr>
	        <td id="bartxtOrderNo" class="fieldreq" runat="server">&nbsp;</td>
	        <td class="field"><asp:textbox id="txtOrderNo" style="width:150px;" runat="server" maxlength="100" /></td>
	        </tr>
	        </table>
        </td>
        </tr>

        <tr valign="top">
        <td class="fieldlbl"><span id="labeltxtBillingZip" runat="server">Billing Zip:</span></td>
        <td class="fieldpad">
	        <table cellspacing="0" cellpadding="0" border="0">
	        <tr>
	        <td id="bartxtBillingZip" class="fieldreq" runat="server">&nbsp;</td>
	        <td class="field"><asp:textbox id="txtBillingZip" style="width:150px;" runat="server" maxlength="20" /></td>
	        </tr>
	        </table>
        </td>
        </tr>
        </table>

        </div>
        <div class="center">
            <CC:OneClickButton ID="btnViewOrder" Text="View Order" CssClass="btn" runat="server" ValidationGroup="ViewOrder" />
        </div>
</td>
</tr>
</table>

<CC:RequiredFieldValidatorFront ID="rqtxtUsername" runat="server" ValidationGroup="LoginCheckout" Display="None" EnableClientScript="False" ControlToValidate="txtUsername" ErrorMessage="You must provide your username" />
<CC:RequiredFieldValidatorFront ID="rqtxtPassword" runat="server" ValidationGroup="LoginCheckout" Display="None" EnableClientScript="False" ControlToValidate="txtPassword" ErrorMessage="You must provide your password" />              
<CC:RequiredFieldValidatorFront ID="rqtxtOrderNo" runat="server" ValidationGroup="ViewOrder" Display="None" EnableClientScript="False" ControlToValidate="txtOrderNo" ErrorMessage="You must provide your order #." />
<CC:RequiredFieldValidatorFront ID="rqtxtBillingZip" runat="server" ValidationGroup="ViewOrder" Display="None" EnableClientScript="False" ControlToValidate="txtBillingZip" ErrorMessage="You must provide your billing zip." />              

</CT:masterpage>