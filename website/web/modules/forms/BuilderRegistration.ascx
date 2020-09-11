<%@ Control Language="VB" AutoEventWireup="false" CodeFile="BuilderRegistration.ascx.vb" Inherits="modules_forms_BuilderRegistration" %>

<table class="regform">
    <tr>
        <td>&nbsp;</td>
        <td class="fieldreq">&nbsp;</td>
        <td><span class="smaller"> indicates required field</span></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtFirstName" runat="server">First Name:</span></td>
        <td class="fieldreq" id="bartxtFirstName" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtFirstName" runat="server" MaxLength="50" Rows="50" CssClass="regtxt"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtLastName" runat="server">Last Name:</span></td>
        <td class="fieldreq" id="bartxtLastName" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtLastName" runat="Server" MaxLength="50" Rows="50" CssClass="regtxt"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtCompanyName" runat="server">Company Name:</span></td>
        <td class="fieldreq" id="bartxtCompanyName" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtCompanyName" runat="server" MaxLength="50" Rows="50" CssClass="regtxt"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtAddress" runat="server">Address:</span></td>
        <td class="fieldreq" id="bartxtAddress" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtAddress" runat="server" MaxLength="50" Rows="50" CssClass="regtxt"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl">Address line 2:</td>
        <td>&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtAddress2" runat="server" MaxLength="50" Rows="50" CssClass="regtxt"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtCity" runat="server">City:</span></td>
        <td class="fieldreq" id="bartxtCity" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtCity" runat="server" MaxLength="50" Rows="50" CssClass="regtxt"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtState" runat="server">State:</span></td>
        <td class="fieldreq" id="bartxtState" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtState" runat="server" MaxLength="2" Rows="2" CssClass="regtxtshort"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtZip" runat="server">Zip Code:</span></td>
        <td class="fieldreq" id="bartxtZip" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtZip" runat="server" MaxLength="15" Rows="15" CssClass="regtxtshort"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtPhone" runat="server">Phone:</span></td>
        <td class="fieldreq" id="bartxtPhone" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtPhone" runat="server" MaxLength="50" Rows="50" CssClass="regtxt"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtMobile" runat="server">Mobile:</span></td>
        <td class="fieldreq" id="bartxtMobile" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtMobile" runat="server" MaxLength="50" Rows="50" CssClass="regtxt"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtFax" runat="server">Fax:</span></td>
        <td class="fieldreq" id="bartxtFax" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtFax" runat="server" MaxLength="50" Rows="50" CssClass="regtxt"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtEmail" runat="server">Email:</span></td>
        <td class="fieldreq" id="bartxtEmail" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtEmail" runat="server" MaxLength="100" Rows="50" CssClass="regtxt"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtWebsiteUrl" runat="server">Website:</span></td>
        <td class="fieldreq" id="bartxtWebsiteUrl" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtWebsiteUrl" runat="server" MaxLength="100" Rows="50" CssClass="regtxt"></asp:TextBox></td>
    </tr>

    <p style="text-align:center;">
        <CC:OneClickButton ID="btnContinue" runat="server" Text="Continue" CssClass="btn" ValidationGroup="BuilderReg" />
    </p>
</table>

<CC:RequiredFieldValidatorFront ID="rfvtxtFirstName" runat="server" ControlToValidate="txtFirstName" ErrorMessage="Field 'First Name' is blank" ValidationGroup="BuilderReg"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtLastName" runat="server" ControlToValidate="txtLastName" ErrorMessage="Field 'Last Name' is blank" ValidationGroup="BuilderReg"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtCompanyName" runat='server' ControlToValidate="txtCompanyName" ErrorMessage="Field 'Company Name' is blank" ValidationGroup="BuilderReg"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtAddress" runat="server" ControlToValidate="txtAddress" ErrorMessage="Field 'Address' is blank" ValidationGroup="BuilderReg"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtCity" runat="server" ControlToValidate="txtCity" ErrorMessage="Field 'City' is blank" ValidationGroup="BuilderReg"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtState" runat="server" ControlToValidate="txtState" ErrorMessage="Field 'State' is blank" ValidationGroup="BuilderReg"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtZip" runat="server" ControlToValidate="txtZip" ErrorMessage="Field 'Zip Code' is blank" ValidationGroup="BuilderReg"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtPhone" runat="server" ControlToValidate="txtPhone" ErrorMessage="Field 'Phone' is blank" ValidationGroup="BuilderReg"></CC:RequiredFieldValidatorFront>
<CC:PhoneValidatorFront id="pvftxtPhone" runat="server" ControlToValidate="txtPhone" ErrorMessage="Field 'Phone' is invalid" ValidationGroup="BuilderReg"></CC:PhoneValidatorFront>
<CC:PhoneValidatorFront ID="pvftxtMobile" runat="server" ControlToValidate="txtMobile" ErrorMessage="Field 'Mobile' is invalid" ValidationGroup="BuilderReg"></CC:PhoneValidatorFront>
<CC:PhoneValidatorFront ID="pvftxtFax" runat="server" ControlToValidate="txtFax" ErrorMessage="Field 'Fax' is invalid" ValidationGroup="BuilderReg"></CC:PhoneValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Field 'Email' is blank" ValidationGroup="BuilderReg"></CC:RequiredFieldValidatorFront>
<CC:EmailValidatorFront id="evftxtEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Field 'Email' is invalid" ValidationGroup="BuilderReg"></CC:EmailValidatorFront>
<CC:UrlValidator ID="uvtxtWebsiteUrl" runat="server" Display="none" ControlToValidate="txtWebsiteUrl" EnableClientScript="false" ErrorMessage="Field 'Website URL' is invalid" ValidationGroup="BuilderReg"></CC:UrlValidator>
