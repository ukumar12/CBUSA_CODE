<%@ Page Language="VB" AutoEventWireup="false" CodeFile="default.aspx.vb" Inherits="projects_default" %>

<CT:MasterPage ID="CTMain" runat="server">
<asp:PlaceHolder runat="server">
<div class="pckgwrpr bggray">
<div class="pckghdgltblue">
    Projects
    <span style="float:right;">
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Project" cssClass="btnblue"></CC:OneClickButton>
    </span>
</div>
    <%--<asp:button id ="btnDashBoard" text="Go to DashBoard" class="btnred" runat="server" />--%>
<div class="pckgbdy">
    <table cellpadding="2" cellspacing="2" class="white">
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

<th valign="top">State:</th>
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
<th valign="top">Is Archived:</th>
<td valign="top" class="field">
	<asp:DropDownList ID="F_IsArchived" runat="server">
		<asp:ListItem Value="">-- ALL --</asp:ListItem>
		<asp:ListItem Value="1">Yes</asp:ListItem>
		<asp:ListItem Value="0">No</asp:ListItem>
	</asp:DropDownList>
</td>
<th valign="top">Submitted:</th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0" style="color:#0f2e51 ">
<tr><td class="smaller">From <CC:DatePicker id="F_SubmittedLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_SubmittedUbound" runat="server" /></td>
</tr>
</table>
</td>

</tr>
<tr>
<th valign="top">Status:</th>
<td valign="top" class="field">
	<asp:DropDownList ID="F_Status" runat="server" />
		
</td>
<td colspan="2" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btnred" />
<input class="btnred" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />

</td>
</tr>
</table>
</asp:Panel>
<p></p>

<CC:GridView CausesValidation="false" id="gvList"  CellSpacing="2" CellPadding="2" runat="server" PageSize="10" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links." EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom" class="tblcomprlen" style="margin: 15px 0 15px 0px;" >
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
		    <ItemStyle CssClass="ActionButtons" width="120" />
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?ProjectId=" & DataBinder.Eval(Container.DataItem, "ProjectId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ID="lnkEdit">Edit Project</asp:HyperLink>
			<asp:HyperLink enableviewstate="False" style="margin-top:3px;" Width="100%" runat="server" NavigateUrl='<%# "/order/history.aspx?F_ProjectID=" & DataBinder.Eval(Container.DataItem, "ProjectID") %>' ID="lnkOrders">Orders</asp:HyperLink>
			<asp:HyperLink enableviewstate="False" style="margin-top:3px;" Width="100%" runat="server" NavigateUrl= '<%# "/builder/plansonline/quotes.aspx?F_ProjectId=" & DataBinder.Eval(Container.DataItem, "ProjectId") %>' ID="lnkQuotes">Quotes</asp:HyperLink>
				<asp:HyperLink enableviewstate="False" style="margin-top:3px;" Width="100%" runat="server" NavigateUrl='<%# "/takeoffs/default.aspx?F_ProjectID=" & DataBinder.Eval(Container.DataItem, "ProjectID") %>' ID="lnkTakeOffs">Take-Offs</asp:HyperLink>
            <CC:ConfirmLinkButton CommandName="Remove" style="margin-top:3px;" Width="100%" Message="Are you sure that you want to remove this Project?" runat="server" ID="lnkDelete" Text="Delete" />
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="ProjectName" DataField="ProjectName" HeaderText="Project Name"></asp:BoundField>
		<asp:BoundField SortExpression="Status" DataField="Status" HeaderText="Status"></asp:BoundField>
		<asp:BoundField SortExpression="Subdivision" DataField="Subdivision" HeaderText="Subdivision"></asp:BoundField>
		<asp:BoundField SortExpression="City" DataField="City" HeaderText="City"></asp:BoundField>
		<asp:BoundField SortExpression="State" DataField="State" HeaderText="State"></asp:BoundField>
		<asp:BoundField SortExpression="ContactName" DataField="ContactName" HeaderText="Contact Name"></asp:BoundField>
		<asp:BoundField SortExpression="Submitted" DataField="Submitted" HeaderText="Submitted" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField SortExpression="StartDate" DataField="StartDate" HeaderText="Start Date" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:ImageField SortExpression="IsArchived" DataImageUrlField="IsArchived" HeaderText="Is Archived" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
	</Columns>
</CC:GridView>
</div>
</asp:PlaceHolder>
</CT:MasterPage>