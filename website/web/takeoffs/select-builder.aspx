<%@ Page Language="VB" AutoEventWireup="false" CodeFile="select-builder.aspx.vb" Inherits="takeoffs_select_builder" %>

<CT:MasterPage id="CTMain" runat="server">
<%--<asp:button id ="btnGoToDashBoard" text="Go to DashBoard" class="btnred" runat="server" style="margin-left: 400px;" />--%>
<asp:Panel id="pnlForm" runat="server" DefaultButton="btnSelect" cssclass="pckggraywrpr" style="width:400px;margin:20px auto;margin-top:10px;">
<div class="pckghdgred">Select Builder for Takeoff</div>
<table cellpadding="5" cellspacing="0" border="0">
    <tr valign="top">
        <td>&nbsp;</td>
        <td class="fieldreq">&nbsp;</td>
        <td class="field smaller"> indicates required field</td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl"><span id="labelacBuilder" runat="server">Builder:</span></td>
        <td class="fieldreq" id="baracBuilder" runat="server">&nbsp;</td>
        <td class="field">
            <CC:SearchList ID="acBuilder" runat="server" AllowNew="false" Table="Builder" TextField="CompanyName" ValueField="BuilderId" CssClass="searchlist" Width="200"></CC:SearchList>
        </td>
    </tr>
</table>
<asp:Button id="btnSelect" runat="server" cssclass="btnred" text="Select Builder" CausesValidation="false" />

<CC:RequiredFieldValidatorFront ID="rfvacBuilder" runat="server" ControlToValidate="acBuilder" ErrorMessage="Field 'Builder' is empty" ValidationGroup="SelectBuilder"></CC:RequiredFieldValidatorFront>
</asp:Panel>
</CT:MasterPage>