<%@ Control Language="VB" AutoEventWireup="false" CodeFile="VindiciaHostedPayment.ascx.vb" Inherits="controls_VindiciaHostedPayment" %>

<style>
    iframe {
        height: 20px !important;
        border: 0px;
        width: 100%;
        float: left;
        margin: 0px auto;
    }

    .hide {
        display: none;
    }
</style>

<div class="pckggraywrpr" style="padding-bottom: 1px">
    <div class="pckghdgred">Payment Information</div>

    <table border="0" cellpadding="0" cellspacing="3" style="float: left; width: 50%; margin: 3px 2px 2px 3px; background-color: #fff; height: 264px;">
        <tbody>
            <tr>
                <td colspan="3">
                    <div class="pckghdgblue center">Credit Card Information</div>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td class="fieldreq">&nbsp;</td>
                <td class="smaller">&nbsp;Indicates required field</td>
            </tr>
            <tr>
                <td class="fieldlbl"></td>
                <td width="4" style="padding-top: 3px; padding-bottom: 3px;"></td>
                <td>
                    <img src="/images/utility/visa.gif" alt="Visa" id="imgVisa">&nbsp;

                    <img src="/images/utility/mc.gif" alt="Mastercard/Eurocard" id="imgMasterCard">&nbsp;

                    <img src="/images/utility/amx.gif" alt="American Express" id="imgAmericanExpress">&nbsp;

                    <img src="/images/utility/disc.gif" alt="Discover Network" id="imgDiscover">&nbsp;
                    
                  
                </td>
            </tr>
            <tr>
                <td class="fieldlbl"><span id="labeltxtCardholderName">Cardholder Name</span></td>
                <td id="bartxtCardholderName" class="fieldreq" style="height: 22px;">&nbsp;</td>

                <td>
                    <div id="vin_PaymentMethod_accountHolderName" style="width: 200px"></div>
                </td>
            </tr>
            <tr>
                <td class="fieldlbl"><span id="labeltxtCardNumber">Card Number</span></td>
                <td id="bartxtCardNumber" class="fieldreq" style="height: 22px;">&nbsp;</td>

                <td>
                    <div id="vin_PaymentMethod_creditCard_account" style="width: 200px;"></div>
                </td>
            </tr>
            <tr>
                <td class="fieldlbl"><span id="labelctrlExpDate">Expiration</span></td>
                <td id="barctrlExpDate" class="fieldreq" style="height: 22px;">&nbsp;</td>

                <td>
                    <table cellpadding="0" cellspacing="0" border="0" style="height: 24px;">
                        <tbody>
                            <tr>
                                <td nowrap="">
                                    <div id="vin_PaymentMethod_creditCard_expirationDate_Month" style="width: 100px;"></div>
                                </td>
                                <td nowrap="">
                                    <div id="vin_PaymentMethod_creditCard_expirationDate_Year" style="width: 100px;"></div>
                                </td>
                            </tr>
                        </tbody>
                    </table>

                </td>
            </tr>
            <tr>
                <td class="fieldlbl"><span id="labeltxtCID">Security Code</span></td>
                <td id="bartxtCID" class="fieldreq" style="height: 22px;">&nbsp;</td>
                <td class="smaller nopad">
                    <div id="vin_PaymentMethod_nameValues_cvn" style="width: 200px;"></div>
                    <span class="smaller" style="float: right; display: inline-block; margin-right: 20%; margin-top: -19px; }"><a href="#" onclick="NewWindow('/store/security-info.aspx', 'SecurityInfo', '600', '400', 'no'); return false;">what's this?</a></span>
                </td>

            </tr>
            <tr>
                <td colspan="3" style="text-align: center;">
                    <input id="ae_cbPolicy" type="checkbox" name="ae_cbPolicy"><label for="ae_cbPolicy">I agree to the payment and refund policy</label><br>
                    <a id="lnkPolicy" class="smaller" style="cursor: pointer;" onclick="OpenPolicy();">Click Here to See the Payment and Refund Policy</a>
                </td>
            </tr>
        </tbody>
    </table>
    <table border="0" cellpadding="0" cellspacing="3" style="float: left; width: 49%; margin: 3px 2px 0px 2px; background-color: #fff; height: 60px;">
        <tbody>
            <tr>
                <td colspan="3">
                    <div class="pckghdgblue center">Credit Card Billing Address</div>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td class="fieldreq">&nbsp;</td>
                <td class="smaller">
                    <input id="ae_chkSameAsBilling" type="checkbox" name="ae_chkSameAsBilling" onclick="ToggleForm(event);"><label for="chkSameAsBilling">Click here if Billing Address is the same as Company Address</label></td>
            </tr>
        </tbody>
    </table>
    <table id="tblBilling" border="0" cellpadding="3" cellspacing="0" style="float: left; width: 49%; margin: 0px 2px 2px 2px; background-color: #fff; height: 190px;">
        <tbody>
            <tr>
                <td class="fieldlbl"><span id="labeltxtCompany">Company</span></td>
                <td>&nbsp;</td>
                <td>
                    <input type="text" name="vin_PaymentMethod_billingAddress_name" id="billingAddress_companyName"  />
                </td>
            </tr>
            <tr>
                <td class="fieldlbl"><span id="labeltxtAddress1">Address Line 1</span></td>
                <td id="bartxtAddress1" class="fieldreq" style="height: 22px;">&nbsp;</td>

                <td>
                    <input type="text" name="vin_PaymentMethod_billingAddress_addr1" id="billingAddress_address1"  required />
                </td>
            </tr>
            <tr>
                <td class="fieldlbl"><span id="labeltxtAddress2">Address Line 2</span></td>
                <td>&nbsp;</td>
                <td>
                    <input type="text" id="billingAddress_address2" name="vin_PaymentMethod_billingAddress_addr2"  />

                </td>
            </tr>
            <tr>
                <td class="fieldlbl"><span id="labeltxtCity">City</span></td>
                <td id="bartxtCity" class="fieldreq" style="height: 22px;">&nbsp;</td>

                <td>
                    <input type="text" id="billingAddress_city" name="vin_PaymentMethod_billingAddress_city"  required /></td>
            </tr>
            <tr>
                <td class="fieldlbl"><span id="labeldrpState">State</span></td>
                <td id="bardrpState" class="fieldreq" style="height: 22px;">&nbsp;</td>

                <td class="smaller">
                    <select name="vin_PaymentMethod_billingAddress_district" id="billingAddress_state"  required>
                        <option value="" selected="">--Please Select--</option>
                        <option value="AL">AL</option>
                        <option value="AK">AK</option>
                        <option value="AZ">AZ</option>
                        <option value="AR">AR</option>
                        <option value="AA">AA</option>
                        <option value="AE">AE</option>
                        <option value="AP">AP</option>
                        <option value="CA">CA</option>
                        <option value="CO">CO</option>
                        <option value="CT">CT</option>
                        <option value="DE">DE</option>
                        <option value="DC">DC</option>
                        <option value="FL">FL</option>
                        <option value="GA">GA</option>
                        <option value="GU">GU</option>
                        <option value="HI">HI</option>
                        <option value="ID">ID</option>
                        <option value="IL">IL</option>
                        <option value="IN">IN</option>
                        <option value="IA">IA</option>
                        <option value="KS">KS</option>
                        <option value="KY">KY</option>
                        <option value="LA">LA</option>
                        <option value="ME">ME</option>
                        <option value="MD">MD</option>
                        <option value="MA">MA</option>
                        <option value="MI">MI</option>
                        <option value="MN">MN</option>
                        <option value="MS">MS</option>
                        <option value="MO">MO</option>
                        <option value="MT">MT</option>
                        <option value="NE">NE</option>
                        <option value="NV">NV</option>
                        <option value="NH">NH</option>
                        <option value="NJ">NJ</option>
                        <option value="NM">NM</option>
                        <option value="NY">NY</option>
                        <option value="NC">NC</option>
                        <option value="ND">ND</option>
                        <option value="OH">OH</option>
                        <option value="OK">OK</option>
                        <option value="OR">OR</option>
                        <option value="PA">PA</option>
                        <option value="PR">PR</option>
                        <option value="RI">RI</option>
                        <option value="SC">SC</option>
                        <option value="SD">SD</option>
                        <option value="TN">TN</option>
                        <option value="TX">TX</option>
                        <option value="UT">UT</option>
                        <option value="VT">VT</option>
                        <option value="VI">VI</option>
                        <option value="VA">VA</option>
                        <option value="WA">WA</option>
                        <option value="WV">WV</option>
                        <option value="WI">WI</option>
                        <option value="WY">WY</option>

                    </select>
                </td>
            </tr>
            <tr>
                <td class="fieldlbl"><span id="labeltxtZip">Zip Code</span></td>
                <td id="bartxtZip" class="fieldreq" style="height: 22px;">&nbsp;</td>

                <td>
                    <input type="text" name="vin_PaymentMethod_billingAddress_postalCode" id="billingAddress_zipCode"  required />
                </td>
            </tr>
        </tbody>
    </table>
    <div style="clear: both; height: 0px;">&nbsp;</div>
</div>

<p style="text-align: center;">
    <input type="button" value="Go Back" onclick="history.go(-1); return false;" class="btnred" />
    <input type="submit" value="Process" class="btnred" id="btnProcess" />
</p>

<% if Datalayer.Sysparam.getValue(DB, "TestMode") then %>
<script type="text/javascript" src="https://secure.prodtest.sj.vindicia.com/ws/vindicia.js"></script>
<% else %>
<script type="text/javascript" src="https://secure.vindicia.com/ws/vindicia.js"></script>
<% end if%>

<!--Required-->
<input type="hidden" name="vin_WebSession_version" id="vin_WebSession_version" value="7.0" />
<input type="hidden" id="vin_WebSession_VID" name="vin_WebSession_vid" value='<%= vindiciaWebSession.VinWebSessionVid %>' />
<input type="hidden" name="vin_PaymentMethod_billingAddress_country" value="US" />
<input type="hidden" name="vin_PaymentMethod_replaceOnAllAutoBills" id="vin_PaymentMethod_update_replaceOnAllAutoBills" value="1" />

<!--Custom-->
<input type="text" name="ae_PaymentMethod_cardType" id="ae_PaymentMethod_cardType" value="0" style="visibility: hidden; height: 0px; width: 0px" />
<input type="text" name="ae_Requested_from_page" id="ae_Requested_from_page" value='<%=RequestedFromUpdatePage %>' style="visibility: hidden; height: 0px; width: 0px" />


<script type="text/javascript" defer>
    var vinApp,serverAddress;
    $(function () {
        'use strict';

        vinApp = window.vindicia || new Vindicia();
       
        <% if Datalayer.Sysparam.getValue(DB, "TestMode") then %>
        serverAddress = 'secure.prodtest.sj.vindicia.com';
        vinApp.debuglog = true;
        <% else %>
        serverAddress = 'secure.vindicia.com';
        <% end if%>
        


        const options = {
            vindiciaServer: serverAddress,
            formId: 'main',
            hostedFields: {
                cardNumber: {
                    selector: "#vin_PaymentMethod_creditCard_account",
                    placeholder: "Enter Credit Card Number"
                },
                expirationMonth: {
                    selector: '#vin_PaymentMethod_creditCard_expirationDate_Month',
                    format: "N - A"

                },
                expirationYear: {
                    selector: '#vin_PaymentMethod_creditCard_expirationDate_Year'
                }
                ,
                cvv: {
                    selector: "#vin_PaymentMethod_nameValues_cvn",
                    placeholder: "Enter CVV"
                },
                accountHolderName: {
                    selector: "#vin_PaymentMethod_accountHolderName",
                    placeholder: "Cardholder Name"

                },
                styles: {
                    "input": {
                        "width": "200px",
                        "height": "20px",
                        "font-size": "11px",
                        "border": "1px solid #585858"
                    },
                    "select": {
                        "height": "19px",
                        "width": "95px",
                        "font-size": "11px",
                        "border": "1px solid #585858",
                        "box-shadow": "0px 1px 1px rgba(0,0,0,0.075) inset",
                        "-webkit-transition": "border-color 0.15s ease-in-out 0s, box-shadow 0.15s ease-in-out 0s",
                        "transition": "border-color 0.15s ease-in-out 0s, box-shadow 0.15s ease-in-out 0s"
                    },
                    ".valid": {
                        "border-color": "#228B22"
                    },
                    ".notValid": {
                        "border-color": "#ff0000"
                    }
                }
            },
            onVindiciaFieldEvent: function (event) {
                if (event.detail.fieldType === 'cardNumber') {
                    animateSelectedCard(event.detail.cardType);
                }
            },
            onSubmitEvent: function (theForm) {
                return validateAERules(theForm);
            }
        };

        vinApp.setup(options);

    });

    function animateSelectedCard(type) {

        if (type == 'visa' || type == 'visa_electron') {
            document.getElementById('ae_PaymentMethod_cardType').value = 1;
            $('#imgMasterCard').fadeOut('slow');
            $('#imgAmericanExpress').fadeOut('slow');
            $('#imgDiscover').fadeOut('slow');
            $('#imgVisa').fadeIn('slow');
        }
        else if (type == 'mastercard' || type == 'maestro') {
            document.getElementById('ae_PaymentMethod_cardType').value = 2;
            $('#imgVisa').fadeOut('slow');
            $('#imgAmericanExpress').fadeOut('slow');
            $('#imgDiscover').fadeOut('slow');
            $('#imgMasterCard').fadeIn('slow');
        }
        else if (type == 'amex') {
            document.getElementById('ae_PaymentMethod_cardType').value = 3;
            $('#imgMasterCard').fadeOut('slow');
            $('#imgVisa').fadeOut('slow');
            $('#imgDiscover').fadeOut('slow');
            $('#imgAmericanExpress').fadeIn('slow');
        }
        else if (type == 'discover') {
            document.getElementById('ae_PaymentMethod_cardType').value = 4;
            $('#imgMasterCard').fadeOut('slow');
            $('#imgAmericanExpress').fadeOut('slow');
            $('#imgVisa').fadeOut('slow');
            $('#imgDiscover').fadeIn('slow');
        }
        else {
            document.getElementById('ae_PaymentMethod_cardType').value = 0;
            $('#imgMasterCard').fadeIn('slow');
            $('#imgAmericanExpress').fadeIn('slow');
            $('#imgDiscover').fadeIn('slow');
            $('#imgVisa').fadeIn('slow');
        }
    }


    function validateAERules(contrl) {

        if (!document.getElementById("ae_cbPolicy").checked) {
            alert("Please agree to the payment and refund policy");
            return false;
        }
        return true;
    }

    function ToggleForm(e) {
        var tbl = document.getElementById('tblBilling');
        var target = e.target ? e.target : e.srcElement;
        if (target.checked) {
            
            $("[id*='billingAddress_companyName'").val("<%= Builder.CompanyName %>");
            $("[id*='billingAddress_address1'").val("<%= Builder.Address %>");
            $("[id*='billingAddress_address2'").val("<%= Builder.Address2 %>");
            $("[id*='billingAddress_city'").val("<%= Builder.City %>");
            $("[id*='billingAddress_state'").val("<%= Builder.State %>");
            $("[id*='billingAddress_zipCode'").val("<%= Builder.Zip %>");

        } else {
             $("[id*='billingAddress_companyName'").val('');
             $("[id*='billingAddress_address1'").val('');
             $("[id*='billingAddress_address2'").val('');
            $("[id*='billingAddress_city'").val('');
            $("[id*='billingAddress_state'").val('');
            $("[id*='billingAddress_zipCode'").val('');
        }
        return true;
    }


 

</script>
