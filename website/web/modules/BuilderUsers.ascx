<%@ Control Language="VB" AutoEventWireup="false" CodeFile="BuilderUsers.ascx.vb" Inherits="modules_BuilderUsers" %>

<asp:PlaceHolder runat="server">
<script type="text/javascript">
   
    function OpenEditForm(e) {
       Sys.Application.remove_load(OpenEditForm);

       var frm = $get('<%=frmEdit.ClientID %>').control;
       frm.Open();
    }

    function OpenRemoveForm(e) {
        Sys.Application.remove_load(OpenRemoveForm);

        var frm = $get('<%=frmRemoveRequest.ClientID %>').control;
       frm.Open();
    }
  
    function ConfirmDelete(id, name) {
        var span = $get('spanConfirmName');
        span.innerHTML = name;
        var hdn = $get('<%=hdnConfirmID.ClientID %>');
        hdn.value = id;

        var frm = $get('<%=frmDelete.ClientID %>').control;
        //frm._doMoveToCenter();
        frm.Open();
    }
     
</script>
<style>
    .td-space-modification td{
        padding-left:8px;
    }
</style>
</asp:Placeholder>

<asp:Button id="btnAddUser" runat="server" text="Request To Add User Accounts" cssclass="btnblue" CausesValidation="false" />
<asp:Button id="btnRemoveUser" runat="server" text="Request To Remove User Accounts" cssclass="btnblue" CausesValidation="false" />
<asp:UpdatePanel id="upEdit" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
    <ContentTemplate>
        <CC:PopupForm ID="frmEdit" OpenMode="MoveToClick" runat="server" CloseTriggerId="btnEditCancel" CssClass="pform" ShowVeil="true" VeilCloses="false" ErrorPlaceholderId="spanError">
            <FormTemplate>
                <div class="pckggraywrpr" style="margin-bottom:0px;width:450px;">
                    <div class="pckghdgred"><span id="spanTitle">ADD USER</span></div> 
                    <table cellpadding="2" cellspacing="0" border="0">
                        <tr><td colspan="2"><span id="spanError" runat="server"></span></td></tr>
                        <tr>
                            <td><b>First Name:</b></td>
                            <td><asp:TextBox id="txtFirstName" runat="server" columns="20" maxlength="50"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td><b>Last Name:</b></td>
                            <td><asp:TextBox id="txtLastName" runat="server" columns="20" maxlength="50"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td><b>Email Address:</b></td>
                            <td><asp:TextBox id="txtEmailAddress" runat="server" columns="20" maxlength="100"></asp:TextBox></td>
                        </tr>
                         <tr>
	                        <td><b>Role:</b></td>
	                        <td><CC:CheckBoxListEx ID="F_Role" runat="server" RepeatColumns="1" RepeatDirection="Vertical" ></CC:CheckBoxListEx></td>
                        </tr>
                    </table>
                    <asp:HiddenField id="hdnBuilderAccountID" runat="server"></asp:HiddenField>
                    <%-- </div>--%>
                    <CC:RequiredFieldValidatorFront ID="rfvtxtFirstName" runat="server" ControlToValidate="txtFirstName" ErrorMessage="Field 'First Name' is blank" ValidationGroup="UserReg"></CC:RequiredFieldValidatorFront>
                    <CC:RequiredFieldValidatorFront ID="rfvtxtLastName" runat="server" ControlToValidate="txtLastName" ErrorMessage="Field 'Last Name' is blank" ValidationGroup="UserReg"></CC:RequiredFieldValidatorFront>
                    <CC:requiredfieldvalidatorfront ID="rqtxtEmailAddress" runat="server" Display="none" ControlToValidate="txtEmailAddress" ErrorMessage="Field 'Email Address' is blank" ValidationGroup="UserReg" />
                    <CC:EmailValidatorFront id="evftxtEmailAddress" runat="server" Display="none" ControlToValidate="txtEmailAddress" ErrorMessage="Field 'Email Address' is invalid" ValidationGroup="UserReg"/> 
                    <div class=" center" style="margin-top: 5px;">
                        <asp:Button id="btnEditSave" runat="server" cssclass="btnred" Text="Save Details" ValidationGroup="UserReg" />&nbsp;
                        <asp:Button id="btnEditCancel" runat="server" cssclass="btnred" Text="Cancel" />
                    </div>
                </div>
            </FormTemplate>
            <Buttons>
                <CC:PopupFormButton ControlID="btnEditSave" ButtonType="Postback" />
                <CC:PopupFormButton ControlID="btnEditCancel" ButtonType="ScriptOnly" />
            </Buttons>
        </CC:PopupForm>
    </ContentTemplate>
</asp:UpdatePanel>

<asp:UpdatePanel id="upRemoveRequest" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
    <ContentTemplate>
        <CC:PopupForm ID="frmRemoveRequest" OpenMode="MoveToClick" runat="server" CloseTriggerId="btnRemoveCancel" CssClass="pform" ShowVeil="true" VeilCloses="false">
            <FormTemplate>
                <div class="pckggraywrpr" style="margin-bottom:0px;width:320px;">
                    <div class="pckghdgred"><span id="spanTitleRemove">REMOVE USER</span></div> 
                    <div style="text-align:left;">
                        <div style="float:left;width:95px;">&nbsp;</div>
                        <asp:CheckBoxList ID="chkbxLstBuilderAccounts" runat="server" ClientIDMode="Static" TextAlign="Right" RepeatDirection="Vertical" RepeatColumns="1" RepeatLayout="Table"></asp:CheckBoxList>
                    </div>
                    <div class=" center" style="margin-top: 5px;">
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
    </ContentTemplate>
</asp:UpdatePanel>

<div class="pckghdgred">Existing User Accounts</div>
<div class="pckggraywrpr">
    <CC:GridView ID="gvAccounts" runat="server" AllowPaging="false" AllowSorting="false" CssClass="tblcomprlen td-space-modification" AutoGenerateColumns="false">
        <Columns>
            <%--<asp:TemplateField>
                <ItemTemplate>
                    <asp:ImageButton id="btnEdit" runat="server" CssClass="btnblue" commandargument='<%# DataBinder.Eval(Container.DataItem, "BuilderAccountId") %>' ImageUrl="/images/admin/edit.gif" AltText="Edit" Causesvalidation="false"></asp:ImageButton>
                </ItemTemplate>
            </asp:TemplateField>--%>
            <asp:BoundField DataField="FirstName" HeaderText="First Name" />
            <asp:BoundField DataField="LastName" HeaderText="Last Name" />
            <asp:BoundField DataField="Email" HeaderText="Email Address" />
            <asp:TemplateField HeaderText="Role">
		        <ItemTemplate>
                    <asp:Literal ID="ltlBuilderRoles" runat="server"></asp:Literal>
		        </ItemTemplate>
		    </asp:TemplateField>
            <%--<asp:TemplateField>
            <ItemTemplate>
                <asp:ImageButton id="btnDelete" CssClass="btnblue"  AlternateText ="Delete" runat="server" ImageUrl="/images/admin/delete.gif" onclientclick='<%# "ConfirmDelete(" & DataBinder.Eval(Container.DataItem, "BuilderAccountId") & ",""" & Components.Core.BuildFullName(DataBinder.Eval(Container.DataItem, "FirstName"), "", DataBinder.Eval(Container.DataItem, "LastName")) & """);return false;" %>' />
            </ItemTemplate>
        </asp:TemplateField>--%>
        </Columns>        
    </CC:GridView>
</div>
  
<div class="pckggraywrpr" style="display:none;">
    <div class="pckghdgred">Requested User Accounts</div>
    <CC:GridView ID="gvRequestedUserAccounts" runat="server" Visible="false" AllowPaging="false" AllowSorting="false" CssClass="tblcomprlen" AutoGenerateColumns="false">
            <Columns>
                <%--<asp:TemplateField>
                    <ItemTemplate>
                        <asp:ImageButton id="btnEdit" runat="server" CssClass="btnblue" commandargument='<%# DataBinder.Eval(Container.DataItem, "BuilderAccountId") %>' ImageUrl="/images/admin/edit.gif" AltText="Edit" Causesvalidation="false"></asp:ImageButton>
                    </ItemTemplate>
                </asp:TemplateField>--%>
                <asp:BoundField DataField="FirstName" HeaderText="First Name" />
                <asp:BoundField DataField="LastName" HeaderText="Last Name" />
                <asp:BoundField DataField="Email" HeaderText="Email Address" />
                <asp:TemplateField HeaderText="Role">
		            <ItemTemplate>
                        <asp:Literal ID="ltlBuilderRoles" runat="server"></asp:Literal>
		            </ItemTemplate>
		        </asp:TemplateField>
                <%--<asp:TemplateField>
                   <ItemTemplate>
                        <asp:ImageButton id="btnDelete" CssClass="btnblue"  AlternateText ="Delete" runat="server" ImageUrl="/images/admin/delete.gif" onclientclick='<%# "ConfirmDelete(" & DataBinder.Eval(Container.DataItem, "BuilderAccountId") & ",""" & Components.Core.BuildFullName(DataBinder.Eval(Container.DataItem, "FirstName"), "", DataBinder.Eval(Container.DataItem, "LastName")) & """);return false;" %>' />
                    </ItemTemplate>
                </asp:TemplateField>--%>
            </Columns>        
        </CC:GridView>
</div>

<CC:PopupForm ID="frmDelete" runat="server" OpenMode="MoveToCenter" CloseTriggerId="btnConfirmCancel" CssClass="pform" ShowVeil="true" VeilCloses="false">
        <FormTemplate>
            <div class="pckggraywrpr" style="margin-bottom:0px;">
                <div class="pckghdgred">Delete User?</div>
                    <div class="bold largest center" style="margin-top: 5px;">
                        Are you sure you want to delete user '<span id="spanConfirmName"></span>'?
                        <br />
                        <br />
                        <br />
                        <asp:Button id="btnConfirmDelete" runat="server" text="Continue" cssclass="btnred" CausesValidation="false" />
                        <asp:Button id="btnConfirmCancel" runat="server" text="Cancel" cssclass="btnred" CausesValidation="false" />
                        <asp:HiddenField id="hdnConfirmID" runat="server"></asp:HiddenField>
                    </div>
                </div>
            </div>
        </FormTemplate>
        <Buttons>
            <CC:PopupFormButton ControlID="btnConfirmDelete" ButtonType="Postback" />
            <CC:PopupFormButton ControlID="btnConfirmCancel" ButtonType="ScriptOnly" />
        </Buttons>
    </CC:PopupForm>

      