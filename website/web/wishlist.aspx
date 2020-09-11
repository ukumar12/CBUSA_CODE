<%@ Page Language="vb" AutoEventWireup="false" Inherits="Wishlist" CodeFile="wishlist.aspx.vb" %>
<%@ Register TagName="Wishlist" TagPrefix="CC" Src="~/controls/Wishlist.ascx" %>

<CT:masterpage runat="server" ID="CTMain">

<CC:Wishlist ID="ctrlWishlist" runat="server" />

</CT:masterpage>