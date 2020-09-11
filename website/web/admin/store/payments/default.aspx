<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Payment Transactions" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Payment Transactions</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2" border="0">
<tr>
<th valign="top">Order #:</th>
<td valign="top" class="field"><asp:textbox id="F_OrderNo" runat="server" Columns="15" MaxLength="50"></asp:textbox></td>
<th valign="top">TransactionNo:</th>
<td valign="top" class="field"><asp:textbox id="F_TransactionNo" runat="server" Columns="15" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top"><b>Create Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_CreateDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_CreateDateUbound" runat="server" /></td>
</tr>
</table>
</td>
<th valign="top">Result:</th>
<td valign="top" class="field">
<asp:DropDownList ID="F_Result" runat="server">
<asp:ListItem Value="">All</asp:ListItem>
<asp:ListItem Value="0">Success</asp:ListItem>
<asp:ListItem Value="1">Failure</asp:ListItem>
</asp:DropDownList>
</td>
<td>
<CC:DateValidator runat="server" ID="dvCreateDateLbound" Display="Dynamic" ControlToValidate="F_CreateDateLbound" ErrorMessage="Field 'Create Date From' is invalid" ></CC:DateValidator><br />
<CC:DateValidator runat="server" ID="dvCreateDateUbound" Display="Dynamic" ControlToValidate="F_CreateDateUbound" ErrorMessage="Field 'Create Date To' is invalid" ></CC:DateValidator>
</td>
</tr>
<tr>
<td colspan="4" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
<input class="btn" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />
</td>
</tr>
</table>
</asp:Panel>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
	    <asp:TemplateField HeaderText="Order #">
	       <ItemTemplate>
	         <asp:literal id="ltlOrder" runat="server" />
	       </ItemTemplate>
	    </asp:TemplateField>
		<asp:BoundField SortExpression="TransactionNo" DataField="TransactionNo" HeaderText="Transaction#"></asp:BoundField>
		<asp:BoundField SortExpression="Amount" DataField="Amount" DataFormatString="${0:C}" HeaderText="Amount"></asp:BoundField>
	    <asp:TemplateField HeaderText="Description">
	       <ItemTemplate>
	         <asp:literal id="ltlDescription" runat="server" />
	       </ItemTemplate>
	    </asp:TemplateField>
		<asp:BoundField SortExpression="Result" DataField="Result" HeaderText="Result"></asp:BoundField>
	    <asp:TemplateField HeaderText="Payment Response">
	       <ItemTemplate>
	         <asp:literal id="ltlResponse" runat="server" />
	       </ItemTemplate>
	    </asp:TemplateField>
		<asp:BoundField SortExpression="CreateDate" DataField="CreateDate" HeaderText="Create Date" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
	</Columns>
</CC:GridView>

</asp:content>
