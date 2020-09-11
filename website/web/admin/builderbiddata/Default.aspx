<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Builder Bid Data" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<script type="text/javascript">
    function ChangeTarget() {
        document.forms[0].target = '_blank';
        window.setTimeout('document.forms[0].target="_self"', 1000);
    }
</script>

<h4>Builder Bids Data Administration</h4>

<asp:HyperLink ID="lnkReturn" runat="server" Text="Return to Builders list"></asp:HyperLink><br /><br />

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top"><b>Builder:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_BuilderID" runat="server" /></td>
</tr>
 

<tr>
<th valign="top"><b>LLC:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_LLCID" runat="server" /></td>
</tr>
<tr>
<th valign="top">Historic ID:</th>
<td valign="top" class="field"><asp:textbox id="F_HistoricId" runat="server" Columns="50" MaxLength="5" TextMode="Number"></asp:textbox></td>
</tr>
<tr>
<th valign="top">Email:</th>
<td valign="top" class="field"><asp:textbox id="F_Email" runat="server" Columns="50" MaxLength="100"></asp:textbox></td>
</tr>
<tr>
<th valign="top"><b>Is New:</b></th>
<td valign="top" class="field">
	<asp:DropDownList ID="F_IsNew" runat="server">
		<asp:ListItem Value="">-- ALL --</asp:ListItem>
		<asp:ListItem Value="1">Yes</asp:ListItem>
		<asp:ListItem Value="0">No</asp:ListItem>
	</asp:DropDownList>
</td>
</tr>
<tr>
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
<th valign="top"><b>Submitted:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_CreateDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_CreateDateUbound" runat="server" /></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Total Bids:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding= cellspacing="0">
<tr><td>From<asp:textbox id="F_TotalBidsLBound" runat="server" Columns="5"  MaxLength="10"/></td><td>To<asp:textbox id="F_TotalBidsUBound" runat="server" Columns="5" MaxLength="10"/></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Active Bids:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding= cellspacing="0">
<tr><td>From<asp:textbox id="F_ActiveBidsLBound" runat="server" Columns="5" MaxLength="10"/></td><td>To<asp:textbox id="F_ActiveBidsUBound" runat="server" Columns="5" MaxLength="10"/></td>
</tr>
</table>
</td>
</tr>

<tr>
<th valign="top"><b>Awarded Bids:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding= cellspacing="0">
<tr><td>From<asp:textbox id="F_AwardedBidsLBound" runat="server" Columns="5" MaxLength="10"/></td><td>To<asp:textbox id="F_AwardedBidsUBound" runat="server" Columns="5" MaxLength="10"/></td>
</tr>
</table>
</td>
</tr>


<tr>
<th valign="top"><b>Awarded Total:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding= cellspacing="0">
<tr><td>From<asp:textbox id="F_AwardedTotalLBound" runat="server" Columns="7" MaxLength="10"/></td><td>To<asp:textbox id="F_AwardedTotalUBound" runat="server" Columns="7" MaxLength="10"/></td>
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
<tr>
<td colspan="2" align ="right"> 
<asp:button ID="Export" runat="server" Text="Export" CssClass="btn" />
</td>
</tr>
</table>
</asp:Panel>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		
        <asp:BoundField SortExpression="CompanyName" DataField="CompanyName" HeaderText="Builder"></asp:BoundField>
        <asp:TemplateField SortExpression = "TotalBids"  HeaderText = "Total Bids" ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
                <asp:HyperLink EnableViewState="False" runat="server"       ID="lnktotalbids"  ><%#Eval("TotalBids")%></asp:HyperLink>
                 
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField SortExpression="ActiveBids" DataField="ActiveBids" HeaderText="Active Bids" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
        <asp:BoundField SortExpression="AwardedBids" DataField="AwardedBids" HeaderText="Awarded Bids" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
        <asp:BoundField SortExpression="AwardedTotal" DataField="AwardedTotal" HeaderText="Awarded Total"  DataFormatString="{0:c}" ></asp:BoundField>
	    </Columns>
</CC:GridView>

</asp:content>
