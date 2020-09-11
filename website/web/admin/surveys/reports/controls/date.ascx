<%@ Control Language="VB" AutoEventWireup="false" ClassName="Survey.Report.Controls.Date" CodeFile="date.ascx.vb" Inherits="Survey.Report.QuestionType.Date" %>
<table cellpadding="0" cellspacing="0" border="0" style="width:100%;">
    <tr>
        <td><p class="SurveyQuestionText"><asp:Literal ID="ltlQuestionText" runat="server"></asp:Literal></p></td>
    </tr>
    <tr>
        <td><CC:OneClickButton ID="btnViewResponses" runat="server" CssClass="btn" Text="View Dates" /></td>
    </tr>
</table>
