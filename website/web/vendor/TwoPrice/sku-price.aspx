<%@ Page Language="VB" AutoEventWireup="false" CodeFile="sku-price.aspx.vb" Inherits="twoskuprice"
    EnableEventValidation="false" %>

<%--<%@ Register TagName="Search" TagPrefix="CC" Src="~/controls/SearchSql.ascx" %>--%>
<CT:MasterPage ID="CTMain" runat="server"><asp:panel id="pnMain" runat="server"><asp:PlaceHolder runat="server"><script type="text/javascript">
//    });                                                                                                                    //});
function OpenSkuPrice() {
    var b = Sys.UI.Behavior.getBehaviorByName($get('<%=ctrlSkuPrice.BehaviorId %>'), '<%=ctrlSkuPrice.BehaviorName %>');
    b.moveToCenter();
    b.startFadeIn();
    return false;
}
function OpenPrice() {
    var b = Sys.UI.Behavior.getBehaviorByName($get('<%=ctrlPrice.BehaviorId %>'), '<%=ctrlPrice.BehaviorName %>');
    b.moveToCenter();
    b.startFadeIn();
    return false;
}



function OpenSubmitForm() {
    var b = Sys.UI.Behavior.getBehaviorByName($get('<%=ctrlSpecial.BehaviorId %>'), '<%=ctrlSpecial.BehaviorName %>');
    b.moveToCenter();
    b.startFadeIn();
    if (document.getElementById('hdnAllowPriceUpdate').value == 0)
        document.getElementById('lblSendMessageHeader').innerHTML = 'Send Message to CBUSA and Submit';
    else
        document.getElementById('lblSendMessageHeader').innerHTML = 'Send Message to Builder For Mid Event Price Update';
    return false;
}
function ViewPriceHistoryPopUp() {
    var b = Sys.UI.Behavior.getBehaviorByName($get('<%=ctrlPriceHistory.BehaviorId %>'), '<%=ctrlPriceHistory.BehaviorName %>');
    b.moveToCenter();
    b.startFadeIn();
    b.width = '800';
    return false;
}
</script>
</asp:PlaceHolder>

<span id="trace"></span>

<nStuff:UpdateHistory ID="ctlHistory" runat="server"></nStuff:UpdateHistory>
<asp:HiddenField ID="hdnSearch" runat="server" value=" "></asp:HiddenField>
<div><div class="builderList"><table width="100%"><tr><th class="bggray" style="vertical-align:top"><div class="maincolwrpr"><div class="pckghdgred">Campaign Information </div><asp:Literal ID="ltlTwoPriceInfo" runat="server" /><asp:Repeater ID="rptParticipatingBuilders" runat="server"><HeaderTemplate><h3 style="margin:0">Participating builders:</h3></HeaderTemplate><ItemTemplate><div><%# Container.DataItem.Item("CompanyName")%>
                </div>
            </ItemTemplate>
        </asp:Repeater>
            
            </div>
</th>
                <th class="bggray" style="vertical-align:top"><div class="pckghdgred">Documents </div><asp:Repeater ID="rptDocuments" runat="server"><HeaderTemplate></HeaderTemplate>
                    <ItemTemplate><table border="0" cellpadding="0" cellspacing="0" class="dshbrdtbl" style="font-weight: normal;"><tr><th>Document</th><th>Uploaded</th></tr><tr class='<%#IIf(Container.ItemIndex Mod 2 = 1, "alternate", "row") %>'><td id="tdMessageRow" runat="server" class="dcrow"><a id="lnkMessageTitle" runat="server" class="dctitlelink" href="/" target="_blank"><%#DataBinder.Eval(Container.DataItem, "Title")%></a>&#160; </td><td><%#FormatDateTime(DataBinder.Eval(Container.DataItem, "Uploaded"), DateFormat.ShortDate)%></td>
                                         
                                    </tr></table>
                    
                    
                                </ItemTemplate>
                    
                                <FooterTemplate></FooterTemplate>
                                </asp:Repeater>	
                            <asp:Literal ID="ltlNoDocs" runat="server" text="No documents to display" visible="false" /></th>

            </tr>
            </table>
               
                                       
    </div>
</div>
    
    <table border="0" cellpadding="0" cellspacing="0" class="tblnwto" style="width:auto"><tr valign="top"><td><div class="maincolwrpr"><div class="pckghdgred" style="height:20px;">Bid Sheet </div><div style="float:right; margin-top:-32px; margin-right:5px;"></div>
            
            <div style="float:right; margin-top:-32px; margin-right:5px;"><asp:Button ID="btnPrice" runat="server" cssclass="btnred" onclientclick="return OpenPrice();" text="Export / Import Bid Pricing" /><asp:Button ID="btnDecline" runat="server" cssclass="btnred" text="Decline to Bid" /></div>

            <%If BidSubmitted Then%>
                <div style="font-size: 17px;padding:5px">Your bid was last submitted on <asp:Literal ID="ltlDate" runat="server" />. <br /><asp:literal ID="ltlSubmittedName" runat="server"></asp:literal></div><% End If %><asp:UpdatePanel ID="upProducts" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional"><ContentTemplate><div class="bcrmwrpr"><asp:Literal ID="ltlBreadcrumb" runat="server" enableviewstate="false"></asp:Literal></div><asp:PlaceHolder ID="txtNoResults" runat="server" Visible="False"><div style="margin-top:10px; margin-bottom:10px; margin-left:5px;"><b>No products found matching your search criteria. Please select a new phase of supply or enter a new keyword.</b> </div></asp:PlaceHolder><div style="margin-top:10px; margin-bottom:10px; margin-left:5px;"><asp:Literal ID="ltrErrorMsg" runat="server"></asp:Literal><div style="text-align: center;"><CC:OneClickButton ID="btnImportUpdate" runat="server" CssClass="btnred" Text="Continue New Products and Prices Import" visible="false" /><CC:OneClickButton ID="btnImportPriceUpdate" runat="server" CssClass="btnred" Text="Continue Bid Pricing Import" visible="false"   /></div> 
                </div>
                            <div style="margin-left:5px;"><table style="width:1030px"><tr><td style="width: 2%;"><img border="0" src="/images/admin/true.gif" /></td>
                                    <td><span class="small">Updated successfully</span></td><td style="width:50%;text-align:right;"><asp:Button ID="btnSubmitBidTop" runat="server" cssclass="btnred" text="Submit Bid" /><asp:Button ID="btnExport" runat="server" cssclass="btnred" onclientclick="ShowLoader()" text="Export Bid Price List" Visible="false" />&nbsp;&nbsp;<asp:Button ID="btnViewPriceList" runat="server" cssclass="btnred" onclientclick="ViewPriceHistoryPopUp()" text="View Price History List" Visible="false" /><%--<img src="/images/ajaxloading5.gif" alt="Loading..." id="MultiAssetLoadingBid" style="display: none;" runat="server"/>--%><img id="MultiAssetLoadingBid" runat="server" alt="Loading..." src="/images/ajaxloading5.gif" style="display: none;" /><script>
                                            function ShowLoader() {
                                                //alert('show');
                                                $("#MultiAssetLoadingBid").show();
                                                //setTimeout(
                                                //    function () {
                                                //       $("#MultiAssetLoadingBid").hide();
                                                //    }, 6000);                                        
                                            }
                                            function HideLoader() {
                                                //alert('hide');
                                                $("#MultiAssetLoadingBid").hide();
                                            }
                                        </script></td></tr><tr><td></td>
                                    <td style="color:green;font-size:20px"><asp:Literal ID="ltlStatus" runat="server" /></td>
                                </tr>
                                
                                </table>

                            </div><table border="0" cellpadding="0" cellspacing="0" class="tbltodata" style="border-bottom: 0px;"><tr><th>&#160;</th><th style="text-align:center;">CBUSA SKU</th><th style="text-align:center;">Product Name</th><th style="text-align:center;">Vendor SKU</th><th style="text-align:center;">Current Price in Software</th><th style="text-align:center;">Quantity</th><th style="text-align:center;">Bid Price</th><th>Comments</th><th style="text-align:center;">Total</th><th style="text-align:right;"><asp:Button ID="btnUpdateAll" runat="server" cssclass="btnred" text="Update All" /><asp:Button ID="btnSameAll" runat="server" cssclass="btnred" text="Same Price For All" /></th>
                            </tr>
                        
                        <asp:Repeater ID="rptProducts" runat="server"><headertemplate></headertemplate>
                                <ItemTemplate><tr id="trVendor" runat="server" class='<%#IIf(Container.ItemIndex Mod 2 = 1, "", "alt") %>' itemindex="<%#Container.ItemIndex%>" style="border-bottom:0px"><td style="text-align:center;"><asp:Image ID="imgReq" runat="server" visible="false"></asp:Image></td>
                                        
                                        <td style="text-align:center;"><%#Container.DataItem.Sku%></td>                                
                                        <td nowrap><%#Container.DataItem.Product%></td>
                                        <td style="text-align:center;">
                                            <asp:Label ID="lblSku" runat="server" text="<%#Container.DataItem.VendorSku %>"></asp:Label>
                                        </td>
                                        <td style="text-align:center;">
                                            <asp:Label ID="lblCurrentPrice" runat="server" text="<%#Container.DataItem.VendorPrice%>"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblQuantity" runat="server" text="<%#Container.DataItem.Quantity%>"></asp:Label>
                                        </td>
                                        <%--<td>
                                            <asp:Literal ID="ltlDiscontinued" runat="server"></asp:Literal>
                                        </td>--%>
                                        <td style="text-align:center;">
                                            <asp:TextBox ID="txtNextPrice" runat="server" class="inptqty" columns="6" maxlength="12" text="<%#IIf(IsDBNull(Container.DataItem.NextPrice), Container.DataItem.VendorPrice, Container.DataItem.NextPrice) %>"></asp:TextBox>
                                            <asp:HiddenField ID="hdnPreviousPrice" runat="Server" value="<%#Container.DataItem.LastPrice%>"></asp:HiddenField>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtComments" runat="server" maxlength="100" text="<%#Container.DataItem.Comments%>" width="200"></asp:TextBox>
                                        </td>
                                        <td style="text-align:center;">
                                            <asp:Label ID="lblTotal" runat="server" text="<%#FormatCurrency(Container.DataItem.Quantity * Container.DataItem.NextPrice)%>"></asp:Label>
                                        </td>
                                        <td align="right" nowrap>
                                            <asp:Button ID="btnUpdate" runat="server" CommandArgument="<%#Container.DataItem.ProductID %>" cssclass="btnred" text="Update" /><asp:Button ID="btnSyncPrice" runat="server" CommandArgument="<%#Container.DataItem.ProductID %>" cssclass="btnred" text="Same Price"></asp:Button>
                                        </td>
                                    </tr>                                    
                                </ItemTemplate>
                                <footertemplate></footertemplate>
                            </asp:Repeater>
                                <tr><td></td>
                                        <td style="text-align:center">Total:</td><td colspan="3"></td>
                                        <td><asp:Literal ID="ltlTotalQuantity" runat="server" /></td>
                                        <td colspan="2"></td>
                                        <td><asp:Literal ID="ltlTotalPrice" runat="server" /></td>
                                        <td></td>
                                    </tr>
                               <tr><td colspan="9"></td>
                                   <td style="text-align:right"><asp:Button ID="btnUpdateAllBottom" runat="server" cssclass="btnred" text="Update All" /><asp:Button ID="btnSameAllBottom" runat="server" cssclass="btnred" text="Same Price For All" /></td>
                               </tr>

                          <%-- Added Medullus--%>
                        <tr style="display:none;"><%--<td colspan="2">&#160;</td><td></td>
	                          <td></td>
	                          <td></td>
	                         <td></td>--%>
	                        <td ><asp:Button ID="btnExport2" runat="server" cssclass="btnred" text="Export Bid Price List" /></td>
   
                        </tr>
                        <%-- Added Medullus--%>
                            </table>
                </ContentTemplate>
                    <Triggers></Triggers>
                </asp:UpdatePanel>
            <asp:Button ID="btnSubmit"  runat="server" cssclass="btnred"  text="Submit Bid" /></div>
        </td>     
    </tr>
</table>

<div id="divViewPricingHistory" runat="server" class="window PriceHistory increaseSize" style="visibility:hidden;background-color: #e1e1e1;border: 1px solid #c2c2c2;width:550px;"><div class="pckghdgred">View Pricing History </div><div  class="divPriceHistory priceHistoryDesign"><asp:TextBox ID="txtProductName" runat="server" Width="319px" style="margin-right:29px; margin-top:1px; border-radius: 5px; padding: 1px 5px; border: 1px solid rgb(194, 194, 194);" Font-Size="12px" /><asp:Button runat="server" Text="Search By Product" ID="btnSerachByProduct" onclientclick="Search_Gridview('grdPriceHistory'); return false;"></asp:Button><asp:Button runat="server" Text="Clear" ID="btnClear" onclientclick="ClearSearch('grdPriceHistory'); return false;"></asp:Button>
    <asp:button ID="btnExportToExcel" runat="server" Text="Export" /><div class="table-wrapper"><div class="gridPriceHistoryHeader"><table class="tbltodata" rules="all" ><tr><th>&nbsp;</th><th>Product</th><th>New Price</th></tr></table></div><div class="gridPriceHistoryBody"><asp:GridView ID="grdPriceHistory" runat="server" AutoGenerateColumns="false" CssClass="gridPriceHistory" ShowHeader = "false"
    DataKeyNames="ProductID" OnRowDataBound="OnRowDataBound"><Columns><asp:TemplateField><ItemTemplate><asp:Image ID="imgPlus" runat="server" alt="" style="cursor: pointer" /><asp:Panel ID="pnlOldPrice" runat="server" Style="display: none"><asp:GridView ID="gvOldPrice" runat="server" AutoGenerateColumns="False" CssClass="gridPriceHistory"><Columns><asp:BoundField DataField="OldPrice" HeaderText="Old Price" ItemStyle-Width="150px"><HeaderStyle Wrap="False" /><ItemStyle Width="150px" />

                        </asp:BoundField><asp:BoundField DataField="NewPrice" HeaderText="New Price" ItemStyle-Width="150px"><ItemStyle Width="150px" Wrap="False" /></asp:BoundField>
                        <asp:BoundField DataField="UpdatedOn" HeaderText="Updated On" SortExpression="UpdatedOn"><HeaderStyle Wrap="False" /><ItemStyle Wrap="False" />

                        </asp:BoundField>
                            </Columns>
                    </asp:GridView>
                </asp:Panel>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="Product" HeaderText="Item Name" SortExpression="Product"></asp:BoundField>
       <asp:BoundField DataField="NewPrice" HeaderText="New Price" SortExpression="NewPrice"></asp:BoundField>
        <%-- <asp:BoundField DataField="UpdatedOn" HeaderText="Updated On" SortExpression="UpdatedOn"></asp:BoundField>--%>
    </Columns>
</asp:GridView>
        </div>

        </div>

        </div>
    <div class="table-footer"><asp:Button id="btnCancelHistory" runat="server" style="margin-top: 10px;" cssclass="btnred" Text="Cancel" CausesValidation="false" /></div>
    <div class="background-overlay"></div>
</div>

<asp:HiddenField ID="hdnAllowPriceUpdate" runat="server" />
<div id="divSpecialOrderForm" runat="server" class="window" style="visibility:hidden;background-color: #e1e1e1;border: 1px solid #c2c2c2;width:550px;"><div class="pckghdgred"><asp:Label runat="server" id="lblSendMessageHeader" Text=""></asp:Label></div><table cellpadding="2" cellspacing="0" border="0" style="margin:5px;"><tr valign="top"><td class="fieldlbl bold"><span id="labeltxtDescription" runat="server">Message:</span></td><td class="fieldreq" id="bartxtDescription" runat="server">&nbsp;</td><td class="field"><asp:TextBox id="txtDescription" runat="server" columns="50" maxlength="2000" TextMode="Multiline" rows="7" style="width:400px;"></asp:TextBox></td></tr></table><p style="text-align:center;padding:10px;"><CC:ConfirmButton ID="btnSendMessage" runat="server" CssClass="btnred" Text="Submit" Message="Are you sure you want to submit Bid?"/>&nbsp; <%--<asp:Button id="btnCancelSpecial" runat="server" cssclass="btnred" Text="Cancel" CausesValidation="false" />--%></p></div><CC:DivWindow ID="ctrlPriceHistory" runat="server" TargetControlID="divViewPricingHistory" CloseTriggerId="btnCancelHistory" ShowVeil="true" VeilCloses="false" /><CC:DivWindow ID="ctrlSpecial" runat="server" CloseTriggerId="btnCancelSpecial" ShowVeil="true" TargetControlID="divSpecialOrderForm" VeilCloses="false" /><asp:HiddenField ID="hdnUnsaved" runat="server" /><div id="divSkuPrice" runat="server" class="window" style="visibility:hidden;background-color:#e1e1e1;border:1px solid #c2c2c2;width:350px;"></div>
<CC:DivWindow ID="ctrlSkuPrice" runat="server" TargetControlID="divSkuPrice" CloseTriggerId="btnCancelSkuPrice" ShowVeil="true" VeilCloses="false" /><div id="divPrice" runat="server" class="window" style="visibility:hidden;background-color:#e1e1e1;border:1px solid #c2c2c2;width:350px;"><div class="pckghdgred">Quick Price Update</div><div style="margin-left:10px;margin-right:10px"><br /><p>Use this form if you have already matched your company skus with the product catalog to quickly update pricing.</p><p><asp:LinkButton ID="btnPriceExport" runat="server" cssclass="bold" OnClick="ExportPriceCSV" onclientclick="SwapTarget();" Text="Get Pricing File" /><span class="smallest">(Import file must match the template exactly including the header line)</span></p><center><CC:FileUpload ID="fulPrice" runat="server" /><br /><br /><CC:OneClickButton ID="btnImportPrice" runat="server" cssclass="btnred" Text="Import CSV" /><asp:Button ID="btnCancelPrice" runat="server" CausesValidation="false" cssclass="btnred" Text="Cancel" /><br /></center>
    </div>
</div>
<CC:DivWindow ID="ctrlPrice" runat="server" CloseTriggerId="btnCancelPrice" ShowVeil="true" TargetControlID="divPrice" VeilCloses="false" /><div></div></asp:panel>

<asp:Panel ID="pnlSendMessage" runat="server" Visible="false"><h2>Send Message to CBUSA</h2><div class="field">Message: <br /><asp:Textbox ID="txtMessage" runat="server" Columns="70" Rows="10" MaxLength="255" TextMode="MultiLine"></asp:Textbox></div></asp:Panel><asp:Panel ID="pnComplete" runat="server" Visible="false"><div>Thank you, your bid has been submitted.. </div></asp:Panel></CT:MasterPage>

<style type="text/css">
.gridPriceHistory th {
        color: #FFF !important;
    }
6
    .gridPriceHistory td {
        padding: 4px;
    }

    .PriceHistory {
        float: left;
        width: 100%;
        text-align: center;
        padding: 15px 0px;
    }

    .PriceHistory {
        padding-top: 0px;
        padding-bottom: 0px;
    }

    .gridPriceHistory {
        width: 100%;
        margin: 0;
    }

    .divPriceHistory {
        max-height: 400px;
        overflow-y: auto;
        overflow-x: visible;
    }

        .divPriceHistory.priceHistoryDesign {
            padding-top: 10px;
        }

            .divPriceHistory.priceHistoryDesign input[type="text"] {
                float: left;
                margin: 0px 56px 0px 28px;
                height: 18px;
            }

            .divPriceHistory.priceHistoryDesign input[type="submit"] {
                float: left;
                width: auto;
                overflow: visible;
                color: #656263;
                font-size: 10px;
                font-family: montserrat;
                text-transform: uppercase;
                border-style: none;
                padding: 3px 10px;
                border-radius: 5px;
                border: 1px solid #656263;
                cursor: pointer;
                font-weight: bold;
                background: #fff;
                margin: 2px 0px 7px 10px;
            }

            .divPriceHistory.priceHistoryDesign table, td {
                font-size: 11px;
                vertical-align: middle;
            }

    .table-wrapper {
        padding: 0px 28px;
    }

    .gridPriceHistoryBody {
        height: 200px;
        overflow-y: auto;
    }

        .gridPriceHistoryBody .gridPriceHistory td:first-child {
            /*width: 23px;*/
            width: 32px;
            text-align: center !important;
            padding: 0 !important;
        }

        .gridPriceHistoryBody .gridPriceHistory td:nth-child(2) {
            width: 500px;
        }

    .divPriceHistory.priceHistoryDesign .gridPriceHistory td div {
        margin-right: 16px;
    }

    .divPriceHistory.priceHistoryDesign .gridPriceHistory td .gridPriceHistory tr th {
        padding: 5px 15px;
    }

    .divPriceHistory.priceHistoryDesign .gridPriceHistory td {
        padding: 4px 0px 4px 16px;
        text-align: left;
    }

    .divPriceHistory.priceHistoryDesign .gridPriceHistoryHeader th {
        padding: 8px 0px 8px 16px;
        text-align: left;
    }

        .divPriceHistory.priceHistoryDesign .gridPriceHistoryHeader th:first-child {
            width: 33px;
            text-align: center;
            padding: 0;
        }

        .divPriceHistory.priceHistoryDesign .gridPriceHistoryHeader th:nth-child(2) {
            /*width: 443px;*/
            width: 500px;
        }

    .background-overlay {
        position: fixed;
        background-color: rgba(0,0,0,0.6);
        width: 100%;
        height: 100%;
        left: 0px;
        top: 0px;
        z-index: -1;
    }

    .PriceHistory .divPriceHistory.priceHistoryDesign {
        background-color: #e3e1e3;
        position: relative;
        z-index: 5;
    }

    .PriceHistory .table-footer {
        background-color: #e3e1e3;
        position: relative;
        z-index: 5;
        padding-bottom: 15px;
    }

    .HeaderFreez {
        font-weight: bold;
        background-color: Green;
        position: relative;
        top: expression(this.parentNode.parentNode.parentNode.scrollTop-1);
    }

    div#divViewPricingHistory.increaseSize {
        width: 700px !important;
        left: 50% !important;
        margin-left: -350px !important;
    }
</style>


<script type="text/javascript">
    $("[src*=plus]").live("click", function () {
        $(this).closest("tr").after("<tr><td></td><td colspan = '999'>" + $(this).next().html() + "</td></tr>")
        $(this).attr("src", "../images/minus.png");
    });
    $("[src*=minus]").live("click", function () {
        $(this).attr("src", "../images/plus.png");
        $(this).closest("tr").next().remove();
    });
</script>



<link rel="stylesheet" href="//code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css">
<link rel="stylesheet" href="/resources/demos/style.css">
<script src="https://code.jquery.com/jquery-1.12.4.js"></script>
<script src="https://code.jquery.com/ui/1.12.1/jquery-ui.js"></script>
<script language="javascript" type="text/javascript">
    $(function () {
        $('#<%=txtProductName.ClientID%>').autocomplete({
    source: function (request, response) {
        $.ajax({
            url: "sku-price.aspx/GetProductName",
            data: "{ 'pre':'" + request.term + "'}",
            dataType: "json",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                response($.map(data.d, function (item) {
                    return { value: item }
                }))
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                //alert(textStatus);
            }
        });
    }
});
});
</script>


<script type="text/javascript">
    function Search_Gridview(strGV) {
         var strData = document.getElementById('txtProductName').value.toLowerCase().split(" ");
        var tblData = document.getElementById(strGV);
        var rowData;
        for (var i = 0; i < tblData.rows.length; i++) {
            rowData = tblData.rows[i].innerHTML;
            var styleDisplay = 'none';
            for (var j = 0; j < strData.length; j++) {
                if (rowData.toLowerCase().indexOf(strData[j]) >= 0){
                    //alert(strData[j]);
                    styleDisplay = '';
                }
                else {
                    //alert('no match found');
                    styleDisplay = 'none';
                    break;
                }
            }
            tblData.rows[i].style.display = styleDisplay;
        }
    }

    function ClearSearch(strGV) {
        document.getElementById('txtProductName').value = '';
        var strData = document.getElementById('txtProductName').value.toLowerCase().split(" ");
        var tblData = document.getElementById(strGV);
        var rowData;
        for (var i = 0; i < tblData.rows.length; i++) {
            rowData = tblData.rows[i].innerHTML;
            var styleDisplay = 'none';
            for (var j = 0; j < strData.length; j++) {
                if (rowData.toLowerCase().indexOf(strData[j]) >= 0)
                    styleDisplay = '';
                else {
                    styleDisplay = 'none';
                    break;
                }
            }
            tblData.rows[i].style.display = styleDisplay;
        }
    }
</script>