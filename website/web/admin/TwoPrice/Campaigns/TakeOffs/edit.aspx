<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" Inherits="TwoPriceTakeOffs_edit" EnableEventValidation="false" MasterPageFile="~/controls/AdminMaster.master" %>
<%@ Register TagName="ProductsToCompareForm" TagPrefix="CC" Src="~/controls/ProductsToCompareForm.ascx" %>
<%@ Register TagName="CurrentTakeOffs" TagPrefix="CC" Src="~/modules/twoprice/currenttakeoffs.ascx" %>
<%@ Register TagName="Search" Src="~/controls/SearchSql.ascx" TagPrefix="CC" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
<style type="text/css">
.corwrpr {
    width: 998px;
    background-color: #EBEBEB;
    padding: 18px 0;
    }
    table.tbltodata td.sortCell {
width: 18px;
height: 20px;
background-image: url('/images/admin/move.gif');
background-repeat: no-repeat;
background-position: center 5px;
cursor: move;
}

    table.fltrbar{
        width:auto !important;

    }
</style>
<link rel="stylesheet" href="/includes/style.css" type="text/css">
<asp:ScriptManager ID="sc" runat="server"></asp:ScriptManager>
<script type="text/javascript" src="/includes/formdnd.js"/></script>
<script type="text/javascript">
    function SwapTarget() {
        document.forms[0].target = '_blank';
        window.setTimeout('document.forms[0].target = "_self";', 1000);
    }
    function OpenUploadProduct() {
        var b = Sys.UI.Behavior.getBehaviorByName($get('<%=ctrlProductUpload.BehaviorId %>'), '<%=ctrlProductUpload.BehaviorName %>');
        b.moveToCenter();
        b.startFadeIn();
        return false;
    }
    function CloseSpecialForm() {
        var b = Sys.UI.Behavior.getBehaviorByName($get('<%=ctrlSpecial.BehaviorId %>'), '<%=ctrlSpecial.BehaviorName %>');
        b.startFadeOut();
    }
    function OpenSpecialForm() {
        var b = Sys.UI.Behavior.getBehaviorByName($get('<%=ctrlSpecial.BehaviorId %>'), '<%=ctrlSpecial.BehaviorName %>');
        b.moveToCenter();
        b.startFadeIn();
        return false;
    }
    function OpenSaveForm(e,isCopy) {
        var b = Sys.UI.Behavior.getBehaviorByName($get('<%=ctrlSaveForm.BehaviorId %>'), '<%=ctrlSaveForm.BehaviorName %>');
        var hdn = $get('<%=hdnIsCopy.ClientID %>');
        var txt = $get('<%=txtTwoPriceTakeOffTitle.ClientID %>');
        var lbl = $get('<%=divHead.ClientID %>');
        if(isCopy) {
            hdn.value = 'true';
            lbl.innerHTML = 'Save New Copy';
        } else {
            hdn.value = 'false';
            if(txt.value == '') {
                lbl.innerHTML = 'Save TwoPriceTakeOff';
            } else {
                lbl.innerHTML = 'Rename TwoPriceTakeOff';
            }
        }
        b.moveToClick(e);
        b.startFadeIn();
        return false;
    }
    function CloseSaveForm() {
        $get('<%=lblSavedMsg.ClientId %>').innerHTML = '';
        var b = Sys.UI.Behavior.getBehaviorByName($get('<%=ctrlSaveForm.BehaviorId %>'), '<%=ctrlSaveForm.BehaviorName %>');
        b.startFadeOut();
        return false;
    }
    function AddProductForm(rptId,productId) {
        $get('divAddForm').style.visibility = '';
        $get('divAddWait').style.display = 'none';
        var form = $get('<%=frmAddProduct.ClientID %>').control;
        if (window.TwoPriceTakeOffProductIds) {
            for (var i = 0; i < TwoPriceTakeOffProductIds.length; i++) {
                if (TwoPriceTakeOffProductIds[i] == productId) {
                    LoadLineItems(productId);
                    form.get_input('hdnRptItemId').value = rptId;
                    return false;
                }
            }
            return true;
        }
        return true;
    }
    function AddContinueClick() {
        $get('divAddForm').style.visibility = 'hidden';
        $get('divAddWait').style.display = '';
        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(CloseAddForm);
    }
    function CloseAddForm() {
        Sys.WebForms.PageRequestManager.getInstance().remove_pageLoaded(CloseAddForm)
        var form = $get('<%=frmAddProduct.ClientID %>').control;
        form.Close();
    }
    function LoadLineItems(id) {
        <%=Page.ClientScript.GetCallbackEventReference(Page,"id","LoadLineItemsCb","null") %>;
    }
    function LoadLineItemsCb(res, ctxt) {
        var form = $get('<%=frmAddProduct.ClientID %>').control;
        var div = $get('divLineItems');
        div.innerHTML = res;
        form._doMoveToCenter();
        form.Open();
    }
    function UpdateWhereClauses(ctlProjects) {
        var TwoPriceTakeOffs = $get('<%=slTwoPriceTakeOffs.ClientID %>').control;
        var orders = $get('<%=slOrders.ClientID %>').control;
        TwoPriceTakeOffs.set_whereClause('ProjectID=' + ctlProjects.get_value());
        TwoPriceTakeOffs._updateList();
        orders.set_whereClause('ProjectID=' + ctlProjects.get_value());
        orders._updateList();
    }
    function UpdateTitle() {
        var title = $get('<%=lblTwoPriceTakeOffTitle.ClientID %>');
        var txt = $get('<%=txtTwoPriceTakeOffTitle.ClientID %>');
        title.innerHTML = txt.value;
    }
    function OpenSendForm() {
        var c = $get('<%=frmSendTwoPriceTakeOff.ClientID %>').control;
        c.Open();
    }
    function ShowSearching(sender,args) {
        var req = args.get_webRequest();
        if(req.get_url().toLowerCase().indexOf('edit.aspx') > 0) {
            var frm = $get('<%=frmLoading.ClientID %>').control;
            frm._doMoveToCenter();
            frm.Open();
        }
    }
    function HideSearching(sender,args) {
        var frm = $get('<%=frmLoading.ClientID %>').control;
        frm.Close();
    }
    function pageLoad() {
     //   Sys.Net.WebRequestManager.add_invokingRequest(ShowSearching);
      //  Sys.Net.WebRequestManager.add_completedRequest(HideSearching);
    }
    function OpenPrice() {
        var b = Sys.UI.Behavior.getBehaviorByName($get('<%=ctrlPrice.BehaviorId %>'), '<%=ctrlPrice.BehaviorName %>');
           b.moveToCenter();
           b.startFadeIn();
           return false;
       }
</script>

<div class="corwrpr">
<span id="trace"></span>

<nStuff:UpdateHistory ID="ctlHistory" runat="server"></nStuff:UpdateHistory>

<CC:PopupForm ID="frmLoading" runat="server" CssClass="pform" Animate="false" ShowVeil="true" VeilCloses="false">
    <FormTemplate>
        <div style="background-color:#fff;width:200px;height:80px;text-align:center;padding:30px 10px;">
            <img src="/images/loading.gif" alt="Processing..." /><br /><br />
            <h1 class="largest">Processing... Please Wait</h1>
        </div>
    </FormTemplate>
</CC:PopupForm>

<CC:DivWindow ID="ctrlPrice" runat="server" CloseTriggerId="btnCancelSkuPrice" ShowVeil="true" TargetControlID="divPrice" VeilCloses="false" />
<div id="divPrice" runat="server" class="window" style="visibility:hidden;border:1px solid #000;background-color:#fff;width:350px;">
    <div class="pckghdgred">Quick Product Import</div>
    <div style="margin-left:10px;margin-right:10px">
        <br />
        <p>Use this form if you already have a list of skus for quick take-off importing.</p>
      <p>Use this form upload products and quantities from a csv file. The product should be identified with its CBUSA SKU followed by the required quantity.</p>
        <p><a href="ProductTemplate.csv" target="_blank">Get Product File Template</a> <span class="smallest">(Uploaded file must match the template exactly including the header line)</span></p>
        <center>
            <CC:FileUpload ID="fulPrice" runat="server" /><br /><br />
            <CC:OneClickButton ID="btnImportPrice" runat="server" cssclass="btnred" Text="Import CSV" />
            <asp:Button ID="btnCancelPrice" runat="server" CausesValidation="false" cssclass="btnred" Text="Cancel" /><br />
        </center>
    </div>
</div>
<h1><%=CampaignName%></h1><a href="/admin/twoprice/campaigns/default.aspx" target="main" class="btn campaignBtn" style="text-decoration:none;color:black;">Return to event management</a>
<CC:CurrentTakeOffs id="ccCurrentTakeOffs" runat="server"></CC:CurrentTakeOffs>

<asp:UpdatePanel ID="upFilterBar" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
    <ContentTemplate>
    <asp:Panel runat="server" DefaultButton="btnSearch">
          <table cellpadding="0" cellspacing="0" border="0" class="fltrbar">
            <tr>
                 <td class="hdg">Search keywords:</td>
                <td>
                    <asp:TextBox ID="txtKeyword" runat="server" style="width:154px;color:#666;" onfocus="this.value='',this.style.color='#000'" Text="Keyword Search"></asp:TextBox>
                </td>
                <td>
                    <asp:ImageButton ID="btnSearch" runat="server" ImageUrl="/images/global/btn-fltrbar-search.gif" AlternateText="Search" style="width:28px;border:none;height:26px;" />
                </td>    
                <td style ="display :none;">
                    <span class="smallest">Projects:</span><br />
                    <CC:SearchList ID="slProjects" runat="server" AutoPostback="true" Table="Project" TextField="ProjectName" ValueField="ProjectID" CssClass="searchlist" width="150px" ViewAllLength="20" AllowNew="false" />
                </td>
                <td style ="display :none;">
                    <span class="smallest">Orders:</span><br />
                    <CC:SearchList ID="slOrders" runat="server" AutoPostback="false" Table="Order" TextField="Title" ValueField="OrderId" AllowNew="false" CssClass="searchlist" ViewAllLength="20" width="150px"></CC:SearchList>
                </td>
                <td style ="display :none;">
                    <span class="smallest">TwoPriceTakeOffs:</span><br />
                    <CC:SearchList ID="slTwoPriceTakeOffs"  runat="server" Table="TwoPriceTakeOff" TextField="Title" ValueField="TwoPriceTakeOffId" AllowNew="false" AutoPostBack="false" CssClass="searchlist" ViewAllLength="20" width="150px"></CC:SearchList>
                </td>
                <td style ="display :none;">
                    <asp:ImageButton ID="btnAdd" runat="server" ImageUrl="/images/global/btn-fltrbar-add.gif" AlternateText="Add Related Products to Take-off" style="width:28px;border:none;height:26px;" />
                </td>
                <td style ="display :none;">
                    <asp:ImageButton ID="btnUpload" runat="server" ImageUrl="/images/global/btn-fltrbar-upload.gif" AlternateText="Upload Products to Take-off" style="width:28px;border:none;height:26px;" onclientclick="return OpenUploadProduct();"   />
                </td>
                <td style ="display :none;">
                    <asp:ImageButton ID="btnSpecial" runat="server" ImageUrl="/images/global/btn-fltrbar-addspecial.gif" alternatetext="Add Special Order Product to Take-off" style="width:28px;border:none;height:26px;" onclientclick="return OpenSpecialForm();" />
                </td>
                <td style ="display :none;">
                    <img style="width: 30px; height: 1px" alt="" src="/images/spacer.gif" /><br />
                </td>
               
            </tr>
        </table>
       </asp:Panel>
    </ContentTemplate>
    <Triggers>
        <asp:PostBackTrigger ControlID="btnAdd" />
        <asp:AsyncPostbackTrigger ControlID="slProjects" />
        <asp:AsyncPostBackTrigger ControlID="btnUpload" />
        <asp:PostBackTrigger ControlID="btnSearch" />
        <asp:AsyncPostBackTrigger ControlID="hdnSearch" />
    </Triggers>
</asp:UpdatePanel>
<asp:HiddenField id="hdnSearch" runat="server" value=" "></asp:HiddenField>
<asp:Literal id="ltrErrorMsg" runat="server"></asp:Literal>
<table class="tblnwto" cellpadding="0" cellspacing="0" border="0">
      <tr valign="top">
        <td class="leftcol">
            <asp:UpdatePanel id="upFacets" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="False">
            <ContentTemplate>
                <div class="leftcolwrpr">
                    <div class="pckghdgred">1. Select Category</div>
                    <div class="redbox">
                        <ul>                            
                        <asp:Repeater id="rptProductType" runat="server">
                            <HeaderTemplate>
                                <li>By Product Type
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li><asp:Literal id="ltlLabel" runat="server"></asp:Literal></li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul>
                                </li>
                            </FooterTemplate>
                        </asp:Repeater>
                        <asp:Repeater id="rptManufacturer" runat="server">
                            <HeaderTemplate>
                                <li>By Manufacturer
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                    <li><asp:Literal id="ltlLabel" runat="server"></asp:Literal></li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul>
                                </li>
                            </FooterTemplate>
                        </asp:Repeater>
                        <asp:Repeater id="rptUnitOfMeasure" runat="server">
                            <HeaderTemplate>
                                <li>By Unit of Measure
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                    <li><asp:Literal id="ltlLabel" runat="server"></asp:Literal></li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul>
                                </li>
                            </FooterTemplate>
                        </asp:Repeater>
                        </ul>
                    </div>
                    <CC:Search ID="ctlSearch" runat="server" KeywordsTextboxId="txtKeyword" PageSize="16" CatalogType="CATALOG_TYPE_MARKET" />       
                </div>
            </ContentTemplate>    
        </asp:UpdatePanel>
        
        <!-- 'RAY
                        <asp:PostBackTrigger ControlID="tvSupplyPhases" />
        //-->
        </td>
        <td class="spacercol">&nbsp;</td>        
        <td class="maincol">
            <div class="maincolwrpr">
                <div class="pckghdgred">2. Assemble Bid List</div>
                    <!--<div><a href=""><img style="width: 500px; border:none; height: 66px;" alt="Start a New Take-off!" src="/images/dev/500x66.jpg" /></a><br /></div> -->
                    <asp:Panel ID="pnlPreferredVendor" runat="server" visible="false">
                        <table cellpadding="0" cellspacing="0" border="0" class="fltrbar">
                        <tr>
                        <td class="hdg">Select a Preferred Vendor:</td>
                        <td style="padding-top: 5px;">
                        <div style="width: 205px; margin-left: 10px; margin-bottom: 0px;">
                            <asp:UpdatePanel ID="upPreferred2" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                                <ContentTemplate>
                                    <CC:SearchList ID="slPreferredVendor2" runat="server" Table="Vendor" TextField="CompanyName" ValueField="VendorID" AllowNew="false" AutoPostback="true" Width="200px" ViewAllLength="10" CssClass="searchlist" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        </td>
                        </tr>
                        </table>
                    </asp:Panel>
                               
                <asp:UpdatePanel id="upProducts" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                    <ContentTemplate>
                        <div class="bcrmwrpr"><asp:Literal id="ltlBreadcrumb" runat="server" enableviewstate="false"></asp:Literal></div>
                        
                        <div><b><asp:Literal ID="ltlDidYouMean" runat=Server></asp:Literal></b></div>

                        <asp:PlaceHolder ID="txtOperatorOR" runat="server" Visible="False">
                        <div style="margin-top:5px;margin-bottom:5px;color:#cc0000;font-size: 10px;">
                        There were no documents that contained <b>all</b> of the words in your query. These results contain <b>some</b> of the words.
                        </div>
                        </asp:PlaceHolder >

                        <asp:PlaceHolder ID="txtExpandOR" runat="server" Visible="False">
                        <div class="bdr" style="border:1px; padding:5px; margin-top:5px;margin-bottom:15px;color:#cc0000;font-size: 12px;">
                        <a href="" style="color:#cc0000;" id="lnkExpandOR" runat="server">Click here to expand your search to see results that contain <b>some</b> of your words</a>
                        </div>
                        </asp:PlaceHolder >
                        
                        <asp:PlaceHolder ID="txtNoResults" runat="server" Visible="False">
                        <div style="margin-top:5px;">
                        Search was unable to find any results, you may have typed your word incorrectly, or are being too specific.<br /><br />Try using a broader search phrase.
                        </div>
                        </asp:PlaceHolder >
                                               
                        <table class="tbltodata" cellpadding="0" cellspacing="0" border="0">
                            <tr>
                                <th>Sku Number</th>
                                <th>Name</th>
                                <th id="tdPriceHeader" runat="server">Price</th>
                                <th>Qty</th>
                                <th>&nbsp;</th>
                            </tr>
                            <tr>
                                <td colspan="2">&nbsp;</td>
                                <td colspan="3" class="right"><asp:Button id="btnAddAll" runat="server" cssclass="btnred" text="Add All" /></td>
                            </tr>
                            <asp:Repeater id="rptProducts" runat="server">
                                <ItemTemplate>
                                    <tr class='<%#IIf(Container.ItemIndex Mod 2 = 1, "", "alt") %>'>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Sku")%></td>                                
                                        <td><%#DataBinder.Eval(Container.DataItem, "Product")%></td>
                                        <td id="tdPrice" runat="server"></td>
                                        <td><asp:TextBox id="txtQty" runat="server" maxlength="6" columns="6" class="inptqty"></asp:TextBox></td>
                                        <td>
                                            <asp:Button id="btnAddProduct" runat="server" Text="Add" onclientclick='<%# "return AddProductForm(""" & Container.UniqueId &""","""& DataBinder.Eval(Container.DataItem,"ProductID") & """);" %>' cssclass="btnadd" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"ProductID") %>'></asp:Button>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <tr class="bluerow">
                                <td colspan="2">
                                    <CC:Navigator ID="ctlNavigate" runat="server" Pagersize="5" MaxPerPage="16" AddHiddenField="true" />
                                </td>
                                <td colspan="3" class="right">
                                    <asp:Button id="btnAddAll2" runat="server" cssclass="btnred" text="Add All" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostbackTrigger ControlId="frmAddProduct"></asp:AsyncPostbackTrigger>
                    </Triggers>
                </asp:UpdatePanel>
                        <!-- 'RAY
                        <asp:AsyncPostbackTrigger ControlId="tvSupplyPhases" EventName="SelectedIndexChanged"></asp:AsyncPostbackTrigger>                    
                        //-->
            </div>
        </td>
        <td class="spacercol">&nbsp;</td>        
        <td class="rightcol">
            <div class="rightcolwrpr">
                <div class="pckghdgblue">3. Bid List</div>
                <div class="bold right bgltblue" style="padding:10px 10px 10px 5px;">
                    <asp:Label id="lblTwoPriceTakeOffTitle" runat="server" cssclass="larger center" style="display:block;"></asp:Label>
                    <asp:LinkButton id="btnSaveTop" runat="server" onclientclick="return OpenSaveForm(event,false);">Save Take-off</asp:LinkButton>
                    <span style="width:50%;text-align:right;"><asp:LinkButton id="btnCopyTop" runat="server" onclientclick="return OpenSaveForm(event,true);">Copy Take-off</asp:LinkButton></span>
                    <asp:LinkButton id="btnSendTop" runat="server" onclientclick="OpenSendForm(); return false;">Send to Builder</asp:LinkButton>
                    <asp:Button ID="btnPrice" runat="server" cssclass="btnred" onclientclick="return OpenPrice();" text="Import Products" />
                    <asp:Button ID="btnDelAll" runat="server" cssclass="btnred" text="Remove all" />
                </div>
                <div id="divComparisonLinkTop" runat="server" class="bold center bgpurple" style="padding:10px 4px;"><a class="whtlnk" href="/comparison/default.aspx">Select Vendors for Price Comparison</a> </div>                        
                <asp:UpdatePanel id="upTwoPriceTakeOff" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
						<CC:GridView id="gvTwoPriceTakeOff" runat="server" CellSpacing="0" CellPadding="0" GridLines="None" AutoGenerateColumns="False" BorderWidth="0" CssClass="tbltodata" EnableDragAndDrop="true" DragAndDropIDColumnName="TwoPriceTakeOffProductID" DragAndDropColumnIndex="3">
							<AlternatingRowStyle CssClass="alt" />
							<Columns>
								<asp:TemplateField HeaderText="Name">
									<ItemTemplate>
										<%#IIf(IsDBNull(DataBinder.Eval(Container.DataItem, "Product")), "", DataBinder.Eval(Container.DataItem, "Product"))%>
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField HeaderText="Qty">
									<ItemTemplate>
										<asp:TextBox id="txtQty" runat="server" maxlength="6" columns="6" text='<%#DataBinder.Eval(Container.DataItem, "Quantity")%>' class="inptqty"></asp:TextBox>
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField HeaderText="Price">
									<ItemTemplate>
										<asp:Literal id="ltlPrice" runat="server"></asp:Literal>
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField>
								</asp:TemplateField>
								<asp:TemplateField>
									<ItemTemplate>
										<asp:ImageButton id="btnDelete" runat="server" ImageUrl="/images/global/icon-remove.gif" CommandName="DeleteRecord" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"TwoPriceTakeOffProductId") %>' style="height:13px;width:13px;"></asp:ImageButton>
									</ItemTemplate>
								</asp:TemplateField>
							</Columns>
						</CC:GridView>
                        <%--<table class="tbltodata" cellpadding="0" cellspacing="0" border="0">
                            <tr>
                                <th>Name</th>
                                <th>Qty</th>
                                <th id="tdPrice" runat="server">Price</th>
                                <th>&nbsp;</th>
                            </tr>
                        <asp:Repeater id="rptTwoPriceTakeOff" runat="server">
                            <ItemTemplate>
                                <tr class='<%#iif(Container.ItemIndex mod 2 = 1,"","alt") %>'>
                                    <td><%#IIf(IsDBNull(DataBinder.Eval(Container.DataItem, "Product")), DataBinder.Eval(Container.DataItem, "SpecialOrderProduct"), DataBinder.Eval(Container.DataItem, "Product"))%></td>
                                    <td><asp:TextBox id="txtQty" runat="server" maxlength="6" columns="6" text='<%#DataBinder.Eval(Container.DataItem, "Quantity")%>' class="inptqty"></asp:TextBox></td>
                                    <td id="tdPrice" runat="server"></td>
                                    <td><asp:ImageButton id="btnDelete" runat="server" ImageUrl="/images/global/icon-remove.gif" CommandName="Delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"TwoPriceTakeOffProductId") %>' style="height:13px;width:13px;"></asp:ImageButton></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        </table>--%>
                        <table class="tbltodata" cellpadding="0" cellspacing="0" border="0">
                            <tr class="bluerow">
                                    <td class="center bold" style="padding:10px 0px 2px 0px;">Total Products</td>
                                    <td class="center bold" style="padding:10px 0px 2px 0px;">Total Price</td>
                            </tr>
                            <tr class="bluerow">
                                <td class="center" style="padding:2px 0px 10px 0px;"><asp:Literal id="ltlTotalProducts" runat="server"></asp:Literal></td>
                                <td class="center" style="padding:2px 0px 10px 0px;"><asp:Literal id="ltlTotalPrice" runat="server"></asp:Literal></td>
                            </tr>
                        </table>
                    </ContentTemplate>
                    <Triggers>
                        <asp:asyncpostbacktrigger ControlId="btnAddSpecial"></asp:asyncpostbacktrigger>
                    </Triggers>
                </asp:UpdatePanel> 
                <div id="divComparisonLinkBtm" runat="server" class="bold center bgpurple" style="padding:10px 4px;"><a class="whtlnk" href="/comparison/default.aspx">Select Vendors for Price Comparison</a> </div>                        
                <div class="bold right bgltblue" style="padding:10px;">
                    <asp:LinkButton id="btnSaveBtm" runat="server" onclientclick="return OpenSaveForm(event);">Save Take-off</asp:LinkButton>
                    <asp:LinkButton id="btnCopyBtm" runat="server" onclientclick="return OpenSaveForm(event,true);">Copy Take-off</asp:LinkButton>
                    <asp:LinkButton id="btnSendBtm" runat="server" onclientclick="OpenSendForm(); return false;">Send to Builder</asp:LinkButton>
                </div>
                <div class="btnhldrrt"><asp:Button id="btnUpdate" runat="server" cssclass="btngold" text="Update Quantities" /></div>
            </div>
        </td>
    </tr>
</table>

<CC:PopupForm id="frmSendTwoPriceTakeOff" runat="server" OpenMode="MoveToCenter" CloseTriggerId="btnSendCancel" CssClass="pform" Animate="true" ShowVeil="true" VeilCloses="true" Width="300px">
    <FormTemplate>
        <div class="pckghdggray" style="margin:0px;">
            <div class="pckghdgred">Send TwoPriceTakeOff To Builder</div>
            <table cellpadding="5" cellspacing="0" border="0" style="width:100%;">
                <tr>
                    <td><b>TwoPriceTakeOff Title:</b></td>
                    <td><asp:TextBox id="txtSendTitle" runat="server" columns="20"></asp:TextBox></td>
                </tr>
                <tr>
                    <td><b>Selected Builder:</b></td>
                    <td><asp:Literal id="ltlSendBuilder" runat="server"></asp:Literal></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button id="btnSendTwoPriceTakeOff" runat="server" text="Send to Builder" cssclass="btnred" />
                        <asp:Button id="btnSendCancel" runat="server" text="Cancel" cssclass="btnred" />
                    </td>
                </tr>
            </table>
        </div>
    </FormTemplate>
    <Buttons>
        <CC:PopupFormButton ControlID="btnSendTwoPriceTakeOff" ButtonType="Postback" />
        <CC:PopupFormButton ControlID="btnSendCancel" ButtonType="ScriptOnly" />
    </Buttons>
</CC:PopupForm>

<CC:PopupForm ID="frmAddProduct" runat="server" OpenMode="MoveToCenter" CloseTriggerId="btnAddCancel" CssClass="pform" Animate="true" ShowVeil="true" VeilCloses="false" style="width:300px;">
    <FormTemplate>
        <div class="pckggraywrpr">
            <div class="pckghdgred">Add Product to TwoPriceTakeOff</div>
            <div id="divAddForm" style="text-align:center;">
                This product already exists in the current TwoPriceTakeOff.<br />
                Select the line item to which this quantity should be added, or select 'Add New' to create a new line item.<br />
                <div id="divLineItems">
                    <asp:PlaceHolder id="phLineItems" runat="server"></asp:PlaceHolder>
                </div>
                <asp:Button id="btnAddContinue" runat="server" text="Continue" onclientclick="AddContinueClick();" onclick="frmAddProduct_Postback" />
                <asp:Button id="btnAddCancel" runat="server" text="Cancel" />
                <asp:HiddenField id="hdnRptItemId" runat="server"></asp:HiddenField>
            </div>
            <div id="divAddWait" style="position:absolute;top:0px;bottom:0px;left:0px;right:0px;background-color:#fff;text-align:center;">
                <img src="/images/loading.gif" style="position:absolute;top:45%;" />
            </div>
        </div>
    </FormTemplate>
    <Buttons>
        <%--<CC:PopupFormButton ControlId="btnAddContinue" ButtonType="Postback" />--%>
        <CC:PopupFormButton ControlId="btnAddCancel" ButtonType="ScriptOnly" />
    </Buttons>
</CC:PopupForm>

<div id="divSpecialOrderForm" runat="server" class="window" style="visibility:hidden;border:1px solid #000;background-color:#fff;width:350px;">
    <div class="pckghdgred">Add Special Order Product</div>
    <table cellpadding="2" cellspacing="0" border="0" style="background-color:#fff;margin:5px;">
        <tr valign="top">
            <td>&nbsp;</td>
            <td class="fieldreq">&nbsp;</td>
            <td class="field smaller"> indicates required field</td>
        </tr>
        <tr valign="top">
            <td class="fieldlbl bold" style="white-space:nowrap;"><span id="labeltxtProductName" runat="server">Product Name:</span></td>
            <td class="fieldreq" id="bartxtProductName" runat="server">&nbsp;</td>
            <td class="field">
                <asp:TextBox id="txtProductName" runat="server" columns="50" maxlength="100" style="width:200px;"></asp:TextBox>
                <asp:RequiredFieldValidator id="rfvtxtProductName" runat="server" ValidationGroup="SpecialForm" ControlToValidate="txtProductName" ErrorMessage="Product Name is required"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr valign="top">
            <td class="fieldlbl bold"><span id="labeltxtDescription" runat="server">Description:</span></td>
            <td class="fieldreq" id="bartxtDescription" runat="server">&nbsp;</td>
            <td class="field">
                <asp:TextBox id="txtDescription" runat="server" columns="50" maxlength="2000" TextMode="Multiline" rows="3" style="width:200px;"></asp:TextBox>
                <asp:RequiredFieldValidator id="rfvtxtDescription" runat="server" ValidationGroup="SpecialForm" ControlToValidate="txtDescription" ErrorMessage="Description is required"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr valign="top">
            <td class="fieldlbl bold"><span id="labeltxtSpecialQuantity" runat="server">Quantity:</span></td>
            <td class="fieldreq" id="bartxtSpecialQuantity" runat="server">&nbsp;</td>
            <td class="field">
                <asp:TextBox id="txtSpecialQuantity" runat="server" columns="10" maxlength="10" style="width:50px;"></asp:TextBox>
                <asp:RequiredFieldValidator id="rfvtxtSpecialQuantity" runat="server" ValidationGroup="SpecialForm" ControlToValidate="txtSpecialQuantity" ErrorMessage="Quantity is required"></asp:RequiredFieldValidator>
                <CC:IntegerValidator ID="ivtxtSpecialQuantity" runat="server" ValidationGroup="SpecialForm" ControlToValidate="txtSpecialQuantity" ErrorMessage="Quantity is invalid"></CC:IntegerValidator>
            </td>
        </tr>
        <tr valign="top">
            <td class="fieldlbl bold" style="white-space:nowrap;"><span id="labelacUnit" runat="server">Unit of Measure:</span></td>
            <td class="fieldreq" id="baracUnit" runat="server">&nbsp;</td>
            <td class="field">
                <CC:SearchList ID="acUnit" runat="server" Table="UnitOfMeasure" TextField="UnitOfMeasure" ValueField="UnitOfMeasureId" AllowNew="false" ViewAllLength="15" CssClass="searchlist"></CC:SearchList>
                <asp:RequiredFieldValidator id="rfvacUnit" runat="server" ValidationGroup="SpecialForm" ControlToValidate="acUnit" ErrorMessage="Unit of Measure is required"></asp:RequiredFieldValidator>    
            </td>
        </tr>
    </table>
    <p style="text-align:center;padding:10px;">
        <asp:Button id="btnAddSpecial" runat="server" cssclass="btnred" Text="Add Product" ValidationGroup="SpecialForm" />&nbsp;
        <asp:Button id="btnCancelSpecial" runat="server" cssclass="btnred" Text="Cancel" CausesValidation="false" />
    </p>
</div>
<CC:DivWindow ID="ctrlSpecial" runat="server" TargetControlID="divSpecialOrderForm" CloseTriggerId="btnCancelSpecial" ShowVeil="true" VeilCloses="false" />

<div id="divSaveForm" runat="server" class="window" style="border:1px solid #000;background-color:#fff;width:300px;">
    <asp:UpdatePanel id="upSaveForm" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
        <ContentTemplate>
            <div id="divHead" runat="server" class="pckghdgred">Save TwoPriceTakeOff</div>
            <div style="margin:10px;">
                <table cellpadding="2" cellspacing="0" border="0">
                    <tr>
                        <td colspan="3"><asp:Label id="lblSavedMsg" runat="server"></asp:Label></td>
                    </tr>
                    <tr>
                        <td><b>Title: </b></td>
                        <td class="fieldreq" id="bartxtTwoPriceTakeOffTitle" runat="server">&nbsp;</td>
                        <td><asp:TextBox id="txtTwoPriceTakeOffTitle" runat="server" columns="50" maxlength="100" style="width:200px;"></asp:TextBox><br /></td>
                    </tr>
                    <tr>
                        <td><b>Project: </b></td>
                        <td>&nbsp;</td>
                        <td>
                            <CC:SearchList ID="slTwoPriceTakeOffProject" runat="server" Table="Project" TextField="ProjectName" ValueField="ProjectID" AllowNew="false" ViewAllLength="10" CssClass="searchlist" /><br />
                            <a href="/projects/edit.aspx" target="_blank" class="smaller">Click Here to Start a New Project</a>
                        </td>
                    </tr>                                        
                    <tr>
                        <td colspan="3">
                            <p style="text-align:center;padding:10px;">
                                <asp:Button id="btnSaveSubmit" runat="server" cssclass="btnred" text="Save" onclientclick="UpdateTitle()" ValidationGroup="SaveTwoPriceTakeOff" />
                                <asp:Button id="btnCancel" runat="server" cssclass="btnred" text="Close" onclientclick="return CloseSaveForm();" />
                                <asp:HiddenField id="hdnIsCopy" runat="server"></asp:HiddenField>
                            </p>
                        </td>
                    </tr>
                </table>
            </div>
            <CC:RequiredFieldValidatorFront ID="rfvtxtTwoPriceTakeOffTitle" runat="server" ControlToValidate="txtTwoPriceTakeOffTitle" ErrorMessage="Field 'TwoPriceTakeOff Title' is empty" ValidationGroup="SaveTwoPriceTakeOff"></CC:RequiredFieldValidatorFront>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostbackTrigger ControlId="btnSaveSubmit" EventName="click"></asp:AsyncPostbackTrigger>
        </Triggers>
    </asp:UpdatePanel>
</div>
<CC:DivWindow ID="ctrlSaveForm" runat="server" TargetControlID="divSaveForm" CloseTriggerId="btnCancel" ShowVeil="true" VeilCloses="false" />

<div id="divProductQty" runat="server" class="window" style="visibility:hidden;border:1px solid #000;background-color:#fff;width:350px;">
    <div class="pckghdgred"> Upload Products </div>
    <div style="margin-left:10px;margin-right:10px">
        <br />
        <p>Use this form upload products and quantities from a csv file. The product should be identified with its CBUSA SKU followed by the required quantity.</p>
        <p><a href="ProductTemplate.csv" target="_blank">Get Product File Template</a> <span class="smallest">(Uploaded file must match the template exactly including the header line)</span></p>
        <center>
        <CC:FileUpload ID="fulDocument" runat="server" /><br /><br />
        <CC:OneClickButton ID="btnImport" Text="Import CSV" cssclass="btnred" runat="server" /> 
        <asp:Button id="btnCancelImport" runat="server" cssclass="btnred" Text="Cancel" CausesValidation="false" />
        <br />
        </center>
    </div>
</div>
<CC:DivWindow ID="ctrlProductUpload" runat="server" TargetControlID="divProductQty" CloseTriggerId="btnCancelImport" ShowVeil="true" VeilCloses="false" />
</div>
</asp:content>