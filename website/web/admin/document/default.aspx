<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Document" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Document Administration</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top">Title:</th>
<td valign="top" class="field"><asp:textbox id="F_Title" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top">File Name:</th>
<td valign="top" class="field">
    <asp:textbox id="F_FileName" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top">Audience:</th>
<td valign="top" class="field">
    <CC:CheckBoxListEx ID="F_Audience" runat="server">
        <asp:ListItem Value="Builder">Builder</asp:ListItem>
        <asp:ListItem Value="Vendor">Vendor</asp:ListItem>
        <asp:ListItem Value="PIQ">PIQ</asp:ListItem> 
    </CC:CheckBoxListEx>
</tr>
<tr>
<th valign="top">Company Name:</th>
<td valign="top" class="field">
    <asp:textbox id="F_CompanyName" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top"><b>Uploaded:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_UploadedLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_UploadedUbound" runat="server" /></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Is Approved:</b></th>
<td valign="top" class="field">
	<asp:DropDownList ID="F_IsApproved" runat="server">
		<asp:ListItem Value="">-- ALL --</asp:ListItem>
		<asp:ListItem Value="1">Yes</asp:ListItem>
		<asp:ListItem Value="0">No</asp:ListItem>
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
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Document" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?AdminDocumentID=" & DataBinder.Eval(Container.DataItem, "AdminDocumentID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate="False" Message="Are you sure that you want to remove this Document?" runat="server" NavigateUrl= '<%# "delete.aspx?AdminDocumentID=" & DataBinder.Eval(Container.DataItem, "AdminDocumentID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="Title" DataField="Title" HeaderText="Title"></asp:BoundField>
		<asp:hyperlinkfield Text="Click to View" SortExpression="FileName" datanavigateurlfields="FileName" datanavigateurlformatstring="/assets/document/{0}" navigateurl="/" headertext="File" target="_blank" />
		<asp:BoundField DataField="FileGUID" HeaderText="File GUID"></asp:BoundField>
		<asp:BoundField SortExpression="Uploaded" DataField="Uploaded" HeaderText="Uploaded" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:ImageField DataImageUrlField="IsApproved" HeaderText="Is Approved" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
	</Columns>
</CC:GridView>

</asp:content>
