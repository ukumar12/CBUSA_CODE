<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Advertiser Tracking" CodeFile="report.aspx.vb" Inherits="report"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Banner Tracking - Summary Report</h4>

<span class="smaller">Please provide search criteria below</span>
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top"><b>Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_CreateDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_CreateDateUbound" runat="server" /></td>
</tr>
</table>
</td>
<th valign="top"><b>Is Active:</b></th>
<td valign="top" class="field">
	<asp:DropDownList ID="F_IsActive" runat="server">
		<asp:ListItem Value="">-- ALL --</asp:ListItem>
		<asp:ListItem Value="1">Yes</asp:ListItem>
		<asp:ListItem Value="0">No</asp:ListItem>
	</asp:DropDownList>
</td>
</tr>
<tr>
<th valign="top"><b>Banner:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_BannerId" runat="server" /></td>
<th valign="top"><b>Banner Group:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_BannerGroupId" runat="server" /></td>
</tr>
<tr>
<td colspan="4" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
<input class="btn" type="submit" value="Clear" onclick="window.location='report.aspx';return false;" />
</td>
</tr>
</table>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" AllowPaging="False" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:BoundField SortExpression="FullName" DataField="FullName" HeaderText="Banner"></asp:BoundField>
		<asp:BoundField SortExpression="ImpressionCount" DataField="ImpressionCount" HeaderText="Impression Count" DataFormatString="{0:N0}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField SortExpression="ClickCount" DataField="ClickCount" HeaderText="Click Count" DataFormatString="{0:N0}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField SortExpression="ClickRate" DataField="ClickRate" HeaderText="CTR" DataFormatString="{0}%"></asp:BoundField>
		<asp:BoundField SortExpression="NofOrders" DataField="NofOrders" HeaderText="# of Orders" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField SortExpression="Total" DataField="Total" HeaderText="Total [$]" DataFormatString="{0:c}" HTMLEncode="False">
		    <itemstyle HorizontalAlign="right" /></asp:BoundField>
		<asp:BoundField SortExpression="AvgOrder" DataField="AvgOrder" HeaderText="Avg. Order [$]" DataFormatString="{0:c}" HTMLEncode="False">
		    <itemstyle HorizontalAlign="right" /></asp:BoundField>
		<asp:BoundField SortExpression="Conversion" DataField="Conversion" HeaderText="Conversion [%]" HTMLEncode="False"></asp:BoundField>
		<asp:TemplateField>
			<HeaderTemplate>
				Image Preview
			</HeaderTemplate>
			<ItemTemplate>
				<asp:Label runat="server" ID="imglink"><a href='javascript:expandit(<%#DataBinder.Eval(Container, "DataItem.BannerId")%>);'><span class="smaller" id='imgtext<%#DataBinder.Eval(Container, "DataItem.BannerId")%>'>View Image</span><img id='IMG<%#DataBinder.Eval(Container, "DataItem.BannerId")%>' src="/images/detail-down.gif" width="8" height="8" hspace="2" border="0" alt="Expand" align="absmiddle" ></a></asp:Label><asp:Label runat="server" ID="noimg" Text="N/A" CssClass="smaller" /><span style="display:none" id='SPAN<%#DataBinder.Eval(Container, "DataItem.BannerId")%>'><asp:Literal id="img" Runat="server"></asp:Literal></span>
			</ItemTemplate>
		</asp:TemplateField>
	</Columns>
</CC:GridView>

</asp:content>

