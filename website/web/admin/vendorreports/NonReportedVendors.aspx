  <%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Vendor Reporting Status" CodeFile="NonReportedVendors.aspx.vb" Inherits="Index" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<script type="text/javascript">
    function ChangeTarget() {
        document.forms[0].target = '_blank';
        window.setTimeout('document.forms[0].target="_self"', 1000);
    }
</script>

<script type="text/javascript">
    function setState(field, state) {
        var chkBoxList = document.getElementById(field);
        var chkBoxCount = chkBoxList.getElementsByTagName("input");
        for (var i = 0; i < chkBoxCount.length; i++) {
            chkBoxCount[i].checked = state;
        }

        return false;
    }

    function SetAll(val) {
        for (var el = 0; el < document.forms[0].length; el++) {
            if (document.forms[0][el].name.indexOf('chkSelect') >= 0) {
                document.forms[0][el].checked = val;
            }
        }
    }
</script>

<h4>Vendors - Report</h4>
<h4>Vendors who created a Salesreport with BuilderData but did not submit it .<br /> Allows Admin to submit it. </h4>
<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top"><b>Vendor ID:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_VendorID" runat="server" /></td>
</tr>
 
 <tr>
<th valign="top">Period Year:</th>
<td valign="top" class="field"><asp:textbox id="F_PeriodYear" runat="server" Columns="50" MaxLength="100" TextMode="Number" onkeypress="return this.value.length<=3"></asp:textbox></td>
</tr>

<tr>
<th valign="top">Period Quarter:</th>
<td valign="top" class="field"><asp:textbox id="F_PeriodQuarter" runat="server" Columns="50" MaxLength="100" TextMode="Number" onkeypress="return this.value.length<=1"></asp:textbox></td>
</tr>
<tr>
	<th valign="top">LLC(s):</th>
	<td class="field"><CC:CheckBoxListEx ID="F_LLC" runat="server" RepeatColumns="3"></CC:CheckBoxListEx></td>
</tr>
 

<tr>
<td colspan="2" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
<input class="btn" type="submit" value="Clear" onclick="window.location='NonReportedVendors.aspx';return false;" />
</td>
</tr>
 

</table>
</asp:Panel>
<p></p>
<p> By default displays last year and last quarter</p>
<asp:Button id="btnSelectAll" runat="server" Text="Check All" onclientclick="SetAll(true);return false;" />
<asp:Button ID="btnUnSelectAll" runat="server" Text="Uncheck All" OnClientClick="SetAll(false); return false;" />
<asp:Button ID="btnSubmitAll" runat="server" Text="Submit Checked Sales Reports" />
 <CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom" DataKeyNames ="VendorID">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
     <asp:TemplateField HeaderText="Select">
            <ItemTemplate>
                <asp:CheckBox ID="chkSelect" Checked="false"  runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField Visible=false>
         <ItemTemplate>
             <asp:Label id="ltlSalesReportID" runat ="server" text='<%# Eval("SalesReportID")%>' />
         </ItemTemplate>
      </asp:TemplateField>
		<asp:BoundField SortExpression="CompanyName" DataField="CompanyName" HeaderText="Vendor Name"></asp:BoundField>
        
  <asp:BoundField datafield="SalesReportID" HeaderText ="PeriodYear" Visible ="false" />
 <asp:BoundField datafield="Periodyear" HeaderText ="PeriodYear" />
 <asp:BoundField datafield="PeriodQuarter" HeaderText ="PeriodQuarter" />
 <asp:TemplateField HeaderText="Last Updated Date">
			<ItemTemplate>
			    <asp:Literal id="ltlUpdateDate" runat="server"></asp:Literal>
			</ItemTemplate>
		</asp:TemplateField>
  
  <asp:TemplateField>
		    <ItemTemplate>
		        <asp:Button ID="btnSubmitSalesReport" runat="server" Text="Submit Sales Report" CssClass="btn submit campaignBtn" CommandName="SubmitSalesReport" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"SalesReportID") %>' />
		    </ItemTemplate>
		</asp:TemplateField>
	</Columns>
</CC:GridView> 

</asp:content>


