<%@ Page Language="VB" AutoEventWireup="false" CodeFile="default.aspx.vb" Inherits="Search" %>
<%@ Register Src="~/controls/SearchResults.ascx" TagName="SearchItem"  TagPrefix="CC" %>
<%@ Register TagName="Navigator" TagPrefix="CC" Src="~/controls/StoreNavigator.ascx" %>
<CT:masterpage runat="server" id="CTMain" DefaultButton="btnSearch">

<table align="center" border="0" runat="server">
<tr>
	<td colspan="2" runat="server">
	<asp:Radiobutton id="rbMatchOr" GroupName="Match" runat="server" value="OR" />Match any words
	&nbsp;<asp:Radiobutton id="rbMatchAnd" GroupName="Match" runat="server" value="AND" />Match all words
	&nbsp;<asp:Radiobutton id="rbMatchExact" GroupName="Match" runat="server" value="EXACT" />Match an exact phrase</td>
</tr>
<tr><td align="right"><b>Keyword</b>&nbsp;<asp:TextBox id="txtKeyword" maxlength="50" runat="server" /></td><td width="120"><CC:OneClickButton ID="btnSearch" runat="server" Text="Search" CssClass="btn" /></td></tr>
</table>

<asp:Literal id="ltlHeaderTitle" runat="server" EnableViewstate="false"/>

<div id="divItems" runat="server" visible="false">

<CC:Navigator runat="server" ID="NavigatorTop" />
<asp:Repeater runat="server" id="rptItems" EnableViewstate="False">
<HeaderTemplate>
<table style="width:777px; margin:10px 0 15px 12px;" cellspacing="0" cellpadding="0" border="0" summary="subdept thumbnail display">
</HeaderTemplate>
<ItemTemplate>
<asp:literal id="ltlStart" runat="server" EnableViewstate="False"/>
<td>
<CC:SearchItem ID="ctrlSearchItem" runat="server" />
</td>
<asp:Literal id="ltlEnd" runat="server" EnableViewstate="False"/>

<tr id="trDivider" runat="server" visible="false">
<td colspan="3" id="tdColspan" runat="Server">
	<div class="dashln" style="padding:10px 0 10px 0;">
		&nbsp;
	</div>
</td>
</tr>
</ItemTemplate>
<FooterTemplate>
</table>
</FooterTemplate>
</asp:Repeater>
<CC:Navigator runat="server" ID="NavigatorBottom" />


<div id="divNoRecords" runat="server" visible="False" style="align:center;">
<center>
There are no records that match your search criteria. Please change the search criteria and try again.
</center>
</div>

</div>

</CT:masterpage>