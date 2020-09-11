<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Order" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Order Administration</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top">Order Number:</th>
<td valign="top" class="field"><asp:textbox id="F_OrderNumber" runat="server" Columns="20" MaxLength="20"></asp:textbox></td>
</tr>
<tr>
<th valign="top"><b>Vendor:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_VendorID" runat="server" /></td>
</tr>
<tr>
<th valign="top"><b>Builder:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_BuilderID" runat="server" /></td>
</tr>
<tr>
<th valign="top"><b>Project:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_ProjectID" runat="server" /></td>
</tr>
<tr>
<th valign="top">Title:</th>
<td valign="top" class="field"><asp:textbox id="F_Title" runat="server" Columns="50" MaxLength="100"></asp:textbox></td>
</tr>
<tr>
<th valign="top">P O Number:</th>
<td valign="top" class="field"><asp:textbox id="F_PONumber" runat="server" Columns="20" MaxLength="20"></asp:textbox></td>
</tr>
<tr>
<th valign="top">Orderer Last Name:</th>
<td valign="top" class="field"><asp:textbox id="F_OrdererLastName" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top">Orderer Email:</th>
<td valign="top" class="field"><asp:textbox id="F_OrdererEmail" runat="server" Columns="50" MaxLength="100"></asp:textbox></td>
</tr>
<tr>
<th valign="top">Supervisor Last Name:</th>
<td valign="top" class="field"><asp:textbox id="F_SuperLastName" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top">Supervisor Email:</th>
<td valign="top" class="field"><asp:textbox id="F_SuperEmail" runat="server" Columns="50" MaxLength="100"></asp:textbox></td>
</tr>
<tr>
<th valign="top"><b>Requested Delivery:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_RequestedDeliveryLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_RequestedDeliveryUbound" runat="server" /></td>
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
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Order" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?OrderID=" & DataBinder.Eval(Container.DataItem, "OrderID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate="False" Message="Are you sure that you want to remove this Order?" runat="server" NavigateUrl= '<%# "delete.aspx?OrderID=" & DataBinder.Eval(Container.DataItem, "OrderID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="OrderNumber" DataField="OrderNumber" HeaderText="Order Number"></asp:BoundField>
		<asp:BoundField SortExpression="VendorID" DataField="VendorID" HeaderText="Vendor"></asp:BoundField>
		<asp:BoundField SortExpression="BuilderID" DataField="BuilderID" HeaderText="Builder"></asp:BoundField>
		<asp:BoundField SortExpression="ProjectID" DataField="ProjectID" HeaderText="Project"></asp:BoundField>
		<asp:BoundField SortExpression="Title" DataField="Title" HeaderText="Title"></asp:BoundField>
		<asp:BoundField DataField="PONumber" HeaderText="P O Number"></asp:BoundField>
		<asp:BoundField SortExpression="OrdererLastName" DataField="OrdererLastName" HeaderText="Orderer Last Name"></asp:BoundField>
		<asp:BoundField DataField="OrdererEmail" HeaderText="Orderer Email"></asp:BoundField>
		<asp:BoundField SortExpression="SuperLastName" DataField="SuperLastName" HeaderText="Supervisor Last Name"></asp:BoundField>
		<asp:BoundField DataField="SuperEmail" HeaderText="Supervisor Email"></asp:BoundField>
		<asp:BoundField DataField="OrderStatusID" HeaderText="Order Status I D"></asp:BoundField>
		<asp:BoundField SortExpression="RequestedDelivery" DataField="RequestedDelivery" HeaderText="Requested Delivery" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField DataField="Subtotal" HeaderText="Subtotal" DataFormatString="{0:c}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField DataField="Tax" HeaderText="Tax" DataFormatString="{0:c}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField SortExpression="Total" DataField="Total" HeaderText="Total" DataFormatString="{0:c}" HTMLEncode="False"></asp:BoundField>
	</Columns>
</CC:GridView>

</asp:content>

