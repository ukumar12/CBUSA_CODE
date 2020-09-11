<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Inherits="member_wishlist_send" CodeFile="send.aspx.vb" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
<h4>Send WishList</h4>
<asp:Panel runat="server" id="pnlSend" defaultbutton="btnSend">

<table cellspacing="0" cellpadding="0" border="0" style="margin-top:10px; margin-left: 25px;">
<tr>
<td colspan="2"><font color="red">Red color indicates required field</font></td>
</tr>
</table>

<table width="100%">
<tr valign="top">
<td style="padding-left:25px;">

		<table>
		<tr valign="top">
		<td class="required" style="width:130px;"><span id="labeltxtEmail" runat="server">Recipient E-mail</span></td>
	    <td class="field"><asp:textbox id="txtEmail" runat="server" style="width:250px;" maxlength="100" /><br /></td>
	    </tr>

		<tr valign="top">
		<td class="optional" style="width:130px;"><span id="labeltxtMessage" runat="server">Personal Message</span><br />(optional)</td>
		<td class="field">
			<asp:textbox id="txtMessage" runat="server" columns="25" rows="5" style="width:250px;height:85px;" TextMode="Multiline" /></td>
		</tr>
        
        <tr>
        <td></td>
        <td>&nbsp;</td>
        </tr>			
	    </table>
	    
        <p>
        <table cellspacing="0" cellpadding="4" width="100%">
        <tr><td colspan=2 class="optional"><b>MESSAGE PREVIEW</b></td></tr>
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
        <CC:OneClickButton runat="server" cssClass="btn" id="btnCancel" Text="Cancel"/>
        
	    
</td></tr>
</table>

<CC:RequiredFieldValidatorFront ID="rvtxtEmail" runat="server" Display="None" ControlToValidate="txtEmail" ErrorMessage="You must provide  email address" />
<CC:EmailValidatorFront ID="evtxtEmail" runat="server" Display="None" ControlToValidate="txtEmail" ErrorMessage="You must provide a valid email address" />


</asp:Panel>

</asp:Content>