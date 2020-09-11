<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PriceRequests.ascx.vb" Inherits="modules_PriceRequests" %>
<%@ Register TagName="Search" TagPrefix="CC" Src="~/controls/SearchSql.ascx" %>
<asp:PlaceHolder   runat="server">
<script type="text/javascript">
    function OpenSubstitute(id, name) {
        //var id = /btnSubstitute_([\d]+)/.exec(btn.id)[0][0];
        var hdn = $get('<%=hdnRequestId.ClientID %>');
        var hdr = $get('<%=spanSubHeaderProduct.ClientID %>');
        hdn.value = id;
        hdr.innerHTML = name;
        var b = Sys.UI.Behavior.getBehaviorByName($get('<%=frmSubstitute.BehaviorId %>'), '<%=frmSubstitute.BehaviorName %>');
        b.moveToCenter();
        b.fadeIn();
        return false;
    }
    function SwapForms() {
        Sys.Application.remove_load(SwapForms);
        CloseSubForm();
        OpenSelectForm();
    }
    function CloseSubForm() {
        Sys.Application.remove_load(CloseSubForm);
        var b = Sys.UI.Behavior.getBehaviorByName($get('<%=frmSubstitute.BehaviorId %>'), '<%=frmSubstitute.BehaviorName %>');
        b.fadeOut();
        return true;
    }
    function CloseSubForm2() {
        Sys.Application.remove_load(CloseSubForm);
        var b = Sys.UI.Behavior.getBehaviorByName($get('<%=frmSubstitute.BehaviorId %>'), '<%=frmSubstitute.BehaviorName %>');
        b.fadeOut();
        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(CloseSubForm);
        return true;
    }
    function OpenSelectForm() {
        var frm = $get('<%=frmSelect.ClientID %>').control;
        //frm._doMoveToCenter();
        frm.Open();
    }
    function CloseSelectForm() {
        Sys.Application.remove_load(CloseSelectForm);
        var frm = $get('<%=frmSelect.ClientID %>').control;
        frm.Close();
        return true;
    }
    function UpdateTotal(e) {
        var target = e.target ? e.target : e.srcElement;
        var qty = $get('<%=spanQuantity.ClientID %>').innerHTML;
        var span = $get('<%=spanRecommendedQty.ClientID %>');
        var multiply = target.value.replace(/[^\d.]/, '');
        if (multiply != '') {
            span.innerHTML = Math.ceil(parseFloat(multiply) * parseInt(qty));
        }
    }
</script>
</asp:PlaceHolder>
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
                            <tr id="trApplies" runat="server">
                                <td class="fieldlbl">Applies To:</td>
                                <td class="fieldreq">&nbsp;</td>
                                <td class="field">
                                    <asp:RadioButton ID="rbAppliesAlways" runat="server" GroupName="Applies" Checked="true" text="All Requests For This Product" /><br />
                                    <asp:RadioButton ID="rbAppliesOnce" runat="server" Text="This Builder's Request Only" GroupName="Applies" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" style="text-align:center;">
                                    <asp:Button ID="btnSave" runat="server" Text="Save Substitute" CssClass="btnred" OnClientClick="CloseSubForm2();"  onclick="frmSelect_Postback"/>&nbsp;
                                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btnred" OnClientClick="CloseSubForm2();" />
                                    <asp:HiddenField ID="hdnSubstituteID" runat="server" />
                                </td>
                            </tr>
                        </table>
                        <CC:RequiredFieldValidatorFront ID="rfvtxtQuantity" runat="server" EnableClientScript="true" ControlToValidate="txtQuantity" ErrorMessage="Quantity Multiplier is empty"></CC:RequiredFieldValidatorFront>
                        <CC:FloatValidatorFront ID="ivtxtQuantity" runat="server" EnableClientScript="true" ControlToValidate="txtQuantity" ErrorMessage="Quantity Multiplier is invalid"></CC:FloatValidatorFront>
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

<CC:DivWindow ID="frmSubstitute" runat="server" TargetControlID="divSubstitute" CloseTriggerId="btnCancelSub" ShowVeil="false" VeilCloses="true" />
    <div id="divSubstitute" runat="server" class="window" style="width:900px;">
        <div class="pckggraywrpr" style="margin-bottom:0px;">
            <div class="pckghdgred" style="height:15px;">Substitute Product</div>
            <div >
                <table cellpadding="0" cellspacing="10" border="0" style="table-layout:fixed;width:900px;height:600px;">
                    <tr valign="top">
                        <td style="width:20%;">
                            <div style="height:600px;">
                                <CC:Search ID="ctlSearch" runat="server" KeywordsTextboxId="txtKeywords" PageSize="10" />
                            </div>
                        </td>
                        <td>
                            <asp:UpdatePanel ID="upResults" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                            <ContentTemplate>
                            <input type="hidden" id="hdnRequestId" runat="server" />
                            <div class="pckghdgblue autoHeight" >
                                <div style="float:left;margin-right:100px;">
                                    <span class=" center" >Substitute product for:<br /><span runat="server" class="smaller nopad" ID="spanSubHeaderProduct"></span></span>
                                </div>
                                <div style="float:left;color:#fff;">
                                    <asp:RadioButtonList ID="rblPricedOnly" runat="server" AutoPostBack="true" Visible="false">
                                        <asp:ListItem Value="true" Selected="True">Show Priced Products Only</asp:ListItem>
                                        <asp:ListItem Value="false">Show All LLC Products</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                                <div class="clear">&nbsp;</div>
                                <div style="color:#fff;">
                                    <asp:Literal ID="ltlBreadcrumbs" runat="server"></asp:Literal><br />
                                    <span><asp:TextBox ID="txtKeywords" runat="server" Columns="25" Text=""></asp:TextBox>&nbsp;<asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btnred" /></span>
                                </div>
                                <div class="clear">&nbsp;</div>
                            </div>
                            <div class="PriceRequestGrid">
                            <table class="tblcompr" cellpadding="2" cellspacing="1" border="0" style="width:100%;margin: 15px 0 15px 0;"> 
                                <tr>
                                    <th>My SKU</th>
                                    <th>CBUSA SKU</th>
                                    <th>Name</th>
                                    <th>&nbsp;</th>
                                </tr>
                            <asp:Repeater ID="rptResults" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td><span ID="spanVendorSku" runat="server"><%#DataBinder.Eval(Container.DataItem,"VendorSku") %></span></td>
                                        <td><span ID="spanCBUSASku" runat="server"><%#DataBinder.Eval(Container.DataItem,"ProductSku") %></span></td>
                                        <td><span id="spanName" runat="server"><%#DataBinder.Eval(Container.DataItem, "Product") %></span></td>
                                        <td>
                                            <asp:Button ID="btnSelect" CausesValidation="false" runat="server" CssClass="btnblue" Text="Select" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"ProductID") %>' />
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            </table>
                            <CC:Navigator ID="ctlNavigator" runat="server" MaxPerPage="10" PagerSize="5" AddHiddenField="true" />
                                </div>
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
<div class="pckgwrpr">
    <div class="pckghdgltblue">Builder Price Requests</div>
    <asp:UpdatePanel id="upRequests" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
        <ContentTemplate>
        <CT:ErrorMessage ID="ctlError" runat="server"></CT:ErrorMessage>
        <asp:Repeater ID="rptRequests" runat="server">
            <HeaderTemplate>
                <table cellpadding="5" cellspacing="0" border="0" class="tblcompr" style="width:100%;margin:0px;table-layout:fixed;">
                    <tr>
                        <th colspan="2" style="width:200px;">&nbsp;</th>
                        <th style="width:150px;">Builder</th>
                        <th style="width:250px;">Product</th>
                        <th>Date</th>
                        <th></th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr id="trInfo" runat="server" valign="top">
                    <td><asp:Button id="btnUpdate" runat="server" CausesValidation="false" text="Update Price" cssclass="btnred" CommandName="Update" /></td>
                    <td><asp:Button id="btnSubstitute" runat="server" CausesValidation="false" text="Substitute" cssclass="btnred" OnClientClick='<%# "return OpenSubstitute(""" & Convert.ToString(DataBinder.Eval(Container.DataItem,"VendorProductPriceRequestId")) &""","""& convert.tostring(dataBinder.Eval(Container.dataItem,"Product")).Replace("""","\""") &""");" %>'></asp:Button></td>
                    <td><asp:Literal ID="ltlCompanyName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem,"CompanyName") %>'></asp:Literal></td>                
                    <td><asp:Literal ID="ltlProduct" runat="server" Text='<%#DataBinder.Eval(Container.DataItem,"Product") %>'></asp:Literal></td>
                    <td><asp:Literal ID="ltlCreated" runat="server" Text='<%#FormatDateTime(DataBinder.Eval(Container.DataItem,"Created"), DateFormat.ShortDate) %>'></asp:Literal></td>
                    <td align="right"><CC:ConfirmImageButton Message="Are you sure you want to delete this request?" ID="btnRemove" runat="server" AlternateText="Remove" ImageUrl="/images/global/icon-remove.gif" CommandName="Remove" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"VendorProductPriceRequestId") %>' /></td>
                </tr>
                <tr id="trForm" runat="server" valign="bottom">
                    <td style="text-align:right;"><asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btnred" CommandName="Save" ValidationGroup="PriceRequests" /></td>
                    <td style="text-align:left;"><asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btnred" CommandName="Cancel" CausesValidation="false" /></td>
                    <td>
                        <span class="smaller">Vendor SKU:</span><br />
                        <asp:TextBox ID="txtVendorSku" runat="server" Columns="25" MaxLength="50"></asp:TextBox>
                        <%--<asp:RequiredFieldValidator ID="rfvtxtVendorSku" runat="server" ValidationGroup="PriceRequests" ControlToValidate="txtVendorSku" ErrorMessage="Vendor SKU is empty"></asp:RequiredFieldValidator>--%>
                    </td>
                    <td>
                        <span class="smaller">Price:</span><br />
                        <asp:TextBox ID="txtPrice" runat="server" Columns="10" MaxLength="15"></asp:TextBox><br />
                        <asp:RequiredFieldValidator ID="rfvtxtPrice" runat="server" ValidationGroup="PriceRequests" ControlToValidate="txtPrice" ErrorMessage="Price is empty"></asp:RequiredFieldValidator>
                        <%--<CC:FloatValidator ID="cvtxtPrice" runat="server" ValidationGroup="PriceRequests" ControlToValidate="txtPrice" ErrorMessage="Price is invalid" />--%>
                    </td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
        <asp:Literal id="ltlNone" runat="server"></asp:Literal>
        </ContentTemplate>
    </asp:UpdatePanel>        
</div>