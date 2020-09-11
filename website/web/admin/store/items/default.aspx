<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Store Item" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Store Item Administration</h4>

<span class="smaller">Please provide search criteria below</span>
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top">Item Name:</th>
<td valign="top" class="field"><asp:textbox id="F_ItemName" runat="server" Columns="50" MaxLength="200"></asp:textbox></td>
<th valign="top">Department</th>
<td valign="top" class="field"><CC:DropDownListEx id="F_DepartmentId" runat="server" /></td>
</tr>
<tr>
<th valign="top"><b>Template:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_TemplateId" runat="server" /></td>
<th valign="top"><b>Brand:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_BrandId" runat="server" /></td>
</tr>
<tr>
<th valign="top"><b>Is On Sale:</b></th>
<td valign="top" class="field">
	<asp:DropDownList ID="F_IsOnSale" runat="server">
		<asp:ListItem Value="">-- ALL --</asp:ListItem>
		<asp:ListItem Value="1">Yes</asp:ListItem>
		<asp:ListItem Value="0">No</asp:ListItem>
	</asp:DropDownList>
</td>
<th valign="top"><b>Is Active:</b></th>
<td valign="top" class="field">
	<asp:DropDownList ID="F_IsActive" runat="server">
		<asp:ListItem Value="">-- ALL --</asp:ListItem>
		<asp:ListItem Value="1">Yes</asp:ListItem>
		<asp:ListItem Value="0">No</asp:ListItem>
	</asp:DropDownList>
</td>
</tr>
<tr>
<th valign="top"><b>Is Featured:</b></th>
<td valign="top" class="field">
	<asp:DropDownList ID="F_IsFeatured" runat="server">
		<asp:ListItem Value="">-- ALL --</asp:ListItem>
		<asp:ListItem Value="1">Yes</asp:ListItem>
		<asp:ListItem Value="0">No</asp:ListItem>
	</asp:DropDownList>
</td>
<th valign="top">SKU:</th>
<td valign="top" class="field"><asp:textbox id="F_SKU" runat="server" Columns="20" MaxLength="20"></asp:textbox></td>
</tr><tr>
<td colspan="4" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
<input class="btn" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />
</td>
</tr>
</table>
<p></p>
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Store Item" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "edit.aspx?ItemId=" & DataBinder.Eval(Container.DataItem, "ItemId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate=False Message="Are you sure that you want to remove this Store Item?" runat="server" NavigateUrl= '<%# "delete.aspx?ItemId=" & DataBinder.Eval(Container.DataItem, "ItemId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "images/default.aspx?F_ItemId=" & DataBinder.Eval(Container.DataItem, "ItemId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/image.gif">Images</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>	
		<asp:TemplateField>
			<ItemTemplate>
            <asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "related/default.aspx?F_ItemId=" & DataBinder.Eval(Container.DataItem, "ItemId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/related.gif">Related Items</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>			
		<asp:BoundField SortExpression="ItemName" DataField="ItemName" HeaderText="Item Name"></asp:BoundField>
		<asp:BoundField SortExpression="SKU" DataField="SKU" HeaderText="SKU"></asp:BoundField>
		<asp:BoundField SortExpression="BrandName" DataField="BrandName" HeaderText="Brand"></asp:BoundField>
		<asp:TemplateField>
			<HeaderTemplate>
                <asp:LinkButton enableviewstate="False" CommandArgument="Price" CommandName="sort" id="SortMime" runat="server">Price</asp:LinkButton>
                <asp:LinkButton enableviewstate="False" visible='<%# gvList.SortBy = "Price" and gvList.SortOrder = "ASC" %>' CommandArgument="Price" CommandName=sort runat="server"><img border="0" src="/images/admin/Asc3.gif" alt="" align="absmiddle" /></asp:LinkButton>
                <asp:LinkButton enableviewstate="False" visible='<%# gvList.SortBy = "Price" and gvList.SortOrder = "DESC" %>' CommandArgument="Price" CommandName=sort runat="server"><img border="0" src="/images/admin/Desc3.gif" alt="" align="absmiddle" /></asp:LinkButton>
		    </HeaderTemplate>
			<ItemTemplate>
				<asp:Literal runat="server" ID="ltlPrice" />
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<HeaderTemplate>
				Department(s)
			</HeaderTemplate>
			<ItemTemplate>
				<asp:Repeater ID="Departments" Runat="server">
					<ItemTemplate>
						<li style="list-style-image:url(/images/minifolder.gif)">
							<%# DataBinder.Eval(Container, "DataItem.Name") %>
							<br>
					</ItemTemplate>
				</asp:Repeater>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<HeaderTemplate>
				Image Preview
			</HeaderTemplate>
			<ItemTemplate>
				<asp:Label runat="server" ID="imglink"><a href='javascript:expandit(<%#DataBinder.Eval(Container, "DataItem.ItemId")%>);'><span class="smaller" id='imgtext<%#DataBinder.Eval(Container, "DataItem.ItemId")%>'>View Image</span><img id='IMG<%#DataBinder.Eval(Container, "DataItem.ItemId")%>' src="/images/detail-down.gif" width="8" height="8" hspace="2" border="0" alt="Expand" align="absmiddle" ></a></asp:Label><asp:Label runat="server" ID="noimg" Text="N/A" CssClass="smaller" /><span style="display:none" id='SPAN<%#DataBinder.Eval(Container, "DataItem.ItemId")%>'><asp:Literal id="img" Runat="server"></asp:Literal></span>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:ImageField SortExpression="IsActive" DataImageUrlField="IsActive" HeaderText="Is Active" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
		<asp:ImageField SortExpression="IsFeatured" DataImageUrlField="IsFeatured" HeaderText="Is Featured" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
	</Columns>
</CC:GridView>

</asp:content>
