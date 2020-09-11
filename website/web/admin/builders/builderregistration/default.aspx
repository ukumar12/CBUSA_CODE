<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Builder Registration" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Builder Registration Administration</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top"><b>Builder:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_BuilderID" runat="server" /></td>
</tr>
<tr>
<th valign="top"><b>Year Started:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding= cellspacing="0">
<tr><td>From<asp:textbox id="F_YearStartedLBound" runat="server" Columns="5" MaxLength="10"/></td><td>To<asp:textbox id="F_YearStartedUBound" runat="server" Columns="5" MaxLength="10"/></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Employees:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding= cellspacing="0">
<tr><td>From<asp:textbox id="F_EmployeesLBound" runat="server" Columns="5" MaxLength="10"/></td><td>To<asp:textbox id="F_EmployeesUBound" runat="server" Columns="5" MaxLength="10"/></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Homes Built And Delivered:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding= cellspacing="0">
<tr><td>From<asp:textbox id="F_HomesBuiltAndDeliveredLBound" runat="server" Columns="5" MaxLength="10"/></td><td>To<asp:textbox id="F_HomesBuiltAndDeliveredUBound" runat="server" Columns="5" MaxLength="10"/></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Home Starts Last Year:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding= cellspacing="0">
<tr><td>From<asp:textbox id="F_HomeStartsLastYearLBound" runat="server" Columns="5" MaxLength="10"/></td><td>To<asp:textbox id="F_HomeStartsLastYearUBound" runat="server" Columns="5" MaxLength="10"/></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Home Starts Next Year:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding= cellspacing="0">
<tr><td>From<asp:textbox id="F_HomeStartsNextYearLBound" runat="server" Columns="5" MaxLength="10"/></td><td>To<asp:textbox id="F_HomeStartsNextYearUBound" runat="server" Columns="5" MaxLength="10"/></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Registration Status:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_RegistrationStatusID" runat="server" /></td>
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
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Builder Registration" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?BuilderRegistrationID=" & DataBinder.Eval(Container.DataItem, "BuilderRegistrationID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate="False" Message="Are you sure that you want to remove this Builder Registration?" runat="server" NavigateUrl= '<%# "delete.aspx?BuilderRegistrationID=" & DataBinder.Eval(Container.DataItem, "BuilderRegistrationID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="CompanyName" DataField="CompanyName" HeaderText="Builder"></asp:BoundField>
		<asp:BoundField SortExpression="YearStarted" DataField="YearStarted" HeaderText="Year Started"></asp:BoundField>
		<asp:BoundField SortExpression="Employees" DataField="Employees" HeaderText="Employees"></asp:BoundField>
		<asp:BoundField SortExpression="HomesBuiltAndDelivered" DataField="HomesBuiltAndDelivered" HeaderText="Homes Built And Delivered"></asp:BoundField>
		<asp:BoundField SortExpression="HomeStartsLastYear" DataField="HomeStartsLastYear" HeaderText="Home Starts Last Year"></asp:BoundField>
		<asp:BoundField SortExpression="HomeStartsNextYear" DataField="HomeStartsNextYear" HeaderText="Home Starts Next Year"></asp:BoundField>
		<asp:BoundField SortExpression="UnsoldLastYear" DataField="UnsoldLastYear" HeaderText="Unsold Last Year"></asp:BoundField>
		<asp:BoundField SortExpression="UnderConstructionLastYear" DataField="UnderConstructionLastYear" HeaderText="Under Construction Last Year"></asp:BoundField>
		<asp:BoundField SortExpression="RegistrationStatus" DataField="RegistrationStatus" HeaderText="Registration Status"></asp:BoundField>
		<asp:BoundField SortExpression="DirectCostsLastYear" DataField="DirectCostsLastYear" HeaderText="Direct Costs Last Year" DataFormatString="{0:c}" HTMLEncode="False"></asp:BoundField>
	</Columns>
</CC:GridView>

</asp:content>

