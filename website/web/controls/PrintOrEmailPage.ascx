<%@ Control Language="VB" EnableViewState="False" AutoEventWireup="false" CodeFile="PrintOrEmailPage.ascx.vb" Inherits="PrintOrEmailPage" %>
<%@ Import Namespace="Components"%>
<a href="<% =PrintUrl%>" target="_blank">PRINT PAGE</a> |
<a href="" onclick="NewWindow(<%=Core.Escape("/friend.aspx?URL=" & Server.UrlEncode(SendUrl))%>, 'sendtofriend', 700, 430, 'yes', 'no'); return false;">SENT TO FRIEND</a><br />
