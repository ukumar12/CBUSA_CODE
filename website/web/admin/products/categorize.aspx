<%@ Page Language="VB" AutoEventWireup="false" CodeFile="categorize.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Product"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
<asp:ScriptManager ID="AjaxManager" runat="server" EnablePartialRendering="true" EnablePageMethods="true"></asp:ScriptManager>
	
<h4>Categorize Products</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
	    <td class="required">Supply Phase(s):</td>
	    <td class="field"><CC:CustomTreeView ID="ctvSupplyPhase" runat="server" Type="Checkbox"></CC:CustomTreeView></td>
	</tr>
</table>

<h5>Selected Products</h5>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:CheckBox ID="chkSelected" runat="server" />
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="Product" DataField="Product" HeaderText="Name"></asp:BoundField>
		<asp:BoundField SortExpression="SKU" DataField="SKU" HeaderText="CBUSA SKU"></asp:BoundField>
		<asp:BoundField SortExpression="Manufacturer" DataField="Manufacturer" HeaderText="Manufacturer"></asp:BoundField>
		<asp:BoundField SortExpression="Created" DataField="Created" HeaderText="Created" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:ImageField SortExpression="IsActive" DataImageUrlField="IsActive" HeaderText="Is Active" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
	</Columns>
</CC:GridView>

<CC:OneClickButton ID="btnUp" runat="server" Text="Up" CssClass="btn" />
<CC:OneClickButton ID="btnDown" runat="server" Text="Down" CssClass="btn" />

<h5>Unselected Products</h5>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top">Name:</th>
<td valign="top" class="field"><asp:textbox id="F_Product" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top">CBUSA SKU:</th>
<td valign="top" class="field"><asp:textbox id="F_SKU" runat="server" Columns="20" MaxLength="20"></asp:textbox></td>
</tr>
<tr>
<th valign="top"><b>Manufacturer:</b></th>
<td valign="top" class="field">
<CC:AutoComplete ID="F_Manufacturer" runat="server" Table="Manufacturer" TextField="Manufacturer" ValueField="ManufacturerId" AllowNew="false" CssClass="aclist"></CC:AutoComplete>
</td>
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
</td>
</tr>
</table>
</asp:Panel>

<CC:GridView id="gvList2" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:CheckBox ID="chkSelected" runat="server" />
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="Product" DataField="Product" HeaderText="Name"></asp:BoundField>
		<asp:BoundField SortExpression="SKU" DataField="SKU" HeaderText="CBUSA SKU"></asp:BoundField>
		<asp:BoundField SortExpression="Manufacturer" DataField="Manufacturer" HeaderText="Manufacturer"></asp:BoundField>
		<asp:BoundField SortExpression="Created" DataField="Created" HeaderText="Created" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:ImageField SortExpression="IsActive" DataImageUrlField="IsActive" HeaderText="Is Active" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
	</Columns>
</CC:GridView>

<p></p>


<CC:OneClickButton ID="Categorize" runat="server" Text="Apply Categories" CssClass="btn" />

</asp:content>

