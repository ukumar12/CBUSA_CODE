<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Individual Survey" CodeFile="details.aspx.vb" Inherits="admin_surveys_fullsurveys_details" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Individual Survey Response</h4>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:BoundField SortExpression="Question" DataField="Question" HeaderText="Question" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField SortExpression="Answer" DataField="Answer" HeaderText="Answer"></asp:BoundField>
	</Columns>
</CC:GridView>

</asp:content>
