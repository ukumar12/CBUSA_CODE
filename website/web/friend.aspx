<%@ Page Language="VB" AutoEventWireup="false" CodeFile="friend.aspx.vb" Inherits="SendToFriend" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html lang="en">
<head>
<title>Email a Web Page from <%=SiteName%></title>
<meta name="robots" content="noindex, nofollow" />
<meta http-equiv="content-type" content="text/html; charset=iso-8859-1" />
<link rel="stylesheet" href="/includes/style.css" type="text/css" />
<script type="text/javascript"  src="/includes/functions.js.aspx"></script>
</head>

<body>
<!-- start -->

	<table style="width:640px" cellpadding="0" cellspacing="0" border="0">
	<tr valign="top">
	<td style="width:628px;">

		<div style="margin:10px 2px 0 8px">

<!-- start -->

<script type="text/javascript">
<!--
	function ShowMore(){
		var divFriend2 = document.getElementById('divFriend2').style;
		var divFriend3 = document.getElementById('divFriend3').style;
		var divAddMore = document.getElementById('divAddMore').style;

		if (divFriend2.display == 'none'){
			divFriend2.display = 'block';
		}
		if (divFriend3.display == 'none'){
			divFriend3.display = 'block';
			divAddMore.display = 'none';
		}
	}
//-->
</script>

<form method="post" id="main" runat="server">

<div style="width:620px;"><CT:ErrorMessage id="ErrorPlaceHolder" runat="server"/></div>

<asp:Panel ID="pnlResult" runat="Server">
<%=Session("Result") %>

<p></p>
<input type="button" class="btn" onclick="window.close();" value="Close Window"><br />

</asp:Panel>

<asp:Panel ID="pnlFields" runat="Server">

<table cellspacing="0" cellpadding="0" border="0">
<tr>
<td class="fieldreq" style="padding-bottom:6px;">&nbsp;</td>
<td class="smaller" style="padding:0 0 6px 4px;">Indicates required field</td>
</tr>
</table>

<table width="100%" border="0">
<tr valign="top">
<td>
<table width="50%" border="0" cellspacing="2" cellpadding="0">
	<tr valign=top>
		<td colspan="3">
		<table cellspacing="0" cellpadding="0" border="0">
		<tr valign="top">
		<td class="bold largest">1.&nbsp;</td>
		<td>Tell us who you are. We will use this information to
		    identify you to your friend. We will not add you to any
		    mailing lists.
		</td>
		</tr>
		</table>
		</td>
	</tr>
	<tr>
		<td class="field" align="left" id="labelYourName" runat="server"><nobr><b>Your Name:</b></nobr></td>
		<td class="fieldreq" id="barYourName" runat="server">&nbsp;</td>
		<td><asp:TextBox id="YourName" columns="18" maxlength="50" style="width: 200px;" runat="server"></asp:TextBox><br /></td>
		<CC:RequiredFieldValidatorFront ControlToValidate="YourName" id="rfvYourName" runat="server" ErrorMessage="Your Name is blank"></cc:RequiredFieldValidatorFront>
	</tr>
	<tr>
		<td class="field" align="left" id="labelYourEmail" runat="server"><b><nobr>Your E-mail:</nobr></b></td>
	    <td class="fieldreq" id="barYourEmail" runat="server">&nbsp;</td>
		<td><asp:TextBox ID="YourEmail" columns="18" maxlength="50" style="width: 200px;" runat="server"></asp:TextBox><br /></td>
		<CC:RequiredFieldValidatorFront ControlToValidate="YourEmail" id="RequiredFieldValidatorFront1" runat="server" ErrorMessage="Your E-mail is blank"></cc:RequiredFieldValidatorFront>
		<CC:EmailValidatorFront ID="EmailValidatorFront1" ControlToValidate="YourEmail" runat="server" ErrorMessage="E-mail is invalid"></CC:EmailValidatorFront>
	</tr>
	<tr>
		<td colspan="3">
		<table cellspacing="0" cellpadding="0" border="0">
		<tr valign="top">
		<td class="bold largest">2.&nbsp;</td>
		<td>Tell us about your friend. We will use this information to
		    send an e-mail to your friend. We will not add your friend to any
		    mailing lists.
		</td>
		</tr>
		</table>
		</td>
	</tr>

	<tr>
	<td colspan="3" align="right">
	    <div id="divFriend1" style="display:block">
	    <table cellspacing="2" cellpadding="0" border="0">
	    <tr><td align="left" runat="Server" id="labelFriendName1"><b>Name:</b></td><td class="fieldreq" runat="server" id="barFriendName1">&nbsp;</td><td><asp:TextBox id="FriendName1" columns="18" maxlength="50" style="width: 200px;" runat="server"/><br /></td></tr>
	    <tr><td align="left" runat="Server" id="labelFriendEmail1"><b>E-mail:</b></td><td class="fieldreq" runat="server" id="barFriendEmail1">&nbsp;</td><td><asp:TextBox ID="FriendEmail1" columns="18" maxlength="50" style="width: 200px;" runat="server"/><br /></td></tr>
		<CC:RequiredFieldValidatorFront ControlToValidate="FriendName1" id="RequiredFieldValidatorFront2" runat="server" ErrorMessage="Your Friend name is blank"></cc:RequiredFieldValidatorFront>
		<CC:RequiredFieldValidatorFront ControlToValidate="FriendEmail1" id="RequiredFieldValidatorFront3" runat="server" ErrorMessage="Your Friend email is blank"></cc:RequiredFieldValidatorFront>
		<CC:EmailValidatorFront ID="EmailValidatorFront2" ControlToValidate="FriendEmail1" runat="server" ErrorMessage="Friend E-mail is invalid"></CC:EmailValidatorFront>
	    </table>
	    </div>
	    <div id="divFriend2" style="display:<%=Display2%>;margin-top:5px;">
	    <table cellspacing="2" cellpadding="0" border="0">
	    <tr><td class="field" align="left" runat="Server" id="labelFriendName2"><b>Name:</b></td><td class="field">&nbsp;</td><td><asp:TextBox id="FriendName2" columns="18" maxlength="50" style="width: 200px;" runat="server"/><br /></td></tr>
	    <tr><td class="field" align="left" runat="Server" id="labelFriendEmail2"><b>E-mail:</b></td><td class="field">&nbsp;</td><td><asp:TextBox ID="FriendEmail2" columns="18" maxlength="50" style="width: 200px;" runat="server"/><br /></td></tr>
		<CC:EmailValidatorFront ID="EmailValidatorFront3" ControlToValidate="FriendEmail2" runat="server" ErrorMessage="Friend E-mail is invalid"></CC:EmailValidatorFront>
		<CC:CustomValidatorFront ID="valCustom2" runat="server" ControlToValidate="FriendName2" ErrorMessage="Both email and name must be entered"></CC:CustomValidatorFront>
	    </table>
	    </div>
	    <div id="divFriend3" style="display:<%=Display3%>;margin-top:5px;">
	    <table cellspacing="2" cellpadding="0" border="0">
	    <tr><td class="field" align="left" runat="Server" id="labelFriendName3"><b>Name:</b></td><td class="field">&nbsp;</td><td><asp:TextBox id="FriendName3" columns="18" maxlength="50" style="width: 200px;" runat="server"/><br /></td></tr>
	    <tr><td class="field" align="left" runat="Server" id="labelFriendEmail3"><b>E-mail:</b></td><td class="field">&nbsp;</td><td><asp:TextBox ID="FriendEmail3" columns="18" maxlength="50" style="width: 200px;" runat="server"/><br /></td></tr>
		<CC:EmailValidatorFront ID="EmailValidatorFront4" ControlToValidate="FriendEmail3" runat="server" ErrorMessage="Friend E-mail is invalid"></CC:EmailValidatorFront>
		<CC:CustomValidatorFront ID="valCustom3" runat="server" ControlToValidate="FriendName3" ErrorMessage="Both email and name must be entered"></CC:CustomValidatorFront>
	    </table>
	    </div>
	</td>
	</tr>
	<tr>
	<td colspan="3" align="right">
	    <div id="divAddMore" style="display:<%=DisplayAddMore %>"><a href="javascript:ShowMore();">more &raquo;</a></div>
	</td>
	</tr>
	
</table>
</td>

<td>
<table width="50%" border="0" cellspacing="2" cellpadding="0">
	<tr valign="top">
		<td colspan="3">
		<table cellspacing="0" cellpadding="0" border="0">
		<tr valign="top">
		<td class="bold largest">3.&nbsp;</td>
		<td>Please enter the subject of your email.
		</td>
		</tr>
		</table>
		</td>
	</tr>
	<tr>
		<td align="left" runat="Server" id="labelSubject"><b>Subject:</b></td>
		<td class="fieldreq" runat="server" id="barSubject">&nbsp;</td>
		<td><asp:TextBox id="Subject" columns="18" maxlength="50" style="width: 200px;" runat="server"></asp:TextBox><br /></td>
		<CC:RequiredFieldValidatorFront ControlToValidate="Subject" id="RequiredFieldValidatorFront4" runat="server" ErrorMessage="Subject is blank"></cc:RequiredFieldValidatorFront>
	</tr>
	<tr><td colspan="3"><br /></td></tr>
	<tr valign="top">
		<td colspan="3">
		<table cellspacing="0" cellpadding="0" border="0">
		<tr valign="top">
		<td class="bold largest">4.&nbsp;</td>
		<td>Include a personal message. This message will be put at the top of the
		    e-mail sent to your friend.
		</td>
		</tr>
		<tr>
		<td colspan="2">
		<asp:TextBox id="Message" runat="server" TextMode="MultiLine" Columns="20" Rows="5" Wrap="true" style="width: 260px; height: 100px;"/>
		</td>
		</tr>
		</table>
		</td>
	</tr>
	<tr><td colspan="3"><br /></td></tr>
	<tr>
		<td colspan="3" align="right">
		<CC:OneClickButton  id="btnSend" CssClass="btn" Runat="server" Text="Send to Your Friend(s) &raquo;" />
		</td>
	</tr>
</table>
</td>
</tr>
</table>

</asp:Panel>

</form>

<!-- end -->


</body>
</html>