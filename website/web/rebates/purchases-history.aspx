<%@ Page Language="VB" AutoEventWireup="false" CodeFile="purchases-history.aspx.vb" Inherits="rebates_purchases_history" %>

<CT:MasterPage ID="CTMain" runat="server">
    <div class="pckggraywrpr">
        <div class="pckghdgred">Purchases Report History</div>
		<!--<p><br />&nbsp;This page is currently down for maintenance. We apologize for any inconvenience this may cause. Please check back soon.-->
        <div id="divNoPurchases" runat="server" class="bold center">
            There are no reports available to view.
        </div>
       <CC:GridView id="gvPurchases" runat="server" AutoGenerateColumns="false" AllowPaging="false" AllowSorting="false"   CssClass="tblcompr" Width="60%" HorizontalAlign="Center">
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