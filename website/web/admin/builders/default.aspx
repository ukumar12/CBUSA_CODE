<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Builder" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Builder Administration</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top">Historic ID:</th>
<td valign="top" class="field"><asp:textbox id="F_HistoricId" runat="server" Columns="50" MaxLength="5" TextMode="Number"></asp:textbox></td>

<th valign="top"><b>Is Active:</b></th>
<td valign="top" class="field">
	<asp:DropDownList ID="F_IsActive" runat="server">
		<asp:ListItem Value="">-- ALL --</asp:ListItem>
		<asp:ListItem Value="1" Selected ="True" >Yes</asp:ListItem>
		<asp:ListItem Value="0">No</asp:ListItem>
	</asp:DropDownList>
</td>
</tr>
<tr>
<th valign="top">CRM ID:</th>
<td valign="top" class="field"><asp:textbox id="F_CrmId" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
<th valign="top">LLC:</th>
<td valign="top" class="field"><asp:DropDownList ID="F_LLCId" runat="server"></asp:DropDownList></td>
</tr>
<tr>
<th valign="top">Company Name:</th>
<td valign="top" class="field"><asp:textbox id="F_CompanyName" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>

<th valign="top">Website URL:</th>
<td valign="top" class="field"><asp:textbox id="F_WebsiteURL" runat="server" Columns="50" MaxLength="100"></asp:textbox></td>
</tr>
<tr>
<th valign="top"><b>Registration Status:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_RegistrationStatusID" runat="server" /></td>

<th valign="top"><b>Submitted:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_SubmittedLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_SubmittedUbound" runat="server" /></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Is New:</b></th>
<td valign="top" class="field">
	<asp:DropDownList ID="F_IsNew" runat="server">
		<asp:ListItem Value="">-- ALL --</asp:ListItem>
		<asp:ListItem Value="1">Yes</asp:ListItem>
		<asp:ListItem Value="0">No</asp:ListItem>
	</asp:DropDownList>
</td>
<td align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
<input class="btn" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />
<CC:OneClickButton id="btnExport" Runat="server" Text="Export" cssClass="btn" />
</td>
</tr>
<tr>
<th valign="top"><b>Skip Entitlement:</b></th>
<td valign="top" class="field">
	<asp:DropDownList ID="F_drpSkipEntitlement" runat="server">
		<asp:ListItem Value="">-- ALL --</asp:ListItem>
		<asp:ListItem Value="1">Yes</asp:ListItem>
		<asp:ListItem Value="0">No</asp:ListItem>
	</asp:DropDownList>
</td>
</tr>
</table>
</asp:Panel>
<p></p>
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Builder" cssClass="btn"></CC:OneClickButton>
<CC:OneClickButton id="btnSubmitA" Runat="server" Text="Vindicia" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?BuilderID=" & DataBinder.Eval(Container.DataItem, "BuilderID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate="False" Visible="false" Message="Are you sure that you want to remove this Builder?" runat="server" NavigateUrl= '<%# "delete.aspx?BuilderID=" & DataBinder.Eval(Container.DataItem, "BuilderID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "/admin/builders/builderregistration/default.aspx?F_BuilderId=" & DataBinder.Eval(Container.DataItem, "BuilderID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/collection.gif" ID="lnkAds">Registrations</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink EnableViewState="false" runat="server" NavigateUrl= '<%# "/admin/builderaccount/default.aspx?F_BuilderID="& DataBinder.Eval(Container.DataItem,"BuilderID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/person-computer.gif" ID="lnkUsers">Accounts</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="HistoricId" DataField="HistoricId" HeaderText="Historic ID"></asp:BoundField>
		<asp:BoundField SortExpression="LLC" DataField="LLC" HeaderText="LLC"></asp:BoundField>
		<asp:BoundField SortExpression="CompanyName" DataField="CompanyName" HeaderText="Company Name"></asp:BoundField>
        <asp:BoundField SortExpression="Zip" DataField="Zip" HeaderText="Zip"></asp:BoundField>
		<asp:BoundField SortExpression="RegistrationStatus" DataField="RegistrationStatus" HeaderText="Registration Status"></asp:BoundField>
		<asp:BoundField SortExpression="Submitted" DataField="Submitted" HeaderText="Submitted" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:ImageField SortExpression="IsActive" DataImageUrlField="IsActive" HeaderText="Is Active?" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
		<asp:ImageField SortExpression="IsNew" DataImageUrlField="IsNew" HeaderText="Is New?" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
		<asp:ImageField SortExpression="IsRegistrationCompleted" DataImageUrlField="IsRegistrationCompleted" HeaderText="Registration<br />Completed?" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
		<asp:ImageField SortExpression="IsReportSubmitted" DataImageUrlField="IsReportSubmitted" HeaderText="Last Quarter<br />Report Submitted?" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
	</Columns>
</CC:GridView>

</asp:content>
