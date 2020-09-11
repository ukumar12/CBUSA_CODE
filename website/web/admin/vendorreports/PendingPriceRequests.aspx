<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Pending Price Requests" CodeFile="PendingPriceRequests.aspx.vb" Inherits="Index" %>

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

<h4>Vendors Pending Price Request- Report</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
<table cellpadding="2" cellspacing="2">
<tr>
<th valign="top"><b>Vendor ID:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_VendorID" runat="server" /></td>
</tr>
 
  
<tr>
<td colspan="2" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
<input class="btn" type="submit" value="Clear" onclick="window.location='PendingPriceRequests.aspx';return false;" />
</td>
</tr>
 

</table>
</asp:Panel>
<p></p>
<p>Please select a vendor</p>
<asp:Button id="btnSelectAll" runat="server" Text="Check All" onclientclick="SetAll(true);return false;" />
<asp:Button ID="btnUnSelectAll" runat="server" Text="Uncheck All" OnClientClick="SetAll(false); return false;" />
<asp:Button ID="btnSubmitAll" runat="server" Text="Delete Selected Price Requests" />
 <CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server"   AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
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
             <asp:Label id="ltlVendorProductPriceRequestId" runat ="server" text='<%# Eval("VendorProductPriceRequestId")%>' />
         </ItemTemplate>
      </asp:TemplateField>
        <asp:TemplateField >
         <ItemTemplate>
             <asp:Literal id="ltlVendorName" runat ="server"  />
         </ItemTemplate>
      </asp:TemplateField>
		<asp:BoundField SortExpression="CompanyName" DataField="CompanyName" HeaderText="Builder Name"></asp:BoundField>
        
  <asp:BoundField datafield="CompanyName" HeaderText ="CompanyName" Visible ="false" />
 <asp:BoundField datafield="Product" HeaderText ="Product" />
 <asp:BoundField datafield="Created" HeaderText ="Created" />
   <asp:TemplateField>
		    <ItemTemplate>
		        <asp:Button ID="btnDeletePriceRequest" runat="server" Text="Delete" CssClass="btn submit campaignBtn" CommandName="DeletePriceRequest" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"VendorProductPriceRequestId") %>' />
		    </ItemTemplate>
		</asp:TemplateField>
	</Columns>
</CC:GridView> 

</asp:content>


