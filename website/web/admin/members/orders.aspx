<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" CodeFile="orders.aspx.vb" Inherits="admin_members_orders" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Web Orders</h4>
Order History for <asp:Label ID="txtMemberName" runat="server"></asp:Label>
| <a runat="server" id="lnkBack">&laquo; Go Back to Member Profile</a><br /><br />
<asp:Literal ID="ltlSummary" runat="server" />
<p></p>
<CC:GridView ShowFooter="True" id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
    <RowStyle HorizontalAlign="Right" />
	<FooterStyle CssClass="header" HorizontalAlign="Right" />
	<Columns>
        <asp:TemplateField>
            <ItemTemplate>
			    <asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "/admin/store/orders/edit.aspx?MemberId=" & MemberId & "&OrderId=" & DataBinder.Eval(Container.DataItem, "OrderId") & "&RedirectUrl=/admin/members/orders.aspx?" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
            </ItemTemplate>
        </asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate=False Message="Are you sure that you want to remove this Order?" runat="server" NavigateUrl= '<%# "/admin/store/orders/delete.aspx?OrderId=" & DataBinder.Eval(Container.DataItem, "OrderId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="OrderNo" DataField="OrderNo" HeaderText="Order#"></asp:BoundField>
		<asp:BoundField SortExpression="BillingLastName" DataField="FullName" HeaderText="Customer Name"></asp:BoundField>
		<asp:BoundField SortExpression="ProcessDate" HTMLEncode="False" DataFormatString="{0:d}" DataField="ProcessDate" HeaderText="Order Date"></asp:BoundField>
		<asp:BoundField SortExpression="ShippedDate" HTMLEncode="False" DataFormatString="{0:d}" DataField="ShippedDate" HeaderText="Shipped Date"></asp:BoundField>
		<asp:BoundField SortExpression="Subtotal" HTMLEncode="False" DataFormatString="{0:c}" DataField="Subtotal" HeaderText="Subtotal"></asp:BoundField>
		<asp:BoundField SortExpression="Discount" HTMLEncode="False" DataFormatString="{0:c}" DataField="Discount" HeaderText="Discount"></asp:BoundField>
		<asp:BoundField SortExpression="Tax" HTMLEncode="False" DataFormatString="{0:c}" DataField="Tax" HeaderText="Tax"></asp:BoundField>
		<asp:BoundField SortExpression="Shipping" HTMLEncode="False" DataFormatString="{0:c}" DataField="Shipping" HeaderText="Shipping"></asp:BoundField>
		<asp:BoundField SortExpression="Total" HTMLEncode="False" DataFormatString="{0:c}" DataField="Total" HeaderText="Total"></asp:BoundField>
		<asp:BoundField SortExpression="Status" DataField="Status" HeaderText="Status"></asp:BoundField>
	</Columns>
</CC:GridView>
</asp:content>
