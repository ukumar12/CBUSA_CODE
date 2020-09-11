<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" CodeFile="ViewResponses.aspx.vb" Inherits="admin_surveys_reports_ViewResponses" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Survey Results</h4>

<p></p>
<CC:OneClickButton ID="btnBack" Text="Back to Results" runat="server" CssClass="btn" />
<CC:OneClickButton ID="btnRefresh" Text="Refresh" runat="server" CssClass="btn" />

<p></p>
<asp:Label ID="lblQuestion" runat="server"></asp:Label>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>		
		<asp:BoundField SortExpression="Response" DataField="Response" HeaderText="Response"></asp:BoundField>
		<asp:BoundField SortExpression="surveyanswer.CreateDate" DataField="CreateDate" HeaderText="Date"></asp:BoundField>
	</Columns>
</CC:GridView>


</asp:content>
