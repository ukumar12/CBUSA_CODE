<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Contact.ascx.vb" Inherits="Contact" %>
<%@ Register TagName="Discovery" TagPrefix="CC" Src="~/controls/Discovery.ascx" %>

<asp:Panel runat="server" ID="pnlMain">

<script language="javascript">
<!--
function newsletterChange() {
	var ctrl = document.getElementById('<%=drpNewsletter.ClientID%>');
	if (ctrl.value == 'Yes') {
		document.getElementById('tr1').style.display = 'block';
		document.getElementById('tr2').style.display = 'block';
	} else {
		document.getElementById('tr1').style.display = 'none';
		document.getElementById('tr2').style.display = 'none';
	}
}

if (window.addEventListener) {
	window.addEventListener('load', newsletterChange, false);
} else if (window.attachEvent) {
	window.attachEvent('onload', newsletterChange);
}

//-->
</script>

<table border="0" cellspacing="3" cellpadding="0" style="margin:5px;">
<tr>
<td></td>
<td class="fieldreq">&nbsp;</td>
<td>indicates required field</td>
</tr>
<tr>
<td colspan="3"><img src="/images/spacer.gif" height="5" width="1" /></td>
</tr>
<tr>
<td runat="server" id="labeltxtFullName" class="fieldtext">Full Name</td>
<td runat="server" id="bartxtFullName" class="fieldreq">&nbsp;</td>
<td><asp:TextBox runat="server" ID="txtFullName" MaxLength="50" style="width:240px;" /></td>
</tr>
<tr>
<td runat="server" id="labeltxtEmail" class="fieldtext">E-mail</td>
<td runat="server" id="bartxtEmail" class="fieldreq">&nbsp;</td>
<td><asp:TextBox runat="server" ID="txtEmail" MaxLength="100" style="width:240px;" /></td>
</tr>
<tr runat="server" id="trOrderNo">
<td>Order #</td>
<td>&nbsp;</td>
<td><asp:TextBox runat="server" ID="txtOrderNumber" MaxLength="12" style="width:100px;" /></td>
</tr>
<tr>
<td>Phone</td>
<td>&nbsp;</td>
<td><asp:TextBox runat="server" ID="txtPhone" MaxLength="50" style="width:240px;" /></td>
</tr>
<tr>
	<td class="fieldtext"><label for="howheard" id="labeldrpDiscovery" runat="server">How did you hear about us?</label></td>
	<td width="4" style="padding-top:3px;padding-bottom:3px;"><div class="fieldnorm" style="height:22px;" id="bardrpDiscovery" runat="server">&nbsp;</div></td>
	<td><CC:Discovery ID="drpDiscovery" runat="server" style="width:175px;"/></td>
</tr>
<tr runat="server" id="trQuestion">
<td runat="server" id="labeldrpQuestionId" class="fieldtext">Question</td>
<td runat="server" id="bardrpQuestionId" class="fieldreq">&nbsp;</td>
<td><asp:DropDownList runat="server" ID="drpQuestionId" /></td>
</tr>
<tr>
<td runat="server" id="labeltxtYourMessage" class="fieldtext">Message</td>
<td runat="server" id="bartxtYourMessage" class="fieldreq">&nbsp;</td>
<td><asp:TextBox runat="server" TextMode="multiLine" Rows="5" ID="txtYourMessage" MaxLength="50" style="width:240px;" /></td>
</tr>
<tr>
<td runat="server" id="Td1" class="fieldtext">Would you like to sign up<br />for our newsletter?</td>
<td runat="server" id="Td2" class="fieldreq">&nbsp;</td>
<td><asp:DropDownList runat="server" ID="drpNewsletter" onchange="newsletterChange(this);"><asp:ListItem Value="No" Text="No" /><asp:ListItem Value="Yes" Text="Yes" /></asp:DropDownList></td>
</tr>
<tr id="tr1">
    <td class="field" align="left" id="labelrblMimeType" runat="server">E-mail format</td>
    <td></td>
    <td class="field"><asp:RadioButtonList id="rblMimeType" runat="server" RepeatDirection="Horizontal">
        <asp:ListItem Value="HTML" Selected="True">HTML</asp:ListItem>
        <asp:ListItem Value="TEXT">Plain Text</asp:ListItem>
    </asp:RadioButtonList></td>
    <CC:RequiredFieldValidatorFront ID="rfvMimeType" runat="server" Display="Dynamic" ControlToValidate="rblMimeType" ErrorMessage="E-mail format is blank"></CC:RequiredFieldValidatorFront>
</tr>
<tr valign="top" id="tr2">
    <td class="field">Which e-mail lists would<br />you like to receive?</td>
    <td></td>
    <td class="field"><CC:CheckBoxListEx ID="cblLists" runat="server" RepeatColumns="2"></CC:CheckBoxListEx></td>
</tr>
<tr>
<td colspan="2"></td>
<td><CC:OneClickButton runat="server" ID="lnkSubmit" CssClass="btn" Text="Submit" /></td>
</tr>
</table>

<CC:RequiredFieldValidatorFront ID="RequiredFieldValidatorFront1" runat="server" ControlToValidate="txtFullName" ErrorMessage="Field 'Full Name' is required" Display="None" EnableClientScript="false" />
<CC:RequiredFieldValidatorFront ID="RequiredFieldValidatorFront2" runat="server" ControlToValidate="txtEmail" ErrorMessage="Field 'E-mail' is required" Display="None" EnableClientScript="false" />
<CC:EmailValidatorFront ID="RequiredFieldValidatorFront4" runat="server" ControlToValidate="txtEmail" ErrorMessage="Field 'E-mail' is invalid" Display="None" EnableClientScript="false" />
<CC:RequiredFieldValidatorFront runat="server" ID="rqdrpQuestionId" ControlToValidate="drpQuestionId" ErrorMessage="Field 'Question' is required" Display="None" EnableClientScript="false" />
<CC:RequiredFieldValidatorFront ID="RequiredFieldValidatorFront3" runat="server" ControlToValidate="txtYourMessage" ErrorMessage="Field 'Your Message' is required" Display="None" EnableClientScript="false" />
</asp:Panel>

<asp:Panel runat="server" ID="pnlThanks">
<div style="margin:25px;">Thank you for your submission. <%=DataLayer.SysParam.GetValue(DB, "SiteName")%> will review it shortly.</div>
</asp:Panel>