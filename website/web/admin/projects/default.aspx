<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Project" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Project Administration</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top"><b>Builder:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_BuilderID" runat="server" /></td>
</tr>
<tr>
<th valign="top">Project Name:</th>
<td valign="top" class="field"><asp:textbox id="F_ProjectName" runat="server" Columns="50" MaxLength="150"></asp:textbox></td>
</tr>
<tr>
<th valign="top">Lot Number:</th>
<td valign="top" class="field"><asp:textbox id="F_LotNumber" runat="server" Columns="20" MaxLength="20"></asp:textbox></td>
</tr>
<tr>
<th valign="top">Subdivision:</th>
<td valign="top" class="field"><asp:textbox id="F_Subdivision" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top">City:</th>
<td valign="top" class="field"><asp:textbox id="F_City" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top"><b>State:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_State" runat="server" /></td>
</tr>
<tr>
<th valign="top">Zip:</th>
<td valign="top" class="field"><asp:textbox id="F_Zip" runat="server" Columns="15" MaxLength="15"></asp:textbox></td>
</tr>
<tr>
<th valign="top">County:</th>
<td valign="top" class="field"><asp:textbox id="F_County" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top"><b>Portfolio:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_PortfolioID" runat="server" /></td>
</tr>
<tr>
<th valign="top"><b>Project Status:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_ProjectStatusID" runat="server" /></td>
</tr>
<tr>
<th valign="top"><b>Start Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_StartDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_StartDateUbound" runat="server" /></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Submitted:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_SubmittedLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_SubmittedUbound" runat="server" /></td>
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
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Project" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?ProjectID=" & DataBinder.Eval(Container.DataItem, "ProjectID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate="False" Message="Are you sure that you want to remove this Project?" runat="server" NavigateUrl= '<%# "delete.aspx?ProjectID=" & DataBinder.Eval(Container.DataItem, "ProjectID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="BuilderName" DataField="BuilderName" HeaderText="Builder"></asp:BoundField>
		<asp:BoundField SortExpression="ProjectName" DataField="ProjectName" HeaderText="Project Name"></asp:BoundField>
		<asp:BoundField SortExpression="LotNumber" DataField="LotNumber" HeaderText="Lot Number"></asp:BoundField>
		<asp:BoundField SortExpression="Subdivision" DataField="Subdivision" HeaderText="Subdivision"></asp:BoundField>
		<asp:BoundField SortExpression="StartDate" DataField="StartDate" HeaderText="Start Date" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField SortExpression="Submitted" DataField="Submitted" HeaderText="Submitted" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
	</Columns>
</CC:GridView>

</asp:content>

