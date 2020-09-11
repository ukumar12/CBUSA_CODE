<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" CodeFile="Default.aspx.vb" Inherits="Index" %>


<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

  <script src="../../../includes/jquery.tablesorter.js" type="text/javascript"></script>

 <script   type="text/javascript">
     $(document).ready(function () {


         //  $("#gvList").tablesorter({ sortList: [[2, 0]] });

         $("#ctl00_ph_gvList").tablesorter();
     });

</script>
<h4>APV Administration</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
 <tr>
<th valign="top"><b>Vendor:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_VendorID" runat="server" /></td>
</tr>
 <tr>
	<th valign="top">LLC(s):</th>
	<td class="field"><asp:ListBox ID="F_LLC" runat="server" SelectionMode="Multiple" Rows="8"></asp:ListBox></td>
</tr>
 

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
<th valign="top"><b>To Period Year:</b></th>
<td valign="top" class="field"> 
<CC:DropDownListEx ID="F_ComparePeriodYear" runat="server">
                    </CC:DropDownListEx>
</td>
<th valign="top"><b>To Quarter:</b></th>
<td valign="top" class="field">
<CC:DropDownListEx ID="F_ComparePeriodQuarter" runat="server">
                    </CC:DropDownListEx>
 
</td>
</tr>

 
<tr>
<td colspan="2" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
<asp:button id="btnExport" Runat="server" Text="Export" cssClass="btn" />
<input class="btn" type="submit" value="Clear" onclick="window.location='QtrComparisionByVendor.aspx';return false;" />
</td>
</tr>
</table>
</asp:Panel>
<p></p>
<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
  
  
	<asp:BoundField   DataField="LLC" HeaderText=" LLC"></asp:BoundField>
		<asp:BoundField   DataField="VendorName" HeaderText="Vendor Name"></asp:BoundField>

         <asp:TemplateField   HeaderText = "Paymentterms" ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
                <asp:Literal EnableViewState="False" runat="server"  ID="ltlPaymentterms"  ></asp:Literal>
                 
            </ItemTemplate>
        </asp:TemplateField>
	 
		<asp:BoundField  DataField="FinalAmount" HeaderText="TotalArbitratedPurchaseVolume($)" DataFormatString="{0:c}"></asp:BoundField>
		 
	</Columns>
</CC:GridView>



</asp:content>