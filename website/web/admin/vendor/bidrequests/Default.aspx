<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="P O Quote Request" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>P O Quote Request Administration</h4>

<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="4" class="AdminSearchFieldContainer">
<tr>
<th valign="top"><b>Quote Id:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_QuoteId" runat="server" /></td>
</tr>
<tr>
<th valign="top"><b>Vendor Id:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_VendorId" runat="server" /></td>
</tr>
<tr>
<th valign="top">Request Status:</th>
<td valign="top" class="field"><asp:textbox id="F_RequestStatus" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top">Vendor Contact Name:</th>
<td valign="top" class="field"><asp:textbox id="F_VendorContactName" runat="server" Columns="50" MaxLength="100"></asp:textbox></td>
</tr>
<tr>
<th valign="top">Vendor Contact Email:</th>
<td valign="top" class="field"><asp:textbox id="F_VendorContactEmail" runat="server" Columns="50" MaxLength="100"></asp:textbox></td>
</tr>
<tr>
<th valign="top"><b>Create Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_CreateDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_CreateDateUbound" runat="server" /></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Modify Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_ModifyDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_ModifyDateUbound" runat="server" /></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Start Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_StartDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_StartDateUbound" runat="server" /></td>
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
<table cellpadding="0" cellspacing="0" border="0" style="width:98%;">
	<tr id="trSort" runat="server">
        <td>
			<table cellpadding="0" cellspacing="0" border="0" style="width:98%;">
				<tr>
					<td><span class="softest italic">Currently Sorted By:</span> <asp:Literal ID="ltlSortBy" runat="server"></asp:Literal></td>
			        <td style="text-align:right;padding-bottom:2px;"><b>Sort by</b> 
			        <asp:dropdownlist ID="F_Sort" runat="server" AutoPostBack="true">
													<asp:ListItem Value="VendorId">Vendor Id</asp:ListItem>
													<asp:ListItem Value="RequestStatus">Request Status</asp:ListItem>
													<asp:ListItem Value="VendorContactName">Vendor Contact Name</asp:ListItem>
													<asp:ListItem Value="VendorContactEmail">Vendor Contact Email</asp:ListItem>
													<asp:ListItem Value="CreateDate">Create Date</asp:ListItem>
													<asp:ListItem Value="ModifyDate">Modify Date</asp:ListItem>
													<asp:ListItem Value="StartDate">Start Date</asp:ListItem>
									            
			        </asp:dropdownlist>
			        <asp:LinkButton id="lnkSortOrder" runat="server" style="text-decoration:none;"><img src="/images/admin/sort_asc.gif" border="0"  align="absmiddle" /> Descend</asp:LinkButton>
			        </td>
				</tr>
			</table>				
		</td>
	</tr>
<tr>
	<td style="width:100%">
<CC:GridView id="gvList" CellPadding="5" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use Sort By drop down" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom" CssClass="MultilineTable" gridlines="none">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemStyle CssClass="ActionButtons" width="50" />
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?QuoteRequestId=" & DataBinder.Eval(Container.DataItem, "QuoteRequestId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			<CC:ConfirmImageButton CommandName="Remove" Message="Are you sure that you want to remove this P O Quote Request?" runat="server" ImageUrl="/images/admin/delete.gif" ID="lnkDelete"></CC:ConfirmImageButton>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField SortExpression="Builder" HeaderText="Builder">
		<ItemStyle width="150" />
			<ItemTemplate>
			    <asp:literal id="ltlContact" runat="server"></asp:literal>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField SortExpression="Project" HeaderText="Project">
		<ItemStyle />
			<ItemTemplate>
			    <asp:literal id="ltlProject" runat="server"></asp:literal>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField SortExpression="Quote" HeaderText="Bid Request">
		<ItemStyle />
			<ItemTemplate>
			    <asp:literal id="ltlQuote" runat="server"></asp:literal>
			</ItemTemplate>
		</asp:TemplateField>		
			
		<asp:BoundField DataField="QuoteId" HeaderText="Quote Id"></asp:BoundField>
		<asp:BoundField DataField="VendorId" HeaderText="Vendor Id"></asp:BoundField>
		<asp:BoundField DataField="RequestStatus" HeaderText="Request Status"></asp:BoundField>
		<asp:BoundField DataField="VendorContactName" HeaderText="Vendor Contact Name"></asp:BoundField>
		<asp:BoundField DataField="VendorContactEmail" HeaderText="Vendor Contact Email"></asp:BoundField>
		<asp:BoundField DataField="CreateDate" HeaderText="Create Date" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField DataField="ModifyDate" HeaderText="Modify Date" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField DataField="StartDate" HeaderText="Start Date" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
	</Columns>
</CC:GridView>
</td></tr></table>
</asp:content>