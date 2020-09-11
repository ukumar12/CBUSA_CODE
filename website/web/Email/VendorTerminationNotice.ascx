<%@ Control Language="VB" AutoEventWireup="false" CodeFile="VendorTerminationNotice.ascx.vb" Inherits="Email_VendorTerminationNotice" %>
<asp:PlaceHolder ID="PlaceHolder1" runat="server">
    <div>
         Dear  <asp:Literal ID="VendorName" runat="server" /> ,<br /><br />
        We regret to inform you that <%=sVendorName%> has been terminated as a member of CBUSA for failure to pay rebates in a timely manner.

The following CBUSA rebate invoices remain unpaid and will be written-off unless payment is received in the next 14 days:

         <br /><br />
         Information as of <%= System.DateTime.Now.Date%>
		<br />
         <br />
         <asp:Repeater  ID="rptVendors" runat="server">
            <HeaderTemplate>
                <table width="100%" border="1" class="arTable">
                    <tr>
                        <th><b>CBUSA Invoice #</b></th>
                        <th><b>Qtr-Yr</b></th>
                        <th><b>Invoice Date</b></th>
                        <th><b>Days Past Due</b></th>
                        <th><b>Invoice Amount</b></th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>     
                    <tr>
                        <td><%# Eval("invoice")%></td>
                        <td><%# Eval("Period")%> - <%# Eval("year")%></td>
                        <td><%# Replace(Eval("date"), " 12:00:00 AM", "") %></td>
                        <td><%# Eval("dayspastdue")%></td>
                       <td><%# FormatCurrency(Eval("amountdue"), 2)%></td>
                    </tr>
                
            </ItemTemplate>
            <FooterTemplate>
                    <tr class="arTotal">
                        <td colspan="4"><b>TOTAL DUE </b></td>
                        <td>$<%#sumBuilder%></td>
                    </tr>
                </table>
            </FooterTemplate>
         </asp:Repeater>
		<br />
         <br />
         <br />
          
         <p>
            Please remit payment at your earliest possible convenience to avoid any adverse consequences such as termination from CBUSA. If you have already remitted payment, please disregard this notice as the check may be in transit.<br />
			<br />
			Address for remittance:<br />
			<br />
			CBUSA LLC<br />
			P.O. Box 842691<br />
			Dallas, TX 75284-2691<br />
			<br />
            For Overnight Payments via Fedex or UPS, please remit to:<br /><br />
            CAPITOL ONE BANK-LOCKBOX<br />
            c/o CBUSA<br />
            6200 CHEVY CHASE BANK<br />
            LAUREL, MD 20707<br />
            <br />
         <b>
           If you have already remitted payment, or wish to discuss this termination notice for any other reason, please do not hesitate to contact CBUSA at  %%Operations_manager%%.
         </b>
         <br />
         <br />
         <br />
        <b> Sincerely   %%Builder_Group%% ,</b>
    </div>
   </asp:PlaceHolder>    