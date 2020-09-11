<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Mailing Member" CodeFile="default.aspx.vb" Inherits="index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Mailing Members</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top">Email:</th>
<td valign="top" class="field"><asp:textbox id="F_Email" runat="server" Columns="50" MaxLength="255"></asp:textbox></td>
</tr>
<tr>
<th valign="top">Name:</th>
<td valign="top" class="field"><asp:textbox id="F_Name" runat="server" Columns="50" MaxLength="255"></asp:textbox></td>
</tr>
<tr>
<th valign="top">Join Date:</th>
<td valign="top" class="field">

<table cellpadding="0" cellspacing="0">
<tr><td class="smallest">From</td><td>&nbsp;</td><td class="smallest">To</td></tr>
<tr><td><CC:DatePicker ID="F_JoinDate_Lower" runat="server"></CC:DatePicker></td><td>&nbsp;</td><td><CC:DatePicker ID="F_JoinDate_Upper" runat="server"></CC:DatePicker></td></tr></table></td>
<td>
<CC:DateValidator runat="server" ID="dvJoinDate_Lower" Display="Dynamic" ControlToValidate="F_JoinDate_Lower" ErrorMessage="Field 'Join Date From' is invalid" ></CC:DateValidator><br />
<CC:DateValidator runat="server" ID="dvJoinDate_Upper" Display="Dynamic" ControlToValidate="F_JoinDate_Upper" ErrorMessage="Field 'Join Date To' is invalid" ></CC:DateValidator>
</td>
</tr>
<tr>
<th valign="top">E-mail format:</th>
<td valign="top" class="field"><asp:RadioButtonList  id="F_MimeType" runat="server" RepeatDirection="Horizontal" >
    <asp:ListItem Value="" Text="All"></asp:ListItem>
    <asp:ListItem Value="HTML">HTML</asp:ListItem>
    <asp:ListItem Value="TEXT">Plain Text</asp:ListItem>
</asp:RadioButtonList></td>
</tr>
<tr>
<th valign="top">Lists:</th>
<td valign="top" class="field"><CC:CheckBoxListEx  ID="F_ListId" runat="server" RepeatColumns="2"></CC:CheckBoxListEx></td>
</tr>

<tr>
<td colspan="2" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />&nbsp;
    <CC:OneClickButton ID="btnExport" runat="server" CssClass="btn" Text="Export" />&nbsp;
<input class="btn" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />
</td>
</tr>
</table>
</asp:Panel>

<p></p>
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Mailing Member" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate"></AlternatingRowStyle>
	<RowStyle CssClass="row"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "edit.aspx?MemberId=" & DataBinder.Eval(Container.DataItem, "MemberId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate=False Message="Are you sure that you want to remove this Mailing Member?" runat="server" NavigateUrl= '<%# "delete.aspx?MemberId=" & DataBinder.Eval(Container.DataItem, "MemberId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="Email" DataField="Email" HeaderText="Email"></asp:BoundField>
		<asp:BoundField SortExpression="Name" DataField="Name" HeaderText="Name"></asp:BoundField>
		<asp:BoundField SortExpression="MimeType" DataField="MimeType" HeaderText="E-mail format"></asp:BoundField>
		<asp:BoundField SortExpression="CreateDate" DataField="CreateDate" HeaderText="Join Date"></asp:BoundField>
	</Columns>
</CC:GridView>

<div runat="server" id="divDownload" visible="false">
<asp:HyperLink id="lnkDownload" runat="server">Download File</asp:HyperLink>
</div>

</asp:content>
