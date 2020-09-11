<%@ Control Language="VB" AutoEventWireup="false" ClassName="Survey.Report.Controls.LongResponse" CodeFile="longresponse.ascx.vb" Inherits="Survey.Report.QuestionType.LongResponse" %>
<table cellpadding="0" cellspacing="0" border="0" style="width:760px;">
    <tr>
        <td><p class="SurveyQuestionText"><asp:Literal ID="ltlQuestionText" runat="server"></asp:Literal></p></td>
    </tr>
    <tr>
        <td><CC:OneClickButton ID="btnViewResponses" runat="server" CssClass="btn" Text="View Responses" /></td>
    </tr>
</table>
