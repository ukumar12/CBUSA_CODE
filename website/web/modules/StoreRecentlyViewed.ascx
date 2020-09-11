<%@ Control Language="VB" AutoEventWireup="false" CodeFile="StoreRecentlyViewed.ascx.vb" Inherits="StoreRecentlyViewed" %>
<div class="largest bold">Recently Viewed:</div>
<div class="rcntvwwrpr">
<asp:Repeater runat="server" id="rptRecentlyViewed">
    <ItemTemplate>
        <a href="" runat="server" id="lnkItem" style="text-decoration:none;">
            <img src="/assets/item/related/<%#Container.DataItem("Image")%>" alt="<%# Server.HTMLEncode(Container.DataItem("ItemName")) %>" /><br />
            <%#Container.DataItem("ItemName")%><br />
            <strong id="ltlPrice" runat="server"></strong>
        </a><br />
    </ItemTemplate>
</asp:Repeater>
</div>
<div style="clear:both;"></div>
