<%@ Page Language="VB" AutoEventWireup="false" CodeFile="beta-index.aspx.vb" Inherits="beta_index" %>

<CT:MasterPage ID="CTMain" runat="server">

    <div class="pckggraywrpr" style="width:500px;margin:10px auto;">
        <div class="pckghdgred nobdr">
            CBUSA.us Beta Site
        </div>
        <div class="bdr" style="background-color:#fff;margin:5px;padding:10px;">
            <h3>Links</h3>
            <b>Builder Tools (visit the Takeoff link first to log in as builder)</b><br />
            <ul style="list-style-type:none;">
                <li><a href="/forms/builder-registration/">Builder Registration</a></li>
                <li><a href="/builder/">Builder Dashboard</a></li>
                <li><a href="/takeoffs/default.aspx">Start Takeoff</a></li>
                <li>Price Comparison (requires active Takeoff)</li>
                <li>Orders (requires active Price Comparison)</li>
                <li>Drop Manager (requires Order)</li>
                <li><a href="/projects/default.aspx">Projects</a></li>
                <li><a href="/rebates/builder-purchases.aspx">Purchases Report</a></li>
                <li><a href="/rebates/discrepancy-report.aspx">Discrepancy Report</a></li>
            </ul>
            <br />
            <b>Vendor Tools (visit the Vendor Sales Report link first to log in as vendor)</b>
            <ul style="list-style-type:none;">
                <li><a href="/forms/vendor-registration/">Vendor Registration</a></li>
                <li><a href="/rebates/vendor-sales.aspx">Sales Report</a></li>
                <li><a href="/rebates/discrepancy-response.aspx">Sales Report Disputes</a></li>
                <li>Rebate Terms (testing)</li>
                <li>Custom Rebate (in progress)</li>
            </ul>
        </div>
    </div>

</CT:MasterPage>
