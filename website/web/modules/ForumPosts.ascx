<%@ Control EnableViewstate="False" Language="VB" AutoEventWireup="false" CodeFile="ForumPosts.ascx.vb" Inherits="_default" %>
<script type="text/javascript">
    var GB_ROOT_DIR = "/includes/greybox/";
</script>
<script src="/includes/greybox/AJS.js" type="text/javascript"></script>
<script src="/includes/greybox/AJS_fx.js" type="text/javascript"></script>
<script src="/includes/greybox/gb_scripts.js" type="text/javascript"></script>
<link href="/includes/greybox/gb_styles.css" rel="stylesheet" type="text/css" />
<div class="fpheader"><p class="fptitle">Forum Posts</p></div>
<div>
<asp:Repeater id="rptForumPosts" runat="server" enableviewstate="False">
<HeaderTemplate>
<table class="fptable">
</HeaderTemplate>
<ItemTemplate>
    <tr class="fprow">
        <td class="fprow">
            <a rel="gb_page_center[500,500]" class="fptitlelink" href="/builder/message/popup.aspx?messageid=<%#DataBinder.Eval(Container.DataItem, "AutomaticMessageID")%>"><%#DataBinder.Eval(Container.DataItem, "Title")%></a>&#160;
            <p class="fpposteddate">Posted:&#160;&#160;<%#DataBinder.Eval(Container.DataItem, "Created")%></p>
            <span class="fpmessagetext"><%#DataBinder.Eval(Container.DataItem, "Message")%>&#160;<a rel="gb_page_center[500,500]" class="fpreadmorelink" href="/builder/message/popup.aspx?messageid=<%#DataBinder.Eval(Container.DataItem, "AutomaticMessageID")%>">... read more</a></span>
        </td>
    </tr>
</ItemTemplate>
<FooterTemplate>
</table>
</FooterTemplate>
</asp:Repeater>	 
</div>