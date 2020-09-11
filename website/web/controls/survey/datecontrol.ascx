<%@ Control Language="VB" AutoEventWireup="false" CodeFile="datecontrol.ascx.vb" ClassName="Survey.QuestionType.Date"  Inherits="Survey.Controls.Date" %>
<table cellpadding="0" cellspacing="0" border="0" style="width: 100%">
    <tr>
        <td id="tdRequire" runat="server" class="fieldreq">&nbsp;</td>
        <td><p class="SurveyQuestionText"><asp:Literal ID="ltlQuestionText" runat="server"></asp:Literal></p></td>
    </tr>
    <tr>
        <td style="width:10px;">&nbsp;</td>
        <td><p><CC:DatePicker ID="dpDate" runat="server" CssClass="ibox"></CC:DatePicker></p></td>
    </tr>
</table>
