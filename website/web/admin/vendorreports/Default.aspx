<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Vendor Reporting Status" CodeFile="Default.aspx.vb" Inherits="Index" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<script type="text/javascript">
    function ChangeTarget() {
        document.forms[0].target = '_blank';
        window.setTimeout('document.forms[0].target="_self"', 1000);
    }
</script>

<h4>Vendors - Report</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top"><b>Vendor ID:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_VendorID" runat="server" /></td>
</tr>
 
<tr>
	<th valign="top">LLC(s):</th>
	<td class="field"><CC:CheckBoxListEx ID="F_LLC" runat="server" RepeatColumns="3"></CC:CheckBoxListEx></td>
</tr>
<tr>
<th valign="top">Reported : </th>
<td class="field">
<asp:DropDownList ID="F_Reported" runat="server">
		 
		<asp:ListItem Value="1">No</asp:ListItem>
		<asp:ListItem Value="0">Yes</asp:ListItem>
</asp:DropDownList> 
</td>


</tr>

<tr>
<td colspan="2" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
<input class="btn" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />
</td>
</tr>
<tr>
<td colspan="2" align ="right"> 
<asp:button ID="Export" runat="server" Text="Export " CssClass="btn" />
</td>
</tr>

</table>
</asp:Panel>
<p></p>

 <CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom" DataKeyNames ="VendorID">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:BoundField SortExpression="Vendor" DataField="Vendor" HeaderText="Vendor Name"></asp:BoundField>
        <asp:TemplateField HeaderText = "Market(LLC)" >
        <ItemTemplate >
        <asp:Literal ID="LLCName" runat ="server" ></asp:Literal>
        </ItemTemplate>
        </asp:TemplateField>
 <asp:BoundField SortExpression="" DataField="VendorPhone" HeaderText="Phone"></asp:BoundField>
 <asp:BoundField datafield="PrimaryContact" HeaderText ="Primary Contact" />
 <asp:BoundField datafield="PrimaryContactEmail" HeaderText ="Primary Contact Email" />
 <asp:BoundField datafield="QuarterlyReportContact" HeaderText ="Quarterly Reporter" />
 <asp:BoundField datafield="QuarterlyReportEmail" HeaderText ="Quarterly Reporter Email" />
 
	
	</Columns>
</CC:GridView> 

</asp:content>

