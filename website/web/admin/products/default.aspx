<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Product" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
<asp:ScriptManager ID="AjaxManager" runat="server" EnablePartialRendering="true" EnablePageMethods="true"></asp:ScriptManager>

<h4>Product Administration</h4>

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
	    <td class="required">Supply Phase(s):</td>
	    <td class="field"><CC:CustomTreeView ID="ctvSupplyPhase"   runat="server"     Type="Checkbox"></CC:CustomTreeView></td>
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
	<asp:DropDownList ID="F_IsActive" runat="server" >
		<asp:ListItem Value="1" Selected  ="True" >Yes</asp:ListItem>
        
		<asp:ListItem Value="0">No</asp:ListItem>
        <asp:ListItem Value="">ALL </asp:ListItem>
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
</asp:Panel>
<p></p>
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Product" cssClass="btn"></CC:OneClickButton>
<CC:OneClickButton ID="Categorize" runat="server" Text="Batch Categorize" CssClass="btn" />
<asp:button ID="Export" runat="server" Text="Export" CssClass="btn" />

<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?ProductID=" & DataBinder.Eval(Container.DataItem, "ProductID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate="False" Message="Are you sure that you want to remove this Product?" runat="server" NavigateUrl= '<%# "delete.aspx?ProductID=" & DataBinder.Eval(Container.DataItem, "ProductID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="Product" DataField="Product" HeaderText="Name"></asp:BoundField>
		<asp:BoundField SortExpression="SKU" DataField="SKU" HeaderText="CBUSA SKU"></asp:BoundField>
		<asp:BoundField SortExpression="Manufacturer" DataField="Manufacturer" HeaderText="Manufacturer"></asp:BoundField>
        <asp:BoundField SortExpression="Description" DataField="Description" HeaderText="Description" Visible ="false" ></asp:BoundField>
        <asp:BoundField SortExpression="Created" DataField="Created" HeaderText="Created" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:ImageField SortExpression="IsActive" DataImageUrlField="IsActive" HeaderText="Is Active" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
	</Columns>
</CC:GridView>

</asp:content>

