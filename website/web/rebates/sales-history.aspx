<%@ Page Language="VB" AutoEventWireup="false" CodeFile="sales-history.aspx.vb" Inherits="rebates_sales_history" %>

<CT:MasterPage ID="CTMain" runat="server">
    <div class="pckggraywrpr">
        <div class="pckghdgred">Sales Report History</div>
        <div id="divNoSales" runat="server" class="bold center">
            There are no reports available to view.
        </div>
        <CC:GridView id="gvSales" runat="server" AutoGenerateColumns="false" AllowPaging="false" AllowSorting="false" CssClass="tblcompr" Width="60%" HorizontalAlign="Center">
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:HyperLink id="lnkReport" runat="server" cssclass="btnred" text="View Report" ></asp:HyperLink>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="PeriodQuarter" HeaderText="Quarter" SortExpression="PeriodQuarter" />
                <asp:BoundField DataField="PeriodYear" HeaderText="Year" SortExpression="PeriodYear" />
                
                
                <asp:TemplateField HeaderText="Final Resolution Amount">
                 <ItemTemplate>
		        <asp:Literal  id="ltlFinalResolutionAmount" runat="server"></asp:Literal>
		    </ItemTemplate>
             </asp:TemplateField>

            </Columns>
        </CC:GridView>
    </div>
</CT:MasterPage>