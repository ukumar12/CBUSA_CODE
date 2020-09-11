﻿<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="LLC" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>LLC Administration</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top">LLC:</th>
<td valign="top" class="field"><asp:textbox id="F_LLC" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
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
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New LLC" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?LLCID=" & DataBinder.Eval(Container.DataItem, "LLCID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate="False" Message="Are you sure that you want to remove this LLC?" runat="server" NavigateUrl= '<%# "delete.aspx?LLCID=" & DataBinder.Eval(Container.DataItem, "LLCID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="LLC" DataField="LLC" HeaderText="LLC"></asp:BoundField>
        <asp:BoundField SortExpression="BuilderGroup" DataField="BuilderGroup" HeaderText="BuilderGroup"></asp:BoundField>
         <asp:BoundField SortExpression="OperationsManager" DataField="OperationsManager" HeaderText="OperationsManager"></asp:BoundField>
		<asp:BoundField SortExpression="Created" DataField="Created" HeaderText="Created" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:ImageField DataImageUrlField="IsActive" HeaderText="Is Active" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
        <asp:ImageField DataImageUrlField="AllowExcludingVendors" HeaderText="Allow Excluding From Bids" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
	</Columns>
</CC:GridView>

</asp:content>
