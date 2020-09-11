<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Rating And Comments Data" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Ratings and Comments</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top"><b>Builder:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_BuilderID" runat="server" /></td>

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
<th valign="top">Last Name:</th>
<td valign="top" class="field"><asp:textbox id="F_LastName" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>

<th valign="top">Company Name:</th>
<td valign="top" class="field"><asp:textbox id="F_CompanyName" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top">Email:</th>
<td valign="top" class="field"><asp:textbox id="F_Email" runat="server" Columns="50" MaxLength="100"></asp:textbox></td>

<th valign="top">Website URL:</th>
<td valign="top" class="field"><asp:textbox id="F_WebsiteURL" runat="server" Columns="50" MaxLength="100"></asp:textbox></td>
</tr>
<tr>
<td align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
<input class="btn" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />
</td>
</tr>
</table>
</asp:Panel>
<p></p>

<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate="False" Message="Are you sure that you want to remove Comments and Rating by this Builder?" runat="server" NavigateUrl= '<%# "delete.aspx?BuilderID=" & DataBinder.Eval(Container.DataItem, "BuilderID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		
		<asp:BoundField SortExpression="CompanyName" DataField="CompanyName" HeaderText="Company Name"></asp:BoundField>
        
 <asp:TemplateField SortExpression = "Comments"  HeaderText="Comments" ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
                <asp:HyperLink EnableViewState="False" runat="server"  NavigateUrl='<%# "displayBuilderComments.aspx?BuilderID=" & DataBinder.Eval(Container.DataItem, "BuilderID")  %>'     ID="lnktotalComments"  ><%#Eval("Comments")%></asp:HyperLink>
                  </ItemTemplate>
        </asp:TemplateField>

      <%--  <asp:BoundField SortExpression="Comments" DataField="Comments" HeaderText="Comments  "></asp:BoundField>--%>
        <asp:BoundField SortExpression="Ratings" DataField="Ratings" HeaderText="Ratings "></asp:BoundField>
		<asp:ImageField SortExpression="IsActive" DataImageUrlField="IsActive" HeaderText="Is Active?" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
		
	</Columns>
</CC:GridView>

</asp:content>
