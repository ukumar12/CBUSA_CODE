<%@ Page Language="VB" AutoEventWireup="false" CodeFile="discrepancy-response.aspx.vb" Inherits="rebates_discrepancy_response" %>

<CT:MasterPage ID="CTMain" runat="server">
<asp:PlaceHolder runat="server">
<script type="text/javascript">
    function isNumberKey(evt) {
        var charCode = (evt.which) ? evt.which : event.keyCode
        if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57))
            return false;

        return true;
    }
    
    function ShowResponseForm(frmid) {
        var frm = document.getElementById(frmid);

        frm.style.position = 'absolute';
        frm.style.left = '450px';
        frm.style.top = '200px';
        frm.style.display = 'block';

	var frmIndex = frmid.replace("_window", "");

        var formIndex = frmid.charAt(frmid.length - 1);

        var trReason = document.getElementById(frmIndex + '_trReason');
        var trNewAmount = document.getElementById(frmIndex + '_trNewAmount');
        var trComments = document.getElementById(frmIndex + '_trComments');
        var trAccept = document.getElementById(frmIndex + '_trAccept');
        var divTitle = document.getElementById(frmIndex + '_frmTitle');
        var pSubmit = document.getElementById(frmIndex + '_pSubmit');
        var trAdjustAmount = document.getElementById(frmIndex + '_trAdjustAmount');

        trReason.style.display = 'none';
        trComments.style.display = 'none';
        trAdjustAmount.style.display = 'none';
        trNewAmount.style.display = 'none';
        trAccept.style.display = 'none';
        pSubmit.style.display = 'none';

        document.getElementById(frmIndex + '_rptOptionButtons_ctl00_btnReason').style.display = 'block';
        document.getElementById(frmIndex + '_rptOptionButtons_ctl00_btnReason').style.display = 'block';
        document.getElementById(frmIndex + '_rptOptionButtons_ctl00_btnReason').style.display = 'block';

        return false;
    }

    function HideResponseForm(frmid) {
        var frm = document.getElementById(frmid);
        frm.style.display = 'none';
        return false;
    }

    function ResponseSubmitResult(res, ctxt) {
        if (res.errors) return;
        var div = $get(ctxt.control.id + '_divFormWrapper');
        div.style.display = 'none';
    }
    function CloseForm(form) {
        form.Close();
    }
    function ResetForm(sender) {
        var trReason = $(sender).siblings('div').find('.trReason');
        var trNewAmount = $(sender).siblings('div').find('.trNewAmount');
        var trComments = $(sender).siblings('div').find('.trComments');
        var trAccept = $(sender).siblings('div').find('.trAccept');
        var divTitle = $(sender).siblings('div').find('.frmTitle');
        var pSubmit = $(sender).siblings('div').find('.pSubmit');
        var trAdjustAmount = $(sender).siblings('div').find('.trAdjustAmount');
        trReason.hide();
        trComments.hide();
        trAdjustAmount.hide();
        trNewAmount.hide();
        trAccept.hide();
        pSubmit.hide();
        $('.btnChoice').show();
    }
    function UpdateReason(sender) {
        var trReason = $(sender).closest('.formWrapper').find('.trReason');
        var trNewAmount = $(sender).closest('.formWrapper').find('.trNewAmount');
        var trComments = $(sender).closest('.formWrapper').find('.trComments');
        var trAccept = $(sender).closest('.formWrapper').find('.trAccept');
        var divTitle = $(sender).closest('.pckggraywrpr').find('.frmTitle');
        var pSubmit = $(sender).closest('.pckggraywrpr').find('.pSubmit');
        var trAdjustAmount = $(sender).closest('.formWrapper').find('.trAdjustAmount');

        var sel = sender.value.toLowerCase();
        pSubmit.toggle(true);
        if (sel.indexOf('refuse') >= 0) {
            trReason.toggle(true);
            trAdjustAmount.toggle(true);
            trComments.toggle(true);
            trNewAmount.toggle(false);
            trAccept.toggle(false);
            divTitle.text("Refuse builders dispute to reported sales number");
        } else if (sel.indexOf('new') >= 0) {
            trReason.toggle(false);
            trNewAmount.toggle(true);
            trComments.toggle(true);
            trAccept.toggle(false)
            divTitle.text("Adjust vendors reported sales number");
        } else if (sel.indexOf('accept') >= 0) {
            trReason.toggle(false);
            trNewAmount.toggle(false);
            trComments.toggle(false);
            trAccept.toggle(true);
            divTitle.text("Accept builder reported purchase number");
        }

        $('.btnChoice').hide();
        $(sender).show();
    }
    function OpenDeadline() {
        Sys.Application.remove_load(OpenDeadline);
        var frm = $get('<%=frmDeadline.ClientID %>').control;
        frm._doMoveToCenter();
        frm.Open();
    }

    function ToggleDetails(btn) {
        var div = $get(btn.id.replace('btnDetails', 'divDetails'));
        if (div.style.display == 'none') {
            $(div).slideDown('slow', null);
            btn.value = 'Collapse';
        } else {
            $(div).slideUp('slow', null);
            btn.value = 'Invoice Detail';
        }
    }
    function ClosePopup() {
        $(".spanClose").click();
            return false;
    }

</script>


</asp:PlaceHolder>
    <CC:PopupForm id="frmDeadline" runat="server" ShowVeil="true" VeilCloses="false" CssClass="pform" Width="300px">
        <FormTemplate>
            <div class="pckggraywrpr" style="margin-bottom:0px;">
                <div class="pckghdgred">Dispute Period Closed</div>
                <p align="center" style="padding:10px;">
                    <asp:Literal id="ltlDeadline" runat="server"></asp:Literal>
                    <br /><br />
                    <asp:Button id="btnReturn" runat="server" text="Go Back" cssclass="btnred" onclientclick="history.go(-1);return false;" CausesValidation="false" />                    
                </p>
            </div>
        </FormTemplate>
        <Buttons>
            <CC:PopupFormButton ControlID="btnReturn" ButtonType="ScriptOnly" />
        </Buttons>
    </CC:PopupForm>
    
    <div class="pckggraywrpr">
    <div class="pckghdgred">Discrepancy Report - Vendor Response</div>
    <asp:UpdatePanel id="upReport" runat="server" UpdateMode="Conditional" childrenastriggers="false">
    <ContentTemplate>
        <asp:PlaceHolder id="phResult" runat="server" visible="false">
            <h2>Dispute response has been submitted.</h2>
        </asp:PlaceHolder>
        <asp:Repeater id="rptReport" runat="server">
            <HeaderTemplate>
                <table cellpadding="4" cellspacing="1" border="0" class="tblcompr" style="display:block;width:98%;">
                    <tr>
                        <th>Builder</th>
                        <th>Initial Vendor Total</th>
                        <th>Builder Total</th>
                        <th>Accepted Total</th>
                        <th></th>
                        <th>Response</th>
                        
                        <th>Reason</th>
                        <th>Last Modified On</th>
                        <th colspan="2">&nbsp;</th>
                     </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%#DataBinder.Eval(Container.DataItem, "BuilderCompany")%></td>
                    <td><asp:Literal id="ltlVendorTotal" runat="server"></asp:Literal></td>
                    <td><asp:Literal id="ltlBuilderTotal" runat="server"></asp:Literal></td>
                    <td><asp:Literal runat="server" id="ltlFinalAmount"></asp:Literal></td>
                    <td><asp:Button id="btnDetails" runat="server" text="Invoice Detail" onclientclick="ToggleDetails(this);return false;" cssclass="btnred" /></td>
                    <td><asp:Literal id="ltlResponse" runat="server"></asp:Literal></td>
                    <td><asp:Literal id="ltlReason" runat="server"></asp:Literal></td>
                     <td><%#DataBinder.Eval(Container.DataItem, "ModifyDate")%></td>
                    <td>
                        <asp:Button id="btnResponse" runat="server" text="Respond" cssclass="btnred" onclientclick="ResetForm(this)" />
                        <CC:PopupForm EnableViewState="true"  ID="frmResponse" runat="server" OpenMode="MoveToCenter" Animate="true" CssClass="pform" ErrorPlaceholderId="spanErrors" ValidateCallback="true" ShowVeil="true" VeilCloses="false" style="width:450px;">
                            <FormTemplate>
                                <div class="pckggraywrpr automargin">
                                <asp:Button  id="btnSpanClose" runat="server" class="btnred" text="Close" style="cursor:pointer;float:right;margin-right:10px;"></asp:button>
                                <div class="pckghdgred frmTitle">Respond to Builder Dispute</div>
                                <div id="divFormWrapper" runat="server" class="formWrapper">
                                    <span id="spanErrors" runat="server"></span>
                                    <asp:updatepanel id="upErrors" runat="server" updatemode="conditional">
                                        <ContentTemplate>
                                            <div align="center" id="divError" style="background-color:#ffff99; border:#ff0000 1px solid;" runat="server" visible="false">
                                                <div class="bold red" style="margin-bottom:4px;">
                                                    <img src="/images/exclam.gif" width="24" height="24" style="border-style:none; padding-top:6px;" alt=""Error"" />
                                                    <asp:literal id="ltlErrors" runat="server" />
                                                </div>
                                            </div>

                                            <asp:DropDownList id="drpResponse" runat="server" visible="false"></asp:DropDownList>
                                        </ContentTemplate>
                                    </asp:updatepanel>
                                    
                                    <table style="width:100%;">
                                        <tr>
                                            <td><span class="fieldlbl">Builder:</span></td>
                                            <td>&nbsp;</td>
                                            <td>
                                                <asp:Literal id="ltlBuilder" runat="server"></asp:Literal>
                                                <asp:HiddenField id="hdnSalesReportDisputeId" runat="server"></asp:HiddenField>
                                                <asp:HiddenField id="hdnReasonId" runat="server"></asp:HiddenField>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td><span class="fieldlbl">Builder Purchase Report:</span></td>
                                            <td>&nbsp;</td>
                                            <td><asp:Literal id="ltlBuilderAmount" runat="server"></asp:Literal></td>
                                        </tr>
                                        <tr>
                                            <td><span class="fieldlbl">Builder Comments:</span></td>
                                            <td>&nbsp;</td>
                                            <td><asp:Literal id="ltlBuilderComments" runat="server"></asp:Literal></td>
                                        </tr>
                                         <tr>
                                            <td><span class="fieldlbl">Vendor Comments:</span></td>
                                            <td>&nbsp;</td>
                                            <td><asp:Literal id="ltlVendorComments" runat="server"></asp:Literal></td>
                                        </tr>
                                        <tr>
                                            <td><span class="fieldlbl">Vendor Sales Report:</span></td>
                                            <td>&nbsp;</td>
                                            <td><asp:Literal id="ltlVendorAmount" runat="server"></asp:Literal></td>
                                        </tr>
                                    </table>
                                    <br />
                                    <table style="width:100%;">
                                        <tr>
                                            <asp:Repeater id="rptOptionButtons" runat="server">
                                                <itemtemplate>
                                                    <td>
                                                        <asp:Button OnClick="ReasonClick" onclientclick="UpdateReason(this)" id="btnReason" runat="server" cssclass="btnred btnChoice" />
                                                    </td>
                                                </itemtemplate>
                                            </asp:Repeater>
                                        </tr>
                                    </table>
                                    <table style="width:100%;">
                                        <tr id="trReason" runat="server" class="trReason">
                                            <td>
                                                <span>Reason:</span>
                                                <br />
                                                <span class="smallest">required field</span>
                                            </td>
                                            <td id="bardrpReason" runat="server">&nbsp;</td>
                                            <td>
                                                <asp:radiobuttonlist id="rblReason" runat="server" repeat-direction="horizontal"></asp:radiobuttonlist>
                                            </td>
                                        </tr>

                                         <tr id="trAdjustAmount" runat="server" class="trAdjustAmount">
                                            <td>
                                                <span>Adjust sales report:</span>
                                                <br />
                                             
                                            </td>
                                            <td id="bartrAdjustAmount" runat="server">&nbsp;</td>
                                            <td>
                                                <asp:TextBox id="txtAdjustAmount"  class="txtAdjustAmount" onkeypress="return isNumberKey(event)" columns="15" maxlength="15" runat="server" ></asp:TextBox>
                                            </td>
                                        </tr>

                                        <tr id="trNewAmount" runat="server" class="trNewAmount">
                                            <td>
                                                <span class="fieldlbl">Adjusted sales report </span>
                                                <br />
                                                <span class="smallest">required field</span>
                                            </td>
                                            <td id="bartxtNewAmount" runat="server">&nbsp;</td>
                                            <td><asp:TextBox id="txtNewAmount" runat="server" columns="10" maxlength="10"></asp:TextBox></td>
                                        </tr>
                                        <tr id="trAccept" runat="server" class="trAccept">
                                            <td><span class="fieldlbl">Vendor adjusted sales report to match builders purchase report </span></td>
                                            <td>&nbsp;</td>
                                            <td><asp:textbox enabled="false" runat="server" id="txtPrevAmount" /></td>
                                        </tr>
                                        <tr id="trComments" runat="server" class="trComments">
                                            <td><span class="fieldlbl">Comments:</span></td>
                                            <td>&nbsp;</td>
                                            <td><asp:TextBox id="txtComments" runat="server" textmode="MultiLine" rows="4" columns="50"></asp:TextBox></td>
                                        </tr>
                                    </table>
                                    <p style="text-align:center;display:none" class="pSubmit" id="pSubmit" runat="server">
                                        <asp:Button id="btnSubmit" runat="server" text="Submit" cssclass="btnred" />
                                        <asp:Button id="btnCancel" runat="server" text="Cancel" cssclass="btnred spanClose" />
                                    </p>
                                   
                                </div>
                                <div id="divResult" runat="server" style="display:none;">
                                    <p style="text-align:center;">
                                        <b>Your response has been submitted</b><br /><br />
                                        <asp:Button id="btnClose" runat="server" text="Close" cssclass="btnRed" />
                                    </p>
                                </div>
                                </div>
                            </FormTemplate>
                            <Buttons>
                                <CC:PopupFormButton ControlID="btnSubmit" ButtonType="Postback" />
                                <CC:PopupFormButton ControlID="btnCancel" ButtonType="ScriptOnly" />
                            </Buttons>
                        </CC:PopupForm>
                    </td>
                   </tr>
                <tr>
                <td colspan="7" style="padding:0px;margin:0px;">
                    <div id="divDetails" runat="server">
                        <table cellpadding="3" cellspacing="1" border="0" style="width:49%;float:left;">
                            <tr>
                                <td class="bold bgpurple" style="color:#fff;" colspan="3">Vendor Invoices</td>
                            </tr>
                            <tr>
                                <th>Amount</th>
                                <th>Invoice #</th>
                                <th>Date of Sale</th>
                            </tr>
                            <asp:PlaceHolder id="phNoInvoices" runat="server" visible="false">
                                <tr>
                                    <td colspan="3">
                                        <b>No invoices have been entered by this vendor.</b>
                                    </td>
                                </tr>
                            </asp:PlaceHolder>
                            <asp:Repeater id="rptInvoices" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td><%#FormatCurrency(DataBinder.Eval(Container.DataItem, "InvoiceAmount"))%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "InvoiceNumber")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "InvoiceDate")%></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </table>
                       
                    </div>
                </td>
            </tr>
            </ItemTemplate>
        </asp:Repeater>
           </ContentTemplate>
    </asp:UpdatePanel>
    </div>
</CT:MasterPage>
