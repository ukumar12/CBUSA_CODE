<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Advertiser" CodeFile="default.aspx.vb" Inherits="index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Banners</h4>

<p></p>
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Banner" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate"></AlternatingRowStyle>
	<RowStyle CssClass="row"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "edit.aspx?BannerId=" & DataBinder.Eval(Container.DataItem, "BannerId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate=False Message="Are you sure that you want to remove this Banner?" runat="server" NavigateUrl= '<%# "delete.aspx?BannerId=" & DataBinder.Eval(Container.DataItem, "BannerId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
            <asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "groups.aspx?F_BannerId=" & DataBinder.Eval(Container.DataItem, "BannerId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/related.gif">Groups</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>			
		<asp:BoundField SortExpression="Name" DataField="Name" HeaderText="Name"/>
		<asp:BoundField SortExpression="BannerType" DataField="BannerType" HeaderText="Type"/>
		<asp:BoundField SortExpression="Width" DataField="Width" HeaderText="Width"/>
		<asp:BoundField SortExpression="Height" DataField="Height" HeaderText="Height"/>
		<asp:TemplateField>
			<ItemTemplate>
			<a target="_blank" href="<%# System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & DataBinder.Eval(Container.DataItem, "Link")  %>" ID="lnkLinks"><img alt="<%# DataBinder.Eval(Container.DataItem, "AltText")%>" src="/assets/banner/<%# DataBinder.Eval(Container.DataItem, "Filename")%>" border="0" /></a>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:ImageField SortExpression="IsActive" DataImageUrlField="IsActive" HeaderText="Is Active" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
		<asp:ImageField SortExpression="IsOrderTracking" DataImageUrlField="IsOrderTracking" HeaderText="E-commerce" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
	</Columns>
</CC:GridView>

</asp:content>

