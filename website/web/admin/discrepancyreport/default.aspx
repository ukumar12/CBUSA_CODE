<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Discrepancy Reports" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<%--<h4>Price Comparison Administration</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top"><b>Builder ID:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_BuilderID" runat="server" /></td>
</tr>
<tr>
<th valign="top"><b>Created:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_CreatedLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_CreatedUbound" runat="server" /></td>
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
</table>
</asp:Panel>
<p></p>
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Price Comparison" cssClass="btn"></CC:OneClickButton>
<p></p>--%>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="False" AllowSorting="False" HeaderText="" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="1" PagerSettings-Position="Bottom">
	<%--<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>--%>
	<Columns>
		<asp:BoundField DataField="SalesReportId" HeaderText="SalesReportId"></asp:BoundField>
		<asp:BoundField DataField="VendorId" HeaderText="VendorId"></asp:BoundField>
		<asp:BoundField DataField="VendorHistoricId" HeaderText="VendorHistoricId"></asp:BoundField>
		<asp:BoundField DataField="VendorCompany" HeaderText="VendorName"></asp:BoundField>
		<asp:BoundField DataField="VendorTotal" HeaderText="OriginalVendorReportedAmount"></asp:BoundField>
		<asp:BoundField DataField="PurchasesReportId" HeaderText="PurchasesReportId"></asp:BoundField>
		<asp:BoundField DataField="BuilderId" HeaderText="BuilderId"></asp:BoundField>
		<asp:BoundField DataField="BuilderHistoricId" HeaderText="BuilderHistoricId"></asp:BoundField>
		<asp:BoundField DataField="BuilderName" HeaderText="BuilderName"></asp:BoundField>
		<asp:BoundField DataField="BuilderTotal" HeaderText="OriginalBuilderReportedAmount"></asp:BoundField>
		<asp:BoundField DataField="SalesReportDisputeId" HeaderText="SalesReportDisputeId"></asp:BoundField>
		<asp:BoundField DataField="DisputeResponse" HeaderText="DisputeResponse"></asp:BoundField>
		<asp:BoundField DataField="DisputeResponseReason" HeaderText="DisputeResponseReason"></asp:BoundField>
		<asp:BoundField DataField="BuilderTotalAmount" HeaderText="DisputeBuilderTotalAmount"></asp:BoundField>
		<asp:BoundField DataField="VendorTotalAmount" HeaderText="VendorTotalAmount"></asp:BoundField>
		<asp:BoundField DataField="ResolutionAmount" HeaderText="DisputeResolutionAmount"></asp:BoundField>
		<asp:BoundField DataField="BuilderComments" HeaderText="DisputeBuilderComments"></asp:BoundField>
		<asp:BoundField DataField="VendorComments" HeaderText="DisputeVendorComments"></asp:BoundField>
	</Columns>
</CC:GridView>

</asp:content>

