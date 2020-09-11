<%@ Control Language="VB" AutoEventWireup="false" CodeFile="percentagerank.ascx.vb" ClassName="Survey.QuestionType.PercentageRank" Inherits="Survey.Controls.PercentageRank" %>

<table cellpadding="0" cellspacing="0" border="0" style="width: 100%">
    <tr>
        <td id="tdRequire" runat="server" class="fieldreq">&nbsp;</td>
        <td><p class="SurveyQuestionText"><asp:Literal ID="ltlQuestionText" runat="server"></asp:Literal></p></td>
    </tr>
    <tr>
        <td style="width:10px;">&nbsp;</td>
        <td>
            <p>
            <asp:Repeater ID="rptChoices" runat="server" EnableViewState=true>
            <HeaderTemplate>
                <table cellpadding="0" cellspacing="0" border="0" width="100%" style="width:100%;">
            </HeaderTemplate>
            <ItemTemplate>
                    <tr>        
                        <td style="width:20px;height:26px;">
                            <asp:textbox ID="txtChoice" runat="Server" CssClass="ibox" Width="25" />
                        </td>
                        <td class="ChoiceText" style="padding-left:5px;"><asp:Literal ID="ltlChoiceText" runat="server"></asp:Literal></td>
                    <tr>
                    <tr id="trChoiceResponse" runat="Server">
                        <td>&nbsp;</td>
                        <td style="padding-left:5px;"><asp:TextBox ID="txtChoiceResponse" runat="Server" CssClass="ibox"></asp:TextBox></td>
                    </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
            </asp:Repeater>
            </p>
        </td>
    </tr>
</table>