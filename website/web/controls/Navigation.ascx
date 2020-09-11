<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Navigation.ascx.vb" Inherits="Navigation" %>

<table cellspacing="0" cellpadding="0" border="0" width="100%">
<tr>
<td nowrap>
<asp:Literal ID="ltlShowing" runat="server"></asp:Literal>
</td>
<td width="50">&nbsp;</td>
<td align="right">

<div class="pager">
<a class="backnext" href="" enableviewstate="False" id="lnkPrev2" runat="server">&laquo; prev</a>
<asp:Literal ID="ltl" runat="server" EnableViewState="false" />
<a class="backnext" href="" enableviewstate="False" id="lnkNext1" runat="server">next &raquo;</a>
</div>

</td></tr>
</table>
