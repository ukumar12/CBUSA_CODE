<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Sales Report Dispute" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Sales Report Dispute Administration</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top"><b>Sales Report I D:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding= cellspacing="0">
<tr><td>From<asp:textbox id="F_SalesReportIDLBound" runat="server" Columns="5" MaxLength="10"/></td><td>To<asp:textbox id="F_SalesReportIDUBound" runat="server" Columns="5" MaxLength="10"/></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Dispute Reponse I D:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_DisputeResponseID" runat="server" /></td>
</tr>
<tr>
<th valign="top"><b>Dispute Reponse Reason I D:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_DisputeResponseReasonID" runat="server" /></td>
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
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Sales Report Dispute" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?SalesReportDisputeID=" & DataBinder.Eval(Container.DataItem, "SalesReportDisputeID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate="False" Message="Are you sure that you want to remove this Sales Report Dispute?" runat="server" NavigateUrl= '<%# "delete.aspx?SalesReportDisputeID=" & DataBinder.Eval(Container.DataItem, "SalesReportDisputeID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="SalesReportID" DataField="SalesReportID" HeaderText="Sales Report I D"></asp:BoundField>
		<asp:BoundField SortExpression="Created" DataField="Created" HeaderText="Created" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField DataField="BuilderTotalAmount" HeaderText="Builder Total Amount" DataFormatString="{0:c}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField DataField="VendorTotalAmount" HeaderText="Vendor Total Amount" DataFormatString="{0:c}" HTMLEncode="False"></asp:BoundField>
	</Columns>
</CC:GridView>

</asp:content>
