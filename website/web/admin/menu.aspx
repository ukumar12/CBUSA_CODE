<%@ Page Language="VB" AutoEventWireup="false" CodeFile="menu.aspx.vb" Inherits="MenuPage" %>
<HTML>
	<HEAD>
		<title>Welcome to Admin Section</title>
		<LINK href="/includes/admin.css" type="text/css" rel="stylesheet">
		<link href="/includes/menu.css.aspx" rel="stylesheet" type="text/css">
		<script src="/includes/menu.js"></script>
		<style>
		body,.body,.code{color:black;background:#B8CDE7;}
		</style>	
	</HEAD>
	<body>
	    <img src="/images/utility/logo.gif?key=ce" border="0"><br />

        <p></p>
		<div class=menutext>Welcome<br />
		<b><asp:label id="FullName" runat="server"></asp:label></b><br />
		</div>

		<form id="main" runat="server">
		<asp:Label ID=lblMenu Runat=server></asp:Label>
		</form>
		
        <p></p>
        <form method=post action="smartbug.aspx" target=main>
        <input type="submit" value="Edit Smart Bug" style="width:134px;" class="btn">
        </form>
	</body>
</HTML>
