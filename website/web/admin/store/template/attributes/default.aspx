<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Item Template Attribute" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4><asp:literal ID="ltlTemplateName" runat="server" /> Template Attributes</h4>

<a href="/admin/store/template/">&laquo; Return to template listing</a>
<p></p>
<CC:OneClickButton id="AddNew" Runat="server" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use arrows" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "edit.aspx?TemplateId=" & Eval("TemplateId") & "&TemplateAttributeId=" & DataBinder.Eval(Container.DataItem, "TemplateAttributeId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate=False Message="Are you sure that you want to remove this Item Template Attribute?" runat="server" NavigateUrl= '<%# "delete.aspx?TemplateId=" & Eval("TemplateId") & "&TemplateAttributeId=" & DataBinder.Eval(Container.DataItem, "TemplateAttributeId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField DataField="AttributeName" HeaderText="Attribute Name"></asp:BoundField>
		<asp:CheckBoxField DataField="IsInventoryManagement" HeaderText="Inventory" ItemStyle-HorizontalAlign="center"></asp:CheckBoxField>
		<asp:BoundField DataField="Parent" HeaderText="Parent"></asp:BoundField>
		<asp:BoundField DataField="AttributeType" HeaderText="Attribute Type"></asp:BoundField>
		<asp:BoundField DataField="FunctionType" HeaderText="Function Type"></asp:BoundField>
	</Columns>
</CC:GridView>

</asp:content>
