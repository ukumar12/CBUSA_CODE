<%@ Control EnableViewstate="True" Language="VB" AutoEventWireup="false" CodeFile="MyMoneyReports.ascx.vb" Inherits="_default" %>
<SCRIPT LANGUAGE="JavaScript">
<!-- Begin
 function retrieveVendors(supplyPhases) 
{

    var objXHR, buffer, selections, i, h, len;
    
    selections = "0"
    
    for (var i = 0; i < supplyPhases.options.length; ++i) {
        if (supplyPhases.options[i].selected) 
        {
            selections += "," + supplyPhases.options[i].value;
        }
    }
    
    try {
        var objXHR = new XMLHttpRequest();
        } catch (e) {
        try {
            var objXHR = new ActiveXObject('Msxml2.XMLHTTP');
            } catch (e) {
            try {
                var objXHR = new ActiveXObject('Microsoft.XMLHTTP');
            } catch (e) {
            document.write('XMLHttpRequest not supported'); }
        }
    }

    if (objXHR) {

        objXHR.onreadystatechange = function() {
            if (objXHR.readyState == 4) {
                if (objXHR.status == 200) {

                    var vendors = document.getElementById('<%=F_Vendor.ClientID%>');

                    var num = vendors.options.length;
                    for (i = 0; i < num; i++) {
                        vendors.options[0] = null;
                    }

                    if (objXHR.responseText.length > 1) {
                        var vendorarray = objXHR.responseText.split(",");
                        for (var j = 0, len = vendorarray.length; j < len; ++j) {
                            addOption(vendors, vendorarray[j].split(":")[1], vendorarray[j].split(":")[0]);
                        }
                    }
                }
            }
        };

        objXHR.open('POST', "vendorselect.aspx", true);
        objXHR.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        objXHR.send("supplyphases=" + selections);
      }
  }

  function addOption(selectbox, text, value) 
  {
      var optn = document.createElement("OPTION");
      optn.text = text;
      optn.value = value;
      selectbox.options.add(optn);
  }


//  End -->
</script>

<asp:Panel ID="pMyMoneyRebates" runat="server" DefaultButton="btnUpdate">
<div class="pckggraywrpr">
    <div class="pckghdgred">Rebate Report</div>
    <table>
       <tr>
	        <td class="bold" style="padding:4px 2px;">Start Qtr/Yr:</td>
	        <td>
                <CC:DropDownListEx ID="drpStartQuarter" runat="server" AutoPostBack="false" >
                </CC:DropDownListEx>
                <CC:DropDownListEx ID="drpStartYear" runat="server" AutoPostBack="false"  >
                </CC:DropDownListEx>
            </td>
	    </tr>
	    <tr>
	        <td class="bold" style="padding:4px 2px;">End Qtr/Yr:</td>
	        <td>
                <CC:DropDownListEx ID="drpEndQuarter" runat="server" AutoPostBack="false"  >
                </CC:DropDownListEx>
                <CC:DropDownListEx ID="drpEndYear" runat="server" AutoPostBack="false" >
                </CC:DropDownListEx>
	        </td>
	    </tr>
        <tr>
            <td>
                Vendor:
            </td>
            <td>
                <CC:SearchList ID="F_Vendor" runat="server" CssClass="searchlist" Table="Vendor" TextField="CompanyName" ValueField="HistoricID" ViewAllLength="10" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <CC:OneClickButton id="btnUpdate" Runat="server" Text="Update" cssClass="btnred" CausesValidation="false" />
            </td>
        </tr>
    </table>
    <br />
    <div id="divGrid" runat="server">
        <div class="pckghdgblue">My Rebates</div>
        <asp:Repeater ID="rptQuarters" runat="server">
            <ItemTemplate>
                <CC:GridView id="gvBuilderRebates" width="98%" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="False" AllowSorting="False"  EmptyDataText="There are no records to display." AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom" cssclass="tblcompr">
                    <AlternatingRowStyle CssClass="alternate" />
                    <RowStyle CssClass="row" />
                    <Columns>
                        <asp:BoundField DataField="InvoiceNumber" HeaderText="Invoice Number" />
                        <asp:BoundField DataField="CompanyName" HeaderText="Vendor" />
                        <asp:BoundField DataField="PeriodQuarter" HeaderText="Period" />
                        <asp:BoundField DataField="PeriodYear" HeaderText="Year" />
                        <asp:BoundField DataField="PurchaseVolume" HeaderText="Purchase Volume" DataFormatString="{0:c}" HtmlEncode="false" />
                        <asp:BoundField DataField="TotalInvoiced" HeaderText="Rebates Invoiced" DataFormatString="{0:c}" HtmlEncode="false" />
                        <asp:BoundField DataField="TotalCollected" HeaderText="Rebates Paid" DataFormatString="{0:c}" HtmlEncode="false" />
                        <asp:BoundField DataField="TotalDue" HeaderText="Rebates Due" DataFormatString="{0:c}" HtmlEncode="false" />
                    </Columns>
                </CC:GridView>
            </ItemTemplate>
        </asp:Repeater>
    </div>
    <div>
        <div class="pckghdgblue">All LLC Rebates</div>
        <CC:GridView ID="gvLLCRebates" runat="server" Width="98%" CellSpacing="2" CellPadding="2" AllowPaging="false" AllowSorting="false" EmptyDataText="There are no records to display." AutoGenerateColumns="false" BorderWidth="0" CssClass="tblcompr">
            <AlternatingRowStyle CssClass="alternate" />
            <RowStyle CssClass="row" />
            <Columns>
                <asp:BoundField DataField="InvoiceNumber" HeaderText="Invoice Number" />
                <asp:BoundField DataField="InvoiceDate" HeaderText="Invoice Date" DataFormatString="{0:d}" HtmlEncode="false" />
                <asp:BoundField DataField="InvoiceDueDate" HeaderText="Due Date" DataFormatString="{0:d}" HtmlEncode="false" />
                <asp:BoundField DataField="CompanyName" HeaderText="Vendor" />
                <asp:BoundField DataField="PeriodQuarter" HeaderText="Period" />
                <asp:BoundField DataField="PeriodYear" HeaderText="Year" />
                <asp:BoundField DataField="TotalDue" HeaderText="Rebates Due" DataFormatString="{0:c}" HtmlEncode="false" />
            </Columns>
        </CC:GridView>
    </div>
    <br />
</div>
<br />
</asp:Panel>                