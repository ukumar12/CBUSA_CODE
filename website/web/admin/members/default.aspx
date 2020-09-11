<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Members" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Members</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="Panel1"  DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top">Username:</th>
<td valign="top" class="field"><asp:textbox id="F_Username" runat="server" Columns="20" MaxLength="50"></asp:textbox></td>
<th valign="top"><b>Last Name:</b></th>
<td valign="top" class="field"><asp:textbox id="F_LastName" runat="server" Columns="20" MaxLength="50"></asp:textbox></td>
</tr>
<tr>
<th valign="top"><b>Zip Code:</b></th>
<td valign="top" class="field"><asp:textbox id="F_Zip" runat="server" Columns="12" MaxLength="12"></asp:textbox></td>
<th valign="top"><b>Is Active:</b></th>
<td valign="top" class="field">
	<asp:DropDownList ID="F_IsActive" runat="server">
		<asp:ListItem Value="">-- ALL --</asp:ListItem>
		<asp:ListItem Value="1">Yes</asp:ListItem>
		<asp:ListItem Value="0">No</asp:ListItem>
	</asp:DropDownList>
</td>
</tr>
<tr>
<th valign="top"><b>Create Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_CreateDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_CreateDateUbound" runat="server" /></td>
</tr>
</table>
</td>
<th valign="top">Email:</th>
<td valign="top" class="field"><asp:textbox id="F_Email" runat="server" Columns="20" MaxLength="50" /></td>
<td>
<CC:DateValidator runat="server" ID="dvCreateDateLbound" Display="Dynamic" ControlToValidate="F_CreateDateLbound" ErrorMessage="Field 'Create Date From' is invalid" ></CC:DateValidator><br />
<CC:DateValidator runat="server" ID="dvCreateDateUbound" Display="Dynamic" ControlToValidate="F_CreateDateUbound" ErrorMessage="Field 'Create Date To' is invalid" ></CC:DateValidator>
</td>
</tr>
<tr>
<th valign="top"><b># Orders :</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0"><tr><td class="smaller">From
<asp:textbox id="F_NumOrdersLBound" runat="server" Columns="4" MaxLength="4"></asp:textbox>
</td><td>&nbsp;</td><td class="smaller">To
<asp:textbox id="F_NumOrdersUbound" runat="server" Columns="4" MaxLength="4"></asp:textbox></td>
</tr>
</table>
</td>
<th valign="top"><b> Total Dollars Spent:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0"><tr><td class="smaller">From
<asp:textbox id="F_OrderTotalLBound" runat="server" Columns="4" MaxLength="4"></asp:textbox></td>
<td>&nbsp;</td><td class="smaller">To
<asp:textbox id="F_OrderTotalUbound" runat="server" Columns="4" MaxLength="4"></asp:textbox></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Average Order Size:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0"><tr><td class="smaller">From
<asp:textbox id="F_AvgOrderSizeLBound" runat="server" Columns="4" MaxLength="4"></asp:textbox></td>
<td>&nbsp;</td><td class="smaller">To
<asp:textbox id="F_AvgOrderSizeUBound" runat="server" Columns="4" MaxLength="4"></asp:textbox></td>
</tr>
</table>
</td>
<th valign="top"><b>Output As:</b></th>
<td valign="top" class="field">
<asp:DropDownList ID="F_OutputAs" runat="server">
<asp:ListItem Value="HTML" Text="HTML Page"></asp:ListItem>
<asp:ListItem Value="Excel" Text="Excel Document"></asp:ListItem>
</asp:DropDownList>
</td>
</tr>
<tr>
<td colspan="4" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
<input class="btn" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />
</td>
</tr>
</table>
<p></p><CC:OneClickButton id="AddNew" Runat="server" Text="Add New Member"  CausesValidation="false" cssClass="btn" /> <CC:OneClickButton runat="server" ID="btnPlaceOrder" CssClass="btn" Text="Place Order as Non-Member" />
</asp:Panel>

<p></p>
<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "view.aspx?MemberId=" & DataBinder.Eval(Container.DataItem, "MemberId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/preview.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate=False Message="Are you sure that you want to remove this Member?" runat="server" NavigateUrl= '<%# "delete.aspx?MemberId=" & DataBinder.Eval(Container.DataItem, "MemberId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:LinkButton runat="server" id="lnkLogin" CommandName="Login" CommandArgument='<%#Container.DataItem("MemberId")%>' Text="<img src='/images/admin/shoppingcart.gif' style='border-style:none;' alt='Login &amp; Place Order' />" />
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="Username" DataField="Username" HeaderText="Username"></asp:BoundField>
		<asp:BoundField SortExpression="LastName" DataField="LastName" HeaderText="Last Name"></asp:BoundField>
		<asp:BoundField SortExpression="Zip" DataField="Zip" HeaderText="Zip Code"></asp:BoundField>
		<asp:BoundField SortExpression="OrderCount" DataField="OrderCount" HeaderText="# Orders" NullDisplayText="0"></asp:BoundField>
		<asp:TemplateField HeaderText="Total Dollars Spent" SortExpression="OrderTotal">
		  	<ItemTemplate>
			   <asp:literal runat="server" id="ltlOrderTotal" ></asp:literal>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField HeaderText=" Average Order Size" SortExpression="AverageOrderSize" >
		 	<ItemTemplate>
			   <asp:literal runat="server" id="ltlAvgOrderSize" ></asp:literal>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="m.CreateDate" DataField="CreateDate" HeaderText="Member Since" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:ImageField SortExpression="IsActive" DataImageUrlField="IsActive" HeaderText="Is Active" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
	</Columns>
</CC:GridView>

<div runat="server" id="divDownload" visible="false">
<asp:HyperLink id="lnkDownload" runat="server">Download File</asp:HyperLink>
</div>

</asp:content>
