<%@ Control Language="VB" AutoEventWireup="false" CodeFile="StoreItemEmailMe.ascx.vb" Inherits="StoreItemEmailMe" %>

<div><strong>Notify me when this item becomes available</strong></div>
<div runat="server" id="divEmail">
<table border="0" cellspacing="0" cellpadding="2">
<tr>
<td colspan="2"></td>
<td>
	<asp:Literal runat="server" ID="ltlErrorMsg" EnableViewState="false" />
</td>
</tr>
<tr valign="middle">
<td class="fieldlbl" runat="server" id="labeltxtEmail">Email:</td>
<td class="fieldreq" runat="server" id="bartxtEmail">&nbsp;</td>
<td class="field">
	<asp:TextBox runat="server" ID="txtEmail" MaxLength="50" style="width:150px;" /> 
	<asp:Button runat="server" ID="btnNotify" ValidationGroup="NotifyMe" Text="GO" CssClass="btn" />
</td>
</tr>
</table>
<CC:RequiredFieldValidatorFront runat="server" ID="rfvtxtEmail" ValidationGroup="NotifyMe" ControlToValidate="txtEmail" ErrorMessage="Email is required" />
<CC:EmailValidatorFront runat="server" ID="evtxtEmail" ValidationGroup="NotifyMe" ControlToValidate="txtEmail" ErrorMessage="Email is invalid" />
</div>
<div runat="server" id="divSubmit" visible="false">
<asp:Literal runat="server" ID="ltlMessage" />
</div>
<div runat="server" id="divRemove" visible="false">
The email address provided has already been submitted.<br />
<asp:LinkButton runat="server" Text="Click here" ID="lnkRemove" /> to remove your email address.
</div>