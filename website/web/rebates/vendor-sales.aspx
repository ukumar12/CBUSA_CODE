<%@ Page Language="VB" AutoEventWireup="false" CodeFile="vendor-sales.aspx.vb" Inherits="rebates_vendor_sales" %>

<CT:MasterPage ID="CTMain" runat="server">

<asp:PlaceHolder runat="server">
    <script type="text/javascript">
        function UpdateBuilders(ctl) {
            if(ctl.PostbackTimer) {
                window.clearTimeout(ctl.PostbackTimer);
            }
            $get('<%=hdnPostback.ClientId %>').value = ctl.value;
            ctl.PostbackTimer = window.setTimeout('<%=Page.ClientScript.GetPostbackEventReference(hdnPostback,"").Replace("'","\'") %>',500);
        }
        function UpdateInvoices(rb,showForm) {
            var span = $get(rb.id.replace('rbInvoicesYes','spanTotal').replace('rbInvoicesNo','spanTotal'));
            var txt = $get(rb.id.replace('rbInvoicesYes','txtTotal').replace('rbInvoicesNo','txtTotal'));
            var div = $get(rb.id.replace('rbInvoicesYes','divInvoiceForm').replace('rbInvoicesNo','divInvoiceForm'));
            if(showForm) {
                span.style.display = '';
                txt.style.display = 'none';
                $(div).slideDown('slow',null);
            } else {
                span.style.display='none';
                txt.style.display = '';
                $(div).slideUp('slow',null);
            }
        }
        function ToggleInvoices(btn) {
            var hdn = $get(btn.id.replace('btnInvoices','hdnInvoiceState'));
            var div = $get(btn.id.replace('btnInvoices','divInvoices'));
            if(div.style.display=='none') {
                hdn.value = 'visible';
                $(div).slideDown('slow',null);    
                btn.value = 'Hide Invoices';
            } else {
                hdn.value = 'hidden';
                $(div).slideUp('slow',null);
                btn.value = 'Show Invoices';
            }
        }
        function ClearBuilders(update) {
            var txt = $get('<%=txtBuilderFilter.ClientID %>');
            txt.value = '';
            if(update) UpdateBuilders(txt);
        }
        function OpenConfirm() {
            var tbl = $get('<%=tblConfirm.ClientID %>');
            var div = $get('<%=divConfirmResult.ClientID %>');
            var ctl = $get('<%=frmConfirm.ClientID %>').control;
            var btnOk = $get('<%=btnConfirmOk.ClientID %>');
            var btnCancel = $get('<%=btnConfirmCancel.ClientID %>');

            btnOk.style.display = '';
            btnCancel.value = 'Cancel';
            tbl.style.display = '';
            div.style.display = 'none';
            ctl.Open();
            if(tbl.clientHeight > 600) {
                ctl._window.style.height = '600px';
                ctl._window.style.overflow = 'auto';
                ctl._doMoveToCenter();
            } else {
                ctl._window.style.height = '';
                ctl._window.style.overflow = '';
            }            
            return false;
        }
        function LoadDataItems(sender, args) {
            if(args.get_dataItems) {
                var data = args.get_dataItems();
                if(data && data['frmConfirm']) {
                    var lists = args.get_dataItems()['frmConfirm'];
                    $get('<%=ltlConfirmUnreported.ClientID %>').innerHTML = lists.unreported;
                    $get('<%=ltlConfirmReported.ClientID %>').innerHTML = lists.reported;
                }
            }
        }
        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(LoadDataItems);
        function ReportConfirmCallback(res, ctxt) {
            if(res.errors) alert(res);
            var tbl = $get('<%=tblConfirm.ClientID %>');
            var div = $get('<%=divConfirmResult.ClientID %>');
            var btnOk = $get('<%=btnConfirmOk.ClientID %>');
            var btnCancel = $get('<%=btnConfirmCancel.ClientID %>');
            var ctl = $get('<%=frmConfirm.ClientID %>').control;
            div.innerHTML = res;
            ctl._window.style.height = '';
            ctl._window.style.overflow = '';
            tbl.style.display = 'none';
            tbl.style.height = '';
            div.style.display = '';
            ctl._doMoveToCenter();
            btnOk.style.display = 'none';
            btnCancel.style.display = 'none';
        }
    </script>
</asp:PlaceHolder>

<div style="display:none;">
    <CC:DatePicker id="dpPlaceholder" runat="server" />
</div>

<CC:PopupForm ID="frmConfirm" runat="server" CssClass="pform" style="padding:10px;width:400px;" CloseTriggerId="btnConfirmCancel" OpenMode="MoveToCenter" ShowVeil="true" VeilCloses="false" ErrorPlaceholderId="spanErrors">
    <FormTemplate>
        <h2 align="center">Confirm Sales Report</h2>
        <span id="spanErrors" runat="server"></span>
        <p style="text-align:center;">            
            <input type="button" id="btnConfirmOk" runat="server" class="btnred" value="OK" />
            <input type="button" runat="server" id="btnConfirmCancel" class="btnred" value="Cancel" />
        </p>
        <table id="tblConfirm" runat="server" cellpadding="5" cellspacing="10" border="0" class="tblcompr automargin" style="width:0px">
            <tr valign="top">
                <th>Unreported Builders</th>
                <th>Reported Builders</th>
            </tr>
            <tr valign="top">
                <td><span id="ltlConfirmUnreported" runat="server"></span></td>
                <td><span id="ltlConfirmReported" runat="server"></span></td>
            </tr>
            <tr valign="bottom">
                <td colspan="2" align="center">
                </td>
            </tr>
        </table>
        <div id="divConfirmResult" runat="server" style="display:none;">
        </div>
        
    </FormTemplate>
    <Buttons>
        <CC:PopupFormButton ControlId="btnConfirmOk" ButtonType="Callback" ClientCallback="ReportConfirmCallback" />
        <CC:PopupFormButton ControlId="btnConfirmCancel" ButtonType="ScriptOnly" />
    </Buttons>
</CC:PopupForm>
     <%--<asp:button id ="btnDashBoard" text="Go to DashBoard" class="btnred" runat="server" style="margin-left:361px;"/> --%>          
<div class="slctwrpr" style="width:300px;margin:10px auto;margin-top:5px;text-align:center;background:#e1e1e1">
    <div class="pckghdgred nobdr" style="margin-bottom: 5px;">1. Filter Builder List</div>
    <asp:TextBox id="txtBuilderFilter" runat="server" columns="25" onkeyup="UpdateBuilders(this)"></asp:TextBox>
    <a style="cursor:pointer;" onclick="ClearBuilders(true);">Clear</a>
</div>

<div class="pckggraywrpr">
    <div class="pckghdgred nobdr">2. Enter Sales Data</div>
    <table class="tblcompr" cellpadding="0" cellspacing="1" border="0">
        <tr>
            <th id="trLeftHeader" runat="server" style="width:40%;" class="center">
                Unreported Builders
            </th>
            <th style="width:60%;" class="center">
                Reported Builders
                <div style="float:right; margin-top:-17px;"><asp:Button runat="server" id="btnHistory" text="Sales Report History" cssclass="btnred" /></div>
            </th>
        </tr>
        <tr>
            <td id="trLeftColumn" runat="server" style="padding:5px;">
                <asp:UpdatePanel id="upUnreported" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <asp:literal id="ltlUnreportedMsg" runat="server"/>
                        <table cellpadding="10" cellspacing="1" border="0">
                            <tr>
                                <th>Builder</th>
                                <th>Total Amount</th>
                                <th><asp:Button id="btnSaveAll" runat="server" cssclass="btnred" text="Save All" /></th>
                            </tr>
                            <asp:Repeater id="rptUnreported" runat="server">
                                <ItemTemplate>
                                    <tr class='<%# iif(Container.ItemIndex mod 2 = 1,"row","alternate") %>'>
                                        <td><b><asp:Literal id="ltlBuilder" runat="server"></asp:Literal></b></td>
                                        <td><asp:TextBox id="txtTotal" runat="server" columns="8" maxlength="15"></asp:TextBox></td>
                                        <td><asp:Button id="btnSaveBuilder" runat="server" text="Save" cssclass="btnblue" CommandArgument='<%#Container.DataItem("BuilderId") %>' onclientclick="ClearBuilders(false)" /></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </table>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostbackTrigger ControlID="btnSaveAll"></asp:AsyncPostbackTrigger>
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <asp:UpdatePanel id="upReported" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                    <ContentTemplate>
                        <div class="pckghdgblue center autoHeight">
                            <b>Report Summary for:</b><br />
                            <asp:Literal id="ltlCurrentQuarter" runat="server"></asp:Literal>
                            <asp:RadioButtonList id="rblQuarter" runat="server" RepeatDirection="Horizontal" AutoPostback="true">
                            </asp:RadioButtonList>
                        </div>
                        <p id="pSubmit" runat="server" style="text-align:center;">
                            <asp:Button id="btnSubmit" runat="server" onclientclick="return OpenConfirm();" cssclass="btnred" style="color:white;background-color:#d27676" /><br />
                            <asp:Literal id="ltlDeadline" runat="server"></asp:Literal>
                        </p>
                        <asp:literal id="ltlReportedMsg" runat="server"/>
                        <table cellpadding="0" cellspacing="1" border="0" style="width:100%;">
                            <tr>
                                <th>Builder</th>
                                <th>Total Amount</th>
                                <th style="width:260px;">&nbsp;</th>
                            </tr>
                            <asp:Repeater id="rptReported" runat="server">
                                <ItemTemplate>
                                    <tr class='<%# iif(Container.ItemIndex mod 2 = 1,"row","alternate") %>'>
                                        <td><b><%#DataBinder.Eval(Container.DataItem, "CompanyName")%></b></td>
                                        <td style="text-align:right;">
                                            <span id="spanTotal" runat="server"><%#FormatCurrency(DataBinder.Eval(Container.DataItem, "TotalAmount"))%><br /></span>
                                            <asp:TextBox id="txtTotal" runat="server" text='<%#DataBinder.Eval(Container.DataItem,"TotalAmount") %>' columns="6" maxlength="8"></asp:TextBox>
                                            <CC:FloatValidator ID="fvTotal" runat="server" ControlToValidate="txtTotal" ErrorMessage="Total Amount is invalid"></CC:FloatValidator>
                                            <CC:RequiredFieldValidatorFront id="rfvTotal" runat="server" ControlToValidate="txtTotal" ErrorMessage="Total Amount is empty"></CC:RequiredFieldValidatorFront>
                                        </td>
                                        <td style="text-align:right;">
                                            <asp:Button id="btnEdit" runat="server" CommandName="Edit" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"BuilderID") %>' text="Edit" cssclass="btnred" />
                                            <CC:ConfirmButton ID="btnDelete" runat="server" CommandName="Delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"BuilderID") %>' Text="Delete" cssclass="btnred" Message="Are you sure you want to delete this report?" />
                                            <asp:Button id="btnSave" runat="server" CommandName="Save" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"BuilderID") %>' text="Save" cssclass="btnred" />
                                            <CC:ConfirmButton ID="btnCancel" runat="server" CommandName="Cancel" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"BuilderID") %>' Text="Cancel" CssClass="btnred" Message="Are you sure you want to cancel changes?" />
                                            <input type="button" id="btnInvoices" runat="server" class="btnred" value="Show Invoices" onclick="ToggleInvoices(this);" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" style="padding:0px;margin:0px;" class="bdrbottom">
                                            <div id="divWarning" runat="server" class="red bold" visible="false"></div>
                                            <div id="divInvoices" runat="server" >
                                                <b>Invoices:</b>
                                                <asp:HiddenField id="hdnInvoiceState" runat="server" enableviewstate="false"></asp:HiddenField>
                                                <table cellpadding="0" cellspacing="1" border="0" width="100%">
                                                    <tr>
                                                        <th>Invoice Amount</th>
                                                        <th>Invoice Number</th>
                                                        <th>Date of Sale</th>
                                                        <th style="width:150px;">&nbsp;</th>
                                                    </tr>
                                                <asp:PlaceHolder id="phNoInvoices" runat="server">
                                                    <tr>
                                                        <td colspan="4"><b>No matching invoices found</b></td>
                                                    </tr>
                                                </asp:PlaceHolder>
                                                <asp:Repeater id="rptInvoices" runat="server">
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td style="text-align:right;">
                                                                <asp:Literal id="ltlAmount" runat="server" text='<%#FormatCurrency(DataBinder.Eval(Container.DataItem,"InvoiceAmount")) %>'></asp:Literal>
                                                                <asp:TextBox id="txtAmount" runat="server" text='<%#FormatNumber(DataBinder.Eval(Container.DataItem,"InvoiceAmount"),2) %>'></asp:TextBox>
                                                                <asp:RequiredFieldValidator id="rfvAmount" runat="server" ControlToValidate="txtAmount" ErrorMessage="Amount is required" ValidationGroup='<%# Container.UniqueId %>'></asp:RequiredFieldValidator>
                                                                <CC:CurrencyValidator ID="fvAmount" runat="server" ControlToValidate="txtAmount" ErrorMessage="Amount is invalid" ValidationGroup='<%# Container.UniqueId %>'></CC:CurrencyValidator>
                                                            </td>
                                                            <td style="text-align:right;">
                                                                <asp:Literal id="ltlNumber" runat="server" text='<%#DataBinder.Eval(Container.DataItem,"InvoiceNumber") %>'></asp:Literal>
                                                                <asp:TextBox id="txtNumber" runat="server" text='<%#DataBinder.Eval(Container.DataItem,"InvoiceNumber") %>'></asp:TextBox>
                                                            </td>
                                                            <td style="text-align:right;">
                                                                <asp:Literal id="ltlDate" runat="server"></asp:Literal>
                                                                <CC:DatePicker ID="dpDate" runat="server"></CC:DatePicker>
                                                                <CC:DateValidator ID="dvDate" runat="server" MinDate='<%#(((ReportQuarter - 1) * 3 + 1) & "/1/" & ReportYear) %>' ControlToValidate="dpDate" ErrorMessage="Invoice date is invalid" ValidationGroup='<%# Container.UniqueId %>'></CC:DateValidator>
                                                            </td>
                                                            <td style="text-align:right;">
                                                                <asp:Button id="btnEdit" runat="server" text="Edit" CommandName="Edit" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"SalesReportBuilderInvoiceID") %>' cssClass="btnblue" />
                                                                <CC:ConfirmButton ID="btnDelete" runat="server" Text="Delete" CommandName="Delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"SalesReportBuilderInvoiceID") %>' cssClass="btnblue" Message="Are you sure you want to delete this invoice?" />
                                                                <asp:Button id="btnSave" runat="server" text="Save" CommandName="Save" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"SalesReportBuilderInvoiceID") %>' cssClass="btnblue" />
                                                                <CC:ConfirmButton id="btnCancel" runat="server" text="Cancel" CommandName="Cancel" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"SalesReportBuilderInvoiceID") %>' cssClass="btnblue" Message="Any changes will be lost.  Continue?" />
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                    
                                                </asp:Repeater>
                                                </table>
                                                <asp:Button id="btnAddInvoice" runat="server" cssclass="btnred" text="Add Invoice" CommandName="AddInvoice" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"BuilderID") %>' />
                                            </div>&nbsp;
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <footertemplate>
                                    <tr>
                                    <td><b>Total:</b></td>
                                    <td style="text-align:right;"><b><%=FormatCurrency(TotalAmount)%></b></td>
                                    </tr>            
                                </footertemplate>
                            </asp:Repeater>
                        </table>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostbackTrigger ControlId="rblQuarter"></asp:AsyncPostbackTrigger>
                        <asp:PostbackTrigger ControlId="btnSubmit"></asp:PostbackTrigger>
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
</div>
<asp:Panel ID="pnlPrint" runat="server">
    <div style="text-align: center;">
        <input type="button" class="btnred" value="Print This Page" onclick="window.open('<%=PrintUrl%>', 'PrintPage', ''); return false;" />
    </div>    
</asp:Panel>
<asp:HiddenField id="hdnPostback" runat="server"></asp:HiddenField>

</CT:MasterPage>