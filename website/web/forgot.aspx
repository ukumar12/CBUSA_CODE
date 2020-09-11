<%@ Page Language="VB" AutoEventWireup="false" CodeFile="forgot.aspx.vb" Inherits="forgot" %>

<CT:MasterPage ID="CTMain" runat="server">

    <div class="pckggraywrpr" style="width:500px;margin:50px auto;padding:10px;">
        <div class="pckghdgred">Forgot Password</div>
        <table id="tblForm" runat="server" cellpadding="5" cellspacing="0" border="0" style="width:100%;background-color:#fff;">
            <tr>
                <td colspan="2" class="center bold">Enter your username in the box below, and a new password will be emailed to the address registered with your account.</td>
            </tr>
            <tr>
                <td>Username:</td>
                <td><asp:TextBox id="txtUsername" runat="server" maxlength="50" columns="50"></asp:TextBox></td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <CC:OneClickButton ID="btnSubmit" runat="server" Text="Submit" CssClass="btnred" />
                </td>
            </tr>
        </table>
        <div id="divSent" runat="server" visible="false" style="background-color:#fff;padding:15px;">
            <h1 align="center">Password Sent</h1>
            <b>Your new password has been emailed to your account's registered email address.  You can use your new password to log in to the CBUSA website where you will have the opportunity to change your password.</b>
        </div>
    </div>

</CT:MasterPage>