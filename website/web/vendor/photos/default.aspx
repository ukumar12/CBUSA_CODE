<%@ Page Language="VB" AutoEventWireup="false" Title="Vendor Photo" CodeFile="default.aspx.vb" Inherits="VendorPhotos"  %>

<CT:MasterPage ID="CTMain" runat="server">
<div class="pckgwrpr bggray">
<div class="pckghdgred">
    My Photos
</div>
<%--<asp:button id ="btnGoToDashBoard" text="Go to DashBoard" class="btnred" runat="server" />--%>
<div class="pckgbdy">
<p></p>
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Photo" cssClass="btnred"></CC:OneClickButton>
<asp:hyperlink id="Preview" Runat="server" Text="Preview Photo Gallery" target="_blank" />
<p></p>
<p><b>You can upload up to 10 photos.</b></p>
<CC:GridView id="gvList" Width="98%" CellSpacing="2" CellPadding="2" class="tblcompr" runat="server" HeaderText="" EmptyDataText="You do not have any photos." AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?PhotoId=" & DataBinder.Eval(Container.DataItem, "PhotoId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate="False" Message="Are you sure that you want to delete this Photo?" runat="server" NavigateUrl= '<%# "delete.aspx?PhotoId=" & DataBinder.Eval(Container.DataItem, "PhotoId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:ImageField HeaderText="Photo" DataImageUrlField="Photo" DataImageUrlFormatString="/assets/vendorphoto/thumb/{0}"></asp:ImageField> 
		<asp:BoundField SortExpression="Caption" DataField="Caption" HeaderText="Caption"></asp:BoundField>
		<asp:BoundField SortExpression="AltText" DataField="AltText" HeaderText="Alt Text"></asp:BoundField>
		
	</Columns>
</CC:GridView>
</div></div>
</CT:MasterPage>

