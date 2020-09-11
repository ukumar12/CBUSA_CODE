<%@ Page Language="VB" AutoEventWireup="false" CodeFile="custom.aspx.vb" Inherits="rebates_custom" %>

<CT:MasterPage id="CTMain" runat="server">

    <asp:PlaceHolder runat="server">
        <script>
            function OpenEditForm(e) {
                if(e) Sys.Application.remove_load(OpenEditForm);
                var frm = $get('<%=frmRebate.ClientID %>').control;
                frm.Open();
            }
            function OpenProgramForm(e) {
                if (e) Sys.Application.remove_load(OpenProgramForm);
                var frm = $get('<%=frmProgram.ClientID %>').control
                frm.Open();
            }
        </script>
    </asp:PlaceHolder>

    <asp:UpdatePanel id="upProgramForm" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
        <ContentTemplate>
            <CC:PopupForm id="frmProgram" runat="server" CloseTriggerId="btnCancel" cssclass="pform" openmode="MoveToClick" ShowVeil="true" VeilCloses="false">
                <FormTemplate>
                    <div class="pckghdgred">Edit Program</div>
                    <div style="padding:10px;">
                        <table cellpadding="4" cellspacing="0" border="0">
                            <tr>
                                <td>&nbsp;</td>
                                <td class="fieldreq">&nbsp;</td>
                                <td class="field smaller"> indicates required field</td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td><b>Program Name:</b></td>
                                <td class="fieldreq">&nbsp;</td>
                                <td class="field"><asp:TextBox id="txtProgramName" runat="server" columns="50" maxlength="100"></asp:TextBox></td>
                                <td><asp:RequiredFieldValidator id="rfvtxtProgramName" runat="server" ControlToValidate="txtProgramName" ErrorMessage="Program Name is required" ValidationGroup="Program"></asp:RequiredFieldValidator></td>
                            </tr>
                            <tr>
                                <td><b>Minimum Purchase Amount:</b></td>
                                <td>&nbsp;</td>
                                <td class="field"><asp:TextBox id="txtProgramMinimumAmount" runat="server" columns="10" maxlength="20"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td><b>Rebate Percentage:</b></td>
                                <td>&nbsp;</td>
                                <td class="field"><asp:TextBox id="txtProgramRebatePercentage" runat="server" columns="5" maxlength="5"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td><b>Details:</b></td>
                                <td>&nbsp;</td>
                                <td class="field"><asp:TextBox id="txtProgramDetails" runat="server" textmode="MultiLine" rows="4" columns="50"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td colspan="3" class="field">
                                    <asp:Button id="btnSave" runat="server" text="Save" cssclass="btnred" validationgroup="Program" />
                                    <asp:Button id="btnCancel" runat="server" text="Cancel" cssclass="btnred" />
                                    <asp:HiddenField id="hdnCustomRebateProgramId" runat="server"></asp:HiddenField>
                                </td>
                            </tr>
                        </table>
                    </div>
                </FormTemplate>
                <Buttons>
                    <CC:PopupFormButton ControlId="btnSave" ButtonType="Postback"></CC:PopupFormButton>
                    <CC:PopupFormButton ControlId="btnCancel" ButtonType="ScriptOnly"></CC:PopupFormButton>
                </Buttons>
            </CC:PopupForm>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel id="upRebateForm" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
        <ContentTemplate>
            <CC:PopupForm ID="frmRebate" runat="server" CssClass="pform" OpenMode="MoveToCenter" CloseTriggerId="btnCancelRebate" ShowVeil="true" VeilCloses="false" Width="500px">
                <FormTemplate>
                    <div class="pckggraywrpr" style="margin-bottom:0px;">
                        <div class="pckghdgred">Add/Edit Custom Rebate</div>
                        <table cellpadding="5" cellspacing="0" border="0">
                            <tr>
                                <td>&nbsp;</td>
                                <td class="fieldreq">&nbsp;</td>
                                <td colspan="2" class="field smaller"> indicates required field</td>
                            </tr>

                            <tr>
                                <td class="bold">Builder:</td>
                                <td class="fieldreq">&nbsp;</td>
                                <td><asp:DropDownList id="drpBuilder" runat="server"></asp:DropDownList></td>
                                <td><asp:RequiredFieldValidator id="rfvdrpBuilder" runat="server" COntrolToValidate="drpBuilder" ErrorMessage="Builder is required" ValidationGroup="RebateForm"></asp:RequiredFieldValidator></td>
                            </tr>
                            <tr>
                                <td class="bold">Program:</td>
                                <td>&nbsp;</td>
                                <td><asp:DropDownList id="drpProgram" runat="server" autopostback="true"></asp:DropDownList></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td class="bold">Minimum Purchase Amount:</td>
                                <td>&nbsp;</td>
                                <td><asp:TextBox id="txtMinimumPurchase" runat="server" columns="10"></asp:TextBox></td>
                                <td><CC:CurrencyValidator ID="fvtxtMinimumPurchase" runat="server" ControlToValidate="txtMinimumPurchase" ErrorMessage="Amount is invalid" ValidationGroup="RebateForm"></CC:CurrencyValidator></td>
                            </tr>
                            <tr>
                                <td class="bold">Rebate Percentage:</td>
                                <td>&nbsp;</td>
                                <td><asp:TextBox id="txtRebatePercentage" runat="server" columns="10"></asp:TextBox></td>
                                <td><CC:FloatValidator ID="fvtxtRebatePercentage" runat="server" ControlToValidate="txtRebatePercentage" ValidationGroup="RebateForm"></CC:FloatValidator></td>
                            </tr>
                            <tr>
                                <td class="bold">Applicable Purchase Amount:</td>
                                <td>&nbsp;</td>
                                <td><asp:TextBox id="txtApplicablePurchaseAmount" runat="server" columns="10"></asp:TextBox></td>
                                <td><CC:CurrencyValidator ID="cvtxtApplicablePurchaseAmount" runat="server" ControlToValidate="txtApplicablePurchaseAmount" ErrorMessage="Amount is invalid" ValidationGroup="RebateForm"></CC:CurrencyValidator></td>
                            </tr>                      
                            <tr>
                                <td class="bold">Rebate Amount:</td>
                                <td class="fieldreq">&nbsp;</td>
                                <td><asp:TextBox id="txtRebateAmount" runat="server" columns="10"></asp:TextBox></td>
                                <td><CC:CurrencyValidator ID="cvtxtRebateAmount" runat="server" ControlToValidate="txtRebateAmount" ErrorMessage="Amount is invalid" ValidationGroup="RebateForm"></CC:CurrencyValidator><asp:RequiredFieldValidator id="rfvtxtRebateAmount" runat="server" controltovalidate="txtRebateAmount" errormessage="Rebate Amount is required" ValidationGroup="RebateForm"></asp:RequiredFieldValidator></td>
                            </tr>
                            <tr>
                                <td>Details:</td>
                                <td>&nbsp;</td>
                                <td><asp:TextBox id="txtDetails" runat="server" textmode="MultiLine" columns="50" rows="3"></asp:TextBox></td>
                                <td></td>
                            </tr>                                      
                            <tr>
                                <td colspan="4" align="center">
                                    <asp:Button id="btnSubmitRebate" runat="server" text="Submit" cssclass="btnred" ValidationGroup="RebateForm" />
                                    <asp:Button id="btnCancelRebate" runat="server" text="Cancel" cssclass="btnred" Causesvalidation="false" />
                                    <asp:HiddenField id="hdnCustomRebateID" runat="server"></asp:HiddenField>
                                </td>
                            </tr>
                        </table>
                    </div>
                </FormTemplate>
                <Buttons>
                    <CC:PopupFormButton ControlID="btnSubmitRebate" ButtonType="Postback" />
                    <CC:PopupFormButton ControlID="btnCancelRebate" ButtonType="ScriptOnly" />
                </Buttons>
            </CC:PopupForm>
        </ContentTemplate>
    </asp:UpdatePanel>

    <div class="pckggraywrpr">
        <p align="center">
            <asp:Button id="btnPrograms" runat="server" text="Add Rebate Programs" cssclass="btnred" onclientclick="OpenProgramForm();return false;" />
            <asp:Button id="btnAdd" runat="server" text="Add Custom Rebate Terms" cssclass="btnred" onclientclick="OpenEditForm();return false;" />
        </p>
        <div class="pckghdgred">Rebate Programs</div>
        <CC:GridView id="gvPrograms" runat="server" AutoGenerateColumns="false" AllowPaging="false" AllowSorting="false" CssClass="tblcompr">
            <Columns>
                <asp:BoundField DataField="ProgramName" HeaderText="Name" />
                <asp:BoundField DataField="MinimumPurchase" HeaderText="Minimu Purchase" />
                <asp:BoundField DataField="Rebatepercentage" HeaderText="Rebate Percentage" />
                <asp:BoundField DataField="Details" HeaderText="Details" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:Button id="btnEdit" runat="server" text="Edit" cssclass="btnred" commandargument='<%#DataBinder.Eval(Container.DataItem,"CustomRebateProgramID") %>' causesvalidation="false" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </CC:GridView>
        <div class="pckghdgred">Custom Rebate Terms</div>
        <CC:GridView ID="gvTerms" runat="server" AutoGenerateColumns="false" AllowPaging="false" AllowSorting="true" CssClass="tblcompr">
            <Columns>
                <asp:BoundField DataField="CompanyName" HeaderText="Builder" SortExpression="CompanyName" />
                <asp:BoundField DataField="ProgramName" HeaderText="Program Name" sortExpression="ProgramName" />
                <asp:BoundField DataField="RebateQuarter" HeaderText="Quarter" SortExpression="RebateQuarter" />
                <asp:BoundField DataField="RebateYear" HeaderText="Year" SortExpression="RebeateYear" />
                <asp:BoundField DataField="MinimumPurchase" HeaderText="Program Minimum Purchase" SortExpression="MinimumPurchase" />
                <asp:BoundField DataField="RebatePercentage" HeaderText="Program Rebate Percentage" SortExpression="RebatePercentage" />
                <asp:BoundField DataField="ApplicablePurchaseAmount" HeaderText="Applicable Purchase Amount" SortExpression="ApplicablePurchaseAmount" />
                <asp:BoundField DataField="RebateAmount" HeaderText="Rebate Amount" SortExpression="RebateAmount" />
                <asp:BoundField DataField="Details" HeaderText="Details" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:Button id="btnEdit" runat="server" text="Edit" cssclass="btnred" commandargument='<%#DataBinder.Eval(Container.DataItem,"CustomRebateID") %>' causesvalidation="false" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </CC:GridView>
    </div>
</CT:MasterPage>