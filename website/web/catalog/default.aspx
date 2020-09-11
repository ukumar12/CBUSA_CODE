<%@ Page Language="VB" AutoEventWireup="false" CodeFile="default.aspx.vb" Inherits="catalog_default" %>

<%@ Register TagName="Search" Src="~/controls/SearchSql.ascx" TagPrefix="CC" %>
<%@ Register TagPrefix="CC" TagName="Notification" Src="~/controls/Notification.ascx" %>

<CT:MasterPage ID="CTMain" runat="server">
<asp:PlaceHolder id="phCatalog" runat="server">


<asp:PlaceHolder runat="server">
    <script type="text/javascript">
        function ClearKeyword() {
            window.setTimeout(SetKeywordTextBlank(), 3000);
        }
        function SetKeywordTextBlank() {
            $get('<%=txtKeywords.ClientID %>').value = '';
        }
        function OpenHistory() {
            Sys.Application.remove_load(OpenHistory);
            var ctl = $get('<%=frmHistory.ClientID %>').control;
            ctl._doMoveToCenter();
            ctl.Open();
        }
        function OpenIndicatorConfirm() {
            Sys.Application.remove_load(OpenIndicatorConfirm);
            var ctl = $get('<%=frmConfirmIndicator.ClientID %>').control;
            ctl._doMoveToCenter();
            ctl.Open();
        }
        function InitQuoteForm(id, name) {
            $get('<%=spanQuoteProduct.ClientID %>').innerHTML = name;
            $get('<%=hdnProductID.ClientID %>').value = id;
            var ctl = $get('<%=frmQuote.ClientID %>').control;
            ctl._doMoveToCenter();
            ctl.Open();
        }
        function OpenQuoteForm() {
            Sys.Application.remove_load(OpenQuoteForm);
            var ctl = $get('<%=frmQuote.ClientID %>').control;
            ctl._doMoveToCenter();
            ctl.Open();
        }
        function OpenPricingForm() {
            Sys.Application.remove_load(OpenPricingForm);
            var ctl = $get('<%=frmPricing.ClientID %>').control;
            ctl._doMoveToCenter();
            ctl.Open();
        }
    </script>
</asp:PlaceHolder>

<CC:PopupForm ID="frmConfirmIndicator" runat="server" CssClass="pform" ShowVeil="true" VeilCloses="true" CloseTriggerId="btnCloseIndicator" Width="300px">
    <FormTemplate>
        <div class="pckggraywrpr" style="margin:0px;">
            <div class="pckghdgred">Market Indicator Added</div>
            <p class="bold" align="center" style="padding:10px;">
                Product has been added to your dashboard Market Indicators section.<br /><br />
                <asp:Button id="btnCloseIndicator" runat="server" cssclass="btnred" text="Close" />
            </p>
        </div>
    </FormTemplate>
    <Buttons>
        <CC:PopupFormButton ControlID="btnCloseIndicator" ButtonType="ScriptOnly" />
    </Buttons>
</CC:PopupForm>

<asp:UpdatePanel id="upPricing" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
    <ContentTemplate>
        <CC:PopupForm ID="frmPricing" runat="server" CssClass="pform" Width="700px" CloseTriggerId="spanClosePricing" ShowVeil="true" VeilCloses="true">
            <FormTemplate>
                <div class="pckggraywrpr" style="margin-bottom:0px;">
                    <div class="pckghdgred"><span id="spanClosePricing" runat="server" style="float:right;cursor:pointer">Close</span>Pricing for <asp:Literal id="ltlPricingHeaderProduct" runat="server"></asp:Literal></div>
                    <div  style="padding:10px;">
                        <table cellspacing="0" cellpadding="5" border="0" class="bdr" style="width:500px;display:block;margin:10px auto;background:#fff">
                            <tr>
                                <td class="bold">Product Name:</td>
                                <td><asp:Literal id="ltlPricingProduct" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <td class="bold">CBUSA SKU:</td>
                                <td><asp:Literal id="ltlPricingCBUSASKU" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <td class="bold">Description:</td>
                                <td><asp:Literal id="ltlPricingDescription" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <td class="bold">Prices good until:</td>
                                <td><asp:Literal id="ltlPricingPriceLockDate" runat="server"></asp:Literal></td>
                            </tr>
                        </table>
                        <table class="tblcompr" style="width:650px;margin:10px auto;display:block;table-layout:fixed;">
                            <tr>
                                <th width="200px">Vendor</th>
                                <th width="75px">Vendor SKU</th>
                                <th width="75px">Unit Price</th>
                                <th width="75px">Last Updated</th>
                                <th width="225px">&nbsp;</th>
                            </tr>
                            <asp:Repeater id="rptPricing" runat="server">
                                <ItemTemplate>
                                    <tr class='<%#IIf(Container.ItemIndex Mod 2 = 1, "alternate", "") %>'>
                                        <td><%#DataBinder.Eval(Container.DataItem, "CompanyName") %></td>
                                        <td><asp:Literal id="ltlPricingSku" runat="server"></asp:Literal></td>
                                        <td><asp:Literal id="ltlPricingPrice" runat="server"></asp:Literal></td>
                                        <td><asp:Literal id="ltlLastUpdate" runat="server"></asp:Literal></td>
                                        <td><asp:Literal id="ltlSubstituteNotes" runat="server"></asp:Literal></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </table>
                        <asp:Literal id="ltlNoPrices" runat="server"></asp:Literal>
                    </div>
                </div>
            </FormTemplate>
        </CC:PopupForm>
    </ContentTemplate>    
</asp:UpdatePanel>

<asp:UpdatePanel id="upHistory" runat="server" Updatemode="Conditional" ChildrenAsTriggers="false">
    <ContentTemplate>
        <CC:PopupForm runat="server" ID="frmHistory" CssClass="pform" Width="800px" CloseTriggerId="spanCloseHistory" ShowVeil="true" VeilCloses="true">
            <FormTemplate>
                <div class="pckggraywrpr" style="margin-bottom:0px;">
                    <div class="pckghdgred"><span id="spanCloseHistory" runat="server" style="float:right;cursor:pointer;">Close</span>Price History</div>

                    <div style="background-color:#fff;border:1px solid #ccc;padding:10px;width:400px;margin:10px auto;">
                        <table cellpadding="5" cellspacing="2" border="0" style="width:100%;" class="tblcompr">
                            <tr>
                                <th>Product:</td>
                                <td><asp:Literal id="ltlProduct" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>Vendor:</td>
                                <td><asp:Literal id="ltlVendor" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>Date Range:</td>
                                <td>
                                    <table cellpadding="0" cellspacing="5" border="0">
                                        <tr>
                                            <td class="smaller">From</td>
                                            <td class="smaller">To</td>
                                        </tr>
                                        <tr>
                                            <td><CC:DatePicker ID="dtHistoryFrom" runat="server"></CC:DatePicker></td>
                                            <td><CC:DatePicker ID="dtHistoryTo" runat="server"></CC:DatePicker></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" align="center">
                                    <asp:Button id="btnUpdateHistory" runat="server" cssclass="btnblue" text="Update" />
                                    <asp:HiddenField id="hdnHistoryProductID" runat="server"></asp:HiddenField>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div style="text-align:center;">
                        <asp:Literal id="ltlChart" runat="server"></asp:Literal>
                    </div>
                </div>
            </FormTemplate>
            <Buttons>
                <CC:PopupFormButton ControlID="btnUpdateHistory" ButtonType="Postback" />
            </Buttons>
        </CC:PopupForm>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostbackTrigger ControlID="frmHistory"></asp:AsyncPostbackTrigger>
    </Triggers>
</asp:UpdatePanel>

<asp:UpdatePanel id="upQuote" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
<ContentTemplate>
    <CC:PopupForm ID="frmQuote" runat="server" CssClass="pform" ShowVeil="true" VeilCloses="true" CloseTriggerId="btnCancelQuote" ValidateCallback="true" Width="400px">
        <FormTemplate>
            <div class="pckggraywrpr" style="margin:0px;">
                <div class="pckghdgred">Lookup Price for product '<span id="spanQuoteProduct" runat="server"></span>'</div>
                <table cellpadding="2" cellspacing="0" border="0" style="margin:5px;">
                    <tr>
                        <td><b>Select Date:</b></td>
                        <td><CC:DatePicker ID="dtHistoryDate" runat="server"></CC:DatePicker></td>
                        <td><CC:RequiredDateValidator ID="rdvHistoryDate" runat="server" ControlToValidate="dtHistoryDate" ErrorMessage="Please select a date" ValidationGroup="HistoryForm"></CC:RequiredDateValidator></td>
                    </tr>
                    <tr>
                        <td><b>Select Vendor:</b></td>
                        <td><asp:DropDownList id="drpQuoteVendor" runat="server"></asp:DropDownList></td>
                        <td><asp:RequiredFieldValidator id="rfvdrpQuoteVendor" runat="server" ControlToValidate="drpQuoteVendor" ErrorMessage="Please select a vendor" ValidationGroup="HistoryForm"></asp:RequiredFieldValidator></td>
                    </tr>
                    <tr>
                        <td colspan="3" class="center"><asp:Literal id="ltlQuotePrice" runat="server"></asp:Literal></td>
                    </tr>
                </table>
                <p align="center">
                    <asp:Button id="btnUpdateQuote" runat="server" cssclass="btnred" text="Update" />
                    <asp:Button id="btnCancelQuote" runat="server" cssclass="btnred" text="Close" />
                    <asp:HiddenField id="hdnProductID" runat="server"></asp:HiddenField>
                </p>
            </div>
        </FormTemplate>
        <Buttons>
            <CC:PopupFormButton ControlID="btnUpdateQuote" ButtonType="Postback" />
            <CC:PopupFormButton ControlID="btnCancelQuote" ButtonType="ScriptOnly" />
        </Buttons>    
    </CC:PopupForm>
</ContentTemplate>
</asp:UpdatePanel>
<div class="pckggraywrpr" style="margin-top:10px;">
    <div class="pckghdgred">Product Catalog</div>
    <table cellpadding="0" cellspacing="0" border="0" style="table-layout:fixed;">
        <tr valign="top">
            <td width="20%">
                <CC:Search ID="ctlSearch" runat="server" KeywordsTextboxId="txtKeywords" />
            </td>
            <td width="80%">
                <asp:UpdatePanel id="upResults" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
                <ContentTemplate>
                <div class="msgblock" id="divNotification" runat="server" visible="false">
                    <div class="iconhldr"><img src="/images/global/icon-message.gif" style="width:34px; height:34px; border-style:none;" alt="" /></div>
                    <div class="msghldr">
	                    <div class="multimsgbox">
	                        <div class="btnclose"><asp:ImageButton ID="btnClose" runat="Server" CausesValidation="False" ImageUrl="/images/global/btn-close.gif" style="width:20px; height:20px; border-style:none;" alt="" /></div>
		                    <div class="msgtxt">Your export file is being prepared.
		                    Due to the large number of records requested, this will take 1-5 minutes.
		                    An email will be sent when the export is complete and your file is ready.</div>
		                    <div style="clear:both;"></div>
	                    </div>
                    </div>
                    <div style="clear:both;"></div>
                </div>
                <CC:Notification ID="Notification1" runat="server" Visible="false" />
                <div style="border:1px solid #666;background-color:#fff;width:500px;margin:10px auto;padding:10px;text-align:center;">
                    <h1 class="largest">Catalog Search</h1>
                    Current Search: <asp:Literal id="ltlBreadcrumbs" runat="server"></asp:Literal><br /><br />
                    <asp:TextBox id="txtKeywords" runat="server" text=""></asp:TextBox>&nbsp;<asp:Button id="btnSearch" runat="server" Text="Search" cssclass="btnred" />&nbsp;
 
                    <CC:OneClickButton id="btnExport" runat="server" text="Export to Excel" cssclass="btnred" visible="true" />
                    <%--<input type="button" class="btnred" value="Export to Excel" onclick="window.open('<%=ExportUrl%>', 'ExportWindow', ''); return false;" />--%>
                    <br />
                    <asp:Panel ID="pnlPreferredVendor" runat="server" visible="false" style="padding-top:10px;">
                        <h1 class="largest">Select a Preferred Vendor</h1>
	                    <div style="z-index:2; width: 205px;">
		                    <asp:UpdatePanel ID="upPreferred2" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
		                        <ContentTemplate>
		                            <CC:SearchList ID="slPreferredVendor2" runat="server" Table="Vendor" TextField="CompanyName" ValueField="VendorID" AllowNew="false" AutoPostback="true" Width="200px" ViewAllLength="10" CssClass="searchlist" />
                                </ContentTemplate>
		                    </asp:UpdatePanel>
	                    </div>
                        <br />
                    </asp:Panel>
                    <CC:PopupForm id="frmDownload" runat="server" CloseTriggerId="lnkClose" ShowVeil="true" VeilCloses="true" CssClass="pform" style="width:300px;">
                        <FormTemplate>
                            <div class="pckggraywrpr" style="margin-bottom:0px;">
                                <div class="pckghdgblue" style="text-align:left;"><a id="lnkClose" runat="server" class="bold smallest white" style="float:right;cursor:pointer;">CLOSE</a>Download Excel File</div>
                                <div style="background-color:#fff;padding:25px;">
                                    <asp:HyperLink id="lnkDownload" runat="server" cssclass="bold largest" text="Click Here To Download Results" visible="false"></asp:HyperLink>                                
                                </div>
                            </div>
                        </FormTemplate>
                    </CC:PopupForm>
                </div>
                     <div class="CatalogGrid">
                <table class="tblcompr" cellpadding="5" cellspacing="1" border="0" style="width:100%;table-layout:fixed;">
                    <tr>
                        <th width="10px">SKU</th>
                        <th id="thVendorSku" runat="server" width="25px">Vendor SKU</th>
                        <th width="50px">Product</th>
                        <th id="thPrice" runat="server" width="20px">Price</th>
                        <th id="thHistory" runat="server" width="30px">&nbsp;</th>
                        <th width="50px">&nbsp;</th>
                        <th width="50px">&nbsp;</th>
                    </tr>
                    <asp:Repeater id="rptProducts" runat="server">
                        <ItemTemplate>
                            <tr class='<%#IIf(Container.ItemIndex Mod 2 = 1, "alternate", "row") %>'>
                                <td><b><%#DataBinder.Eval(Container.DataItem, "SKU")%></b></td>
                                <td id="tdVendorSku" runat="server" class="bold"></td>
                                <td><asp:Literal id="ltlProductName" runat="server" text='<%#DataBinder.Eval(Container.DataItem, "Product")%>'></asp:Literal></td>
                                <td id="tdPrice" runat="server"></td>
                                <td id="tdHistory" runat="server"><asp:Button id="btnHistory" runat="server" text="View History" cssclass="btnblue" commandargument='<%#DataBinder.Eval(Container.DataItem, "ProductID") %>' /></td>
                                <td><asp:Button id="btnIndicator" runat="server" text="Add Market Indicator" cssclass="btnblue" commandargument='<%#DataBinder.Eval(Container.DataItem, "ProductID") %>' /></td>
                                <td align="center">
                                    <asp:Button id="btnPricing" runat="server" text="View All Pricing" cssclass="btnblue" commandargument='<%#DataBinder.Eval(Container.DataItem, "ProductID") %>' /><br />
                                    <asp:Button id="btnQuote" runat="server" text="Lookup Past Pricing" cssclass="btnblue" onclientclick='<%# "InitQuoteForm(" & DataBinder.Eval(Container.DataItem, "ProductID") & "," & Components.Core.Escape(DataBinder.Eval(Container.DataItem, "Product")) & ");return false;" %>' />
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
                <asp:Literal id="ltlNoResults" runat="server"><p class="bold" style="padding-left:10px">Search returned no results.</p></asp:Literal>
                <CC:Navigator ID="ctlNavigator" runat="server" PagerSize="5" MaxPerPage="25" />
                        </div>
                </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
</div>
</asp:PlaceHolder>
<asp:Literal id="ltlMsg" runat="server"></asp:Literal>
</CT:MasterPage>
