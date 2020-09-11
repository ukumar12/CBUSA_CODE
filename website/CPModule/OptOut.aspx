<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OptOut.aspx.cs" Inherits="OptOut" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="Content/images/favicon.ico" />
    <title>CP-OptOut</title>
    <!--[if lt IE 9]>
      <script src="https://oss.maxcdn.com/html5shiv/3.7.3/html5shiv.min.js"></script>
      <script src="https://oss.maxcdn.com/respond/1.4.2/respond.min.js"></script>
    <![endif]-->
    <style type="text/css">
        .overlay {
            position: fixed;
            top: 0;
            bottom: 0;
            left: 0;
            right: 0;
            background: rgba(0, 0, 0, 0.7);
            transition: opacity 500ms;
            visibility: hidden;
            opacity: 0;
        }

            .overlay:target {
                visibility: visible;
                opacity: 1;
            }

        .popup {
            margin: 70px auto;
            padding: 20px;
            background: #fff;
            border-radius: 5px;
            width: 30%;
            position: relative;
            transition: all 2s ease-in-out;
        }

            .popup .close {
                position: absolute;
                top: 20px;
                right: 30px;
                transition: all 200ms;
                font-size: 30px;
                font-weight: bold;
                text-decoration: none;
                color: #333;
            }

            .popup .content {
                max-height: 30%;
                overflow: auto;
            }
    </style>

</head>
<body>
    <form id="form1" runat="server">
        <div class="table-middle" style="position: absolute; top: 50%; left: 50%; margin-left: -327px; margin-top: -165px;">
            <div id="mailErrorContent" runat="server" style="background-color:#0770ad; padding:10px 6px; font-family: Arial,Helvetica,Verdana,sans-serif; font-size:15px; line-height:12px;
             color:#FFF; text-align:center; border-radius:6px; width:655px;" visible="false" >
                <asp:Label ID="Label1" runat="server" Text="Sorry! The response deadline for this event has crossed."></asp:Label>
            </div>
            <div id="mailSuccessContent" runat="server" visible="true">
                <table align="center" width="655" border="0" cellspacing="0" cellpadding="0" style="font-family: Arial,Helvetica,Verdana,sans-serif; font-size: 12px;">
                    <tr>
                        <td align="center" valign="middle">
                            <table width="100%" height="330" border="0" cellspacing="0" cellpadding="0" style="border: 2px solid #2f567a; background-color: rgba(255,255,255,0.5); border-radius: 10px;">
                                <tr>
                                    <td align="left" valign="top" style="padding: 6px;">
                                        <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                            <tr>
                                                <td align="left" valign="top">
                                                    <table width="100%" border="0" cellspacing="0" cellpadding="0" style="border-bottom: 2px solid #dfdfdf; padding: 20px; background-color: #f7f7f7;">
                                                        <tr>
                                                            <td align="left" valign="top" width="223">
                                                                <img src="https://app.custombuilders-usa.com//images/global/hdr-logo.gif" style="width: 223px; height: 70px; border-style: none;" alt="CBUSA"></td>
                                                            <td align="left" valign="top">
                                                                <h1 style="margin: 0px; padding: 0px; text-align: center; font-size: 16px; line-height: 16px; color: #0e2d50; font-weight: 400;">
                                                                    <asp:Label ID="lblNameWdMonth" runat="server" Text="N/A-CP Name"></asp:Label></h1>
                                                                <h1 style="margin: 0px; padding: 10px 0 0 0; text-align: center; font-size: 14px; line-height: 16px; color: #0e2d50; font-weight: 400;">
                                                                    <asp:Label ID="lbllblBuilderName" runat="server" Text="N/A-BuilderName1 Name"></asp:Label></h1>
                                                                <h2 style="margin: 0px; padding: 10px 0 0 0; text-align: center; font-size: 16px; line-height: 16px; color: #ae2a1a; font-weight: 400;">PARTICIPATION DECLINED</h2>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" valign="top" style="padding: 0px 40px;">
                                                    <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                                        <tr>
                                                            <td align="left" valign="top">
                                                                <p style="margin: 0px; padding: 20px 0 0 0; font-size: 12px; line-height: 16px; color: #494949; font-weight: 400;">
                                                                    Thanks for your response!
                                                                </p>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" valign="top">
                                                                <p style="margin: 0px; padding: 20px 0 0 0; font-size: 12px; line-height: 16px; color: #494949; font-weight: 400;">
                                                                    You have successfully opted out of the
                                                                    <asp:Label ID="lblNameWdMonthBody" runat="server" Text="N/A-CP Name"></asp:Label>
                                                                    committed purchase event. If you change your mind before
                                                                    <asp:Label ID="lblResponseDeadlineFull" runat="server" Text="N/A-CP Response Deadline"></asp:Label>, you can still participate. Simply go to the event's <a href="javascript:void(0);" style="text-decoration: underline; color: #073e6a;" id="aDataEntryPage" runat="server">Enrollment Details screen</a> in the CBUSA Builder Portal.
                                                                </p>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" valign="top">
                                                                <p style="margin: 0px; padding: 20px 0 0 0; font-size: 12px; line-height: 16px; color: #494949; font-weight: 400;">
                                                                    If you decide to participate, you'll be able to take advantage of special pricing on the <a href="#popup1" style="text-decoration: underline; color: #073e6a;" id="aMaterialsDetails" runat="server">materials included in the buy.</a>
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
                <div id="popup1" class="overlay">
                    <div class="popup">
                        <h2>Event Description</h2>
                        <a class="close" href="#">&times;</a>
                        <div class="content" id="lblEventDescription" runat="server">
                            <%--<asp:Label ID="lblEventDescription" runat="server" Text="N/A-EventDescription"></asp:Label>--%>
                        </div>
                    </div>
                </div>
            </div>

            <div id="mailContent" style="display: none;">
                <table align="center" width="600" border="0" cellspacing="0" cellpadding="0" style="line-height: 0px; font-family: Arial,Helvetica,Verdana,sans-serif;" id="dvEventDetails" runat="server"> <tr style="page-break-before: always"> <td align="center" height="300" valign="middle" style="line-height: 0px;"> <table width="100%" cellspacing="0" cellpadding="0" style="border: 2px solid #2f567a; background-color: rgba(255,255,255,0.5); border-radius: 10px; border-collapse: separate; line-height: 0px;"> <tr style="page-break-before: always"> <td align="left" valign="top" style="padding: 6px;" style="line-height: 0px"> <table width="100%" border="0" cellspacing="0" cellpadding="0" style="line-height: 0px"> <tr style="page-break-before: always"> <td align="left" valign="top" style="line-height: 0px; margin:0px; padding:0px;"> <table width="100%" border="0" cellspacing="0" cellpadding="0" style="border-bottom: 2px solid #dfdfdf; padding: 20px; background-color: #f7f7f7;"> <tr style="page-break-before: always"> <td align="left" valign="top" style="margin: 0px; padding:10px 0px 0px 0px; text-align: center; font-size: 14px; line-height: 16px; color: #0e2d50; font-weight: 400;"><asp:Label ID="lblNameWdMonth1" runat="server" Text="N/A-CP Name"></asp:Label></td> </tr><tr style="page-break-before: always"> <td align="left" valign="top" style="margin: 0px; padding:5px 0 5px 0px; text-align: center; font-size: 14px; line-height: 16px; color: #0e2d50; font-weight: 400;"><asp:Label ID="lblBuilderName1" runat="server" Text="N/A-BuilderName1 Name"></asp:Label></td> </tr><tr style="page-break-before: always"> <td align="left" valign="top" style="margin: 0px; padding:0px 0px 10px 0px; text-align: center; font-size: 14px; line-height: 16px; color: #ae2a1a; font-weight: 400;">PARTICIPATION DECLINED</td> </tr></table> </td> </tr> <tr style="page-break-before: always"> <td align="left" valign="top" style="padding: 0px 40px; line-height: 0px;"> <table width="100%" border="0" cellspacing="0" cellpadding="0" style="line-height: 0px"> <tr style="page-break-before: always"> <td align="left" valign="top" style="line-height: 0px; padding-top:15px;"> <p style="margin: 0px; padding:0; font-size: 12px; line-height: 16px; color: #494949; font-weight: 400;"> Thanks for your response! </p> </td> </tr> <tr> <td align="left" valign="top" style="line-height: 0px; padding-top:15px;padding-bottom:15px;"> <p style="margin: 0px; padding:0; font-size: 12px; line-height: 16px; color: #494949; font-weight: 400;"> You have successfully opted out of the <asp:Label ID="lblNameWdMonthBody1" runat="server" Text="N/A-CP Name" style="font-weight:bold;"></asp:Label> committed purchase event. If you change your mind before <asp:Label ID="lblResponseDeadlineFull1" runat="server" Text="N/A-CP Response Deadline"></asp:Label>, you can still participate. Simply go to the event's <a href="javascript:void(0);" style="text-decoration: underline; color: #073e6a;" id="aDataEntryPage1" runat="server">Enrollment Details screen</a> in the CBUSA Builder Portal. </p> </td> </tr> <tr> <td align="left" valign="top" style="line-height: 0px; padding-bottom:20px;"> <p style="margin: 0px; padding: 10px 0 0 0; font-size: 12px; line-height: 16px; color: #494949; font-weight: 400;"> If you decide to participate, you'll be able to take advantage of special pricing on the materials included in the buy. </p> </td> </tr> </table> </td> </tr> </table> </td> </tr> </table> </td> </tr> </table>
            </div>
        </div>


    </form>
</body>
</html>
