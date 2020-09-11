<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Content Items" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Content Items Administration</h4>

<p></p>
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Content Items" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?ContentItemsID=" & DataBinder.Eval(Container.DataItem, "ContentItemsID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate="False" Message="Are you sure that you want to remove this Content Items?" runat="server" NavigateUrl= '<%# "delete.aspx?ContentItemsID=" & DataBinder.Eval(Container.DataItem, "ContentItemsID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
          <asp:BoundField SortExpression="Title" DataField="Title" HeaderText="Title"></asp:BoundField>
		    <asp:BoundField SortExpression="FileName" DataField="FileName" HeaderText="FileName"></asp:BoundField>
		   <asp:hyperlinkfield Text="Click to View" SortExpression="FileName" datanavigateurlfields="FileName" datanavigateurlformatstring="/assets/contentitems/{0}" navigateurl="/" headertext="File" target="_blank" />
          	<asp:TemplateField HeaderText="URL">
		    <ItemTemplate>
                <asp:Literal ID="ltlURL" runat="server"></asp:Literal>
		    </ItemTemplate>
		</asp:TemplateField>
            <asp:BoundField SortExpression="Uploaded" DataField="Uploaded" HeaderText="Uploaded"></asp:BoundField>

	</Columns>
</CC:GridView>

</asp:content>
