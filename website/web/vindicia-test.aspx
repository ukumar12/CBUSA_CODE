<%@ Page Language="VB" AutoEventWireup="false" CodeFile="vindicia-test.aspx.vb" Inherits="vindicia_test" %>
<%@ Register TagName="StoreCreditCardImages" TagPrefix="CC" Src="~/controls/StoreCreditCardImages.ascx" %>

<CT:MasterPage ID="CTMain" runat="server">


<asp:PlaceHolder runat="server">
<script type="text/javascript">
    function ToggleForm(e) {
        var tbl = $get('tblBilling');
        var target = e.target ? e.target : e.srcElement;
        if (target.checked) {
            tbl.style.display = 'none';
        } else {
            tbl.style.display = '';
        }
    }
    function OpenPolicy() {
        var frm = $get('<%=frmPolicy.ClientID %>').control;
        frm._doMoveToCenter();
        frm.Open();
    }
    function ClosePolicy() {
        var cb = $get('<%=cbPolicy.ClientID %>');
        cb.checked = true;
        $get('<%=frmPolicy.ClientID %>').control.Close();
    }
    function HandleFirstKeyUp(e) {
        var src = $get('<%=txtCardholderFirstName.ClientID %>');
        var dest = $get('<%=txtFirstName.ClientID %>');
        dest.value = src.value;
    }
    function HandleLastKeyUp(e) {
        var src = $get('<%=txtCardholderLastName.ClientID %>');
        var dest = $get('<%=txtLastName.ClientID %>');
        dest.value = src.value;
    }
</script>
</asp:PlaceHolder>


    <div class="pckggraywrpr">
        <div class="pckghdgred">Vindicia Test Page</div>
        
        <asp:Literal id="ltlResult" runat="server"></asp:Literal><br /><br />
    
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
                <CC:PopupForm ID="frmPolicy" runat="server" OpenMode="MoveToCenter" ShowVeil="true" VeilCloses="false" OpenTriggerId="lnkPolicy" CssClass="pform">
                <FormTemplate>
                <div style="background-color:#fff;border:1px solid #aaa;padding:10px;width:800px;text-align:left;">
                    <b>Sales and Refund Policy; Payment Terms</b><br/><br/>
                    <em>
                        CB-USA offers various paid products and services to Users of the Site.  You agree to pay for all products and services ordered through the Site using the payment method indicated.  You also agree to provide express authorization to CB-USA to charge said fees to your payment provider at the time of purchase or renewal.  Fees owed depend on the specific type and quantity of CB-USA products, services, information, or subscription ordered, as specified by CB-USA at the time of sale.  All fees paid by you are non-refundable.  Without limiting the foregoing, all fees paid for monthly or quarterly subscriptions are non-refundable, regardless of whether the subscription is terminated prior to the end of the then-current billing period.  Subscriptions will automatically renew using your current credit card account number unless you cancel your subscription by emailing customerservice@cbusa.us  at least five days before the beginning of the next billing term.  All cancellation requests will be processed within five business days of receipt.  Once cancellation is processed, a confirmation email will be sent via your email account on record with CB-USA.  If you have a question about a cancellation, you should contact CB-USA at customerservice@cbusa.us You must notify CB-USA about any billing problems or discrepancies within thirty (30) days after charges first appear on your account statement, otherwise you agree to waive your right to dispute such problems or discrepancies.  CB-USA reserves the right, at any time, to change the nature and amount of fees charged for access to its products and services and the manner in which such fees are assessed.  CB-USA will provide timely notice to affected Users of any such changes.  You are responsible for the timely payment of any fees incurred by your use of products and services available on the Site and all applicable taxes.  It is your responsibility to provide CB-USA with any contact or billing information changes or updates (including without limitation email address, phone number, credit card number, etc.).  To make changes to your account, visit the My Account sections of the CB-USA System. 
                    </em>
                    <p class="larger" style="text-align:center;">                                        
                        <asp:Button id="btnOK" runat="server" text="OK" cssclass="btnred" onclientclick="ClosePolicy();return false;" /><br />
                    </p>
                </div>
                </FormTemplate>
                </CC:PopupForm>            
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
            <td class="smaller"><asp:CheckBox id="chkSameAsBilling" runat="server" text="Click here if Billing Address is the same as Registration Address" onclick="ToggleForm(event);"></asp:CheckBox></td>
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
	        <td><asp:textbox id="txtZip" runat="server" style="width:80px;" columns="10" maxlength="15"></asp:textbox></td>
	    </tr>
    </table>    
    <div style="clear:both;height:0px;">&nbsp;</div>
    
    <p class="center">
        <asp:Button id="btnProcess" runat="server" text="Process" cssclass="btnred" />
    </p>
</div>
</CT:MasterPage>