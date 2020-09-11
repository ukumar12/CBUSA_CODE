<%@ Page Language="vb" AutoEventWireup="false" Inherits="member_orderhistory_default" CodeFile="default.aspx.vb" %>

<CT:masterpage runat="server" id="CTMain">

<h2 class="hdng">My Order History</h2>

<!-- cart table -->
<div runat="server" id="divNoRecords" style="margin-top:10px;">There are no orders on file for your account</div>

<asp:repeater id="rptOrderHistory" EnableViewState="False" runat="server">
<HeaderTemplate>
   <table cellspacing="0" cellpadding="0" border="0" class="bdrtop bdrright" style="width:100%;" summary="shopping cart table">
    <tr>
        <th style="padding:6px 0 6px 0;">&nbsp;</th>
        <th style="padding:6px 0 6px 0;">Order#</th>
        <th style="padding:6px 0 6px 0;">Billing Name</th>	        
        <th style="padding:6px 10px 6px 0;" class="right">Total</th>
        <th style="padding:6px 0 6px 0;">Purchase Date</th>
        <th style="padding:6px 0 6px 0;">Status</th>
        </tr>      
</HeaderTemplate>
<ItemTemplate>
    <tr valign="top">
    <td class="center bdrbottom bdrleft" style="padding-top: 5px; padding-bottom: 5px;"><CC:OneClickButton id="btnDetails" class="btn" commandname="Details" Text="Details" runat="server" /></td>
    <td class="blue bdrbottom" style="padding-top: 5px; padding-bottom: 5px;"><%#DataBinder.Eval(Container.DataItem, "OrderNo")%></td>
    <td class="bdrbottom" style="padding-top: 5px; padding-bottom: 5px;"><%#DataBinder.Eval(Container.DataItem, "BillingName")%></td>
    <td class="blue bdrbottom right" style="padding-top: 5px; padding-bottom: 5px;padding-right:10px"><%#FormatCurrency(DataBinder.Eval(Container.DataItem, "Total"))%></td>
    <td class="blue bdrbottom" style="padding-top: 5px; padding-bottom: 5px;"><%#DataBinder.Eval(Container.DataItem, "ProcessDate")%></td>
    <td class="blue bdrbottom" style="padding-top: 5px; padding-bottom: 5px;"><%#DataBinder.Eval(Container.DataItem, "Status")%></td>
    </tr>      
</ItemTemplate>
<FooterTemplate>
</table>
</FooterTemplate>
</asp:repeater>

</CT:masterpage>