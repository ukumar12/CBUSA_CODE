<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Mailing Message" CodeFile="links.aspx.vb" Inherits="Links"  %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4><asp:Literal ID="ltlName" runat="server"></asp:Literal> - Link Tracking Details</h4>

<div><a href="view.aspx?MessageId=<%=MessageId %>" class="L1"><< Go Back</a></div>

<div style="margin-top:20px"></div>

<span class="smaller">Please provide search criteria below</span>
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top">Mime Type:</th>
<td valign="top" class="field">
<asp:DropDownList ID="F_MimeType" runat="server">
<asp:ListItem Text="-- ALL --" Value=""></asp:ListItem>
<asp:ListItem Text="HTML" Value="HTML"></asp:ListItem>
<asp:ListItem Text="TEXT" Value="TEXT"></asp:ListItem>
</asp:DropDownList>
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
</td>
</tr>
</table>

<p></p>
<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="100" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:BoundField SortExpression="Name" DataField="Name" HeaderText="Name"></asp:BoundField>
		<asp:BoundField SortExpression="MimeType" DataField="MimeType" HeaderText="Mime Type"></asp:BoundField>
		<asp:BoundField SortExpression="Qty" DataField="Qty" HeaderText="Nof Clicks"></asp:BoundField>
		<asp:BoundField SortExpression="Link" DataField="Link" HeaderText="Link"></asp:BoundField>
		</Columns>
</CC:GridView>

</asp:content>

