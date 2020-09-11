<%@ Control Language="VB" AutoEventWireup="false" CodeFile="CurrentTakeOffs.ascx.vb" Inherits="modules_TwoPrice_CurrentTwoPriceTakeOffs" %>
<asp:UpdatePanel ID="upMain" runat="server">
    <ContentTemplate>               
        <div class="pckggraywrpr">
            <div class="pckghdgred">Existing TwoPrice TakeOffs</div>
            <div class="pckghdgblue">
                <div>
                    <asp:DropDownList ID="drpBuilders" runat="server" AutoPostBack="true"></asp:DropDownList>
                </div>
            </div>
            <CC:GridView id="gvList" CellSpacing="2" CellPadding="2" CssClass="tblcompr" runat="server" PageSize="3200" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	            <HeaderStyle CssClass="sbttl" />
	            <AlternatingRowStyle CssClass="alternate" VerticalAlign="Top" Font-Size="Larger"></AlternatingRowStyle>
	            <RowStyle CssClass="row" VerticalAlign="Top" Font-Size="Larger" Font-Bold="true"></RowStyle>
	            <Columns>
		            <asp:TemplateField>
		                <ItemStyle width="20px" />
		                <ItemTemplate>
		                    <asp:CheckBox id="cbInclude" runat="server"></asp:CheckBox>
		                </ItemTemplate>
		            </asp:TemplateField>
                    <asp:BoundField DataField="Project" HeaderText="Project" SortExpression="ProjectName" />
                    <asp:BoundField DataField="Title" HeaderText="Title" SortExpression="Title" />
                    <asp:BoundField DataField="Saved" HeaderText="Save Date" SortExpression="Saved" />
                    <asp:TemplateField HeaderText="Saved Comparisons">
                        <ItemTemplate>
                            <asp:Literal id="ltlComparisons" runat="server"></asp:Literal>
                        </ItemTemplate>
                    </asp:TemplateField>
	            </Columns>
            </CC:GridView>
            <asp:Button id="btnAdd" runat="server" text="Add Products from Selected TwoPriceTakeOff(s) to Current TwoPriceTakeOff" cssclass="btnred" />
        </div>
    </ContentTemplate>
</asp:UpdatePanel>