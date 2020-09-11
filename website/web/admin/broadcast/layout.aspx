<%@ Page ValidateRequest="False" Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Mailing Message" CodeFile="layout.aspx.vb" Inherits="layout"  %>
<%@ Register TagPrefix="CC" TagName="MailingSteps" Src="~/controls/MailingSteps.ascx" %>
<%@ Register TagPrefix="CC" TagName="MailingPreview" Src="~/controls/MailingPreview.ascx" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<div id="divLayout" runat="Server">

<h4>Send E-mail: Step 1 of 4</h4>

<CC:MailingSteps CurrentStep="1" id="Steps" runat="server" />

<table border="0" cellspacing="2" cellpadding="3" style="margin-top:10px;">
<tr><td colspan="3"><span class="red">red color</span> - denotes required fields</td></tr>
<tr><th colspan="2">General</th></tr>
<tr>
	<td class="required" style="width:100px;">E-mail Name:</td>
	<td class="field" valign="top">
	<span class="smallest">This name will be displayed on the E-mail List</span><br />
	<asp:TextBox id="txtName" columns="50" maxlength="100" runat="server" />
	</td>
</tr>
<tr>
	<td class="required" style="width:100px;">E-mail Date:</td>
	<td class="field" valign="top"><CC:DatePicker ID="dtNewsletterDate" runat="server" /></td>
	<td>
	    <CC:RequiredDateValidator ID="reqDateValidator1" ControlToValidate="dtNewsletterDate" ErrorMessage="E-mail date is blank" runat="server" Display="Dynamic" />
	    <CC:DateValidator ID="DateValidator1" ControlToValidate="dtNewsletterDate" ErrorMessage="E-mail date is invalid" runat="server" Display="Dynamic" />
	</td>
</tr>
<tr><th colspan="2">E-mail settings</th></tr>
<tr>
	<td class="required">E-mail format:</td>
	<td class="field">
	    <asp:RadioButtonList ID="rblMimeType" RepeatDirection="Horizontal" runat="server" AutoPostBack="true">
	    <asp:ListItem Value="BOTH">HTML & Plain Text</asp:ListItem>
	    <asp:ListItem Value="HTML">HTML</asp:ListItem>
	    <asp:ListItem Value="TEXT">Plain Text</asp:ListItem>
	    </asp:RadioButtonList>
	</td>
</tr><tr>
	<td class="required" style="width:100px;">From E-mail:</td>
	<td class="field" valign="top">
	<asp:TextBox id="txtFromEmail" columns="50" maxlength="100" runat="server" />
	</td>
	<td><CC:EmailValidator ID="valFromEmail" ControlToValidate="txtFromEmail" ErrorMessage="From E-mail is invalid" runat="server" /></td>
</tr><tr>
	<td class="required" style="width:100px;">From Name:</td>
	<td class="field" valign="top">
	<asp:TextBox id="txtFromName" columns="50" maxlength="100" runat="server" />
	</td>
	<td><asp:RequiredFieldValidator ID="reqValidator1" ControlToValidate="txtFromName" ErrorMessage="From Name is blank" runat="server" /></td>
</tr><tr>
	<td class="required" style="width:100px;">Reply-To Email:</td>
	<td class="field" valign="top">
	<asp:TextBox id="txtReplyEmail" columns="50" maxlength="100" runat="server" />
	</td>
	<td><CC:EmailValidator ID="EmailValidator2" ControlToValidate="txtReplyEmail" ErrorMessage="Reply-To Email is invalid" runat="server" /></td>
</tr><tr>
	<td class="required" style="width:100px;">Subject:</td>
	<td class="field"><span class="smallest">The email subject the members will receive</span><br />
	<asp:TextBox id="txtSubject" columns="50" maxlength="100" runat="server" /><br />
	<span class="smallest"><span style='font-size:8.5pt;font-family:Arial;font-weight:bold'>Note:</span> In accordance with the "CAN-SPAM" Act, signed into law in 2003, it is unlawful to use a subject heading that would be likely to mislead a recipient about the subject of the email.</span>
	</td>
	<td><asp:RequiredFieldValidator ID="reqValidator2" ControlToValidate="txtSubject" ErrorMessage="Subject is blank" runat="server" /></td>
</tr><tr>
	<td class="required" style="width:100px;">Recipient type:</td>
	<td class="field"><% If TargetType = "DYNAMIC" Then%>Uploaded E-mail List<% else %>Subscribers<% end if %></td>
</tr>
<tr><td>&nbsp;</td></tr>
<tr><td colspan="2" class="field">

<asp:Repeater id="rptSlots" runat="server">
<ItemTemplate>
<table border="0" cellpadding="2" cellspacing="3">
<tr>
	<th><b>Headline:</b></th>
	<td><asp:TextBox id="txtHeadline" columns="50" runat="server" MaxLength="100" /> &nbsp;<CC:OneClickButton runat="server" cssClass="btn" ID="btnPreview" CausesValidation="False" Text="Preview" OnClick="btnPreview_Click" /></td>
	<td valign="top" rowspan="2"><img src="/assets/broadcast/templates/slots/<%# Container.Dataitem("ImageName") %>" alt="Slot" /></td>
</tr>
<tr>
	<td colspan="2" valign="top">
	<CC:CKEditor id="htmSlot" runat="server" Width="600" Height="300" />
	<asp:TextBox ID="txtSlot" runat="server" Width="600" Height="300" TextMode="MultiLine"></asp:TextBox>
	</td>
</tr>
</table>
</ItemTemplate>
</asp:Repeater>


</td>
</tr>
</table>

<p>
You can save this e-mail and continue later.<br />
If you are ready to send this email, please depress "Save & Continue &raquo;"
</p>

<CC:OneClickButton id="btnSave" Runat="server" Text="Save for Later" cssClass="btn"></CC:OneClickButton>&nbsp;
<CC:OneClickButton id="btnContinue" Runat="server" Text="Save & Continue &raquo;" cssClass="btn"></CC:OneClickButton>

</div>

<div id="divPreview" runat="Server" visible="False">

<h4><asp:Literal ID="ltlName" runat="server"></asp:Literal> - E-mail Preview</h4>

<table cellpadding="0" cellspacing="0">
<tr>
<td><div><asp:LinkButton id="lnkBack" runat="server" CssClass="L1" Text="<< Go Back" /></div></td>
</tr>
</table>

<br />
<CC:MailingPreview id="PreviewCtrl" runat="server"></CC:MailingPreview>

</div>

</asp:content>