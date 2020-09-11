<%@ Page Language="VB" AutoEventWireup="false" CodeFile="memberreference.aspx.vb" Inherits="forms_vendor_registration_memberreference" %>

<CT:MasterPage ID="CTMain" runat="server">
<script src="/includes/XmlHttpLookup.js" type="text/javascript"></script>
<script type="text/javascript">
<!--     
   
	
//-->
</script>
<h4>Member Reference Info</h4>
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
        <td class="fieldlbl"><span id="labeltxtBusinessName" runat="server">Business Name:</span></td>
        <td class="fieldreq" id="bartxtBusinessName" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtBusinessName" runat="server" MaxLength="50" Rows="50" CssClass="regtxt"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl" colspan="3">
            <p style="text-align:center;">
                <CC:OneClickButton ID="btnSave" runat="server" Text="Save" CssClass="btn" ValidationGroup="VendorReg" />
                <CC:OneClickButton ID="btnDelete" runat="server" Text="Delete" CssClass="btn" />
                <CC:OneClickButton ID="btnCancel" runat="server" Text="Cancel" CssClass="btn" />
            </p>
        </td>
    </tr>

    
</table>
<CC:RequiredFieldValidatorFront ID="rfvtxtFirstName" runat="server" ControlToValidate="txtFirstName" ErrorMessage="Field 'First Name' is blank" ValidationGroup="VendorReg"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtLastName" runat="server" ControlToValidate="txtLastName" ErrorMessage="Field 'Last Name' is blank" ValidationGroup="VendorReg"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtBusinessName" runat="server" ControlToValidate="txtBusinessName" ErrorMessage="Field 'Business Name' is blank" ValidationGroup="VendorReg"></CC:RequiredFieldValidatorFront>

</CT:MasterPage>