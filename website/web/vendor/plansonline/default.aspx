<%@ Page Language="VB" AutoEventWireup="false" Title="Plans Online - Quote Requests" CodeFile="default.aspx.vb" Inherits="POVendorQuoteRequests"  %>

<CT:MasterPage ID="CTMain" runat="server">
<script type="text/javascript">
    function ShowSearch() {
        document.getElementById("tblSearch").style.display = "block";
    }
    function HideSearch() {
        document.getElementById("tblSearch").style.display = "none";
    }
</script>
<div class="pckgwrpr bggray">
<div class="pckghdgltblue" style="height:23px;">
    Plans Online > <span style="font-size:20px;">Bid Requests</span>
    <span style="float:right;">
        <a href="#" id="aShowSearch" class="btnblue" onclick="ShowSearch();">Search Bid Requests</a>&nbsp;&nbsp;
    </span>
</div>
<div class="pckgbdy">
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2" id="tblSearch" style="display:none;" class="white">
<tr>
<th valign="top"><b>Project:</b></th>
<td valign="top" class="field"><asp:textbox id="F_Project" runat="server" Columns="50" MaxLength="100" /></td>
<th valign="top"><b>Supply Phase:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_VendorCategoryId" runat="server" /></td>
</tr>
<tr>
<th valign="top"><b>Bid Request:</b></th>
<td valign="top" class="field"><asp:textbox id="F_Quote" runat="server" Columns="50" MaxLength="100" /></td>
<th valign="top"><b>Bid Request #:</b></th>
<td valign="top" class="field"><asp:textbox id="F_QuoteNumber" runat="server" Columns="50" MaxLength="100" /></td>
</tr>
<tr>
<th valign="top"><b>Builder:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_BuilderId" runat="server" /></td>
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
</tr>
<tr>
<th valign="top">Builder Contact Name:</th>
<td valign="top" class="field"><asp:textbox id="F_BuilderContactName" runat="server" Columns="50" MaxLength="100"></asp:textbox></td>
<th valign="top">Builder Contact Email:</th>
<td valign="top" class="field"><asp:textbox id="F_BuilderContactEmail" runat="server" Columns="50" MaxLength="100"></asp:textbox></td>
</tr>
<tr>
<th valign="top"><b>Builder Document:</b></th>
<td valign="top" class="field"><asp:textbox id="F_BuilderDocument" runat="server" Columns="50" MaxLength="100" /></td>
<th valign="top"><b>Vendor Document:</b></th>
<td valign="top" class="field"><asp:textbox id="F_VendorDocument" runat="server" Columns="50" MaxLength="100" /></td>
</tr>
<tr>
<th valign="top"><b>Total:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0" style="color:#0f2e51 ">
<tr><td class="smaller">From<asp:textbox id="F_QuoteTotalLBound" runat="server" Columns="5" MaxLength="10"/></td><td class="smaller">To<asp:textbox id="F_QuoteTotalUbound" runat="server" Columns="5" MaxLength="10"/></td>
</tr>
</table>
</td>
<th valign="top"><b>Expiration Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0" style="color:#0f2e51 ">
<tr><td class="smaller">From <CC:DatePicker id="F_QuoteExpirationDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_QuoteExpirationDateUbound" runat="server" /></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Create Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0" style="color:#0f2e51 ">
<tr><td class="smaller">From <CC:DatePicker id="F_CreateDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_CreateDateUbound" runat="server" /></td>
</tr>
</table>
</td>
<th valign="top"><b>Bid Request Deadline:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0" style="color:#0f2e51 ">
<tr><td class="smaller">From <CC:DatePicker id="F_DeadlineLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_DeadlineUbound" runat="server" /></td>
</tr>
</table>
</td>
</tr>
<tr>
<td colspan="4" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btnred" />
<input class="btnred" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />
<a href="#" id="a1" class="btnblue" onclick="HideSearch();">Hide Search</a>
</td>
</tr>
</table>
</asp:Panel>
<center>
<div class="pckgwrpr bggray center" id="divMsg" runat="server" visible="false" style="width:800px;">
    <div class="pckghdgblue">Bid Submission Confirmation</div>
    <div style="text-align:center;">
        <p class="bold green"><asp:Literal id="ltlMsg" runat="server" /></p>
    </div>
</div>
</center>

<CC:GridView  id="gvList" Width="100%" class="tblcompr" style="margin: 15px 0 15px 0px;" CellSpacing="2" CellPadding="2" runat="server" PageSize="10" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links.<br>*Details (Unread Messages from Builder / Total Number of Messages from Builder).<br>*RFIs (Number of Posts added Today by Builder / Total Number of Posts)" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField SortExpression="Builder" HeaderText="Builder">
		<ItemStyle width="150" />
			<ItemTemplate>
			    <asp:literal id="ltlContact" runat="server"></asp:literal>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField SortExpression="Project" HeaderText="Project">
		<ItemStyle />
			<ItemTemplate>
			    <asp:literal id="ltlProject" runat="server"></asp:literal>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField SortExpression="Quote" HeaderText="Bid Request">
		<ItemStyle />
			<ItemTemplate>
			    <asp:literal id="ltlQuote" runat="server"></asp:literal>
			</ItemTemplate>
		</asp:TemplateField>
		
		<asp:BoundField SortExpression="RequestStatus" DataField="RequestStatus" HeaderText="Status"></asp:BoundField>
		<asp:BoundField SortExpression="QuoteTotal" DataField="QuoteTotal" HeaderText="Total" DataFormatString="{0:c}"></asp:BoundField>
		<asp:BoundField SortExpression="QuoteExpirationDate" DataField="QuoteExpirationDate" HeaderText="Expires" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField SortExpression="Deadline" DataField="Deadline" HeaderText="Deadline" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:TemplateField SortExpression="LastMessageDate" HeaderText="Latest Message">
		<ItemStyle width="300" />
			<ItemTemplate>
			    <asp:literal id="ltlMessage" runat="server"></asp:literal>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
		    <ItemStyle CssClass="ActionButtons" width="120" />
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False"  class="btnred" runat="server" NavigateUrl= '<%# "quoterequestmessages.aspx?QuoteRequestId=" & DataBinder.Eval(Container.DataItem, "QuoteRequestId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ID="lnkQuoteRequests">Details <b>(<%#DataBinder.Eval(Container.DataItem, "UnreadMessages")%>/<%#DataBinder.Eval(Container.DataItem, "TotalMessages")%>)</b></asp:HyperLink>
			<asp:HyperLink enableviewstate="False" class="btnred" runat="server" NavigateUrl= '<%# "quoterequestmessages.aspx?QuoteRequestId=" & DataBinder.Eval(Container.DataItem, "QuoteRequestId") & "&" & GetPageParams(Components.FilterFieldType.All) & "&#rfi" %>' ID="lnkRFI">RFIs <b>(<%#DataBinder.Eval(Container.DataItem, "NewPosts")%>/<%#DataBinder.Eval(Container.DataItem, "TotalPosts")%>)</b></asp:HyperLink>
			<asp:HyperLink enableviewstate="False"  class="btnblue" runat="server" NavigateUrl= '<%# "quoterequestmessages.aspx?QuoteRequestId=" & DataBinder.Eval(Container.DataItem, "QuoteRequestId") & "&" & GetPageParams(Components.FilterFieldType.All) & "&#submitbid" %>' ID="lnkSubmitBid">Submit Bid</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
	</Columns>
</CC:GridView>






</div>
</div>
</CT:MasterPage>

