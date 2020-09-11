<%@ Page Language="VB" AutoEventWireup="false" CodeFile="summary.aspx.vb" Inherits="order_summary" %>

<CT:MasterPage ID="CTMain" runat="server">

<asp:PlaceHolder runat="server">
    <script type="text/javascript">
        function OpenSubmitted() {
            Sys.Application.remove_load(OpenSubmitted);
            var frm = $get('<%=frmSubmitted.ClientID %>').control;
            frm._doMoveToCenter();
            frm.Open();
        }
    </script>
</asp:PlaceHolder>

<CC:PopupForm ID="frmSubmitted" runat="server" CloseTriggerId="btnCloseSubmitted" CssClass="pform" Width="300px" ShowVeil="true" VeilCloses="true">
    <FormTemplate>
        <div class="pckggraywrpr" style="margin:0px;">
            <div class="pckghdgred">Order Submission Confirmation</div>
            <p class="center bold larger" style="padding:10px;">
                <asp:Literal id="ltlStatusMsg" runat="server"></asp:Literal><br /><br />
                <asp:Button id="btnCloseSubmitted" runat="server" Cssclass="btnred" text="Close" />
            </p>
        </div>
    </FormTemplate>
    <Buttons>
        <CC:PopupFormButton ControlID="btnCloseSubmitted" ButtonType="ScriptOnly" />
    </Buttons>
</CC:PopupForm>
     
    <h1>Order Summary</h1>
    
    <div class="pckgwhtwrpr">
        <div class="pckghdgred">Order Details</div>
        <table class="bdrgray" border="0" cellpadding="2" cellspacing="2" style="width:550px;margin:10px auto;">
            <tr id="trCampaignName" runat="server">
                <td style="font-size:13px;"><b>Campaign Name:</b></td>
                <td style="font-size:13px;"><asp:Literal id="ltlCampaignName" runat="server"></asp:Literal></td>
            </tr>
            <tr class="alternate">
                <td style="font-size:13px;"><b>Order Title:</b></td>
                <td style="font-size:13px;"><asp:Literal id="ltlTitle" runat="server"></asp:Literal></td>
            </tr>
            <tr>
                <td style="font-size:13px;"><b>Order Number:</b></td>
                <td style="font-size:13px;"><asp:Literal id="ltlOrderNumber" runat="server"></asp:Literal></td>
            </tr>
            <tr class="alternate">
                <td style="font-size:13px;"><b>Project:</b></td>
                <td style="font-size:13px;"><asp:Literal id="ltlProject" runat="server"></asp:Literal></td>
            </tr>
            <tr class="alternate">
                <td style="font-size:13px;"><b>Project Address:</b></td>
                <td style="font-size:13px;"><asp:Literal id="ltlProjectAddress" runat="server"></asp:Literal></td>
            </tr>
            <tr>
                <td style="font-size:13px;"><b>Builder:</b></td>
                <td style="font-size:13px;"><asp:Literal id="ltlBuilder" runat="server"></asp:Literal></td>
            </tr>
	        <tr class="alternate">
		        <td style="font-size:13px;"><b>Vendor:</b></td>
		        <td style="font-size:13px;"><asp:literal id="ltlVendor" runat="server" /></td>
	        </tr>
	        <tr>
		        <td style="font-size:13px;"><b>PO Number:</b></td>
		        <td style="font-size:13px;"><asp:literal id="ltlPONumber" runat="server"></asp:literal></td>
	        </tr>
	        <tr class="alternate largest">
	            <td style="font-size:13px;"><b>Status:</b></td>
	            <td style="font-size:13px;"><asp:Literal id="ltlStatus" runat="server"></asp:Literal></td>
	        </tr>
	        <tr class="largest">
	            <td style="font-size:13px;"><b>Last Updated:</b></td>
	            <td style="font-size:13px;"><asp:Literal id="ltlUpdated" runat="server"></asp:Literal></td>
	        </tr>
	    </table>
        <table cellpadding="5" cellspacing="5" border="0" class="bdrgray" style="width:98%;margin:10px auto;">	    
	    <tr valign="top">
	    <td style="width:45%">
	    <table cellpadding="2" cellspacing="2" border="0">
	        <tr class="alternate">
		        <td style="font-size:13px;">Your First Name:</td>
		        <td style="font-size:13px;"><asp:literal id="ltlOrdererFirstName" runat="server" ></asp:literal></td>
	        </tr>
	        <tr>
		        <td style="font-size:13px;">Your Last Name:</td>
		        <td style="font-size:13px;"><asp:literal id="ltlOrdererLastName" runat="server" ></asp:literal></td>
	        </tr>
	        <tr class="alternate">
		        <td style="font-size:13px;">Your Email:</td>
		        <td style="font-size:13px;"><asp:literal id="ltlOrdererEmail" runat="server" ></asp:literal></td>
	        </tr>
	        <tr>
		        <td style="font-size:13px;">Your Phone:</td>
		        <td style="font-size:13px;"><asp:literal id="ltlOrdererPhone" runat="server" ></asp:literal></td>
	        </tr>
	        <tr class="alternate">
		        <td style="font-size:13px;">Supervisor First Name:</td>
		        <td style="font-size:13px;" class="field"><asp:literal id="ltlSuperFirstName" runat="server" ></asp:literal></td>
	        </tr>
	        <tr>
		        <td style="font-size:13px;" class="required">Supervisor Last Name:</td>
		        <td style="font-size:13px;" class="field"><asp:literal id="ltlSuperLastName" runat="server" ></asp:literal></td>
	        </tr>
	        <tr class="alternate">
		        <td style="font-size:13px;" class="required">Supervisor Email:</td>
		        <td style="font-size:13px;" class="field"><asp:literal id="ltlSuperEmail" runat="server" ></asp:literal></td>
	        </tr>
	        <tr>
		        <td style="font-size:13px;" class="required">Supervisor Phone:</td>
		        <td style="font-size:13px;" class="field"><asp:literal id="ltlSuperPhone" runat="server" ></asp:literal></td>
	        </tr>
	    </table>
	    </td>
	    <td class="spacercol">&nbsp;</td>
	    <td>
	    <table cellpadding="2" cellspacing="2" border="0">
	        <tr class="alternate">
		        <td style="font-size:13px;" class="required">Subtotal:</td>
		        <td style="font-size:13px;" class="field"><asp:literal id="ltlSubtotal" runat="server" ></asp:literal></td>
	        </tr>
	        <tr>
		        <td style="font-size:13px;" class="required">Tax:</td>
		        <td style="font-size:13px;" class="field"><asp:literal id="ltlTax" runat="server" ></asp:literal></td>
	        </tr>
	        <tr class="alternate">
		        <td style="font-size:13px;" class="required">Total:</td>
		        <td style="font-size:13px;" class="field"><asp:literal id="ltlTotal" runat="server" ></asp:literal></td>
	        </tr>
	        <tr>
		        <td style="font-size:13px;" class="optional">Requested Delivery:</td>
		        <td style="font-size:13px;" class="field"><asp:Literal id="ltlRequestedDelivery" runat="server"></asp:Literal></td>
	        </tr>
        </table>
        </td>
        <td class="spacercol">&nbsp;</td>
        <td>
        <table cellpadding="2" cellspacing="2" border="0">
            <tr class="alternate">
	            <td style="font-size:13px;" class="optional">Delivery Instructions:</td>
	            <td style="font-size:13px;" class="field"><asp:Literal id="ltlDeliveryInstructions" runat="server"></asp:literal></td>
            </tr>
	        <tr>
		        <td style="font-size:13px;" class="required">Remote IP:</td>
		        <td style="font-size:13px;" class="field"><asp:literal id="ltlRemoteIP" runat="server" ></asp:literal></td>
	        </tr>
	        <tr class="alternate">
		        <td style="font-size:13px;" class="required">Creator Account:</td>
		        <td style="font-size:13px;" class="field"><asp:literal id="ltlCreatorBuilder" runat="server" ></asp:literal></td>
	        </tr>
        </table>
        </td>
        </tr>
        </table>
        
        <table cellpadding="0" cellspacing="2" border="0" style="table-layout:fixed;width:80%;margin:5px auto;" class="bdrgray">
            <tr>
                <td><div class="pckghdgblue">Builder Notes</div></td>
                <td><div class="pckghdgblue">Vendor Notes</div></td>
            </tr>
            <tr>
                <td style="padding:5px;">
                    <asp:TextBox id="txtBuilderNotes" runat="server" textmode="Multiline" rows="10" style="width:100%;"></asp:TextBox>
                </td>
                <td style="padding:5px;">
                    <asp:TextBox id="txtVendorNotes" runat="server" textmode="Multiline" rows="10" style="width:100%;"></asp:TextBox>    
                </td>
            </tr>
        </table>
    </div>

    <div class="pckggraywrpr">
        <div class="pckghdgred">Items And Drops</div>
        <asp:Repeater id="rptDrops" runat="server">
            <ItemTemplate>
                <div class="pckgltgraywrpr" style="width:90%;margin:10px auto;">
                    <table cellpadding="5" cellspacing="0" border="0" class="tblcompr automargin larger" style="width:100%;">
                    <tr>
                        <td colspan="6" style="padding:0px;"><div class="pckghdgblue"><asp:Literal id="ltlDropTitle" runat="server"></asp:Literal></div></td>
                    </tr>
                    <tr>
                        <td colspan="6" ><asp:Literal id="ltlDropDetails" runat="server"></asp:Literal></td>
                    </tr>
                    <tr>
                        <th>Qty</th>
                        <th>SKU</th>
                        <th>CBUSA-SKU</th>
                        <th>Product</th>
                        <th>Unit Price</th>
                        <th>Total Price</th>
                    </tr>
                <asp:Repeater id="rptItems" runat="server">
                    <ItemTemplate>
                        <tr valign="top" class='<%#iif(Container.ItemIndex mod 2 = 1,"alternate","row") %>'>
                            <td><b><%#DataBinder.Eval(Container.DataItem, "QUantity")%></b></td>
                            <td style="font-size:13px;"><asp:Literal id="ltlVendorSku" runat="server"></asp:Literal></td>
                            <td style="font-size:13px;"><asp:Literal id="ltlcbusaSku" runat="server"></asp:Literal></td>
                            <td style="font-size:13px;"><asp:Literal id="ltlProduct" runat="server"></asp:Literal></td>
                            <td style="font-size:13px;"><asp:Literal id="ltlUnitPrice" runat="server"></asp:Literal></td>
                            <td style="font-size:13px;"><asp:Literal id="ltlPrice" runat="server"></asp:Literal></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                    <tr>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>                        
                        <td class="bold larger">Subtotals</td>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>Total Items:</td>
                        <td><asp:Literal id="ltlTotalItems" runat="server"></asp:Literal></td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>Total Quantity:</td>
                        <td><asp:Literal id="ltlTotalQuantity" runat="server"></asp:Literal></td>                    
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>Drop Subtotal:</td>
                        <td><asp:Literal id="ltlSubtotal" runat="server"></asp:Literal></td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>Drop Tax:</td>
                        <td><asp:Literal id="ltlTax" runat="server"></asp:Literal></td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>Drop Total:</td>
                        <td><asp:Literal id="ltlTotalPrice" runat="server"></asp:Literal></td>                    
                    </tr>
                </table>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <div class="clear">&nbsp;</div>

                <div class="pckgltgraywrpr" style="width:90%;margin:10px auto;">
                    <table cellpadding="5" cellspacing="1" border="0" class="tblcompr larger" style="background-color:#fff;width:100%;margin:0px">
                    <tr>
                        <td colspan="6" style="padding:0px;"><div class="pckghdgblue">All Items</div></td>
                    </tr>
                    <tr>
                        <th>Qty</th>
                        <th>SKU</th>
                        <th>CBUSA-SKU</th>
                        <th>Product</th>
                        <th>Unit Price</th>
                        <th>Total Price</th>
                    </tr>
                <asp:Repeater id="rptItems" runat="server">
                    <ItemTemplate>
                        <tr valign="top" class='<%#iif(Container.ItemIndex mod 2 = 1,"alternate","row") %>'>
                            <td><b><%#DataBinder.Eval(Container.DataItem, "QUantity")%></b></td>
                            <td style="font-size:13px;"><asp:Literal id="ltlVendorSku" runat="server"></asp:Literal></td>
                            <td style="font-size:13px;"><asp:Literal id="ltlcbusaSku" runat="server"></asp:Literal></td>
                            <td style="font-size:13px;"><asp:Literal id="ltlProduct" runat="server"></asp:Literal></td>
                            <td style="font-size:13px;"><asp:Literal id="ltlUnitPrice" runat="server"></asp:Literal></td>
                            <td style="font-size:13px;"><asp:Literal id="ltlPrice" runat="server"></asp:Literal></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                    <tr>
                        <td colspan="6" align="center" class="bold larger">Order Totals</td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td class="bold">Total Items:</td>
                        <td style="font-size:13px;"><asp:Literal id="ltlTotalItems" runat="server"></asp:Literal></td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td class="bold">Total Quantity:</td>
                        <td style="font-size:13px;"><asp:Literal id="ltlTotalQuantity" runat="server"></asp:Literal></td>                    
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td class="bold">Order Subtotal:</td>
                        <td style="font-size:13px;"><asp:Literal id="ltlTotalSubtotal" runat="server"></asp:Literal></td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td class="bold">Order Tax:</td>                    
                        <td style="font-size:13px;"><asp:Literal id="ltlTotalTax" runat="server"></asp:Literal></td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td class="bold">Order Total:</td>
                        <td style="font-size:13px;"><asp:Literal id="ltlTotalPrice" runat="server"></asp:Literal></td>                    
                    </tr>
                </table>
                <p class="center">
                    <table cellpadding="0" cellspacing="0" border="0">
                        <tr valign="middle">
                        <td>
                            <asp:Button id="btnSubmit" runat="server" text="Submit Order to Vendor" cssclass="btnred" />
                        </td>
                        <td>
                            <asp:HyperLink id="lnkDrops" runat="server" text="Edit Drops" cssclass="btndrop"></asp:HyperLink>    
                        </td>
                        <td>
                            <asp:Hyperlink id="lnkDetails" runat="server" text="Edit Order Details" cssclass="btndrop"></asp:Hyperlink>    
                        </td>
                          <td>
                            <asp:Button id="btnExport" runat="server" text="Export Order(CSV Format)" cssclass="btnred"/>
                        </td>
                           <td>
                            <asp:Button id="btnExportMAS" runat="server" text="Export Order(Pipe.Txt Format)" cssclass="btnred"/>
                        </td>

                        </tr>
                    </table>
                </p>
                </div>
                
                <asp:Panel ID="pnlPrint" runat="server">
                    <div style="text-align: center;">
                        <input type="button" class="btnred" value="Print This Page" onclick="window.open('<%=PrintUrl%>', 'PrintPage', ''); return false;" />
                    </div>    
                </asp:Panel>

</CT:MasterPage>