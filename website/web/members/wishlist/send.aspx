<%@ Page Language="vb" AutoEventWireup="false" Inherits="member_wishlist_send" CodeFile="send.aspx.vb" %>

<CT:masterpage runat="server" id="CTMain">

<asp:Panel runat="server" id="pnlSend" defaultbutton="btnSend">

<table cellspacing="0" cellpadding="0" border="0" style="margin-top:10px; margin-left: 25px;">
<tr>
<td class="fieldreq" style="padding-bottom:6px;">&nbsp;</td>
<td class="smaller" style="padding:0 0 6px 4px;">Indicates required field</td>
</tr>
</table>

<table width="100%" cellspacing="0" cellpadding="0" border="0">
<tr valign="top">
<td style="padding-left:25px;">

		<table cellspacing="0" cellpadding="0" border="0" style="width:700px; margin:15px 0 10px 20px;">
		<tr valign="top">
		<td class="fieldlbl" style="width:130px;"><span id="labeltxtEmail" runat="server">Recipient E-mail</span></td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldreq" id="bartxtEmail" runat="server">&nbsp;</td>
			<td class="field"><asp:textbox id="txtEmail" runat="server" style="width:250px;" maxlength="100" /><br /></td>
			</tr>
			</table>
		</td>
		</tr>

		<tr valign="top">
		<td class="fieldlbl" style="width:130px;"><span id="labeltxtMessage" runat="server">Personal Message</span><br />(optional)</td>
		<td class="fieldpad">
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
			<td class="fieldnorm" id="bartxtMessage" runat="server">&nbsp;</td>
			<td class="field">
			<asp:textbox id="txtMessage" runat="server" columns="25" rows="5" style="width:250px;height:85px;" TextMode="Multiline" /></td>
			</tr>
			</table>
		</td>
		</tr>
        
        <tr>
        <td></td>
        <td class="fieldpad">&nbsp;</td>
        </tr>			
	    </table>
	    
        <p>
        <table cellspacing="0" cellpadding="4" width="100%" class=bdr>
        <tr><td colspan=2 class="baghdr"><b>MESSAGE PREVIEW</b></td></tr>
        <tr>
        <td>

        <div><b>Subject:</b></div>
        <div> A <%=WebsiteName%> Wish List for you from <%=FullName%>!</div>
        <p>
        <div><b>Body:</b></div>
        <div>Greetings from <%=WebsiteName%>!</div>
        <p>
        <div><%=FullName%> has sent you a Wish List from <%=WebsiteName%> website.</div>
        <p>
        <div>Click on the link below to see what items are on the list.</div>
        <div><a href="<%=RefererName%>/wishlist.aspx?w=<%=dbMember.Guid%>"><%=RefererName%>/wishlist.aspx?w=<%=dbMember.Guid%></a></div>
        <p>
        <div>Now you don't have to read <%=dbMemberAddress.FirstName%>'s mind -- buy a wished-for item and make <%=dbMemberAddress.FirstName%>'s day!</div>

        <p>
        <div>--------------------------------------------------------------</div>
        <div><%=dbMemberAddress.FirstName%>'s message to you:</div>
        <p>
        <div><b>&laquo;&laquo; YOUR OPTIONAL PERSONAL MESSAGE&raquo;&raquo;</b></div>

        </td>
        </tr>
        </table>
	    
	    <p></p>
        <CC:OneClickButton runat="server" cssClass="btn" id="btnSend" Text="Send"/>
        <input type="button" class="btn" value="Cancel" onclick="document.location.href='default.aspx'" />
	    
</td></tr>
</table>

<CC:RequiredFieldValidatorFront ID="rvtxtEmail" runat="server" Display="None" ControlToValidate="txtEmail" ErrorMessage="You must provide  email address" />
<CC:EmailValidatorFront ID="evtxtEmail" runat="server" Display="None" ControlToValidate="txtEmail" ErrorMessage="You must provide a valid email address" />


</asp:Panel>

</CT:masterpage>