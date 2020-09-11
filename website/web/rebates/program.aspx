<%@ Page Language="VB" AutoEventWireup="false" CodeFile="program.aspx.vb" Inherits="rebates_program" %>

<CT:MasterPage ID="CTMain" runat="server">

    <div class="pckggraywrpr">
        <div class="pckghdgred">Rebate Programs</div>
        <asp:UpdatePanel id="upPrograms" runat="server" UpdateMode="Always">
        <ContentTemplate>
        <CC:GridView id="gvPrograms" runat="server" AutoGenerateColumns="false" cssclass="tblcompr">
            <HeaderStyle CssClass="sbttl" />
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:Button id="btnEdit" runat="server" cssclass="btnred" text="Edit" />
                        <CC:PopupForm id="frmEdit" runat="server" CloseTriggerId="btnCancel" cssclass="pform" openmode="MoveToClick" ShowVeil="true" VeilCloses="false">
                            <FormTemplate>
                                <div class="pckghdgred">Edit Program</div>
                                <div style="padding:10px;">
                                    <table cellpadding="4" cellspacing="0" border="0">
                                        <tr>
                                            <td>&nbsp;</td>
                                            <td class="fieldreq">&nbsp;</td>
                                            <td class="field smaller"> indicates required field</td>
                                        </tr>
                                        <tr>
                                            <td><b>Program Name:</b></td>
                                            <td class="fieldreq">&nbsp;</td>
                                            <td class="field"><asp:TextBox id="txtProgramName" runat="server" columns="50" maxlength="100"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td><b>Minimum Purchase Amount:</b></td>
                                            <td>&nbsp;</td>
                                            <td class="field"><asp:TextBox id="txtMinimumAmount" runat="server" columns="10" maxlength="20"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td><b>Rebate Percentage:</b></td>
                                            <td>&nbsp;</td>
                                            <td class="field"><asp:TextBox id="txtRebatePercentage" runat="server" columns="5" maxlength="5"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td><b>Details:</b></td>
                                            <td>&nbsp;</td>
                                            <td class="field"><asp:TextBox id="txtDetails" runat="server" textmode="MultiLine" rows="4" columns="50"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td colspan="3" class="field">
                                                <asp:Button id="btnSave" runat="server" text="Save" cssclass="btnred" />
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
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="ProgramName" HeaderText="Program Name" SortExpression="ProgramName" />
                <asp:BoundField Datafield="MinimumPurchase" HeaderText="Minimum Purchase" SortExpression="MinimumPurchase" />
                <asp:BoundField DataField="RebatePercentage" HeaderText="Percentage" SortExpression="RebatePercentage" />
                <asp:BoundField DataField="Details" HeaderText="Details" />           
            </Columns>
        </CC:GridView>
        </ContentTemplate>
        </asp:UpdatePanel>
    </div>

</CT:MasterPage>