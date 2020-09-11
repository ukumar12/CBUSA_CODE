<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="How Heard" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Discovery Sources Administration</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top">How Heard:</th>
<td valign="top" class="field"><asp:textbox id="F_HowHeard" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top"><b>Is User Input:</b></th>
<td valign="top" class="field">
	<asp:DropDownList ID="F_IsUserInput" runat="server">
		<asp:ListItem Value="">-- ALL --</asp:ListItem>
		<asp:ListItem Value="1">Yes</asp:ListItem>
		<asp:ListItem Value="0">No</asp:ListItem>
	</asp:DropDownList>
</td>
</tr>
<tr>
<th valign="top">User Input Label:</th>
<td valign="top" class="field"><asp:textbox id="F_UserInputLabel" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
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
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Discovery Source Entry" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use arrows" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?HowHeardId=" & DataBinder.Eval(Container.DataItem, "HowHeardId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate="False" Message="Are you sure that you want to remove this Discovery Source Entry?" runat="server" NavigateUrl= '<%# "delete.aspx?HowHeardId=" & DataBinder.Eval(Container.DataItem, "HowHeardId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField DataField="HowHeard" HeaderText="How Heard"></asp:BoundField>
		<asp:ImageField DataImageUrlField="IsUserInput" HeaderText="Required User Input" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
		<asp:BoundField DataField="UserInputLabel" HeaderText="User Input Label"></asp:BoundField>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "move.aspx?ACTION=UP&HowHeardId=" & DataBinder.Eval(Container.DataItem, "HowHeardId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/moveup.gif" ID="lnkMoveUp">Move Up</asp:HyperLink>
		    </ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "move.aspx?ACTION=DOWN&HowHeardId=" & DataBinder.Eval(Container.DataItem, "HowHeardId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/movedown.gif" ID="lnkMoveDown">Move Down</asp:HyperLink>
		    </ItemTemplate>
		</asp:TemplateField>
	</Columns>
</CC:GridView>

</asp:content>