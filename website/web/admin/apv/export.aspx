<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="APV" CodeFile="export.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>APV Administration</h4>
<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" AllowPaging="False" AllowSorting="False" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:BoundField SortExpression="BuilderName" DataField="BuilderName" HeaderText="Builder Name"></asp:BoundField>
		<asp:BoundField SortExpression="HistoricVendorID" DataField="HistoricVendorID" HeaderText="Historic Vendor ID"></asp:BoundField>
		<asp:BoundField SortExpression="VendorName" DataField="VendorName" HeaderText="Vendor Name"></asp:BoundField>
		<asp:BoundField SortExpression="HasReported" DataField="HasReported" HeaderText="Has Reported"></asp:BoundField>
		<asp:BoundField SortExpression="LLC" DataField="LLC" HeaderText="LLC"></asp:BoundField>
		<asp:BoundField SortExpression="PeriodYear" DataField="PeriodYear" HeaderText="Period Year"></asp:BoundField>
		<asp:BoundField SortExpression="PeriodQuarter" DataField="PeriodQuarter" HeaderText="Period Quarter"></asp:BoundField>
		<asp:BoundField SortExpression="VendorID" DataField="VendorID" HeaderText="Vendor ID"></asp:BoundField>
		<asp:BoundField SortExpression="DisputeResponse" DataField="DisputeResponse" HeaderText="Dispute Response"></asp:BoundField>
		<asp:BoundField SortExpression="DisputeResponseReason" DataField="DisputeResponseReason" HeaderText="Dispute Response Reason"></asp:BoundField>
		<asp:BoundField DataField="InitialVendorAmount" HeaderText="Initial Vendor Amount" DataFormatString="{0:c}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField DataField="InitialBuilderAmount" HeaderText="Initial Builder Amount" DataFormatString="{0:c}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField DataField="BuilderAmountInDispute" HeaderText="Builder Amount In Dispute" DataFormatString="{0:c}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField DataField="VendorAmountInDispute" HeaderText="Vendor Amount In Dispute" DataFormatString="{0:c}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField DataField="ResolutionAmount" HeaderText="Resolution Amount" DataFormatString="{0:c}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField DataField="FinalAmount" HeaderText="Final Amount" DataFormatString="{0:c}" HTMLEncode="False"></asp:BoundField>
	</Columns>
</CC:GridView>

</asp:content>

