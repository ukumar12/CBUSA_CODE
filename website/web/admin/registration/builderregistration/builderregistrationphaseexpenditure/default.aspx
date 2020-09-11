<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Builder Registration Phase Expenditure" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Builder Registration Phase Expenditure Administration</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top"><b>Supply Phase ID:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_SupplyPhaseID" runat="server" /></td>
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
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Builder Registration Phase Expenditure" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?BuilderRegistrationPhaseExpenditureID=" & DataBinder.Eval(Container.DataItem, "BuilderRegistrationPhaseExpenditureID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate="False" Message="Are you sure that you want to remove this Builder Registration Phase Expenditure?" runat="server" NavigateUrl= '<%# "delete.aspx?BuilderRegistrationPhaseExpenditureID=" & DataBinder.Eval(Container.DataItem, "BuilderRegistrationPhaseExpenditureID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="SupplyPhaseID" DataField="SupplyPhaseID" HeaderText="Supply Phase ID"></asp:BoundField>
		<asp:BoundField SortExpression="AmountSpentLastYear" DataField="AmountSpentLastYear" HeaderText="Amount Spent Last Year" DataFormatString="{0:c}" HTMLEncode="False"></asp:BoundField>
	</Columns>
</CC:GridView>

</asp:content>
