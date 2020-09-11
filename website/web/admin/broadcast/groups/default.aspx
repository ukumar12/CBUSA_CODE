<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Mailing List" CodeFile="default.aspx.vb" Inherits="index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Recipient Group</h4>

<span class="smaller">Please provide search criteria below</span>
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top">Name:</th>
<td valign="top" class="field"><asp:textbox id="F_Name" runat="server" Columns="50" MaxLength="255" /><br />
Display number of subscribers for each group? &nbsp;<asp:Checkbox id="F_Qty" runat="server" />
</td>
</tr>
<tr>
<td colspan="2" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />&nbsp;
<input class="btn" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />
</td>
</tr>
</table>

<p></p>
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Recipient Group" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate"></AlternatingRowStyle>
	<RowStyle CssClass="row"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "edit.aspx?GroupId=" & Container.DataItem("GroupId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate=False Message="Are you sure that you want to remove this Recipient Group?" runat="server" NavigateUrl= '<%# "delete.aspx?GroupId=" & Container.DataItem("GroupId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="Name" DataField="Name" HeaderText="Name"></asp:BoundField>
		<asp:BoundField DataField="Description" HeaderText="Description"></asp:BoundField>
		<asp:BoundField DataField="HtmlMembers" HeaderText="HTML Members" />
		<asp:BoundField DataField="TextMembers" HeaderText="Text Members"  />
		<asp:BoundField DataField="TotalMembers" HeaderText="Total Members"  />
	</Columns>
</CC:GridView>

</asp:content>
