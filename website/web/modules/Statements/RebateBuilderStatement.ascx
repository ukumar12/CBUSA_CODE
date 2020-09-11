<%@ Control Language="VB" AutoEventWireup="false" CodeFile="RebateBuilderStatement.ascx.vb"
    Inherits="modules_Statements_RebateBuilderStatement" %>
 
<script type="text/javascript">
//    $(document).ready(function () {
//        $('input.datepicker-control').datepicker({
//        changeMonth: true,
//        changeYear: true,
//        buttonImage: '/images/calendar/picker.gif',
//        onChangeMonthYear: function (year, month, datePicker) {
//         
//            var $t = $(this);
//            var day = $t.data('preferred-day') || new Date().getDate();
//            var newDate = new Date(month + '/' + day + '/' + year);
//            while (day > 28) {
//                if (newDate.getDate() == day && newDate.getMonth() + 1 == month && newDate.getFullYear() == year) {
//                    break;
//                } else {
//                    day -= 1;
//                    newDate = new Date(month + '/' + day + '/' + year);
//                }
//            }
//            $t.datepicker('setDate', newDate);
//        },

//        beforeShow: function (dateText, datePicker) {
//            // Record the starting date so that
//            // if the user changes months from Jan->Feb->Mar
//            // and the original date was 1/31,
//            // then we go 1/31->2/28->3/31.
//            $(this).data('preferred-day', ($(this).datepicker('getDate') || new Date()).getDate());
//        }
//    });
//});
//
    
    function CalcDate() {
        //do nothing
    }

    function GetRebateLineDetails(builderId, vendorId, VendorName, DisplayReportingYearQtr, DateFrom, DateTo, OrigDocNumber, RowNumber) {
        $.ajax({
            type: "POST",
            async: false,
            url: "/builder/StatementService.aspx/GetRebateLineDetails",
            data: '{BuilderId: "' + new String(builderId.trim()) + '",VendorId: "' + new String(vendorId.trim()) + '",VendorName: "' + new String(VendorName.trim()) + '",DisplayReportingYearQtr: "' + new String(DisplayReportingYearQtr.trim()) + '",DateFrom: "' + new String(DateFrom.trim()) + '",DateTo: "' + new String(DateTo.trim()) + '",OrigDocNumber: "' + new String(OrigDocNumber.trim()) + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                //alert(response.d);
                OnSuccess(response.d, RowNumber);
            },
            failure: function (error) {
                alert(error.d);
            }
        });
    }
    function OnSuccess(response, RowNumber) {
       $("#divRebateLineDetail-" + RowNumber.trim()).html(response);
        $("#divRebateLineDetail-" + RowNumber.trim()).slideToggle();
    }

    $(document).ready(function () {
        $(".rebateLineDetail").delegate(".rebateStatementClose", "click", function () {
            $(this).parent().slideUp();
        });


    });
     
</script>
    <%--<asp:button id ="btnDashBoard" text="Go to DashBoard" class="btnred" runat="server" />--%>
<div class="rebateStatementWrpr" style="margin-top:10px;">
<div class="arLink">
		<a href="javascript:window.print()" class="printlink">Print</a>
		 
        <asp:LinkButton ID="lnkExport" runat ="server" >Export to Excel</asp:LinkButton>
		<a href="/rebates/rebate-notification.aspx">View Rebate AR Report</a>		
	</div>
<table cellpadding="0" cellspacing="0" class="rebateStatementHdr">
    <tr>
			<td><strong>Program</strong></td>
			<td> <asp:DropDownList   ID="ddlPrograms" runat="server">                    
                    </asp:DropDownList></td>
                    
		</tr>
    <tr>
        <td>
            <strong>From</strong>
        </td>
        <td>
            <CC:DatePicker ID="dpDateLbound" onChangeMonthYear="true" runat="server" />
        </td>
        <td>
         <CC:RequiredDateValidator ID="reqDateValidator1" ControlToValidate="dpDateLbound" ErrorMessage="From Date is blank." runat="server" Display="None" />
	   
        </td>
        <td>
         <CC:DateValidator ID="DateValidator1" MinDate ="11/1/2015" ControlToValidate="dpDateLbound" ErrorMessage="From Date must be greater than 11/1/2015." runat="server" Display="None" />
        </td>
    </tr>
    <tr>
        <td>
            <strong>To</strong>
        </td>
        <td>
            <CC:DatePicker ID="dpDateUbound" onChangeMonthYear="true" runat="server" />
        </td>
        <td>
        <td>
         <CC:RequiredDateValidator ID="RequiredDateValidator1" ControlToValidate="dpDateUbound" ErrorMessage="To Date is blank." runat="server" Display="None"  />
	    <CC:DateValidator ID="DateValidator2" MinDate ="11/1/2015" ControlToValidate="dpDateUbound" ErrorMessage="To Date is invalid." runat="server" Display="None"  />
        <asp:CustomValidator ID="cvDate" runat="server" ErrorMessage="To Date must be greater than From Date." Display ="None"></asp:CustomValidator> 
        </td>
        </td>
    </tr>
    <tr>
        <td>
            <strong>Search</strong>
        </td>
        <td>
            <CC:OneClickButton ID="btnSubmit" CssClass="btnred" Text="Search" runat="server" />
        </td>
    </tr>
</table>

<div class="rebateStatement">

<div class="rebateRowhdr rebateHdr">
                <div class="rebateDet rebateDate">
                    Date
                </div>
                <div class="rebateDet columnSortable rebateType">
                    <span class="sortArrow"></span>Transaction Type
                    <asp:DropDownList ID="ddlTransactionType" runat="server"  OnSelectedIndexChanged="TransactionTypeChanged"
                        AutoPostBack="true" AppendDataBoundItems="true" >
                        <asp:ListItem Text="ALL" Value=""></asp:ListItem>
                        <asp:ListItem Text="Volume Fee" Value="Volume Fee"></asp:ListItem>
                        <asp:ListItem Text="Rebate Pmt" Value="Rebate Pmt"></asp:ListItem>
                        <asp:ListItem Text="Distribution" Value="Distribution"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="rebateDet columnSortable rebateYear">
                    <span class="sortArrow"></span>Year Qtr
                    <asp:DropDownList ID="ddlReportingYearQtr"  OnSelectedIndexChanged ="ReportingYearQtrChanged" runat="server" AutoPostBack="true">
                    
                    </asp:DropDownList>
                </div>
                <div class="rebateDet columnSortable rebateName">
                    <span class="sortArrow"></span>Name
                    <asp:DropDownList ID="ddlVendorName"    OnSelectedIndexChanged ="VendorNameChanged" runat="server" AutoPostBack="true">
                    
                    </asp:DropDownList>
                </div>
                <div class="rebateDet rebatePV">
                    PV
                </div>
                <div class="rebateDet rebatePVFee">
                    PV Fee Rate
                </div>
                <div class="rebateDet rebateAmount">
                    Amount
                </div>
                <div class="rebateDet rebateBalance">
                    Balance
                </div>
                <div class="clear">
                </div>
            </div>
    <asp:Repeater ID="rptList" runat="server">
        <HeaderTemplate>
            
        </HeaderTemplate>
        <ItemTemplate>
            <div class='<%#IIf(Container.ItemIndex Mod 2 = 1, "rebateRow rebateRowAlt", "rebateRow") %>'>
                <div class="rebateDet rebateDate">
                <asp:Literal ID="ltlDocumentDate" runat ="server" />
                    <%# FormatDateTime(Eval("DisplayDate"), DateFormat.ShortDate)%>
                </div>
                <div class="rebateDet columnSortable rebateType">
                    <%# Eval("TransactionType")%>
                </div>
                <div class="rebateDet columnSortable rebateYear">
                   <asp:Literal ID="ltlDisplayReportingYearQtr" runat ="server" ></asp:Literal>
                </div>
                <div class="rebateDet columnSortable rebateName">
                    <span class="vendorName" runat="server" id="spnVendorName"  >
                        <asp:Literal ID="ltlVendorName" runat ="server" ></asp:Literal></span>
                </div>
                <div class="rebateDet rebatePV">
                 
                  <asp:Literal ID="ltlPurchaseVolume" runat ="server" ></asp:Literal>
                   </div>
                <div class="rebateDet rebatePVFee">
                <asp:Literal ID="ltlPVFeeRate" runat ="server" ></asp:Literal>
                  
                </div>
                <div class="rebateDet rebateAmount">
                   <asp:Literal ID="ltlAmount" runat ="server" ></asp:Literal>
                </div>
                <div class="rebateDet rebateBalance">
                   <asp:Literal ID="ltlBalance" runat ="server" ></asp:Literal>
                </div>
                <div class="clear">
                </div>
                <div class="rebateLineDetail" id='<%# String.Concat("divRebateLineDetail-", Convert.ToString(Eval("RowNumber")).Trim) %>'>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
 
 </div>
 