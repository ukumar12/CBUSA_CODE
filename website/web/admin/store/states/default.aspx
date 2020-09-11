<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="State" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>States Administration</h4>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "edit.aspx?StateId=" & DataBinder.Eval(Container.DataItem, "StateId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="StateName" DataField="StateName" HeaderText="State Name"></asp:BoundField>
		<asp:BoundField SortExpression="TaxRate" HTMLEncode="False" DataFormatString="{0:N2}%" DataField="TaxRate" HeaderText="Tax Rate"></asp:BoundField>
		<asp:ImageField SortExpression="TaxShipping" DataImageUrlField="TaxShipping" HeaderText="Shipping Taxable" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
		<asp:ImageField SortExpression="TaxGiftWrap" DataImageUrlField="TaxGiftWrap" HeaderText="Gift Wrap Taxable" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
	</Columns>
</CC:GridView>

</asp:content>

