 <%@ Page Language="VB" AutoEventWireup="false" CodeFile="rebate-notification.aspx.vb"    Inherits="rebate_notification" %>

    <CT:MasterPage ID="CTMain" runat="server">

<asp:PlaceHolder runat="server">
<script type="text/javascript">

    $(function () {
        var checkboxes = $('.allcheckbox :checkbox');
        $('input[name="btnCheckAll"]').click(function () {
            checkboxes.attr('checked', 'checked');
        });

        $('input[name="btnUnCheckAll"]').click(function () {
            checkboxes.removeAttr('checked', 'checked');
        });
    });
	
</script>
<style>
#btnHistory{margin-left:465px;}
 
</style>
    <div class="pckggraywrpr">
   
    <div   id="divbtn" runat="server" >
      <div class="pckghdgred">Report </div> 
      
                 	<asp:RadioButtonList  runat="server" ID="RblEmailPreference" Autopostback= "true" RepeatDirection="Vertical">
			<asp:ListItem Text="Automatically send Past Due Rebate Notice to all vendors once every three weeks" Value="True"  />
			<asp:ListItem Text="Manually send Past Due Rebate Notice to selected vendors" Value="False" />
			</asp:RadioButtonList>

        
      <input type="button" name="btnCheckAll" class="btnblue" value="Check All" />
                    <input type="button" name="btnUnCheckAll" class="btnblue" value="Uncheck All" />   
                 <asp:Button id="btnSubmitTop" runat="server" Text="Send Notice to Selected" CssClass="btnblue" /> 
    
                      <asp:Button id="btnHistory" runat="server" Text="View History" CssClass="btnblue" /></div>
 				   <asp:literal id="ltlmsgSend" runat="server" Text="Notice successfully sent to Vendors" visible="false" /></div>
                      <div id="NoRecords" runat="server">
                        No records are available.
                    </div>
    <asp:Repeater id="rptVendors" runat="server">
        <HeaderTemplate>
            <table cellpadding="4" cellspacing="1" border="0" style="display:block;width:100%;margin:auto;" class="tblcompr rebateReportList">   
				 <tr>
					<th>Vendor Name</th>
					<th width="90">CBUSA Invoice #</th>
					<th width="90">Qtr-Yr </th>
					<th width="90">Invoice Date</th>
					<th width="70">Days Past Due</th>
					<th width="100">Invoice Amount</th>
                    <th width="75">Rebate Amount</th>
					<th width="75">Last Sent Date</th>
					<th width="250"></th>
               </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td class="rebateListVendorName">
					<table>
						
							<td><asp:CheckBox  ID="chkSelect" runat="server" Checked="true" class="allcheckbox" /></td>
							<td><asp:Literal ID="litVendorId" runat="server" Text='<%# Eval("vndrid")%>' Visible="false" /> <%# Eval("vndrname")%></td>
                             
						</tr>
					</table>
                </td>
                <td colspan="7">  
                    <asp:Repeater id="rptReport" runat="server">
                        <HeaderTemplate>
                            <table cellpadding="4" cellspacing="1" border="0" style="display:block;width:100%;margin:auto; border: none;" class="tblcompr">

                        </HeaderTemplate>
                        <ItemTemplate>
                                <tr>
                                    <td width="90"><%# Eval("invoice")%></td>
                                     <td width="90"><%# Eval("Period")%> - <%# Eval("year")%></td>
                                    <td width="90"><%# Replace(Eval("date"), " 12:00:00 AM", "") %></td>
                                    <td width="70"><%# Eval("dayspastdue")%></td>
                                    <td width="75"><asp:Literal ID="litRebateAmount" runat="server" /></td>
                                    <td width="100"><%# FormatCurrency(Eval("amountdue"), 2)%></td>
                                    <td width="75"><asp:Literal ID="litSubmittedDate" runat="server" /></td>
                               </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>

                </td>
                
                <td style="text-align: center;" width="250"> <asp:HyperLink ID ="lnkNoticeType" Text="Preview"  CommandArgument='<%# Eval("vndrid")%>' runat ="server"  CssClass="btnred" />
                <asp:LinkButton id="lnkSendNotice" runat="server" Text="Send Notice" CssClass="btnred" CommandArgument='<%# Eval("vndrid")%>' CommandName="SendNotice" /></td>				
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <asp:Button id="btnSubmit" runat="server" Text="Send Notice to Selected" CssClass="btnblue" />
    </div>

</asp:PlaceHolder>    
    </CT:MasterPage>
