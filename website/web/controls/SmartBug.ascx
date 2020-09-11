<%@ Control Language="VB" EnableViewState="False" AutoEventWireup="false" CodeFile="SmartBug.ascx.vb" Inherits="SmartBug" %>
<%@ Import Namespace="Utility" %>
<%  If BugVisible Then%>
<div style="bottom:10px; right:10px; position:fixed;">
	<a href="<%=Server.HTMLEncode(URL)%>" target="_blank"><img src="/images/utility/bug.gif" width="34" height="38" style="border-style:none" alt="Smart Bug" /></a><br />
</div>
<% End If%>
