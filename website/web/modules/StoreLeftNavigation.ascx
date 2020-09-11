<%@ Control Language="VB" AutoEventWireup="false" CodeFile="StoreLeftNavigation.ascx.vb" Inherits="StoreLeftNavigation" %>

<div class="lnavwrpr">

<div class="lnvgrphdg"><asp:Literal id="ltlHeader" runat="server" /></div>

<a href="" id="lnkBack" runat="server" />

<asp:Repeater runat="server" ID="rptDepartments">
	<HeaderTemplate>
	        <div class="lnvgrphdg">Shop By Department</div>
			<ul class="lnav">
	</HeaderTemplate>
	<ItemTemplate>
	            <li id="liSel" runat="server" visible="false"><%#Container.DataItem("Name")%></li>
				<li id="liLnk" runat="server" visible="false"><a href='<%#System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & IIf(IsDBNull(Container.DataItem("CustomURL")), "/store/default.aspx?DepartmentId=" & Container.DataItem("DepartmentId"), Container.DataItem("CustomURL"))%>'><%#Container.DataItem("Name")%></a></li>
	</ItemTemplate>
	<FooterTemplate>
			</ul>
	</FooterTemplate>
</asp:Repeater>

<asp:Repeater runat="server" ID="rptBrands">
	<HeaderTemplate>
	        <div class="lnvgrphdg">Shop By Brand</div>
			<ul class="lnav">
	</HeaderTemplate>
	<ItemTemplate>
	            <li id="liSel" runat="server" visible="false"><%#Container.DataItem("Name")%></li>
				<li id="liLnk" runat="server" visible="false"><a href='<%#System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & IIf(IsDBNull(Container.DataItem("CustomURL")), "/store/brand.aspx?F_BrandId=" & Container.DataItem("BrandId"), Container.DataItem("CustomURL"))%>'><%#Container.DataItem("Name")%></a></li>
	</ItemTemplate>
	<FooterTemplate>
			</ul>
	</FooterTemplate>
</asp:Repeater>

</div>