<%@ Page Language="VB" AutoEventWireup="false" CodeFile="transaction-history.aspx.vb" Inherits="builder_transaction_history" %>

<CT:MasterPage ID="CTMain" runat="server">
<div class="pckggraywrpr" style="width:600px;margin:10px auto;">
    <div class="pckghdgred">Account Billing History</div>
    <div style="padding:10px;text-align:center;">
        <CC:GridView ID="gvTransactions" runat="server" CellSpacing="1" CellPadding="5" BorderWidth="0" AutoGenerateColumns="false" AllowPaging="false" CssClass="tblcompr" Width="500px">
            <AlternatingRowStyle CssClass="alternate" Font-Size="Larger" />
            <RowStyle CssClass="row" Font-Size="Larger" />
            <Columns>
                <asp:BoundField DataField="Timestamp" HeaderText="Date" DataFormatString="{0:d}" HtmlEncode="false"></asp:BoundField>
                <asp:BoundField DataField="merchantTransactionId" HeaderText="Transaction ID" />
                <asp:BoundField DataField="Amount" HeaderText="Transaction Amount" />            
            </Columns>
        </CC:GridView>
        <p class="center">
            <a href="default.aspx" class="btnred">Return to Dashboard</a>
            <a href="update.aspx" class="btnred">Update Billing Details</a>
        </p>
    </div>
</div>
</CT:MasterPage>