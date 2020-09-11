<%@ Page Language="VB" AutoEventWireup="false" CodeFile="default.aspx.vb" Inherits="forms_projects_default" %>

<CT:MasterPage ID="CTMain" runat="server">
<h1>Add Project</h1>
<table cellpadding="5" cellspacing="0" border="0">
    <tr valign="top">
        <td>&nbsp;</td>
        <td class="fieldreq">&nbsp;</td>
        <td class="field smaller"> indicates required field</td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl"><span id="labeltxtProjectName" runat="server">Project Name:</span></td>
        <td class="fieldreq" id="bartxtProjectName" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox id="txtProjectName" runat="server" columns="150" maxlength="150" style="width:319px;"></asp:TextBox></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl"><span id="labeltxtLotNo" runat="server">Lot #:</span></td>
        <td class="fieldreq" id="bartxtLotNo" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox id="txtLotNo" runat="server" columns="20" maxlength="20" style="width:319px;"></asp:TextBox></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl"><span id="labeltxtSubdivision" runat="server">Subdivision:</span></td>
        <td class="fieldreq" id="bartxtSubdivision" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox id="txtSubdivision" runat="server" maxlength="50" columns="50" style="width:319px;"></asp:TextBox></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl"><span id="labeltxtAddress1" runat="server">Address:</span></td>
        <td class="fieldreq" id="bartxtAddress1" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox id="txtAddress1" runat="server" columns="50" maxlength="50" style="width:319px;"></asp:TextBox></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl">Address 2:</td>
        <td>&nbsp;</td>
        <td class="field"><asp:TextBox id="txtAddress2" runat="server" Columns="50" maxlength="50" style="width:319px;"></asp:TextBox></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl"><span id="labeltxtCity" runat="server">City:</span></td>
        <td class="fieldreq" id="bartxtCity" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox id="txtCity" runat="server" columns="50" maxlength="50" style="width:319px;"></asp:TextBox></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl"><span id="labeldrpState" runat="server">State:</span></td>
        <td class="fieldreq" id="bardrpState" runat="server">&nbsp;</td>
        <td class="field"><asp:DropDownList id="drpState" runat="server"></asp:DropDownList></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl"><span id="labeltxtZip" runat="server">Zip:</span></td>
        <td class="fieldreq" id="bartxtZip" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox id="txtZip" runat="server" maxlength="15" columns="15" style="width:50px;"></asp:TextBox></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl"><span id="labelacCounty" runat="server">County:</span></td>
        <td class="fieldreq" id="baracCounty" runat="server">&nbsp;</td>
        <td class="field"><CC:AutoComplete ID="acCounty" runat="server" AllowNew="false"></CC:AutoComplete></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl">Tax Rate:</td>
        <td>&nbsp;</td>
        <td class="field"><span id="spanTaxRate" runat="server">Select your County above to load its tax rate.</span></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl">Portfolio:</td>
        <td>&nbsp;</td>
        <td class="field"><asp:DropDownList id="drpPortfolio" runat="server"></asp:DropDownList></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl"><span id="labeldrpStatus" runat="server">Status:</span></td>
        <td class="fieldreq" id="bardrpStatus" runat="server">&nbsp;</td>
        <td class="field"><asp:DropDownList id="drpStatus" runat="server"></asp:DropDownList></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl">Start Date:</td>
        <td>&nbsp;</td>
        <td class="field"><CC:DatePicker ID="dpStartDate" runat="server"></CC:DatePicker></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl"><span id="labelrblArchive" runat="server">Archive:</span></td>
        <td class="fieldreq" id="barrblArchive" runat="server">&nbsp;</td>
        <td class="field">
            <asp:RadioButtonList id="rblArchive" runat="server">
                <asp:ListItem value="true">Yes</asp:ListItem>
                <asp:ListItem value="false" selected="true">No</asp:ListItem>
            </asp:RadioButtonList>
        </td>
    </tr>
</table>
<asp:Button id="btnSave" runat="server" text="Add Project" ValidationGroup="ProjectForm" />

<CC:RequiredFieldValidatorFront ID="rfvProjectName" runat="server" ControlToValidate="txtProjectName" ErrorMessage="Field 'Project Name' is blank" ValidationGroup="ProjectForm"></CC:RequiredFieldValidatorFront> 
<CC:RequiredFieldValidatorFront ID="rfvAddress1" runat="server" ControlToValidate="txtAddress1" ErrorMessage="Field 'Address 1' is blank" ValidationGroup="ProjectForm"></CC:RequiredFieldValidatorFront> 
<CC:RequiredFieldValidatorFront ID="rfvCity" runat="server" ControlToValidate="txtCity" ErrorMessage="Field 'City' is blank" ValidationGroup="ProjectForm"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvState" runat="server" ControlToValidate="drpState" ErrorMessage="Field 'State' is blank" ValidationGroup="ProjectForm"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvZip" runat="server" ControlToValidate="txtZip" ErrorMessage="Field 'Zip' is blank" ValidationGroup="ProjectForm"></CC:RequiredFieldValidatorFront> 
<CC:RequiredFieldValidatorFront ID="rfvCounty" runat="server" ControlToValidate="acCounty" ErrorMessage="Field 'County' is blank" ValidationGroup="ProjectForm"></CC:RequiredFieldValidatorFront> 
<CC:RequiredFieldValidatorFront ID="rfvStatus" runat="server" ControlToValidate="drpStatus" ErrorMessage="Field 'Status' is blank" ValidationGroup="ProjectForm"></CC:RequiredFieldValidatorFront> 

<CC:DateValidatorFront ID="dvStartDate" runat="server" ControlToValidate="dpStartDate" ErrorMessage="Field 'Start Date' is invalid" ValidationGroup="ProjectForm"></CC:DateValidatorFront>

</CT:MasterPage>