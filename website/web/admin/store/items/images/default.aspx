<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Item Image" CodeFile="default.aspx.vb" Inherits="Index"  %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Additional Item Images</h4>

<p>
Images for <b><% =dbStoreItem.ItemName%></b> | <a href="/admin/store/items/default.aspx?<%= GetPageParams(Components.FilterFieldType.All,"F_ItemId") %>">&laquo; Go Back To Store Item List</a>
</p>

<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Alternate Image" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use arrows" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "edit.aspx?ImageId=" & DataBinder.Eval(Container.DataItem, "ImageId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate=False Message="Are you sure that you want to remove this Item Image?" runat="server" NavigateUrl= '<%# "delete.aspx?ImageId=" & DataBinder.Eval(Container.DataItem, "ImageId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:Templatefield>
            <ItemTemplate>
		    <img alt="<%# DataBinder.Eval(Container.DataItem, "ImageAltTag")%>" src="/assets/item/alternate/<%# DataBinder.Eval(Container.DataItem, "Image")%>" border="0" />
		    </ItemTemplate>
		</asp:Templatefield>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "move.aspx?ACTION=UP&ImageId=" & DataBinder.Eval(Container.DataItem, "ImageId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/moveup.gif" ID="lnkMoveUp">Move Up</asp:HyperLink>
		    </ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "move.aspx?ACTION=DOWN&ImageId=" & DataBinder.Eval(Container.DataItem, "ImageId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/movedown.gif" ID="lnkMoveDown">Move Down</asp:HyperLink>
		    </ItemTemplate>
		</asp:TemplateField>
	</Columns>
</CC:GridView>

</asp:content>

