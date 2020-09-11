<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Contact Us" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Contact Us Administration</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top">Full Name:</th>
<td valign="top" class="field"><asp:textbox id="F_FullName" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top">Email:</th>
<td valign="top" class="field"><asp:textbox id="F_Email" runat="server" Columns="50" MaxLength="100"></asp:textbox></td>
</tr>
<tr>
<th valign="top">Order Number:</th>
<td valign="top" class="field"><asp:textbox id="F_OrderNumber" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top">Phone:</th>
<td valign="top" class="field"><asp:textbox id="F_Phone" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top"><b>Question:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_QuestionId" runat="server" /></td>
</tr>
<tr>
<th valign="top"><b>Create Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_CreateDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_CreateDateUbound" runat="server" /></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Output As:</b></th>
<td valign="top" class="field">
<asp:DropDownList id="F_OutputAs" runat="server">
	<asp:ListItem value="HTML">HTML Page</asp:ListItem>
	<asp:ListItem value="Excel">Excel Document</asp:ListItem>
</asp:DropDownList>
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

<CC:GridView id="gvList" GridLines="none" CssClass="MultilineTable" CellSpacing="0" CellPadding="5" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?ContactUsId=" & DataBinder.Eval(Container.DataItem, "ContactUsId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate="False" Message="Are you sure that you want to remove this Contact Us?" runat="server" NavigateUrl= '<%# "delete.aspx?ContactUsId=" & DataBinder.Eval(Container.DataItem, "ContactUsId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="FullName" DataField="FullName" HeaderText="Full Name"></asp:BoundField>
		<asp:BoundField SortExpression="Email" DataField="Email" HeaderText="Email"></asp:BoundField>
		<asp:BoundField SortExpression="OrderNumber" DataField="OrderNumber" HeaderText="Order Number"></asp:BoundField>
		<asp:BoundField SortExpression="Phone" DataField="Phone" HeaderText="Phone"></asp:BoundField>
		<asp:BoundField SortExpression="HowHeardName" DataField="HowHeardName" HeaderText="How Heard"></asp:BoundField>
		<asp:BoundField SortExpression="Question" DataField="Question" HeaderText="Question"></asp:BoundField>
		<asp:BoundField SortExpression="YourMessage" DataField="YourMessage" HeaderText="YourMessage"></asp:BoundField>
		<asp:BoundField SortExpression="CreateDate" DataField="CreateDate" HeaderText="Create Date" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
	</Columns>
</CC:GridView>

<div runat="server" id="divDownload" visible="false">
<asp:HyperLink id="lnkDownload" runat="server">Download File</asp:HyperLink>
</div>

</asp:content>

