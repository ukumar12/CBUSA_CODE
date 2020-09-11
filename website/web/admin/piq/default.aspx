<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="PIQ" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>PIQ Administration</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top">Company Name:</th>
<td valign="top" class="field"><asp:textbox id="F_CompanyName" runat="server" Columns="40" MaxLength="50"></asp:textbox></td>
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
<th valign="top">City:</th>
<td valign="top" class="field"><asp:textbox id="F_City" runat="server" Columns="40" MaxLength="50"></asp:textbox></td>
<th valign="top"><b>State:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_State" runat="server" /></td>
</tr>
<tr>
<th valign="top">Contact Last Name:</th>
<td valign="top" class="field"><asp:textbox id="F_LastName" runat="server" Columns="40" MaxLength="50"></asp:textbox></td>
<th valign="top">Contact Email:</th>
<td valign="top" class="field"><asp:textbox id="F_Email" runat="server" Columns="40" MaxLength="100"></asp:textbox></td>
</tr>

<tr>
<th valign="top"><b>Start Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_StartDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_StartDateUbound" runat="server" /></td>
</tr>
</table>
</td>

<th valign="top"><b>End Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_EndDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_EndDateUbound" runat="server" /></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Catalog Access:</b></th>
<td valign="top" class="field" colspan="3">
	<asp:DropDownList ID="F_HasCatalogAccess" runat="server">
		<asp:ListItem Value="">-- ALL --</asp:ListItem>
		<asp:ListItem Value="1">Yes</asp:ListItem>
		<asp:ListItem Value="0">No</asp:ListItem>
	</asp:DropDownList>
</td>
</tr>
<tr>
<td colspan="4" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
<input class="btn" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />
</td>
</tr>
</table>
</asp:Panel>
<p></p>
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New PIQ" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?PIQID=" & DataBinder.Eval(Container.DataItem, "PIQID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate="False" Message="Are you sure that you want to remove this PIQ?" runat="server" NavigateUrl= '<%# "delete.aspx?PIQID=" & DataBinder.Eval(Container.DataItem, "PIQID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "/admin/piq/ads/default.aspx?PIQID=" & DataBinder.Eval(Container.DataItem, "PIQID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/collection.gif" ID="lnkAds">PIQ Ads</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
		    <ItemTemplate>
            <asp:HyperLink EnableViewState="false" runat="server" NavigateUrl='<%# "/admin/piq/accounts/default.aspx?F_PIQID="& DataBinder.Eval(Container.DataItem,"PIQID") & "&" & GetPageParams(Components.FilterFieldType.All,"F_SortBy") %>' ImageUrl="/images/admin/person-computer.gif" ID="lnkAccounts">PIQ Accounts</asp:HyperLink>
		    </ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="CompanyName" DataField="CompanyName" HeaderText="Company Name"></asp:BoundField>
		<asp:BoundField DataField="City" HeaderText="City"></asp:BoundField>
		<asp:BoundField DataField="State" HeaderText="State"></asp:BoundField>
		<asp:BoundField SortExpression="LastName" DataField="LastName" HeaderText="Contact Last Name"></asp:BoundField>
		<asp:BoundField DataField="Email" HeaderText="Contact Email"></asp:BoundField>
		<asp:BoundField SortExpression="StartDate" DataField="StartDate" HeaderText="Start Date" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField SortExpression="EndDate" DataField="EndDate" HeaderText="End Date" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:ImageField DataImageUrlField="IsActive" HeaderText="Is Active" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
		<asp:ImageField DataImageUrlField="HasCatalogAccess" HeaderText="Catalog Access" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
	</Columns>
</CC:GridView>

</asp:content>
