<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Vendor Registration Member Reference" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Vendor Registration Member Reference Administration</h4>
<CC:OneClickButton id="btnBackToRegistration" Runat="server" CausesValidation="false" Text="Back to Registration" cssClass="btn"></CC:OneClickButton>
<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top">First Name:</th>
<td valign="top" class="field"><asp:textbox id="F_FirstName" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top">Last Name:</th>
<td valign="top" class="field"><asp:textbox id="F_LastName" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top">Company Name:</th>
<td valign="top" class="field"><asp:textbox id="F_CompanyName" runat="server" Columns="50" MaxLength="100"></asp:textbox></td>
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
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Vendor Registration Member Reference" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?VendorRegistrationMemberReferenceID=" & DataBinder.Eval(Container.DataItem, "VendorRegistrationMemberReferenceID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate="False" Message="Are you sure that you want to remove this Vendor Registration Member Reference?" runat="server" NavigateUrl= '<%# "delete.aspx?VendorRegistrationMemberReferenceID=" & DataBinder.Eval(Container.DataItem, "VendorRegistrationMemberReferenceID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="FirstName" DataField="FirstName" HeaderText="First Name"></asp:BoundField>
		<asp:BoundField SortExpression="LastName" DataField="LastName" HeaderText="Last Name"></asp:BoundField>
		<asp:BoundField SortExpression="CompanyName" DataField="CompanyName" HeaderText="Company Name"></asp:BoundField>
	</Columns>
</CC:GridView>

</asp:content>
