<%@ Page Language="VB" AutoEventWireup="false" CodeFile="RebateStatements-Vendor.aspx.vb" Inherits="rebates_RebateStatements_Vendor" %>

<!DOCTYPE html>

<%--<script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>--%>
<%--<script src="../includes/jquery-3.3.1.min.js"></script>--%>
<script type="text/javascript" src="https://code.jquery.com/jquery-3.3.1.min.js"></script>
<script type="text/javascript">
    //$("[src*=Collection]").live("click", function () {
    //    $(this).closest("tr").after("<tr><td></td><td colspan = '999'>" + $(this).next().html() + "</td></tr>")
    //    $(this).attr("src", "/images/admin/AdminMoveUp.gif");
    //});
    //$("[src*=AdminMoveUp]").live("click", function () {
    //    $(this).attr("src", "/images/admin/Collection.gif");
    //    $(this).closest("tr").next().remove();
    //});
    $(document).on("click", "[src*=Collection]", function () {
        $(this).closest("tr").after("<tr><td></td><td colspan = '999'>" + $(this).next().html() + "</td></tr>")
        $(this).attr("src", "/images/admin/AdminMoveUp.gif");
    });
    $(document).on("click", "[src*=AdminMoveUp]", function () {
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

    function ViewPDF(InvoiceNo) {
        window.open('ViewPDF.aspx?invoice=' + InvoiceNo, '_blank');
        return false;
    }
</script>
<CT:MasterPage ID="CTMain" runat="server">
    <CC:DivWindow ID="frmSend" runat="server" CloseTriggerId="btnSendCancel" MoveToTop="false" ShowVeil="false" TargetControlID="divPrevTakeoffs" VeilCloses="true" /><div id="divPrevTakeoffs" runat="server" class="window window2 rebateMailPopUp"><div class="pckggraywrpr" style="margin:0px"><%--  <FormTemplate>--%>
        <div class="popUpTitelCont">Send Options:</div><div class="popUpBodyFild"><table><tr class="selectAll"><td><asp:CheckBox ID="CheckBoxAll" runat="server" onclick="CheckAllSendMail(this);" Text="Select All"></asp:CheckBox>
                    </td>
                </tr>
                <tr><td><asp:CheckBoxList ID="chkVendorRole" runat="server" DataTextField="VendorRole" DataValueField="VendorRoleID"></asp:CheckBoxList>
                    </td>
                </tr>
            </table>
            <div class="rightBtn"><asp:HiddenField ID="hdnVendorAccountID" runat="server"></asp:HiddenField>
                <asp:Button ID="btnSend" runat="server" CssClass="btnred" Text="Send" />&nbsp; <asp:Button ID="btnSendCancel" runat="server" CssClass="btnred" Text="Cancel" /></div>
            <div class="clearfix"></div>
        </div>
    </div>
    </div>
    <CC:DivWindow ID="frmSuccess" runat="server" TargetControlID="divInvoiceSent" CloseTriggerId="btnClose" ShowVeil="false" VeilCloses="true" MoveToTop="false"/><div id="divInvoiceSent" runat="server" class="window window2 rebateMailPopUp"><%--  <FormTemplate>--%>
        <div class="popUpTitelCont">Success</div><div class="popUpBodyFild"><table><tr><td>&nbsp; </td></tr><tr><td><asp:Label ID="lblErrorMsg" runat="server"></asp:Label></td></tr></table><div class="rightBtn"><asp:HiddenField ID="HiddenField1" runat="server"></asp:HiddenField>                
                <asp:Button ID="btnClose" runat="server" CssClass="btn" Text="Close" /></div>
            <div class="clearfix"></div>
        </div>
    </div>
    <div class="pckggraywrpr"><div class="pckghdgltblue">Rebate Invoices</div><div class="pckgbdy"><%--<span class="smaller" style="margin-bottom:5px">Please provide search criteria below</span>--%>
    <span id="LblFilePath" runat="server"></span>

  <%--  <asp:PlaceHolder runat="server">--%>
         
    <asp:Panel ID="pnlSearch" runat="server" DefaultButton="btnSearch"><table cellpadding="2" cellspacing="2" class="white"><tr><th valign="top">Historic ID:</th><td class="field" valign="top"><asp:TextBox ID="F_HistoricId" runat="server" Columns="40" MaxLength="5" TextMode="Number"></asp:TextBox></td><th valign="top">Company Name:</th><td class="field" valign="top"><asp:TextBox ID="F_CompanyName" runat="server" Columns="40" MaxLength="50"></asp:TextBox></td></tr><tr><th valign="top"><b>Vendor Name:</b></th><td class="field" valign="top"><asp:DropDownList ID="F_VendorID" runat="server" /></td>
                
                    <th valign="top">Invoice:</th><td class="field" valign="top"><asp:TextBox ID="F_Invoice" runat="server" Columns="40" MaxLength="100"></asp:TextBox></td></tr><tr><th valign="top">Period Year:</th><td class="field" valign="top"><CC:DropDownListEx ID="F_PeriodYear" runat="server"></CC:DropDownListEx>
                    </td>
               
                    <th valign="top">Period Quarter:</th><td class="field" valign="top"><CC:DropDownListEx ID="F_PeriodQuarter" runat="server"></CC:DropDownListEx>
                    </td>
                </tr>
                <tr><th valign="top">Zero Due:</th><td class="field" valign="top"><input id="F_ZeroDue" runat="server" type="checkbox"> </input></td><th valign="top">LLC(s):</th><td class="field" style="color:#0f2e51 "><CC:CheckBoxListEx ID="F_LLC" runat="server" RepeatColumns="3"></CC:CheckBoxListEx></td>
                </tr>
                <tr><td align="right" colspan="4"><CC:OneClickButton ID="btnSearch" runat="server" CssClass="btnred" Text="Search" /><asp:Button ID="btnExport" runat="server" CssClass="btnred" Text="Export" /><input class="btnred" onclick="window.location = 'RebateStatements-Vendor.aspx'; return false;" type="submit" value="Clear" /><%--<asp:Button ID="ActionButtonARRebate" runat="server" Text="Button" 
            OnClick="ActionButtonARRebate_Click" />--%>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <p></p>
        <p><input id="btnSendEmail" class="btnred RebatesendEmail" disabled="disabled" onclick="return OpenForm();" type="button" value="Send" /> </p><CC:GridView ID="gvList" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" BorderWidth="0" CellPadding="2" CellSpacing="2" cssclass="adminRebateTbl gridLableCont tblcomprlen" EmptyDataText="There are no records that match the search criteria" HeaderText="In order to change display order, please use header links" PagerSettings-Position="Bottom" PageSize="50" style="margin: 15px 0 15px 0px;"><AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns><asp:TemplateField><ItemTemplate><img alt="" src="/images/admin/Collection.gif" style="cursor: pointer" /><asp:Panel ID="pnlBuilderRebates" runat="server" Style="display: none"><CC:GridView ID="gvBuilderRebates" runat="server" AllowPaging="False" AllowSorting="False" AutoGenerateColumns="False" BorderWidth="0" CellPadding="2" CellSpacing="2" cssclass="tblcompr adminRebateSubTbl" EmptyDataText="There are no records to display." PagerSettings-Position="Bottom" PageSize="50" width="100%"><AlternatingRowStyle CssClass="alternate" /><RowStyle CssClass="row" /><PagerStyle CssClass="row" HorizontalAlign="Right" /><Columns><asp:BoundField DataField="" HeaderText="" Visible="false" /><asp:BoundField DataField="BldrID" HeaderText="BldrID" /><asp:BoundField DataField="BuilderName" HeaderText="Bldr Name" /><asp:BoundField DataField="PurchVol" DataFormatString="{0:c}" HeaderText="PurchVol" /><asp:BoundField DataField="RebateRate" HeaderText="RebateRate" /><asp:BoundField DataField="AmountDue" DataFormatString="{0:c}" HeaderText="AmountDue" /></Columns>
                </CC:GridView>
            </asp:Panel>
         </ItemTemplate>
         </asp:TemplateField>
          <asp:TemplateField><HeaderTemplate><asp:CheckBox ID="ChkSelectAll" runat="server" onclick="CheckAll(this);" Text="Select All" /></HeaderTemplate>
                    <ItemTemplate><asp:CheckBox ID="ChkSelectRow" runat="server" onclick="Check_Click(this)" /></ItemTemplate>
                </asp:TemplateField>
          <asp:TemplateField HeaderText="Market"><ItemTemplate><asp:Literal ID="LLCName" runat="server"></asp:Literal></ItemTemplate></asp:TemplateField><asp:BoundField DataField="VendorHistoricID" HeaderText="VendorID" ItemStyle-Width="5%" SortExpression="VendorHistoricID"></asp:BoundField>
		  <asp:BoundField DataField="VendorName" HeaderText="VendorName" SortExpression="VendorName"></asp:BoundField>
         <asp:TemplateField HeaderText="AP Contact"><ItemTemplate><asp:Literal ID="apContactName" runat="server"></asp:Literal></ItemTemplate></asp:TemplateField><asp:TemplateField HeaderText="Email"><ItemTemplate><asp:Literal ID="apEmail" runat="server"></asp:Literal></ItemTemplate></asp:TemplateField><asp:TemplateField HeaderText="Phone"><ItemTemplate><asp:Literal ID="apPhone" runat="server"></asp:Literal></ItemTemplate></asp:TemplateField><%--<asp:TemplateField HeaderText = "Doc Type" >
            <ItemTemplate >
                <asp:Literal ID="apDocType" runat ="server" ></asp:Literal>
            </ItemTemplate>
         </asp:TemplateField>--%><asp:BoundField DataField="DocType" HeaderText="Doc Type" SortExpression="DocType"></asp:BoundField>
        <asp:BoundField DataField="invoice" HeaderText="Invoice" SortExpression="invoice"></asp:BoundField>
        <asp:BoundField DataField="PeriodQuarter" HeaderText="QTR" SortExpression="PeriodQuarter"></asp:BoundField>
        <asp:BoundField DataField="PeriodYear" HeaderText="YR" SortExpression="PeriodYear"></asp:BoundField>
        <asp:BoundField DataField="invoiceDate" DataFormatString="{0:d}" HeaderText="Date" SortExpression="invoiceDate"></asp:BoundField>
        <asp:BoundField DataField="DaysPastDue" HeaderText="Days" SortExpression="DaysPastDue"></asp:BoundField>
        <asp:BoundField DataField="PurchaseVolume" DataFormatString="{0:c}" HeaderText="Purch Vol" SortExpression="PurchaseVolume"></asp:BoundField>
        <asp:BoundField DataField="RebateRate" HeaderText="RebateRate" SortExpression="RebateRate"></asp:BoundField>
        <asp:BoundField DataField="AmountDue" DataFormatString="{0:c}" HeaderText="Due" SortExpression="AmountDue"></asp:BoundField>

        <asp:TemplateField HeaderText="Action" ShowHeader="True"><ItemTemplate><%--<asp:ImageButton runat="server" ID="BtnViewFileARRebate" ImageUrl="/images/admin/Preview.gif" CommandName="ViewFile" CommandArgument="<%# Container.DataItemIndex %>" OnClick="BtnViewFileARRebate_Click" />
                <asp:ImageButton runat="server" ID="BtnDownloadFileARRebate" ImageUrl="/images/admin/Download.png" CommandName="DownloadFile" CommandArgument="<%# Container.DataItemIndex %>" OnClick="BtnDownloadFileARRebate_Click" />--%>
                <asp:ImageButton ID="BtnViewFileARRebate" runat="server" CommandArgument="<%# Container.DataItemIndex %>" CommandName="ViewFile" ImageUrl="/images/admin/Preview.gif" /><asp:ImageButton ID="BtnDownloadFileARRebate" runat="server" CommandArgument="<%# Container.DataItemIndex %>" CommandName="DownloadFile" ImageUrl="/images/admin/Download.png" /></ItemTemplate>
        </asp:TemplateField>

	</Columns>
</CC:GridView>
</div>
         </div>
  
</CT:MasterPage>