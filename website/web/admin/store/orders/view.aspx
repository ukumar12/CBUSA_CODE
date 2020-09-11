<%@ Page Language="vb" AutoEventWireup="false" Inherits="OrderSTatus_orderhistory_view" CodeFile="view.aspx.vb" %>
<%@ Register TagName="StoreShoppingCartStatus" TagPrefix="CC" Src="~/controls/StoreShoppingCartStatus.ascx" %>
<%@ Import Namespace="Components" %>
<%@ Import Namespace="DataLayer" %>

<CT:masterpage runat="server" id="CTMain">

<asp:Placeholder id="ph" runat="server">

<div style="height: 100px; width:100%;padding:10px;">

    <!-- MESSAGE -->

</div>


<table width="600" border="0" cellspacing="0" cellpadding="4" class="bdr" style="margin-top:20px;">
<tr><td class="baghdr bold" width="90">Order#</td><td><%=dbOrder.OrderNo%></td></tr>
<tr><td class="baghdr bold">Order Date:</td><td><%=FormatDateTime(dbOrder.ProcessDate,1)%></td></tr>
<tr><td class="baghdr bold">Order Status:</td><td><%=StoreOrderStatusRow.GetRowByCode(DB, dbOrder.Status).Name%></td></tr>
</table>

<CC:StoreShoppingCartStatus ID="ctrlCart" runat="server" />


</asp:Placeholder>

</CT:masterpage>