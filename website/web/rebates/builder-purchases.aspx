zgvvbbbbbbbbbvv,<%@ Page Language="VB" AutoEventWireup="false" CodeFile="builder-purchases.aspx.vb" Inherits="rebates_builder_purchases" %>

<CT:MasterPage ID="CTMain" runat="server"><asp:PlaceHolder runat="server"><script type="text/javascript">
    function UpdateVendors(ctl) {
        if(ctl.PostbackTimer) {
            window.clearTimeout(ctl.PostbackTimer);
        }
        $get('<%=hdnPostback.ClientID %>').value = ctl.value;
        ctl.PostbackTimer = window.setTimeout('<%=Page.ClientScript.GetPostBackEventReference(hdnPostback, "").Replace("'", "\'") %>',500);
    }

    function TogglePurchases(btn) {
        var hdn = $get(btn.id.replace('btnPurchases','hdnPurchasesState'));
        var div = $get(btn.id.replace('btnPurchases','divPurchases'));
        if(div.style.display=='none') {
            hdn.value = 'visible';
            $(div).slideDown('slow',null);    
            btn.value = 'Hide Purchases';
        } else {
            hdn.value = 'hidden';
            $(div).slideUp('slow',null);
            btn.value = 'Show Purchases';
        }
    }

    function ClearVendors() {
        $get('<%=txtVendorFilter.ClientID %>').value = '';

        UpdateVendors('<%=txtVendorFilter %>');
    }

    function OpenConfirm() {
        var Varhdnsurvey = document.getElementById("hdnPostSurvey").value;
	Varhdnsurvey = 'False';
        if (Varhdnsurvey == 'True') {
            //ShowSurveyDiv();
            //document.getElementById("PopupAreaEnable").style.display = 'block';
        } else {
            var tbl = $get('<%=tblConfirm.ClientID %>');
            var div = $get('<%=divConfirmResult.ClientID %>');
            var ctl = $get('<%=frmConfirm.ClientID %>').control;
            var btnOk = $get('<%=btnConfirmOk.ClientID %>');
            var btnCancel = $get('<%=btnConfirmCancel.ClientID %>');

            btnOk.style.display = '';
            btnCancel.style.display = '';
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

    function isNumber(evt) {
        evt = (evt) ? evt : window.event;

        var charCode = (evt.which) ? evt.which : evt.keyCode;
        if (charCode > 31 && (charCode < 48 || charCode > 57)) {
            return false;
        }
        return true;
    }

    function isNumberOrDecimal(evt) {
        evt = (evt) ? evt : window.event;

        var charCode = (evt.which) ? evt.which : evt.keyCode;

        if (charCode > 31 && (charCode < 48 || charCode > 57) && (charCode != 46)) {
            return false;
        }
        return true;
    }

    function ShowSurveyDiv() {

        var div = document.getElementById("divPerformanceSurvey");
        document.getElementById("divPerformanceSurvey").style.display = "block";

        if (document.getElementById("hdnCurrentQuarter").value == 4) {
            $('#divHomeStarts').removeClass("HomeStart-lft");
            document.getElementById('divQuarterlyHeader').style.display = 'none';
            document.getElementById('divYearlyHeader').style.display = 'block';

            var txtVal4 = document.getElementById('txtvalue4');
            var txtVal5 = document.getElementById('txtvalue5');
            
            if (txtVal4 != null && document.getElementById('txtvalue4').length > 0 && document.getElementById('txtvalue5').length > 0                                         // simplified condition for save button enable when right side voxes are hidden as requested in dec
                && document.getElementById('txtRevenueActual').length > 0 && document.getElementById('txtRevenueProj').length > 0
                && document.getElementById('txtStartActual').length > 0 && document.getElementById('txtStartProj').length > 0) {
                    EnableSaveButton();
                } else {
                    DisableSaveButton();
                }
        } else {
            if (document.getElementById('txtvalue4').length > 0 && document.getElementById('txtvalue5').length > 0) {
                EnableSaveButton();
            } else {
                DisableSaveButton();
            }
        }

        var hdnPerformanceSurveyId1 = document.getElementById('hdnPerformanceSurveyId1').value;
        var hdnPerformanceSurveyId2 = document.getElementById('hdnPerformanceSurveyId2').value;
        var hdnPerformanceSurveyId3 = document.getElementById('hdnPerformanceSurveyId3').value;
        var hdnPerformanceSurveyId4 = document.getElementById('hdnPerformanceSurveyId4').value;

        if (hdnPerformanceSurveyId1.length > 0) {
            var SurveydataValue1 = document.getElementById('txtvalue1').value;
            $('#txtvalue1').attr('disabled', true);

            var ProjectedValue1 = document.getElementById('ProjectedValue1').value;
            if ((SurveydataValue1.length > 0 && ProjectedValue1 == "False") || (SurveydataValue1.length < 0 && ProjectedValue1 == "")) {
                imgEdit1.style.display = 'block';
            } else {
                imgNotProjected1.style.display = 'block';
            }
        }

        if (hdnPerformanceSurveyId2.length > 0) {
            var SurveydataValue2 = document.getElementById('txtvalue2').value;
            $('#txtvalue2').attr('disabled', true);

            var ProjectedValue2 = document.getElementById('ProjectedValue2').value;
            if ((SurveydataValue2.length > 0 && ProjectedValue2 == "False") || (SurveydataValue2.length < 0 && ProjectedValue2 == "")) {
                imgEdit2.style.display = 'block';
            } else {
                document.getElementById("imgNotProjected2").style.display = "block";
            }
        } 

        if (hdnPerformanceSurveyId3.length > 0) {
            var SurveydataValue3 = document.getElementById('txtvalue3').value;         
            $('#txtvalue3').attr('disabled', true);

            var ProjectedValue3 = document.getElementById('ProjectedValue3').value;
            if ((SurveydataValue3.length > 0 && ProjectedValue3 == "False") || (SurveydataValue3.length < 0 && ProjectedValue3 == "")) {
                imgEdit3.style.display = 'block';
            } else {
                imgNotProjected3.style.display = 'block';
            }
        }
            
        if (hdnPerformanceSurveyId4.length > 0) {
            var dataValue4 = document.getElementById('txtvalue4').value;
            var projected4 = document.getElementById('ProjectedValue4').value;
            if (dataValue4.length > 0 && projected4 == "True") {
                imgSave4.style.display = 'block';
                imgSave4.style.color = 'gray';
            } else if (dataValue4.length < 0 && projected4 == "") {
                imgNotProjected4.style.display = 'block';
            } else if (dataValue4.length > 0 && projected4 == "False") {
                imgSave4.style.display = 'block';
                imgSave4.style.color = ' green';
                $('#txtvalue4').removeClass("blueborder");
                $('#txtvalue4').addClass("greenborder");
            }
        } else {
            var dataValue4 = document.getElementById('txtvalue4').value;
            if (dataValue4 != null && dataValue4.length > 0) {
                var date = new Date();
                var Month = moment().month()+1;

                imgSave4.style.display = 'block';
                imgSave4.style.color = ' green';

                $('#txtvalue4').removeClass("blueborder");
                $('#txtvalue4').addClass("greenborder");

                //document.getElementById('divModifiedDate4').innerHTML = 'Entered ' + Month + '/' + moment().date() + '/' + moment().year(); 
                //document.getElementById('divModifiedDate4').innerHTML = 'Entered with ' + $("#Year_Quarter4").text().replace(moment().year(), '').replace(moment().year() - 1, '') + ' Report'; 
            } else {
                imgNotProjected4.style.color = ' red';
                imgNotProjected4.style.display = 'block';
            }
        }

        var dataValue5 = document.getElementById('txtvalue5').value;
        if (dataValue5.length > 0) {
            var date = new Date();
            var Month = moment().month() + 1;

            imgProjectedSave.style.display = 'block';
            imgProjected.style.display = 'none';
            imgProjectedSave.style.color = ' green';

            $('#txtvalue5').removeClass("blueborder");
            $('#txtvalue5').addClass("greenborder");

            //document.getElementById('divProjected').innerHTML = 'Entered ' + Month + '/' + moment().date() + '/' + moment().year(); 
            //document.getElementById('divProjected').innerHTML = 'Estimated Q' + $("#hdnProjectedQuarter").val() + ' Starts'; 
        } else {
            imgProjectedSave.style.display = 'none';
            imgProjected.style.display = 'block';
        }
    }

    function ShowHideIcons() {

    }

    function EditSurvey(pCtrlIndex) {
        document.getElementById("imgEdit" + pCtrlIndex).style.display = 'none';
        document.getElementById("imgSave_Cancel" + pCtrlIndex).style.display = 'block';
        $('#txtvalue' + pCtrlIndex).attr('disabled', false);
        $('#divModifiedDate' + pCtrlIndex).toggle('hide');
    }

    function CancelEdit(pCtrlIndex) {
        document.getElementById("imgEdit" + pCtrlIndex).style.display = 'block';
        document.getElementById("imgSave_Cancel" + pCtrlIndex).style.display = 'none';
        $('#txtvalue' + pCtrlIndex).attr('disabled', true);
        $('#divModifiedDate' + pCtrlIndex).toggle('show');
        $('#txtvalue' + pCtrlIndex).val($('#SurveyValue' + pCtrlIndex).val());
    }

    function txtRequired() {        
        var Requiretxt4 = document.getElementById('txtvalue4').value;
        var Requiretxt5 = document.getElementById('txtvalue5').value;
        var ProjectedValue4 = document.getElementById('ProjectedValue4').value;

        var Month = moment().month() + 1;

        if (document.getElementById("hdnCurrentQuarter").value != 4) {
            if ((Requiretxt4.length > 0) && (Requiretxt5.length > 0)) {
                EnableSaveButton();
            }
        } else {
            var vartxtRevenueActual = document.getElementById('txtRevenueActual').value;
            var vartxtRevenueProj = document.getElementById('txtRevenueProj').value;
            var vartxtStartActual = document.getElementById('txtStartActual').value;
            var vartxtStartProj = document.getElementById('txtStartProj').value;

            if ((Requiretxt4.length > 0) && (Requiretxt5.length > 0) && (vartxtRevenueActual.length > 0)               //---*simplified condition for save button enable when right side voxes are hidden as requested in dec
                 && (vartxtRevenueProj.length > 0) && (vartxtStartActual.length > 0) && (vartxtStartProj.length > 0)) {
                EnableSaveButton();
            } else {
                if ((Requiretxt4.length == 0) || (Requiretxt5.length == 0) || (vartxtRevenueActual.length == 0)       //---*simplified condition for save button enable when right side boxes are hidden as requested in dec
                    || (vartxtRevenueProj.length == 0) || (vartxtStartActual.length == 0) || (vartxtStartProj.length == 0)) {

                    DisableSaveButton();
                }
            }
        }

        if (Requiretxt4.length > 0) {
            imgSave4.style.display = 'block';
            imgNotProjected4.style.display = 'none';
            imgSave4.style.color = 'green';
            //document.getElementById('divModifiedDate4').innerHTML = 'Entered ' + Month + '/' + moment().date()+ '/' + moment().year();
            //document.getElementById('divModifiedDate4').innerHTML = 'Entered with ' + $("#Year_Quarter4").text().replace(moment().year(), '').replace(moment().year() - 1, '') + ' Report';
            $('#txtvalue4').removeClass("blueborder").addClass("greenborder");

        } else {
            $('#txtvalue4').addClass("blueborder");
            DisableSaveButton();
            imgSave4.style.display = 'none';
            imgNotProjected4.style.display = 'block';
            //document.getElementById('divModifiedDate4').innerHTML = 'Enter Actual Starts';
        }

        if (Requiretxt5.length > 0) {
            $('#txtvalue5').removeClass("blueborder");
            $('#txtvalue5').addClass("greenborder");
            imgProjectedSave.style.display = 'block';
            imgProjectedSave.style.color = 'green';
            imgProjected.style.display = 'none';
            //document.getElementById('divProjected').innerHTML = 'Estimated Q' + $('#hdnProjectedQuarter').val() + ' Starts'; //'Entered ' + Month + '/' + moment().date()+ '/' + moment().year(); ;
        } else {
            $('#txtvalue5').addClass("blueborder");
            DisableSaveButton();
            imgProjectedSave.style.display = 'none';
            imgProjected.style.display = 'block';
            //document.getElementById('divProjected').innerHTML = 'Estimated Q' + $('#hdnProjectedQuarter').val() +  ' Starts';
        }
    }

    function EnableSaveButton() {
        $('#btnSavePerfromance').attr('disabled', false);
        $('#btnSavePerfromance').removeClass("btngray");
        $('#btnSavePerfromance').addClass("btnred");
    }

    function DisableSaveButton() {
        $('#btnSavePerfromance').attr('disabled', true);
        $('#btnSavePerfromance').removeClass("btnred");
        $('#btnSavePerfromance').addClass("btngray");
    }

    function SaveEdit() {

        var txtValue1 = document.getElementById('txtvalue1') != null?document.getElementById('txtvalue1').value:"";
        var txtValue2 = document.getElementById('txtvalue2') != null ?document.getElementById('txtvalue2').value:"";
        var txtValue3 = document.getElementById('txtvalue3') != null ?document.getElementById('txtvalue3').value:"";

        var surveydata1 = document.getElementById('SurveyValue1').value;
        var surveydata2 = document.getElementById('SurveyValue2').value;
        var surveydata3 = document.getElementById('SurveyValue3').value;

        if ((txtValue1 != surveydata1) || (txtValue2 != surveydata2) || (txtValue3 != surveydata3)) {
            //document.getElementById("skip").style.display = "none";
        }
        else {
            //document.getElementById("skip").style.display = "block";
        }

        if (txtValue1 != "") {
            $('#hdnNewSurveyValue1').val($('#txtvalue1').val());
            imgEdit1.style.display = 'block';
            imgSave_Cancel1.style.display = 'none'; 
            $('#txtvalue1').attr('disabled', true);
            document.getElementById("divModifiedDate1").style.display = "block";
        }

        if (txtValue2 != "") {
            $('#hdnNewSurveyValue2').val($('#txtvalue2').val());
            imgEdit2.style.display = 'block';
            imgSave_Cancel2.style.display = 'none';  
            $('#txtvalue2').attr('disabled', true);
            document.getElementById("divModifiedDate2").style.display = "block";
        }

        if (txtValue3 != "") {
            $('#hdnNewSurveyValue3').val($('#txtvalue3').val());
            imgEdit3.style.display = 'block';
            imgSave_Cancel3.style.display = 'none'; 
            $('#txtvalue3').attr('disabled', true);
            document.getElementById("divModifiedDate3").style.display = "block";
        }
    }

    function skipPrompt() {
        var promptText = "";

        var txtValue1 = document.getElementById('txtvalue1') != null ? document.getElementById('txtvalue1').value : "";
        var txtValue2 = document.getElementById('txtvalue2') != null ? document.getElementById('txtvalue2').value : "";
        var txtValue3 = document.getElementById('txtvalue3') != null ? document.getElementById('txtvalue3').value : "";

        var surveydata1 = document.getElementById('SurveyValue1').value;
        var surveydata2 = document.getElementById('SurveyValue2').value;
        var surveydata3 = document.getElementById('SurveyValue3').value;

        if ((txtValue1 != surveydata1) || (txtValue2 != surveydata2) || (txtValue3 != surveydata3)) {
            promptText = "You have changed the Home Starts value for one or more previous Quarters. Skipping temporarily will NOT SAVE these changes. Are you sure you want to skip temporarily?";
        } else {
            promptText = "You can skip this survey temporarily. However, you will be prompted every time you return to the screen, and you cannot submit your quarterly report until you have completed the survey.";
        }

        var result = confirm(promptText);
        if (result) {
            var div = document.getElementById("divPerformanceSurvey");
            document.getElementById("divPerformanceSurvey").style.display = "none"; 
            document.getElementById("PopupAreaEnable").style.display = 'none';
        }
    }

    $(document).ready(function () {      
        //var Varhdnsurvey = document.getElementById("hdnSurvey").value;
        //if (Varhdnsurvey == 'True') {
            //ShowSurveyDiv();
        //} else {
        //    document.getElementById("PopupAreaEnable").style.display = 'none';
        //}
    });

    </script>
    <script src="/includes/moment.js"></script>
    <style type="text/css">
        .windowPerformance {
            position: absolute;
            z-index: 10;
            background-color: #fff;
            border: 2px solid #c2c2c2;
            margin-bottom: 15px;
            margin-left: 259px;
            margin-top: 2px;
        }
    </style>
</asp:PlaceHolder>

<div style="display:none;"><CC:DatePicker ID="dpPlaceholder" runat="server" /></div>
    
<CC:PopupForm ID="frmConfirm" runat="server" Animate="false" CloseTriggerId="btnConfirmCancel" CssClass="pform" ErrorPlaceholderId="spanErrors" OpenMode="MoveToCenter" ShowVeil="true" style="width:400px;background-color:#e1e1e1" VeilCloses="false"><FormTemplate><div align="center" class="pckghdgred">Confirm Purchases Report</div><span id="spanErrors" runat="server"></span><p style="text-align:center;padding:5px"><input id="btnConfirmOk" runat="server" class="btnred" type="button" value="OK" /><input id="btnConfirmCancel" runat="server" class="btnred" type="button" value="Cancel" /></p>
        <div style="overflow:auto;"><table id="tblConfirm" runat="server" border="0" cellpadding="5" cellspacing="1" class="tblcompr" style="width:0px;margin:0px"><tr valign="top"><th>Unreported Vendors</th><th>Reported Vendors</th></tr><tr valign="top"><td><span id="ltlConfirmUnreported" runat="server"></span></td>
                    <td><span id="ltlConfirmReported" runat="server"></span></td>
                </tr>
            </table>
        </div>
        <div id="divConfirmResult" runat="server" style="display:none;"></div>
    </FormTemplate>
    <Buttons><CC:PopupFormButton ButtonType="Callback" ClientCallback="ReportConfirmCallback" ControlId="btnConfirmOk" /><CC:PopupFormButton ButtonType="ScriptOnly" ControlId="btnConfirmCancel" /></Buttons>
</CC:PopupForm>


<div class="slctwrpr" style="width:300px;margin:10px auto;text-align:center;background-color:#e1e1e1;margin-top:0px;"><div class="pckghdgred nobdr">1. Filter Vendor List<br /></div>
    <div><span class="smaller nopad">(List will filter as you type)</span> </div><asp:TextBox ID="txtVendorFilter" runat="server" onkeyup="UpdateVendors(this)"></asp:TextBox><a onclick="ClearVendors();" style="cursor:pointer;">Clear</a> </div><asp:HiddenField ID="hdnSurvey" runat="server" value="False"></asp:HiddenField>
    <asp:HiddenField ID="hdnPostSurvey" runat="server" value="False"></asp:HiddenField>
    <asp:HiddenField ID="hdnSurveyData" runat="server" value="False"></asp:HiddenField>
    <asp:HiddenField ID="hdnCurrentQuarter" runat="server" ></asp:HiddenField>
    <div class="PopupArea"><div id="PopupAreaEnable" style="display:none;" class="BackgroundDisable"></div>
        <div class="windowPerformance" id="divPerformanceSurvey" style="height:auto;display:none;width: auto;padding-bottom: 10px;"><div id="divQuarterlyHeader" class="pckghdgred" style="text-align:center;"><span id="SpnCurrentQuarter" runat="server"></span>Performance Survey</div><div id="divYearlyHeader" class="pckghdgred" style="text-align:center;display:none;">Quarterly And Yearly Performance Survey</div><div id="divHomeStarts" class="windowPerformance-lft HomeStart-lft"><table border="0" cellpadding="0" cellspacing="0" width="100%"><tr><td><table border="0" cellpadding="0" cellspacing="0" style="padding:10px" width="100%"><tr><th colspan="4" style="color: white">Home Starts</th></tr><tr><td width="60"><div id="Year_Quarter1" runat="server"></div></td>
                                <td width="70"><asp:TextBox ID="txtvalue1" runat="server" columns="8" maxlength="3" onkeypress="return isNumber(event)" style="text-align: center;display:none;"></asp:TextBox><asp:HiddenField ID="ProjectedValue1" runat="server"></asp:HiddenField>
                                    <asp:HiddenField ID="hdnPerformanceSurveyId1" runat="server"></asp:HiddenField>
                                    <asp:HiddenField ID="SurveyValue1" runat="server"></asp:HiddenField>
                                    <asp:HiddenField ID="hdnNewSurveyValue1" runat="server"></asp:HiddenField>
                                </td>
                                <td style="padding-left:5px;" valign="middle"><div id="imgEdit1" runat="server" onclick="EditSurvey(1)" style="display:none;cursor:pointer;"><i aria-hidden="true" class="fa fa-pencil graydarktext"></i>
                                    </div>
                                    <div id="imgSave_Cancel1" runat="server" style="display:none;cursor:pointer;"><a id="lnkSaveEdit1" runat="server" onclick="SaveEdit()"><i aria-hidden="true" class="fa fa-floppy-o greentext" style="padding-right:5px"></i></a>
                                        <a id="lnkCancelEdit1" onclick="CancelEdit(1)"><i aria-hidden="true" class="fa fa-ban redtext"></i></a>
                                    </div>
                                    <div id="imgNotProjected1" runat="server" style="display:none;"><i aria-hidden="true" class="fa fa-times-circle redtext"></i>
                                    </div>
                                </td>
                                <td><div id="divModifiedDate1" runat="server"></div> 
                                </td>
                            </tr>

                            <tr><td width="60"><div id="Year_Quarter2" runat="server"></div>
                                    <asp:HiddenField ID="hdnPerformanceSurveyId2" runat="server"></asp:HiddenField>
                                    <asp:HiddenField ID="SurveyValue2" runat="server"></asp:HiddenField>
                                    <asp:HiddenField ID="hdnNewSurveyValue2" runat="server"></asp:HiddenField>
                                </td>
                                <td><asp:TextBox ID="txtvalue2" runat="server" columns="8" maxlength="3" onkeypress="return isNumber(event)" style="text-align: center;display:none;"></asp:TextBox><asp:HiddenField ID="ProjectedValue2" runat="server"></asp:HiddenField>
                                </td>
                                <td style="padding-left:5px;" valign="middle"><div id="imgEdit2" runat="server" onclick="EditSurvey(2)" style="display:none;cursor:pointer;"><i aria-hidden="true" class="fa fa-pencil graydarktext"></i>
                                    </div>
                                    <div id="imgSave_Cancel2" runat="server" style="display:none;cursor:pointer;"><a id="lnkSaveEdit2" runat="server" onclick="SaveEdit()"><i aria-hidden="true" class="fa fa-floppy-o greentext" style="padding-right:5px"></i></a>
                                        <a id="lnkCancelEdit2" onclick="CancelEdit(2)"><i aria-hidden="true" class="fa fa-ban redtext"></i></a>
                                    </div>
                                    <div id="imgNotProjected2" runat="server" style="display:none;"><i aria-hidden="true" class="fa fa-times-circle redtext"></i>
                                    </div>
                                </td>
                                <td><div id="divModifiedDate2" runat="server"></div>
                                </td>
                            </tr>

                            <tr><td width="60"><div id="Year_Quarter3" runat="server"></div>
                                    <asp:HiddenField ID="hdnPerformanceSurveyId3" runat="server"></asp:HiddenField>
                                    <asp:HiddenField ID="SurveyValue3" runat="server"></asp:HiddenField>
                                    <asp:HiddenField ID="hdnNewSurveyValue3" runat="server"></asp:HiddenField>
                                </td>
    		                    <td><asp:TextBox ID="txtvalue3" runat="server" columns="8" maxlength="3" onkeypress="return isNumber(event)" style="text-align: center;display:none;"></asp:TextBox><asp:HiddenField ID="ProjectedValue3" runat="server"></asp:HiddenField>
    		                    </td>
                                <td style="padding-left:5px;" valign="middle"><div id="imgEdit3" runat="server" onclick="EditSurvey(3)" style="display:none;cursor:pointer;"><i aria-hidden="true" class="fa fa-pencil graydarktext"></i>
                                    </div>
                                    <div id="imgSave_Cancel3" runat="server" style="display:none;cursor:pointer;"><a id="lnkSaveEdit3" runat="server" onclick="SaveEdit()"><i aria-hidden="true" class="fa fa-floppy-o greentext" style="padding-right:5px"></i></a>
                                        <a id="lnkCancelEdit3" onclick="CancelEdit(3)"><i aria-hidden="true" class="fa fa-ban redtext"></i></a>
                                    </div>
                                    <div id="imgNotProjected3" runat="server" style="display:none;"><i aria-hidden="true" class="fa fa-times-circle redtext"></i>
                                    </div>
                                </td>
                                <td><div id="divModifiedDate3" runat="server"></div>
                                </td>
                            </tr>

                            <tr><td width="60"><div id="Year_Quarter4" runat="server"></div>
                                    <asp:HiddenField ID="hdnPerformanceSurveyId4" runat="server"></asp:HiddenField>
                                </td>
    		                    <td><asp:TextBox ID="txtvalue4" runat="server" class="blueborder" columns="8" maxlength="3" onkeypress="return isNumber(event)" onkeyup="return txtRequired()" style="text-align: center;display:none;"></asp:TextBox><asp:HiddenField ID="ProjectedValue4" runat="server"></asp:HiddenField>
    		                    </td>
                                <td style="padding-left:5px;" valign="middle"><div id="imgSave4" runat="server" style="display:none;"><i aria-hidden="true" class="fa fa-check-circle"></i>
                                    </div>
                                    <div id="imgNotProjected4" runat="server" style="display:none;"><i aria-hidden="true" class="fa fa-times-circle redtext"></i>
                                    </div>
                                </td>
                                <td><div id="divModifiedDate4" runat="server"></div>
                                </td>
                            </tr>

                            <tr><td colspan="4">&nbsp;</td></tr><tr><td colspan="4" style="border-bottom:2px solid #c2c2c2; padding:0px;"></td>
                            </tr>

                            <tr><td width="60"><div id="divProjectedYear" runat="server" style="text-align: left;"></div>
                                    <asp:HiddenField ID="hdnProjectedYear" runat="server"></asp:HiddenField>
                                    <asp:HiddenField ID="hdnProjectedQuarter" runat="server"></asp:HiddenField>
                                    <asp:HiddenField ID="hdnPerformanceSurveyId" runat="server"></asp:HiddenField>
                                </td>
                        	    <td><asp:TextBox ID="txtvalue5" runat="server" class="blueborder" columns="8" maxlength="3" onkeypress="return isNumber(event)" onkeyup="return txtRequired()" style="text-align: center;"></asp:TextBox></td><td style="padding-left:5px;" valign="middle"><div id="imgProjected" runat="server"><i aria-hidden="true" class="fa fa-question-circle-o"></i>
                                    </div>
                                    <div id="imgProjectedSave" runat="server"><i aria-hidden="true" class="fa fa-check-circle"></i>
                                    </div>
                                </td>
                                <td><div id="divProjected" runat="server"></div>
                                </td>
                            </tr>
	                    </table> 
                    </td>               
                </tr>
            </table>
        </div>
           
        <div id="divPerformance_Q4" runat="server" class="windowPerformance-rht" visible="false"><table border="0" cellpadding="0" cellspacing="0" style="padding:10px" width="100%"><tr><td><table border="0" cellpadding="0" cellspacing="0" width="100%"><tr><th colspan="3" style="color: white">Yearly Actuals And Projections</th></tr><tr style="display:none"><td width="70%"><label id="lblPercent" runat="server"></label>
                                </td>
                                <td align="right" width="30%"><input id="txtPercent" runat="server" maxlength="3" name="txtPercent" onkeypress="return isNumber(event)" onkeyup="return txtRequired()" size="8" style="text-align: center;" type="text"> <span><i aria-hidden="true" class="fa fa-percent"></i></span></input></td></tr><tr style="display:none"><td width="70%"><label id="lblURSquare" runat="server"></label>
                                </td>
                                <td align="right" width="30%"><input id="txtURSquare" runat="server" maxlength="3" name="txtURSquare" onkeypress="return isNumber(event)" onkeyup="return txtRequired()" size="8" style="text-align: center;" type="text"> </input></td></tr><tr style="display:none"><td width="70%"><label id="lblAvg" runat="server"></label>
                                    <span id="spnAvg" runat="server" style="font-size:10px; font-style:italic;">(excluding cost of land)</span> </td><td align="right" width="30%"><span><i aria-hidden="true" class="fa fa-usd"></i></span>
                                    <input id="txtAvg" runat="server" maxlength="3" name="txtAvg" onkeypress="return isNumber(event)" onkeyup="return txtRequired()" size="8" style="text-align: center;" type="text"> </input></td></tr><tr><td colspan="4">&nbsp;</td></tr><tr><td align="right" width="70%"><label id="lblActualYear" runat="server">2019</label> </td><td align="right" width="30%"><label id="lblProjYear" runat="server">2020</label> </td></tr><tr><td colspan="4" style="border-bottom:2px solid #c2c2c2; padding:0px;"></td>
                            </tr>
                            <tr><td colspan="4"><table border="0" cellpadding="0" cellspacing="0" class="bottom-table-area" width="100%"><tr><td align="left" width="50%">Gross Revenue(millions):</td><td align="left" width="5%"><span><i aria-hidden="true" class="fa fa-usd"></i></span></td>
                                            <td align="left" width="20%"><input id="txtRevenueActual" runat="server" maxlength="4" name="txtRevenueActual" onkeypress="return isNumberOrDecimal(event)" onkeyup="return txtRequired()" size="8" style="text-align: center;" type="text"> </input></td><td width="5%">&nbsp;</td><td width="5%"><span><i aria-hidden="true" class="fa fa-usd"></i></span></td>
                                            <td align="right" width="20%"><input id="txtRevenueProj" runat="server" maxlength="4" name="txtRevenueProj" onkeypress="return isNumberOrDecimal(event)" onkeyup="return txtRequired()" size="8" style="text-align: center;" type="text"> </input></td></tr></table></td></tr><tr><td colspan="4"><table border="0" cellpadding="0" cellspacing="0" class="bottom-table-area" width="100%"><tr><td align="left" width="50%">Total Starts:</td><td width="5%">&nbsp;</td><td align="left" width="20%"><input id="txtStartActual" runat="server" maxlength="3" name="txtStartActual" onkeypress="return isNumber(event)" onkeyup="return txtRequired()" size="8" style="text-align: center;" type="text"> </input></td><td align="left" width="5%"><span style="display:none" title="Calculated as the sum of your quarterly entries. You should edit this value if necessary."><i aria-hidden="true" class="fa fa-info-circle"></i></span></td>
                                            <td width="5%">&nbsp;</td><td align="right" width="25%"><input id="txtStartProj" runat="server" maxlength="3" name="txtStartProj" onkeypress="return isNumber(event)" onkeyup="return txtRequired()" size="8" style="text-align: center;" type="text"> </input></td></tr></table></td></tr></table></td></tr></table></div><div class="btnSaveSurvey"><asp:Button ID="btnSavePerfromance" runat="server" cssclass="btnred" style="text-align: center;" text="Save" /></div>
        <div id="skip" runat="server" class="skipClick" onclick="skipPrompt()" onmouseover="" style=" cursor: pointer;color: #0859c1;text-decoration-line: underline;position: absolute;right:5px;bottom: 10px;">Skip Temporarily</div></div></div><div class="pckggraywrpr"><div class="pckghdgred nobdr">2. Enter Purchases Data </div><asp:Literal id="ltrErrorMsg" runat="server"></asp:Literal><table border="0" cellpadding="0" cellspacing="1" class="tblcompr"><tr><th id="trLeftHeader" runat="server" align="center" style="width:40%;">Unreported Vendors </th><th align="center" style="width:60%;">Reported Vendors <div style="float:right; margin-top:-17px;"><asp:Button ID="btnHistory" runat="server" cssclass="btnred" text="Purchases Report History" /></div>
                </th>
            </tr>
            <tr><td id="trLeftColumn" runat="server" style="padding:5px;"><asp:UpdatePanel ID="upUnreported" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional"><ContentTemplate><asp:Literal ID="ltlUnreportedMsg" runat="server" /><table border="0" cellpadding="10" cellspacing="1"><tr><th>Vendor</th><th>Total Purchases</th><th><asp:Button ID="btnSaveAll" runat="server" cssclass="btnred" Text="Save All" /></th>
                                </tr>
                                <asp:Repeater ID="rptUnreported" runat="server"><ItemTemplate><tr class='<%#IIf(Container.ItemIndex Mod 2 = 1, "row", "alternate") %>'><td><b><asp:Literal ID="ltlVendor" runat="server"></asp:Literal></b></td><td><asp:TextBox ID="txtTotal" runat="server" Columns="8" maxlength="15"></asp:TextBox></td><td><asp:Button ID="btnSaveVendor" runat="server" CommandArgument='<%#Container.DataItem("VendorId") %>' cssclass="btnblue" text="Save" /></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
                <td><asp:UpdatePanel ID="upReported" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional"><ContentTemplate><div class="pckghdgblue center autoHeight"><b>View Report for:</b><br /> <asp:Literal ID="ltlCurrentQuarter" runat="server"></asp:Literal><asp:RadioButtonList ID="rblQuarter" runat="server" AutoPostback="true" RepeatDirection="Horizontal"></asp:RadioButtonList>
                                <!--<a href="purchases-history.aspx">All Past Reports</a>-->
                            </div>
                            <p id="pSubmit" runat="server" style="text-align:center;"><asp:Button ID="btnSubmit" style="color:white;background-color:#d27676" runat="server" cssclass="btnred" /><br /><asp:Literal ID="ltlDeadline" runat="server"></asp:Literal></p><asp:Literal ID="ltlReportedMsg" runat="server" /><table border="0" cellpadding="3" cellspacing="1" style="width:100%;"><tr><th>Vendor</th><th>Total Amount</th><th style="width:260px;">&#160;</th></tr><asp:Repeater ID="rptReported" runat="server"><ItemTemplate><tr class='<%#IIf(Container.ItemIndex Mod 2 = 1, "row", "alternate") %>'><td><b><%#DataBinder.Eval(Container.DataItem, "CompanyName")%></b></td>
                                            <td style="text-align:right;"><span id="spanTotal" runat="server"><%#FormatCurrency(DataBinder.Eval(Container.DataItem, "TotalAmount"))%><br /></span>
                                                <asp:TextBox ID="txtTotal" runat="server" columns="6" maxlength="8" text='<%#DataBinder.Eval(Container.DataItem,"TotalAmount") %>'></asp:TextBox><CC:FloatValidator ID="fvTotal" runat="server" ControlToValidate="txtTotal" ErrorMessage="Total Amount is invalid" ValidationGroup="<%#Container.UniqueID %>"></CC:FloatValidator>
                                                <asp:RequiredFieldValidator ID="rfvTotal" runat="server" ControlToValidate="txtTotal" ErrorMessage="Total Amount is required" ValidationGroup="<%#Container.UniqueID %>"></asp:RequiredFieldValidator></td><td style="text-align:right;"><asp:Button ID="btnEdit" runat="server" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"VendorID") %>' CommandName="Edit" cssclass="btnred" text="Edit" /><CC:ConfirmButton ID="btnDelete" runat="server" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"VendorID") %>' CommandName="Delete" cssclass="btnred" Message="Are you sure you want to delete this report?" Text="Delete" /><asp:Button ID="btnSave" runat="server" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"VendorID") %>' CommandName="Save" cssclass="btnred" text="Save" /><CC:ConfirmButton ID="btnCancel" runat="server" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "VendorID") %>' CommandName="Cancel" CssClass="btnred" Message="Are you sure you want to cancel changes?" Text="Cancel" /><input id="btnPurchases" runat="server" class="btnred" onclick="TogglePurchases(this);" type="button" value="Show Purchases" /></td>
                                        </tr>
                                        <tr><td class="bdrbottom" colspan="3" style="padding:0px;margin:0px;"><div id="divWarning" runat="server" class="red bold" visible="false"></div>
                                                <div id="divPurchases" runat="server"><b>Purchase Orders:</b> <asp:HiddenField ID="hdnPurchasesState" runat="server" enableviewstate="false"></asp:HiddenField>
                                                    <table border="0" cellpadding="3" cellspacing="1" width="100%"><tr><th>PO Amount</th><th>PO Number</th><th>Date of Purchase</th><th style="width:150px;">&#160;</th></tr><asp:PlaceHolder ID="phNoPurchases" runat="server"><tr><td colspan="4"><b>No matching purchase orders found</b></td></tr></asp:PlaceHolder><asp:Repeater ID="rptPurchases" runat="server"><ItemTemplate><tr><td style="text-align:right;"><asp:Literal ID="ltlAmount" runat="server" text='<%#FormatCurrency(DataBinder.Eval(Container.DataItem, "POAmount")) %>'></asp:Literal><asp:TextBox ID="txtAmount" runat="server" text='<%#FormatNumber(DataBinder.Eval(Container.DataItem, "POAmount"), 2) %>'></asp:TextBox><br /><asp:RequiredFieldValidator ID="rfvAmount" runat="server" ControlToValidate="txtAmount" ErrorMessage="Amount is required" ValidationGroup="<%# Container.UniqueId %>"></asp:RequiredFieldValidator><CC:CurrencyValidator ID="fvAmount" runat="server" ControlToValidate="txtAmount" ErrorMessage="Amount is invalid" ValidationGroup="<%# Container.UniqueId %>"></CC:CurrencyValidator>
                                                                    </td>
                                                                    <td style="text-align:right;"><asp:Literal ID="ltlNumber" runat="server" text='<%#DataBinder.Eval(Container.DataItem, "PONumber") %>'></asp:Literal><asp:TextBox ID="txtNumber" runat="server" text='<%#DataBinder.Eval(Container.DataItem, "PONumber") %>'></asp:TextBox></td><td style="text-align:right;"><asp:Literal ID="ltlDate" runat="server"></asp:Literal><CC:DatePicker ID="dpDate" runat="server"></CC:DatePicker>
                                                                        <CC:DateValidator ID="dvDate" runat="server" ControlToValidate="dpDate" ErrorMessage="Purchase date is invalid" MinDate='<%# (((ReportQuarter - 1) * 3 + 1) & "/1/" & ReportYear) %>' ValidationGroup="<%# Container.UniqueId %>"></CC:DateValidator>
                                                                    </td>
                                                                    <td style="text-align:right;"><asp:Button ID="btnEdit" runat="server" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "PurchasesReportVendorPOID") %>' CommandName="Edit" cssClass="btnblue" text="Edit" /><CC:ConfirmButton ID="btnDelete" runat="server" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "PurchasesReportVendorPOID") %>' CommandName="Delete" cssClass="btnblue" Message="Are you sure you want to delete this invoice?" Text="Delete" /><asp:Button ID="btnSave" runat="server" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "PurchasesReportVendorPOID") %>' CommandName="Save" cssClass="btnblue" text="Save" /><CC:ConfirmButton ID="btnCancel" runat="server" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "PurchasesReportVendorPOID") %>' CommandName="Cancel" cssClass="btnblue" Message="Any changes will be lost.  Continue?" text="Cancel" /></td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </table>
                                                    <asp:Button ID="btnAddPurchase" runat="server" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "VendorID") %>' CommandName="AddPurchase" cssclass="btnred" text="Add Purchase" /><asp:Button ID="btnImport" runat="server" cssclass="btnred" data-vendorid='<%#DataBinder.Eval(Container.DataItem,"VendorID") %>' onclientclick="javascript:return OpenUploadPO(this);" Text="Import Purchase"></asp:Button>
					                            </div>&#160; </td></tr></ItemTemplate><footertemplate><tr><td><b>Total:</b></td><td style="text-align:right;"><b><%=FormatCurrency(TotalAmount)%></b></td>
                                        </tr>
                                    </footertemplate>
                                </asp:Repeater>
                            </table>
                        </ContentTemplate>
                        <Triggers><asp:AsyncPostbackTrigger ControlId="rblQuarter"></asp:AsyncPostbackTrigger>
                        <asp:PostbackTrigger ControlId="btnSubmit"></asp:PostbackTrigger>
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
</div>

<asp:Panel ID="pnlPrint" runat="server"><div class="pckggraywrpr" style="text-align: center;"><input class="btnred" onclick="window.print();" type="button" value="Print This Page" /></div>    
</asp:Panel>
<asp:HiddenField ID="hdnPostback" runat="server"></asp:HiddenField>

<div id="divProductQty" runat="server" class="window" style="visibility:hidden;width:350px;"><div class="pckggraywrpr" style="margin-bottom:0px"><div class="pckghdgred">Upload PO </div><div style="margin-left:10px;margin-right:10px"><br />
            <p>Use this form to upload your purchase orders from a csv file. </p><p><a href="POTemplate.csv" target="_blank">Get PO Template</a> <span class="smallest">(Uploaded file must match the template exactly including the header line and the sheet name)</span> </p><center><CC:FileUpload ID="fulDocument" runat="server" /><br /><br />
                <CC:OneClickButton ID="btnImportProduct" runat="server" cssclass="btnred" OnClick="btnImportProduct_Click" Text="Import CSV" /><asp:Button ID="btnCancelImport" runat="server" CausesValidation="false" cssclass="btnred" Text="Cancel" /><asp:HiddenField ID="hdnVendorID" runat="server"></asp:HiddenField>
                <br />
            </center>
        </div>
    </div>
    <CC:DivWindow ID="ctrlProductUpload" runat="server" TargetControlID="divProductQty" CloseTriggerId="btnCancelImport" ShowVeil="true" VeilCloses="false" /><script type="text/javascript">
        function OpenUploadPO(ctrl) {
            $("#hdnVendorID").val('');
            var VendorId = ctrl.getAttribute('data-vendorid');
            $("#hdnVendorID").val(VendorId);
            //  alert($("#hdnVendorID").val());

            var b = Sys.UI.Behavior.getBehaviorByName($get('<%=ctrlProductUpload.BehaviorId %>'), '<%=ctrlProductUpload.BehaviorName %>');
            b.moveToCenter();
            b.startFadeIn();
            return false;
        }
    </script>
</div>
</CT:MasterPage>
