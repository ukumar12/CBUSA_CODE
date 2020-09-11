<%@ Page Language="VB" AutoEventWireup="false" CodeFile="survey.aspx.vb" Inherits="homestarts_survey" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>


    <form id="form1" runat="server">
        <asp:ScriptManager ID="AjaxManager" runat="server"></asp:ScriptManager>
        <asp:UpdatePanel runat="server">
            <ContentTemplate>
                <table style="width: 100%">
                    <tr>
                        <td style=""></td>
                        <td style="width: 45%; text-align: center;">
                            <img src="img/cbusa-emailHeader.jpg" />
                            <hr />
                        </td>
                        <td style=""></td>
                    </tr>
                    <tr style="height: 450px;">
                        <td style=""></td>
                        <td style="width: 45%;" valign="top">
                            <table style="width: 100%">
                                <tr>
                                    <td colspan="3">
                                        <table style="width: 100%;">
                                            <tr>
                                                <td align="left">
                                                    <asp:Label runat="server" ID="lblCompanyName" CssClass="info" Text=""></asp:Label></td>
                                                <td align="right">
                                                    <asp:Label runat="server" ID="lblUserName" CssClass="info" Text=""></asp:Label></td>

                                            </tr>
                                        </table>
                                    </td>
                                </tr>

                                <tr style="height: 25px;">
                                    <td colspan="3"></td>

                                </tr>
                                <tr>

                                    <td colspan="3" style="width: 100%; text-align: center"><span class="survey-header">Monthly Home Starts Survey</span></td>
                                </tr>
                                <tr style="height: 25px;">
                                    <td colspan="3"></td>

                                </tr>
                                <tr>

                                    <td style="width: 100%;" colspan="3">
                                        <table style="width: 100%">
                                            <tr style="height: 30px;">
                                                <td align="center" valign="top">
                                                    <table width="100%">
                                                        <tr>
                                                            <td>
                                                                <b>
                                                                    <asp:Label runat="server" ID="lblActualStart" CssClass="info" Text=""></asp:Label></b> Actual Starts</td>
                                                            <td>
                                                                <asp:TextBox TextMode="Number" ID="txtActualStart" onchange="enableSubmitButton()" runat="server" Width="60px" Font-Size="Medium"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2" align="center">

                                                                <asp:RequiredFieldValidator ID="rfvActualStart" runat="server" CssClass="error" Display="Dynamic" ValidationGroup="survey" ControlToValidate="txtActualStart" ErrorMessage="Enter Actual Start." SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                                <asp:RangeValidator runat="server" Type="Integer" ControlToValidate="txtActualStart" Display="Dynamic" ValidationGroup="survey" CssClass="error" ErrorMessage="Enter Number between 0 to 45" MinimumValue="0" MaximumValue="45" SetFocusOnError="true"></asp:RangeValidator>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td valign="top" align="center">
                                                    <table width="100%">
                                                        <tr>
                                                            <td>
                                                                <b>
                                                                    <asp:Label runat="server" ID="lblProjectedStart" CssClass="info" Text=""></asp:Label></b> Projected Starts
                                                            </td>
                                                            <td>
                                                                <td>
                                                                    <asp:TextBox TextMode="Number" ID="txtProjectedStart" onchange="enableSubmitButton()" runat="server" Width="60px" Font-Size="Medium"></asp:TextBox></td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2" align="center">
                                                                <asp:RequiredFieldValidator ID="rfvProjectedStart" runat="server" CssClass="error" Display="Dynamic" ValidationGroup="survey" ControlToValidate="txtProjectedStart" ErrorMessage="Enter Projected Start." SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                                <asp:RangeValidator runat="server" Type="Integer" ControlToValidate="txtProjectedStart" Display="Dynamic" ValidationGroup="survey" CssClass="error" ErrorMessage="Enter Number between 0 to 45" MinimumValue="0" MaximumValue="45" SetFocusOnError="true"></asp:RangeValidator>
                                                            </td>
                                                        </tr>
                                                    </table>

                                                </td>
                                            </tr>
                                            <tr id="trNetPromoter" style="height: 30px;" runat="server" visible="false">
                                                <td align="center" colspan="2">

                                                    <div id="dvNetPromoter" style="margin-top: 20px;">
                                                        <label class="NetPromoter_Header" style="margin: 5px 0px 5px 5px; float: left; color: #0e2d50; clear: both;">On a scale from 0 to 10, how likely are you to recommend CBUSA to another builder?</label><br />
                                                        <div class="NetPromoterGroup">
                                                            <div class="tooltip">
                                                                <span class="tooltiptext">Not At All Likely</span>
                                                                <asp:RadioButton runat="server" ID="rb_0" GroupName="NetPromoter" Value="0" Text="0" />

                                                            </div>
                                                            <div>
                                                                <asp:RadioButton runat="server" ID="rb_1" GroupName="NetPromoter" Value="1" Text="1" />
                                                            </div>
                                                            <div>
                                                                <asp:RadioButton runat="server" ID="rb_2" GroupName="NetPromoter" Value="2" Text="2" />
                                                            </div>
                                                            <div>
                                                                <asp:RadioButton runat="server" ID="rb_3" GroupName="NetPromoter" Value="3" Text="3" />
                                                            </div>
                                                            <div>
                                                                <asp:RadioButton runat="server" ID="rb_4" GroupName="NetPromoter" Value="4" Text="4" />
                                                            </div>
                                                            <div>
                                                                <asp:RadioButton runat="server" ID="rb_5" GroupName="NetPromoter" Value="5" Text="5" />
                                                            </div>
                                                            <div>
                                                                <asp:RadioButton runat="server" ID="rb_6" GroupName="NetPromoter" Value="6" Text="6" />
                                                            </div>
                                                            <div>
                                                                <asp:RadioButton runat="server" ID="rb_7" GroupName="NetPromoter" Value="7" Text="7" />
                                                            </div>
                                                            <div>
                                                                <asp:RadioButton runat="server" ID="rb_8" GroupName="NetPromoter" Value="8" Text="8" />
                                                            </div>
                                                            <div>
                                                                <asp:RadioButton runat="server" ID="rb_9" GroupName="NetPromoter" Value="9" Text="9" />
                                                            </div>
                                                            <div class="tooltip">
                                                                <span class="tooltiptext">Extremely Likely</span>
                                                                <asp:RadioButton runat="server" ID="rb_10" GroupName="NetPromoter" Value="10" Text="10" />
                                                            </div>
                                                            <div style="float: left; width: 80px; margin-left: 20px; clear: both;"><span>Not At All Likely</span></div>
                                                            <div style="float: right; width: 100px;"><span>Extremely Likely</span></div>

                                                            <asp:CustomValidator ID="cvNetPromoter" runat="server" Display="Dynamic" ErrorMessage="please choose number from scale!" ClientValidationFunction="NetPromoter_ClientValidate" CssClass="error" SetFocusOnError="true" ValidationGroup="survey" OnServerValidate="NetPromoter_ServerValidate"></asp:CustomValidator>
                                                        </div>
                                                    </div>
                                                </td>

                                            </tr>
                                            <tr style="height: auto; max-height:50px;">
                                                <td colspan="2" align="center" valign="bottom">
                                                    <asp:Button runat="server" CssClass="submit-btn" ID="btnSubmitSurvey" OnClick="btnSubmitSurvey_Click" CausesValidation="true" ValidationGroup="survey" Enabled="false" Text="SUBMIT MY STARTS" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                             
                                <tr>
                                    <td colspan="3">
                                        <asp:Panel runat="server" ID="pnlAlreadySubmittedSurvayData" Visible="false">
                                            <div class="message">
                                                Thank you for visiting! Somebody from your company has already submitted your home starts. 
                                                            You can review the results for your market and the entire CBUSA network by clicking <a href="survey_report.aspx?<%= sReportUrl %>">here</a>.
                                            </div>
                                        </asp:Panel>

                                        <asp:Panel runat="server" ID="pnlException" Visible="false">
                                            <div class="message">
                                                <asp:Literal runat="server" ID="ltrlException"></asp:Literal>
                                                <%--An error occurred, and your data couldn't be saved. Please try again. If the problem persists, please contact <a href="mailto:customerservice@cbusa.us">customerservice@cbusa.us</a>.--%>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel runat="server" ID="pnlSuccess" Visible="false">
                                            <div class="message">
                                                Thank you for your response!  You can review the results for your market and the entire CBUSA network by clicking <a href="survey_report.aspx?<%= sReportUrl %>">here</a>.

                                            </div>
                                        </asp:Panel>
                                        <asp:Panel ID="pnlBestPersion" runat="server" Visible="false" Style="padding: 10px; background-color: #d7dadb;">
                                            <table style="width:100%;">
                                                <tr>
                                                    <td>Are you the person who's qualified to provide this information?<br /><br />
                                                    </td>

                                                </tr>

                                                <tr>
                                                    <td>&nbsp;&nbsp;&nbsp;&nbsp;<asp:RadioButton runat="server" Text="Yes" value="Y" Checked="true" AutoPostBack="true" OnCheckedChanged="rbBestPersion_CheckedChanged" GroupName="grpBest" ID="rbBestPersionY" /><br />
                                                        &nbsp;&nbsp;&nbsp;&nbsp;<asp:RadioButton runat="server" Text="No. Please don't send me these home-starts surveys." AutoPostBack="true" value="N" GroupName="grpBest" OnCheckedChanged="rbBestPersion_CheckedChanged" ID="rbBestPersionN" />
                                                        <br />
                                                        <br />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Here is the person in the company who would be better suited to provide the information.
                                                    <br />
                                                        <br />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:DropDownList runat="server" ValidationGroup="bestPerson" ID="ddlBestPersons" Height="30" style="-webkit-appearance: menulist-button;line-height:30px !important;">
                                                        </asp:DropDownList>
                                                        <asp:RequiredFieldValidator ID="rfvBestPersons" runat="server" CssClass="error" Display="Dynamic" ValidationGroup="bestPerson" ControlToValidate="ddlBestPersons" InitialValue="-1" ErrorMessage="Please select the person from the list who's best qualified to provide this information." SetFocusOnError="true"></asp:RequiredFieldValidator>

                                                        <br />
                                                        <br />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <div style="text-align: center;">
                                                            <asp:Button runat="server" CssClass="update-btn" ID="btnUpdate" OnClick="btnUpdate_Click" CausesValidation="true" ValidationGroup="bestPerson" Text="UPDATE" />
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>


                                        </asp:Panel>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style=""></td>
                    </tr>



                    <tr>
                        <td style=""></td>
                        <td style="width: 45%; text-align: center;">
                            <%--<img src="img/cbusa-emailFooter.jpg" />--%>
                            <div class="footer">
                                <span>WWW.CBUSA.US</span>
                            </div>
                        </td>
                        <td style=""></td>

                    </tr>

                </table>
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="btnUpdate" />
                <asp:PostBackTrigger ControlID="rbBestPersionY" />
                <asp:PostBackTrigger ControlID="rbBestPersionN" />
            </Triggers>
        </asp:UpdatePanel>
    </form>
</body>
</html>
<style>
    html {
        font-family: Arial,Helvetica,Verdana,sans-serif;
    }

    .container {
    }

    hr {
        padding: 2px 20px;
        background-color: #ef0b35;
        border: none;
    }

    .survey-header {
        color: #0e2d50;
        font-size: 25px;
        font-weight: bold;
        text-align: center;
        /*margin-left: -100px;*/
    }

    .info {
        font-weight: 600;
        color: #656263;
        font-size: 18px;
    }

    .submit-btn, .update-btn {
        background-color: #a61d21;
        border: none;
        color: white;
        text-align: center;
        text-decoration: none;
        display: inline-block;
        font-size: 13px;
        margin: 4px 2px 4px 2px;
        cursor: pointer;
        padding: 10px;
        border-radius: 10px;
    }

    .NetPromoterGroup {
        padding: 15px;
        text-align: center;
        float: left;
        border: 1px dotted #D7D7D7;
        border-radius: 10px;
        width: 96%;
    }

    .NetPromoter_Header {
        color: #0e2d50;
        font-size: 14px;
        font-weight: bold;
        text-align: center;
    }

    .NetPromoterGroup div {
        float: left;
        padding: 10px 0px 0px 0px;
        width: 9%;
    }

    .NetPromoterGroup input[type=radio] {
        display: block;
        margin: 0 auto;
        width: 20px;
        height: 20px;
    }

    .NetPromoterGroup label {
        display: inline-block;
        font-size: 12px;
    }

    .NetPromoterGroup span {
        margin: 0px 0px 0px -5px;
        font-size: 12px;
    }

    .submit-btn:disabled, .update-btn:disabled {
        background-color: #D7D7D7;
        border: none;
        color: white;
        text-align: center;
        text-decoration: none;
        display: inline-block;
        font-size: 13px;
        margin: 2px 2px 4px -20px;
        cursor: pointer;
        padding: 10px;
        border-radius: 10px;
        cursor: default;
    }

    input[type='text'], input[type='number'] {
        text-align: center;
        padding: 3px;
    }

    .message {
        padding: 10px;
        line-height: 20px;
        color: #656263;
        font-size: 16px;
        font-weight: 50;
    }

    .messag a, message a {
        color: #AFE7FB;
    }

    select {
        width: 100%;
        padding: 7px;
    }

    .error {
        color: red;
        font-size: 12px;
    }

    .footer {
        background-color: #0e2d50;
        width: 100%;
        height: 80px;
    }

        .footer span {
            text-align: center;
            vertical-align: middle;
            color: #fff;
            font-size: 11px;
            font-weight: bold;
            margin-top: 38px;
            display: inline-block;
        }
    /*Tooltip start*/
    .tooltip {
        position: relative;
        display: inline-block;
    }

        .tooltip .tooltiptext {
            visibility: hidden;
            width: auto;
            background-color: #555;
            color: #fff;
            text-align: center;
            border-radius: 6px;
            padding: 5px 5px;
            position: absolute;
            z-index: 1;
            bottom: 125%;
            left: 50%;
            margin-left: -60px;
            opacity: 0;
            transition: opacity 0.3s;
            min-width: 120px;
        }

            .tooltip .tooltiptext::after {
                content: "";
                position: absolute;
                top: 100%;
                left: 50%;
                margin-left: -5px;
                border-width: 5px;
                border-style: solid;
                border-color: #555 transparent transparent transparent;
            }

        .tooltip:hover .tooltiptext {
            visibility: visible;
            opacity: 1;
        }

    /*Tooltip end*/
</style>

<script>

    function ValidatorUpdateDisplay(val) {
        if (typeof (val.display) == "string") {
            if (val.display == "None") {
                return;
            }
            if (val.display == "Dynamic") {
                val.style.display = val.isvalid ? "none" : "inline";
                if (val.isvalid) {
                    document.getElementById(val.controltovalidate).style.border = '1px solid #333';
                }
                else {

                    document.getElementById(val.controltovalidate).style.border = '1px solid red';
                    document.getElementById(val.controltovalidate).select();
                }
                return;
            }

        }
    }


    function enableSubmitButton() {

        document.getElementById("btnSubmitSurvey").disabled = true;
        txtActualStart = document.getElementById("txtActualStart").value;
        txtProjectedStart = document.getElementById("txtProjectedStart").value;

        var IsNetPromoterEnabled = document.getElementById("trNetPromoter");

        if (txtActualStart.length > 0 && txtProjectedStart.length > 0) {
            if(numbers_ranges(parseInt(txtActualStart), parseInt(txtProjectedStart)) && IsNetPromoterEnabled!=null){
                if (numbers_ranges(parseInt(txtActualStart), parseInt(txtProjectedStart))  &&
                    (document.getElementById("<%= rb_0.ClientID %>").checked
                || document.getElementById("<%= rb_1.ClientID %>").checked
                || document.getElementById("<%= rb_2.ClientID %>").checked
                || document.getElementById("<%= rb_3.ClientID %>").checked
                || document.getElementById("<%= rb_4.ClientID %>").checked
                || document.getElementById("<%= rb_5.ClientID %>").checked
                || document.getElementById("<%= rb_6.ClientID %>").checked
                || document.getElementById("<%= rb_7.ClientID %>").checked
                || document.getElementById("<%= rb_8.ClientID %>").checked
                || document.getElementById("<%= rb_9.ClientID %>").checked
                || document.getElementById("<%= rb_10.ClientID %>").checked)
                    ) {
                    document.getElementById("btnSubmitSurvey").disabled = false;
                }
            }
            else if(numbers_ranges(parseInt(txtActualStart), parseInt(txtProjectedStart)) && IsNetPromoterEnabled==null){
                document.getElementById("btnSubmitSurvey").disabled = false;
            }
        } else {
            document.getElementById("btnSubmitSurvey").disabled = true;
        }
    }


    function numbers_ranges(x, y) {
        if (x >= 0 && x <= 45 && y >= 0 && y <= 45) {
            return true;
        }
        else {
            return false;
        }
    }
    function NetPromoter_ClientValidate(source, args) {
        if (document.getElementById("<%= rb_0.ClientID %>").checked
            || document.getElementById("<%= rb_1.ClientID %>").checked
            || document.getElementById("<%= rb_2.ClientID %>").checked
            || document.getElementById("<%= rb_3.ClientID %>").checked
            || document.getElementById("<%= rb_4.ClientID %>").checked
            || document.getElementById("<%= rb_5.ClientID %>").checked
            || document.getElementById("<%= rb_6.ClientID %>").checked
            || document.getElementById("<%= rb_7.ClientID %>").checked
            || document.getElementById("<%= rb_8.ClientID %>").checked
            || document.getElementById("<%= rb_9.ClientID %>").checked
            || document.getElementById("<%= rb_10.ClientID %>").checked
            ) {
            args.IsValid = true;
        }
        else {
            args.IsValid = false;
        }

    }
    window.onload = function () {
        enableSubmitButton();
        NetPromoterChange();
        //var submitBtn= document.getElementById('btnSubmitSurvey');
        //submitBtn.onclick = function () {
        //    submitBtn.setAttribute("disabled", "disabled");
        //    __doPostBack('btnSubmitSurvey', '');            
        //}

    }
    function NetPromoterChange() {
        var rbNetPromoters = document.querySelectorAll('input[name="NetPromoter"]');
        for (var i = 0; i < rbNetPromoters.length; i++) {
            var rbNetPromoter = rbNetPromoters[i];
            rbNetPromoter.onchange = function () {
                enableSubmitButton();
            }
        }
    }

    Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
    function BeginRequestHandler(sender, args) { var oControl = args.get_postBackElement(); oControl.disabled = true; }
</script>
