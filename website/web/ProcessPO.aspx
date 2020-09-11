<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ProcessPO.aspx.vb" Inherits="ProcessPO" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Order Submitted</title>
    <script type="text/javascript" src="/includes/jquery-3.3.1.min.js"></script> 

    <script type="text/javascript">
        function ClickProcessButton() {
            document.getElementById("btnProcess").click();
        }
    </script>

    <style>
        .button {
          border: none;
          color: white;
          padding: 10px 25px;
          text-align: center;
          text-decoration: none;
          display: inline-block;
          font-size: 16px;
          font-weight: bold;
        }

        .submit {
          background-color: #4CAF50; /* Green */
        }

        .cancel {
          background-color: #9e0d1e; 
        }

        table {
            margin: 0px auto;
        }
    </style>
</head>
<body>
    <div id="divConfirm" runat="server" style="text-align:center; font:16px Verdana;">
        <p>You are about to place an Order with 84 Lumber with the following delivery details:</p>
        <table border="0">
            <tr>
                <td style="text-align:right;width:48%;">Drop Name</td>
                <td style="text-align:center;width:5px;">:</td>
                <td style="text-align:left;"><span id="spnDropName" runat="server"></span></td>
            </tr>
            <tr>
                <td style="text-align:right;">Delivery Date</td>
                <td style="text-align:center;">:</td>
                <td style="text-align:left;"><span id="spnDeliveryDate" runat="server"></span></td>
            </tr>
            <tr>
                <td style="text-align:right;">Delivery Time</td>
                <td style="text-align:center;">:</td>
                <td style="text-align:left;"><span id="spnDeliveryTime" runat="server"></span></td>
            </tr>
            <tr>
                <td colspan="3">&nbsp;</td>
            </tr>
        </table>
        <button id="btnSubmit" onclick="ClickProcessButton();" value="SUBMIT" class="button submit">SUBMIT</button>
    </div>

    <div id="divSubmit" runat="server" style="text-align:center;font:16px Verdana;display:none;">
        <p>We have submitted your order to the vendor and requested your scheduled delivery date.</p>
        <p><img src="https://f-scope.net/images/green-check-mark-png-13.jpg" height="150"/></p>
        <p>Please monitor your e-mail; you will receive confirmations shortly.</p>
        <p><i>You can now close this pop-up window.</i></p>
    </div>

    <form id="form1" runat="server">
        <div style="display:none;">
            <asp:HiddenField ID="hdnSheetId" runat="server" Value="False" ClientIDMode="Static" />
            <asp:HiddenField ID="hdnDate" runat="server" Value="False" ClientIDMode="Static" />
            <asp:HiddenField ID="hdnTime" runat="server" Value="False" ClientIDMode="Static" />
            <asp:HiddenField ID="hdnDropName" runat="server" Value="False" ClientIDMode="Static" />
            <asp:HiddenField ID="hdnProjectId" runat="server" Value="False" ClientIDMode="Static" />

            <asp:HiddenField ID="hdnIsPagePostback" runat="server" Value="False" ClientIDMode="Static" />
            <asp:Button ID="btnProcess" runat="server" Text="Process PO" ClientIDMode="Static" />
        </div>
    </form>
</body>
</html>
