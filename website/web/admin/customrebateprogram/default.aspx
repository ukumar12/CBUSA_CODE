<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Custom Rebate Program" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Custom Rebate Program Administration</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top"><b>Vendor I D:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_VendorID" runat="server" /></td>
</tr>
<tr>
<th valign="top">Program Name:</th>
<td valign="top" class="field"><asp:textbox id="F_ProgramName" runat="server" Columns="50" MaxLength="100"></asp:textbox></td>
</tr>
<tr>
<th valign="top"><b>Rebate Year:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding= cellspacing="0">
<tr><td>From<asp:textbox id="F_RebateYearLBound" runat="server" Columns="5" MaxLength="10"/></td><td>To<asp:textbox id="F_RebateYearUBound" runat="server" Columns="5" MaxLength="10"/></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Rebate Quarter:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding= cellspacing="0">
<tr><td>From<asp:textbox id="F_RebateQuarterLBound" runat="server" Columns="5" MaxLength="10"/></td><td>To<asp:textbox id="F_RebateQuarterUBound" runat="server" Columns="5" MaxLength="10"/></td>
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
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Custom Rebate Program" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?CustomRebateProgramID=" & DataBinder.Eval(Container.DataItem, "CustomRebateProgramID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate="False" Message="Are you sure that you want to remove this Custom Rebate Program?" runat="server" NavigateUrl= '<%# "delete.aspx?CustomRebateProgramID=" & DataBinder.Eval(Container.DataItem, "CustomRebateProgramID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="VendorID" DataField="VendorID" HeaderText="Vendor I D"></asp:BoundField>
		<asp:BoundField SortExpression="ProgramName" DataField="ProgramName" HeaderText="Program Name"></asp:BoundField>
		<asp:BoundField SortExpression="RebateYear" DataField="RebateYear" HeaderText="Rebate Year"></asp:BoundField>
		<asp:BoundField SortExpression="RebateQuarter" DataField="RebateQuarter" HeaderText="Rebate Quarter"></asp:BoundField>
		<asp:BoundField DataField="RebatePercentage" HeaderText="Rebate Percentage"></asp:BoundField>
		<asp:BoundField DataField="MinimumPurchase" HeaderText="Minimum Purchase" DataFormatString="{0:c}" HTMLEncode="False"></asp:BoundField>
	</Columns>
</CC:GridView>

</asp:content>
