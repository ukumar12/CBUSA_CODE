<%@ Control Language="VB" AutoEventWireup="false" CodeFile="VendorInvoiceHistory.ascx.vb" Inherits="modules_VendorInvoiceHistory" %>

<div class="pckgwhtwrpr">
    <div class="pckghdgred">Invoice History</div>
    <asp:UpdatePanel id="upInvoices" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
        <ContentTemplate>
            <CC:GridView ID="gvInvoices" runat="server" BorderWidth="0" AllowPaging="true" AllowSorting="true" AutoGenerateColumns="false" width="100%">
                <Columns>
                    <asp:BoundField HeaderText="Date" DataField="EditDate" DataFormatString="{0:d}" HTMLEncode="false" SortExpression="EditDate" />
                    <asp:BoundField HeaderText="Invoice Number" DataField="InvoiceNumber" SortExpression="InvoiceNumber" />
                    <asp:BoundField HeaderText="Quarter" DataField="PeriodQuarter" SortExpression="PeriodQuarter" />
                    <asp:BoundField HeaderText="Year" DataField="PeriodYear" SortExpression="PeriodYear" />
                    <asp:BoundField HeaderText="Rebate Amount" DataField="TotalAmount" SortExpression="TotalAmount" />
                    <asp:BoundField HeaderText="Payment Amount" DataField="AmountPaid" SortExpression="AmountPaid" />
                    <asp:TemplateField HeaderText="Balance">
                        <ItemTemplate>
                            <%#FormatCurrency(Components.Core.GetDouble(DataBinder.Eval(Container.DataItem, "TotalAmount")) - Components.Core.GetDouble(DataBinder.Eval(Container.DataItem, "AmountPaid")))%>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </CC:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
    
</div>