<%@ Control Language="VB" AutoEventWireup="false" CodeFile="shortresponse.ascx.vb" ClassName="Survey.QuestionType.ShortResponse"  Inherits="Survey.Controls.ShortResponse" %>
<table cellpadding="0" cellspacing="0" border="0" style="width:100%;">
    <tr>
        <td id="tdRequire" runat="server" class="fieldreq">&nbsp;</td>
        <td><p class="SurveyQuestionText"><asp:Literal ID="ltlQuestionText" runat="server"></asp:Literal></p></td>
    </tr>
    <tr>
        <td style="width:10px;">&nbsp;</td>
        <td><p><asp:TextBox ID="txtResponse" ValidationGroup="Survey" runat="server" CssClass="SurveyInputField"></asp:TextBox></p></td>
    </tr>
</table>
