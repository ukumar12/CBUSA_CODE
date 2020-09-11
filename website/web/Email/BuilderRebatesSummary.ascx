<%@ Control Language="VB" AutoEventWireup="false" CodeFile="BuilderRebatesSummary.ascx.vb" Inherits="Email_BuilderRebatesSummary" %>
 
<asp:PlaceHolder ID="PlaceHolder1" runat="server">
    <div>
         Dear %%BUILDER_COMPANY%%,<br /><br />
         The following vendors are past due on rebate payments. Please click the following link to send automated emails to these vendors requesting prompt payment.
       <br />
         %%Rebate_Report%%
         <br /><br />
         Information as of <%= System.DateTime.Now.Date%>
		 <br />
         <br />
         <asp:Repeater ID="rptVendors" runat="server">
            <HeaderTemplate>
                <table width="100%" border="1" class="arTable">
                    <tr>
                        <th><b>Vendor Name</b></th>
                       <%-- <th><b>Vendor Phone</b></th>--%>
                        <th><b>CBUSA Invoice #</b></th>
                        <th><b>Qtr-Yr</b></th>
                        <th><b>Invoice Date</b></th>
                        <th><b>Days Past Due</b></th>
                       
                        
                        <th><b>Builder Amount</b></th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                    
                    <tr>
                        <td><%# Eval("vndrname")%></td>
                        <%--<td><%# Eval("phone")%></td>--%>
                        <td><%# Eval("invoice")%></td>
                        <td><%# Eval("Period")%> - <%# Eval("year")%></td>
                         <td><%# Replace(Eval("date"), " 12:00:00 AM", "")%></td>
                        <td><%# Eval("dayspastdue")%></td>
                        
                        
                        <td><%# FormatCurrency(Eval("amountdue"), 2)%></td>
                    </tr>
                
            </ItemTemplate>
            <FooterTemplate>
                    <tr class="arTotal">
                        <td colspan="5"><b>TOTAL DUE TO %%BUILDER_COMPANY%%</b></td>
                        <td><asp:Literal ID="litTotalAmount" runat="server" /></td>
                    </tr>
                </table>
            </FooterTemplate>
         </asp:Repeater>
         <br />

           <p style="color:#b4050c; font-size: 14px; font-weight: strong; text-align: center;"> *Any vendor that has an invoice that reaches 180 days past due will be terminated from CBUSA and their rebate invoices will be written off.</p>
			
    </div>
   </asp:PlaceHolder>    