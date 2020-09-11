<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Country" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Country</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top">Country Code:</th>
<td valign="top" class="field"><asp:textbox id="F_CountryCode" runat="server" Columns="2" MaxLength="2"></asp:textbox></td>
</tr>
<tr>
<th valign="top">Country Name:</th>
<td valign="top" class="field"><asp:textbox id="F_CountryName" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top"><b>Shipping:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td>From<asp:textbox id="F_ShippingLBound" runat="server" Columns="5" MaxLength="10"/></td><td>To<asp:textbox id="F_ShippingUbound" runat="server" Columns="5" MaxLength="10"/></td>
</tr>
</table>
</td>
<td>
<CC:FloatValidator runat="server" ID="fvShippingLbound" Display="Dynamic" ControlToValidate="F_ShippingLbound" ErrorMessage="Field 'Shipping From' is invalid" ></CC:FloatValidator><br />
<CC:FloatValidator runat="server" ID="fvShippingUbound" Display="Dynamic" ControlToValidate="F_ShippingUbound" ErrorMessage="Field 'Shipping to To' is invalid" ></CC:FloatValidator>
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

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "edit.aspx?CountryId=" & DataBinder.Eval(Container.DataItem, "CountryId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="CountryCode" DataField="CountryCode" HeaderText="Country Code"></asp:BoundField>
		<asp:BoundField SortExpression="CountryName" DataField="CountryName" HeaderText="Country Name"></asp:BoundField>
		<asp:BoundField SortExpression="Shipping" DataField="Shipping" HTMLEncode="False" DataFormatString="{0:c}" HeaderText="Shipping"></asp:BoundField>
	</Columns>
</CC:GridView>

</asp:content>

