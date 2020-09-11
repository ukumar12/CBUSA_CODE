<%@ Page Language="VB" AutoEventWireup="false" CodeFile="top.aspx.vb" Inherits="TopPage" %>
<%@ Import Namespace="System.Configuration.ConfigurationManager" %>
<html>
<head>
<link href="/includes/admin.css" rel="stylesheet" type="text/css">
<link href="/includes/menu_top.css" rel="stylesheet" type="text/css">
</head>

<body topmargin="0" leftmargin="0">

<table cellpadding="0" cellspacing="0" border="0" width="100%">
<tr>
<td bgcolor="#E2EBF7" class="login" nowrap ><img src="/images/spacer.gif" width="10" height="1" border="0"><%=AppSettings("GlobalWebsiteName")%> Content Management System</td>
<td bgcolor="#E2EBF7" align="right" width=100%>

<table cellpadding="0" cellspacing="0" border="0">
<tr>
<td align="right" bgcolor="#E2EBF7">
<table cellspacing=0 cellpadding=0>
<tr>
<td class="login"><b>ADMIN:</td>
<td><asp:Literal ID="ltlChangePassword" runat="server" /></td>
<td><asp:Literal ID="ltlSystemParameters" runat="server" /></td>
<td><asp:Literal ID="ltlLogout" runat="server" /></td>
</td>
</tr>
</table>
<img src="/images/spacer.gif" width="15" height="1"></td>
</tr>
</table>

</td>
</tr>
</table>

</body>
</html>