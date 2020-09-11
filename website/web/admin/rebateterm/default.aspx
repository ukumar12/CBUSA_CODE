<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Rebate Term" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Rebate Term Administration</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top">Historic ID:</th>
<td valign="top" class="field"><asp:textbox id="F_HistoricId" runat="server" Columns="50" MaxLength="5" TextMode="Number"></asp:textbox></td>
</tr>
<tr>
<th valign="top"><b>Vendor:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_VendorID" runat="server" /></td>
</tr>
<tr>
<th valign="top"><b>Start Year:</b></th>
<td valign="top" class="field"><asp:textbox ID="F_StartYear" runat="server" /></td>
</tr>
<tr>
<th valign="top"><b>Start Quarter:</b></th>
<td valign="top" class="field"><asp:textbox ID="F_StartQuarter" runat="server" /></td>
</tr>
<tr>
	<th valign="top">LLC(s):</th>
	<td class="field"><CC:CheckBoxListEx ID="F_LLC" runat="server" RepeatColumns="3"></CC:CheckBoxListEx></td>
</tr>
<tr>
<td colspan="2" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
<input class="btn" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />
</td>
</tr>

<tr>
<td colspan="2" align="right">
<asp:button ID="Export" runat="server" Text="Export " CssClass="btn" />
</td> 

</tr>
</table>
</asp:Panel>
<p></p>
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Rebate Term" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom" DataKeyNames ="VendorID">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?RebateTermsID=" & DataBinder.Eval(Container.DataItem, "RebateTermsID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate="False" Message="Are you sure that you want to remove this Rebate Term?" runat="server" NavigateUrl= '<%# "delete.aspx?RebateTermsID=" & DataBinder.Eval(Container.DataItem, "RebateTermsID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="Vendor" DataField="Vendor" HeaderText="Vendor"></asp:BoundField>
		<asp:BoundField SortExpression="VendorHistoricId" DataField="VendorHistoricId" HeaderText="VendorHistoricId"></asp:BoundField>
		<asp:BoundField SortExpression="StartYear" DataField="StartYear" HeaderText="Start Year"></asp:BoundField>
		<asp:BoundField SortExpression="StartQuarter" DataField="StartQuarter" HeaderText="Start Quarter"></asp:BoundField>
		<asp:BoundField DataField="RebatePercentage" HeaderText="Rebate Percentage"></asp:BoundField>
		<asp:BoundField DataField="Created" HeaderText="Created" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
         <asp:TemplateField HeaderText = "Market(LLC)" >
        <ItemTemplate >
        <asp:Literal ID="LLCName" runat ="server" ></asp:Literal>
        </ItemTemplate>
        </asp:TemplateField>
		<asp:BoundField DataField="LogMsg" HeaderText="Log" HTMLEncode="False"></asp:BoundField>
	</Columns>
</CC:GridView>

</asp:content>
