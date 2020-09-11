<%@ Page Language="VB" AutoEventWireup="false" Title="Plans Online - Bid Requests" CodeFile="quotes.aspx.vb" Inherits="POQuotes"  %>

<CT:MasterPage ID="CTMain" runat="server">
<style>
.bidMsg{
background-color: white;
display: block;
border-color: red;
border-style: solid;
vertical-align: middle;
text-align: left;
border-width: 1px;
padding: 10px;
text-align: center;
font-size: 12px;
font-weight: bold;
}
</style>
<script type="text/javascript">
    function ShowSearch() {
        document.getElementById("tblSearch").style.display = "block";
    }
    function HideSearch() {
        document.getElementById("tblSearch").style.display = "none";
    }
</script>

<div class="pckgwrpr bggray">
<div class="pckghdgltblue">
     Plans Online > <a href='default.aspx' >Projects</a> > <span style="font-size:20px;">Bid Requests</span>
    <span style="float:right;">
        <a href="#" id="aShowSearch" class="btnblue" onclick="ShowSearch();">Search Existing Bid Requests</a>&nbsp;&nbsp;
        <CC:OneClickButton id="AddNew" Runat="server" Text="Add New Bid Request" cssClass="btnred"></CC:OneClickButton>
    </span>
</div>
<div class="pckgbdy">
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2" id="tblSearch" style="display:none;"class="white">
<tr>
<th valign="top"><b>Project:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_ProjectId" runat="server" /></td>
<th valign="top"><b>Supply Phase:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_VendorCategoryId" runat="server" /></td>
</tr>
<tr>
<th valign="top">Bid Request Title:</th>
<td valign="top" class="field"><asp:textbox id="F_Title" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
<th valign="top"><b>Deadline:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0" style="color:#0f2e51 ">
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
<table border="0" cellpadding="0" cellspacing="0" style="color:#0f2e51 ">
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
<table border="0" cellpadding="0" cellspacing="0" style="color:#0f2e51 ">
<tr><td class="smaller">From <CC:DatePicker id="F_AwardedDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_AwardedDateUbound" runat="server" /></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Awarded Total:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0" style="color:#0f2e51 ">
<tr><td class="smaller">From<asp:textbox id="F_AwardedTotalLBound" runat="server" Columns="5" MaxLength="10"/></td><td class="smaller">To<asp:textbox id="F_AwardedTotalUbound" runat="server" Columns="5" MaxLength="10"/></td>
</tr>
</table>
</td>

<th valign="top"><b>Create Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0" style="color:#0f2e51 ">
<tr><td class="smaller">From <CC:DatePicker id="F_CreateDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_CreateDateUbound" runat="server" /></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Document:</b></th>
<td valign="top" class="field"><asp:textbox id="F_BuilderDocument" runat="server" Columns="50" MaxLength="100" /></td>
<td align="right" colspan="2">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btnred" />
<input class="btnred" type="submit" value="Clear" onclick="window.location='quotes.aspx';return false;" />
<input class="btnblue" type="submit" value="Projects" onclick="window.location='default.aspx';return false;" />
<a href="#" id="a1" class="btnblue" onclick="HideSearch();">Hide Search</a>
</td>
</tr>
</table>
</asp:Panel>
<p></p>

<div class="pckgltgraywrpr center" id="divMsg" runat="server" visible="false">
    <div class="pckghdgred">Bid Request Update Confirmation</div>
    <div style="text-align:center;">
        <p class="bold "><asp:Literal id="ltlMsg" runat="server" /></p>
    </div>
</div>

<p></p>
<asp:Literal id="ltlProjectHeader" runat="server"></asp:Literal>


<CC:GridView CausesValidation="false" id="gvList"  CellSpacing="2" CellPadding="2" runat="server" PageSize="20" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links.<br>*Bid Comparison Matrix (Number of Active Bid Requests with Unread Messages)" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False"  PagerSettings-Position="Bottom" BorderWidth="0" class="tblcomprlen" Width="100%" style="margin:15px 0 15px 0px">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
		    <HeaderStyle forecolor="#ffffff" />
		    <HeaderTemplate>
		        Edit
		    </HeaderTemplate>
			<ItemTemplate>
			    <asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "editquote.aspx?QuoteId=" & DataBinder.Eval(Container.DataItem, "QuoteId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ID="lnkEdit" ImageUrl="/images/admin/edit.gif">Edit Quote</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="QuoteId" DataField="QuoteId" HeaderText="#"></asp:BoundField>
		<asp:BoundField SortExpression="Project" DataField="Project" HeaderText="Project"></asp:BoundField>
		<asp:BoundField SortExpression="Title" DataField="Title" HeaderText="Title"></asp:BoundField>
		<asp:TemplateField SortExpression="Status" HeaderText="Status">
			<ItemTemplate>
			    <asp:Literal id="ltlStatus" runat="server"></asp:Literal>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
		    <HeaderTemplate>
		        Phases
		    </HeaderTemplate>
		    <ItemTemplate>
		        <asp:Literal id="ltlPhases" runat="server"></asp:Literal>
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
		<asp:TemplateField>
		    <ItemStyle width="250" HorizontalAlign="Center" />
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" class="btnblue" runat="server" NavigateUrl= '<%# "quoterequests.aspx?F_QuoteId=" & DataBinder.Eval(Container.DataItem, "QuoteId") & "&" & GetPageParams(Components.FilterFieldType.All, "F_pg;F_SortBy;F_SortOrder") %>' ID="lnkQuoteRequests">Bid Comparison Matrix <b>(<%#DataBinder.Eval(Container.DataItem, "ActiveQuoteRequests")%>)</b></asp:HyperLink>
			<%--<asp:HyperLink enableviewstate="False" class="btnred" runat="server" NavigateUrl= '<%# "editquote.aspx?QuoteId=" & DataBinder.Eval(Container.DataItem, "QuoteId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ID="lnkSubmit">Submit</asp:HyperLink>
			<asp:HyperLink enableviewstate="False" class="btnred" runat="server" NavigateUrl= '<%# "editquote.aspx?QuoteId=" & DataBinder.Eval(Container.DataItem, "QuoteId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ID="lnkCancel">Cancel</asp:HyperLink>--%>
			<CC:ConfirmLinkButton CommandName="Copy" class="btnred" Message="Are you sure that you want to create a copy of this Bid Request?" runat="server" ID="lnkCopy" Text="Copy" />
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
		<HeaderStyle forecolor="#ffffff" />
		    <HeaderTemplate>Delete</HeaderTemplate>
			<ItemStyle width="20" HorizontalAlign="center" />
			<ItemTemplate>
			    <CC:ConfirmImageButton CommandName="Remove" message="Are you sure that you want to remove this Bid Request?" runat="server" ID="lnkDelete" ImageUrl="/images/admin/delete.gif" />
			</ItemTemplate>
		</asp:TemplateField>
	</Columns>
</CC:GridView>




</div>
</div>
</CT:MasterPage>

