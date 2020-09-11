<%@ Page Language="VB" AutoEventWireup="false" CodeFile="default.aspx.vb" Inherits="index" %>
<%@ Import Namespace="DataLayer" %>
<%@ Import Namespace="System.Configuration.ConfigurationManager" %>
<html>
	<head>
		<title><%=AppSettings("GlobalWebsiteName")%> Admin Section</title>
    
	</head>
    <frameset rows="20 px,*" cols="*" frameborder="no" border="0" framespacing="0">
    <frame src="top.aspx" name="top_menu" scrolling="no" target="">
    <frameset cols="170,*" border="0" frameborder="0" framespacing="0">
    <frame src="menu.aspx" name="menu" noresize scrolling="yes">
    <frame src="<%=FrameURL%>" name="main" noresize>
    </frameset>
    </frameset>
</html>
