<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Mailing Message" CodeFile="review.aspx.vb" Inherits="Review"  %>
<%@ Register TagPrefix="CC" TagName="MailingSteps" Src="~/controls/MailingSteps.ascx" %>
<%@ Register TagPrefix="CC" TagName="MailingSummary" Src="~/controls/MailingSummary.ascx" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Send E-mail: Step 3 of 4</h4>

<CC:MailingSteps CurrentStep="3" id="Steps" runat="server" />

<div style="margin-top:20px"></div>

<CC:MailingSummary id="MailingSummaryCtrl" runat="server" />

<div style="margin-top:20px">
<b>Send Preview E-mail</b><br />
Before you can continue to the next step, please send a preview.<br />
To make sure that the message is accurate, please provide the recipient name and E-mail address below.
</div>

<table border="0" cellspacing="2" cellpadding="3" style="margin-top:10px">
<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
<tr>
	<td class="required"><b>Member Name:</b></td>
	<td class="field"><asp:TextBox ID="txtPreviewName" runat="server" columns="50" maxlength="100" /></td>
	<td><asp:RequiredFieldValidator ID="rvPreviewName" runat="server" Display="Dynamic" ControlToValidate="txtPreviewName" ErrorMessage="Preview name is blank"></asp:RequiredFieldValidator></td>
</tr>
<tr>
	<td class="required"><b>Preview E-mail:</b></td>
	<td class="field"><asp:TextBox ID="txtPreviewEmail" runat="server" columns="50" maxlength="100" /></td>
	<td>
	    <asp:RequiredFieldValidator ID="rvPreviewEmail" runat="server" Display="Dynamic" ControlToValidate="txtPreviewEmail" ErrorMessage="Preview E-mail is blank"></asp:RequiredFieldValidator>
        <CC:EmailValidator id="emailvalidatoremail" runat="server" ErrorMessage="E-mail is not valid" ControlToValidate="txtPreviewEmail" Display="Dynamic"></CC:EmailValidator> 
	</td>
</tr>
</table>

<div style="margin-top:10px">
<CC:OneClickButton id="btnContinue" Runat="server" Text="Send Preview & Continue &raquo;" cssClass="btn"></CC:OneClickButton>
</div>

</asp:content>

