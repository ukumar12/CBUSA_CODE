<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Mailing Message" CodeFile="send.aspx.vb" Inherits="SendSchedule"  %>
<%@ Register TagPrefix="CC" TagName="MailingSteps" Src="~/controls/MailingSteps.ascx" %>
<%@ Register TagPrefix="CC" TagName="MailingSummary" Src="~/controls/MailingSummary.ascx" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Send E-mail: Step 4 of 4</h4>

<CC:MailingSteps CurrentStep="4" id="Steps" runat="server" />

<div style="margin-top:20px"></div>

<CC:MailingSummary id="MailingSummaryCtrl" runat="server" />

<div style="margin-top:20px">
<b>Send / Schedule E-mail</b><br />
If you want to send the messages instantly, please leave the scheduled date blank.</div>

<table border="0" cellspacing="2" cellpadding="3" style="margin-top:10px">
<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
<tr>
	<td class="optional"><b>Scheduled Date:</b></td>
	<td class="field"><CC:DatePicker id="ScheduledDate" runat="server" /></td>
	<td><CC:DateValidator ID="DateValidator1" runat="server" Display="Dynamic" ControlToValidate="ScheduledDate" ErrorMessage="Invalid 'Scheduled Date'"></CC:DateValidator></td>
</tr>
<tr>
	<td class="optional"><b>Scheduled Time:</b></td>
	<td class="field"><CC:TimePicker id="ScheduledTime" runat="server" /></td>
	<td><CC:TimeValidator runat="server" ID="TimeVal1" Display="Dynamic" ControlToValidate="ScheduledTime" ErrorMessage="Invalid 'Scheduled Time'"></CC:TimeValidator></td>
</tr>
</table>

<asp:CustomValidator Display="None" ErrorMessage="Both, scheduled date and time must be empty or entered at the same time" ID="valCustom" runat="server"></asp:CustomValidator>

<div style="margin-top:10px">
<CC:OneClickButton id="btnContinue" Runat="server" Text="Send &raquo;" cssClass="btn"></CC:OneClickButton>
</div>

</asp:content>

