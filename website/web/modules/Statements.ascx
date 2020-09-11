<%@ Control EnableViewstate="True" Language="VB" AutoEventWireup="false" CodeFile="Statements.ascx.vb" Inherits="_default" %>
    <%--<asp:button id ="btnDashBoard" text="Go to DashBoard" class="btnred" runat="server" />--%>
<div style="padding: 0 20px 20px;" >
<asp:Panel ID="pStatements" runat="server" >

    <script type="text/javascript">
        function ChangeTarget() {
            document.forms[0].target = '_blank';
            window.setTimeout('document.forms[0].target="_self"', 1000);
        }
</script>
<h4>Statements</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch"  runat="server" style="margin:15px 0 15px 0px;">
<table cellpadding="2" cellspacing="2" class="white">
 
<tr>
<th valign="top">Date:</th>
<td valign="top" class="field">
    <asp:DropDownList runat="server" ID="F_drpYear" AutoPostBack="true"></asp:DropDownList>
</td>
</tr>

</table>
</asp:Panel>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom" Class="tblcompr" style="margin:15px 0 15px 0px;width:0px">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		
         <asp:TemplateField SortExpression = "StatementDate"  HeaderText = "Statements files" ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
                <asp:HyperLink EnableViewState="False" runat="server"       ID="lnktostatement"  ></asp:HyperLink> 
            </ItemTemplate>
        </asp:TemplateField>

       </Columns>
</CC:GridView>
</asp:Panel>                
</div>