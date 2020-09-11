<%@ Page Language="VB" AutoEventWireup="false" CodeFile="terms.aspx.vb" Inherits="rebates_terms" %>
<%@ Register TagName="VendorRegistrationSteps" TagPrefix="CC" Src="~/controls/VendorRegistrationSteps.ascx" %>

<CT:MasterPage ID="CTMain" runat="server">

<CC:VendorRegistrationSteps ID="ctlSteps" runat="server" RegistrationStep="5" />


<div class="pckggraywrpr">
    <div class="pckghdgred nobdr">Rebate Terms</div>
<%--<asp:button id ="btnGoToDashBoard" text="Go to DashBoard" class="btnred" runat="server" />--%>
    <div class="largest pckgltgraywrpr nobdr pckgbdy" style="width:600px;margin:10px auto;"> 
        <p>
            <b>Current Rebate Percentage is displayed below.  Please follow these rules when updating your rebate percentage value:
                <br /><br />
                1. Value must be greater than your market's minimal rebate percentage. Please check with your operations manager if you need clarification.<br />
                2. New value must be greater that current value. If you would like to lower your rebate percentage please contact us.<br />
                3. New value will be affected immediately for current and future quarters.<br />
            </b>
        </p>
    </div>
    <CT:ErrorMessage id="ctlErrors" runat="server"></CT:ErrorMessage>
    
    <asp:UpdatePanel id="upRebates" runat="server" updatemode="conditional">
        <ContentTemplate>
            <asp:Literal id="ltlMsg" runat="server"></asp:Literal>
            <table cellpadding="4" cellspacing="1" border="0" class="tblcompr largest" style="margin: 15px 23px 15px 23px">
                <tr class="subttl">
                    <th>Current Quarter</th>
                    <th>Current Rebate Percentage</th>
                    <th>New Rebate Percentage</th>
                    <th>&nbsp;</th>
                    <th>Update History</th>
                </tr>
                <tr>
                    <td><asp:Literal id="ltlCurrentQuarter" runat="server" /></td>
                    <td><asp:Literal id="ltlCurrentRebate" runat="server" />%</td>
                    <td><asp:TextBox id="txtNewRebate" runat="server" columns="1" maxlength="4" />%</td>
                    <td><asp:Button id="btnUpdateRebate" runat="server" text="Update" cssclass="btnred" /></td>
                    <td><asp:Literal id="ltlLogMsg" runat="server" /></td>
                </tr>
                <tr class="subttl">
                    <th>Last Quarter</th>
                    <th>Last Rebate Percentage</th>
                    <th>&nbsp;</th>
                    <th>&nbsp;</th>
                    <th>Update History</th>
                </tr>
                <tr>
                    <td><asp:Literal id="ltlLastQuarter" runat="server" /></td>
                    <td><asp:Literal id="ltlLastRebate" runat="server" /></td>
                    <td></td>
                    <td></td>
                    <td><asp:Literal id="ltlLastLogMsg" runat="server" /></td>
                </tr>
            </table>
            
        </ContentTemplate>
    </asp:UpdatePanel>
    
    <p align="center" style="margin:10px;">
        <asp:Button id="btnContinue" runat="server" cssclass="btnred" />
    </p>
</div>
</CT:MasterPage>