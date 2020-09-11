<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Swatches" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Swatches Administration</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top">Name:</th>
<td valign="top" class="field"><asp:textbox id="F_Name" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top">SKU:</th>
<td valign="top" class="field"><asp:textbox id="F_SKU" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top"><b>Price:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td>From<asp:textbox id="F_PriceLBound" runat="server" Columns="5" MaxLength="10"/></td><td>To<asp:textbox id="F_PriceUbound" runat="server" Columns="5" MaxLength="10"/>
</tr>
</table>
</td>
<td><CC:FloatValidator runat="server" ID="fvPriceLbound" ControlToValidate="F_PriceLbound" Display="Dynamic" ErrorMessage="Field 'Price From' must be numeric" ></CC:FloatValidator><br />
<CC:FloatValidator runat="server" ID="fvPriceUbound" ControlToValidate="F_PriceUbound" Display="Dynamic" ErrorMessage="Field 'Price To' must be numeric" ></CC:FloatValidator>
</td>
</tr>
<tr>
<th valign="top"><b>Weight:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td>From<asp:textbox id="F_WeightLBound" runat="server" Columns="5" MaxLength="10"/></td><td>To<asp:textbox id="F_WeightUbound" runat="server" Columns="5" MaxLength="10"/></td>
</tr>
</table>
</td>
<td><CC:FloatValidator runat="server" ID="fvWeightLbound" ControlToValidate="F_WeightLbound" Display="Dynamic" ErrorMessage="Field 'Weight From' must be numeric" ></CC:FloatValidator><br />
<CC:FloatValidator runat="server" ID="fvWeightUbound" ControlToValidate="F_WeightUbound" Display="Dynamic" ErrorMessage="Field 'Weight To' must be numeric" ></CC:FloatValidator>
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
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Swatch" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?SwatchId=" & DataBinder.Eval(Container.DataItem, "SwatchId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate="False" Message="Are you sure that you want to remove this Swatch?" runat="server" NavigateUrl= '<%# "delete.aspx?SwatchId=" & DataBinder.Eval(Container.DataItem, "SwatchId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="Name" DataField="Name" HeaderText="Name"></asp:BoundField>
		<asp:BoundField SortExpression="SKU" DataField="SKU" HeaderText="SKU"></asp:BoundField>
		<asp:BoundField SortExpression="Price" DataField="Price" HeaderText="Price"></asp:BoundField>
		<asp:BoundField SortExpression="Weight" DataField="Weight" HeaderText="Weight"></asp:BoundField>
		<asp:ImageField HeaderText="Image" DataImageUrlField="Image" DataImageUrlFormatString="/assets/item/swatch/{0}"></asp:ImageField> 
	</Columns>
</CC:GridView>

</asp:content>

