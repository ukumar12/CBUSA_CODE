<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Builder Bid Data" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<script type="text/javascript">
    function ChangeTarget() {
        document.forms[0].target = '_blank';
        window.setTimeout('document.forms[0].target="_self"', 1000);
    }
</script>

<h4>Builder Statements Administration</h4>

<asp:HyperLink ID="lnkReturn" runat="server" Text="Return to Builders list"></asp:HyperLink><br /><br />

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top"><b>Builder:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_BuilderID" runat="server" /></td>
</tr>
 
<tr>
<th valign="top"><b>Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0">
<tr><td class="smaller">From <CC:DatePicker id="F_CreateDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_CreateDateUbound" runat="server" /></td>
</tr>
</table>
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

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		
        <asp:BoundField SortExpression="CompanyName" DataField="CompanyName" HeaderText="Builder"></asp:BoundField>
        
        
           

        <asp:TemplateField SortExpression = "FileName"  HeaderText = "File" ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
               <asp:HyperLink EnableViewState="False" runat="server"       ID="lnktostatement"  ></asp:HyperLink> 
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField SortExpression = "MONTH(StatementDate)"  HeaderText = "Month" ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
                <asp:Literal EnableViewState="False" runat="server"       ID="ltlMonth"  ></asp:Literal>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField SortExpression = "YEAR(StatementDate)"  HeaderText = "Year" ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
                 <asp:Literal EnableViewState="False" runat="server"  ID="ltlYear"  ></asp:Literal>
            </ItemTemplate>
        </asp:TemplateField>
        </Columns>
</CC:GridView>

</asp:content>
