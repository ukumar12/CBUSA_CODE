<%@ Page Language="VB" AutoEventWireup="false" CodeFile="forgot.aspx.vb" Inherits="forgot" %>

<CT:masterpage runat="server" id="CTMain" DefaultFocus="txtUsername" DefaultButton="btnRetrieve">

<h2 class="hdng">Forgot Your Password?</h2>

Lost your password? Please fill in your username. Note that it is necessary that username be specified exactly as when you have subscribed. 

<table cellspacing="0" cellpadding="2" style="margin-top:10px; margin-bottom:10px">
<tr >
    <td class="fieldlbl"><span id="labeltxtUsername" runat="server">Username:</span></td>
    <td><span id="bartxtUsername" class="fieldreq" runat="server">&nbsp;</span></td>
    <td><asp:textbox id="txtUsername" runat="server" maxlength="50" style="width:200px;" /></td>
    <td valign="top">
        <CC:OneClickButton id="btnRetrieve" runat="server" Text="Retrieve Password" CssClass="btn" />
    </td>
</tr>
</table>

<span class="smaller">If you are having difficulty logging in, you may also contact <a id="lnk" runat="server" href=""></a></span>
    
<CC:RequiredFieldValidatorFront ID="eqTxtUsername" runat="server" ControlToValidate="txtUsername" ErrorMessage="Username is required"/>

</CT:masterpage>