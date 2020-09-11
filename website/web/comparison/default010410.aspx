<%@ Page Language="VB" AutoEventWireup="false" CodeFile="default.aspx.vb" Inherits="comparison_default2" %>
<%@ Register TagName="VendorSelectForm" TagPrefix="CC" Src="~/controls/VendorSelectForm.ascx" %>

<CT:MasterPage ID="CTMain" runat="server">
<asp:PlaceHolder runat="server">
<script type="text/javascript">
    function pageLoad() {
        Sys.Application.add_init(InitPriceComparison);
    }
    function InitPriceComparison() {
        $addHandler(document.forms[0], 'submit', ShowSearching);
        var vl = $get('<%=lsVendors.ClientId %>');
        if (vl && !vl.ClickAdded) {
            vl.control.add_listclick(handleClick);
            vl.ClickAdded = true;
        }
    }
    function handleClick(sender,args) {
        ShowSearching();
    }
    function OpenSubForm(takeoffProductId, vendorId, name, sku, price, qty) {
        var c = $get('<%=frmSubstitute.ClientID %>').control;
        c.GetElementByServerId('spanProduct').innerHTML = name;
        c.GetElementByServerId('spanSubSku').innerHTML = sku ? sku : 'N/A';
        c.GetElementByServerId('spanSubPrice').innerHTML = price;
        c.get_input('txtSubQuantity').value = qty;
        c.get_input('hdnSubTakeoffProductID').value = takeoffProductId;
        c.get_input('hdnSubVendorID').value = vendorId;
        c._doMoveToCenter();
        c.Open();
        return false;
    }
    function OpenSpecialForm(takeoffProductId,vendorId,name,price) {
        var c = $get('<%=frmAverage.ClientID %>').control;
        c.GetElementByServerId('spanAverageProduct').innerHTML = name;
        c.GetElementByServerId('spanAveragePrice').innerHTML = price;
        c.get_input('hdnAvgTakeoffProductID').value = takeoffProductId;
        c.get_input('hdnAvgVendorID').value = vendorId;
        c._doMoveToCenter();
        c.Open();
    }
    function OpenExportForm() {
        Sys.Application.remove_load(OpenExportForm);
        var c = $get('<%=frmExport.ClientID %>').control;
        c.Open();
    }
    function ToggleDetails() {
        var div = $get('divTakeoff');
        $(div).slideToggle('fast', null);
        var lnk = $get('lnkDetails');
        if (lnk.innerHTML == 'Show Details') {
            lnk.innerHTML = 'Hide Details';
        } else {
            lnk.innerHTML = 'Show Details';
        }
    }
    function ShowSearching() {
        var frm = $get('<%=frmLoading.ClientID %>').control;
        frm._doMoveToCenter();
        frm.Open();
    }
    function HideSearching(sender, args) {
        var frm = $get('<%=frmLoading.ClientID %>').control;
        frm.Close();
    }    
</script>
</asp:PlaceHolder>

<CC:PopupForm ID="frmLoading" runat="server" CssClass="pform" Animate="false" ShowVeil="true" VeilCloses="false">
    <FormTemplate>
        <div style="background-color:#fff;width:200px;height:80px;text-align:center;padding:30px 10px;">
            <img src="/images/loading.gif" alt="Processing..." /><br /><br />
            <h1 class="largest">Processing... Please Wait</h1>
        </div>
    </FormTemplate>
</CC:PopupForm>

<CC:PopupForm ID="frmExport" runat="server" ShowVeil="true" VeilCloses="true" CssClass="pform" OpenMode="MoveToCenter" width="300px" CloseTriggerId="btnCloseExport">
    <FormTemplate>
        <div class="pckggraywrpr" style="margin:0px;">
            <div class="pckghdgred">Price Comparison Export</div>
            <p class="center bold" style="margin:10px;">
                <a id="lnkExportFile" runat="server">Click here to download export file.</a><br /><br />
                <asp:Button id="btnCloseExport" runat="server" cssclass="btnred" text="Close" />
            </p>
        </div>
    </FormTemplate>
    <Buttons>
        <CC:PopupFormButton ControlID="btnCloseExport" ButtonType="ScriptOnly" />
    </Buttons>
</CC:PopupForm>

<asp:UpdatePanel id="upForm" runat="server" UpdateMode="Conditional">
<ContentTemplate>
<CC:PopupForm ID="frmSubstitute" runat="server" showVeil="true" ValidateCallback="true" ErrorPlaceholderId="spanErrors" VeilCloses="true" CssClass="pform" Animate="true" OpenMode="MoveToCenter">
    <FormTemplate>
        <div class="pckggraywrpr" style="margin-bottom:0px;">
        <div class="pckghdgred" style="height:15px;">Substitute Product Options</div>
        <table cellpadding="2" cellspacing="0" border="0">
            <tr>
                <td style="padding:0px;margin:0px;" colspan="2"><span id="spanErrors" runat="server"><img src="/images/spacer.gif" alt="" style="height:1px;" /></span></td>
            </tr>
            <tr>
                <th>Product:</th>
                <td><span id="spanProduct" runat="server"></span></td>
            </tr>
            <tr>
                <th>Substitute SKU:</th>
                <td><span id="spanSubSku" runat="server"></span></td>
            </tr>
            <tr>
                <th>Unit Price:</th>
                <td><span id="spanSubPrice" runat="server"></span></td>
            </tr>
            <tr>
                <th>Quantity:</th>
                <td><asp:TextBox id="txtSubQuantity" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td colspan="2" style="padding-top:0px;" class="smallest">Default is Vendor Recommendation</td>
            </tr>
            <tr>
                <th colspan="2">
                    <CC:OneClickButton id="btnAccept" runat="server" cssclass="btnred" text="Accept" commandname="Accept" />
                    <CC:OneClickButton id="btnReject" runat="server" cssclass="btnred" text="Reject" commandname="Reject" />
                    <asp:HiddenField id="hdnSubTakeoffProductID" runat="server"></asp:HiddenField>
                    <asp:HiddenField id="hdnSubVendorID" runat="server"></asp:HiddenField>
                </th>
            </tr>
        </table>
        <CC:IntegerValidatorFront ID="ivSubQuantity" runat="server" ControlToValidate="txtSubQuantity" ErrorMessage="Quantity is invalid"></CC:IntegerValidatorFront>
        </div>
    </FormTemplate>
    <Buttons>
        <CC:PopupFormButton ControlId="btnAccept" ButtonType="Postback" />
        <CC:PopupFormButton ControlID="btnReject" ButtonType="Postback" />
    </Buttons>
</CC:PopupForm>
</ContentTemplate>
</asp:UpdatePanel>
<CC:PopupForm ID="frmAverage" runat="server" CloseTriggerId="btnCancelAverage" ShowVeil='true' ValidateCallback="true" ErrorPlaceholderId="spanAverageErrors" VeilCloses="false" cssclass="pform" style="text-align:center;">
    <FormTemplate>
        <div class="pckggraywrpr" style="width:450px;margin-bottom:0px;">
            <div class="pckghdgred">Pricing Options</div>
            <table cellpadding="4" cellspacing="0" border="0" style="margin:15px;background-color:Transparent;">
                <tr>
                    <td style="padding:0px;margin:0px;" colspan="2"><span id="spanAverageErrors" runat="server"></span></td>
                </tr>        
                <tr>
                    <td colspan="2" class="largest bold">
                        Vendor has not priced this item.  What do you want to do?
                    </td>
                </tr>
                <tr>
                    <td><b>Product:</b></td>
                    <td><span id="spanAverageProduct" runat="server"></span></td>
                </tr>
                <tr>
                    <td><b>Average Price:</b></td>
                    <td>
                        <span id="spanAveragePrice" runat="server"></span><br />
                        <span class="smallest">Average price from all vendors in this LLC</span>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align:center;">
                        <asp:Button id="btnAcceptAverage" runat="server" cssclass="btnred" commandname="Accept" text="Keep in order and accept unknown price" /><br /><br />
                        <asp:Button id="btnOmitAverage" runat="server" cssclass="btnred" commandname="Omit" text="Remove from order for this vendor only" /><br /><br />
                        <asp:Button id="btnRequestAverage" runat="server" cssclass="btnred" commandname="Request" text="Request pricing from this vendor" /><br /><br />
                        <asp:Button id="btnCancelAverage" runat="server" cssclass="btnred" text="Cancel" /><br /><br />
                        <asp:HiddenField id="hdnAvgTakeoffProductID" runat="server"></asp:HiddenField>
                        <asp:HiddenField id="hdnAvgVendorID" runat="server"></asp:HiddenField>
                    </td>
                </tr>            
            </table>
        </div>
    </FormTemplate>
    <Buttons>
        <CC:PopupFormButton ControlId="btnAcceptAverage" ButtonType="Postback" />
        <CC:PopupFormButton ControlId="btnOmitAverage" ButtonType="Postback" />
        <CC:PopupFormButton ControlId="btnRequestAverage" ButtonType="Postback" />
        <CC:PopupFormButton ControlId="btnCancelAverage" ButtonType="ScriptOnly" />
    </Buttons>
</CC:PopupForm>

    <div class="slctwrpr">
        <div class="pckghdgred nobdr">Select Your Price Comparison </div>
        <table class="slctcomr" cellpadding="0" cellspacing="0" border="0">
            <tr>
                <td class="bold" colspan="3" align="center">
                    <div style="width:40%;margin:0px auto;position:relative;background-color:#fff;border:1px solid #666;">
                        <b class="smallest">Comparing Prices For:</b><br />
                        <h1 style="font-size:16px;"><asp:Literal id="ltlTakeoffTitle" runat="server"></asp:Literal></h1>
                        <asp:UpdatePanel id="upCheckboxes" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                            <ContentTemplate>
                                <div style="text-align:center;">
                                    <asp:Checkbox id="cbDashboard" runat="server" text="Show On Dashboard" autopostback="true"></asp:Checkbox><br />
                                    <asp:CheckBox id="cbIsAdminComparison" runat="server" text="Eagle One Comparison" autopostback="true"></asp:CheckBox>
                                </div>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostbackTrigger ControlID="cbDashboard"></asp:AsyncPostbackTrigger>
                                <asp:asyncpostbacktrigger controlid="cbIsAdminComparison"></asp:asyncpostbacktrigger>
                            </Triggers>
                        </asp:UpdatePanel>
                        <div class="clear">&nbsp;</div>
                        <p align="center">
                            <asp:HyperLink id="lnkTakeoff" runat="server" cssclass="btnblue" text="Edit Takeoff" NavigateUrl="/takeoffs/edit.aspx"></asp:HyperLink>
                        </p>
                        <a id="lnkDetails" onclick="ToggleDetails()" style="cursor:pointer;">Show Details</a>
                        <div id="divTakeoff" style="display:none;">
                            <table id="tblTakeoff" cellpadding="2" cellspacing="2" border="0" style="text-align:left;">
                                <tr>
                                    <td><b>Project: </b></td>
                                    <td><asp:Literal id="ltlTakeoffProject" runat="server"></asp:Literal></td>
                                </tr>       
                                <tr>
                                    <td><b>Last Updated: </b></td>
                                    <td><asp:Literal id="ltlTakeoffUpdate" runat="server"></asp:Literal></td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                <td class="bold">Select Vendors</td>
                <td class="bold">Table Legend</td>
                <td class="bold">&nbsp;</td>
            </tr>
            <tr>
                <td width="40%">
                    <CC:ListSelect ID="lsVendors" runat="server" SelectLimit="5" Height="200px" AddImageUrl="/images/admin/true.gif" DeleteImageUrl="/images/admin/delete.gif" AutoPostback="true" />
                    <asp:Literal id="ltlNoVendors" runat="server"></asp:Literal>
                </td>
                <td>
                    <table class="complgndtbl" cellSpacing="1" cellPadding="0" border="0">
                        <tr>
                            <td class="desc">Lowest Price</td>
                            <td style="padding: 0px;"><div class="clgnd1" style="background:#ebebeb; padding: 2px;"><img src="/images/global/icon-clgnd1.gif" border="0" alt="" /></div></td>
                        </tr>
                        <tr>
                            <td class="desc">Highest Price</td>
                            <td style="padding: 0px;"><div class="clgnd2" style="background:#ebebeb; padding: 2px;"><img src="/images/global/icon-clgnd2.gif" border="0" alt="" /></div></td>
                        </tr>
                        <tr>
                            <td class="desc">Product is Vendor's Substitution</td>
                            <td style="padding: 0px;"><div class="clgnd3" style="background:#ebebeb; padding: 2px;"><img src="/images/global/icon-clgnd3.gif" border="0" alt="" /></div></td>
                        </tr>
                        <tr>
                            <td class="desc">Substitution Accepted</td>
                            <td style="padding: 0px;"><div class="clgnd4" style="background:#ebebeb; padding: 2px;"><img src="/images/global/icon-clgnd4.gif" border="0" alt="" /></div></td>
                        </tr>
                        <tr>
                            <td class="desc">Substitution Rejected: Omit </td>
                            <td style="padding: 0px;"><div class="clgnd5" style="background:#ebebeb; padding: 2px;"><img src="/images/global/icon-clgnd5.gif" border="0" alt="" /></div></td>
                        </tr>
                    </table>
                </td>
                <td>
                    <table class="complgndtbl" cellSpacing="1" cellPadding="0" border="0">
                        <tr>
                            <td class="desc">No Pricing from Vendor</td>
                            <td style="padding: 0px;"><div class="clgnd6" style="background:#ebebeb; padding: 2px;"><img src="/images/global/icon-clgnd6.gif" border="0" alt="" /></div></td>
                        </tr>
                        <tr>
                            <td class="desc">No Pricing: Request Pending</td>
                            <td style="padding: 0px;"><div class="clgnd7" style="background:#ebebeb; padding: 2px;"><img src="/images/global/icon-clgnd7.gif" border="0" alt="" /></div></td>
                        </tr>
                        <tr>
                            <td class="desc">No Pricing: Accept without Price</td>
                            <td style="padding: 0px;"><div class="clgnd8" style="background:#ebebeb; padding: 2px;"><img src="/images/global/icon-clgnd8.gif" border="0" alt="" /></div></td>
                        </tr>
                        <tr>
                            <td class="desc">No Pricing: Omit</td>
                            <td style="padding: 0px;"><div class="clgnd9" style="background:#000000; padding: 2px;"><img src="/images/global/icon-clgnd9.gif" border="0" alt="" /></div></td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>    
    <div class="pckggraywrpr">
        <div class="pckghdgred nobdr">Compare Prices </div>
        <table id="tblPriced" runat="server" cellspacing="1" cellpadding="0" border="0" class="tblcompr">        
            <tr><td class="shim" style="width:44px;"><img src="/images/spacer.gif" style="width:44px; height:1px;" alt="" /><br /></td><td class="shim" style="width:89px;"><img src="/images/spacer.gif" style="width:89px; height:1px;" alt="" /><br /></td> <td class="shim" style="width:250px;"><img src="/images/spacer.gif" style="width:250px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td> <td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td></tr>            
            <tr>
                <th colspan="8">Products All Vendors Priced</th>
            </tr>
            <tr id="trPricedHeader" runat="server">
				<th>Qty</th>
				<th>Sku</th>
				<th>Product Name</th>
			</tr>
        </table>
        
        <asp:UpdatePanel ID="upSub" runat="server" UpdateMode="Conditional">
			<ContentTemplate>
				
			
        <table id="tblSub" runat="server" cellspacing="1" cellpadding="0" border="0" class="tblcompr comprsub">
		    <tr><td class="shim" style="width:44px;"><img src="/images/spacer.gif" style="width:44px; height:1px;" alt="" /><br /></td><td class="shim" style="width:89px;"><img src="/images/spacer.gif" style="width:89px; height:1px;" alt="" /><br /></td> <td class="shim" style="width:250px;"><img src="/images/spacer.gif" style="width:250px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td> <td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td></tr>            
		    <tr>
				<th colspan="8">Products Priced with Some Vendors Providing Substitutions</th>
			</tr>
            <tr id="trSubHeader" runat="server">
				<th>Qty</th>
				<th>Sku</th>
				<th>Product Name</th>
			</tr>
        </table>
        </ContentTemplate>
        <Triggers>
			<asp:AsyncPostBackTrigger ControlId="frmSubstitute" />
        	<%--<asp:AsyncPostBackTrigger ControlId="btnReject" />--%>
        </Triggers>
        </asp:UpdatePanel>
        
        <table id="tblAvg" runat="server" cellspacing="1" cellpadding="0" border="0" class="tblcompr compravg">
            <tr><td class="shim" style="width:44px;"><img src="/images/spacer.gif" style="width:44px; height:1px;" alt="" /><br /></td><td class="shim" style="width:89px;"><img src="/images/spacer.gif" style="width:89px; height:1px;" alt="" /><br /></td> <td class="shim" style="width:250px;"><img src="/images/spacer.gif" style="width:250px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td> <td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td></tr>            
		    <tr>
				<th colspan="8">Products Some Vendors Did Not Price</th>
            </tr>
            <tr id="trAvgHeader" runat="server">
				<th>Qty</th>
				<th>Sku</th>
				<th>Product Name</th>
			</tr>
                    
        </table>
        
        <table id="tblSpec" runat="server" cellspacing="1" cellpadding="0" border="0" class="tblcompr">
            <tr><td class="shim" style="width:44px;"><img src="/images/spacer.gif" style="width:44px; height:1px;" alt="" /><br /></td><td class="shim" style="width:89px;"><img src="/images/spacer.gif" style="width:89px; height:1px;" alt="" /><br /></td> <td class="shim" style="width:250px;"><img src="/images/spacer.gif" style="width:250px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td> <td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td></tr>            
		    <tr>
				<th colspan="8">Special Order Products</th>
            </tr>
            <tr id="trSpecHeader" runat="server">
				<th>Qty</th>
				<th>Sku</th>
				<th>Product Name</th>
			</tr>
        </table>

        <table id="tblSum" runat="server" cellspacing="1" cellpadding="0" border="0" class="tblcompr comprsum">        
			<tr>
				<td class="shim" style="width:383px;"><img src="/images/spacer.gif" style="width:383px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td>
			</tr>
            <tr id="trSumHeader" runat="server">
				<th>Summary</th>
            </tr>
        </table>
    </div>
    
    <CC:UnvalidatedPostback ID="btnOrder" runat="server"></CC:UnvalidatedPostback>
    <CC:UnvalidatedPostback ID="btnRequestPricing" runat="server"></CC:UnvalidatedPostback>
    
    <asp:Panel ID="pnlPrint" runat="server">
        <div class="pckggraywrpr" style="text-align: center;">
            <input type="button" class="btnred" value="Print This Page" onclick="window.open('<%=PrintUrl%>', 'PrintPage', ''); return false;" />
            <asp:Button id="btnExport" runat="server" cssclass="btnred" text="Export Comparison" />
        </div>    
    </asp:Panel>
</CT:MasterPage>