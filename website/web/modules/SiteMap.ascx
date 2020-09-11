<%@ Control Language="VB" AutoEventWireup="false" CodeFile="SiteMap.ascx.vb" Inherits="SiteMapCtrl" %>

<asp:Repeater id="rptSiteMap" runat="server" EnableViewState="False">
<HeaderTemplate>
<center>
<table border="0" style="text-align:left">
<tr><td valign="top">
</HeaderTemplate>
<ItemTemplate>

<div class="bold" style="padding-top:15px" id="divSection" runat="server" visible="false">
<a href="<%#DataBinder.Eval(Container.DataItem, "SectionURL")%>"><%#DataBinder.Eval(Container.DataItem, "SectionName")%></a><br />
</div>
<div style="padding:2px 2px 2px 15px; width:200px" id="divSubSection" runat="server" visible="false">
<a href="<%#DataBinder.Eval(Container.DataItem, "SubSectionURL")%>"><%#DataBinder.Eval(Container.DataItem, "SubSectionName")%></a><br />
</div>

</ItemTemplate>
<FooterTemplate>
</td>
</tr>
</table>
</center>
</FooterTemplate>
</asp:Repeater>
