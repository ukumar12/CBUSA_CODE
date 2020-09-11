<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Mailing List" CodeFile="default.aspx.vb" Inherits="index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Mailing List</h4>

<p></p>
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Mailing List" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate"></AlternatingRowStyle>
	<RowStyle CssClass="row"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "edit.aspx?ListId=" & DataBinder.Eval(Container.DataItem, "ListId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit" visible='<%# Not CBool(DataBinder.Eval(Container.DataItem, "IsPermanent")) %>'>Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate=False Message="Are you sure that you want to remove this Mailing List?" runat="server" NavigateUrl= '<%# "delete.aspx?ListId=" & DataBinder.Eval(Container.DataItem, "ListId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete" visible='<%# Not CBool(DataBinder.Eval(Container.DataItem, "IsPermanent")) %>'>Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="Name" DataField="Name" HeaderText="Name"></asp:BoundField>
		<asp:TemplateField>
			<ItemTemplate>
			<input type="button" class="btnsmall" enableviewstate="False" runat="server" id="btnSubscribers" />
			</ItemTemplate>
		</asp:TemplateField>
	</Columns>
</CC:GridView>

</asp:content>
