<%@ Page Language="VB" AutoEventWireup="false" CodeFile="default.aspx.vb" Inherits="order_default" %>

<CT:MasterPage id="CTMain" runat="server">
<asp:PlaceHolder runat="server">

<script type="text/javascript">
    function UpdateName() {
        $get('<%=txtOrderTitle.ClientID %>').value = $get('<%=acProject.ClientID %>').value;
        var ProjectID = $get('<%=acProject.ClientID %>').control.get_value();
        if (ProjectID !== "") {
            UpdateOrderTaxRate($get('<%=acProject.ClientID %>').control.get_value());
        } 
    }

   function UpdateOrderTaxRate(ProjectID) {
      Sys.Net.WebServiceProxy.invoke('default.aspx', 'GetTaxRateFromProjectZip', false, { 'ProjectID': ProjectID }, UpdateOrderTaxRateCB, UpdateOrderTaxRateCB);
   }

   function UpdateOrderTaxRateCB(res, ctxt) {

       var span = $get('<%=txtTaxRate.ClientID %>');
      
    if (res.get_exceptionType) {
        $get('<%=txtTaxRate.ClientID %>').value = 'Error -- please check Zip Code';
       } else {
        $get('<%=txtTaxRate.ClientID %>').value = res;
       }
   }


//   function UpdateOrderTaxRate(res, ctxt) {

//       var span = $get('<%=txtTaxRate.ClientID %>').value;
//      
//       if (res.get_exceptionType) {
//           $get('<%=txtTaxRate.ClientID %>').value = 'Error -- please check Zip Code';
//       } else {
//           $get('<%=txtTaxRate.ClientID %>').value = res ;
//       }
//   }

    function UpdateDrops(show) {
        if (show) {
            $get('trDrops').style.display = '';
            $get('trDelivery').style.display = 'none';
        } else {
            $get('trDrops').style.display = 'none';
            $get('trDelivery').style.display = '';
        }
    }
    function ProjectFormResult(res,ctxt) {
        if(!res.errors) $get('<%=frmProject.ClientID %>').control.Close();
    }
    function UpdateTaxRate(zip) {
        Sys.Net.WebServiceProxy.invoke('/order/default.aspx', 'GetTaxRate', false, { 'Zip': zip }, UpdateTaxRateCB, UpdateTaxRateCB);
    }
    function UpdateTaxRateCB(res, ctxt) {
        var span = $get('<%=spanTaxRate.ClientID %>');
        if (res.get_exceptionType) {
            span.innerHTML = 'Error -- please check Zip Code';
        } else {
            span.innerHTML = res + '%';
        }
    }
</script>

<CC:PopupForm ID="frmProject" runat="server" cssclass="pform" style="width:425px;" OpenMode="MoveToCenter" ShowVeil="true" VeilCloses="false" Animate="true" OpenTriggerId="btnAddProject" CloseTriggerId="btnCancel">
    <FormTemplate>
        <div class="pckggraywrpr" style="margin-bottom:0px;">
            <div class="pckghdgred" style="height:15px;">Add Project</div>
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
                    <td class="fieldlbl"><span id="labeltxtProjectLotNo" runat="server">Lot #:</span></td>
                    <td>&nbsp;</td>
                    <td class="field"><asp:TextBox id="txtProjectLotNo" runat="server" columns="20" maxlength="20" style="width:319px;"></asp:TextBox></td>
                </tr>
                <tr valign="top">
                    <td class="fieldlbl"><span id="labeltxtProjectSubdivision" runat="server">Subdivision:</span></td>
                    <td>&nbsp;</td>
                    <td class="field"><asp:TextBox id="txtProjectSubdivision" runat="server" maxlength="50" columns="50" style="width:319px;"></asp:TextBox></td>
                </tr>
                <tr valign="top">
                    <td class="fieldlbl"><span id="labeltxtProjectAddress1" runat="server">Address:</span></td>
                    <td class="fieldreq" id="bartxtProjectAddress1" runat="server">&nbsp;</td>
                    <td class="field"><asp:TextBox id="txtProjectAddress1" runat="server" columns="50" maxlength="50" style="width:319px;"></asp:TextBox></td>
                </tr>
                <tr valign="top">
                    <td class="fieldlbl">Address 2:</td>
                    <td>&nbsp;</td>
                    <td class="field"><asp:TextBox id="txtProjectAddress2" runat="server" Columns="50" maxlength="50" style="width:319px;"></asp:TextBox></td>
                </tr>
                <tr valign="top">
                    <td class="fieldlbl"><span id="labeltxtProjectCity" runat="server">City:</span></td>
                    <td class="fieldreq" id="bartxtProjectCity" runat="server">&nbsp;</td>
                    <td class="field"><asp:TextBox id="txtProjectCity" runat="server" columns="50" maxlength="50" style="width:319px;"></asp:TextBox></td>
                </tr>
                <tr valign="top">
                    <td class="fieldlbl"><span id="labeldrpProjectState" runat="server">State:</span></td>
                    <td class="fieldreq" id="bardrpProjectState" runat="server">&nbsp;</td>
                    <td class="field"><asp:DropDownList id="drpProjectState" runat="server"></asp:DropDownList></td>
                </tr>
                <tr valign="top">
                    <td class="fieldlbl"><span id="labeltxtProjectZip" runat="server">Zip:</span></td>
                    <td class="fieldreq" id="bartxtProjectZip" runat="server">&nbsp;</td>
                    <td class="field"><asp:TextBox id="txtProjectZip" runat="server" maxlength="15" columns="15" style="width:50px;" onchange="UpdateTaxRate(this.value);"></asp:TextBox></td>
                </tr>
                <tr valign="top">
                    <td class="fieldlbl">Tax Rate:</td>
                    <td>&nbsp;</td>
                    <td class="field"><span id="spanTaxRate" runat="server">Enter your Zip Code above to load its tax rate.</span></td>
                </tr>
                <tr valign="top">
                    <td class="fieldlbl">Portfolio:</td>
                    <td>&nbsp;</td>
                    <td class="field"><asp:DropDownList id="drpProjectPortfolio" runat="server"></asp:DropDownList></td>
                </tr>
                <tr valign="top">
                    <td class="fieldlbl"><span id="labeldrpProjectStatus" runat="server">Status:</span></td>
                    <td class="fieldreq" id="bardrpProjectStatus" runat="server">&nbsp;</td>
                    <td class="field"><asp:DropDownList id="drpProjectStatus" runat="server"></asp:DropDownList></td>
                </tr>
                <tr valign="top">
                    <td class="fieldlbl">Start Date:</td>
                    <td>&nbsp;</td>
                    <td class="field"><CC:DatePicker ID="dpProjectStartDate" runat="server"></CC:DatePicker></td>
                </tr>
                <tr valign="top">
                    <td class="fieldlbl"><span id="labelrblProjectArchive" runat="server">Archive:</span></td>
                    <td class="fieldreq" id="barrblProjectArchive" runat="server">&nbsp;</td>
                    <td class="field">
                        <asp:RadioButtonList id="rblProjectArchive" runat="server">
                            <asp:ListItem value="true">Yes</asp:ListItem>
                            <asp:ListItem value="false" selected="true">No</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
            </table>
            <asp:Button id="btnSaveProject" runat="server" cssclass="btnred" text="Add Project" ValidationGroup="ProjectForm" />
            <asp:Button id="btnCancel" runat="server" cssclass="btnred" text="Cancel"></asp:Button>
        </div>
        <CC:RequiredFieldValidatorFront ID="rfvProjectName" runat="server" ControlToValidate="txtProjectName" ErrorMessage="Field 'Project Name' is blank" ValidationGroup="ProjectForm"></CC:RequiredFieldValidatorFront> 
        <CC:RequiredFieldValidatorFront ID="rfvAddress1" runat="server" ControlToValidate="txtProjectAddress1" ErrorMessage="Field 'Address 1' is blank" ValidationGroup="ProjectForm"></CC:RequiredFieldValidatorFront> 
        <CC:RequiredFieldValidatorFront ID="rfvCity" runat="server" ControlToValidate="txtProjectCity" ErrorMessage="Field 'City' is blank" ValidationGroup="ProjectForm"></CC:RequiredFieldValidatorFront>
        <CC:RequiredFieldValidatorFront ID="rfvState" runat="server" ControlToValidate="drpProjectState" ErrorMessage="Field 'State' is blank" ValidationGroup="ProjectForm"></CC:RequiredFieldValidatorFront>
        <CC:RequiredFieldValidatorFront ID="rfvZip" runat="server" ControlToValidate="txtProjectZip" ErrorMessage="Field 'Zip' is blank" ValidationGroup="ProjectForm"></CC:RequiredFieldValidatorFront> 
        <CC:RequiredFieldValidatorFront ID="rfvStatus" runat="server" ControlToValidate="drpProjectStatus" ErrorMessage="Field 'Status' is blank" ValidationGroup="ProjectForm"></CC:RequiredFieldValidatorFront> 

        <CC:DateValidatorFront ID="dvStartDate" runat="server" ControlToValidate="dpProjectStartDate" ErrorMessage="Field 'Start Date' is invalid" ValidationGroup="ProjectForm"></CC:DateValidatorFront>
    </FormTemplate>
    <Buttons>
        <CC:PopupFormButton ControlId="btnSaveProject" ButtonType="Callback" ClientCallback="ProjectFormResult" />
        <CC:PopupFormButton ControlId="btnCancel" ButtonType="ScriptOnly" />
    </Buttons>
</CC:PopupForm>

<div class="pckggraywrpr" style="width:600px;margin:auto;">
<div class="pckghdgred">Submit Order</div>

<table cellpadding="2" cellspacing="2" border="0">
    <tr valign="top">
        <td>&nbsp;</td>
        <td class="fieldreq">&nbsp;</td>
        <td class="bold smaller"> indicates required field</td>
    </tr>
    <tr valign="top" style="position:relative;z-index:2;">
        <td class="fieldlbl"><span id="labelacProject" runat="server" style="position:relative;z-index:9;">Project:</span></td>
        <td class="fieldreq" id="baracProject" runat="server">&nbsp;</td>
        <td class="field">
            <CC:SearchList id="acProject" runat="server" OnClientTextChanged="UpdateName();"  OnClientValueUpdated="UpdateName();" AllowNew="false" Table="Project" TextField="ProjectName" ValueField="ProjectId"  CssClass="searchlist" style="width:150px;" ViewAllLength="15"></CC:SearchList>
            <asp:Button id="btnAddProject" runat="server" cssclass="btnred" text="Add Project" /> 
        </td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl"><span id="labeltxtOrderTitle" runat="server">Order Title:</span></td>
        <td class="fieldreq" id="bartxtOrderTitle" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox id="txtOrderTitle" runat="server" columns="50" maxlength="100"></asp:TextBox></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl"><span id="labeltxtPONumber" runat="server">PO Number:</span></td>
        <td class="fieldreq" id="bartxtPONumber" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox id="txtPONumber" runat="server" columns="20" maxlength="20"></asp:TextBox></td>
    </tr>
    <tr valign="top" style="position:relative;z-index:1;">
        <td class="fieldlbl"><span id="labelacVendorAccount" runat="server">Vendor Sales Rep:</span></td>
        <td class="fieldreq" id="baracVendorAccount" runat="server">&nbsp;</td>
        <td class="field"><CC:SearchList ID="acVendorAccount" runat="server" AllowNew="false" CssClass="searchlist" Width="150px" SearchFunction="GetVendorUsers" Table="VendorAccount" TextField="Username" ValueField="VendorAccountID" ViewAllLength="10" /></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl"><span id="labeltxtOrdererFirstName" runat="server">Order-Placer First Name:</span></td>
        <td class="fieldreq" id="bartxtOrdererFirstName" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox id="txtOrdererFirstName" runat="server" columns="50" maxlength="50"></asp:TextBox></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl"><span id="labeltxtOrdererLastName" runat="server">Order-Placer Last Name:</span></td>
        <td class="fieldreq" id="bartxtOrdererLastName" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox id="txtOrdererLastName" runat="server" columns="50" maxlength="50"></asp:TextBox></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl"><span id="labeltxtOrdererEmail" runat="server">Order-Placer Email:</span></td>
        <td class="fieldreq" id="bartxtOrdererEmail" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox id="txtOrdererEmail" runat="server" columns="50" maxlength="100"></asp:TextBox></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl"><span id="labeltxtOrdererPhone" runat="server">Order-Placer Phone:</span></td>
        <td class="fieldreq" id="bartxtOrdererPhone" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox id="txtOrdererPhone" runat="server" columns="50" maxlength="50"></asp:TextBox></td>
    </tr>

     <tr valign="top">
     <input type="hidden" id="hdnProjectZip" runat="server" /> 
        <td class="fieldlbl"><span id="labeltxtTaxRate" runat="server">Tax Rate (In %):</span></td>
        <td class="fieldreq" id="bartxtTaxRate" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox id="txtTaxRate" runat="server" columns="50" maxlength="50"></asp:TextBox></td>
    </tr>

    <tr valign="top">
        <td class="fieldlbl"><span id="labeltxtSuperFirstName" runat="server">Site Super. First Name:</span></td>
        <td>&nbsp;</td>
        <td class="field"><asp:TextBox id="txtSuperFirstName" runat="server" columns="50" maxlength="50"></asp:TextBox></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl"><span id="labeltxtSuperLastName" runat="server">Site Super. Last Name:</span></td>
        <td>&nbsp;</td>
        <td class="field"><asp:TextBox id="txtSuperLastName" runat="server" columns="50" maxlength="50"></asp:TextBox></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl"><span id="labeltxtSuperEmail" runat="server">Site Super. Email:</span></td>
        <td>&nbsp;</td>
        <td class="field"><asp:TextBox id="txtSuperEmail" runat="server" columns="50" maxlength="100"></asp:TextBox></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl"><span id="labeltxtSuperPhone" runat="server">Site Super. Phone:</span></td>
        <td>&nbsp;</td>
        <td class="field"><asp:Textbox id="txtSuperPhone" runat="server" columns="50" maxlength="50"></asp:Textbox></td>
    </tr>
    <tr valign="top" id="trDelivery">
        <td class="fieldlbl"><span id="labeldpRequestedDelivery" runat="server">Requested Delivery:</span></td>
        <td>&nbsp;</td>
        <td class="field"><CC:DatePicker ID="dpRequestedDelivery" runat="server"></CC:DatePicker></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl"><span id="labeltxtDeliveryInstructions" runat="server">Shipping/Delivery Instructions:</span></td>
        <td>&nbsp;</td>
        <td class="field"><asp:TextBox id="txtDeliveryInstructions" runat="server" textmode="MultiLine" columns="50" rows="3"></asp:TextBox></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl"><span id="labeltxtNotes" runat="server">Notes/Comments:</span></td>
        <td>&nbsp;</td>
        <td class="field"><asp:TextBox id="txtNotes" runat="server" textmode="MultiLine" columns="50" rows="3"></asp:TextBox></td>
    </tr>
    <tr valign="top" id="trRbDrops" runat="server">
        <td class="fieldlbl"><span id="labelrbDrops" runat="server">Set up Drops:</span></td>
        <td class="fieldreq" id="barrbDrops" runat="server">&nbsp;</td>
        <td class="field">
            <asp:RadioButton id="rbDropsYes" runat="server" value="true" text="Yes" GroupName="rbDrops" OnClick="UpdateDrops(true);"></asp:RadioButton>
            <asp:RadioButton id="rbDropNo" runat="server" checked="true" value="false" text="No" GroupName="rbDrops" OnClick="UpdateDrops(false);"></asp:RadioButton>
        </td>
    </tr>
    <tr valign="top" id="trDrops" style="display:none;">
        <td colspan="2">&nbsp;</td>
        <td>
            <CC:SubForm id="sfDrops" runat="server" InitialRows="1">
                <Fields>
                    <CC:SubFormField FieldName="Name" runat="server">
                        <HtmlTemplate>
                            <asp:TextBox id="txtDropName" runat="server" columns="50" maxlength="50" style="width:75px;"></asp:TextBox>
                            <div id="divDropNameError" runat="server" class="red smallest bold" style="display:none;">Name is required</div>
                        </HtmlTemplate>
                        <Inputs>
                            <CC:SubFormInput ServerId="txtDropName" ErrorSpanId="divDropNameError" ValidateFunction="function() {return !isEmptyField(this)}" />
                        </Inputs>
                    </CC:SubFormField>
                    <CC:SubFormField FieldName="Requested Delivery" runat="server">
                        <HtmlTemplate>
                            <CC:DatePicker ID="dpRequestedDelivery" runat="server"></CC:DatePicker>
                            <div id="divRequestedDeliveryError" runat="server" class="red smallest bold" style="display:none;">Requested Delivery is invalid</div>                        </HtmlTemplate>
                        <Inputs>
                            <CC:SubFormInput ServerId="dpRequestedDelivery" ErrorSpanId="divRequestedDeliveryError"   ValidateFunction="function() {return isEmptyField(this)}" />
                        </Inputs>
                    </CC:SubFormField>
                </Fields>
            </CC:SubForm>
        </td>
    </tr>
</table>

<CC:SubFormValidator id="sfvDrops" runat="server" ControlToValidate="sfDrops" ErrorMessage="One or more drops is invalid" ValidationGroup="AddProject"></CC:SubFormValidator>

<CC:RequiredFieldValidatorFront ID="rfvacProject" runat="server" ControlToValidate="acProject" ErrorMessage="Project is required" ValidationGroup="AddProject"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtOrderTItle" runat="server" ControlToValidate="txtOrderTitle" ErrorMessage="Title is required" ValidationGroup="AddProject"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtPONumber" runat='server' ControlToValidate="txtPONumber" ErrorMessage="PO Number is required" ValidationGroup="AddProject"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtOrdererFirstName" runat="server" ControlToValidate="txtOrdererFirstName" ErrorMessage="Orderer First Name is required" ValidationGroup="AddProject"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtOrdererLastName" runat="server" ControlToValidate="txtOrdererLastName" ErrorMessage="Orderer Last Name is required" ValidationGroup="AddProject"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtOrdererEmail" runat="server" ControlToValidate="txtOrdererEmail" ErrorMessage="Orderer Email is required" ValidationGroup="AddProject"></CC:RequiredFieldValidatorFront>
<CC:EmailValidatorFront ID="evftxtOrdererEmail" runat='server' ControlToValidate="txtOrdererEmail" ErrorMessage="Orderer Email is invalid" ValidationGroup="AddProject"></CC:EmailValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtOrdererPhone" runat="server" ControlToValidate="txtOrdererPhone" ErrorMessage="Orderer Phone is required" ValidationGroup="AddProject"></CC:RequiredFieldValidatorFront>
<CC:PhoneValidatorFront ID="pvftxtOrdererPhone" runat="server" ControlToValidate="txtOrdererPhone" ErrorMEsage="Orderer Phone is invalid" ValidationGroup="AddProject"></CC:PhoneValidatorFront>

<%--<CC:RequiredFieldValidatorFront ID="rfvtxtSuperFirstName" runat="server" ControlToValidate="txtSuperFirstName" ErrorMessage="Supervisor First Name is required" ValidationGroup="AddProject"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtSuperLastName" runat='server' ControlToValidate="txtSuperLastName" ErrorMessage="Supervisor Last Name is required" ValidationGroup="AddProject"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtSuperEmail" runat="server" ControlToValidate="txtSuperEmail" ErrorMessage="Supervisor Email is required" ValidationGroup="AddProject"></CC:RequiredFieldValidatorFront>--%>
<CC:EmailValidatorFront ID="evftxtSuperEmail" runat="server" ControlToValidate="txtSuperEmail" ErrorMessage="Supervisor Email is invalid" ValidationGroup="AddProject"></CC:EmailValidatorFront>
<%--<CC:RequiredFieldValidatorFront ID="rfvtxtSuperPhone" runat="server" ControlToValidate="txtSuperPhone" ErrorMessage="Supervisor Phone is required" ValidationGroup="AddProject"></CC:RequiredFieldValidatorFront>
--%><CC:PhoneValidatorFront ID="pvftxtSuperPhone" runat="server" ControlToValidate="txtSuperPhone" ErrorMEsage="Supervisor Phone is invalid" ValidationGroup="AddProject"></CC:PhoneValidatorFront>

<p style="text-align:center;">
    <asp:Button id="btnSubmit" runat="server" text="Continue" Cssclass="btnred" ValidationGroup="AddProject" />
</p>
</div>
</asp:PlaceHolder>
</CT:MasterPage>