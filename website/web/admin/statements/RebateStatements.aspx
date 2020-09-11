<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Rebate Document Archive" CodeFile="RebateStatements.aspx.vb" Inherits="Index" %>

<asp:Content ContentPlaceHolderID="ph" ID="Content" runat="server">
    <asp:ScriptManager ID="AjaxManager" runat="server"></asp:ScriptManager>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript">
        $("[src*=Collection]").live("click", function () {
            $(this).closest("tr").after("<tr><td></td><td colspan = '999'>" + $(this).next().html() + "</td></tr>")
            $(this).attr("src", "/images/admin/AdminMoveUp.gif");
        });
        $("[src*=AdminMoveUp]").live("click", function () {
            $(this).attr("src", "/images/admin/Collection.gif");
            $(this).closest("tr").next().remove();
        });

        $(document).ready(function () {           
            var vendorInfoW = 0;
            var colBldrId = $(".adminRebateTbl th").eq(1).width();
            $(".adminRebateSubTbl").each(function () {
                $(this).find("th").eq(0).width(colBldrId);
            });
            var colVendorInfo = $('.adminRebateTbl th:gt(1):lt(10)').addClass("vendorInfo");
            $(".vendorInfo").each(function (index) {
                vendorInfoW += parseInt($(this).width(), 10);
            });
            $(".adminRebateSubTbl").each(function () {
                $(this).find("th").eq(1).width(vendorInfoW);
            });
            var colPurchVal = $(".adminRebateTbl th").eq(12).width();
            $(".adminRebateSubTbl").each(function () {
                $(this).find("th").eq(2).width(colPurchVal);
            });

            var colRebateRate = $(".adminRebateTbl th").eq(13).width();
            $(".adminRebateSubTbl").each(function () {
                $(this).find("th").eq(3).width(colRebateRate);
            });

            var colDue = $(".adminRebateTbl th").eq(14).width();
            $(".adminRebateSubTbl").each(function () {
                $(this).find("th").eq(4).width(colDue);
            });           
        });

        function CheckAll(objRef) {
            var GridView = objRef.parentNode.parentNode.parentNode;            
            var inputList = GridView.getElementsByTagName("input");
            var checked = "";
            for (var i = 0; i < inputList.length; i++) {
                var row = inputList[i].parentNode.parentNode;
                if (inputList[i].type == "checkbox" && objRef != inputList[i]) {
                    if (objRef.checked) {
                        inputList[i].checked = true;                        
                        checked = true;
                    }
                    else {
                        inputList[i].checked = false;                      
                    }
                }
            }

            if (checked == true) {
                $('#btnSendEmail').removeAttr("disabled");
            } else {
                $('#btnSendEmail').attr("disabled", "disabled");
            }
        }

        function Check_Click(objRef) {
            //Get the Row based on checkbox
            var row = objRef.parentNode.parentNode;
            var GridView = row.parentNode;
            var inputList = GridView.getElementsByTagName("input");

            var checked = false;
            for (var i = 0; i < inputList.length; i++) {
                var headerCheckBox = inputList[0];

                if (inputList[i].type == "checkbox" && inputList[i] != headerCheckBox) {
                    if (inputList[i].checked) {
                        checked = true;
                        break;
                    }
                }
            }

            if (checked == true) {
                $('#btnSendEmail').removeAttr("disabled");
            } else {
                $('#btnSendEmail').attr("disabled", "disabled");
            }
        }

        function SelectDefaultVendorRole() {
            var checkboxes = document.getElementById('<%=chkVendorRole.ClientID %>').getElementsByTagName("input");
            
            for (var i = 0; i < checkboxes.length; i++) {
                if (checkboxes[i].type == 'checkbox') {
                    var SelectedValue = $(checkboxes[i]).next('label').text();                    
                    if (SelectedValue == 'Account Manager') {
                        checkboxes[i].checked = true;
                    }                    
                }
            }            
        }
        function OpenForm() {
            var b = Sys.UI.Behavior.getBehaviorByName($get('<%=frmSend.BehaviorId%>'), '<%=frmSend.BehaviorName%>');
            b.moveToCenter()
            b.fadeIn();
            SelectDefaultVendorRole();
            return false;
        }

        function OpenInvoiceForm() {
            var b = Sys.UI.Behavior.getBehaviorByName($get('<%=frmSuccess.BehaviorId%>'), '<%=frmSuccess.BehaviorName%>');
            b.moveToCenter()
            b.fadeIn();
            return false;
        }

        function CheckAllSendMail(ele) {
            var checkboxes = document.getElementById('<%=chkVendorRole.ClientID %>').getElementsByTagName("input");
            if (ele.checked) {
                for (var i = 0; i < checkboxes.length; i++) {
                    if (checkboxes[i].type == 'checkbox') {
                        checkboxes[i].checked = true;
                    }
                }
            } else {
                for (var i = 0; i < checkboxes.length; i++) {
                    if (checkboxes[i].type == 'checkbox') {
                        checkboxes[i].checked = false;
                    }
                }
            }
        }

        function ViewPDF(InvoiceNo)
        {
            window.open('ViewPDF.aspx?invoice='+InvoiceNo, '_blank');
            return false;
        }
    </script>
    <CC:DivWindow ID="frmSend" runat="server" TargetControlID="divPrevTakeoffs" CloseTriggerId="btnSendCancel" ShowVeil="false" VeilCloses="true" MoveToTop="false" />
    <div id="divPrevTakeoffs" runat="server" class="window window2 rebateMailPopUp">
        <%--  <FormTemplate>--%>
        <div class="popUpTitelCont">Send Options:</div>
        <div class="popUpBodyFild">
            <table id="TblVendorRole">
                <tr class="selectAll">
                    <td>
                        <asp:CheckBox ID="CheckBoxAll" runat="server" Text="Select All" onclick="CheckAllSendMail(this);"></asp:CheckBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBoxList ID="chkVendorRole" runat="server" DataValueField="VendorRoleID" DataTextField="VendorRole"></asp:CheckBoxList>
                    </td>
                </tr>
            </table>
            <div class="rightBtn">
                <asp:HiddenField ID="hdnVendorAccountID" runat="server"></asp:HiddenField>
                <asp:Button ID="btnSend" runat="server" CssClass="btn" Text="Send" />&nbsp;
                <asp:Button ID="btnSendCancel" runat="server" CssClass="btn" Text="Cancel" />
             </div>
            <div class="clearfix"></div>
        </div>
    </div>
    <CC:DivWindow ID="frmSuccess" runat="server" TargetControlID="divInvoiceSent" CloseTriggerId="btnClose" ShowVeil="false" VeilCloses="true" MoveToTop="false" />
    <div id="divInvoiceSent" runat="server" class="window window2 rebateMailPopUp">
        <%--  <FormTemplate>--%>
        <div class="popUpTitelCont">Success</div>
        <div class="popUpBodyFild">
            <table>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblErrorMsg" runat="server"></asp:Label>   
                    </td>
                </tr>
            </table>
            <div class="rightBtn">
                <asp:HiddenField ID="HiddenField1" runat="server"></asp:HiddenField>                
                <asp:Button ID="btnClose" runat="server" CssClass="btn" Text="Close" />
             </div>
            <div class="clearfix"></div>
        </div>
    </div>
    <h4>Rebate Documents Archive</h4>
    <span class="smaller">Please provide search criteria below</span>
    <span id="LblFilePath" runat="server"></span>
    <div class="pckggraywrpr">
        <asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
            <table cellpadding="2" cellspacing="2">
                <tr>
                    <th valign="top">Historic ID:</th>
                    <td valign="top" class="field">
                        <asp:TextBox ID="F_HistoricId" runat="server" Columns="50" MaxLength="5" TextMode="Number"></asp:TextBox></td>
                </tr>
                <tr>
                    <th valign="top">Company Name:</th>
                    <td valign="top" class="field">
                        <asp:TextBox ID="F_CompanyName" runat="server" Columns="50" MaxLength="50"></asp:TextBox></td>
                </tr>
                <tr>
                    <th valign="top"><b>Vendor Name:</b></th>
                    <td valign="top" class="field">
                        <asp:DropDownList ID="F_VendorID" runat="server" /></td>
                </tr>
                <tr>
                    <th valign="top">Invoice:</th>
                    <td valign="top" class="field">
                        <asp:TextBox ID="F_Invoice" runat="server" Columns="50" MaxLength="100"></asp:TextBox></td>
                </tr>
                <tr>
                    <th valign="top">Period Year:</th>
                    <td valign="top" class="field">
                        <CC:DropDownListEx ID="F_PeriodYear" runat="server">
                        </CC:DropDownListEx>
                    </td>
                </tr>
                <tr>
                    <th valign="top">Period Quarter:</th>
                    <td valign="top" class="field">
                        <CC:DropDownListEx ID="F_PeriodQuarter" runat="server">
                        </CC:DropDownListEx>
                    </td>
                </tr>
                <tr>
                    <th valign="top">Zero Due:</th>
                    <td valign="top" class="field">
                        <input type="checkbox" id="F_ZeroDue" runat="server">
                    </td>
                </tr>
                <tr>
                    <th valign="top">LLC(s):</th>
                    <td class="field">
                        <CC:CheckBoxListEx ID="F_LLC" runat="server" RepeatColumns="3"></CC:CheckBoxListEx></td>
                </tr>
                <tr>
                    <td colspan="2" align="right">
                        <CC:OneClickButton ID="btnSearch" runat="server" Text="Search" CssClass="btn" />
                        <asp:Button ID="btnExport" runat="server" Text="Export" CssClass="btn" />
                        <input class="btn" type="submit" value="Clear" onclick="window.location = 'RebateStatements.aspx'; return false;" />
                        <%--<asp:Button ID="ActionButtonARRebate" runat="server" Text="Button" 
            OnClick="ActionButtonARRebate_Click" />--%>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <p></p>
        <p>
            <input ID="btnSendEmail" type="button" value="Send" Class="btn sendEmail" disabled="disabled" onclick="return OpenForm();"/>
        </p>
        <CC:GridView ID="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom" CssClass="adminRebateTbl">
            <AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
            <RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <img alt="" style="cursor: pointer" src="/images/admin/Collection.gif" />
                        <asp:Panel ID="pnlBuilderRebates" runat="server" Style="display: none">
                            <CC:GridView ID="gvBuilderRebates" Width="100%" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="False" AllowSorting="False" EmptyDataText="There are no records to display." AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom" CssClass="tblcompr adminRebateSubTbl">
                                <AlternatingRowStyle CssClass="alternate" />
                                <RowStyle CssClass="row" />
                                <Columns>
                                    <asp:BoundField DataField="" HeaderText="" Visible="false" />
                                    <asp:BoundField DataField="BldrID" HeaderText="BldrID" />
                                    <asp:BoundField DataField="BuilderName" HeaderText="Bldr Name" />
                                    <asp:BoundField DataField="PurchVol" HeaderText="PurchVol" DataFormatString="{0:c}" />
                                    <asp:BoundField DataField="RebateRate" HeaderText="RebateRate" />
                                    <asp:BoundField DataField="AmountDue" HeaderText="AmountDue" DataFormatString="{0:c}" />
                                </Columns>
                            </CC:GridView>
                        </asp:Panel>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:CheckBox Text="Select All" ID="ChkSelectAll" runat="server" onclick="CheckAll(this);" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:CheckBox ID="ChkSelectRow" runat="server" onclick="Check_Click(this)" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Market">
                    <ItemTemplate>
                        <asp:Literal ID="LLCName" runat="server"></asp:Literal>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField SortExpression="VendorHistoricID" ItemStyle-Width="5%" DataField="VendorHistoricID" HeaderText="VendorID"></asp:BoundField>
                <asp:BoundField SortExpression="VendorName" DataField="VendorName" HeaderText="VendorName"></asp:BoundField>
                <asp:TemplateField HeaderText="AP Contact">
                    <ItemTemplate>
                        <asp:Literal ID="apContactName" runat="server"></asp:Literal>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Email">
                    <ItemTemplate>
                        <asp:Literal ID="apEmail" runat="server"></asp:Literal>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Phone">
                    <ItemTemplate>
                        <asp:Literal ID="apPhone" runat="server"></asp:Literal>
                    </ItemTemplate>
                </asp:TemplateField>
                <%--<asp:TemplateField HeaderText = "Doc Type" >
            <ItemTemplate >
                <asp:Literal ID="apDocType" runat ="server" ></asp:Literal>
            </ItemTemplate>
         </asp:TemplateField>--%>
                <asp:BoundField SortExpression="DocType" DataField="DocType" HeaderText="Doc Type"></asp:BoundField>
                <asp:BoundField SortExpression="invoice" DataField="invoice" HeaderText="Invoice"></asp:BoundField>
                <asp:BoundField SortExpression="PeriodQuarter" DataField="PeriodQuarter" HeaderText="QTR"></asp:BoundField>
                <asp:BoundField SortExpression="PeriodYear" DataField="PeriodYear" HeaderText="YR"></asp:BoundField>
                <asp:BoundField SortExpression="invoiceDate" DataField="invoiceDate" HeaderText="Date" DataFormatString="{0:d}"></asp:BoundField>
                <asp:BoundField SortExpression="DaysPastDue" DataField="DaysPastDue" HeaderText="Days"></asp:BoundField>
                <asp:BoundField SortExpression="PurchaseVolume" DataField="PurchaseVolume" HeaderText="Purch Vol" DataFormatString="{0:c}"></asp:BoundField>
                <asp:BoundField SortExpression="RebateRate" DataField="RebateRate" HeaderText="RebateRate"></asp:BoundField>
                <asp:BoundField SortExpression="AmountDue" DataField="AmountDue" HeaderText="Due" DataFormatString="{0:c}"></asp:BoundField>

                <asp:TemplateField ShowHeader="True" HeaderText="Action">
                    <ItemTemplate>
                        <%--<asp:ImageButton runat="server" ID="BtnViewFileARRebate" ImageUrl="/images/admin/Preview.gif" CommandName="ViewFile" CommandArgument="<%# Container.DataItemIndex %>" OnClick="BtnViewFileARRebate_Click" />
                <asp:ImageButton runat="server" ID="BtnDownloadFileARRebate" ImageUrl="/images/admin/Download.png" CommandName="DownloadFile" CommandArgument="<%# Container.DataItemIndex %>" OnClick="BtnDownloadFileARRebate_Click" />--%>
                        <asp:ImageButton runat="server" ID="BtnViewFileARRebate" ImageUrl="/images/admin/Preview.gif" CommandName="ViewFile" CommandArgument="<%# Container.DataItemIndex %>" />
                        <asp:ImageButton runat="server" ID="BtnDownloadFileARRebate" ImageUrl="/images/admin/Download.png" CommandName="DownloadFile" CommandArgument="<%# Container.DataItemIndex %>" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </CC:GridView>
    </div>
</asp:Content>
