<%@ Page Language="VB" AutoEventWireup="false" CodeFile="discrepancy-report.aspx.vb" Inherits="rebates_discrepancy_report" %>

<CT:MasterPage ID="CTMain" runat="server">
<asp:PlaceHolder runat="server">
<style type="text/css">
#frmCancelConfirm_window {top: 200px !important;}
</style>
    <script type="text/javascript">

        function ShowDisputeForm(frmid) {

            var frm = document.getElementById(frmid);

            frm.style.position = 'absolute';
            frm.style.left = '450px';
            frm.style.top = '200px';
            frm.style.display = 'block';

            return false;
        }
	
	    function HideDisputeForm(frmid) {
            var frm = document.getElementById(frmid);
            frm.style.display = 'none';
            return false;
        }

        function DisputeSubmitResult(res, ctxt) {
            if (res.errors) return;

            var form = ctxt.control.get_element();            
	    var frmIndex = form.id.charAt(form.id.length-1);

            var frm = document.getElementById(form.id + '_window');
            frm.style.display = 'none';

            var div = $get(ctxt.control.get_element().id + '_divFormWrapper');
            var res = $get(ctxt.control.get_element().id + '_divResult');
            div.style.display = 'none';
            res.style.display = '';
            var form = ctxt.control.get_element();
            var btnDispute = $get(form.id.replace('frmDispute', 'btnDispute'));
            var btnCancel = $get(form.id.replace('frmDispute', 'btnCancelDispute'));
            var btnAccept = $get(form.id.replace('frmDispute', 'btnAccept'));
            if (btnDispute) btnDispute.style.display = 'none';
            if (btnCancel) btnCancel.style.display = '';
            if (btnAccept) btnAccept.style.display = 'none';
        }
        
	    function CloseForm(form) {
            form.Close();
        }
        function CloseDisputeForm(id) {
            $get(id).control.Close();
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
        function CancelDispute() {
            var c = $get('<%=frmCancelConfirm.ClientID %>').control;
            if (window.SalesReportDisputeIds) {
                for (var i = 0; i < SalesReportDisputeIds.length; i++) {
                    if (SalesReportDisputeIds[i].btnId == c.CancelButtonID) {
                        Sys.Net.WebServiceProxy.invoke('discrepancy-report.aspx', 'CancelDispute', false, { 'SalesReportDisputeID': SalesReportDisputeIds[i].disputeId }, CancelDisputeCallback, CancelDisputeCallback, { 'btnId': c.CancelButtonID });
                        return;
                    }
                }
            }
        }
        function CancelDisputeCallback(res, ctxt) {
            if (res.get_exceptionType) return;
            var btnCancel = $get(ctxt.btnId);
            var btnDispute = $get(btnCancel.id.replace('btnCancelDispute', 'btnDispute'));
            btnDispute.style.display = '';
            btnCancel.style.display = 'none';
            $get('<%=frmCancelConfirm.ClientID %>').control.Close();
        }
        function OpenCancelForm(e, id) {
            var hdn = $get('<%=hdnCancelDisputeID.ClientID %>');
            hdn.value = id;

            var c = $get('<%=frmCancelConfirm.ClientID %>').control;
            c._doMoveToClick(e);
            c.Open();
            window.scrollTo(0, 0);
        }
        function OpenDeadline() {
            Sys.Application.remove_load(OpenDeadline);
            var frm = $get('<%=frmDeadline.ClientID %>').control;
            frm._doMoveToCenter();
            frm.Open();
        }
        function OpenAccept(did, rid) {
            $get('<%=hdnAcceptDisputeID.ClientID %>').value = did;
            $get('<%=hdnAcceptReportID.ClientID %>').value = rid;
            var frm = $get('<%=frmAcceptConfirm.ClientID %>').control;
            frm._doMoveToCenter();
            frm.Open();
        }
    </script> <CC:PopupForm ID="frmAcceptConfirm" runat="server" ShowVeil="true" VeilCloses="true" CloseTriggerId="btnAcceptConfirmCancel" CssClass="pform" Width="300px">
        <FormTemplate>
            <div class="pckggraywrpr automargin">
                <div class="pckghdgred">Accept Vendor Total?</div>
                <p style="font-weight:bold;text-align:center;padding:10px;">
                    Your total for this Vendor will be updated to match the Vendor's total.<br /><br />Continue?
                    <br />
                    <br />
                    <asp:Button id="btnAcceptConfirmOk" runat="server" text="OK" cssclass="btnred" CausesValidation="false" />
                    <asp:Button id="btnAcceptConfirmCancel" runat="server" text="Cancel" cssclass="btnred" />
                    <asp:HiddenField id="hdnAcceptDisputeID" runat="server"></asp:HiddenField>
                    <asp:HiddenField id="hdnAcceptReportID" runat="server"></asp:HiddenField>
                </p>
            </div>
        </FormTemplate>
        <Buttons>
            <CC:PopupFormButton ControlID="btnAcceptConfirmOK" ButtonType="Postback" />
            <CC:PopupFormButton ControlID="btnAcceptConfirmCancel" ButtonType="ScriptOnly" />
        </Buttons>
    </CC:PopupForm>
    <CC:PopupForm id="frmDeadline" runat="server" ShowVeil="true" VeilCloses="false" CssClass="pform" Width="300px">
        <FormTemplate>
            <div class="pckggraywrpr automargin">
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
    <CC:PopupForm id="frmCancelConfirm" runat="server" CloseTriggerID="btnConfirmCancel" Animate="true" ShowVeil="true" VeilCloses="true" OpenMode="MoveToClick" width="200px" cssclass="pform">
        <FormTemplate>
            <div class="pckggraywrpr automargin">
                <div class="pckghdgred">Cancel Dispute?</div>
                <p style="font-weight:bold;text-align:center;padding:10px;">
                    This dispute will be cancelled.  Continue?
                </p>
                <p align="center">
                <asp:HiddenField id="hdnCancelDisputeID" runat="server"></asp:HiddenField>
                <asp:Button id="btnConfirmOk" runat="server" text="OK" cssclass="btnred" />
                <asp:Button id="btnConfirmCancel" runat="server" text="Cancel" cssclass="btnred" />
                </p>
            </div>
        </FormTemplate>
        <Buttons>
            <CC:PopupFormButton ControlID="btnConfirmOk" ButtonType="PostBack"></CC:PopupFormButton>
            <CC:PopupFormButton ControlID="btnConfirmCancel" ButtonType="ScriptOnly"></CC:PopupFormButton>
        </Buttons>
    </CC:PopupForm>
    <div class="pckggraywrpr">
    <div class="pckghdgred">Discrepancy Report</div>
    <asp:Repeater id="rptReport" runat="server">
        <HeaderTemplate>
            <table cellpadding="4" cellspacing="1" border="0" style="display:block;width:98%;" class="tblcompr">
                <tr>
                    <th>Vendor</th>
                    <th>Original Vendor Reported Total</th>
                    <th>Original Builder Reported Total</th>
                    <th>Difference</th>
                    <th>Builder Proposed Total</th>
                    <th>Vendor Final Accepted Total</th>
                    <th colspan="3">&nbsp;</th>
                    <th>Dispute Details</th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td><%#DataBinder.Eval(Container.DataItem, "VendorCompany")%></td>
                <td><asp:Literal id="ltlVendorTotal" runat="server"></asp:Literal></td>
                <td><asp:Literal id="ltlBuilderTotal" runat="server"></asp:Literal></td>
                <td><asp:Literal id="ltlDifference" runat="server"></asp:Literal></td>
                <td><asp:Literal id="ltlBuilderProposedTotal" runat="server"></asp:Literal></td>
                <td><asp:Literal id="ltlVendorAcceptedTotal" runat="server"></asp:Literal></td>
                <td><asp:Button id="btnDetails" runat="server" text="Invoice Detail" onclientclick="ToggleDetails(this);return false;" cssclass="btnred" /></td>
                <td>
                    <asp:Button id="btnAccept" runat="server" text="Accept" cssclass="btnred" onclientclick='<%# "OpenAccept(""" & DataBinder.Eval(Container.DataItem, "SalesReportDisputeID") & """,""" & DataBinder.Eval(Container.DataItem, "SalesReportID") & """);return false;" %>' />
                    <asp:HiddenField id="hdnSalesReportDisputeID" runat="server" value='<%#DataBinder.Eval(Container.DataItem, "SalesReportDisputeID") %>'></asp:HiddenField>
                </td>
                <td>
                    <asp:Button id="btnDispute" runat="server" text="Dispute" cssclass="btnred" />
                    <asp:Button id="btnCancelDispute" runat="server" text="Cancel Dispute" cssclass="btnred" onclientclick='<%# "OpenCancelForm(event,""" & DataBinder.Eval(Container.DataItem, "SalesReportDisputeID") & """);return false" %>' /><br />
                     <asp:Literal runat="server" id="ltlComments"></asp:Literal>
                     <CC:PopupForm ID="frmDispute" runat="server" OpenMode="MoveToCenter" Animate="true" CssClass="pform" CloseTriggerId="btnCancel" ErrorPlaceholderId="spanErrors" ValidateCallback="true" ShowVeil="true" VeilCloses="false" style="width:400px;">
                        <FormTemplate>
                            <div class="pckggraywrpr automargin">
                                <div class="pckghdgred">Dispute Vendor Sales</div>
                                <div id="divFormWrapper" runat="server">
                                    <span id="spanErrors" runat="server"></span>
                                    <table>
                                        <tr>
                                            <td>&nbsp;</td>
                                            <td class="fieldreq">&nbsp;</td>
                                            <td class="smallest"> indicates required field</td>
                                        </tr>
                                        <tr>
                                            <td><span class="fieldlbl">Vendor:</span></td>
                                            <td>&nbsp;</td>
                                            <td>
                                                <asp:Literal id="ltlVendor" runat="server"></asp:Literal>
                                                <asp:HiddenField id="hdnVendorId" runat="server"></asp:HiddenField>
                                                <asp:HiddenField id="hdnSalesReportId" runat="server"></asp:HiddenField>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td><span class="fieldlbl">Vendor Report:</span></td>
                                            <td>&nbsp;</td>
                                            <td><asp:Literal id="ltlVendorAmount" runat="server"></asp:Literal></td>
                                        </tr>
                                        <tr>
                                            <td><span class="fieldlbl" id="labeltxtBuilderAmount" runat="server">Builder Report:</span></td>
                                            <td class="fieldreq" id="bartxtBuilderAmount" runat="server">&nbsp;</td>
                                            <td class="field"><asp:TextBox id="txtBuilderAmount" runat="server" columns="15" maxlength="20"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td><span class="fieldlbl">Comments:</span></td>
                                            <td>&nbsp;</td>
                                            <td class="field"><asp:TextBox id="txtComments" runat="server" TextMode="Multiline" rows="4" columns="50"></asp:TextBox></td>
                                        </tr>
                                    </table>
                                    <p style="text-align:center;">
                                        <asp:Button id="btnSubmit" runat="server" text="Submit" cssclass="btnred" ValidationGroup="DisputeForm" />
                                        <asp:Button id="btnCancel" runat="server" text="Cancel" cssclass="btnred" ValidationGroup="DisputeForm" />
                                    </p>
                                    <CC:RequiredFieldValidatorFront ID="rfvBuilderAmount" runat="server" ControlToValidate="txtBuilderAmount" ErrorMessage="Field 'Builder Amount' is required" ValidationGroup="DisputeForm"></CC:RequiredFieldValidatorFront>
                                    <CC:CustomCurrencyValidator ID="fvBuilderAmount" runat="server" EnableClientScript="false" Display="None" ControlToValidate="txtBuilderAmount" ErrorMessage="Field 'Builder Amount' is invalid" ValidationGroup="DisputeForm"></CC:CustomCurrencyValidator>
                                </div>
                                <div id="divResult" runat="server" style="display:none;padding:25px;">
                                    <p style="text-align:center;">
                                        <b>Your dispute has been submitted</b><br /><br />
                                        <asp:Button id="btnClose" runat="server" text="Close" cssclass="btnred" />
                                    </p>
                                </div>
                            </div>
                        </FormTemplate>
                        <Buttons>
                            <CC:PopupFormButton ControlID="btnSubmit" ButtonType="Callback" ClientCallback="DisputeSubmitResult" />
                            <CC:PopupFormButton ControlID="btnCancel" ButtonType="ScriptOnly" />
                            <CC:PopupFormButton ControlID="btnClose" ButtonType="ScriptOnly" />
                        </Buttons>
                    </CC:PopupForm>
                </td>
                <td> <asp:Literal id="ltlDisputeResponse" runat="server"></asp:Literal></td>
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
                        <table cellpadding="3" cellspacing="1" border="0" style="width:49%;float:left;">
                            <tr>
                                <td class="bold bgpurple" style="color:#fff;" colspan="3">Builder Purhcases</td>
                            </tr>
                            <tr>
                                <th>Amount</th>
                                <th>PO Number</th>
                                <th>Date of Purchase</th>
                            </tr>
                            <asp:PlaceHolder id="phNoPurchases" runat="server" visible="false">
                                <tr>
                                    <td colspan="3">
                                        <b>No purchases have been entered for this vendor.</b>
                                    </td>
                                </tr>
                            </asp:PlaceHolder>
                            <asp:Repeater id="rptPurchases" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td><%#FormatCurrency(DataBinder.Eval(Container.DataItem, "POAmount"))%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "PONumber")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "PODate")%></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </table>
                    </div>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
    </div>
</asp:PlaceHolder>    
</CT:MasterPage>