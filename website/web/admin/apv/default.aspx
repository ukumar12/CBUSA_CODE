<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="APV" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>APV Administration</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top">Builder Name:</th>
<td valign="top" class="field"><asp:DropDownList ID="F_BuilderName" runat="server"></asp:DropDownList></td>

<th valign="top"><b>Historic Vendor ID:</b></th>
<td valign="top" class="field"><asp:textbox id="F_HistoricVendorID" runat="server" Columns="5"/>
</td>
</tr>
<tr>
<th valign="top">Vendor Name:</th>
<td valign="top" class="field"><asp:DropDownList ID="F_VendorName" runat="server"></asp:DropDownList></td>

<th valign="top"><b>Vendor ID:</b></th>
<td valign="top" class="field"><asp:textbox id="F_VendorID" runat="server" Columns="5"/>
</td>
</tr>
<tr>
<th valign="top"><b>Has Reported:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_HasReported" runat="server">
		<asp:ListItem Value="">-- ALL --</asp:ListItem>
		<asp:ListItem Value="Yes">Yes</asp:ListItem>
		<asp:ListItem Value="No">No</asp:ListItem>
	</asp:DropDownList></td>

<th valign="top">LLC:</th>
<td valign="top" class="field"><asp:ListBox ID="F_LLC" runat="server" SelectionMode="Multiple" Rows="8"></asp:ListBox></td>
</tr>

<tr>
<th valign="top"><b>Start Period Year:</b></th>
<td valign="top" class="field"><asp:textbox id="F_StartPeriodYear" runat="server" Columns="5"/>
</td>
<th valign="top"><b>Start Period Quarter:</b></th>
<td valign="top" class="field"><asp:textbox id="F_StartPeriodQuarter" runat="server" Columns="5"/>
</td>
</tr>

<tr>
<th valign="top"><b>End Period Year:</b></th>
<td valign="top" class="field"><asp:textbox id="F_EndPeriodYear" runat="server" Columns="5"/>
</td>
<th valign="top"><b>End Period Quarter:</b></th>
<td valign="top" class="field"><asp:textbox id="F_EndPeriodQuarter" runat="server" Columns="5"/>
</td>
</tr>

<tr>
<th valign="top"><b>Dispute Response:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_DisputeResponse" runat="server" /></td>

<th valign="top"><b>Dispute Response Reason:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_DisputeResponseReason" runat="server" /></td>
</tr>
<tr>
<td colspan="2" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
<asp:button id="btnExport" Runat="server" Text="Export" cssClass="btn" />
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

