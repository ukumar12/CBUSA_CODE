<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="APV" CodeFile="QtrComparisionReport.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>APV Administration</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
 
 
 

<tr>
<th valign="top"><b>Period Year:</b></th>
<td valign="top" class="field">
  <CC:DropDownListEx ID="F_StartPeriodYear" runat="server">
                    </CC:DropDownListEx>
</td>
<th valign="top"><b>Period Quarter:</b></th>
<td valign="top" class="field"> 
 <CC:DropDownListEx ID="F_StartPeriodQuarter" runat="server">
                    </CC:DropDownListEx>

</td>
</tr>

<tr>
<th valign="top"><b>To Compare Period Year:</b></th>
<td valign="top" class="field"> 
 <CC:DropDownListEx ID="F_ComparePeriodYear" runat="server">
                    </CC:DropDownListEx>
</td>
<th valign="top"><b>To Compare Quarter:</b></th>
<td valign="top" class="field"> 
 <CC:DropDownListEx ID="F_ComparePeriodQuarter" runat="server">
                    </CC:DropDownListEx>
</td>
</tr>

 
<tr>
<td colspan="2" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
<asp:button id="btnExport" Runat="server" Text="Export" cssClass="btn" />
<input class="btn" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />
</td>
</tr>
</table>
</asp:Panel>
<p></p>

<CC:GridView id="gvList" ShowFooter="True" CellSpacing="2" CellPadding="2" runat="server"   HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
    <FooterStyle CssClass="header" VerticalAlign="Top" />
	<Columns>
		<asp:BoundField SortExpression="LLC" DataField="LLC" HeaderText="Market"></asp:BoundField>
		<asp:BoundField SortExpression="Year1InitialVendorAmount" DataField="Year1InitialVendorAmount" HeaderText="Initial Vendor Amount (Quarter)" DataFormatString="{0:c}"></asp:BoundField>
		<asp:BoundField SortExpression="Year2InitialVendorAmount" DataField="Year2InitialVendorAmount" HeaderText="Initial Vendor Amount (Compared Quarter)" DataFormatString="{0:c}"></asp:BoundField>
		<asp:BoundField SortExpression="Variance" DataField="Variance" HeaderText="Variance" DataFormatString="{0:c}"></asp:BoundField>
		 
	</Columns>
    


</CC:GridView>

</asp:content>

