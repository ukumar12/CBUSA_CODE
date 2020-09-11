<%@ Page Language="vb" AutoEventWireup="false" Inherits="faq" CodeFile="~/service/faq.aspx.vb" %>
<CT:masterpage runat="server" id="CTMain">

<asp:ScriptManager runat="server" id="sm" EnablePartialRendering="true" />

<a name="top"></a>
<asp:repeater id="rptTop" runat="server">
<itemtemplate>
<h2 style="margin-bottom:3px;" class="secondarytxtc"><%# DataBinder.Eval(Container.DataItem, "CategoryName")%></h2>
<asp:repeater id="rptInner" runat="server">
<itemtemplate><a href="#<%# DataBinder.Eval(Container.DataItem, "FaqId")%>"><%# DataBinder.Eval(Container.DataItem, "Question")%></a></itemtemplate>
<separatortemplate><br /></separatortemplate>
</asp:repeater>

<asp:UpdatePanel runat="server" id="upSend" visible="false">
<ContentTemplate>
	<div style="margin:10px 0;">Can't find what you're looking for in this section?<br />
	<asp:LinkButton runat="server" id="lnkAsk" CommandName="Ask" /></div>
	<asp:PlaceHolder runat="server" id="phAsk" visible="false">
	Please ask your question below.
	<table border="0" cellspacing="1" cellpadding="2">
		<tr>
		<td></td>
		<td class="fieldreq">&nbsp;</td>
		<td>indicates required field</td>
		</tr>
		<tr>
			<td runat="server" id="labeltxtEmail" class="fieldtext">Your Email</td>
			<td runat="server" id="bartxtEmail" class="fieldreq">&nbsp;</td>
			<td class="field"><asp:textbox id="txtEmail" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		</tr>
		<tr>
			<td runat="server" id="labeltxtMessage" class="fieldtext">Question</td>
			<td runat="server" id="bartxtMessage" class="fieldreq">&nbsp;</td>
			<td class="field"><asp:TextBox id="txtMessage" style="width: 319px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
		</tr>
		<tr>
			<td></td>
			<td></td>
			<td class="right"><asp:Button runat="server" id="btnSubmit" Text="Submit" CssClass="btn" CommandName="Submit" /> <asp:Button runat="server" id="btnCancel" Text="Cancel" CssClass="btn" CommandName="Cancel" /></td>
		</tr>
	</table>
	<CC:RequiredFieldValidatorFront ID="RequiredFieldValidatorFront3" runat="server" ControlToValidate="txtMessage" ErrorMessage="Field 'Message' is required" Display="None" EnableClientScript="false" />
	<CC:RequiredFieldValidatorFront ID="RequiredFieldValidatorFront5" runat="server" ControlToValidate="txtEmail" ErrorMessage="Field 'E-mail' is required" Display="None" EnableClientScript="false" />
	<CC:EmailValidatorFront ID="EmailValidatorFront1" runat="server" ControlToValidate="txtEmail" ErrorMessage="Field 'E-mail' is invalid" Display="None" EnableClientScript="false" />
	</asp:PlaceHolder>
	<asp:PlaceHolder runat="server" id="phSubmitted" visible="false">
		<div style="margin:10px 0;">Thank you!<br />We have received your question and will provide an answer shortly.</div>
	</asp:PlaceHolder>
</ContentTemplate>
</asp:UpdatePanel>

</itemtemplate>
</asp:repeater>

<asp:repeater id="rptMain" runat="server">
<itemtemplate>
<h2 style="margin-bottom:3px;" class="primarytxtc"><%#DataBinder.Eval(Container.DataItem, "CategoryName")%></h2>
<asp:repeater id="rptInner" runat="server">
<itemtemplate>
<a name="<%# DataBinder.Eval(Container.DataItem, "FaqId")%>"></a><b class="secondarytxtc">Q: <%# DataBinder.Eval(Container.DataItem, "Question")%></b><br />
A: <%# DataBinder.Eval(Container.DataItem, "Answer")%>
<p style="text-align:right;"><a class="smallest" href="#top">back to top <img src="/images/arrow.gif" border="0" /></a></p>
</itemtemplate>
<separatortemplate><br /></separatortemplate>
</asp:repeater>

<asp:UpdatePanel runat="server" id="upSend" visible="false">
<ContentTemplate>
	<div style="margin:10px 0;">Can't find what you're looking for in this section?<br />
	<asp:LinkButton runat="server" id="lnkAsk2" CommandName="Ask" /></div>
	<asp:PlaceHolder runat="server" id="phAsk2" visible="false">
	Please ask your question below.
	<table border="0" cellspacing="1" cellpadding="2">
		<tr>
		<td></td>
		<td class="fieldreq">&nbsp;</td>
		<td>indicates required field</td>
		</tr>
		<tr>
			<td runat="server" id="labeltxtEmail2" class="fieldtext">Your Email</td>
			<td runat="server" id="bartxtEmail2" class="fieldreq">&nbsp;</td>
			<td class="field"><asp:textbox id="txtEmail2" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		</tr>
		<tr>
			<td runat="server" id="labeltxtMessage2" class="fieldtext">Question</td>
			<td runat="server" id="bartxtMessage2" class="fieldreq">&nbsp;</td>
			<td class="field"><asp:TextBox id="txtMessage2" style="width: 319px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
		</tr>
		<tr>
			<td></td>
			<td></td>
			<td class="right"><asp:Button runat="server" id="btnSubmit2" Text="Submit" CssClass="btn" CommandName="Submit" /> <asp:Button runat="server" id="btnCancel2" Text="Cancel" CausesValidation="false" CssClass="btn" CommandName="Cancel" /></td>
		</tr>
	</table>
	<CC:RequiredFieldValidatorFront ID="RequiredFieldValidatorFront1" runat="server" ControlToValidate="txtMessage2" ErrorMessage="Field 'Message' is required" Display="None" EnableClientScript="false" />
	<CC:RequiredFieldValidatorFront ID="RequiredFieldValidatorFront2" runat="server" ControlToValidate="txtEmail2" ErrorMessage="Field 'E-mail' is required" Display="None" EnableClientScript="false" />
	<CC:EmailValidatorFront ID="RequiredFieldValidatorFront4" runat="server" ControlToValidate="txtEmail2" ErrorMessage="Field 'E-mail' is invalid" Display="None" EnableClientScript="false" />
	</asp:PlaceHolder>
	<asp:PlaceHolder runat="server" id="phSubmitted2" visible="false">
		<div style="margin:10px 0;">Thank you!<br />We have received your question and will provide an answer shortly.</div>
	</asp:PlaceHolder>
</ContentTemplate>
</asp:UpdatePanel>

</itemtemplate>
</asp:repeater>
</CT:masterpage>