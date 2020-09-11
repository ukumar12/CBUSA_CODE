<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Shipping Method" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Shipping Methods</h4>

<p></p>
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Shipping Method" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use arrows" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "edit.aspx?MethodId=" & DataBinder.Eval(Container.DataItem, "MethodId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate=False Message="Are you sure that you want to remove this Shipping Method?" runat="server" NavigateUrl= '<%# "delete.aspx?MethodId=" & DataBinder.Eval(Container.DataItem, "MethodId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField DataField="Name" HeaderText="Name"></asp:BoundField>
		<asp:BoundField DataField="UPSCode" HeaderText="UPS Code"></asp:BoundField>
		<asp:BoundField DataField="FedExCode" HeaderText="FedEx Code"></asp:BoundField>
		<asp:ImageField SortExpression="IsActive" DataImageUrlField="IsActive" HeaderText="Is Active" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
		<asp:ImageField SortExpression="IsInternational" DataImageUrlField="IsInternational" HeaderText="Is International" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "move.aspx?ACTION=UP&MethodId=" & DataBinder.Eval(Container.DataItem, "MethodId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/moveup.gif" ID="lnkMoveUp">Move Up</asp:HyperLink>
		    </ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "move.aspx?ACTION=DOWN&MethodId=" & DataBinder.Eval(Container.DataItem, "MethodId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/movedown.gif" ID="lnkMoveDown">Move Down</asp:HyperLink>
		    </ItemTemplate>
		</asp:TemplateField>
	</Columns>
</CC:GridView>

</asp:content>

