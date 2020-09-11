<%@ Page Language="VB" AutoEventWireup="false" CodeFile="financial.aspx.vb" Inherits="forms_builder_registration_financial" %>
<%@ Import Namespace="DataLayer" %>
<%@ Register TagName="BuilderRegistrationSteps" TagPrefix="CC" Src="~/controls/BuilderRegistrationSteps.ascx" %>
<%@ Register TagName="InfoBox" TagPrefix="CC" Src="~/controls/InfoBox.ascx" %>

<CT:MasterPage id="CTMain" runat="server">
<asp:PlaceHolder runat="server">


<div style="margin:10px auto;text-align:center;">
    <CC:BuilderRegistrationSteps ID="ctrlSteps" runat="server" RegistrationStep="3" />
</div>

<asp:Panel id="pnlForm" runat="server">
<div class="pckggraywrpr">
    <div class="pckghdgred nobdr">Company Data and Projections</div>
    <table cellpadding="2" cellspacing="2" border="0" style="table-layout:fixed;">
        <tr>
            <td>&nbsp;</td>
            <td class="fieldreq">&nbsp;</td>
            <td><span class="smaller"> indicates required field</span></td>
        </tr>
        <tr>
            <td class="fieldlbl"><span id="labeltxtNumStarts" runat="server">Total Starts in <%=Sysparam.GetValue(DB,"PreviousDataYear") %>:</span></td>
            <td class="fieldreq" id="bartxtNumStarts" runat="server">&nbsp;</td>
            <td class="field"><asp:TextBox ID="txtNumStarts" runat="server" MaxLength="10" Columns="4" ValidationGroup="BuilderFinance"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="fieldlbl"><span id="labeltxtNumClosings" runat="server">Total Closings in <%=Sysparam.GetValue(DB,"PreviousDataYear") %>:</span></td>
            <td class="fieldreq" id="bartxtNumClosings" runat="server">&nbsp;</td>
            <td class="field"><asp:TextBox id="txtNumClosings" runat="server" MaxLength="10" Columns="4" ValidationGroup="BuilderFinance"></asp:TextBox></td>
        </tr>
        <tr valign="middle">
            <td class="fieldlbl">
                <span id="labeltxtDirectCosts" runat="server">Total Direct Costs in <%=Sysparam.GetValue(DB,"PreviousDataYear") %>:</span>
            </td>
            <td class="fieldreq" id="bartxtDirectCosts" runat="server">&nbsp;</td>
            <td class="field" valign="bottom">
                <asp:TextBox id="txtDirectCosts" runat="server" Maxlength="10" Columns="4" ValidationGroup="BuilderFinance"></asp:TextBox>
                <CC:InfoBox ID="ctlDirectCostInfo" runat="server" style="margin-top:5px;">                  
                    Total Material and Labor costs associated with all construction for the year.  Please exclude land and overhead.                
                </CC:InfoBox>            
            </td>
        </tr>
        <tr valign="middle">
            <td class="fieldlbl">
                <span id="labeltxtUnsold" runat="server">Total Unsold Completed Specs at the End of <%=Sysparam.GetValue(DB,"PreviousDataYear") %>:</span>
            </td>
            <td class="fieldreq" id="bartxtUnsold" runat="server">&nbsp;</td>
            <td class="field" valign="bottom">
                <asp:TextBox id="txtUnsold" runat="server" MaxLength="10" Columns="4" ValidationGroup="BuilderFinance"></asp:TextBox>
                <CC:InfoBox ID="ctlUnsoldInfo" runat="server">
                    Total completed inventory that is still for sale as of the end of the year.
                </CC:InfoBox>            
            </td>
        </tr>
        <tr valign="middle">
            <td class="fieldlbl">
                <span id="labeltxtUnderConstruction" runat="server">Total homes under construction at the end of <%=Sysparam.GetValue(DB,"PreviousDataYear") %>:</span>
            </td>
            <td class="fieldreq" id="bartxtUnderConstruction" runat="server">&nbsp;</td>
            <td class="field" valign="bottom">
                <asp:TextBox id="txtUnderConstruction" runat="server" Maxlength="10" Columns="4" ValidationGroup="BuilderFinanace"></asp:TextBox>
                <CC:InfoBox ID="ctlConstructionInfo" runat="server">
                    Total projects (specs and pre-sales) that are still being worked on as of the end of the year.                
                </CC:InfoBox>
            </td>
        </tr>
        <tr>
            <td class="fieldlbl"><span id="labeltxtNumProjected" runat="server">Projected # of Starts in <%=Sysparam.GetValue(DB,"CurrentDataYear")%>:</span></td>
            <td class="fieldreq" id="bartxtNumProjected" runat="server">&nbsp;</td>
            <td class="field"><asp:TextBox ID="txtNumProjected" runat="server" MaxLength="10" Columns="4" ValidationGroup="BuilderFinance"></asp:TextBox></td>
        </tr>
    </table>
</div>

<div class="pckggraywrpr">
    <div class="pckghdgred">Sample chart from the CBUSA Builder Dashboard:</div>
    <div style="padding:20px;background-color:#fff;text-align:center;">
        <%=CreateMoneyTrend() %>
    </div>
</div>

<p style="text-align:center;">
    <CC:OneClickButton ID="btnDashboard" runat="server" Text="Save" CssClass="btnred" ValidationGroup="BuilderFinance" />
    <asp:Button id="btnBack" runat="server" text="Go Back" onclientclick="history.go(-1);return false;" cssclass="btnred" />
    <CC:OneClickButton ID="btnContinue" runat="server" Text="Continue" CssClass="btnred" ValidationGroup="BuilderFinance" />
</p>

<CC:RequiredFieldValidatorFront ID="rfvtxtNumStarts" ControlToValidate="txtNumStarts" ErrorMessage="# Home Starts is required" ValidationGroup="BuilderFinance" runat="server"></CC:RequiredFieldValidatorFront>
<CC:IntegerValidatorFront ID="ivtxtNumStarts" ControlToValidate="txtNumStarts" ErrorMessage="# Home Starts is invalid" ValidationGroup="BuilderFinance" runat="server"></CC:IntegerValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtNumProjected" ControlToValidate="txtNumProjected" ErrorMessage="Projected # of Starts is required" ValidationGroup="BuilderFinance" runat="server"></CC:RequiredFieldValidatorFront>
<CC:IntegerValidatorFront ID="ivtxtNumProjected" ControlToValidate="txtNumProjected" ErrorMessage="Projected # of Starts is invalid" ValidationGroup="BuilderFinance" runat="server"></CC:IntegerValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtNumClosings" runat="server" ControlToValidate="txtNumClosings" ErrorMessage="# Closings is required" ValidationGroup="BuilderFinance"></CC:RequiredFieldValidatorFront>
<CC:IntegerValidatorFront id="ivftxtNumClosings" runat="server" ControlToValidate="txtNumClosings" ErrorMessage="# Closings is invalid" ValidationGroup="BuilderFinance"></CC:IntegerValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtDirectCosts" runat="server" ControlToValidate="txtDirectCosts" ErrorMessage="Direct Costs is required" ValidationGroup="BuilderFinance"></CC:RequiredFieldValidatorFront>
<CC:CustomCurrencyValidator ID="fvtxtDirectCosts" runat="server" EnableClientScript="false" ControlToValidate="txtDirectCosts" ErrorMessage="Direct Costs is invalid" ValidationGroup="BuilderFinance"></CC:CustomCurrencyValidator>
<CC:RequiredFieldValidatorFront ID="rfvtxtUnsold" runat="server" ControlToValidate="txtUnsold" ErrorMessage="End Of Year Unsold is required" ValidationGroup="BuilderFinance"></CC:RequiredFieldValidatorFront>
<CC:IntegerValidatorFront ID="ivtxtUnsold" runat="server" ControlToValidate="txtUnsold" ErrorMessage="End of Year Unsold is invalid" ValidationGroup="BuilderFinance"></CC:IntegerValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtUnderConstruction" runat="server" ControlToValidate="txtUnderConstruction" ErrorMessage="End of Year Under Construction is required" ValidationGroup="BuilderFinance"></CC:RequiredFieldValidatorFront>
<CC:IntegerValidatorFront ID="ivtxtUnderConstruction" runat="server" ControlToValidate="txtUnderConstruction" ErrorMessage="End of Year Under Construction is invalid" ValidationGroup="BuilderFinance"></CC:IntegerValidatorFront>
</asp:Panel>
</asp:PlaceHolder>
</CT:MasterPage>