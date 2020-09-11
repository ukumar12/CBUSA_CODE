<%@ Page Language="VB" AutoEventWireup="false" CodeFile="default.aspx.vb" Inherits="_default" %>
<%@ Register TagName="ShareAndEnjoy" TagPrefix="CC" Src="~/controls/ShareAndEnjoy.ascx" %>

<CT:masterpage runat="server" ID="CTMain">

<asp:Panel id="pnlMain" runat="server" DefaultButton="btnLogin" style="width:400px;margin:200px auto;" cssclass="pckggraywrpr">
    <div class="pckghdgred">CBUSA Portal Login</div>
    <table cellpadding="2" cellspacing="2" border="0">
        <tr>
            <td><b>Username:</b></td>
            <td><asp:TextBox id="txtUsername" runat="server" columns="50" maxlength="50"></asp:TextBox></td>
        </tr>
        <tr>
            <td><b>Password:</b></td>
            <td><asp:TextBox id="txtPassword" runat="server" columns="50" maxlength="50" TextMode="Password"></asp:TextBox></td>
        </tr>
        <tr>
            <td></td>
            <td><asp:checkbox id="chkPersist" runat="server" text="Remember me on this computer"></asp:checkbox></td>
        </tr>
        <tr>
            <td colspan="2" align="center">
                <asp:Button id="btnLogin" runat="server" Text="Login" cssclass="btnred" />
            </td>
        </tr>
        <tr>
            <td colspan="2" align="center"><a href="/forgot.aspx">Forgot your password? Click Here</a></td>
        </tr>
    </table>
</asp:Panel>

</CT:masterpage>