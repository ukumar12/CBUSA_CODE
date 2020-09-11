<%@ Control Language="VB" EnableViewState="true" AutoEventWireup="false" CodeFile="StoreShoppingCartAdmin.ascx.vb" Inherits="StoreShoppingCartAdmin" %>
<%@ Import Namespace="Components" %>

<asp:UpdatePanel ID="upShoppingCart" runat="server" UpdateMode="Conditional">
<ContentTemplate>
<table cellspacing="0" cellpadding="0" border="0" class="carttbl">
<tr id="trGiftWrapping" runat="server" visible="False"><td colspan="5" align="right"><b>Gift Wrapping</b>: <asp:Literal id="ltlGiftWrappingPrice" runat="server"></asp:Literal> per item</td></tr>
<tr><td colspan="5">
    <div class="bdr center" style="padding:10px; background-color:#fff;text-align:left;" id="divExtraInfo">To send a order status update email, select the items you would like to include and click the 'Save and Send' button. A pop-up box will appear where you can edit some of the information. Then click 'Send' button in the pop-up to send the email to the customer.</div>
</td></tr>
<tr id="trSendStatus" runat="server" visible="false">
    <td colspan="5" align="right" style="padding: 10px 0 10px 0;"><CC:OneClickButton ID="btnPrintPacking" CssClass="btn" runat="server" Text="Print Packing List(s)" /> <CC:OneClickButton ID="btnSendStatus" CssClass="btn" runat="server" Text="Save & Send Email" /><input type="checkbox" ID="chkSelectAll" IsItem="0" IsAll="1" onclick="selectAll(this);" runat="server" />
    </td></tr>
<asp:Repeater runat="server" id="rptRecipients">
<ItemTemplate>
<tr id="trMultipleShipTo" runat="server"><th colspan="5" class="bdrbottom">For <%#Container.DataItem("Label")%><%#IIf(IsDBNull(Container.DataItem("AddressId")), "", " (address book)")%></th></tr>
<tr id="trSingleShipTo" runat="server"><th colspan="5" class="bdrbottom">&nbsp;</th></tr>
<tr><td colspan="5" class="bdrleft bdrright bdrbottom" id="tdShipping" runat="server">
	<table border="0">
	<tr><td>
	<b>Ship To:&nbsp;&nbsp;&nbsp;</b>
	</td><td>
	<div><asp:Literal ID="ltlFullName" runat="server"  /></div>
	<div id="divCompany" runat="server"><%#Core.HTMLEncode(Container.DataItem("Company"))%></div>
	<div><%#Core.HTMLEncode(Container.DataItem("Address1"))%></div>
	<div id="divAddress2" runat="server"><%#Core.HTmlEncode(Container.DataItem("Address2"))%></div>
	<div><%#Core.HTMLEncode(Container.DataItem("City"))%>, <%#Core.HTMLEncode(Container.DataItem("State"))%>&nbsp;<%#Core.HTMLEncode(Container.DataItem("Zip"))%></div>
	<div id="divRegion" runat="server"><%#Core.HTMLEncode(Container.DataItem("Region"))%></div>
	<div><asp:Literal ID="ltlCountry" runat="server" EnableViewState="False" /></div>
	<div><%#Core.HTMLEncode(Container.DataItem("PHONE"))%></div>

	</td><td style="vertical-align: bottom;">&nbsp;&nbsp;&nbsp;</td><td style="vertical-align: bottom;">

	<div runat="server" id="divShippingMethod"></div>

	<% If EditMode Then%>
	<div style="margin-top:10px;"><input type="button" class="btn" id="btnEdit" value="Edit Shipping" runat="server"></div>
	<% end if %>

	</td>
	<td>&nbsp;&nbsp;&nbsp;</td>
	<td id="tdGiftMessageLabel" runat="server">
	<b>Gift Message:&nbsp;&nbsp;&nbsp;</b>
	</td>
	<td id="tdGiftMessage" runat="server">
		<div><asp:Literal Id="ltlGiftMessage" runat="server" /></div>
    </td></tr>
	</table>
</td></tr>
<tr runat="server" id="trAdminShippingStatus" visible="false">
        <td colspan="5" class="bdrbottom bdrleft bdrright" style="background-color:#ffffff;padding:0px;">
        <table cellpadding="0" cellspacing="1" border="0" width="100%">
<tr>
                <td class="field" style="vertical-align:middle;padding:3px;"><span class="smallest">For this recipient:</span><br />
                <asp:DropDownList ID="drpIsShippingIndividually" runat="server" OnSelectedIndexChanged="drpIsShippingIndividually_SelectedIndexChanged">
                    <asp:ListItem Value="false">Ship items together</asp:ListItem>
                    <asp:ListItem Value="true">Ship items individually</asp:ListItem></asp:DropDownList></td>
                <td class="field" style="width:100%;">
                <table id="tblRecipientStatusFields" runat="server" border="0" cellpadding="3" cellspacing="1" style="width:100%;padding-top: 0px;">
                    <tr>
                        <td class="field" style="vertical-align:middle;width:100px;"><span class="smallest">Status</span><br />
                            <asp:DropDownList ID="drpShipmentStatus" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpRecipientStatus_SelectedIndexChanged"></asp:DropDownList></td>
                        <td class="field" style="width:100px;" id="tdRecipientShippedDate" runat="server"><span class="smallest">Shipped Date</span><br />
                            <CC:DatePicker ID="dpRecipientShippedDate" runat="server"></CC:DatePicker>
                    </td>
                        <td class="field" id="tdRecipientTrackingNumber" runat="server"><span class="smallest">Tracking Number</span><br />
                            <asp:TextBox ID="txtRecipientTrackingNumber" runat="server" Width="200"></asp:TextBox></td>                            
            </tr>            
        </table>
                </td>
                <td class="field right" style="vertical-align:bottom;padding:4px;width:20px;"><input type="checkbox" ID="chkSelectAllRecipientItems" IsItem="0" runat="server" /></td>
            </tr>
        </table>
        
        </td>
    </tr>	
<tr>
<td class="baghdr bdrleft"><b>Item</b></td>
<td id="tdQuantity" runat="server" class="baghdr"><b>Quantity</b></td>
<td id="tdGiftWrap" runat="server" class="baghdr"><b>Gift Wrap</b></td>
<td class="baghdr" align="right"><b>Price</b></td>
<td class="baghdr bdrright" align="right"><b>Total</b></td>
</tr>
<asp:Repeater runat="server" id="rptCart">
	<ItemTemplate>
    <tr valign="top">
    <td id="tdDetails" class="bdrbottom bdrleft" width="400" runat="server">
		<table border="0">
		<tr>
		<td id="tdImage" runat="server" align="center" valign="top">
		<a id="lnk1" runat="server" href=""><img id="img" src="" runat="server" alt="" border="0" /></a><br />
		<a id="lnk2" runat="server" href="" class="smallest">Edit Item</a>
		</td>
		<td valign="top">
		<a id="lnk3" runat="server" href=""><b><%#Container.DataItem("ItemName")%></b></a><br />
		Item no: <%#Container.DataItem("SKU")%><br /><br />
		<span id="spanAttributesWrapper" runat="server">Attributes:<br /><span id="spanAttributes" runat="server" class="smallest"></span></span>
		<span id="spanShipStatus" runat="server"></span>
		</td>
		</tr>
		</table>
	</td>
	<td id="tdQuantity" runat="server" class="bdrbottom"><%#Container.DataItem("Quantity")%></td>
	<td id="tdGiftWrap" runat="server" class="bdrbottom">&nbsp;</td>
	<td id="tdItemPrice" runat="server" class="bdrbottom right"><%#FormatCurrency(Container.DataItem("Price"))%></td>
	<td id="tdPrice" runat="server" class="bdrright bdrbottom right"><%#FormatCurrency(Container.DataItem("Price") * Container.DataItem("Quantity"))%></td>
	</tr>    
    <tr runat="server" id="trAdminShippingStatus" visible="false">
        <td colspan="4" class="bdrbottom bdrleft  field" style="padding:0px;">
                    <table id="tblItemStatusFields" runat="server" cellpadding="3" border="0" cellspacing="1">
                        <tr>
                            <td class="field" style="width:100px;"><span class="smallest">Item Status</span><br />
                                <asp:DropDownList ID="drpItemStatus" AutoPostBack="true" OnSelectedIndexChanged="drpItemStatus_SelectedIndexChanged" runat="server"></asp:DropDownList></td>
                            <td class="field" style="width:100px;" id="tdShippedDate" runat="server"><span class="smallest">Shipped Date</span><br />
                                <CC:DatePicker ID="dpShippedDate" runat="server"></CC:DatePicker>
                            </td>
                            <td class="field" style="width:200px;" id="tdItemTrackingNumber" runat="server"><span class="smallest">Tracking Number</span><br />
                                <asp:TextBox ID="txtTrackingNumber" runat="server" Width="200"></asp:TextBox></td>
                        </tr>
                    </table>
        </td>
        <td class="field bdrright bdrbottom right" style="width:20px; vertical-align:middle; text-align:right;padding:4px;"><input type="checkbox" ID="chkStatusItem" IsItem="1" runat="server" /></td>
    </tr>    	
	</ItemTemplate>
</asp:Repeater>

<tr id="trRecipientSummary" runat="server">
<td runat="server" colspan="4" class="bdrleft right">Merchandise Subtotal:</td>
<td runat="server" class="bdrright right"><%#FormatCurrency(Container.DataItem("BaseSubtotal"))%></td>
</tr>
<tr id="trGiftWrappingBottom" runat="server">
<td colspan="4" class="bdrleft right">Gift Wrapping:</td>
<td id="tdBottom3" class="bdrright right"><%#FormatCurrency(Container.DataItem("GiftWrapping"))%></td>
</tr>
<tr id="trDiscountBottom" runat="server">
<td colspan="4" class="bdrleft right">Discount:</td>
<td class="bdrright right"><%#FormatCurrency(Container.DataItem("Discount"))%></td>
</tr>
<tr id="trShippingBottom" runat="server">
<td colspan="4" class="bdrleft right">Shipping:</td>
<td class="bdrright right"><%#FormatCurrency(Container.DataItem("Shipping"))%></td>
</tr>
<tr id="trTaxBottom" runat="server">
<td colspan="4" class="bdrleft right">Tax:</td>
<td class="bdrright right"><%#FormatCurrency(Container.DataItem("Tax"))%></td>
</tr>
<tr id="trTotalBottom" runat="server">
<td colspan="4" class="bdrleft right bdrbottom">Total:</td>
<td class="bdrright right bdrbottom"><%#FormatCurrency(Container.DataItem("Total"))%></td>
</tr>
</ItemTemplate>

<SeparatorTemplate>
<tr><td colspan="5">&nbsp;</td></tr>
</SeparatorTemplate>

</asp:Repeater>

<tr><td colspan="5">&nbsp;</td></tr>
<tr><td colspan="5" class="baghdr bdr">

<table cellpadding="5" cellspacing="0" border="0" width="100%" id="tblTotal" runat="server">
<tr>
<td id="tdTotal" runat="server">TOTAL</td>
<td align="right">
<div class="red"><asp:Literal runat="server" id="ltlPromotionMessage" /></div>
</td></tr>
</table>

</td></tr>

<tr id="trOrderSubTotal" runat="server">
<td rowspan="5" colspan="2" valign="top">
&nbsp;
</td>
<td colspan="2" class="top right bold">Merchandise Subtotal:</td>
<td align="right" valign="top"><% =FormatCurrency(dbOrder.BaseSubtotal)%></td>
</tr>
<%  If dbOrder.GiftWrapping > 0 Then%>
<tr id="trOrderGiftWrapping" runat="server">
<td colspan="2" class="top right bold">Gift Wrapping:</td>
<td class="top right bold"><% =FormatCurrency(dbOrder.GiftWrapping)%></td>
</tr>
<% End If%>
<%  If dbOrder.Discount > 0 Then%>
<tr id="trOrderDiscount" runat="server">
<td colspan="2" class="top right bold">Discount:</td>
<td class="top right bold"><% =FormatCurrency(dbOrder.Discount)%></td>
</tr>
<% End If%>
<tr  id="trOrderShipping" runat="server">
<td colspan="2" class="top right bold">Shipping:</td>
<td class="top right bold"><% =FormatCurrency(dbOrder.Shipping)%></td>
</tr>
<tr id="trOrderTax" runat="server">
<td colspan="2" class="top right bold">Tax:</td>
<td class="top right bold"><% =FormatCurrency(dbOrder.Tax)%></td>
</tr>
<tr id="trOrderTotal" runat="server">
<td colspan="2" class="top right bold">Total:</td>
<td class="top right bold"><% =FormatCurrency(dbOrder.Total)%></td>
</tr>
</table>

            <table cellpadding="5" cellspacing="2" border="0" id="tblOrderSTatus" runat="server" visible="false">
                <tr>
                    <td class="optional">Order Status</td>
                <td class="field">
                    <asp:RadioButtonList ID="radOrderStatus" runat="server" CellSpacing="2" RepeatLayout="Table" RepeatColumns="4" RepeatDirection="Horizontal" /></td></tr></table>
        <div id="divSendStatusEmail" style="z-index: 9000; display:none; width:850px; height: 650px;position:absolute; left:25px; top:25px; border:1px solid #e3e3e3; background-color: White;"><iframe id="ifrmSendEmail"  src="" frameborder="0" style="width:850px; height:650px;"></iframe></div>
        <div id="divPackingList" style="z-index: 9000; display:none; width:850px; height: 680px;position:absolute; left:25px; top:25px; border:1px solid #e3e3e3; background-color: White;"><center><a href="javascript:HidePackingList();" class="smaller">Close Window</a><asp:Literal ID="ltlPackingListPrint" runat="server" /></center><br /><iframe id="ifrmPackingList"  src="" frameborder="0" style="width:850px; height:650px;"></iframe></div>
            
<asp:Panel ID="pnlAdminScripts" runat="server" Visible="false">
<script type="text/javascript">
<!--

function selectAll(chk){

    var o = document.getElementsByTagName('INPUT');
    for(var i = 0; i<o.length;i++){
        
        if(o[i]){
            if(o[i].type=='checkbox'){
                var isItem = o[i].getAttribute('IsItem');
                if (isItem!=null){
                    if(isItem == '0' && o[i].id!=chk.id){
                        o[i].checked = chk.checked;
                        if(chk.checked){
                            selectAllRecipientItems(o[i].getAttribute('RecipientId'),1)
                        }else{  
                            selectAllRecipientItems(o[i].getAttribute('RecipientId'),0)
                        }
                        
                    }
                }
            }
        }    
    } 
}


function selectAllRecipientItems(RecipientId,chk){

    var o = document.getElementsByTagName('INPUT');
    for (var i = 0; i<o.length;i++){
        if(o[i]){
            if(o[i].type=='checkbox'){
                var iRecipientId = o[i].getAttribute('RecipientId');
                if (iRecipientId!=null){
                    if(iRecipientId==RecipientId && o[i].getAttribute('IsItem') == '1'){
                        if (chk=='1')                
                            o[i].checked = true;
                        else
                            o[i].checked = false;
                    }
                }
            
            }
        }
    }
}

function deselectSelectAllCheckbox(RecipientId){
    var o = document.getElementsByTagName('INPUT');
    for (var i = 0; i<o.length;i++){
        if(o[i]){
            if(o[i].type=='checkbox'){
                var iRecipientId = o[i].getAttribute('RecipientId');
                if (iRecipientId!=null){
                    if(iRecipientId==RecipientId && o[i].getAttribute('IsItem') == '0' ){
                            o[i].checked = false;
                    }
                }
            
            }
        }
    }
}

function PrintPackingList(URL) {
  document.getElementById('ifrmPackingList').src = URL;
  ShowDiv('divPackingList');
}

function HidePackingList() { 
  HideDiv('divPackingList');
}

function popSendStatus(){  
    var o = document.getElementsByTagName('INPUT');
    var RecipientId = '0';
    var OrderItemId = '0';
    for (var i = 0; i<o.length;i++){
        if(o[i]){
            if(o[i].type=='checkbox'){
                var isItem = o[i].getAttribute('IsItem');
                var iRecipientId = o[i].getAttribute('RecipientId');
                var iOrderItemId = o[i].getAttribute('OrderItemId');
                var isAll = o[i].getAttribute('IsAll');
                if (isAll==null){
                    if(iRecipientId!=null){
                        if(iOrderItemId!=null){ 
                            if(o[i].checked){
                                OrderItemId += ',' + iOrderItemId;
                            }
                        }else{
                            if (o[i].checked)
                                RecipientId += ',' + iRecipientId;
                        }
                    }                
                }                
            }
        }
    }
    if (RecipientId == "0" && OrderItemId == "0"){
        alert("Please select the items/recipients of which you would like to send an update.");
        return;
    }
    document.getElementById('ifrmSendEmail').src = '/admin/store/orders/sendstatus.aspx?OrderId=<%=OrderId %>&RecipientId=' + RecipientId + '&OrderItemId=' + OrderItemId + '&StatusEmail=y';
    ShowDiv('divSendStatusEmail');
}

function closeSend(){
    HideDiv('divSendStatusEmail');
    document.getElementById('iframenotes').src = "/admin/store/orders/notes.aspx?OrderId=<%=OrderId%>";
}
    
//-->
</script>

</asp:Panel>
</ContentTemplate>
<Triggers>
  <asp:AsyncPostBackTrigger ControlID="btnPrintPacking" EventName="Click" />
</Triggers>
</asp:UpdatePanel>
