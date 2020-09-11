<%@ Page Language="VB" AutoEventWireup="false" CodeFile="VendorRebateNotice1.aspx.vb" Inherits="Email_VendorRebateNotice" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

 <CT:MasterPage ID="CTMain" runat="server">

<asp:PlaceHolder runat="server">
    <div>
         Dear %%BUILDER_COMPANY%%,<br /><br />
         The following venodrs are past due on rebate payments. Please click the link below the table to send automated reminders to these vendors requesting prompt payment.
         <br /><br />
         Information as of <%= System.DateTime.Now.Date%>

         <br />
         <asp:Repeater ID="rptVendors" runat="server">
            <HeaderTemplate>
                <table width="100%" border="1">
                    <tr>
                        <td><b>CBUSA Invoice #</b></td>
                        <td><b>Qtr-Yr</b></td>
                        <td><b>Invoice Date</b></td>
                        <td><b>Dayes Past Due</b></td>
                        <td><b>Amount Due</b></td>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                    
                    <tr>
                        <td><%# Eval("invoice")%></td>
                        <td><%# Eval("Qtr/Yr")%></td>
                        <td><%# Eval("date")%></td>
                        <td><%# Eval("dayspastdue")%></td>
                        <td><%# Eval("amountdue")%></td>
                    </tr>
                
            </ItemTemplate>
            <FooterTemplate>
                    <tr>
                        <td colspan="8"><b>TOTAL DUE TO %%BUILDER_COMPANY%%</b></td>
                        <td><asp:Literal ID="litTotalAmount" runat="server" /></td>
                    </tr>
                </table>
            </FooterTemplate>
         </asp:Repeater>

         <br />
         <asp:Repeater ID="rptOtherBuildersVendors" runat="server">
            <HeaderTemplate>
                <table width="100%" border="1">
                    <tr>
                        <td><b>CBUSA Invoice #</b></td>
                        <td><b>Qtr-Yr</b></td>
                        <td><b>Invoice Date</b></td>
                        <td><b>Dayes Past Due</b></td>
                        <td><b>Amount Due</b></td>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                    
                    <tr>
                        <td><%# Eval("invoice")%></td>
                        <td><%# Eval("Qtr/Yr")%></td>
                        <td><%# Eval("date")%></td>
                        <td><%# Eval("dayspastdue")%></td>
                        <td><%# Eval("amountdue")%></td>
                    </tr>
                
            </ItemTemplate>
            <FooterTemplate>
                    <tr>
                        <td colspan="8"><b>TOTAL DUE TO %%BUILDER_COMPANY%%</b></td>
                        <td><asp:Literal ID="litTotalAmount" runat="server" /></td>
                    </tr>
                </table>
            </FooterTemplate>
         </asp:Repeater>
         <b>
            *Any vendor that has an invoice that reaches 174 days past due will be terminated from CBUSA and their rebate invoices will be written off
         </b>
    </div>
   </asp:PlaceHolder>    
    </CT:MasterPage>

