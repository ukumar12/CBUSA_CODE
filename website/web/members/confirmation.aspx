<%@ Page Language="vb" AutoEventWireup="false" Inherits="Components.SitePage" %>
<CT:masterpage runat="server" id="CTMain">

<h2 class="hdng">Confirmation Page</h2>

<asp:PlaceHolder runat="server">
<%=Session("Confirmation")%>
</asp:PlaceHolder>
</CT:masterpage>
