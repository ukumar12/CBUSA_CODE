<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TestMail.aspx.vb" Inherits="TestMail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function ConfirmDelete() {
            if (confirm("are you sure?") == true) {
                return true;
            } else {
                return false;
            }
        }

        function confirmClick(args) {
            window.returnValue = args;
            window.close();
        }

        function ShowConfirmationDiv() {
            document.getElementById('dlgMessage').style.display = "block";
        }
    </script>
</head>
<body>
    <div id="dlgMessage" style="display:none;border:solid 1px black;background-color:gray;z-index:99;">
        <span>Are you sure you want to delete the projects? . . .</span>
        <input type="button" value="Yes" id="btnYes" onclick="javascript:confirmClick('yes')"/>
        <input type="button" value="No" id="btnNo" onclick="javascript:confirmClick('no');"/>
    </div>

    <button id="btnTestDialog" onclick="ShowConfirmationDiv();" value="">Confirm Delete</button>

    <form id="form1" runat="server">
    <div>
        <asp:Button ID="btnSendEmail" runat="server" Text="Send Email" />
        <asp:CheckBox ID="RblPortfolio" AutoPostBack="true" Text="Portfolio"  Checked="true"  runat="server" onclick="if (ConfirmDelete() == false) return false;" OnCheckedChanged="RblPortfolio_CheckedChanged" />

        <asp:CheckBox ID="RblCustom" AutoPostBack="true" Text="Custom"  Checked="true"  runat="server" onclick="if (ConfirmDelete() == false) return false;" OnCheckedChanged="RblCustom_CheckedChanged" />
    </div>

        <div id="mailContent" style="" runat="server">
                <table align="center" width="390" border="0" cellspacing="0" cellpadding="0" style="line-height: 0px" id="dvEventDetails" runat="server">
                    <tr>
                        <td align="center" valign="middle" style="line-height: 0px">
                            <table width="100%" cellspacing="0" cellpadding="0" style="height: 370px; border: 2px solid #2f567a; background-color: rgba(255,255,255,0.5); border-radius: 10px; line-height: 0px;">
                                <tr>
                                    <td align="left" valign="top" style="padding: 6px;" style="line-height: 0px">
                                        <table width="100%" border="0" cellspacing="0" cellpadding="0" style="line-height: 0px">
                                            <tr>
                                                <td align="left" valign="top" style="line-height: 0px">
                                                    <table width="100%" border="0" cellspacing="0" cellpadding="0" style="border-bottom: 2px solid #dfdfdf; padding: 20px; background-color: #f7f7f7;">
                                                        <tr>
                                                            <td align="left" valign="top">
                                                                <h1 style="margin: 0px; padding: 0px; text-align: center; font-size: 14px; line-height: 16px; color: #0e2d50; font-weight: 400;">
                                                                    <asp:Label ID="lblNameWdMonth1" runat="server" Text="N/A-CP Name"></asp:Label></h1>
                                                                <h2 style="margin: 0px; padding: 10px 0 20px 0; text-align: center; font-size: 14px; line-height: 16px; color: #ae2a1a; font-weight: 400;">PARTICIPATION DECLINED</h2>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" valign="top" style="padding: 0px 40px; line-height: 0px;">
                                                    <table width="100%" border="0" cellspacing="0" cellpadding="0" style="line-height: 0px">
                                                        <tr>
                                                            <td align="left" valign="top" style="line-height: 0px">
                                                                <p style="margin: 0px; padding: 15px 0 0 0; font-size: 12px; line-height: 16px; color: #494949; font-weight: 400;">
                                                                    Thanks for your response!
                                                                </p>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" valign="top" style="line-height: 0px">
                                                                <p style="margin: 0px; padding: 15px 0 0 0; font-size: 12px; line-height: 16px; color: #494949; font-weight: 400;">
                                                                    You have successfully opted out of the 
                                                                <asp:Label ID="lblNameWdMonthBody1" runat="server" Text="N/A-CP Name"></asp:Label>
                                                                    committed purchase event.If you change your mind before
                                                                <asp:Label ID="lblResponseDeadlineFull1" runat="server" Text="N/A-CP Response Deadline"></asp:Label>,
                                                                you can still participate Simply go to the event's 
                                                                <a href="javascript:void(0);" style="text-decoration: underline; color: #073e6a;" id="aDataEntryPage1" runat="server">Enrollment Details screen</a> in the CBUSA
                                                               Builder Portal.
                                                                </p>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" valign="top" style="line-height: 0px">
                                                                <p style="margin: 0px; padding: 20px 0 0 0; font-size: 12px; line-height: 16px; color: #494949; font-weight: 400;">
                                                                    If you decide to participate,you'll be able to take advantage of special pricing on the materials included in the buy.
                                                                </p>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </form>
</body>
</html>
