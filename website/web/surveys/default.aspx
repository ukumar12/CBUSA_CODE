<%@ Page Language="VB" AutoEventWireup="false" CodeFile="default.aspx.vb" Inherits="Surveys_default" %>
<%@ Reference Control="~\controls\survey\longresponse.ascx" %>
<%@ Reference Control="~\controls\survey\shortresponse.ascx" %>
<%@ Reference Control="~\controls\survey\rate.ascx" %>
<%@ Reference Control="~\controls\survey\selectone.ascx" %>
<%@ Reference Control="~\controls\survey\selectallthatapply.ascx" %>
<%@ Reference Control="~\controls\survey\standardrank.ascx" %>
<%@ Reference Control="~\controls\survey\percentagerank.ascx" %>
<%@ Reference Control="~\controls\survey\quantity.ascx" %>
<%@ Reference Control="~\controls\survey\datecontrol.ascx" %>
<%@ Reference Control="~\controls\survey\demographic.ascx" %>


<CT:masterpage runat="server" ID="CTMain">
<table cellpadding="0" cellspacing="0" border="0" style="width:100%; ">
    <tr>
        <td style="padding:20px; margin:0px;padding-bottom:0px;">
        <h2><asp:Literal id="ltlSurveyName" runat="server"></asp:Literal></h2>
        <p><asp:Literal id="ltlSurveyDesc" runat="server"></asp:Literal></p>
        <div >
        
                <table style="width: 100%;" border="0" cellpadding="0" cellspacing="0">
                    <tbody>
                        <tr valign="top">
                            <td style="padding: 8px; font-size: 16px;">
                                <!-- product name -->
                                <table border="0" cellpadding="0" cellspacing="0">
                                    <tbody>
                                        <tr valign="top">
                                            <td style="padding-right: 8px;">
                                            </td>
                                            <td class="bold" style="line-height: 20px;">
                                                <asp:label id="lblPageName" runat="Server"></asp:label>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                                <!-- end product name -->
                            </td>
                            <td class="smallest right txtc3" style="padding-right: 6px;">
                                <!-- pager -->
                                <table style="height: 36px; margin-left: auto;" border="0" cellpadding="0" cellspacing="0">
                                    <tbody>
                                        <tr>
                                            <td class="txtc2" style="padding-left: 4px;">
                                                <asp:Button id="lnkBtnPrevious" runat="Server" Text="Previous" CssClass="btnred" style="width:120px; Height: 40px;"/><br />
                                            </td>
                                            <td class="txtc2" style="padding-left: 4px;" id="sep2" runat="server">
                                            </td>
                                            <td class="txtc2" style="padding-left: 4px;">
                                                    <asp:button id="lnkBtnNext" runat="Server" Text="Next" CssClass="btnred" style="width:120px; Height: 40px;" /><br />
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                                <!-- end pager -->
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        
         </td>
    </tr>
    <tr>
        <td style="padding:36px;">
            <asp:placeholder id="plcQuestions" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="smallest right txtc3" style="padding-right: 30px;padding-bottom:20px;">
            <!-- pager -->
            <table style="height: 36px; margin-left: auto;" border="0" cellpadding="0" cellspacing="0">
                <tbody>
                                        <tr>
                                            <td class="txtc2" style="padding-left: 4px;">
                                                <asp:Button id="lnkBtnPreviousBottom" runat="Server" Text="Previous" CssClass="btnred" style="width:120px; Height: 40px;"/><br />
                                            </td>
                                            <td class="txtc2" style="padding-left: 4px;" id="sep" runat="server">
                                            </td>
                                            <td class="txtc2" style="padding-left: 4px;">
                                                    <asp:button id="lnkBtnNextBottom" runat="Server" Text="Next" CssClass="btnred" style="width:120px; Height: 40px;" /><br />
                                            </td>
                                        </tr>
                </tbody>
            </table>
            <!-- end pager -->
        </td>
    </tr>
</table>
</CT:masterpage>