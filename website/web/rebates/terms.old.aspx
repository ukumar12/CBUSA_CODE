<%@ Page Language="VB" AutoEventWireup="false" CodeFile="terms.old.aspx.vb" Inherits="rebates_terms_old" %>
<%@ Register TagName="VendorRegistrationSteps" TagPrefix="CC" Src="~/controls/VendorRegistrationSteps.ascx" %>

<CT:MasterPage ID="CTMain" runat="server">

<CC:VendorRegistrationSteps ID="ctlSteps" runat="server" RegistrationStep="5" />

<asp:PlaceHolder runat="server">
    <script type="text/javascript">
        function OpenTermsConfirm(event) {
            var frm = $get('<%=frmConfirm.ClientID %>');
            var rbl = $get('<%=rblProgramType.ClientID %>');
            var val = null;
            var ctls = GetProgramTypeOptions();
            for (var i = 0; i < ctls.length; i++) {
                if (ctls[i].value == 'tiered') return;
            }
            if (event.isConfirmed) {
                frm.control.event = null;
                return true;
            }
            frm.control.event = event;
            frm.control._doMoveToCenter();
            frm.control.Open();
        }
        function ConfirmOKClick(event) {
            var frm = $get('<%=frmConfirm.ClientID %>');
            if (frm.control.event) {
                var target = frm.control.event.target ? frm.control.event.target : frm.control.event.srcElement;
                var name = frm.control.event.type;
                event.isConfirmed = true;
                target.fireEvent(name,event);
            }
        }
        function GetProgramTypeOptions() {
            var ctls = [];
            for (var i = 0; i < document.forms[0].length; i++) {
                if (document.forms[0][i].id.indexOf('<%=rblProgramType.ClientID %>') >= 0) {
                    ctls.push(document.forms[0][i]);
                }
            }
            return ctls;
        }
    </script>
</asp:PlaceHolder>

<CC:PopupForm ID="frmConfirm" runat="server" CssClass="pform" CloseTriggerId="btnConfirmCancel" OpenMode="MoveToCenter" ShowVeil="true" VeilCloses="false">
    <FormTemplate>
        <div class="pckggraywrpr" style="margin-bottom:0px;">
            <div class="pckghdgred">Confirm Program Change</div>
            <div style="background-color:#fff;text-align:center;">
                <b>Warning: Changing to Flat program type will delete any existing program tiers.<br /><br />Continue?</b><br /><br />
                <asp:Button id="btnConfirmOK" runat="server" text="Continue" cssclass="btnred" />
                <asp:Button id="btnConfirmCancel" runat="server" text="Cancel" cssclass="btnred" />
            </div>
        </div>
    </FormTemplate>
    <Buttons>
        <CC:PopupFormButton ControlID="btnConfirmOK" ButtonType="ScriptOnly" />
        <CC:PopupFormButton ControlID="btnConfirmCancel" ButtonType="ScriptOnly" />
    </Buttons>
</CC:PopupForm>

<div class="pckggraywrpr">
    <div class="pckghdgred nobdr">Rebate Terms</div>

    <asp:UpdatePanel id="upTerms" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
    <ContentTemplate>

    <div class="pckghdgblue center" style="width:200px;margin:10px auto;">
        <p align="center">
            <b>Current Rebate Terms are displayed below.  Changes will be saved automatically as you add/modify the terms.</b>
        </p>
        <table cellpadding="2" cellspacing="0" border="0" style="text-align:left">
            <tr>
                <td>Program Type:</td>
                <td>
                    <asp:RadioButtonList id="rblProgramType" runat="server" autopostback="true">
                        <asp:ListItem value="flat">Flat</asp:ListItem>
                        <asp:ListItem value="tiered">Tiered</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
        </table>
    </div>
        <CT:ErrorMessage id="ctlErrors" runat="server"></CT:ErrorMessage>

        <asp:Repeater id="rptTerms" runat="server">
            <HeaderTemplate>
                <table cellpadding="4" cellspacing="1" border="0" class="tblcompr largest">
                    <tr class="subttl">
                        <th>Starting Period</th>
                        <th id="tdRangeFloor" runat="server">Purchase Range Floor</th>
                        <th id="tdRangeCeiling" runat="server">Purchase Range Ceiling</th>
                        <th id="tdRangeApplies" runat="server">Range Applies To</th>
                        <th>Rebate Percentage</th>
                        <th>&nbsp;</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><asp:Literal id="ltlPeriod" runat="server"></asp:Literal></td>
                    <td id="tdRangeFloor" runat="server"></td>
                    <td id="tdRangeCeiling" runat="server">
                        <asp:Literal id="ltlRangeCeiling" runat="server"></asp:Literal>
                        <asp:TextBox id="txtRangeCeiling" runat="server" columns="15"></asp:TextBox>
                    </td>
                    <td id="tdRangeApplies" runat="server">
                        <asp:RadioButtonList id="rblRangeApplies" runat="server">
                            <asp:ListItem value="quarter">Quarter Purchases</asp:ListItem>
                            <asp:ListItem value="annual">Annual Purchases</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                    <td id="tdRebatePercentage" runat="server">
                        <asp:Literal id="ltlRebatePercentage" runat="server"></asp:Literal>
                        <asp:TextBox id="txtRebatePercentage" runat="server" columns="5" maxlength="4"></asp:TextBox>
                    </td>
                    <td style="white-space:nowrap;">
                        <asp:Button id="btnEdit" runat="server" text="Edit" cssclass="btnred" CommandName="Edit" />
                        <asp:Button id="btnSave" runat="server" text="Save" cssclass="btnred" CommandName="Save" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"RebateTermsID") %>' />
                        <asp:Button id="btnCancel" runat="server" text="Cancel" cssclass="btnred" CommandName="Cancel" />
                        <asp:Button id="btnDelete" runat="server" text="Delete" cssclass="btnred" CommandName="Delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"RebateTermsID") %>' />
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
        <asp:Button id="btnAddTerms" runat="server" cssclass="btngold" text="Add New Tier" />

    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostbackTrigger ControlId="rblProgramType"></asp:AsyncPostbackTrigger>
        <asp:AsyncPostbackTrigger ControlId="btnAddTerms"></asp:AsyncPostbackTrigger>
    </Triggers>
    </asp:UpdatePanel>
    <p align="center" style="margin:10px;">
        <asp:Button id="btnContinue" runat="server" cssclass="btnred" />
    </p>
</div>
</CT:MasterPage>