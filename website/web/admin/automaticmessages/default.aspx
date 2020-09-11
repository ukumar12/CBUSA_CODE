<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Automatic Messages" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Automatic Messages Administration</h4>
<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top">Title:</th>
<td valign="top" class="field"><asp:textbox id="F_Title" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top">Subject:</th>
<td valign="top" class="field"><asp:textbox id="F_Subject" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
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
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Automatic Messages" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?AutomaticMessageID=" & DataBinder.Eval(Container.DataItem, "AutomaticMessageID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink visible='<%#LoggedInIsInternal %>' enableviewstate="False" Message="Are you sure that you want to remove this Automatic Messages?" runat="server" NavigateUrl= '<%# "delete.aspx?AutomaticMessageID=" & DataBinder.Eval(Container.DataItem, "AutomaticMessageID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField DataField="Title" HeaderText="Title" SortExpression="Title"></asp:BoundField>
		<asp:BoundField DataField="Subject" HeaderText="Subject" SortExpression="Subject"></asp:BoundField>
		<asp:TemplateField HeaderText="Message Preview">
		    <ItemTemplate>
                <asp:Literal ID="ltlMessage" runat="server"></asp:Literal>
		    </ItemTemplate>
		</asp:TemplateField>
	</Columns>
</CC:GridView>

</asp:content>
