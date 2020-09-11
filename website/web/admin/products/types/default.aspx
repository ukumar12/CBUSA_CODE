<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Product Type" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Product Type Administration</h4>

<p></p>
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Product Type" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?ProductTypeID=" & DataBinder.Eval(Container.DataItem, "ProductTypeID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate="False" Message="Are you sure that you want to remove this Product Type?" runat="server" NavigateUrl= '<%# "delete.aspx?ProductTypeID=" & DataBinder.Eval(Container.DataItem, "ProductTypeID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
		    <ItemTemplate>
		    <input type="button" onclick='<%# "location.href=""/admin/products/types/attributes/default.aspx?F_ProductTypeId="& DataBinder.Eval(Container.DataItem,"ProductTypeId") &"""" %>' value="Attributes" class="btn" />
		    </ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField DataField="ProductType" HeaderText="Product Type"></asp:BoundField>
	</Columns>
</CC:GridView>

</asp:content>
