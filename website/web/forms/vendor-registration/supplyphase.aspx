<%@ Page Language="VB" AutoEventWireup="false" CodeFile="supplyphase.aspx.vb" Inherits="forms_vendor_registration_supplyphase" %>
<%@Register TagName="VendorRegistrationSteps" TagPrefix="CC" Src="~/controls/VendorRegistrationSteps.ascx" %>

<CT:MasterPage ID="CTMain" runat="server">
    <CC:VendorRegistrationSteps ID="ctlSteps" runat="server" RegistrationStep="6" />
    <div class="pckggraywrpr">
        <div class="pckghdgred">Select Supply Phases</div>
<%--<asp:button id ="btnGoToDashBoard" text="Go to DashBoard" class="btnred" runat="server" />--%>
        <span style="text-align:left;">
            <CC:ListSelect ID="lsSupplyPhases" runat="server" style="text-align:left;width:600px;table-layout:fixed;margin-top:5px;" Height="600" AddImageUrl="/images/admin/true.gif" DeleteImageUrl="/images/admin/delete.gif" />
        </span>
        <table cellpadding="0" cellspacing="5" border="0">
            <tr>
            <td><asp:button ID="btnSKUPrice" runat="server" Text="Save & Input Pricing" CssClass="btnred" /></td>
            <td><asp:Button id="btnDashboard" runat="server" Text="Save & Return to Dashboard" cssclass="btnred" /></td>
            </tr>
        </table>
    </div>
</CT:MasterPage>