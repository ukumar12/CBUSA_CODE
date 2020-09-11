<%@ Page Language="VB" AutoEventWireup="false" CodeFile="registration.aspx.vb" Inherits="forms_vendor_registration_default" %>
<script type="text/javascript">
<!--

    function atload() { DisplaySubsidiary();DisplayLawsuit();}
    window.onload = atload;
    
    function DisplaySubsidiary() {

        var i;
        var value;
        var rblIsSubsidiary = document.getElementById('rblIsSubsidiary');
        var trSubsidiary = document.getElementById('trSubsidiary');
        
        for (var i = 0; i < document.all.rblIsSubsidiary.length; i++) {
            if (document.all.rblIsSubsidiary[i].checked) {
                var value = document.all.rblIsSubsidiary[i].value;
            }
        }

        if (value == "True")
            trSubsidiary.style.display = '';
        else
            trSubsidiary.style.display = 'none';
    }

    function DisplayLawsuit() {

        var i;
        var value;
        var rblHadLawsuit = document.getElementById('rblHadLawsuit');
        var trLawsuit = document.getElementById('trLawsuit');

        for (var i = 0; i < document.all.rblHadLawsuit.length; i++) {
            if (document.all.rblHadLawsuit[i].checked) {
                var value = document.all.rblHadLawsuit[i].value;
            }
        }

        if (value == "True")
            trLawsuit.style.display = '';
        else
            trLawsuit.style.display = 'none';
    }
    
//-->
</script>
<CT:MasterPage ID="CTMain" runat="server">
<h4>Registration Info</h4>

<table class="regform">
    <col width="150px" />
    <col width="25px" />
    <col width="" />
    <tr>
        <td>&nbsp;</td>
        <td class="fieldreq">&nbsp;</td>
        <td><span class="smaller"> indicates required field</span></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtCompanyName" runat="server">Company Name:</span></td>
        <td class="fieldreq" id="bartxtCompanyName" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtCompanyName" runat="server" MaxLength="50" Rows="50" CssClass="regtxt" style="width: 319px;"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtAddress" runat="server">Address:</span></td>
        <td class="fieldreq" id="bartxtAddress" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtAddress" runat="server" MaxLength="50" Rows="50" CssClass="regtxt" style="width: 319px;"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl">Address line 2:</td>
        <td>&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtAddress2" runat="server" MaxLength="50" Rows="50" CssClass="regtxt" style="width: 319px;"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtCity" runat="server">City:</span></td>
        <td class="fieldreq" id="bartxtCity" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtCity" runat="server" MaxLength="50" Rows="50" CssClass="regtxt" style="width: 319px;"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeldrpState" runat="server">State:</span></td>
        <td class="fieldreq" id="bardrpState" runat="server">&nbsp;</td>
        <td class="field"><asp:DropDownList  ID="drpState" runat="server" /></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtZip" runat="server">Zip Code:</span></td>
        <td class="fieldreq" id="bartxtZip" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtZip" runat="server" MaxLength="15" Rows="15" CssClass="regtxtshort" style="width: 319px;"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtPhone" runat="server">Phone:</span></td>
        <td class="fieldreq" id="bartxtPhone" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtPhone" runat="server" MaxLength="50" Rows="50" CssClass="regtxt" style="width: 319px;"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtMobile" runat="server">Mobile:</span></td>
        <td class="fieldreq" id="bartxtMobile" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtMobile" runat="server" MaxLength="50" Rows="50" CssClass="regtxt" style="width: 319px;"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtPager" runat="server">Pager:</span></td>
        <td>&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtPager" runat="server" MaxLength="50" Rows="50" CssClass="regtxt" style="width: 319px;"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtOtherPhone" runat="server">Other Phone:</span></td>
        <td>&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtOtherPhone" runat="server" MaxLength="50" Rows="50" CssClass="regtxt" style="width: 319px;"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtFax" runat="server">Fax:</span></td>
        <td class="fieldreq" id="bartxtFax" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtFax" runat="server" MaxLength="50" Rows="50" CssClass="regtxt" style="width: 319px;"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtEmail" runat="server">Email:</span></td>
        <td class="fieldreq" id="bartxtEmail" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtEmail" runat="server" MaxLength="100" Rows="50" CssClass="regtxt" style="width: 319px;"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtWebsiteUrl" runat="server">Website:</span></td>
        <td class="fieldreq" id="bartxtWebsiteUrl" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtWebsiteUrl" runat="server" MaxLength="100" Rows="50" CssClass="regtxt" style="width: 319px;"></asp:TextBox></td>
    </tr>
	<tr>
        <td class="fieldlbl"><span id="labeltxtYearStarted" runat="server">Years In Business:</span></td>
        <td class="fieldreq" id="bartxtYearStarted" runat="server">&nbsp;</td>
        <td class="field"><asp:textbox id="txtYearStarted" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
	</tr>
	<tr>
        <td class="fieldlbl"><span id="labeltxtEmployees" runat="server">Employees:</span></td>
        <td class="fieldreq" id="bartxtEmployees" runat="server">&nbsp;</td>
        <td class="field"><asp:textbox id="txtEmployees" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
	</tr>
	<tr>
        <td class="fieldlbl"><span id="labeltxtProductsOffered" runat="server">Products Offered:</span></td>
        <td class="fieldreq" id="bartxtProductsOffered" runat="server">&nbsp;</td>
        <td class="field"><asp:textbox id="txtProductsOffered" runat="server" maxlength="2000" rows="5" TextMode="Multiline" columns="50" style="width: 319px;"></asp:textbox></td>
	</tr>
	<tr>
        <td class="fieldlbl"><span id="labeltxtCompanyMemberships" runat="server">Company Memberships:</span></td>
        <td class="fieldreq" id="bartxtCompanyMemberships" runat="server">&nbsp;</td>
        <td class="field"><asp:textbox id="txtCompanyMemberships" runat="server" maxlength="500" rows="5" TextMode="Multiline" columns="50" style="width: 319px;"></asp:textbox></td>
	</tr>
	<tr>
        <td class="fieldlbl"><span id="labelrblIsSubsidiary" runat="server"  >Is Subsidiary?</span></td>
        <td class="fieldreq" id="barrblIsSubsidiary" runat="server">&nbsp;</td>
        <td class="field">
			<asp:RadioButtonList runat="server" ID="rblIsSubsidiary" onclick="DisplaySubsidiary()" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" />
			<asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
	</tr>
	<tr id="trSubsidiary" >
        <td class="fieldlbl"><span id="labeltxtSubsidiaryExplanation" runat="server">Subsidiary Explanation:</span></td>
        <td></td>
        <td class="field"><asp:textbox id="txtSubsidiaryExplanation" runat="server" maxlength="1000" rows="5" TextMode="Multiline" columns="50" style="width: 319px;"></asp:textbox></td>
	</tr>
	<tr>
        <td class="fieldlbl"><span id="labelrblHadLawsuit" runat="server">Had Lawsuit?</span></td>
        <td class="fieldreq" id="barrblHadLawsuit" runat="server">&nbsp;</td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblHadLawsuit" onclick="DisplayLawsuit()" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" />
			<asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
	</tr>
	<tr id="trLawsuit" >
        <td class="fieldlbl"><span id="labeltxtLawsuitExplanation" runat="server">Lawsuit Explanation:</span></td>
        <td></td>
        <td class="field"><asp:textbox id="txtLawsuitExplanation" runat="server" maxlength="2000" rows="5" TextMode="Multiline" columns="50" style="width: 319px;"></asp:textbox></td>
	</tr>
	<tr>
        <td class="fieldlbl"><span id="labeltxtSupplyArea" runat="server">Supply Area:</span></td>
        <td class="fieldreq" id="bartxtSupplyArea" runat="server">&nbsp;</td>
        <td class="field"><asp:textbox id="txtSupplyArea" runat="server" maxlength="1000" columns="50" rows="5" TextMode="Multiline" style="width: 319px;"></asp:textbox></td>
	</tr>
	<tr>
        <td class="fieldlbl"><span id="labeldrpPrimarySupplyPhase" runat="server">Primary Supply Phase:</span></td>
        <td class="fieldreq" id="bardrpPrimarySupplyPhase" runat="server">&nbsp;</td>
        <td class="field"><asp:DropDownList  ID="drpPrimarySupplyPhase" runat="server" /></td>
	</tr>
	<tr>
        <td class="fieldlbl"><span id="labeldrpSeconarySupplyPhase" runat="server">Secondary Supply Phase ID:</span></td>
        <td class="fieldreq" id="bardrpSeconarySupplyPhase" runat="server">&nbsp;</td>
        <td class="field"><asp:DropDownList  ID="drpSeconarySupplyPhase" runat="server" /></td>
	</tr>
	<tr>
        <td class="fieldlbl"><span id="labeltxtNotes" runat="server">Notes:</span></td>
        <td></td>
        <td class="field"><asp:textbox id="txtNotes" runat="server" maxlength="2000" columns="50" rows="5" TextMode="Multiline" style="width: 319px;"></asp:textbox></td>
	</tr>
    <tr>
        <td class="fieldlbl" colspan="3">
            <p style="text-align:center;">
                <CC:OneClickButton ID="btnSave" runat="server" Text="Save" CssClass="btn" />
                <CC:OneClickButton ID="btnCancel" runat="server" Text="Cancel" CssClass="btn" />
            </p>
        </td>
    </tr>
</table>
<CC:RequiredFieldValidatorFront ID="rfvtxtCompanyName" runat='server' ControlToValidate="txtCompanyName" ErrorMessage="Field 'Company Name' is blank" ValidationGroup="VendorReg"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtAddress" runat="server" ControlToValidate="txtAddress" ErrorMessage="Field 'Address' is blank" ValidationGroup="VendorReg"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtCity" runat="server" ControlToValidate="txtCity" ErrorMessage="Field 'City' is blank" ValidationGroup="VendorReg"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvdrpState" runat="server" ControlToValidate="drpState" ErrorMessage="Field 'State' is blank" ValidationGroup="VendorReg"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtZip" runat="server" ControlToValidate="txtZip" ErrorMessage="Field 'Zip Code' is blank" ValidationGroup="VendorReg"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtPhone" runat="server" ControlToValidate="txtPhone" ErrorMessage="Field 'Phone' is blank" ValidationGroup="VendorReg"></CC:RequiredFieldValidatorFront>
<CC:PhoneValidatorFront id="pvftxtPhone" runat="server" ControlToValidate="txtPhone" ErrorMessage="Field 'Phone' is invalid" ValidationGroup="VendorReg"></CC:PhoneValidatorFront>
<CC:PhoneValidatorFront ID="pvftxtMobile" runat="server" ControlToValidate="txtModile" ErrorMessage="Field 'Mobile' is invalid" ValidationGroup="VendorReg"></CC:PhoneValidatorFront>
<CC:PhoneValidatorFront id="pvftxtPager" runat="server" ControlToValidate="txtPager" ErrorMessage="Field 'Pager' is invalid" ValidationGroup="VendorReg"></CC:PhoneValidatorFront>
<CC:PhoneValidatorFront ID="pvftxtOtherPhone" runat="server" ControlToValidate="txtOtherPhone" ErrorMessage="Field 'Other Phone' is invalid" ValidationGroup="VendorReg"></CC:PhoneValidatorFront>
<CC:PhoneValidatorFront ID="pvftxtFax" runat="server" ControlToValidate="txtFax" ErrorMessage="Field 'Fax' is invalid" ValidationGroup="VendorReg"></CC:PhoneValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Field 'Email' is blank" ValidationGroup="VendorReg"></CC:RequiredFieldValidatorFront>
<CC:EmailValidatorFront id="evftxtEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Field 'Email' is invalid" ValidationGroup="VendorReg"></CC:EmailValidatorFront>
<CC:UrlValidator ID="uvtxtWebsiteUrl" runat="server" Display="none" ControlToValidate="txtWebsiteUrl" EnableClientScript="false" ErrorMessage="Field 'Website URL' is invalid" ValidationGroup="VendorReg"></CC:UrlValidator>
<asp:RequiredFieldValidator ID="rfvYearStarted" runat="server" Display="none" ControlToValidate="txtYearStarted" ErrorMessage="Field 'Years In Business' is blank" ValidationGroup="VendorReg"></asp:RequiredFieldValidator>
<CC:IntegerValidator Display="Dynamic" runat="server" id="fvYearStarted" ControlToValidate="txtYearStarted" ErrorMessage="Field 'Years In Business' is invalid" ValidationGroup="VendorReg"/>
<asp:RequiredFieldValidator ID="rfvEmployees" runat="server" Display="none" ControlToValidate="txtEmployees" ErrorMessage="Field 'Employees' is blank" ValidationGroup="VendorReg"></asp:RequiredFieldValidator>
<CC:IntegerValidator Display="Dynamic" runat="server" id="fvEmployees" ControlToValidate="txtEmployees" ErrorMessage="Field 'Employees' is invalid" ValidationGroup="VendorReg"/>
<asp:RequiredFieldValidator ID="rfvProductsOffered" runat="server" Display="none" ControlToValidate="txtProductsOffered" ErrorMessage="Field 'Products Offered' is blank" ValidationGroup="VendorReg"></asp:RequiredFieldValidator>
<asp:RequiredFieldValidator ID="rfvtxtCompanyMemberships" runat="server" Display="none" ControlToValidate="txtCompanyMemberships" ErrorMessage="Field 'Company Memberships' is blank" ValidationGroup="VendorReg"></asp:RequiredFieldValidator>
<asp:RequiredFieldValidator ID="rfvHadLawsuit" runat="server" Display="none" ControlToValidate="rblHadLawsuit" ErrorMessage="Field 'Had Lawsuit' is blank" ValidationGroup="VendorReg"></asp:RequiredFieldValidator>
<asp:RequiredFieldValidator ID="rfvSupplyArea" runat="server" Display="none" ControlToValidate="txtSupplyArea" ErrorMessage="Field 'Supply Area' is blank" ValidationGroup="VendorReg"></asp:RequiredFieldValidator>
<asp:RequiredFieldValidator ID="rfvdrpPrimarySupplyPhase" runat="server" Display="none" ControlToValidate="drpPrimarySupplyPhase" ErrorMessage="Field 'Primary Supply Phase' is blank" ValidationGroup="VendorReg"></asp:RequiredFieldValidator>
<asp:RequiredFieldValidator ID="rfvdrpSeconarySupplyPhase" runat="server" Display="none" ControlToValidate="drpSeconarySupplyPhase" ErrorMessage="Field 'Secondary Supply Phase' is blank" ValidationGroup="VendorReg"></asp:RequiredFieldValidator>
</CT:MasterPage>