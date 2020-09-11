<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Mailing Template Slot" CodeFile="default.aspx.vb" Inherits="index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Mailing Template Slot</h4>

Template Slots for <b><% =dbTemplate.Name%></b> | <a href="/admin/broadcast/templates/default.aspx?<%= GetPageParams(Components.FilterFieldType.All,"F_SortBy;F_SortOrder") %>">&laquo; Go Back To Template List</a>

<p></p>
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Mailing Template Slot" cssClass="btn"></CC:OneClickButton>

<p></p>
<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use arrows" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "edit.aspx?SlotId=" & DataBinder.Eval(Container.DataItem, "SlotId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate=False Message="Are you sure that you want to remove this Mailing Template Slot?" runat="server" NavigateUrl= '<%# "delete.aspx?SlotId=" & DataBinder.Eval(Container.DataItem, "SlotId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField DataField="Name" HeaderText="Template"></asp:BoundField>
		<asp:BoundField DataField="SlotName" HeaderText="Slot Name"></asp:BoundField>
				<asp:TemplateField><ItemTemplate>
            <asp:Image runat="server" ImageUrl = '<%# "/assets/broadcast/templates/slots/" & DataBinder.Eval(Container.DataItem, "ImageName")%>'>
            </asp:Image></ItemTemplate>
		</asp:TemplateField>
				<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "move.aspx?Action=Up&SlotId=" & DataBinder.Eval(Container.DataItem, "SlotId") & "&TemplateId=" & DataBinder.Eval(Container.DataItem, "TemplateId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/moveup.gif" ID="lnkMoveUp">Move Up</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "move.aspx?Action=Down&SlotId=" & DataBinder.Eval(Container.DataItem, "SlotId") & "&TemplateId=" & DataBinder.Eval(Container.DataItem, "TemplateId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/movedown.gif" ID="lnkMoveDown">Move Down</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
	</Columns>
</CC:GridView>

</asp:content>
