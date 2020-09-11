<%@ Control Language="VB" AutoEventWireup="false" CodeFile="HelpMessage.ascx.vb" Inherits="controls_HelpMessage" %>
<a href="#" onclick="showQW(event,'<%=Help.ClientId%>'); return false;"><img runat="server" id="HelpIcon"  border="0" alt="" /></a>
<asp:ImageButton ImageUrl="/images/admin/edit.gif" runat="server" ID="lnkEdit" Visible="false" AlternateText ="Edit Tag" />
<div id="Help" runat="server" style="display:none" />
