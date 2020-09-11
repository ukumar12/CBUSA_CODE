<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Store Promotion" CodeFile="promotionreports.aspx.vb" Inherits="PromotionalReport"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Store Promotions Reports</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="Panel1"  DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top">Promotion Name:</th>
<td valign="top" class="field"><asp:TextBox id="F_PromotionName" runat="server" Columns="50" MaxLength="255"></asp:TextBox></td>
</tr>
<tr>
<th valign="top">Promotion Code:</th>
<td valign="top" class="field"><asp:TextBox id="F_PromotionCode" runat="server" Columns="50" MaxLength="50"></asp:TextBox></td>
</tr>
<tr>
<th valign="top">Promotion Type:</th>
<td valign="top" class="field">
  <asp:DropDownList ID="F_drpPromotionType" runat="server">
  <asp:ListItem value="">--ALL--</asp:ListItem>
  <asp:ListItem Value="Percentage">Percent Off</asp:ListItem>
  <asp:ListItem Value="Monetary">Dollar off</asp:ListItem>
  </asp:DropDownList>
</td>
</tr>
<tr>
<th valign="top"><b>Start Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_StartDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_StartDateUbound" runat="server" /></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>End Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_EndDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_EndDateUbound" runat="server" /></td>
</tr>
</table>
</td>
</tr>
<tr>
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
<td colspan="2" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
<input class="btn" type="submit" value="Clear" onclick="window.location='promotionreports.aspx';return false;" />
</td>
</tr>
</table>
<p></p>
</asp:Panel>

<p></p>
<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
				
		<asp:BoundField SortExpression="PromotionCode" DataField="PromotionCode" HeaderText="Promotion Code"></asp:BoundField>
		<asp:templateField ItemStyle-HorizontalAlign="center" SortExpression="PromotionType" HeaderText="Promotion Type">
		   <itemTemplate>
		        <asp:literal id="ltlPromotionType" runat="server"></asp:literal>
		    </itemTemplate>
		</asp:templateField>
		<asp:BoundField SortExpression="sp.DeliveryMethod" DataField="DeliveryMethod" HeaderText="Delivery Method"></asp:BoundField>
		<asp:BoundField SortExpression="sp.NumberSent" ItemStyle-HorizontalAlign="Center" DataField="NumberSent" HeaderText="Number Sent"></asp:BoundField>
		<asp:BoundField SortExpression="PromotionRedeemed" ItemStyle-HorizontalAlign="Center" DataField="PromotionRedeemed" HeaderText="Redeemed"></asp:BoundField>
		<asp:templateField ItemStyle-HorizontalAlign="center">
		    <headertemplate>
		        Conversion
		    </headertemplate>
		    <itemTemplate>
		        <asp:literal id="ltlConversion" runat="server"></asp:literal>
		    </itemTemplate>
		</asp:templateField>
		<asp:BoundField SortExpression="TotalSubTotal" ItemStyle-HorizontalAlign="Right" DataField="TotalSubTotal" DataFormatString="{0:c}" HTMLEncode="False"  HeaderText="Total"></asp:BoundField>
		<asp:BoundField SortExpression="AVGSubTotal" ItemStyle-HorizontalAlign="Right" DataField="AVGSubTotal" DataFormatString="{0:c}" HTMLEncode="False"  HeaderText="Avg. Total"></asp:BoundField>
		
		
		<asp:Checkboxfield ItemStyle-HorizontalAlign="Center" SortExpression="IsActive" DataField="IsActive" HeaderText="Is Active"/>		
	</Columns>
</CC:GridView>

</asp:content>
