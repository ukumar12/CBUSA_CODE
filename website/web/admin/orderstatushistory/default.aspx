<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Order Status History" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Order Status History Administration</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top"><b>Order:</b></th>
<td valign="top" class="field">
<asp:DropDownList ID="F_OrderID" runat="server"></asp:DropDownList>
</td>
</tr>
<tr>
<th valign="top"><b>Order Status:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_OrderStatusID" runat="server" /></td>
</tr>
<tr>
<th valign="top"><b>Created:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_CreatedLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_CreatedUbound" runat="server" /></td>
</tr>
</table>
</td>
</tr>
<tr>
<td colspan="2" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
<input class="btn" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />
</td>
</tr>
</table>
</asp:Panel>
<p></p>
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Order Status History" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?OrderStatusHistoryID=" & DataBinder.Eval(Container.DataItem, "OrderStatusHistoryID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate="False" Message="Are you sure that you want to remove this Order Status History?" runat="server" NavigateUrl= '<%# "delete.aspx?OrderStatusHistoryID=" & DataBinder.Eval(Container.DataItem, "OrderStatusHistoryID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="OrderNumber" DataField="OrderNumber" HeaderText="Order Number"></asp:BoundField>
		<asp:BoundField SortExpression="OrderStatusID" DataField="OrderStatus" HeaderText="Order Status"></asp:BoundField>
		<asp:BoundField SortExpression="Created" DataField="Created" HeaderText="Created" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
	</Columns>
</CC:GridView>

</asp:content>

