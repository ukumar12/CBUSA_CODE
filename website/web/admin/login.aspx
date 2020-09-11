<%@ Page Language="VB" MasterPageFile="~/controls/AdminMaster.master" AutoEventWireup="false" CodeFile="login.aspx.vb" Inherits="Login" title="Americaneagle.com Admin" %>
<asp:Content ID="Content" ContentPlaceHolderID="ph" Runat="Server">
<script language="javascript">
	if (parent.location != document.location) parent.location = document.location;
    function setLoginFocus() {
		var uname = document.getElementById('<%=Username.ClientID%>');
		var pword = document.getElementById('<%=UserPass.ClientID%>');
		trim(uname.value) == '' ? uname.focus() : pword.focus();
    }
	if (window.addEventListener) {
		window.addEventListener('load', setLoginFocus, false);
	} else if (window.attachEvent) {
		window.attachEvent('onload', setLoginFocus);
	}

</script>

<table cellspacing=5 cellpadding=5 border=0 align="center">
<tr><td>

<table border="0" align="center">
<tr>
<td colspan=3>
    <img src="/images/utility/logo_400x93.png" style="border-style:none; margin-left:-100px;" alt=""><br />

    <h3>Admin Section Login</h3>
    <asp:Label id="Msg" ForeColor="red" Font-Name="Verdana" Font-Size="10" runat="server" />
    </td>
<tr>
	<td>Username:</td>
	<td><input id="Username" type="text" runat="server"></td>
	<td><ASP:RequiredFieldValidator ControlToValidate="Username" Display="Static" ErrorMessage="Please enter username" runat="server" id="RequiredFieldValidator1" /></td>
</tr>
<tr>
	<td>Password:</td>
	<td><input id="UserPass" type="password" runat="server"></td>
	<td><ASP:RequiredFieldValidator ControlToValidate="UserPass" Display="Static" ErrorMessage="Please enter password" runat="server" id="RequiredFieldValidator2" /></td>
</tr>
<tr>
<td colspan=3>
<CC:OneClickButton CssClass="btn" text="Login" runat="server" id="btnLogin" />
</td>
</tr>
</table>

</td></tr></table>


</asp:Content>

