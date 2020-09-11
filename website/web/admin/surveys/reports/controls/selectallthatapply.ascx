<%@ Control Language="VB" AutoEventWireup="false" ClassName="Survey.Report.Controls.SelectAllThatApply" CodeFile="selectallthatapply.ascx.vb" Inherits="Survey.Report.QuestionType.SelectAllThatApply" %>
<table cellpadding="0" cellspacing="0" border="0" style="width:100%;">
    <tr>
        <td><p class="SurveyQuestionText"><asp:Literal ID="ltlQuestionText" runat="server"></asp:Literal></p></td>
    </tr>
    <tr>
        <td>
        
        <asp:Repeater ID="rptChoices" runat="Server">
        <HeaderTemplate>
        <table cellpadding="0" cellspacing="0" border="0" style="width: 760px;" rules="all">
            <thead>
                <tr style="height:24px;">
                    <th style="text-align:left;">Choice</th>
                    <th style="width:400px;">&nbsp;</th>
                    <th style="width:75px;">Totals</th>
                    <th style="width:100px;">&nbsp;</th>
                </tr>
            </thead>
            <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="row" height="26">
                    <td><%#Container.DataItem("Name")%></td>
                    <td><img src="/images/report-bg.gif" width="400" height="5" id="img" runat="server" /></td>
                    <td style="text-align:center;"><%#Container.DataItem("AnswerCount")%>/<%=TotalResponses%> (<%#Math.Round((Container.DataItem("AnswerCount") / TotalResponses) * 100, 1)%>%)</td>
                    <td style="text-align:center;"><a href="" visible="false" id="lnkView" runat="Server" class="btn">Responses</a></td>
                </tr>
                </ItemTemplate>
                <FooterTemplate>
            </tbody>
        </table></FooterTemplate>
        </asp:Repeater>
        
        </td>
    </tr>
</table>
