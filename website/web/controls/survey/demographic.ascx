<%@ Control Language="VB" AutoEventWireup="false" CodeFile="demographic.ascx.vb" ClassName="Survey.QuestionType.Demographic" Inherits="Survey.Controls.Demographic" %>
<table cellpadding="0" cellspacing="0" border="0" style="width: 100%">
    <tr>
        <td id="tdRequire" runat="server" class="fieldreq">&nbsp;</td>
        <td><p class="SurveyQuestionText"><asp:Literal ID="ltlQuestionText" runat="server"></asp:Literal></p></td>
    </tr>
    <tr>
        <td style="width:10px;">&nbsp;</td>
        <td>
            <p>

                <asp:Repeater ID="rptDemographics" runat="server">

                    <HeaderTemplate>
                        <table cellpadding="0" cellspacing="0" border="0" style="width:400px;">
                           <tbody>
                    </HeaderTemplate>
                    
                        <ItemTemplate>
                            <tr style="height:26px;">
                                <td class="DemographicField" id="tdText" runat="server" style="text-align:right; font-weight:700;"></td>
                                <td style="width:8px;">&nbsp;</td>
                                <td id="tdRequireDemographic" runat="server" class="fieldreq">&nbsp;</td>
                                <td id="tdField" runat="Server"></td>
                            </tr>        
                        </ItemTemplate>    
                    
                    <FooterTemplate>
                    </tbody>
                        </table>
                    </FooterTemplate>

                </asp:Repeater>

            </p>
        </td>
    </tr>
</table>