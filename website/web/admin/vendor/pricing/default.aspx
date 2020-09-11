<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Vendor Product Price" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Vendor Product Price Administration</h4>

<p>
    <CC:ConfirmButton ID="btnRollPricing" runat="server" Message="Click OK to roll all prices for this vendor" Text="Roll All Pricing" CssClass="btn" />
</p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?ProductID="& Databinder.Eval(Container.DataItem,"ProductID") & "&VendorID=" & DataBinder.Eval(Container.DataItem, "VendorID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate="False" Message="Are you sure that you want to remove this Vendor Product Price?" runat="server" NavigateUrl= '<%# "delete.aspx?VendorID=" & DataBinder.Eval(Container.DataItem, "VendorID") & "&ProductID="& Databinder.Eval(Container.DataItem,"ProductID") &"&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField DataField="ProductName" SortExpression="ProductName" HeaderText="Product"></asp:BoundField>
		<asp:BoundField DataField="VendorSKU" HeaderText="Vendor SKU"></asp:BoundField>
		<asp:BoundField DataField="VendorPrice" HeaderText="Vendor Price" DataFormatString="{0:c}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField DataField="NextPrice" HeaderText="Next Price" DataFormatString="{0:c}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField SortExpression="NextPriceApplies" DataField="NextPriceApplies" HeaderText="Next Price Applies" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:ImageField DataImageUrlField="IsSubstitution" HeaderText="Is Substitution" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
		<asp:ImageField DataImageUrlField="IsDiscontinued" HeaderText="Is Discontinued" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
		<asp:BoundField SortExpression="Submitted" DataField="Submitted" HeaderText="Submitted" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField SortExpression="Updated" DataField="Updated" HeaderText="Updated" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
	</Columns>
</CC:GridView>

</asp:content>
