<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master"  Title="Vendor Bid Data" CodeFile="Default.aspx.vb" Inherits="Index" %>


<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<script type="text/javascript">
    function ChangeTarget() {
        document.forms[0].target = '_blank';
        window.setTimeout('document.forms[0].target="_self"', 1000);
    }
</script>

<h4>Vendor Bid Data</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top"><b>Vendor ID:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_VendorID" runat="server" /></td>
</tr>
<tr>
<th valign="top">Historic ID:</th>
<td valign="top" class="field"><asp:textbox id="F_HistoricId" runat="server" Columns="50" MaxLength="5" TextMode="Number"></asp:textbox></td>
</tr>
<tr>
	<th valign="top">LLC(s):</th>
	<td class="field"><CC:CheckBoxListEx ID="F_LLC" runat="server" RepeatColumns="3"></CC:CheckBoxListEx></td>
</tr>
<tr>
<th valign="top"><b>Total Requests:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding= cellspacing="0">
<tr><td>From<asp:textbox id="F_TotalRequestsLBound" runat="server" Columns="5"  Text = "1" MaxLength="10"/></td><td>To<asp:textbox id="F_TotalRequestsUBound" runat="server" Columns="5" MaxLength="10"/></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Active Bids:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding= cellspacing="0">
<tr><td>From<asp:textbox id="F_ActiveBidsLBound" runat="server" Columns="5" MaxLength="10"/></td><td>To<asp:textbox id="F_ActiveBidsUBound" runat="server" Columns="5" MaxLength="10"/></td>
</tr>
</table>
</td>
</tr>

<tr>
<th valign="top"><b>Total Pending Bids:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding= cellspacing="0">
<tr><td>From<asp:textbox id="F_TotalPendingBidsLBound" runat="server" Columns="5" MaxLength="10"/></td><td>To<asp:textbox id="F_TotalPendingBidsUBound" runat="server" Columns="5" MaxLength="10"/></td>
</tr>
</table>
</td>
</tr>


<tr>
<th valign="top"><b>Total Bids:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding= cellspacing="0">
<tr><td>From<asp:textbox id="F_TotalBidsLBound" runat="server" Columns="7" MaxLength="10"/></td><td>To<asp:textbox id="F_TotalBidsUBound" runat="server" Columns="7" MaxLength="10"/></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Total Bid Amount:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding= cellspacing="0">
<tr><td>From<asp:textbox id="F_TotalBidAmountLBound" runat="server" Columns="7" MaxLength="10"/></td><td>To<asp:textbox id="F_TotalBidAmountUBound" runat="server" Columns="7" MaxLength="10"/></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Total Awarded Bids:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding= cellspacing="0">
<tr><td>From<asp:textbox id="F_TotalAwardedBidsLBound" runat="server" Columns="7" MaxLength="10"/></td><td>To<asp:textbox id="F_TotalAwardedBidsUBound" runat="server" Columns="7" MaxLength="10"/></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Total Awarded Bids Amount:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding= cellspacing="0">
<tr><td>From<asp:textbox id="F_TotalAwardedBidsAmountLBound" runat="server" Columns="7" MaxLength="10"/></td><td>To<asp:textbox id="F_TotalAwardedBidsAmountUBound" runat="server" Columns="7" MaxLength="10"/></td>
</tr>
</table>
</td>
</tr>


<tr>
<td colspan="2" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
<input class="btn" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />
</td>
</tr>
<tr>
<td colspan="2" align ="right"> 
<asp:button ID="Export" runat="server" Text="Export " CssClass="btn" />
</td>
</tr>

</table>
</asp:Panel>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:BoundField SortExpression="CompanyName" DataField="CompanyName" HeaderText="Vendor Name"></asp:BoundField>
<%--<asp:BoundField SortExpression="TotalRequests" DataField="TotalRequests" HeaderText="Total Requests"></asp:BoundField>--%>

 <asp:TemplateField SortExpression = "TotalRequests"  HeaderText="Total Requests" ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
                <asp:HyperLink EnableViewState="False" runat="server"  NavigateUrl='<%# "displayVendorBids.aspx?VendorID=" & DataBinder.Eval(Container.DataItem, "Vendorid") %>'     ID="lnktotalRequests"  ><%#Eval("TotalRequests")%></asp:HyperLink>
                 
            </ItemTemplate>
        </asp:TemplateField>

	<asp:BoundField SortExpression="ActiveBids" DataField="ActiveBids" HeaderText="Active Bids "></asp:BoundField>
		<asp:BoundField SortExpression="PendingBids" DataField="PendingBids" HeaderText="Total Pending Bids"></asp:BoundField>
		<asp:BoundField  SortExpression="TotalBids" DataField="TotalBids" HeaderText="TotalBids"></asp:BoundField>
		<asp:BoundField SortExpression="TotalBidsAmount" DataField="TotalBidsAmount" HeaderText="Total Bid Amount($)" DataFormatString="{0:c}"></asp:BoundField>
		<asp:BoundField SortExpression="TotalAwardedBids" DataField="TotalAwardedBids" HeaderText="Total Awarded Bids" ></asp:BoundField>
		<asp:BoundField SortExpression="TotalAwardedBidsAmount" DataField="TotalAwardedBidsAmount" HeaderText="Total Awarded Bids Amount($)" DataFormatString="{0:c}"></asp:BoundField>
		
	</Columns>
</CC:GridView>

</asp:content>

