<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Vindicia Soap Log" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Vindicia Soap Log Administration</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top">Soap Id:</th>
<td valign="top" class="field"><asp:textbox id="F_SoapId" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top"><b>Return Code:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding= cellspacing="0">
<tr><td>From<asp:textbox id="F_ReturnCodeLBound" runat="server" Columns="5" MaxLength="10"/></td><td>To<asp:textbox id="F_ReturnCodeUBound" runat="server" Columns="5" MaxLength="10"/></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top">Return String:</th>
<td valign="top" class="field"><asp:textbox id="F_ReturnString" runat="server" Columns="50" MaxLength="1000"></asp:textbox></td>
</tr>
<tr>
<th valign="top"><b>Create Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_CreateDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_CreateDateUbound" runat="server" /></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top">Builder:</th>
<td valign="top" class="field"><asp:textbox id="F_BuilderGUID" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
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


<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:BoundField SortExpression="SoapId" DataField="SoapId" HeaderText="Soap Id"></asp:BoundField>
		<asp:BoundField SortExpression="ReturnCode" DataField="ReturnCode" HeaderText="Return Code"></asp:BoundField>
		<asp:BoundField SortExpression="ReturnString" DataField="ReturnString" HeaderText="Return String"></asp:BoundField>
		<asp:BoundField SortExpression="BuilderGUID" DataField="BuilderGUID" HeaderText="Builder"></asp:BoundField>
		<asp:BoundField SortExpression="CreateDate" DataField="CreateDate" HeaderText="Create Date" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField SortExpression="SoapMethod" DataField="SoapMethod" HeaderText="Soap Method"></asp:BoundField>
	</Columns>
</CC:GridView>

</asp:content>

