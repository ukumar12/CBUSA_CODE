<%@ Control Language="VB" AutoEventWireup="false" CodeFile="StoreNavigator.ascx.vb" Inherits="StoreNavigator" %>
<table cellspacing="0" cellpadding="0" border="0" width="100%">
<tr>
<td nowrap>sort by:
<asp:Literal ID="ltlAlphabetical" runat="server" /> |
<asp:Literal ID="ltlPriceAsc" runat="server" /> |
<asp:Literal ID="ltlPriceDesc" runat="server" />
</td>
<td align="right" style="border-right:20px;">
<div class="paginate">
	<span id="Span1" runat="server" visible="false"><a class="previous" enableviewstate="False" id="lnkPrev" runat="server" href="#">back</a><asp:Label runat="server" ID="lnkPrev2" CssClass="previous" Text="back" /> | <a class="next" href="#" enableviewstate="False" id="lnkNext" runat="server">next</a><asp:Label runat="server" ID="lnkNext2" CssClass="next" Text="next" /></span>
	<asp:PlaceHolder runat="server" ID="ph">Page <asp:Literal ID="ltl" runat="server" EnableViewState="false" /></asp:PlaceHolder><span id="Span2" runat="server" visible="false"> of <%=NofPages%></span> <asp:Literal runat="server" ID="litDivider" Text="&nbsp;" /> <a href="#" runat="server" id="lnkAll">View All</a><span id="Span3" runat="server" visible="false"> (<%=NofRecords%> items)</span>
	<span id="Span4" runat="server" visible="false"><span runat="server" id="spanPerPage">Per Page</span> <a href="#" runat="server" id="lnk12">12</a> | <a href="#" runat="server" id="lnk24">24</a> | <a href="#" runat="server" id="lnk48">48</a></span>
</div>
</td>
</tr>
</table>

