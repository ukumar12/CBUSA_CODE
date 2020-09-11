<%@ Page Language="vb" AutoEventWireup="false" Inherits="Optout" CodeFile="optout.aspx.vb" %>
<CT:masterpage runat="server" id="CTMain">

<div class="hdng">Newsletter Unsubscribe Request</div>

<div id="divMain" runat="server">
Please provide Your email address below and depress "Unsubscribe" button.
<table border="0" cellspacing="1" cellpadding="2" style="margin-top:10px;">
	<tr><td colspan="3"><span class="smallest"><span class="red">*</span> - denotes required fields</span></td></tr>
	<tr>
		<td>*</td><td class="required"><b>Email:</b></td>
		<td valign="top" class="field"><asp:Textbox id="txtEmail" Columns="50" MaxLength="255" runat="server"></asp:Textbox></td>
		<td>
		<asp:requiredfieldvalidator id="rfvEmail" Display="Dynamic" ErrorMessage="Email field is blank" runat="server" ControlToValidate="txtEmail"></asp:requiredfieldvalidator>
		<CC:EmailValidator ID="evalEmail" Display="Dynamic" ErrorMessage="Email address is invalid" runat="server" ControlToValidate="txtEmail"></CC:EmailValidator>
		</td> 
	</tr>
</table>

<div style="margin-top:10px">
<CC:OneClickButton id="btnUnsubscribe" Runat="server" Text="Unsubscribe" cssClass="btn" />
</div>
</div>

<div id="divConfirm" runat="server" visible="false">
E-mail <b><asp:Literal id="ltlEmail" runat="server"/></b> has been successfully removed from our mailing list.
</div>

</CT:masterpage>

