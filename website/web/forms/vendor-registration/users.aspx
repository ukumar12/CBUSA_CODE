<%@ Page Language="VB" AutoEventWireup="false" CodeFile="users.aspx.vb" Inherits="forms_vendor_registration_terms" %>
<%@ Register TagName="VendorRegistrationSteps" TagPrefix="CC" Src="~/controls/VendorRegistrationSteps.ascx" %>
<%@Register TagName="ManageRoles" TagPrefix="CC" Src="~/modules/ManageRoles.ascx" %>

<CT:MasterPage ID="CTMain" runat="server"><%--<asp:button id ="btnGoToDashBoard" text="Go to DashBoard" class="btnred" runat="server" style="margin-left: 119px;"/>--%>
<CC:VendorRegistrationSteps ID="ctlSteps" runat="server" RegistrationStep="4"></CC:VendorRegistrationSteps>
<asp:PlaceHolder runat="server"><script type="text/javascript">
    function OpenForm() {
        var frm = $get('<%=frmEdit.ClientID %>').control;
        frm._doMoveToCenter();
        frm.Open();
    }

    function OpenRemoveForm(e) {
        Sys.Application.remove_load(OpenRemoveForm);
        var frm = $get('<%=frmRemoveRequest.ClientID %>').control;
        frm._doMoveToCenter();
        frm.Open();
    }

    function LoadAccount(id) {
        Sys.Net.WebServiceProxy.invoke('users.aspx', 'LoadAccount', false, { 'VendorAccountID': id }, LoadAccountCb, LoadAccountCb);
        return false;
    }
    function LoadAccountCb(res, ctxt) {
        if (res.get_exceptionType) return;
        var o = Sys.Serialization.JavaScriptSerializer.deserialize(res);

        $get('<%=hdnVendorAccountID.ClientID %>').value = o.ID;
    if (o.ID == -1) {
        $get('spanTitle').innerHTML = "Add User";
    }
    else {
        $get('spanTitle').innerHTML = "Edit User";
    }
    $get('<%=txtFirstName.ClientID %>').value = o.FirstName;
    $get('<%=txtLastName.ClientID %>').value = o.LastName;
    $get('<%=txtEmailAddress.ClientID %>').value = o.Email;
    OpenForm();
    }
    function ConfirmDelete(id, name) {
        var span = $get('spanConfirmName');
        span.innerHTML = name;
        var hdn = $get('<%=hdnConfirmID.ClientID %>');
    hdn.value = id;

    var frm = $get('<%=frmDelete.ClientID %>').control;
    frm._doMoveToCenter();
    frm.Open();
    }
</script>
</asp:PlaceHolder>
<CC:PopupForm ID="frmDelete" runat="server" CloseTriggerId="btnConfirmCancel" CssClass="pform" ShowVeil="true" VeilCloses="false"><FormTemplate><div class="pckggraywrpr" style="margin-bottom:0px;"><div class="pckghdgred">Delete User?</div><div class="bold largest center">Are you sure you want to delete user &#39;<span id="spanConfirmName"></span>&#39;? <br /><br /><br /><asp:Button ID="btnConfirmDelete" runat="server" cssclass="btnred" text="Continue" /><asp:Button ID="btnConfirmCancel" runat="server" cssclass="btnred" text="Cancel" /><asp:HiddenField ID="hdnConfirmID" runat="server"></asp:HiddenField>
            </div>
        </div>
    </FormTemplate>
    <Buttons><CC:PopupFormButton ButtonType="Postback" ControlID="btnConfirmDelete" /><CC:PopupFormButton ButtonType="ScriptOnly" ControlID="btnConfirmCancel" /></Buttons>
</CC:PopupForm>
        
<CC:PopupForm ID="frmEdit" runat="server" CloseTriggerId="btnEditCancel" CssClass="pform" ErrorPlaceholderId="spanError" ShowVeil="true" VeilCloses="false"><FormTemplate><div class="pckggraywrpr automargin" style="width:250px;"><div class="pckghdgred"><span id="spanTitle"></span></div>
            <table border="0" cellpadding="2" cellspacing="0" style="margin-bottom: 2px;"><tr><td colspan="2"><span id="spanError" runat="server"></span></td></tr>
                <tr><td><b>First Name:</b></td><td><asp:TextBox ID="txtFirstName" runat="server" columns="20" maxlength="50"></asp:TextBox></td></tr><tr><td><b>Last Name:</b></td><td><asp:TextBox ID="txtLastName" runat="server" columns="20" maxlength="50"></asp:TextBox></td></tr><tr><td><b>Email Address:</b></td><td><asp:TextBox ID="txtEmailAddress" runat="server" columns="20" maxlength="100"></asp:TextBox></td></tr></table><asp:HiddenField ID="hdnVendorAccountID" runat="server"></asp:HiddenField>
        <%--</div>--%>
        <CC:RequiredFieldValidatorFront ID="rfvtxtFirstName" runat="server" ControlToValidate="txtFirstName" ErrorMessage="Field 'First Name' is blank" ValidationGroup="UserReg"></CC:RequiredFieldValidatorFront><CC:RequiredFieldValidatorFront ID="rfvtxtLastName" runat="server" ControlToValidate="txtLastName" ErrorMessage="Field 'Last Name' is blank" ValidationGroup="UserReg"></CC:RequiredFieldValidatorFront><CC:RequiredFieldValidatorFront ID="rqtxtEmailAddress" runat="server" ControlToValidate="txtEmailAddress" Display="none" ErrorMessage="Field 'Email Address' is blank" ValidationGroup="UserReg" /><CC:EmailValidatorFront ID="evftxtEmailAddress" runat="server" ControlToValidate="txtEmailAddress" Display="none" ErrorMessage="Field 'Email Address' is invalid" ValidationGroup="UserReg" /><p align="center"><asp:Button ID="btnEditSave" runat="server" cssclass="btnred" Text="Save Details" />&#160; <asp:Button ID="btnEditCancel" runat="server" cssclass="btnred" Text="Cancel" /></p>
            </div>
    </FormTemplate>
    <Buttons><CC:PopupFormButton ButtonType="Postback" ControlID="btnEditSave" /><CC:PopupFormButton ButtonType="ScriptOnly" ControlID="btnEditCancel" /></Buttons>
</CC:PopupForm>

<CC:PopupForm ID="frmRemoveRequest" runat="server" CloseTriggerId="btnRemoveCancel" CssClass="pform" ShowVeil="true" VeilCloses="false">
    <FormTemplate>
        <div class="pckggraywrpr" style="margin-bottom:0px;width:375px;">
            <div class="pckghdgred"><span id="spanTitleRemove">REMOVE USER</span></div> 
            <div style="text-align:left;">
                <div style="float:left;width:95px;">&nbsp;</div>
                <asp:CheckBoxList ID="chkbxLstVendorAccounts" runat="server" TextAlign="Right" RepeatDirection="Vertical" RepeatColumns="1" RepeatLayout="Table">
                    <asp:ListItem text="User AAA" value="1234"></asp:ListItem>
                </asp:CheckBoxList>
            </div>
            <div style="margin-top: 5px;margin-right:10px;text-align:center;">
                <asp:Button id="btnRemove" runat="server" cssclass="btnred" Text="Request Removal" ValidationGroup="UserReg" />&nbsp;
                <asp:Button id="btnRemoveCancel" runat="server" cssclass="btnred" Text="Cancel" />
            </div>
        </div>
    </FormTemplate>
    <Buttons>
        <CC:PopupFormButton ControlID="btnRemove" ButtonType="Postback" />
        <CC:PopupFormButton ControlID="btnRemoveCancel" ButtonType="ScriptOnly" />
    </Buttons>
</CC:PopupForm>

<div class="pckggraywrpr"><div class="pckghdgred">User Accounts &amp; Roles</div><div class="pckgltgraywrpr" style="width:500px;margin:20px auto;z-index:1;position:relative;"><CC:ManageRoles ID="ctlRoles" runat="server" /></div>

<div style="width:95%;margin:20px auto;z-index:2;position:relative;">
    <asp:UpdatePanel ID="upResults" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="bdr larger" style="margin:10px;background-color:#fff;">
                <p align="center" style="margin:10px;">
                    <asp:Button ID="btnAdd" runat="server" cssclass="btnred" onclientclick="return LoadAccount(-1);" Text="Request To Add New User Account" />
                    <asp:Button ID="btnRemoveUser" runat="server" cssclass="btnred" Text="Request To Remove User Account" />
                </p>

                <p align="center" style="margin:10px;">
                    <h3 style="border-top: 1px solid #eee;padding-top: 5px;padding-left: 8px;">Existing User Accounts</h3>
                </p>
                <asp:Repeater ID="rptUsers" runat="server">
                    <HeaderTemplate>
                        <table border="0" cellpadding="4" cellspacing="1" class="tblcompr" style="width:100%;margin:0px">
                            <tr>
                                <th>First Name</th>
                                <th>Last Name</th>
                                <th>Email Address</th>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><%#DataBinder.Eval(Container.DataItem, "FirstName")%> </td>
                            <td><%#DataBinder.Eval(Container.DataItem, "LastName")%> </td>
                            <td><%#DataBinder.Eval(Container.DataItem, "Email")%> </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>	
            </div>
    </ContentTemplate>
</asp:UpdatePanel>
</div>

<div style="width:95%;margin:20px auto;z-index:2;position:relative;display:none;"><asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional"><ContentTemplate><div class="bdr larger" style="margin:10px;background-color:#fff;"><p align="center" style="margin:10px;"><h3 style="margin-left: 8px;">Requested User Accounts</h3></p><asp:Repeater ID="rptRequestedUser" runat="server"><HeaderTemplate><table border="0" cellpadding="4" cellspacing="1" class="tblcompr" style="width:100%;margin:0px"><tr><th>First Name</th><th>Last Name</th><th>Email Address</th></tr></HeaderTemplate><ItemTemplate><tr><td><%#DataBinder.Eval(Container.DataItem, "FirstName")%> </td>
                        <td><%#DataBinder.Eval(Container.DataItem, "LastName")%> </td>
                        <td><%#DataBinder.Eval(Container.DataItem, "Email")%> </td>
                    </tr>
                        </ItemTemplate>
                    <FooterTemplate></table></FooterTemplate>
                </asp:Repeater>	
                        
            </div>
    </ContentTemplate>
</asp:UpdatePanel>
</div>

<p align="center" style="margin:10px;">
    <asp:Button ID="btnDashboard" runat="server" cssclass="btnred" text="Return to Dashboard" />
    <asp:Button ID="btnBack" runat="server" cssclass="btnred" onclientclick="history.go(-1);return false;" text="Go Back" />
    <asp:Button ID="btnContinue" style="margin-left: 4px;" runat="server" cssclass="btnred" text="Continue" />
</p>

</div>
    
</CT:MasterPage>

