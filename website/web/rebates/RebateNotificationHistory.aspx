<%@ Page Language="VB" AutoEventWireup="false" CodeFile="RebateNotificationHistory.aspx.vb" Inherits="RebateNotificationHistory" %>

 
 <%--<html>
 <body>
 <form runat="server">--%>
 
 
    <CT:MasterPage ID="CTMain" runat="server">

<asp:PlaceHolder runat="server">
 <script type="text/javascript">
     function DivToggle(divId) {
         var id = document.getElementById(divId);
         $(id).slideToggle('normal');
     }
     </script>
    <div class="pckggraywrpr">
    <div class="pckghdgred">Report</div>
     <asp:Button id="btnBack" runat="server" Text="Go Back" CssClass="btnblue" />
    <asp:Repeater id="rptVendors" runat="server">
        <HeaderTemplate>
            <table cellpadding="4" cellspacing="1" border="0" style="display:block;width:90%;margin:auto;" class="tblcompr">
            <tr id="trNoRecords" visible="false" runat="server">
                <td>
                    <div id="NoRecords">
                        No records are available.
                    </div>
                </td>
            </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td>
                    <asp:Literal ID="litVendorId" runat="server" Text='<%# Eval("vndrid")%>' Visible="false" /><b> <%# Eval("vndrname")%></b>
                </td>
            </tr>
            <tr>
                <td>  
                    <asp:Repeater id="rptSubmittedDate" runat="server">
                        <HeaderTemplate>
                            <table cellpadding="4" cellspacing="1" border="0" style="display:block;width:100%;margin:auto;" class="tblcompr">
                        </HeaderTemplate> 
                        <ItemTemplate>
                                <tr>
                                    <td><a class="accordionButton" href="#" onclick="DivToggle('tr <%# DirectCast(DirectCast(DirectCast(Container, RepeaterItem).NamingContainer, Repeater).NamingContainer, RepeaterItem).ItemIndex %><%# Container.ItemIndex %>')">View Notices Sent On <asp:Literal ID="litSubmittedDate" runat="server" Text='<%# Eval("submitteddate")%>' /></a></td>
                                   
                                  
                                   
                               </tr>
                               <tr id='tr <%# DirectCast(DirectCast(DirectCast(Container, RepeaterItem).NamingContainer, Repeater).NamingContainer, RepeaterItem).ItemIndex %><%# Container.ItemIndex %>' style="display:none;">
                                    <td colspan ="2">
                                    <asp:Repeater id="rptReportByGroup" runat="server">
                                    <itemtemplate>
                                        <asp:literal id="litGroupId" runat="server" text='<%# Eval("GroupId") %>' visible="false" />
                                       
                                        <asp:Repeater id="rptReport" runat="server">
                                            <HeaderTemplate>
                                                <table cellpadding="4" cellspacing="1" border="0" style="display:block;width:90%;margin:auto;" class="tblcompr">
                                                    
                                                    <tr>
                                                            <th>CBUSA Invoice #</th>
                                                            <th>Name</th>
                                                            <th  width="20%">Qtr-Yr </th>
                                                            <th>Invoice Date</th>
                                                            <th>Days Past Due</th>
                                                            <th>Amount Due</th>
                                                             <th>Last Sent Date/Time</th>
                                                    </tr>
                                                   
                                                     
                                            </HeaderTemplate>
                                         
                                            <ItemTemplate>
                                                    <tr>
                                                        <td><%# Eval("invoice")%></td>
                                                        <td><%# Eval("VNDRNAME")%></td>
                                                        <td><%# Eval("Period")%> - <%# Eval("year")%></td>
                                                        <td><%# Replace(Eval("AECReportDate"), " 12:00:00 AM", "") %></td>
                                                        <td><%# Eval("dayspastdue")%></td>
                                                        <td><%# FormatCurrency(Eval("amountdue"), 2)%></td>
                                                        <td><%# Eval("SubmittedDate")%></td>
                                                   </tr>
                                                 
                                            </ItemTemplate>
                                              
                                            <FooterTemplate>
                                                </table>
                                            </FooterTemplate>
                                        </asp:Repeater>
                                        </itemtemplate>
                                        <separatortemplate>
                                            <hr />
                                        </separatortemplate>
                                        
                                    </asp:Repeater>
                                    </td>
                               </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    
    </div>

</asp:PlaceHolder>    
    </CT:MasterPage>
    <%--</form>
 </body>
 </html>--%>