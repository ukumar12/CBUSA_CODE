<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Vendor Bid Data" CodeFile="displayVendorBids.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<script type="text/javascript">
    function ChangeTarget() {
        document.forms[0].target = '_blank';
        window.setTimeout('document.forms[0].target="_self"', 1000);
    }
</script>
<br />
<asp:HyperLink ID="lnkReturn" runat="server" Text="Return to Vendor Bids Data"></asp:HyperLink><br />
<asp:Literal id="ltlCompanyName" runat="server"></asp:Literal>
<h4>Vendor Bid  Search</h4>
<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top"><b>Project: </b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_ProjectID" runat="server" /></td>
<th valign="top"><b>Builder:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_BuilderId" runat="server" /></td>
</tr>
<tr>
<th valign="top"><b>Request Status:</b></th>
<td valign="top" class="field">
    <asp:DropDownList ID="F_RequestStatus" runat="server" >
        <asp:ListItem Value="All">-- ALL --</asp:ListItem>
		<asp:ListItem Value="Active">Active</asp:ListItem>
		<asp:ListItem Value="New">New</asp:ListItem>
		<asp:ListItem Value="Request Information">Request Information</asp:ListItem>
		<asp:ListItem Value="Awarded">Awarded</asp:ListItem>
		<asp:ListItem Value="Declined By Builder">Declined By Builder</asp:ListItem>
		<asp:ListItem Value="Declined By Vendor">Declined By Vendor</asp:ListItem>
		<asp:ListItem Value="Exited Market">Exited Market</asp:ListItem>
		<asp:ListItem Value="Cancelled">Cancelled</asp:ListItem>
    </asp:DropDownList>
</td>
<th valign="top"><b>Bid Request:</b></th>
<td valign="top" class="field"><asp:textbox id="F_Quote" runat="server" Columns="50" MaxLength="100" /></td>
</tr>



<tr>
<th valign="top"><b>Vendor Document:</b></th>
<td valign="top" class="field"><asp:textbox id="F_VendorDocument" runat="server" Columns="50" MaxLength="100" /></td>
<th valign="top"><b>Builder Document:</b></th>
<td valign="top" class="field"><asp:textbox id="F_BuilderDocument" runat="server" Columns="50" MaxLength="100" /></td>
</tr>


<tr>
<th valign="top"><b>  Total:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td>From<asp:textbox id="F_QuoteTotalLBound" runat="server" Columns="7" MaxLength="10"/></td><td>To<asp:textbox id="F_QuoteTotalUBound" runat="server" Columns="7" MaxLength="10"/></td>
</tr>
</table>
</td>
<th valign="top"><b>Expiration Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_QuoteExpirationDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_QuoteExpirationDateUbound" runat="server" /></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Create Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_CreateDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_CreateDateUbound" runat="server" /></td>
</tr>
</table>
</td>
<th valign="top"><b>Bid Request Deadline:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_DeadlineLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_DeadlineUbound" runat="server" /></td>
</tr>
</table>
</td>
</tr>
<%--<th valign="top"><b>Create Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_CreateDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_CreateDateUbound" runat="server" /></td>
</tr>
</table>
</td>
</tr>--%>

<tr  >
<td  colspan="4" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
<%--<CC:OneClickButton id="btnReset" Runat="server" Text="Clear" cssClass="btn" />--%>
 <input class="btn" type="reset" value="Clear"  />
</td>
</tr>
</table>
</asp:Panel>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="5" runat="server" PageSize="20" AllowPaging="True" AllowSorting="True" CausesValidation="false" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		
      
        <asp:BoundField SortExpression="Builder" DataField="Builder" HeaderText="Builder"></asp:BoundField>
		<asp:BoundField SortExpression="Project" DataField="Project" HeaderText="Project"></asp:BoundField>
        <asp:BoundField SortExpression="Quote" DataField="Quote" HeaderText="Bid Request"></asp:BoundField>
        <asp:BoundField SortExpression="RequestStatus" DataField="RequestStatus" HeaderText="Status"></asp:BoundField>
        <asp:BoundField SortExpression="QuoteTotal" DataField="QuoteTotal" HeaderText="Total" DataFormatString="{0:c}" ></asp:BoundField>
        <asp:BoundField SortExpression="QuoteExpirationDate" DataField="QuoteExpirationDate" HeaderText="Expires" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
        <asp:BoundField SortExpression="Deadline" DataField="Deadline" HeaderText="Deadline" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		  </Columns>
</CC:GridView>

</asp:content>
