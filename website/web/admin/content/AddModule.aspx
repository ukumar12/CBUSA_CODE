<%@ Page ValidateRequest=false Language="VB" AutoEventWireup="false" CodeFile="AddModule.aspx.vb" Inherits="AddModule" title="HTML Module" %>
<html>
<head>
<link REL=stylesheet HREF="/includes/style.css" TYPE=text/css>
<script src="/includes/functions.js.aspx" type="text/javascript"></script>
</head>
<body>
<form id=main method=post runat=server>

<div id=divEditor runat=server>
<CC:CKeditor id="FCKeditor" runat="server" Height="550px" Width="100%" />
<table width=100%>
<tr>
<td>

Add additional margings?
<asp:DropDownList ID="drpPadding" runat="server">
<asp:ListItem Text="No" Value="0"></asp:ListItem>
<asp:ListItem Text="Yes" Value="1"></asp:ListItem>
</asp:DropDownList>

<br />
<CC:OneClickButton ID=btnPreview CssClass="adminbtn" runat=server Text="Preview" />
</td>
<td align=right>
<input type=button class=adminbtn value="Cancel" onClick="window.close();"/>&nbsp;
</td>
</tr>
</table>
</div>

<div id="divPreview" visible=false  runat=server>
<% =dbContent.Content%>

<br/>
<br/>
<table width=100%>
<tr>
<td>
<CC:OneClickButton ID=btnInsertModule CssClass="adminbtn" runat=server Text="Insert Module" />
<CC:OneClickButton ID=btnMakeChanges CssClass="adminbtn" runat=server Text="Make Changes" />
</td>
<td align=right>
<input type=button class=adminbtn value="Cancel" onClick="window.close();"/>&nbsp;
</td>
</tr>
</table>
</div>

</form>
</body>
</html>
