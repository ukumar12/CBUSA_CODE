<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="PIQ Ad" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>PIQ Ad/Call Out Administration for: <asp:Literal ID="ltrPIQ" runat="server" ></asp:Literal></h4>

<p></p>
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New PIQ Ad/Call Out" cssClass="btn"></CC:OneClickButton>
<CC:OneClickButton id="btnBack" Runat="server" Text="Back" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?PIQID=" & DataBinder.Eval(Container.DataItem, "PIQID") & "&PIQAdID=" & DataBinder.Eval(Container.DataItem, "PIQAdID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate="False" Message="Are you sure that you want to remove this PIQ Ad?" runat="server" NavigateUrl= '<%# "delete.aspx?PIQID=" & DataBinder.Eval(Container.DataItem, "PIQID") & "&PIQAdID=" & DataBinder.Eval(Container.DataItem, "PIQAdID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:ImageField HeaderText="Ad/Call Out" DataImageUrlField="AdFile" DataImageUrlFormatString="/assets/piq/ads/thumbnails//{0}"></asp:ImageField> 
		<asp:BoundField SortExpression="AltText" DataField="AltText" HeaderText="Alternate Text"></asp:BoundField>
		<asp:BoundField SortExpression="LinkURL" DataField="LinkURL" HeaderText="Ad Link"></asp:BoundField>
		<asp:BoundField SortExpression="StartDate" DataField="StartDate" HeaderText="Start Date" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField SortExpression="EndDate" DataField="EndDate" HeaderText="End Date" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:ImageField SortExpression="IsActive" DataImageUrlField="IsActive" HeaderText="Is Active" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />

	</Columns>
</CC:GridView>

</asp:content>

