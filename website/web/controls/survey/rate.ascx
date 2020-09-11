<%@ Control Language="VB" AutoEventWireup="false" CodeFile="rate.ascx.vb" ClassName="Survey.QuestionType.Rate" Inherits="Survey.Controls.Rate" %>

<table cellpadding="0" cellspacing="0" border="0" style="width: 100%">
    <tr>
        <td id="tdRequire" runat="server" class="fieldreq">&nbsp;</td>
        <td><p class="SurveyQuestionText"><asp:Literal ID="ltlQuestionText" runat="server"></asp:Literal></p></td>
    </tr>
    <tr>
        <td style="width:10px;">&nbsp;</td>
        <td>
            <p>
                <asp:Repeater ID="rptChoices" runat="server">
                <HeaderTemplate>
                    <table cellpadding="0" cellspacing="0" border="0" width="100%" style="width:100%;">
                    <tr>
                </HeaderTemplate>
                <ItemTemplate>
                        <td  style="border-top:1px solid #eeeeee;border-bottom:1px solid #eeeeee; " id="tdContainer" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                                <tr>
                                    
                                    <td class="ChoiceTextSmall" style="font-size:x-small; text-align: center; width: 100%; height:30px;">
                                        <asp:Literal ID="ltlChoiceText" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="font-size:x-small; text-align: center;  height:30px;">
                                        <input type="radio" id="rbtnChoice" name="rbtnChoice" runat="server" />                            
                                    </td>
                                 </tr>
                            </table>
                        </td>
                </ItemTemplate>
                <FooterTemplate>
                    </tr>
                    </table>
                </FooterTemplate>
                </asp:Repeater>

            </p>
        </td>
    </tr>
</table>