<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Item Template" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Item Template</h4>

<p></p>
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Item Template" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "edit.aspx?TemplateId=" & DataBinder.Eval(Container.DataItem, "TemplateId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink Visible='<%#DB.ExecuteScalar("select top 1 itemid from storeitem where templateid = " & Container.DataItem("TemplateId")) = Nothing%>' enableviewstate=False Message="Are you sure that you want to remove this Item Template?" runat="server" NavigateUrl= '<%# "delete.aspx?TemplateId=" & DataBinder.Eval(Container.DataItem, "TemplateId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			    <CC:OneClickButton Visible='<%#CBool(Container.DataItem("IsAttributes"))%>' id="btnAttributes" CommandName="Attributes" runat="server" Text="Attributes" CssClass="btn" />
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="TemplateName" DataField="TemplateName" HeaderText="Template Name"></asp:BoundField>
		<asp:Checkboxfield ItemStyle-HorizontalAlign="Center" SortExpression="IsAttributes" DataField="IsAttributes" HeaderText="Is Attributes"/>
		<asp:BoundField SortExpression="DisplayMode" DataField="DisplayMode" HeaderText="Display Mode"/>
		<asp:Checkboxfield Visible="false" ItemStyle-HorizontalAlign="Center" SortExpression="IsToAndFrom" DataField="IsToAndFrom" HeaderText="Is To And From"/>
		<asp:Checkboxfield Visible="false" ItemStyle-HorizontalAlign="Center" SortExpression="IsGiftMessage" DataField="IsGiftMessage" HeaderText="Is Gift Message"/>
	</Columns>
</CC:GridView>

</asp:content>
