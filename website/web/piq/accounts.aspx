<%@ Page Language="VB" AutoEventWireup="false" CodeFile="accounts.aspx.vb" Inherits="piq_accounts" %>

<CT:MasterPage ID="CTMain" runat="server">
    
    <asp:PlaceHolder runat="server">
        <script type="text/javascript">
            function OpenDeleteForm(id,name) {
                var hdn = $get('<%=hdnConfirmID.ClientID %>');
                hdn.value = id;
                var span = $get('spanConfirmName');
                span.innerHTML = name;
                var frm = $get('<%=frmDelete.ClientID %>').control;
                frm.Open();
            }
            function OpenEditForm(e) {
                if (e == 'add') {
                    document.getElementById('<%=hdnAccountID.ClientID %>').value = '';
                    document.getElementById('<%=txtFirstName.ClientID %>').value = '';
                    document.getElementById('<%=txtLastName.ClientID %>').value = '';
                    document.getElementById('<%=txtEmailAddress.ClientID %>').value = '';

                    var checks = document.querySelectorAll('input[type="checkbox"]');
                    for (var i = 0; i < checks.length; i++) {
                        var check = checks[i];
                        check.checked = false;
                    }
                }

                Sys.Application.remove_load(OpenEditForm);
                var frm = $get('<%=frmEdit.ClientID %>').control;
                frm.Open();
            }
        </script>
    </asp:PlaceHolder>
    
    <div class="pckggraywrpr">
        <div class="pckghdgred">User Accounts</div>
        <asp:Button id="btnAdd" runat="server" text="Add User" cssclass="btnred" onclientclick="OpenEditForm('add');return false;" />
        <CC:GridView ID="gvAccounts" runat="server" AllowPaging="false" AllowSorting="false" CssClass="tblcompr" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField DataField="FirstName" HeaderText="First Name" />
                <asp:BoundField DataField="LastName" HeaderText="Last Name" />
                <asp:BoundField DataField="Email" HeaderText="Email Address" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:ImageButton id="btnEdit" runat="server" commandargument='<%# DataBinder.Eval(Container.DataItem,"PIQAccountID") %>' ImageUrl="/images/admin/edit.gif" AltText="Edit" Causesvalidation="false"></asp:ImageButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:ImageButton id="btnDelete" runat="server" onclientclick='<%# "OpenDeleteForm("& DataBinder.Eval(Container.DataItem,"PIQAccountID") &","""& DataBinder.Eval(Container.DataItem,"Username") &"""); return false;" %>' causesvalidation="false" Imageurl="/images/global/icon-remove.gif"></asp:ImageButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>        
        </CC:GridView>
    </div>


        <CC:PopupForm ID="frmDelete" runat="server" CloseTriggerId="btnConfirmCancel" CssClass="pform" ShowVeil="true" VeilCloses="false" Width="300px" OpenMode="MoveToCenter">
            <FormTemplate>
                <div class="pckggraywrpr" style="margin-bottom:0px;">
                    <div class="pckghdgred">Delete User?</div>
                    <div class="bold largest center">
                        Are you sure you want to delete user '<span id="spanConfirmName"></span>'?
                        <br />
                        <br />
                        <br />
                        <asp:Button id="btnConfirmDelete" runat="server" text="Continue" cssclass="btnred" CausesValidation="false" />
                        <asp:Button id="btnConfirmCancel" runat="server" text="Cancel" cssclass="btnred" CausesValidation="false" />
                        <asp:HiddenField id="hdnConfirmID" runat="server"></asp:HiddenField>
                    </div>
                </div>
            </FormTemplate>
            <Buttons>
                <%--<CC:PopupFormButton ControlID="btnConfirmDelete" ButtonType="Postback" />--%>
                <CC:PopupFormButton ControlID="btnConfirmCancel" ButtonType="ScriptOnly" />
            </Buttons>
        </CC:PopupForm>
        
    <asp:UpdatePanel id="upEdit" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
        <ContentTemplate>
            <CC:PopupForm ID="frmEdit" runat="server"  CloseTriggerId ="btnEditCancel"  CssClass="pform" ShowVeil="true" VeilCloses="false" ErrorPlaceholderId="spanError" OpenMode="MoveToCenter">
                <FormTemplate>
                    <div class="pckggraywrpr" style="margin-bottom:0px;width:300px;">
                        <div class="pckghdgred">Edit User Account</div>
                        <table cellpadding="2" cellspacing="0" border="0">
                            <tr><td colspan="2"><span id="spanError" runat="server"></span></td></tr>
                            <tr>
                                <td><b>First Name:</b></td>
                                <td><asp:TextBox id="txtFirstName" runat="server" columns="20" maxlength="50"></asp:TextBox></td>
                                <td><asp:RequiredFieldValidator id="rfvtxtFirstName" runat="server" ControlToValidate="txtFirstName" ErrorMessage="First Name is required" ValidationGroup="UserReg"></asp:RequiredFieldValidator></td>
                            </tr>
                            <tr>
                                <td><b>Last Name:</b></td>
                                <td><asp:TextBox id="txtLastName" runat="server" columns="20" maxlength="50"></asp:TextBox></td>
                                <td><asp:RequiredFieldValidator id="rfvtxtLastName" runat="server" ControlToValidate="txtLastName" ErrorMessage="Last Name is required" ValidationGroup="UserReg"></asp:RequiredFieldValidator></td>
                            </tr>
                            <tr>
                                <td><b>Email Address:</b></td>
                                <td><asp:TextBox id="txtEmailAddress" runat="server" columns="20" maxlength="100"></asp:TextBox></td>
                                <td><CC:EmailValidator ID="evtxtEmailAddres" runat="server" ControlToValidate="txtEmailAddress" ErrorMessage="Email is invalid" ValidationGroup="UserReg"></CC:EmailValidator><asp:RequiredFieldValidator id="rftxtEmailAddress" runat="server" controltovalidate="txtEmailAddress" errorMessage="Email is required" ValidationGroup="UserReg"></asp:RequiredFieldValidator></td>
                            </tr>
                            <tr valign="top">
                                <td><b>LLC(s):</b></td>
                                <td>
                                    <CC:CheckBoxListEx ID="cblLLC" runat="server" RepeatColumns="2"></CC:CheckBoxListEx>
                                </td>
                                <td></td>
                            </tr>
                        </table>
                        <asp:HiddenField id="hdnAccountID" runat="server"></asp:HiddenField>
                    </div>
                    <asp:Button id="btnEditSave" runat="server" cssclass="btnred" Text="Save User" ValidationGroup="UserReg"  />&nbsp;
                    <asp:Button id="btnEditCancel" runat="server" cssclass="btnred" Text="Cancel" />
                </FormTemplate>
                <Buttons>
                    <%--<CC:PopupFormButton ControlID="btnEditSave" ButtonType="Postback" />--%>
                    <CC:PopupFormButton ControlID="btnEditCancel" ButtonType="ScriptOnly" />
                </Buttons>
            </CC:PopupForm>        
        </ContentTemplate>
    </asp:UpdatePanel>
</CT:MasterPage>