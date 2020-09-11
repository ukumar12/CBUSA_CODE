<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ResendEmailFromLog.aspx.vb" Inherits="ResendEmailFromLog" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:TextBox ID="txtEmailLogID" runat="server"></asp:TextBox>
        <asp:Button ID="btnResend" runat="server" Text="RE-SEND" />
    </div>
    </form>
</body>
</html>
