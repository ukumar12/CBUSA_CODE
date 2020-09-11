<%@ Page EnableViewstate="False" Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Mailing Message" CodeFile="view.aspx.vb" Inherits="View"  %>
<%@ Register TagPrefix="CC" TagName="MailingSteps" Src="~/controls/MailingSteps.ascx" %>
<%@ Register TagPrefix="CC" TagName="MailingSummary" Src="~/controls/MailingSummary.ascx" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4><asp:Literal ID="ltlName" runat="server"></asp:Literal> - Summary</h4>

<div style="margin-top:20px"></div>

<CC:MailingSummary id="MailingSummaryCtrl" Mode="View" runat="server" />

</asp:content>

