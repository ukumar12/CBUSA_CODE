<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="admin_NationalContracts_Edit" %>

<asp:Content ContentPlaceHolderID="ph" ID="Content" runat="server">
    <script src="../../../includes/jquery/jquery.megaselectlist.js" type="text/javascript"></script>
    <style>
        .field
        {
            background: #E2EBF7;
            vertical-align: top;
            width: 588px;
            font-size: 1.3em;
        }

        .megaselectlistcolumn
        {
            width: 47%;
            margin: 0 1%;
            float: left;
        }

        .megaselectlist .currentitem
        {
            padding: 0;
            border: 0.1em dotted Green;
            color: Black;
        }

        /* Optional styles */

        .megaselectlist
        {
            background-color: #F5F5F5;
            border: 1px solid Silver;
            margin: 0 15px 0 15px;
        }

        .megaselectlistcolumn > h2
        {
            font-size: 1em;
            background-color: #ECECEC;
            text-align: center;
            padding: 2px 0;
            margin: 0;
        }

        .megaselectlist > p
        {
            margin: 1.2em;
            width: 308px;
            float: right;
        }

        .megaselectlistcolumn > ul
        {
            padding-left: 1em;
            margin: 0;
            color: Gray;
            height: auto;
            overflow: auto;
        }

            .megaselectlistcolumn > ul > li
            {
                cursor: pointer;
                padding: 0.1em;
                font-size: small;
            }

                .megaselectlistcolumn > ul > li:hover
                {
                    background-color: #bfcdec !important;
                    color: Black;
                }

        .megaselectlistoptions
        {
            width: 731px;
        }
    </style>
    <script type="text/javascript">
        
        $(function () {
            initMegaSelect();
            $(".submit").click(function () {
                $('#<%=hdnBuilders.ClientID %>').val($('#<%=drpbuilders.ClientID %>').val());
            });
        });
            function initMegaSelect() {
                $(".multiSelect").megaselectlist({
                    classmodifier: "megaselectlist",
                    headers: "rel",
                    animate: false,
                    animateevent: "click",
                    multiple: true,
                    itemseparator: "<br>",
                });

                $(".megaselectlistcolumn ul li:even").css("background", "#E5E5E5");
                $(".megaselectlistcolumn ul li:odd").css("background", "#ffffff");

                var selectedValues = $('#<%=hdnBuilders.ClientID %>').val()
                var spanText = "";
                //clear the list
                $('#megaselectlist1 p span').html('');
                //clear the selected options on load
                $('#<%=drpBuilders.ClientID %>').html('');
                var presentArr = [];
                $('.megaselectlistcolumn ul li').each(function () {
                    var thisID = $(this).attr('rel');
                    var arr = selectedValues.split(',');
                    if ($.inArray(thisID, arr) != -1) {
                        console.log(this);
                        presentArr.push(thisID);
                        $(this).addClass('currentitem');
                        $('#<%=drpBuilders.ClientID %>').append("<option value='" + thisID + "' selected='true'>" + $(this).text() + "</option>");
		               
                        spanText += '<br /> ' + $(this).text()  ;
                    }
                    else {
                        $(this).removeClass('currentitem');
                        var compareText = $(this).text();
                        var spanEl = $(this).parent().parent().parent().parent().find("span:contains('" + compareText + "')").text('');
                        $('#<%=drpBuilders.ClientID %> option').each(function () {
                            if ($(this).val() == thisID) {
                                $(this).remove();
                            }
                        });
                    }
                });
              
                var list = spanText.split('<br />');
                list.sort();
				spanText = "";
                list.forEach(function(entry) {
					spanText += entry + '<br />';
				});
                $('.builders .megaselectlist p span').append(spanText);
                $('#<%=hdnBuilders.ClientID %>').val(presentArr.join());
            }

    </script>

    <script type="text/javascript">
        $(document).ready(function () {
            $('.markerChecks label:contains(All)').siblings('input[type=checkbox]').click(function () {
                var toggle = $('.markerChecks label:contains(All)').siblings('input[type=checkbox]').get(0).checked;

                $('.markerChecks input').each(function () {
                    if (toggle != $(this).get(0).checked && $(this).siblings('label').text() != 'All')
                        $(this).click();
                });
            });
        });
    </script>



    <h4><% If ContractID = 0 Then%>Add<% Else%>Edit<% End If%> Contract</h4>
    <asp:HiddenField ID="hdnBuilders" runat="server" />

    <asp:ScriptManager runat="server" ID="sm" EnablePartialRendering="true" />
    <table border="0" cellspacing="1" cellpadding="2">
        <tr>
            <td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td>
        </tr>
        <tr>
            <td class="required">Title:</td>
            <td class="field">
                <asp:TextBox ID="txtTitle" runat="server" MaxLength="255" Columns="50" Style="width: 200px;"></asp:TextBox></td>
            <td>
                <asp:RequiredFieldValidator ID="rfvTitle" runat="server" Display="Dynamic" ControlToValidate="txtTitle" ErrorMessage="Field 'Title' is blank"></asp:RequiredFieldValidator></td>
        </tr>
        <tr>
            <td class="optional">CBUSA Landing Page:</td>
            <td class="field">
                <asp:TextBox ID="txtDetail" runat="server" MaxLength="1000" Columns="50" Style="width: 200px;"></asp:TextBox></td>
            <td></td>
        </tr>
        <tr>
            <td class="optional">Archive Contract: </td>
            <td class="field">
               <asp:RadioButtonList runat="server" ID="rblArchiveDate" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True"  />
			<asp:ListItem Text="No" Value="False" Selected="True"/>
			</asp:RadioButtonList></td>
            <td></td>
        </tr>
        <tr style="display :none">
            <td class="optional">Contract term: </td>
            <td class="field">
                <table border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td class="smaller">From
                            <CC:DatePicker ID="dpStartDate" runat="server" /></td>
                        <td>&nbsp;</td>
                        <td class="smaller">To 
                            <CC:DatePicker ID="dpEndDate" runat="server" /></td>
                    </tr>
                </table>
            </td>
            <td></td>
        </tr>

         <tr>
            <td class="optional">Contract term:</td>
            <td class="field">
                <asp:TextBox ID="txtContractTerm" runat="server" MaxLength="1000" Columns="50" Style="width: 200px;"></asp:TextBox></td>
            <td></td>
        </tr>


        <tr>
            <td class="optional">Manufacturer:</td>
            <td class="field">
                <asp:TextBox ID="txtManufacturer" runat="server" MaxLength="255" Columns="50" Style="width: 200px;"></asp:TextBox></td>
            <td></td>
        </tr>
        <tr>
            <td class="optional">Manufacturer website:</td>
            <td class="field">
                <asp:TextBox ID="txtManufacturerSite" runat="server" MaxLength="1000" Columns="50" Style="width: 200px;"></asp:TextBox></td>
            <td></td>
        </tr>
        <tr>
            <td class="optional">Products:</td>
            <td class="field">
                <asp:TextBox ID="txtProducts" TextMode="Multiline" runat="server" Columns="50" Style="width: 300px; height: 100px"></asp:TextBox></td>
            <td></td>
        </tr>
        <tr>
            <td class="optional">Market:</td>
            <td class="field">
                <asp:UpdatePanel ID="upMarkets" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <CC:CheckBoxListEx ID="cblLLC" CssClass="markerChecks" runat="server" RepeatColumns="3" AutoPostBack="true"></CC:CheckBoxListEx>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="optional">Builders:</td>
            <td class="field builders">
                <asp:UpdatePanel ID="upBuilders" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Label ID="lblBuilders" AssociatedControlID="drpBuilders" Text="Selected builders" runat="server"></asp:Label>
                        <CC:DropDownListEx ID="drpbuilders" runat="server" CssClass="multiSelect" multiple="true" Style="display: none;"></CC:DropDownListEx>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>

    <p></p>
    <CC:OneClickButton ID="btnSave" runat="server" Text="Save" CssClass="btn submit"></CC:OneClickButton>
    <CC:ConfirmButton ID="btnDelete" runat="server" Message="Are you sure want to delete this Order?" Text="Delete" CssClass="btn" CausesValidation="False"></CC:ConfirmButton>
    <CC:OneClickButton ID="btnCancel" runat="server" Text="Cancel" CssClass="btn" CausesValidation="False"></CC:OneClickButton>
</asp:Content>
