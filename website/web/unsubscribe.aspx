<%@ Page Language="vb" AutoEventWireup="false" Inherits="Unsubscribe" CodeFile="unsubscribe.aspx.vb" %>
<CT:masterpage runat="server" id="CTMain" DefaultButton="btnUpdate">

<div class="cblock">

<div id="divMain" runat="server">

<table cellspacing="0" cellpadding="0" border="0" style="margin-top:10px;">
<tr>
<td class="fieldreq" style="padding-bottom:6px;">&nbsp;</td>
<td class="smaller" style="padding:0 0 6px 4px;">Indicates required field</td>
</tr>
</table>

<p>
You are currently subscribed to following Lists:
</p>
<table border="0" cellspacing="2" cellpadding="0" style="margin-top:10px;">
	<tr>
		<td class="field"><b>Lists</b></td>
	    <td></td>
		<td valign="top"><CC:CheckBoxListEx ID="cblLists" runat="server" RepeatColumns="2"></CC:CheckBoxListEx></td>
		<td></td>
	</tr>
	<tr>
	    <td class="field" align="left" id="labeltxtEmail" runat="server"><b>Email</b></td>
		<td class="fieldreq" id="bartxtEmail" runat="server">&nbsp;</td>
		<td class="field"><asp:Textbox id="txtEmail" Columns="50" MaxLength="255" runat="server"></asp:Textbox></td>
		<td>
		<CC:requiredfieldvalidatorFront id="rfvEmail" Display="Dynamic" ErrorMessage="Email field is blank" runat="server" ControlToValidate="txtEmail"></CC:requiredfieldvalidatorFront>
		<CC:EmailValidatorFront ID="evalEmail" Display="Dynamic" ErrorMessage="Email address is invalid" runat="server" ControlToValidate="txtEmail"></CC:EmailValidatorFront>
		</td> 
	</tr>
	<tr>
	    <td class="field" align="left" id="labeltxtName" runat="server"><b>Name</b></td>
		<td class="fieldreq" id="bartxtName" runat="server">&nbsp;</td>
		<td valign="top" class="field"><asp:Textbox id="txtName" Columns="50" MaxLength="255" runat="server"></asp:Textbox></td>
		<CC:RequiredFieldValidatorFront id="rfvName" Display="Dynamic" ErrorMessage="Name field is blank" runat="server" ControlToValidate="txtName"></CC:RequiredFieldValidatorFront>
	</tr>
	<tr>
	    <td class="field" align="left" id="labelrblMimeType" runat="server"><b>E-mail format</b></td>
		<td></td>
		<td class="field"><asp:RadioButtonList ID="rblMimeType" runat="server" RepeatDirection="Horizontal">
            <asp:ListItem Value="HTML" Selected="True">HTML</asp:ListItem>
            <asp:ListItem Value="TEXT">Plain Text</asp:ListItem>
        </asp:RadioButtonList></td>
		<CC:RequiredFieldValidatorFront ID="rfvMimeType" runat="server" Display="Dynamic" ControlToValidate="rblMimeType" ErrorMessage="E-mail format is blank"></CC:RequiredFieldValidatorFront>
	</tr>
</table>

<div style="margin-top:10px">

<p>Please change your selection and depress "Update Subscription" button below.</p>
<CC:OneClickButton id="btnUpdate" Runat="server" Text="Update Subsciption" CssClass="btn"/>
</div>
</div>

<div id="divConfirm" runat="server" visible="false">
<p>Subscription for E-mail <b><asp:Literal runat="Server" id="ltlEmail"/></b> has been successfully updated.
</div>

</div>

</CT:masterpage>

