<%@ Page Language="VB" AutoEventWireup="false" CodeFile="view-purchases.aspx.vb" Inherits="rebates_view_purchases" %>

<CT:MasterPage ID="CTMain" runat="server">
    <a href="purchases-history.aspx">Return To Purchases Report History</a><br /><br />
    <div class="pckggraywrpr">
        <div class="pckghdgred"><asp:Literal id="ltlTitle" runat="server"></asp:Literal></div>
        
        <CC:GridView ID="gvReport" runat="server" AutoGenerateColumns="false" AllowPaging="false" AllowSorting="false" EmptyDataText ="No Purchases  reported for this Quarter" CssClass="tblcompr">
            <RowStyle CssClass="row" />
            <AlternatingRowStyle CssClass="alternate" />
            <Columns>
                <asp:BoundField DataField="CompanyName" HeaderText="Vendor" />
                <asp:TemplateField HeaderText="Original Vendor Reported Total">
                    <itemtemplate>
                        <asp:Literal id="ltlVendorTotal" runat="server" />
                    </itemtemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Original Builder Reported Total">
                    <itemtemplate>
                        <asp:Literal id="ltlBuilderTotal" runat="server" />
                    </itemtemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Purchases" visible="false">
                    <ItemStyle width="30%" />
                    <ItemTemplate>
                        <div class="thememarket">
                        <CC:GridView id="gvPurchases" runat="server" autogeneratecolumns="false" allowpaging="false" allowsorting="false" Width="90%" HorizontalAlign="Center">
                            <RowStyle CssClass="row" />
                            <AlternatingRowStyle CssClass="alternate" />
                            <Columns>
                                <asp:BoundField DataField="PONumber" HeaderText="PO Number" />
                                <asp:BoundField DataField="POAmount" HeaderText="Amount" DataFormatString="{0:c}" HtmlEncode="false" />
                                <asp:BoundField DataField="PODate" HeaderText="Date" DataFormatString="{0:d}" HtmlEncode="false" />
                            </Columns>
                        </CC:GridView>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Dispute Details">
                    <ItemStyle width="30%" />
                    <ItemTemplate><asp:Literal id="ltlDispute" runat="server"></asp:Literal></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Final Resolution Amount">
                    <ItemTemplate>
                        <asp:Literal id="ltlFinalAmount" runat="server"></asp:Literal>                    
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </CC:GridView>
        
    </div>
</CT:MasterPage>