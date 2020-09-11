<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Mailing Template" CodeFile="default.aspx.vb" Inherits="index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Mailing Template Design</h4>

<p></p>
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Mailing Template" cssClass="btn"></CC:OneClickButton>
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
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "/admin/broadcast/templates/slots/default.aspx?F_TemplateId=" & DataBinder.Eval(Container.DataItem, "TemplateId") %>' ID="lnkSlot">Slots</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="Name" DataField="Name" HeaderText="Name"></asp:BoundField>
		<asp:BoundField SortExpression="NofSlots" DataField="NofSlots" HeaderText="#Slots"></asp:BoundField>
		<asp:TemplateField><ItemTemplate>
            <asp:Image runat="server" ImageUrl = '<%# "/assets/broadcast/templates/" & DataBinder.Eval(Container.DataItem, "ImageName")%>'>
            </asp:Image></ItemTemplate>
		</asp:TemplateField>
	</Columns>
</CC:GridView>

</asp:content>

