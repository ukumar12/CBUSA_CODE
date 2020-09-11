<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Sales Report" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Sales Report</h4>

<span class="smaller">Please provide search criteria below</span>
<table cellpadding="2" cellspacing="2">
<tr>
<th><b>Order Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_ProcessDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_ProcessDateUbound" runat="server" /></td>
</tr>
</table>
</td>
<th valign="top">Department</th>
<td valign="top" class="field"><CC:DropDownListEx id="F_DepartmentId" runat="server" /></td>
<td>
<CC:DateValidator runat="server" ID="dvProcessDateLbound" Display="Dynamic" ControlToValidate="F_ProcessDateLbound" ErrorMessage="Field 'Order Date From' is invalid" ></CC:DateValidator><br />
<CC:DateValidator runat="server" ID="dvProcessDateUbound" Display="Dynamic" ControlToValidate="F_ProcessDateUbound" ErrorMessage="Field 'Order Date To' is invalid" ></CC:DateValidator>
</td>
</tr>
<tr>
<th valign="top">Item Name:</th>
<td valign="top" class="field"><asp:textbox id="F_ItemName" runat="server" Columns="30" MaxLength="255"></asp:textbox></td>
<th valign="top"><b>Status:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_Status" runat="server" /></td>
</tr>
<tr>
<th valign="top"><b>Item #:</b></th>
<td valign="top" class="field"><asp:TextBox ID="F_SKU" runat="server" Columns="20" MaxLength="50" /></td>
<th valign="top"><b>Output As:</b></th>
<td valign="top" class="field">
<asp:DropDownList ID="F_OutputAs" runat="server">
<asp:ListItem Value="HTML" Text="HTML Page"></asp:ListItem>
<asp:ListItem Value="Excel" Text="Excel Document"></asp:ListItem>
</asp:DropDownList>
</td>
</tr>
<tr>
<th valign="top"><b>Discovery Sources:</b></th>
<td valign="top" class="field">
<asp:DropDownList ID="F_DiscoverySources" runat="server">
</asp:DropDownList>
</td>
<th valign="top"><b>Referral Code:</b></th>
<td valign="top" class="field">
<asp:TextBox runat="server" ID="F_ReferralCode" Columns="20" MaxLength="20" />
</td>
</tr>
<tr>
	<th valign="top">
		<b>Promotion Code:</b></th>
	<td class="field" valign="top">
		<asp:TextBox ID="F_PromotionCode" runat="server" Columns="20" MaxLength="20">
		</asp:TextBox>
	</td>
</tr>
<tr>
<td colspan="4" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
<input class="btn" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />
</td>
</tr>
</table>

<p></p>
<CC:GridView ShowFooter="True" id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<FooterStyle CssClass="header" HorizontalAlign="Right" />
	<Columns>
		<asp:BoundField SortExpression="SKU" DataField="SKU" HeaderText="Item #"></asp:BoundField>
		<asp:BoundField SortExpression="ItemName" DataField="ItemName" HeaderText="Item Name"></asp:BoundField>
		<asp:BoundField SortExpression="AvgPrice" HTMLEncode="False" DataFormatString="{0:c}" DataField="AvgPrice" HeaderText="Avg Price"><ItemStyle HorizontalAlign="right" /></asp:BoundField>
		<asp:BoundField SortExpression="Quantity" DataField="Quantity" HeaderText="Units"><ItemStyle HorizontalAlign="right" /></asp:BoundField>
		<asp:BoundField SortExpression="TotalPrice" HTMLEncode="False" DataFormatString="{0:c}" DataField="TotalPrice" HeaderText="Total"><ItemStyle HorizontalAlign="right" /></asp:BoundField>		
	</Columns>
</CC:GridView>

<div runat="server" id="divDownload" visible="false">
<asp:HyperLink id="lnkDownload" runat="server">Download File</asp:HyperLink>
</div>

</asp:content>

