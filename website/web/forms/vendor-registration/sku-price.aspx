<%@ Page Language="VB" AutoEventWireup="false" CodeFile="sku-price.aspx.vb" Inherits="skuprice" EnableEventValidation="false" %>
<%@Register TagName="Search" TagPrefix="CC" Src="~/controls/SearchSql.ascx" %>

<CT:MasterPage id="CTMain" runat="server">
<asp:PlaceHolder runat="server">
<script type="text/javascript">
    function ClearKeyword() {
        window.setTimeout(SetKeywordTextBlank(), 3000);
    }
    function SetKeywordTextBlank() {
        $get('<%=txtKeywords.ClientID %>').value = '';
    }
    function SwapTarget() {
        document.forms[0].target = '_blank';
        window.setTimeout('document.forms[0].target = "_self";', 1000);
    }
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
    function OpenSubstitute(id, name) {

        //var id = /btnSubstitute_([\d]+)/.exec(btn.id)[0][0];
        var hdn = $get('<%=hdnProductID.ClientID %>');

        var hdr = $get('<%=spanSubHeaderProduct.ClientID %>');

        hdn.value = id;

        hdr.innerHTML = name;
        var b = Sys.UI.Behavior.getBehaviorByName($get('<%=frmSubstitute.BehaviorId %>'), '<%=frmSubstitute.BehaviorName %>');
        b.moveToCenter();
        b.fadeIn();
        return false;
    }
    function CloseSubForm() {
        var b = Sys.UI.Behavior.getBehaviorByName($get('<%=frmSubstitute.BehaviorId %>'), '<%=frmSubstitute.BehaviorName %>');
        b.fadeOut();
        return true;
    }
    function OpenSelectForm() {
        var frm = $get('<%=frmSelect.ClientID %>').control;
        frm._doMoveToCenter();
        frm.Open();
    }
    function CloseSelectForm() {
        var frm = $get('<%=frmSelect.ClientID %>').control;
        frm.Close();
    }
    function ConfirmRemoveSub(id) {
        var frm = $get('<%=frmConfirmRemoveSub.ClientID %>').control;
        var hdn = frm.get_input('hdnRemoveSubProductID');
        hdn.value = id;
        frm._doMoveToCenter();
        frm.Open();
    }
    function OpenConfirmDiscontinue(id) {
        var frm = $get('<%=frmConfirmDiscontinue.ClientID %>').control;
        var hdn = frm.get_input('hdnDiscontinueProductID');
        hdn.value = id;
        frm._doMoveToCenter();
        frm.Open();
    }
    function SwapPopups() {
        Sys.Application.remove_load(SwapPopups);
        CloseSubForm();
        OpenSelectForm();
    }
</script>


</asp:PlaceHolder>

<span id="trace"></span>

<nStuff:UpdateHistory ID="ctlHistory" runat="server"></nStuff:UpdateHistory>
                <%--<asp:button id ="btnDashBoard" text="Go to DashBoard" class="btnred" runat="server" />--%>
<asp:UpdatePanel ID="upFilterBar" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional" style="margin-top:10px">
    <ContentTemplate>
        <table cellpadding="0" cellspacing="0" border="0" class="skupricefltrbar">
            <tr>
                <td align="right" style="padding: 15px 0px">
                    <asp:TextBox ID="txtKeywords" runat="server" style="width:154px;color:#666;" onfocus="this.value='',this.style.color='#000'" Text="Keyword Search"></asp:TextBox>
                </td>
                <td align="left" style="padding: 10px 0px">
                    <asp:ImageButton ID="btnSearch" runat="server" ImageUrl="/images/global/btn-fltrbar-search.png" AlternateText="Search" style="width:28px;border:none;height:26px;" />
                </td>    
            </tr>
        </table>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnSearch" />
        <asp:AsyncPostBackTrigger ControlID="hdnSearch" />
    </Triggers>
</asp:UpdatePanel>
<asp:HiddenField id="hdnSearch" runat="server" value=" "></asp:HiddenField>
<table class="tblnwto" cellpadding="0" cellspacing="0" border="0" >
    <tr valign="top">
        <td class="leftcol">
            <div class="leftcolwrpr" style="border:0px">
            <div class="subhdgtakeoff" style="height:15px;border-bottom: 0px;">Supply Phase Tree</div>

            <asp:UpdatePanel id="upFacets" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="True">
            <ContentTemplate>
                <div style="background-color:#e1e1e1;border-left:1px solid #c2c2c2;border-right:1px solid #c2c2c2;" >
                    <asp:RadioButtonList id="rblFilter" runat="server" RepeatDirection="Vertical" autopostback="true">
                        <asp:Listitem value="vendor" selected="true">All Products I Am Pricing</asp:Listitem>
                        <%--<asp:ListItem value="phases">All Products In My Supply Phases</asp:ListItem>--%>
                        <asp:ListItem value="market">All Products Priced In This Market</asp:ListItem>
                        <asp:Listitem value="all">All Products In Catalog</asp:Listitem>
                    </asp:RadioButtonList>
                </div>
                    <CC:Search id="ctlSearch" runat="server" PageSize="10000" ShowTitle="False" KeywordsTextboxId="txtKeywords"></CC:Search>
            </ContentTemplate>
        </asp:UpdatePanel>
            </div>
        
        <p></p>

        </td>       
        <td>
            <div class="maincolwrpr">
                <div class="pckghdgred" > 
                    SKU-Matching & Pricing
                </div>
                <div style="float:right; margin-top:-32px; margin-right:5px;">
                    <asp:Button ID="btnSkuPrice" runat="server" text="Initial Product Sku and Price Import" cssclass="btnred" onclientclick="return OpenSkuPrice();" />
                    <asp:Button ID="btnPrice" runat="server" text="Quick Price Update" cssclass="btnred" onclientclick="return OpenPrice();" />
                </div>
                <asp:UpdatePanel id="upProducts" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                    <ContentTemplate>
                        <div class="bcrmwrpr" style="background-color:#e1e1e1">
                            <asp:Literal id="ltlBreadcrumb" runat="server" enableviewstate="false"></asp:Literal>
                        </div>
                        <asp:PlaceHolder ID="txtNoResults" runat="server" Visible="False">
                        <div style="margin-top:10px; margin-bottom:10px; margin-left:5px;">
                        <b>No products found matching your search criteria. Please select a new phase of supply or enter a new keyword.</b>
                        </div>
                        </asp:PlaceHolder >
                           <div style="float:right;">
                         <CC:OneClickButton ID="btnImportUpdateTop" runat="server" Text="SAVE Pricing Import File to Data Base" CssClass="btnred" visible="false" />
                                <CC:OneClickButton ID="btnImportPriceUpdateTop" runat="server" Text="SAVE Pricing Import File to Data Base" CssClass="btnred" visible="false" />
                                  </div>
						
                        <div style="margin-top:10px; margin-bottom:10px; margin-left:5px;">
                            <asp:Literal id="ltrErrorMsg" runat="server"></asp:Literal>
                            <div style="float:right; margin: 0 0 20px 0;">
                                <CC:OneClickButton ID="btnImportUpdate" runat="server" Text="SAVE Pricing Import File to Data Base" CssClass="btnred" visible="false"/>
                                <CC:OneClickButton ID="btnImportPriceUpdate" runat="server" Text="SAVE Pricing Import File to Data Base" CssClass="btnred" visible="false"/>
                            </div>
                        </div>
                            <div style="margin-left:5px;">
                            <table>
                                <tr>
                                    <td><img src="/images/admin/true.gif" border="0"/></td>
                                    <td><span class="small">Updated successfully in the last 2 hours</span></td>
                                </tr>
                            </table>
                            </div>
                        <div colspan="11" style="text-align:right;margin:5px"><asp:Button id="btnSaveAll1" runat="server" text="Update Pricing for All Product SKUs on this Page" cssclass="btnred" style="width: 200px; white-space: normal; " />
                               <CC:ConfirmButton   id="btnDiscontinueSelected1" runat="server" text="Discontinue Pricing for All Selected Products on this Page" Message="Are you sure that you want to discontinue pricing for the selected items ?" cssclass="btnred" style="width: 220px; white-space: normal; "/></div>
                            <table class="tbltodata" cellpadding="0" cellspacing="0" border="0" width="759" style="border:0px">
                            <%--<tr>
                              <th colspan="11" style="text-align:right;"><asp:Button id="btnSaveAll1" runat="server" text="Update Pricing for All Product SKUs on this Page" cssclass="btnred" style="width: 200px; white-space: normal; " />
                               <CC:ConfirmButton   id="btnDiscontinueSelected1" runat="server" text="Discontinue Pricing for All Selected Products on this Page" Message="Are you sure that you want to discontinue pricing for the selected items ?" cssclass="btnred" style="width: 220px; white-space: normal; "/></th>
                               </tr>--%>
                            <tr>
                                <th width="5%">&nbsp;</th>
                                <th width="5%" style="text-align:center;">CBUSA SKU</th>
                                <th width="52%" style="text-align:center;">Product Name</th>
                                <th width="5%" style="text-align:center;">Vendor SKU</th>
                                <th width="5%" style="text-align:center;">Current Price</th>
                                <th width="5%" style="text-align:center;">Next Price</th>
                                <th width="5%" style="text-align:center;">Next Price Applies</th>
                                <th width="5%" style="text-align:center;">Updated</th>
                                <th width="5%" style="text-align:center;">Substitute</th>
                                <th width="5%" style="text-align:center;">Discontinue</th>
                                 <th width="5%">&nbsp;</th>
                            </tr>
                        <asp:Repeater id="rptProducts" runat="server">
                            <headertemplate>
                                </headertemplate>
                                <ItemTemplate>
                                    <tr class='<%#IIf(Container.ItemIndex Mod 2 = 1, "", "alt") %>' id="trVendor" runat="server">
                                        <td style="text-align:center;"><asp:Image id="imgReq" runat="server" visible="false"></asp:Image></td>
                                        <td style="text-align:center;"><%#Container.DataItem.Sku%></td>                                
                                        <td><%#Container.DataItem.Product%></td>
                                        <td style="text-align:center;" ><asp:TextBox id="txtSku" runat="server" text='<%#Container.DataItem.VendorSku %>' maxlength="20" columns="20" class="inptqty1" style="width: 75px;"></asp:TextBox></td>
                                        <td style="text-align:center;">
                                            <asp:TextBox id="txtCurrentPrice" runat="server" text='<%#Container.DataItem.VendorPrice%>' maxlength="12" columns="6" class="inptqty"></asp:TextBox>
                                            <asp:Literal id="ltlDiscontinued" runat="server"></asp:Literal>
                                        </td>
                                        <td style="text-align:center;">
                                            <asp:TextBox id="txtNextPrice" runat="server" text='<%#IIf(IsDBNull(Container.DataItem.NextPrice), Container.DataItem.VendorPrice, Container.DataItem.NextPrice) %>' maxlength="12" columns="6" class="inptqty"></asp:TextBox>
                                        </td>
                                        <td style="text-align:center;">
                                            <asp:Literal id="ltlApplies" runat="server"></asp:Literal>
                                        </td>
                                        <td style="text-align:center;"><%#FormatDateTime(Container.DataItem.Updated)%></td>    

                                        <td align="center">
                                            <asp:Literal id="ltlSubstitute" runat="server"></asp:Literal><br />
                                            <asp:LinkButton id="btnSubstitute" runat="server" cssclass="smaller nopad" OnClientClick='<%# "return OpenSubstitute(""" & Convert.ToString(DataBinder.Eval(Container.DataItem, "ProductID")) & """,""" & Convert.ToString(DataBinder.Eval(Container.DataItem, "Product")).Replace("""", "\""") & """);" %>'></asp:LinkButton>
                                            <asp:LinkButton id="btnRemoveSub" runat="server" cssclass="smaller nopad" Text="Remove" onclientclick='<%# "ConfirmRemoveSub(" & Container.DataItem.ProductID & ");return false;"%>' />
                                        </td>
                                        <td style="text-align:center;">
                                            <asp:Button id="btnDiscontinue" runat="server" cssclass="btnadd" text="Discontinue" style="display:none" onclientclick='<%# "OpenConfirmDiscontinue(" & DataBinder.Eval(Container.DataItem, "ProductID") & ");return false;" %>' />
                                           <asp:Checkbox id = "chkDiscontinue" runat= "server" ></asp:Checkbox>
                                            <asp:Literal id="ltlDiscontinued2" runat="server"></asp:Literal>
                                        </td>

                                        <td align="right">
                                            <asp:Button  id="btnUpdate" runat="server" visible="false" Text="Update" cssclass="btnadd" CommandArgument='<%#Container.DataItem.ProductID %>'></asp:Button>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <footertemplate>
                                </footertemplate>
                            </asp:Repeater>

                              <%-- <tr>
                              <th colspan="11" style="text-align:right;"><asp:Button id="btnSaveAll2" runat="server" text="Update Pricing for All Product SKUs on this Page" cssclass="btnred" style="width: 200px; white-space: normal;" /> 
							  <CC:ConfirmButton   id="btnDiscontinueSelected2" runat="server" text="Discontinue Pricing for All Selected Products on this Page" Message="Are you sure that you want to discontinue pricing for the selected items ?" cssclass="btnred" style="width: 220px; white-space: normal;"/></th>
                               </tr>--%>


                            </table>
                      <div  colspan="11" style="text-align:right;margin:5px"><asp:Button id="btnSaveAll2" runat="server" text="Update Pricing for All Product SKUs on this Page" cssclass="btnred" style="width: 200px; white-space: normal;" /> 
							  <CC:ConfirmButton   id="btnDiscontinueSelected2" runat="server" text="Discontinue Pricing for All Selected Products on this Page" Message="Are you sure that you want to discontinue pricing for the selected items ?" cssclass="btnred" style="width: 220px; white-space: normal;"/></div>
                            <CC:Navigator ID="ctlNavigator" runat="server" PagerSize="5" MaxPerPage="25" />
                    </ContentTemplate>
                    <Triggers>
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </td>     
    </tr>
</table>

<div id="divSkuPrice" runat="server" class="window" style="visibility:hidden;border:1px solid #000;background-color:#fff;width:350px;">
    <div class="pckggraywrpr" style="margin-bottom:0px">
    <div class="pckghdgred" style="height: 23px;">Initial Product Sku and Price Import</div>
    <div style="margin-left:10px;margin-right:10px">
        <br />
        <p>Use this form if you wish to price new products and match your company sku to them</p>
        <p>
            <asp:LinkButton ID="LinkButton1" Text="Get SKU Matching/Pricing File"  cssclass="bold" runat="server" onclientclick="SwapTarget();" OnClick="ExportCSV" />
            <span class="smallest">(Import file must match the template exactly including the header line)</span>
        </p>
        <center>
        <CC:FileUpload ID="fulDocument" runat="server" /><br /><br />
        <CC:OneClickButton ID="btnImport" Text="Import CSV" cssclass="btnred" runat="server" /> 
        <asp:Button id="btnCancelSkuPrice" runat="server" cssclass="btnred" Text="Cancel" CausesValidation="false" />
        <br />
        </center>
    </div>
        </div>
</div>
<CC:DivWindow ID="ctrlSkuPrice" runat="server" TargetControlID="divSkuPrice" CloseTriggerId="btnCancelSkuPrice" ShowVeil="true" VeilCloses="false" />

<div id="divPrice" runat="server" class="window" style="visibility:hidden;border:1px solid #000;background-color:#fff;width:350px;">
    <div class="pckggraywrpr" style="margin-bottom:0px">
    <div class="pckghdgred">Quick Price Update</div>
    <div style="margin-left:10px;margin-right:10px">
        <br />
        <p>Use this form if you have already matched your company skus with the product catalog to quickly update pricing.</p>
        <p><asp:LinkButton ID="btnPriceExport" Text="Get Pricing File" visible="false" cssclass="bold" runat="server" onclientclick="SwapTarget();" OnClick="ExportPriceCSV" /><a href="VendorPricesTemplate.csv" target="_blank">Get Pricing File Template</a> <span class="smallest">(Import file must match the template exactly including the header line)</span></p>
        <center>
        <CC:FileUpload ID="fulPrice" runat="server" /><br /><br />
        <CC:OneClickButton ID="btnImportPrice" Text="Import CSV" cssclass="btnred" runat="server" /> 
        <asp:Button id="btnCancelPrice" runat="server" cssclass="btnred" Text="Cancel" CausesValidation="false" />
        <br />
        </center>
    </div>
        </div>
</div>
<CC:DivWindow ID="ctrlPrice" runat="server" TargetControlID="divPrice" CloseTriggerId="btnCancelPrice" ShowVeil="true" VeilCloses="false" />

<CC:PopupForm ID="frmConfirmDiscontinue" runat="server" CloseTriggerId="btnCancelDiscontinue" CssClass="pform" ShowVeil="true" VeilCloses="true" Width="300px">
    <FormTemplate>
        <div class="pckggraywrpr" style="margin:0px;">
            <div class="pckghdgred">Discontinue Price?</div>
            <p class="center bold" style="padding:10px;">
                Are you sure you want to discontinue pricing for this product?<br/><br/>
                <asp:HiddenField id="hdnDiscontinueProductID" runat="server"></asp:HiddenField>
                <asp:Button id="btnSubmitDiscontinue" runat="server" text="Continue" cssclass="btnred" />
                <asp:Button id="btnCancelDiscontinue" runat="server" text="Cancel" cssclass="btnred" />
            </p>
        </div>
    </FormTemplate>
    <Buttons>
        <CC:PopupFormButton ControlID="btnSubmitDiscontinue" ButtonType="Postback" />
        <CC:PopupFormButton ControlID="btnCancelDiscontinue" ButtonType="ScriptOnly" />
    </Buttons>
</CC:PopupForm>


<!-- Sub Windows -->

<CC:PopupForm ID="frmConfirmRemoveSub" runat="server" CloseTriggerId="btnCancelRemove" CssClass="pform" ShowVeil="true" VeilCloses="true" Width="300px">
    <FormTemplate>
        <div class="pckggraywrpr" style="margin-bottom:0px;">
            <div class="pckghdgred">Remove Substitution?</div>
            <p class="center bold" style="padding:10px;">
                Are you sure you want to remove this substitution?<br /><br />
                <asp:HiddenField id="hdnRemoveSubProductID" runat="server"></asp:HiddenField>
                <asp:Button id="btnSubmitRemove" runat="server" text="Continue" cssclass="btnred" />
                <asp:Button id="btnCancelRemove" runat="server" Text="Cancel" cssclass="btnred" />
            </p>
        </div>
    </FormTemplate>
    <Buttons>
        <CC:PopupFormButton ControlID="btnSubmitRemove" ButtonType="Postback" />
        <CC:PopupFormButton ControlID="btnCancelRemove" ButtonType="ScriptOnly" />
    </Buttons>
</CC:PopupForm>

<asp:UpdatePanel ID="upSelect" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
        <CC:PopupForm ID="frmSelect" runat="server" style="width:500px;" CssClass="pform" OpenMode="MoveToClick" Animate="true" ShowVeil="true" VeilCloses="false" CloseTriggerId="btnCancel">
            <FormTemplate>
                <div class="pckggraywrpr" style="margin-bottom:0px;">
                    <div class="pckghdgred">Save Substitute</div>
                    <div style="background-color:#fff;padding:10px;">
                        <table border="0" cellpadding="5" cellspacing="0">
                            <tr>
                                <td class="fieldlbl">Requested Product:</td>
                                <td>&nbsp;</td>
                                <td class="field"><asp:Literal ID="ltlRequestedProduct" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <td class="fieldlbl">Requested Quantity:</td>
                                <td>&nbsp;</td>
                                <td class="field"><span id="spanQuantity" runat="server"></span></td>
                            </tr>
                            <tr>
                                <td class="fieldlbl">Substitute Product:</td>
                                <td>&nbsp;</td>
                                <td class="field"><asp:Literal id="ltlProductName" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <td class="fieldlbl"><span id="labeltxtQuantity" runat="server">Quantity Multiplier:</span></td>
                                <td class="fieldreq" id="bartxtQuantity" runat="server">&nbsp;</td>
                                <td class="field"><asp:TextBox ID="txtQuantity" runat="server" Columns="5" MaxLength="5" Text="1"></asp:TextBox></td>
                            </tr>
                            <tr id="trRecommendedQty" runat="server">
                                <td class="fieldlbl">Recommended Quantity:<br /><span class="smaller">(For this request)</span></td>
                                <td>&nbsp;</td>
                                <td class="field"><span id="spanRecommendedQty" runat="server"></span></td>
                            </tr>
                            <tr>
                                <td colspan="3" style="text-align:center;">
                                    <asp:Button ID="btnSave" runat="server" Text="Save Substitute" CssClass="btnred" OnClientClick="CloseSubForm();" />&nbsp;
                                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btnred" OnClientClick="CloseSubForm();" />
                                    <asp:HiddenField ID="hdnSubstituteID" runat="server" />
                                </td>
                            </tr>
                        </table>
                        <CC:RequiredFieldValidatorFront ID="rfvtxtQuantity" runat="server" ControlToValidate="txtQuantity" ErrorMessage="Quantity Multiplier is empty"></CC:RequiredFieldValidatorFront>
                        <CC:FloatValidatorFront ID="ivtxtQuantity" runat="server" ControlToValidate="txtQuantity" ErrorMessage="Quantity Multiplier is invalid"></CC:FloatValidatorFront>
                    </div>
                </div>
            </FormTemplate>
            <Buttons>
                <CC:PopupFormButton Controlid="btnSave" ButtonType="Postback" />
                <CC:PopupFormButton ControlId="btnCancel" ButtonType="ScriptOnly" />
            </Buttons>
        </CC:PopupForm>
    </ContentTemplate>
</asp:UpdatePanel>

<CC:DivWindow ID="frmSubstitute" runat="server" TargetControlID="divSubstitute" CloseTriggerId="btnCancelSub" ShowVeil="true" VeilCloses="true" />
    <div id="divSubstitute" runat="server" class="window" style="width:900px;">
        <div class="pckggraywrpr" style="margin-bottom:0px;">
            <div class="pckghdgred" style="height:15px;">Substitute Product</div>
            <div>
                <table cellpadding="0" cellspacing="10" border="0" style="table-layout:fixed;width:900px;height:600px;">
                    <tr valign="top">
                        <td style="width:20%;">
                            <div style="height:600px;">
                                <CC:Search ID="ctlSubSearch" runat="server" KeywordsTextboxId="txtKeywords" PageSize="10" />
                            </div>
                        </td>
                        <td>
                            <asp:UpdatePanel ID="upResults" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                            <ContentTemplate>
                            <input type="hidden" id="hdnProductID" runat="server" />
                            <div class="pckghdgblue autoHeight">
                                <div style="float:left;margin-right:100px;">
                                    <span class=" center" >Substitute product for:<br /><span runat="server" class="smaller" ID="spanSubHeaderProduct"></span></span>
                                </div>
                                <div style="float:left;color:#fff;">
                                    <asp:RadioButtonList ID="rblSubPricedOnly" runat="server" AutoPostBack="true" visible="false">
                                        <asp:ListItem Value="true" Selected="True">Show Priced Products Only</asp:ListItem>
                                        <asp:ListItem Value="false">Show All LLC Products</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                                <div class="clear">&nbsp;</div>
                                <div style="color:#fff;">
                                    <asp:Literal ID="ltlSubBreadcrumbs" runat="server"></asp:Literal><br />
                                    <span><asp:TextBox ID="txtSubKeywords" runat="server" Columns="25"></asp:TextBox>&nbsp;<asp:Button ID="btnSubSearch" runat="server" Text="Search" CssClass="btnred" /></span>
                                </div>
                                <div class="clear">&nbsp;</div>
                            </div>
                            
                            <table class="tblcompr" cellpadding="2" cellspacing="0" border="0" style="width:100%;margin: 15px 0 15px 0px;">
                                <tr>
                                    <th>My SKU</th>
                                    <th>CBUSA SKU</th>
                                    <th>Name</th>
                                    <th>&nbsp;</th>
                                </tr>
                            <asp:Repeater ID="rptResults" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td><span ID="spanVendorSku" runat="server"><%#DataBinder.Eval(Container.DataItem, "VendorSku") %></span></td>
                                        <td><span ID="spanCBUSASku" runat="server"><%#DataBinder.Eval(Container.DataItem, "ProductSku") %></span></td>
                                        <td><span id="spanName" runat="server"><%#DataBinder.Eval(Container.DataItem, "Product") %></span></td>
                                        <td>
                                            <asp:Button ID="btnSelect" runat="server" CssClass="btnblue" Text="Select" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "ProductID") %>' />
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            </table>
                            <asp:Literal id="ltlSubNoResults" runat="server" visible="false"><p class="center bold">No Matching Products Were Found</p></asp:Literal>
                            <CC:Navigator ID="ctlSubNavigator" runat="server" MaxPerPage="10" PagerSize="5" AddHiddenField="true" />
                            </ContentTemplate>
                            </asp:UpdatePanel>
                            <p style="text-align:center;">
                                <asp:Button id="btnCancelSub" runat="server" text="Cancel" cssclass="btnred" />
                            </p>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>

</CT:MasterPage>