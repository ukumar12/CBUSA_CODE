<%@ Page Language="VB" AutoEventWireup="false" CodeFile="BuilderBillingPlanUpgrade.aspx.vb" Inherits="BuilderBillingPlanUpgrade" EnableViewState="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>UPGRADE BUILDER BILLING PLAN</title>
    <script type="text/javascript" src="/includes/jquery-3.3.1.min.js"></script> 

    <script type="text/javascript">
        $(document).ready(function () {
            CheckIfUpgradeInProgress();
        });

        function CheckIfUpgradeInProgress()
        {
            var ProgressStatus = document.getElementById("hdnUpgradeStatus").value;

            if (ProgressStatus == "InProgress") {
                setTimeout(ClickUpgradeButton, 3000);
            }
        }

        function ClickUpgradeButton() {
            document.getElementById("btnUpgradeBillingPlan").click();
        }
    </script>
</head>
<body>
    <form id="frmBuilderUpgradation" runat="server">
        <div style="font-family:'Courier New';font-size:12px;">
            <asp:Button ID="btnUpgradeBillingPlan" runat="server" Text="UPGRADE BILLING PLAN" Font-Names="Courier New" Font-Bold="true" />
            <asp:Button ID="btnStopProcess" runat="server" Text="STOP UPGRADE PROCESS" Font-Names="Courier New" Font-Bold="true" />
            <asp:HiddenField ID="hdnUpgradeStatus" runat="server" Value="NotStarted" />
            <asp:HiddenField ID="hdnBuilderCounter" runat="server" Value="0" />
            <br /><br />
            <asp:Table ID="tblBuilderList" runat="server" BorderColor="Navy" BorderStyle="Solid" BorderWidth="1" CellPadding="5" CellSpacing="5" BackColor="LemonChiffon">
                <asp:TableHeaderRow>
                    <asp:TableHeaderCell>
                        #
                    </asp:TableHeaderCell>
                    <asp:TableHeaderCell>
                        BUILDER ID
                    </asp:TableHeaderCell>
                    <asp:TableHeaderCell>
                        ACCOUNT ID
                    </asp:TableHeaderCell>
                    <asp:TableHeaderCell>
                        COMPANY
                    </asp:TableHeaderCell>
                    <asp:TableHeaderCell>
                        SUBSCRIPTION PLAN
                    </asp:TableHeaderCell>
                    <asp:TableHeaderCell>
                        STATUS
                    </asp:TableHeaderCell>
                </asp:TableHeaderRow>
            </asp:Table>            
        </div>
    </form>
</body>
</html>
