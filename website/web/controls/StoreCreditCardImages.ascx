<%@ Control Language="VB" AutoEventWireup="false" CodeFile="StoreCreditCardImages.ascx.vb" Inherits="StoreCreditCardImages" %>
<asp:Repeater ID="rptImages" runat="server">
<ItemTemplate>
<img src="/images/utility/<%#Container.DataItem("ImageName")%>" alt="<%#Container.DataItem("Name")%>">&#160;
</ItemTemplate>
</asp:Repeater>