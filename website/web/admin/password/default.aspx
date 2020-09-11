<%@ Page Language="VB" MasterPageFile="~/controls/AdminMaster.master" AutoEventWireup="false" CodeFile="default.aspx.vb" Inherits="PasswordChange" title="Change Password" %>

<asp:Content ID="Content" ContentPlaceHolderID="ph" Runat="Server">

<table id="tblWrapper" runat="server" width="400">
<tr><td>

<h4>Change Password</h4>

<%  If Session("AdminExpiredPassword") Or Session("AdminIsNew") Then%>
<p><img src="/images/utility/logo.gif" border=0></p>
<% end if %>

<asp:Panel ID="pnlAdminIsNew" runat="server" Visible="false">
This is your first time logging in.<br />
You are required to change your password.
<p></p>
</asp:Panel>

<asp:Panel ID="pnlAdminExpiredPassword" runat="server" Visible="false">
Your password has expired.<br />
You are required to change your password.
<p></p>
</asp:Panel>

<TABLE cellSpacing="2" cellPadding="3" border="0">
    <TR><TD colSpan="2"><SPAN class="red">red color</SPAN> - required fields</TD></TR>
    <tr><th colspan="2">Password Information</td></tr>
    <TR vAlign="middle">
        <td class="required"><b>Old Password:</b></td>
        <TD class="field"><asp:TextBox id="PASSWORD_OLD" Runat="server" Columns="20" MaxLength="20" Width="200" TextMode="Password"></asp:TextBox></TD>
        <TD><asp:RequiredFieldValidator EnableClientScript="false" id="REQ_PASSWORD_OLD" Runat="server" Display="None" ControlToValidate="PASSWORD_OLD" ErrorMessage="Old Password is required"></asp:RequiredFieldValidator></TD>
    </TR>
    <TR vAlign="middle">
        <td class="required"><b>New Password:</b></td>
        <TD class="field"><asp:TextBox id="PASSWORD_NEW" Runat="server" Columns="20" MaxLength="20" Width="200" TextMode="Password"></asp:TextBox></TD>
        <TD><asp:RequiredFieldValidator EnableClientScript="false" id="REQ_PASSWORD_NEW" Runat="server" Display="None" ControlToValidate="PASSWORD_NEW" ErrorMessage="New Password is required"></asp:RequiredFieldValidator>
            <CC:PasswordValidator MinLength="7" EnableClientScript="false" id="vPassword" Runat="server" Display="None" ControlToValidate="PASSWORD_NEW" errormessage="Password must contain minimum 7 characters and must contain both numeric and alphabetic characters"></CC:PasswordValidator></TD>
    </TR>
    <TR vAlign="middle">
        <Td class="required"><b>Confirm Password:</b></Td>
        <TD class="field"><asp:TextBox id="PASSWORD_CONFIRM" Runat="server" Columns="20" MaxLength="20" Width="200" TextMode="Password"></asp:TextBox></TD>
        <TD><asp:RequiredFieldValidator EnableClientScript="false"  id="REQ_PASSWORD_CONFIRM" Runat="server" Display="None" ControlToValidate="PASSWORD_CONFIRM" ErrorMessage="Confirm Password is required"></asp:RequiredFieldValidator>
            <asp:CompareValidator EnableClientScript="false" id="CMP_PASSWORD" Runat="server" Display="None" ControlToValidate="PASSWORD_CONFIRM" ErrorMessage="New Password and the Confirmed Password don't match" ControlToCompare="PASSWORD_NEW"></asp:CompareValidator></TD>
    </TR>
    <tr><td>&nbsp;</td></tr>
    </table>
    <table runat="server" ID="pnlPasswordEx" cellSpacing="2" cellPadding="3" border="0">
   
    <tr><th colspan="2">Secondary Password Information</th></tr>
    <TR vAlign="middle">
        <td class="required"><b>Old Password:</b></td>
        <TD class="field"><asp:TextBox id="PASSWORDEX_OLD" Runat="server" Columns="20" MaxLength="20" Width="200" TextMode="Password"></asp:TextBox></TD>
        <TD><asp:RequiredFieldValidator EnableClientScript="false" id="REQ_PASSWORDEX_OLD" Runat="server" Display="None" ControlToValidate="PASSWORDEX_OLD" ErrorMessage="Old Secondary Password is required"></asp:RequiredFieldValidator></TD>
    </TR>
    <TR vAlign="middle">
        <td class="required"><b>New Password:</b></td>
        <TD class="field"><asp:TextBox id="PASSWORDEX_NEW" Runat="server" Columns="20" MaxLength="20" Width="200" TextMode="Password"></asp:TextBox></TD>
        <TD><asp:RequiredFieldValidator EnableClientScript="false" id="REQ_PASSWORDEX_NEW" Runat="server" Display="None" ControlToValidate="PASSWORDEX_NEW" ErrorMessage="New Secondary Password is required"></asp:RequiredFieldValidator>
            <CC:PasswordValidator MinLength="7" EnableClientScript="false" id="vPasswordEx" Runat="server" Display="None" ControlToValidate="PASSWORDEX_NEW" errormessage="Secondary Password must contain minimum 7 characters and must contain both numeric and alphabetic characters"></CC:PasswordValidator></TD>
    </TR>
    <TR vAlign="middle">
        <Td class="required"><b>Confirm Password:</b></Td>
        <TD class="field"><asp:TextBox id="PASSWORDEX_CONFIRM" Runat="server" Columns="20" MaxLength="20" Width="200" TextMode="Password"></asp:TextBox></TD>
        <TD><asp:RequiredFieldValidator EnableClientScript="false"  id="REQ_PASSWORDEX_CONFIRM" Runat="server" Display="None" ControlToValidate="PASSWORDEX_CONFIRM" ErrorMessage="Confirm Secondary Password is required"></asp:RequiredFieldValidator>
            <asp:CompareValidator EnableClientScript="false" id="CMP_PASSWORDEX" Runat="server" Display="None" ControlToValidate="PASSWORDEX_CONFIRM" ErrorMessage="New Secondary Password and the Confirmed Secondary Password don't match" ControlToCompare="PASSWORDEX_NEW"></asp:CompareValidator></TD>
    </TR>
    
</table>

<p></p>
<CC:OneClickButton id="submitButton" Runat="server" CssClass="btn" Text="Save"></CC:OneClickButton>
<CC:OneClickButton id="Cancel" runat="server" Text="Cancel" CausesValidation="False" cssClass="btn"></CC:OneClickButton>

</td>
</tr>
</table>

</asp:Content>

