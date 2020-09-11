<%@ Control Language="VB" AutoEventWireup="false" CodeFile="LimitTextBox.ascx.vb" Inherits="LimitTextBox" %>
<asp:TextBox Runat="server" id="ctrl"></asp:TextBox>
<table cellpadding="0" cellspacing="0" border="0" width="<%=Width%>">
<tr>
<td class="txtlimitbx"><img src="/images/spacer.gif" id="<%= ClientID %>TA1" name="<%=ClientID%>TA1" height="5" width="0"></td>
<td class="txtlimitbx2"><img src="/images/spacer.gif" id="<%= ClientID %>TA2" name="<%=ClientID%>TA2" height="5" width="<%=Width%>"></td>
</tr>
</table>
<div id="<%=ClientID%>DIV">40 characters left</div>

<script language=javascript>
limit('<%=ClientID%>', <%=Width%>, <%=Limit%>);
</script>