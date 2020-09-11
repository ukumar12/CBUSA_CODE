<%@ Page Language="VB" AutoEventWireup="false" CodeFile="EncryptDecrypt.aspx.vb" Inherits="EncryptDecrypt" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:TextBox ID="txtValueToEncrypt" runat="server" Text=""></asp:TextBox>
        <asp:Button ID="btnEncrypt" runat="server" Text="ENCRYPT" />
        <asp:Label ID="lblEncryptedText" runat="server" Text="" ForeColor="DarkRed" Font-Bold="true"></asp:Label>
        <hr />
        <asp:TextBox ID="txtValueToDecrypt" runat="server" Text=""></asp:TextBox>
        <asp:Button ID="btnDecrypt" runat="server" Text="DECRYPT" />
        <asp:Label ID="lblDecryptedText" runat="server" Text="" ForeColor="DarkBlue" Font-Bold="true"></asp:Label>
    </div>
    </form>
</body>
</html>
