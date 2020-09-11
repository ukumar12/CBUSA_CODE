<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Store Orders" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Web Orders</h4>

<span class="smaller">Please provide search criteria below</span>
<table cellpadding="2" cellspacing="2">
<tr>
<th><b>Order Date:</b></th>
<td class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_ProcessDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_ProcessDateUbound" runat="server" /></td>
</tr>
</table>
</td>
<th><b>Status:</b></th>
<td class="field"><asp:DropDownList ID="F_Status" runat="server" /></td>
<td>
<CC:DateValidator ID="vDateLBound" runat="server" Display="Dynamic" ControlToValidate="F_ProcessDateLbound" ErrorMessage="Invalid 'From date'" /><br />
<CC:DateValidator ID="vDateUBound" runat="server" Display="Dynamic" ControlToValidate="F_ProcessDateUbound" ErrorMessage="Invalid 'To date'" />
</td>
</tr>
<tr>
<th valign="top"><b>Order#:</b></th>
<td valign="top" class="field"><asp:TextBox ID="F_OrderNo" runat="server" Columns="20" MaxLength="50" /></td>
<th valign="top"><b>Customer Last Name:</b></th>
<td valign="top" class="field"><asp:TextBox ID="F_Name" runat="server" Columns="20" MaxLength="50" /></td>
</tr>
<tr>
<th valign="top"><b>Output As:</b></th>
<td valign="top" class="field">
<asp:DropDownList ID="F_OutputAs" runat="server">
<asp:ListItem Value="HTML" Text="HTML Page"></asp:ListItem>
<asp:ListItem Value="Excel" Text="Excel Document"></asp:ListItem>
</asp:DropDownList>
</td>
<th valign="top"><b>State:</b></th>
<td>
<asp:DropDownList ID="F_State" runat="server">
</asp:DropDownList>
</td>
</tr>
<tr>
<th valign="top"><b>Promotion Code:</b></th>
<td valign="top" class="field">
<asp:TextBox ID="F_PromotionCode" runat="server"></asp:TextBox>
</td>
</tr>
<tr>
<td colspan="4" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
<input class="btn" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />

</td>
</tr>
</table>

<p></p>
<asp:Literal ID="ltlSummary" runat="server" />
<p></p>
<p class="smaller">Orders with a high fraud risk have been marked in <span style="background-color:#FF0000; color: #FFFFFF">red</span>
<CC:GridView ShowFooter="True" id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
    <RowStyle HorizontalAlign="Right" />
	<FooterStyle CssClass="header" HorizontalAlign="Right" />
	<Columns>
        <asp:TemplateField>
            <ItemTemplate>
			    <asp:HyperLink enableviewstate=False runat="server" NavigateUrl= '<%# "edit.aspx?OrderId=" & DataBinder.Eval(Container.DataItem, "OrderId") & "&RedirectUrl=default.aspx?" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
            </ItemTemplate>
        </asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate=False Message="Are you sure that you want to remove this Order?" runat="server" NavigateUrl= '<%# "delete.aspx?OrderId=" & DataBinder.Eval(Container.DataItem, "OrderId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField SortExpression="OrderNo" HeaderText="Order#">
		     <ItemTemplate>
		  	      <asp:Label enableviewstate = "False" runat="server" id="lblOrderId"></asp:Label>   
		    </ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField HeaderText="Customer Name" SortExpression="FullName">
		    <ItemTemplate>
		        <asp:Literal enableviewstate="False" runat="server" id="ltlCustomerName"></asp:Literal> 
		    </ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="ProcessDate" HTMLEncode="False" DataFormatString="{0:d}" DataField="ProcessDate" HeaderText="Order Date"></asp:BoundField>
		<asp:BoundField SortExpression="ShippedDate" HTMLEncode="False" DataFormatString="{0:d}" DataField="ShippedDate" HeaderText="Shipped Date"></asp:BoundField>
		<asp:BoundField SortExpression="Subtotal" HTMLEncode="False" DataFormatString="{0:c}" DataField="Subtotal" HeaderText="Subtotal"></asp:BoundField>
		<asp:BoundField SortExpression="Discount" HTMLEncode="False" DataFormatString="{0:c}" DataField="Discount" HeaderText="Discount"></asp:BoundField>
		<asp:BoundField SortExpression="Tax" HTMLEncode="False" DataFormatString="{0:c}" DataField="Tax" HeaderText="Tax"></asp:BoundField>
		<asp:BoundField SortExpression="Shipping" HTMLEncode="False" DataFormatString="{0:c}" DataField="Shipping" HeaderText="Shipping"></asp:BoundField>
		<asp:BoundField SortExpression="Total" HTMLEncode="False" DataFormatString="{0:c}" DataField="Total" HeaderText="Total"></asp:BoundField>
		<asp:BoundField SortExpression="Status" DataField="Status" HeaderText="Status"></asp:BoundField>
		<asp:BoundField SortExpression="PromotionCode" DataField="PromotionCode" HeaderText="Promotion Code"></asp:BoundField>
		<asp:BoundField SortExpression="StateName" DataField="StateName" HeaderText="State"></asp:BoundField>
	</Columns>
</CC:GridView>

<div runat="server" id="divDownload" visible="false">
<asp:HyperLink id="lnkDownload" runat="server">Download File</asp:HyperLink>
</div>

</asp:content>

