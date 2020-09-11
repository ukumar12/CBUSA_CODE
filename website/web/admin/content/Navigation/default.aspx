<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Navigation" CodeFile="default.aspx.vb" Inherits="index"  %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Navigation</h4>

<p></p>
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Section" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" AllowSorting="False" HeaderText="In order to change display order, please use arrows" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" AllowPaging="false">
	<AlternatingRowStyle CssClass="alternate"></AlternatingRowStyle>
	<RowStyle CssClass="row"></RowStyle>
	<Columns>
		<asp:TemplateField ItemStyle-VerticalAlign="Top" >
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "edit.aspx?NavigationId=" & DataBinder.Eval(Container.DataItem, "NavigationId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField ItemStyle-VerticalAlign="Top" >
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate=False Message="Are you sure that you want to remove this Navigation?" runat="server" NavigateUrl= '<%# "delete.aspx?NavigationId=" & DataBinder.Eval(Container.DataItem, "NavigationId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField ItemStyle-VerticalAlign="Top" DataField="Title" HeaderText="Title"></asp:BoundField>
		
		<asp:TemplateField ItemStyle-VerticalAlign="Top" >
			<ItemTemplate>
			
            <input type="button" id="btnAdd" class="btn" onclick="document.location.href='edit.aspx?ParentId=<%#DataBinder.Eval(Container.DataItem, "NavigationId") %>'" value="Add New Sub-Section" />
						
		    <CC:GridView id="gvSubList" CellSpacing="2" CellPadding="2" runat="server" HeaderText="In order to change display order, please use arrows" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0">
	            <AlternatingRowStyle cssclass="alternate"></AlternatingRowStyle>
	            <RowStyle cssclass="row"></RowStyle>
	            <Columns>
		        <asp:TemplateField>
			        <ItemTemplate>
			        <asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "edit.aspx?NavigationId=" & DataBinder.Eval(Container.DataItem, "NavigationId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			        </ItemTemplate>
		        </asp:TemplateField>
		        <asp:TemplateField>
			        <ItemTemplate>
			        <CC:ConfirmLink enableviewstate=False Message="Are you sure that you want to remove this Navigation?" runat="server" NavigateUrl= '<%# "delete.aspx?NavigationId=" & DataBinder.Eval(Container.DataItem, "NavigationId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			        </ItemTemplate>
		        </asp:TemplateField>
        		<asp:BoundField ItemStyle-VerticalAlign="Top" DataField="Title" HeaderText="Title"></asp:BoundField>
        		<asp:ImageField SortExpression="IsInternalLink" DataImageUrlField="IsInternalLink" HeaderText="Internal Link?" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
        		<asp:ImageField SortExpression="SkipSitemap" DataImageUrlField="SkipSitemap" HeaderText="Skip Sitemap?" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
        		<asp:ImageField SortExpression="SkipBreadcrumb" DataImageUrlField="SkipBreadcrumb" HeaderText="Skip Breadcrumb?" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
    		    <asp:TemplateField>
			        <ItemTemplate>
			        <asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "move.aspx?ParentId=" & DataBinder.Eval(Container.DataItem, "ParentId") & "&Action=Up&NavigationId=" & DataBinder.Eval(Container.DataItem, "NavigationId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/moveup.gif" ID="lnkMoveUp">Move Up</asp:HyperLink>
			        </ItemTemplate>
		        </asp:TemplateField>
		        <asp:TemplateField>
			        <ItemTemplate>
			        <asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "move.aspx?ParentId=" & DataBinder.Eval(Container.DataItem, "ParentId") & "&Action=Down&NavigationId=" & DataBinder.Eval(Container.DataItem, "NavigationId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/movedown.gif" ID="lnkMoveDown">Move Down</asp:HyperLink>
			        </ItemTemplate>
		        </asp:TemplateField>
   		        
	            </Columns>
	        </CC:GridView>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:ImageField SortExpression="IsInternalLink" DataImageUrlField="IsInternalLink" HeaderText="Internal Link?" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
		<asp:ImageField SortExpression="SkipSitemap" DataImageUrlField="SkipSitemap" HeaderText="Skip Sitemap?" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
		<asp:TemplateField ItemStyle-VerticalAlign="Top" >
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "move.aspx?Action=Up&NavigationId=" & DataBinder.Eval(Container.DataItem, "NavigationId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/moveup.gif" ID="lnkMoveUp">Move Up</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField ItemStyle-VerticalAlign="Top" >
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "move.aspx?Action=Down&NavigationId=" & DataBinder.Eval(Container.DataItem, "NavigationId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/movedown.gif" ID="lnkMoveDown">Move Down</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
	</Columns>
</CC:GridView>

</asp:content>





