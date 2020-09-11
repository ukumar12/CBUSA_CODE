<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Builder Monthly Data" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Builder Monthly Data Administration</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top"><b>LLC:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_LLC" runat="server" /></td>

<th valign="top"><b>Time Period:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_TimePeriodDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_TimePeriodDateUbound" runat="server" /></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Builder:</b></th>
<td valign="top" class="field" colspan="3"><asp:DropDownList ID="F_Builder" runat="server" /></td>
</tr>
<tr>
<th valign="top"><b>Starts:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding= cellspacing="0">
<tr><td>From<asp:textbox id="F_StartedUnitsLBound" runat="server" Columns="5" MaxLength="10"/></td><td>To<asp:textbox id="F_StartedUnitsUBound" runat="server" Columns="5" MaxLength="10"/></td>
</tr>
</table>
</td>

<th valign="top"><b>Sales:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding= cellspacing="0">
<tr><td>From<asp:textbox id="F_SoldUnitsLBound" runat="server" Columns="5" MaxLength="10"/></td><td>To<asp:textbox id="F_SoldUnitsUBound" runat="server" Columns="5" MaxLength="10"/></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Closings:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding= cellspacing="0">
<tr><td>From<asp:textbox id="F_ClosingUnitsLBound" runat="server" Columns="5" MaxLength="10"/></td><td>To<asp:textbox id="F_ClosingUnitsUBound" runat="server" Columns="5" MaxLength="10"/></td>
</tr>
</table>
</td>

<th valign="top"><b>60 Day Projected Starts:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding= cellspacing="0">
<tr><td>From<asp:textbox id="F_UnsoldUnitsLBound" runat="server" Columns="5" MaxLength="10"/></td><td>To<asp:textbox id="F_UnsoldUnitsUBound" runat="server" Columns="5" MaxLength="10"/></td>
</tr>
</table>
</td>
</tr>
<tr>
<td colspan="4" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
<CC:OneClickButton id="btnExport" Runat="server" Text="Export" cssClass="btn" />
<input class="btn" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />
</td>
</tr>
</table>
</asp:Panel>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="False" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:BoundField SortExpression="LLC" DataField="LLC" HeaderText="LLC"></asp:BoundField>
		<asp:BoundField SortExpression="Builder" DataField="Builder" HeaderText="Builder"></asp:BoundField>
		<asp:BoundField SortExpression="StartedUnits" DataField="StartedUnits" HeaderText="Starts"></asp:BoundField>
		<asp:BoundField SortExpression="SoldUnits" DataField="SoldUnits" HeaderText="Sales"></asp:BoundField>
		<asp:BoundField SortExpression="ClosingUnits" DataField="ClosingUnits" HeaderText="Closings"></asp:BoundField>
		<asp:BoundField SortExpression="UnsoldUnits" DataField="UnsoldUnits" HeaderText="60 Day Projected Starts"></asp:BoundField>
		<asp:BoundField SortExpression="TimePeriodDate" DataField="TimePeriodDate" HeaderText="Time Period" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
	</Columns>
</CC:GridView>

</asp:content>

