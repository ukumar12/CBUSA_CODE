<%@ Page Language="VB" AutoEventWireup="false" Title="Plans Online - Projects" CodeFile="default.aspx.vb" Inherits="Projects"  %>

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

<div class="pckgwrpr bggray">
<div class="pckghdgltblue"> 
  Plans Online > <span style="font-size:20px;">Projects</span> 
    <span style="float:right;">
        <a href="#" id="aShowSearch" class="btnblue" onclick="ShowSearch();">Search Existing Projects</a>&nbsp;&nbsp;
        <CC:OneClickButton id="AddNew" Runat="server" Text="Add New Project" cssClass="btnred"></CC:OneClickButton>
    </span>
</div>
    <%--<asp:button id ="btnDashBoard" text="Go to DashBoard" class="btnred" runat="server" />--%>
<div class="pckgbdy">
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2" id="tblSearch" style="display:none;color:white">
<tr>
<th valign="top">Project Name:</th>
<td valign="top" class="field"><asp:textbox id="F_ProjectName" runat="server" Columns="40" MaxLength="50"></asp:textbox></td>

<th valign="top">Subdivision:</th>
<td valign="top" class="field"><asp:textbox id="F_Subdivision" runat="server" Columns="40" MaxLength="255"></asp:textbox></td>
</tr>
<tr>
<th valign="top">Lot Number:</th>
<td valign="top" class="field"><asp:textbox id="F_LotNumber" runat="server" Columns="40" MaxLength="50"></asp:textbox></td>

<th valign="top">Address 1:</th>
<td valign="top" class="field"><asp:textbox id="F_Address1" runat="server" Columns="40" MaxLength="255"></asp:textbox></td>
</tr>
<tr>
<th valign="top">City:</th>
<td valign="top" class="field"><asp:textbox id="F_City" runat="server" Columns="40" MaxLength="50"></asp:textbox></td>

<th valign="top"><b>State:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_State" runat="server" /></td>
</tr>
<tr>
<th valign="top">Zip:</th>
<td valign="top" class="field"><asp:textbox id="F_Zip" runat="server" Columns="40" MaxLength="50"></asp:textbox></td>

<th valign="top">Contact Name:</th>
<td valign="top" class="field"><asp:textbox id="F_ContactName" runat="server" Columns="40" MaxLength="100"></asp:textbox></td>
</tr>
<tr>
<th valign="top">Contact Email:</th>
<td valign="top" class="field"><asp:textbox id="F_ContactEmail" runat="server" Columns="40" MaxLength="100"></asp:textbox></td>

<th valign="top">Contact Phone:</th>
<td valign="top" class="field"><asp:textbox id="F_ContactPhone" runat="server" Columns="40" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top"><b>Is Archived:</b></th>
<td valign="top" class="field">
	<asp:DropDownList ID="F_IsArchived" runat="server">
		<asp:ListItem Value="">-- ALL --</asp:ListItem>
		<asp:ListItem Value="1">Yes</asp:ListItem>
		<asp:ListItem Value="0">No</asp:ListItem>
	</asp:DropDownList>
</td>
<th valign="top"><b>Submitted:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0" style="color:#0f2e51 ">
<tr><td class="smaller">From <CC:DatePicker id="F_SubmittedLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_SubmittedUbound" runat="server" /></td>
</tr>
</table>
</td>

</tr>
<tr>
<th valign="top"><b>Status:</b></th>
<td valign="top" class="field">
	<asp:DropDownList ID="F_Status" runat="server" />
		
</td>
<td colspan="2" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btnred" />
<input class="btnred" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />
<a href="#" id="a1" class="btnblue" onclick="HideSearch();">Hide Search</a>

</td>
</tr>
</table>
</asp:Panel>
<p></p>

<CC:GridView CausesValidation="false" id="gvList" class="tblcomprlen"  CellSpacing="2" CellPadding="2" runat="server" PageSize="10" AllowPaging="True" AllowSorting="True" 
HeaderText="In order to change display order, please use header links.<br>*Bid Requests (Number of Bid Requests in Bidding In Progress Status / Total Number of Bid Requests)" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False"  PagerSettings-Position="Bottom" BorderWidth="0" style="margin: 15px 0 15px 0px;"> 
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		
		<asp:TemplateField>
		    <%--<HeaderStyle forecolor="#ffffff" />--%>
		    <HeaderTemplate>
		        Edit
		    </HeaderTemplate>
			<ItemTemplate>
			    <asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?ProjectId=" & DataBinder.Eval(Container.DataItem, "ProjectId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ID="lnkEdit"  ImageUrl="/images/admin/edit.gif">Edit Project</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		
		<asp:BoundField SortExpression="ProjectName" DataField="ProjectName" HeaderText="Project Name"></asp:BoundField>
		<%--<asp:BoundField SortExpression="Status" DataField="Status" HeaderText="Status"></asp:BoundField>--%>
		<asp:BoundField SortExpression="Subdivision" DataField="Subdivision" HeaderText="Subdivision"></asp:BoundField>
		<asp:BoundField SortExpression="LotNumber" DataField="LotNumber" HeaderText="Lot #"></asp:BoundField>
		<%--<asp:BoundField SortExpression="City" DataField="City" HeaderText="City"></asp:BoundField>
		<asp:BoundField SortExpression="State" DataField="State" HeaderText="State"></asp:BoundField>--%>
		<asp:BoundField SortExpression="StartDate" DataField="StartDate" HeaderText="Start Date" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		
		<asp:TemplateField>
		    <ItemStyle width="280" HorizontalAlign="center" />
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" class="btnblue" runat="server" NavigateUrl= '<%# "quotes.aspx?F_ProjectId=" & DataBinder.Eval(Container.DataItem, "ProjectId") %>' ID="lnkQuotes">Bid Requests <b>(<%#DataBinder.Eval(Container.DataItem, "ActiveQuotes")%>/<%#DataBinder.Eval(Container.DataItem, "TotalQuotes")%>)</b></asp:HyperLink>
			<%--<asp:HyperLink enableviewstate="False" class="btnblue" runat="server" NavigateUrl= '<%# "quoterequests.aspx?F_ProjectId=" & DataBinder.Eval(Container.DataItem, "ProjectId") %>' ID="lnkQuoteRequests">Bid Status <b>(<%#DataBinder.Eval(Container.DataItem, "ActiveQuoteRequests")%>/<%#DataBinder.Eval(Container.DataItem, "TotalQuoteRequests")%>)</b></asp:HyperLink>--%>
			<asp:HyperLink enableviewstate="False" class="btnred" runat="server" NavigateUrl= '<%# "editquote.aspx?F_ProjectId=" & DataBinder.Eval(Container.DataItem, "ProjectId") %>' ID="lnkAddQuote">New Bid Request</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
		    <%--<HeaderStyle forecolor="#ffffff" />--%>
		    <HeaderTemplate>Delete</HeaderTemplate>
			<ItemStyle width="20" HorizontalAlign="center" />
			<ItemTemplate>
			    <CC:ConfirmImageButton CommandName="Remove" message="Are you sure that you want to remove this Project?" runat="server" ID="lnkDelete" ImageUrl="/images/admin/delete.gif" />
			</ItemTemplate>
		</asp:TemplateField>
	</Columns>
</CC:GridView>


</div>
</div>
</CT:MasterPage>

