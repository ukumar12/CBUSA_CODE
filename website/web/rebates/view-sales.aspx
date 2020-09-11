<%@ Page Language="VB" AutoEventWireup="false" CodeFile="view-sales.aspx.vb" Inherits="rebates_view_sales" %>

<CT:MasterPage ID="CTMain" runat="server">
    <a href="sales-history.aspx">Return To Sales Report History</a><br /><br />
    <div class="pckggraywrpr">
        <div class="pckghdgred"><asp:Literal id="ltlTitle" runat="server"></asp:Literal></div>
        
        <CC:GridView ID="gvReport" runat="server" AutoGenerateColumns="false" AllowPaging="false" AllowSorting="false"   CssClass="tblcomprlen">
            <RowStyle CssClass="row" />
            <AlternatingRowStyle CssClass="alternate" />
            <Columns>
                <asp:BoundField DataField="CompanyName" HeaderText="Builder" />
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
                <asp:TemplateField HeaderText="Invoices" visible="false">
                    <ItemStyle width="30%" />
                    <ItemTemplate>
                        <div class="thememarket">
                        <CC:GridView id="gvInvoices" runat="server" autogeneratecolumns="false" allowpaging="false" allowsorting="false" Width="90%" HorizontalAlign="Center">
                            <RowStyle CssClass="row" />
                            <AlternatingRowStyle CssClass="alternate" />
                            <Columns>
                                <asp:BoundField DataField="InvoiceNumber" HeaderText="Invoice Number" />
                                <asp:BoundField DataField="InvoiceAmount" HeaderText="Amount" DataFormatString="{0:c}" HtmlEncode="false" />
                                <asp:BoundField DataField="InvoiceDate" HeaderText="Date" DataFormatString="{0:d}" HtmlEncode="false" />
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
        

        <CC:GridView ID="gvReportNotReported" runat="server" AutoGenerateColumns="false" AllowPaging="false" AllowSorting="false"  CssClass="tblcompr">
            <RowStyle CssClass="row" />
            <AlternatingRowStyle CssClass="alternate" />
            <Columns>
                <asp:BoundField DataField="CompanyName" HeaderText="Builder" />
                <asp:TemplateField HeaderText="Original Vendor Reported Total">
                    <itemtemplate>
                        <asp:Literal id="Literal1" runat="server" />
                    </itemtemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Original Builder Reported Total">
                    <itemtemplate>
                        <asp:Literal id="Literal2" runat="server" />
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
                    <ItemTemplate><asp:Literal id="Literal3" runat="server"></asp:Literal></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Final Resolution Amount">
                    <ItemTemplate>
                        <asp:Literal id="Literal4" runat="server"></asp:Literal>                    
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </CC:GridView>

    </div>
</CT:MasterPage>