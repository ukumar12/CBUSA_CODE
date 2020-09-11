<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MailingPreview.ascx.vb" Inherits="MailingPreview" %>

<script type="text/javascript">
<!--
	function CheckTab(el) {
	  if ((document.all) && (9==event.keyCode) && (event.ctrlKey)) {
		el.selection=document.selection.createRange();
		setTimeout("ProcessTab('" + el.id + "')",0)
	  }
	}
	function ProcessTab(id) {
	  document.all[id].selection.text=String.fromCharCode(9)
	  document.all[id].focus()
	}
//-->
</script>

<table cellpadding="0" cellspacing="0">
<tr>
<%  If MimeType = "HTML" Or MimeType = "BOTH" Then%>
<td><div><asp:LinkButton id="lnkHTML" runat="server">HTML Version</asp:LinkButton></div></td>
<% end if %>
<%  If MimeType = "TEXT" Or MimeType = "BOTH" Then%>
<td><div><asp:LinkButton id="lnkText" runat="server">Text Version</asp:LinkButton></div></td>
<% end if %>
</tr>
</table>

<div id="divHTML" runat="server" enableviewstate="False" style="margin-top:10px">
<iframe src="/assets/temp/<%=FileName%>" width="810" id="main" height="450" frameborder="1"></iframe>
</div>

<div id="divText" runat="server">

<div id="divSavedText" runat="server" enableviewstate="False">
<p>
<b>Customized Message</b><br />
The message below is not automatically generated from HTML data that you entered.<br>
If you made changes to HTML and you wish to re-generate plain text, plese click <CC:OneClickButton ID="btnReload" cssClass="btn" Text="Reload" runat="server" /> button
</div>

<div id="divGeneratedText" runat="server" enableviewstate="False">
<p>
<b>Generated  Message</b><br />
Below is the plain text email message that has been <b>automatically generated</b> from defined HTML message.<br />
You can make any modifications to this message and <b>"Save Changes"</b>.<br />Please note that once you do that, the text message
will not be automatically overwritten unless you refresh it explicitly.
</div>

<div class="smaller" style="margin-top:15px;">If you are using IE, you can enter TAB using CTRL-TAB inside of this text box</div>

<textarea id="txtText" runat="server" onKeyDown="CheckTab(this)" style="font-family:Courier New;font-size:12px;" rows="20" cols="110"></textarea>

<div class="smaller">
<span style='font-size:8.5pt;font-family:Arial;font-weight:bold'>Note:</span> In accordance with the "CAN-SPAM" Act, a link to the unsubscribe page must be inserted at the end of the message along with the physical mailing address.
</div>

<p>
<CC:OneClickButton ID="btnSave" cssClass="btn" Text="Save Changes" runat="server" />
</p>

</div>
