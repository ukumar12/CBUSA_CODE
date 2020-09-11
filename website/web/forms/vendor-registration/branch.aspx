<%@ Page Language="VB" AutoEventWireup="false" CodeFile="branch.aspx.vb" Inherits="forms_vendor_registration_branch" %>

<CT:MasterPage ID="CTMain" runat="server">
<script src="/includes/XmlHttpLookup.js" type="text/javascript"></script>
<script type="text/javascript">
<!--     
   
	
//-->
</script>
<h4>Branch Info</h4>
<table class="regform">
    <tr>
        <td>&nbsp;</td>
        <td class="fieldreq">&nbsp;</td>
        <td><span class="smaller"> indicates required field</span></td>
    </tr>	
    <tr>
        <td class="fieldlbl"><span id="labeltxtBranchAddress" runat="server">Branch Address:</span></td>
        <td class="fieldreq" id="baRtxtBranchAddress" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtBranchAddress" runat="server" MaxLength="50" Rows="50" CssClass="regtxt"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtBranchAddress2" runat="server">Branch Address 2:</span></td>
        <td>&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtBranchAddress2" runat="Server" MaxLength="50" Rows="50" CssClass="regtxt"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtBranchCity" runat="server">Branch City:</span></td>
        <td class="fieldreq" id="bartxtBranchCity" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtBranchCity" runat="server" MaxLength="50" Rows="50" CssClass="regtxt"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeldrpBranchState" runat="server">Branch State:</span></td>
        <td class="fieldreq" id="bardrpBranchState" runat="server">&nbsp;</td>
        <td class="field"><asp:DropDownList  ID="drpBranchState" runat="server" /></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxBranchZip" runat="server">Branch Zip:</span></td>
        <td class="fieldreq" id="bartxtBranchZip" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtBranchZip" runat="server" MaxLength="15" Rows="15" CssClass="regtxtshort"></asp:TextBox></td>
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
<CC:RequiredFieldValidatorFront ID="rfvtxtBranchAddress" runat="server" ControlToValidate="txtBranchAddress" ErrorMessage="Field 'Branch Address' is blank" ValidationGroup="VendorReg"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtBranchCity" runat="server" ControlToValidate="txtBranchCity" ErrorMessage="Field 'Branch City' is blank" ValidationGroup="VendorReg"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvdrpBranchState" runat="server" ControlToValidate="drpBranchState" ErrorMessage="Field 'Branch State' is blank" ValidationGroup="VendorReg"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtBranchZip" runat="server" ControlToValidate="txtBranchZip" ErrorMessage="Field 'Branch Zip' is blank" ValidationGroup="VendorReg"></CC:RequiredFieldValidatorFront>

</CT:MasterPage>