<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" CodeFile="ViewDemographics.aspx.vb" Inherits="admin_surveys_reports_ViewDemographics" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Survey Results</h4>

<p></p>
<CC:OneClickButton ID="btnBack" Text="Back to Results" runat="server" CssClass="btn" />
<CC:OneClickButton ID="btnRefresh" Text="Refresh" runat="server" CssClass="btn" />

<p></p>
<asp:Label ID="lblQuestion" runat="server"></asp:Label>
<p></p>

<asp:Repeater ID="rptList" runat="server">

    <HeaderTemplate>
    <table cellpadding="0" cellspacing="0" border="0" rules="all">
    <thead>
        <tr><asp:Literal ID="ltlHeader" runat="server"></asp:Literal>
            </tr>
    </thead>
    <tbody>
    </HeaderTemplate>
    <ItemTemplate>
    <tr class="row" style="height:24px;">
        <asp:Literal ID="ltlBody" runat="server"></asp:Literal>
    </tr>
    </ItemTemplate>
    <AlternatingItemTemplate>
    <tr class="alternate" style="height:24px;">
        <asp:Literal ID="ltlBody" runat="server"></asp:Literal>
    </tr>
    </AlternatingItemTemplate>
    
    <FooterTemplate>
    </tbody>
    </table>
    </FooterTemplate>

</asp:Repeater>


</asp:content>
