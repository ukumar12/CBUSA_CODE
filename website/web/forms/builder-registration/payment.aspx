<%@ Page Language="VB" AutoEventWireup="false" CodeFile="payment.aspx.vb" Inherits="forms_builder_registration_payment" %>

<%@ Register TagName="StoreCreditCardImages" TagPrefix="CC" Src="~/controls/StoreCreditCardImages.ascx" %>
<%@ Register TagName="BuilderRegistrationSteps" TagPrefix="CC" Src="~/controls/BuilderRegistrationSteps.ascx" %>
<%@ Register TagName="VindicaHostedPayment" TagPrefix="CC" Src="~/controls/VindiciaHostedPayment.ascx" %>

<CT:MasterPage ID="CTMain" runat="server">
<asp:PlaceHolder runat="server">
<script type="text/javascript">

    function OpenPolicy() {
        var frm = $get('<%=frmPolicy.ClientID %>').control;
        frm._doMoveToCenter();
        frm.Open();
    }
    function ClosePolicy() {
        var cb = document.querySelector('#ae_cbPolicy');
        cb.checked = true;
        $get('<%=frmPolicy.ClientID %>').control.Close();
    }

</script>
</asp:PlaceHolder>

<div style="margin:10px auto;text-align:center;">
<%--<asp:button id ="btnGoToDashBoard" text="Go to DashBoard" class="btnred" runat="server" style="margin-right:176px;"/>--%>
    <CC:BuilderRegistrationSteps ID="ctrlSteps" runat="server" RegistrationStep="4" />
</div>

<asp:Literal id="ltlResult" runat="server"></asp:Literal>
<asp:Panel id="pnlForm" runat="server" DefaultButton="">
<div class="thememarket" style="width:500px;margin:25px auto;border:1px solid #c2c2c2">
    <div class="pckghdgred">CBUSA Fee Schedule</div>
    <table cellpadding="4" cellspacing="0" border="0" style="width:100%;margin:0px" class="tblcomprlen">
        <tr>
            <th>Item</th>
            <th>When Charged?</th>
            <th>Price</th>
           <th></th>
        </tr>
    <asp:PlaceHolder id="phRegistrationFee" runat="server">
        <tr>
            <td><asp:Literal id="ltlRegistrationFee" runat="server"></asp:Literal></td>
            <td><asp:Literal id="ltlWhen" runat="server"></asp:Literal></td>
            <td align ="right"><asp:Literal id="ltlPrice" runat="server"></asp:Literal></td>
        </tr>
        <tr><td colspan="3" style="height:1px;background-color:#aaa;padding:0px;margin:0px;"><img src="/images/spacer.gif" style="height:1px;" alt="" /></td></tr>
    </asp:PlaceHolder>
    <asp:Literal id="ltlBackBill" runat="server"></asp:Literal>
    <asp:Repeater id="rptBillingSchedule" runat="server">
        <ItemTemplate>
            <tr id="trRow" runat="server">
                <td>Recurring Monthly Membership Fee</td>
                <td><asp:Literal id="ltlWhen" runat="server"></asp:Literal></td>
                <td align ="right"><asp:Literal id="ltlPrice" runat="server"></asp:Literal></td>
                <%-- <td><asp:Literal id="ltlPrice2" text= "9.87" runat="server"></asp:Literal></td>--%>
            </tr>
        </ItemTemplate>
        <SeparatorTemplate>
            <tr><td colspan="3" style="height:1px;background-color:#aaa;padding:0px;margin:0px;"><img src="/images/spacer.gif" style="height:1px;" alt="" /></td></tr>
        </SeparatorTemplate>
    </asp:Repeater>
     <tr><td colspan="3" style="height:1px;background-color:#aaa;padding:0px;margin:0px;"><img src="/images/spacer.gif" style="height:1px;" alt="" /></td></tr>
    <asp:PlaceHolder id="PhTax" runat="server">
        <tr>
        <td>Tax</td>
            <td> </td>
              <td align ="right"><asp:Literal id="ltlTotalTax"   runat="server"></asp:Literal></td>
            <td>  </td>
        </tr>
        <tr><td colspan="3" style="height:1px;background-color:#aaa;padding:0px;margin:0px;"><img src="/images/spacer.gif" style="height:1px;" alt="" /></td></tr>
    </asp:PlaceHolder>
      <asp:PlaceHolder id="PhTotal" runat="server">
        <tr>
        <td>Total</td>
            <td> </td>
              <td align ="right" ><asp:Literal id="ltlTotal"   runat="server"></asp:Literal></td>
            <td>  </td>
        </tr>
       
    </asp:PlaceHolder>

    </table>
</div>

<%--<div class="pckggraywrpr" style="display: none">
    <div class="pckghdgred">Payment Information</div>

    <table border="0" cellpadding="0" cellspacing="3" style="float:left;width:49%;margin:2px;background-color:#fff;">
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
	        <td width="4" style="padding-top:3px;padding-bottom:3px;"></td>
	        <td><CC:StoreCreditCardImages id="ctrlStoreCreditCardImages" runat="server" /></td>
        </tr>
        <tr>
        <td class="fieldlbl"><span id="labeltxtCardholderName" runat="server">Cardholder Name</span></td>
	        <td class="fieldreq" style="height:22px;" id="bartxtCardholderName" runat="server">&nbsp;</td>
	        <td>
	            <table cellpadding="2" cellspacing="0" border="0">
	                <tr>
	                    <td class="bold smaller">First</td>
	                    <td class="bold smaller">Last</td>
	                </tr>
	                <tr>
	                    <td><asp:textbox id="txtCardholderFirstName" runat="server" style="width:100px;" columns="10" maxlength="50" onkeyup="HandleFirstKeyUp(event);" /></td></td>
	                    <td><asp:TextBox id="txtCardholderLastName" runat="server" style="width:100px;" columns="10" maxlength="50" onkeyup="HandleLastKeyUp(event);"></asp:TextBox></td>
	                </tr>
	            </table>
	        </td>
        </tr>
        <tr>
            <td class="fieldlbl"><span id="labeltxtCardNumber" runat="server">Card Number</span></td>
	        <td class="fieldreq" style="height:22px;" id="bartxtCardNumber" runat="server">&nbsp;</td>
	        <td><asp:textbox id="txtCardNumber" runat="server" autocomplete="off" style="width:200px;" columns="10" maxlength="20" /></td>
        </tr>
        <tr>
	        <td class="fieldlbl"><span id="labelctrlExpDate" runat="server">Expiration</span></td>
	        <td class="fieldreq" style="height:22px;" id="barctrlExpDate" runat="server">&nbsp;</td>
	        <td><CC:ExpDate id="ctrlExpDate" runat="server" /></td>
        </tr>
        <tr>
	        <td class="fieldlbl"><span id="labeltxtCID" runat="server">Security Code</span></td>
	        <td class="fieldreq" style="height:22px;" id="bartxtCID" runat="server">&nbsp;</td>
	        <td class="smaller">
	            <asp:textbox id="txtCID" autocomplete="off" runat="server" style="width:45px;" columns="10" maxlength="4" />
	            <span class="smaller" style="padding-left:10px;"><a href="#" onClick="NewWindow('/store/security-info.aspx', 'SecurityInfo', '600', '400', 'no'); return false;">what's this?</a></span>
	        </td>
        </tr>
        <tr>
	        <td class="fieldlbl"><span id="labeldrpCardType" runat="server">Card Type</span></td>
	        <td class="fieldreq" style="height:22px;" id="bardrpCardType" runat="server">&nbsp;</td>
	        <td><asp:dropdownlist id="drpCardType" runat="server" style="width:175px;height:20px;" /></td>
	     </tr>
	     <tr>
	        <td colspan="3" align="center">
	            <asp:CheckBox id="cbPolicy" runat="server" onclick="OpenPolicy();" Text="I agree to the payment and refund policy"></asp:CheckBox><br />
	            <asp:HyperLink id="lnkPolicy" runat="server" style="cursor:pointer;" text="Click Here to See the Payment and Refund Policy" cssclass="smaller"></asp:HyperLink>
            </td>
	     </tr>
    </table>
    <table border="0" cellpadding="0" cellspacing="3" style="float:left;width:49%;margin:2px 2px 0px 2px;background-color:#fff;">
        <tr>
            <td colspan="3">
                <div class="pckghdgblue center">Credit Card Billing Address</div>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td class="fieldreq">&nbsp;</td>
            <td class="smaller"><asp:CheckBox id="chkSameAsBilling" runat="server" text="Click here if Billing Address is the same as Company Address" onclick="ToggleForm(event);"></asp:CheckBox></td>
        </tr>        
    </table>
    <table id="tblBilling" border="0" cellpadding="3" cellspacing="0" style="float:left;width:49%;height:100%;margin:0px 2px 2px 2px;background-color:#fff;">
        <tr>
            <td class="fieldlbl"><span id="labeltxtFirstName" runat="server">Name</span><br /><span class="smaller">(Edit Cardholder Name)</span></td>
            <td class="fieldreq" style="height:22px;" id="bartxtFirstName" runat="server">&nbsp;</td>
            <td>
                <table cellpadding="2" cellspacing="0" border="0">
                    <tr>
                        <td class="bold smaller">First</td>
                        <td class="bold smaller">Last</td>
                    </tr>
                    <tr>
                        <td><asp:TextBox id="txtFirstName" runat="server" enabled="false" style="width:100px;" columns="10" maxlength="50"></asp:TextBox></td></td>
                        <td><asp:TextBox id="txtLastName" runat="server" enabled="false" style="width:100px;" columns="10" maxlength="50"></asp:TextBox></td>
                    </tr>
                </table>
            </td>            
        </tr>
        <tr>
            <td class="fieldlbl"><span id="labeltxtCompany" runat="server">Company</span></td>
            <td>&nbsp;</td>
            <td><asp:textbox id="txtCompany" runat="server" style="width:200px;" columns="10" maxlength="50" /></td>
        </tr>
        <tr>
            <td class="fieldlbl"><span id="labeltxtAddress1" runat="server">Address Line 1</span></td>
	        <td class="fieldreq" style="height:22px;" id="bartxtAddress1" runat="server">&nbsp;</td>
	        <td><asp:textbox id="txtAddress1" runat="server" style="width:200px;" columns="10" maxlength="100" /></td>
        </tr>
        <tr>
            <td class="fieldlbl"><span id="labeltxtAddress2" runat="server">Address Line 2</span></td>
	        <td>&nbsp;</td>
	        <td><asp:textbox id="txtAddress2" runat="server" style="width:200px;" columns="10" maxlength="100" /></td>
        </tr>
        <tr>
	        <td class="fieldlbl"><span id="labeltxtCity" runat="server">City</span></td>
	        <td class="fieldreq" style="height:22px;" id="bartxtCity" runat="server">&nbsp;</td>
	        <td><asp:TextBox id="txtCity" runat="server" style="width:200px;" columns="10" maxlength="100"></asp:TextBox></td>
        </tr>
        <tr>
	        <td class="fieldlbl"><span id="labeldrpState" runat="server">State</span></td>
	        <td class="fieldreq" style="height:22px;" id="bardrpState" runat="server">&nbsp;</td>
	        <td class="smaller">
                <asp:DropDownList id="drpState" runat="server"></asp:DropDownList>
	        </td>
        </tr>
        <tr>
	        <td class="fieldlbl"><span id="labeltxtZip" runat="server">Zip Code</span></td>
	        <td class="fieldreq" style="height:22px;" id="bartxtZip" runat="server">&nbsp;</td>
	        <td><asp:textbox id="txtZip" runat="server" style="width:80px;" columns="10" maxlength="10"></asp:textbox></td>
	    </tr>
    </table>    
    <div style="clear:both;height:0px;">&nbsp;</div>
</div>--%>
    
<!--VinHOA-->
<CC:VindicaHostedPayment ID="vnhostedPayment" runat="server" />
<!--VinHOA-->
 
     <CC:PopupForm ID="frmPolicy" runat="server" OpenMode="MoveToCenter" ShowVeil="true" VeilCloses="false"  CssClass="pform">
                <FormTemplate>
                <div style="background-color:#fff;border:1px solid #aaa;padding:10px;width:800px;text-align:left;">
                    <b>Sales and Refund Policy; Payment Terms</b><br/><br/>
                    <em>
                        CB-USA offers various paid products and services to Users of the Site.  You agree to pay for all products and services ordered through the Site using the payment method indicated.  You also agree to provide express authorization to CB-USA to charge said fees to your payment provider at the time of purchase or renewal.  Fees owed depend on the specific type and quantity of CB-USA products, services, information, or subscription ordered, as specified by CB-USA at the time of sale.  All fees paid by you are non-refundable.  Without limiting the foregoing, all fees paid for monthly or quarterly subscriptions are non-refundable, regardless of whether the subscription is terminated prior to the end of the then-current billing period.  Subscriptions will automatically renew using your current credit card account number unless you cancel your subscription by emailing customerservice@cbusa.us  at least five days before the beginning of the next billing term.  All cancellation requests will be processed within five business days of receipt.  Once cancellation is processed, a confirmation email will be sent via your email account on record with CB-USA.  If you have a question about a cancellation, you should contact CB-USA at customerservice@cbusa.us You must notify CB-USA about any billing problems or discrepancies within thirty (30) days after charges first appear on your account statement, otherwise you agree to waive your right to dispute such problems or discrepancies.  CB-USA reserves the right, at any time, to change the nature and amount of fees charged for access to its products and services and the manner in which such fees are assessed.  CB-USA will provide timely notice to affected Users of any such changes.  You are responsible for the timely payment of any fees incurred by your use of products and services available on the Site and all applicable taxes.  It is your responsibility to provide CB-USA with any contact or billing information changes or updates (including without limitation email address, phone number, credit card number, etc.).  To make changes to your account, visit the My Account sections of the CB-USA System. 
                    </em>
                    <p class="larger" style="text-align:center;">                                        
                        <input type="button" id="btnOK" class="btnred" value="OK" onclick="ClosePolicy();return false;"><br />
                    </p>
                </div>
                </FormTemplate>
</CC:PopupForm>            
       

<%--<p style="text-align:center;">
    <asp:Button id="btnBack" runat="server" text="Go Back" onclientclick="history.go(-1);return false;" cssclass="btnred" />
    <CC:OneClickButton ID="btnProcess" runat="server" CssClass="btnred" Text="Process" CausesValidation="false" />
</p>--%>

<%--<CC:RequiredFieldValidatorFront ID="rqtxtCardholderFirstName" runat="server" ControlToValidate="txtCardholderFirstName" EnableClientScript="False" Display="None" ErrorMessage="You must enter your credit cardholder name" />
<CC:RequiredFieldValidatorFront ID="rqtxtCardholderLastName" runat="server" ControlToValidate="txtCardholderLastName" EnableClientScript="False" Display="None" ErrorMessage="You must enter your credit cardholder name" />

<CC:CreditCardValidatorFront ID="cvtxtCardNumber" runat="server" ControlToValidate="txtCardNumber" EnableClientScript="False" Display="None" ErrorMessage="The credit card number you provided is not valid" />
<CC:CreditCardTypeValidatorFront ID="cvdrpCardType" runat="server" ControlToValidate="drpCardType" CreditCardNumberControl="txtCardNumber" EnableClientScript="False" Display="None" ErrorMessage="The credit card type doesn't match the credit card number" />
<CC:RequiredFieldValidatorFront ID="rqtxtCardNumber" runat="server" ControlToValidate="txtCardNumber" EnableClientScript="False" Display="None" ErrorMessage="You must enter your credit card number" />
<CC:RequiredFieldValidatorFront ID="rqtxtCID" runat="server" ControlToValidate="txtCID" EnableClientScript="False" Display="None" ErrorMessage="You must enter your card security code" />
<CC:RequiredExpDateValidatorFront ID="rqctrlExpDate" runat="server" ControlToValidate="ctrlExpDate" EnableClientScript="False" Display="None" ErrorMessage="You must provide expiration date" />
<CC:ExpDateValidatorFront ID="valctrlExpDate" runat="server" ControlToValidate="ctrlExpDate" EnableClientScript="False" Display="None" ErrorMessage="You must provide a valid expiration date" />
<CC:RequiredFieldValidatorFront ID="rqdrpCardType" runat="server" ControlToValidate="drpCardType" EnableClientScript="False" Display="None" ErrorMessage="You must enter your credit card type" />

<CC:RequiredFieldValidatorFront ID="rfvtxtAddress1" runat="server" ControlToValidate="txtAddress1" ErrorMessage="Billing Address Line 1 is required"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtCity" runat="server" ControlToValidate="txtCity" ErrorMessage="Billing City is required"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvdrpState" runat="server" ControlToValidate="drpState" ErrorMessage="Billing State is required"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtZip" runat="server" ControlToValidate="txtZip" ErrorMessage="Billing Zip Code is required"></CC:RequiredFieldValidatorFront>
<CC:ZipValidatorFront ID="zvftxtZip" runat="server" ControlToValidate="txtZip" ErrorMessage="Billing Zip Code is invalid"></CC:ZipValidatorFront>--%>
</asp:Panel>
</CT:MasterPage>