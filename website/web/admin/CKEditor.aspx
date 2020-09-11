<%@ Page Language="vb" AutoEventWireup="false" CodeFile="CKEditor.aspx.vb" Inherits="admin_CKEditor" %>
<html>
<head>
<link rel="stylesheet" href="/cms/includes/style.css" type="text/css" />
<script src="/cms/includes/functionsC.js" type="text/javascript"></script>
<style>
body {background: white;}
</style>
</head>
<body>
<form id="main" method="post" runat="server">
<asp:ScriptManager ID="AjaxManager" runat="server"></asp:ScriptManager>

<asp:PlaceHolder runat="server" id="phEdit">
    <p><CC:CKEditor runat="server" ID="ck" Width="98%" Height="500" ShowWindowSize="false" /></p>
    <p><CC:OneClickButton runat="server" ID="btnPreview" CssClass="adminbtn" Text="Preview" /></p>
</asp:PlaceHolder>

<asp:PlaceHolder runat="server" id="phPreview">
    <div id="divCK" style="margin-bottom:10px;"><asp:Literal runat="server" id="ltl" /></div>
    <p><CC:OneClickButton runat="server" ID="btnEdit" CssClass="adminbtn" Text="Modify" />&nbsp;<input type="button" runat="server" ID="btnSave" class="adminbtn" Value="Save" /></p>
</asp:PlaceHolder>

</form>
</body>
</html>