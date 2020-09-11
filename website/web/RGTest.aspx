<%@ Page Language="VB" AutoEventWireup="false" CodeFile="RGTest.aspx.vb" Inherits="RGTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:TextBox ID="txtDateFrom" runat="server" Text="01-01-2016"></asp:TextBox>
        <asp:Button ID="btnGenerateReport" runat="server" Text="RUN SPROC" />
    </div>
    </form>
</body>
</html>
