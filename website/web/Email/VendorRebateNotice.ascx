﻿<%@ Control Language="VB" AutoEventWireup="false" CodeFile="VendorRebateNotice.ascx.vb" Inherits="Email_VendorRebateNotice" %>
<asp:PlaceHolder ID="PlaceHolder1" runat="server">
<div class ="arTotal">
<asp:Button ID="btnSend" Text = "Send Notice" Visible ="false" cssClass="btnred" runat="server" /></div>
    <div>
         Dear  <asp:Literal ID="VendorName" runat="server" /> ,<br /><br />
         It has come to our attention that your company is past due on CBUSA rebates. The following CBUSA rebate invoices are past due:
         <br /><br />
         Information as of <%= System.DateTime.Now.Date.ToShortDateString%>
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
                        <td colspan="4"><b>TOTAL DUE <%--TO  <%#sBuilderName%>--%></b></td>
                        <td>$<%#sumBuilder%></td>
                    </tr>
                </table>
            </FooterTemplate>
         </asp:Repeater>

         <br />
         <asp:Repeater ID="rptOtherBuildersVendors" Visible ="false"  runat="server">
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
                        <td colspan="4"><b>TOTAL DUE  </b></td>
                        <td>  $<%#sumAllBuilder%> </td>
                    </tr>
                </table>
            </FooterTemplate>
         </asp:Repeater>
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
			Any questions regarding this past due rebate notification email should be directed to CBUSA at  <asp:Literal ID="litllcOperationsManager" runat="server" />.<br />
			<br />
                       <a href="https://dev.custombuilders-usa.com/default.aspx?mod=inv">Click here </a>to go to Rebate Invoices.<br/><br />
                       
			Thank you for your continued support of CBUSA member builders.<br />
			<br />
			Sincerely ,<br />
			 <asp:Literal ID="ltlBuilderGroup" runat="server" />
		</p>
    </div>
   </asp:PlaceHolder>    