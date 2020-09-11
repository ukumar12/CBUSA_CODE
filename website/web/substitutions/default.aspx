<%@ Page Language="VB" AutoEventWireup="false" CodeFile="default.aspx.vb" Inherits="substitutions_default" %>

<CT:MasterPage ID="CTMain" runat="server">
<asp:PlaceHolder runat="server">
    <h1>Product Substitutions</h1>
    
    <CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom" CssClass="tblcompr">
	    <AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	    <RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	    <Columns>
		    <asp:TemplateField>
			    <ItemTemplate>
			    <CC:ConfirmLink enableviewstate="False" Message="Are you sure that you want to remove this substitution?" runat="server" NavigateUrl= '<%# "delete.aspx?ProductId=" & DataBinder.Eval(Container.DataItem, "ProductId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			    </ItemTemplate>
		    </asp:TemplateField>
		    <asp:BoundField SortExpression="BuilderID" DataField="BuilderID" HeaderText="Builder"></asp:BoundField>
		    <asp:BoundField SortExpression="Product" DataField="Product" HeaderText="Product Name"></asp:BoundField>
		    <asp:BoundField SortExpression="SubstituteProduct" DataField="SubstituteProduct" HeaderText="Substitute Product Name"></asp:BoundField>
	    </Columns>
    </CC:GridView>
</asp:PlaceHolder>
</CT:MasterPage>