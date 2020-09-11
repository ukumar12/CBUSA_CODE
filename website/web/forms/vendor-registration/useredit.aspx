<%@ Page Language="VB" AutoEventWireup="false" CodeFile="useredit.aspx.vb" Inherits="forms_vendor_registration_terms" %>

<CT:MasterPage ID="CTMain" runat="server">
<script src="/includes/XmlHttpLookup.js" type="text/javascript"></script>
<script type="text/javascript">
<!--

    function CheckPrimary() {

        var chkIsPrimary = document.getElementById('chkIsPrimary');
        if (chkIsPrimary.checked)
            alert("Checking this will make this the primary account and unselect the previous primary account.");
        else
            alert("If this was the primary account there will be no primary accounts set until one is edited.");
        
    }


    function CheckAvailability() {
        var txtUsername = document.getElementById('txtUsername');
        var divAvailablity = document.getElementById('divAvailability');
        if (isEmptyField(txtUsername)) {
            divAvailablity.innerHTML = '<span style=\'color=red;font-weight:bold;\'>Username cannot be blank</span>';
            return;
        }

        var xml = getXMLHTTP();
        if (xml) {
            xml.open("GET", "/ajax.aspx?f=CheckAvailbilityVendor&Username=" + txtUsername.value, true);
            xml.onreadystatechange = function() {
                if (xml.readyState == 4 && xml.responseText) {
                    if (xml.responseText.length > 0) {
                        var sUsername = txtUsername.value
                        sUsername = sUsername.replace(/</g, "&lt;");
                        sUsername = sUsername.replace(/>/g, "&gt;");
                        if (xml.responseText == 'OK') {
                            divAvailablity.innerHTML = '<span style=\'color=green;font-weight:bold;\'>Username ' + sUsername + ' is available</span>';
                        } else {
                            divAvailablity.innerHTML = '<span style=\'color=#8F0407;font-weight:bold;\'>Username ' + sUsername + ' is already taken!</span>';
                        }
                    } else {
                        divAvailablity.innerHTML = '<span style=\'color=red\'>Availability check is disabled for this version of the browser</span>';
                    }
                }
            }
            xml.send(null);
        } else {
            divAvailablity.innerHTML = '<span style=\'color=red\'>Availability check is disabled for this version of the browser</span>';
        }
    }
	
//-->
</script>
<h4>User Info</h4>
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
        <td class="fieldlbl"><span id="labelchkIsPrimary" runat="server">Is Primary:</span></td>
        <td></td>
        <td class="field"><asp:CheckBox id="chkIsPrimary"  onclick="CheckPrimary()" runat="server"></asp:CheckBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labelcblUserRoles" runat="server">User Roles:</span></td>
        <td></td>
        <td class="field"> <CC:CheckBoxListEx ID="cblUserRoles" runat="server"></CC:CheckBoxListEx></td>
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

<CC:RequiredFieldValidatorFront ID="rfvtxtFirstName" runat="server" ControlToValidate="txtFirstName" ErrorMessage="Field 'First Name' is blank" ValidationGroup="UserReg"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtLastName" runat="server" ControlToValidate="txtLastName" ErrorMessage="Field 'Last Name' is blank" ValidationGroup="UserReg"></CC:RequiredFieldValidatorFront>

</CT:MasterPage></CT:MasterPage>
