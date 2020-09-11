<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MailingLists.ascx.vb" Inherits="MailingLists" %>
<span class="smaller"><asp:Literal id="ltlHeader" runat="server"></asp:Literal></span>
<asp:Repeater ID="rptLists" runat="server">
<ItemTemplate>
<li><%#Container.DataItem("Name")%> (<%#Container.DataItem("Total")%>)</li>
</ItemTemplate>
</asp:Repeater>