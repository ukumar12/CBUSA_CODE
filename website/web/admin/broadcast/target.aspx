<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Mailing Message" CodeFile="target.aspx.vb" Inherits="Target"  %>
<%@ Register TagPrefix="CC" TagName="MailingSteps" Src="~/controls/MailingSteps.ascx" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Send E-mail: Step 2 of 4</h4>

<CC:MailingSteps CurrentStep="2" id="Steps" runat="server" />

<div style="margin-top:20px"></div>
<b>INITIALIZE SEARCH CRITERIA</b>
<br />
<span class="smaller">If you wish to initialize search criteria with saved query, please select Member Group below and depress "Refresh" button.</span>
<table>
<tr>
<td valign="top" style="width:200px"><asp:DropDownList AutoPostBack="True" ID="F_GroupId" runat="server"></asp:DropDownList></td>
</tr>
</table>

<div style="margin-top:20px"></div>
<b style="margin-top:20px">DEFINE SEARCH CRITERIA</b><br />
<table border="0" cellspacing="1" cellpadding="2">
	<tr>
		<td class="required">List(s):</td>
		<td class="field">
		<CC:CheckBoxListEx ID="cblLists" runat="server" RepeatColumns="2"></CC:CheckBoxListEx></td>
	</tr>
	<tr>
	<td class="optional"><b>Subscription Date:</b></td>
	<td class="field">
		<table border="0" cellpadding="0" cellspacing="0">
		<tr class="field">
			<td class="field"><span class="smallest">From:</span></td><td>&nbsp;</td><td class="field"><span class="smallest">To:</span></td>
		</tr>
		<tr class="field">
			<td><CC:DatePicker ID="dtStartDate" runat="server"></CC:DatePicker></td>
			<td>&nbsp;</td>
			<td><CC:DatePicker ID="dtEndDate" runat="server"></CC:DatePicker></td>
		</tr>
		</table>
	</td>
    <td>
    <CC:DateValidator Display="Dynamic" runat="server" id="dtvStartDate" ControlToValidate="dtStartDate" ErrorMessage="Date Field 'Start Date' is invalid" />
    <CC:DateValidator Display="Dynamic" runat="server" id="dtvEndDate" ControlToValidate="dtEndDate" ErrorMessage="Date Field 'End Date' is invalid" />
	</td>
	</tr>
</table>

<h4>There is currently <span class="red"><asp:Literal ID="ltlHTMLRecipients" runat="server" /></span> "HTML" recipient(s) in database that match your search criteria.</h4>
<h4>There is currently <span class="red"><asp:Literal ID="ltlTextRecipients" runat="server" /></span> "Plain Text" recipient(s) in database that match your search criteria.</h4>

<div style="margin-top:10px">
If you have changes any of the search criteria above and would like to know how this affected the query, please depress the "Refresh" button <br />
<CC:OneClickButton ID="btnReload" Cssclass="btn" Text="Refresh" runat="server" />
</div>

<div style="margin-top:10px">
You can save this e-mail and continue later.<br />
If you are ready to send this email, please depress "Save & Continue &raquo;"
</div>

<div style="margin-top:10px">
<CC:OneClickButton id="btnSave" Runat="server" Text="Save for Later" cssClass="btn"></CC:OneClickButton>&nbsp;
<CC:OneClickButton id="btnContinue" Runat="server" Text="Save & Continue &raquo;" cssClass="btn"></CC:OneClickButton>
</div>

</asp:content>

