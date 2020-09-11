<%@ Page EnableViewstate="False" Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Mailing Message" CodeFile="log.aspx.vb" Inherits="Log"  %>
<%@ Register TagPrefix="CC" TagName="MailingSteps" Src="~/controls/MailingSteps.ascx" %>
<%@ Register TagPrefix="CC" TagName="MailingSummary" Src="~/controls/MailingSummary.ascx" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4><asp:Literal ID="ltlName" runat="server"></asp:Literal> - Lyris Log</h4>

<div><a href="view.aspx?MessageId=<%=MessageId %>" class="L1"><< Go Back</a></div>

<div style="margin-top:20px"></div>

<asp:Literal ID="ltlLog" runat="server"></asp:Literal>

</asp:content>

