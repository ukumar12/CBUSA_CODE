<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Store Promotion" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Store Promotion</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="Panel1"  DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top">Promotion Name:</th>
<td valign="top" class="field"><asp:textbox id="F_PromotionName" runat="server" Columns="50" MaxLength="255"></asp:textbox></td>
</tr>
<tr>
<th valign="top">Promotion Code:</th>
<td valign="top" class="field"><asp:textbox id="F_PromotionCode" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top">Promotion Type:</th>
<td valign="top" class="field">
  <asp:DropDownList ID="F_drpPromotionType" runat="server">
  <asp:ListItem value="">ALL</asp:ListItem>
  <asp:ListItem Value="Percent">Percent Off</asp:ListItem>
  <asp:ListItem Value="Monetary">Dollar Off</asp:ListItem>
  </asp:DropDownList>
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
<td><CC:DateValidator ID="vStartDateLBound" runat="server" Display="Dynamic" ControlToValidate="F_StartDateLbound" ErrorMessage="Invalid 'Start Date From'" /><br />
<CC:DateValidator ID="vStartDateUBound" runat="server" Display="Dynamic" ControlToValidate="F_StartDateUbound" ErrorMessage="Invalid 'Start Date To'" /></td>
</tr>
<tr>
<th valign="top"><b>End Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_EndDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_EndDateUbound" runat="server" /></td>
</tr>
</table>
</td>
<td><CC:DateValidator ID="vEndDateLBound" runat="server" Display="Dynamic" ControlToValidate="F_EndDateLbound" ErrorMessage="Invalid 'End Date From '" /><br />
<CC:DateValidator ID="vEndDateUBound" runat="server" Display="Dynamic" ControlToValidate="F_EndDateUbound" ErrorMessage="Invalid 'End Date To'" /></td>
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
<td colspan="2" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
<input class="btn" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />
</td>
</tr>
</table>
<p></p>
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Store Promotion" cssClass="btn"></CC:OneClickButton>
</asp:Panel>

<p></p>
<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "edit.aspx?PromotionId=" & DataBinder.Eval(Container.DataItem, "PromotionId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate=False Message="Are you sure that you want to remove this Store Promotion?" runat="server" NavigateUrl= '<%# "delete.aspx?PromotionId=" & DataBinder.Eval(Container.DataItem, "PromotionId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			    <CC:OneClickButton id="btnRelatedItems" runat="server" CssClass="btn" Text="Items/Departments" />
			</ItemTemplate>
		</asp:TemplateField>
		
		<asp:BoundField SortExpression="PromotionName" DataField="PromotionName" HeaderText="Promotion Name"></asp:BoundField>
		<asp:BoundField SortExpression="PromotionCode" DataField="PromotionCode" HeaderText="Promotion Code"></asp:BoundField>
		<asp:BoundField SortExpression="PromotionType" DataField="PromotionType" HeaderText="Promotion Type"></asp:BoundField>
		<asp:TemplateField SortExpression="Discount" HeaderText="Discount">
			<ItemTemplate>
			    <asp:literal id="ltlDiscount" runat="server" />
			</ItemTemplate>
		</asp:TemplateField>

		<asp:BoundField SortExpression="StartDate" DataField="StartDate" HeaderText="Start Date" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField SortExpression="EndDate" DataField="EndDate" HeaderText="End Date" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:ImageField SortExpression="IsActive" DataImageUrlField="IsActive" HeaderText="Is Active" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
	</Columns>
</CC:GridView>

</asp:content>
