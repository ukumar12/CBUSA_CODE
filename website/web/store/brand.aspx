<%@ Page Language="VB" AutoEventWireup="false" CodeFile="brand.aspx.vb" Inherits="store_brand" %>
<%@ Register TagName="Navigator" TagPrefix="CC" Src="~/controls/StoreNavigator.ascx" %>

<CT:masterpage runat="server" id="CTMain">

<div class="bcrmwrpr"><asp:Literal runat="server" id="ltlBreadcrumb" /></div>

<h1 class="hdng" id="hTitle" runat="server"></h1>

<asp:Panel runat="server" id="pnlBrands" visible="false">
	<div class="thumblgwrpr">
    <asp:Repeater runat="server" id="rptBrands" enableviewstate="False">
	<ItemTemplate>
		<a href="" runat="server" id="lnkBrand" enableviewstate="false">
			<img src="" runat="server" id="imgBrand" enableviewstate="false" alt='<%#Container.DataItem("Name")%>' /><br />
			<%#Container.DataItem("Name")%>
		</a>
		<div runat="server" id="divSpacer" visible="False" enableviewstate="false" style="clear:both;">&nbsp;</div>
	</ItemTemplate>
    </asp:Repeater>
    </div>
</asp:Panel>

<asp:Panel runat="server" id="pnlItems" visible="false">
	<CC:Navigator runat="server" ID="NavigatorTop" />
	<asp:Repeater runat="server" id="rptItems">
		<HeaderTemplate>
			<div class="thumbwrpr">
		</HeaderTemplate>
		<ItemTemplate>
		    <a href="/" runat="server" id="lnkItem" enableviewstate="false">    
				<img src="" runat="server" id="imgItem" enableviewstate="false" alt='<%#Container.DataItem.ItemName%>' /><br />
				<%#Container.DataItem.ItemName%><br />
				<strong runat="server" id="ltlPrice"></strong><br />
				<%#Container.DataItem.ShortDescription%>
			</a>
			<div runat="server" id="divSpacer" visible="False" style="clear:both;">&nbsp;</div>
		</ItemTemplate>
		<FooterTemplate>
			</div>
		</FooterTemplate>
	</asp:Repeater>
	<CC:Navigator runat="server" ID="NavigatorBottom" />

</asp:Panel>

</CT:masterpage>
