<%@ Page Language="VB" AutoEventWireup="false" CodeFile="InsertModule.aspx.vb" Inherits="InsertModule" title="Insert Module" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN"  "http://www.w3.org/TR/html4/loose.dtd">
<html>

<head>
<link rel="stylesheet" href="/includes/style.css" type="text/css" />
<script src="/includes/functions.js.aspx" type="text/javascript"></script>
<meta http-equiv="content-type" content="text/html; charset=iso-8859-1" />
<title>Insert Module</title>
</head>

<body>
<form id="main" method="post" runat="server">

<asp:ScriptManager ID="AjaxManager" runat="server"></asp:ScriptManager>

<asp:PlaceHolder ID="ph" runat="server"></asp:PlaceHolder>

<asp:DataList id="dlModules" ItemStyle-VerticalAlign="Top" Runat="server" CellPadding="2"
	CellSpacing="2" RepeatDirection="Horizontal" GridLines="None" EnableViewState="True"> 
	<ItemTemplate>
	
    <table cellspacing="0" cellpadding="2" border="0" width="100%">
    <tr>
    <td style="border: 1px dashed #2B4487">

	<table cellspacing=0 cellpadding=0 border=0 width="100%">
    <tr>
    <td style="background: #B8CDE7; color: #2B4487; font-weight: bold; border: 1px solid #2B4487; text-align:center"><asp:Literal ID=ltlModuleName runat=server></asp:Literal></td>
    </tr></table>

    <div style="margin-top:5px;"></div>
     
    <asp:PlaceHolder ID="ph" runat="server"></asp:PlaceHolder>
    
    <p></p>
    <CC:OneClickButton id="btnAdd" CssClass="adminbtn" Text="Add This Module" runat="server"/>
    
    </td></tr></table>

	</ItemTemplate>
</asp:DataList>	
	
</form>
</body>
</html>

