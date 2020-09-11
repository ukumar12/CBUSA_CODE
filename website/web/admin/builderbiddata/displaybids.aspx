<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Builder Bid Data" CodeFile="displaybids.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<script type="text/javascript">
    function ChangeTarget() {
        document.forms[0].target = '_blank';
        window.setTimeout('document.forms[0].target="_self"', 1000);
    }
</script>
<br />
<asp:HyperLink ID="lnkReturn" runat="server" Text="Return to Builder Bids Data"></asp:HyperLink><br />
<asp:Literal id="ltlCompanyName" runat="server"></asp:Literal>
<h4>Bid Details Search</h4>
<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top"><b>Project: </b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_ProjectID" runat="server" /></td>
<th valign="top"><b>Supply Phase:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_VendorCategoryId" runat="server" /></td>
</tr>
<tr>
<th valign="top">Bid Request Title:</th>
<td valign="top" class="field"><asp:textbox id="F_Title" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
<th valign="top"><b>Deadline:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_DeadlineLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_DeadlineUbound" runat="server" /></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Status:</b></th>
<td valign="top" class="field">
    <asp:DropDownList ID="F_Status" runat="server" >
        <asp:ListItem Value="All">-- ALL --</asp:ListItem>
		<asp:ListItem Value="Active">Active</asp:ListItem>
		<asp:ListItem Value="New">Pending - Bid Request Not Sent</asp:ListItem>
		<asp:ListItem Value="Bidding In Progress">Bidding In Progress</asp:ListItem>
		<asp:ListItem Value="Awarded">Awarded</asp:ListItem>
		<asp:ListItem Value="Cancelled">Cancelled</asp:ListItem>
    </asp:DropDownList>
</td>

<th valign="top"><b>Status Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_StatusDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_StatusDateUbound" runat="server" /></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Awarded To Vendor:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_AwardedToVendorId" runat="server" style="width:200px" /></td>
<th valign="top"><b>Awarded Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_AwardedDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_AwardedDateUbound" runat="server" /></td>
</tr>
</table>
</td>
</tr>

<tr>
<th valign="top"><b>Awarded Total:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td>From<asp:textbox id="F_AwardedTotalLBound" runat="server" Columns="5" MaxLength="10"/></td><td>To<asp:textbox id="F_AwardedTotalUbound" runat="server" Columns="5" MaxLength="10"/></td>
</tr>
</table>
</td>

<th valign="top"><b>Create Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_CreateDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_CreateDateUbound" runat="server" /></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Document:</b></th>
<td valign="top" class="field"><asp:textbox id="F_BuilderDocument" runat="server" Columns="50" MaxLength="100" /></td>
</tr>
<tr  >
<td  colspan="4" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
  <input class="btn" type="submit" onclick="window.location='displaybids.aspx?BuilderId=<%=BuilderID%>';return false;" value="Clear"  />
</td>
</tr>
</table>
</asp:Panel>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="20" AllowPaging="True" AllowSorting="True" CausesValidation="false" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		
      
        <asp:BoundField SortExpression="QuoteId" DataField="QuoteId" HeaderText="Quote #"></asp:BoundField>
		<asp:BoundField SortExpression="Project" DataField="Project" HeaderText="Project"></asp:BoundField>
		<asp:BoundField SortExpression="Title" DataField="Title" HeaderText="Title"></asp:BoundField>
		<asp:TemplateField SortExpression="Status" HeaderText="Status">
			<ItemTemplate>
			    <asp:Literal id="ltlStatus" runat="server"></asp:Literal>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField ItemStyle-Width="20%" >
		    <HeaderTemplate  >
		        Phases
		    </HeaderTemplate>
		    <ItemTemplate>
		        <asp:Literal  id="ltlPhases" runat="server"></asp:Literal>
		    </ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="Deadline" DataField="Deadline" HeaderText="Deadline" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField SortExpression="CreateDate" DataField="CreateDate" HeaderText="Create Date" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:TemplateField>
		    <HeaderTemplate>
		        Bids Received /<br /> Total Active Requests
		    </HeaderTemplate>
		    <ItemTemplate>
		        <b><%#DataBinder.Eval(Container.DataItem, "BidsReceived")%>/<%#DataBinder.Eval(Container.DataItem, "TotalQuoteRequests")%></b>
		    </ItemTemplate>
		</asp:TemplateField>


	    </Columns>
</CC:GridView>

</asp:content>
