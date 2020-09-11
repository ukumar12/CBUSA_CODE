<%@ Control Language="VB" AutoEventWireup="false" ClassName="Survey.Report.Controls.Quantity" CodeFile="quantity.ascx.vb" Inherits="Survey.Report.QuestionType.Quantity" %>
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
                    <th style="width:75px;">Average<br />Quantity</th>
                    <th style="width:75px;">Choice<br />Total</th>
                    <th style="width:75px;">Respondent<br />Count</th>
                    <th style="width:100px;">&nbsp;</th>
                </tr>
            </thead>
            <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="row" height="26">
                    <td><%#Container.DataItem("Name")%></td>
                    <td style="text-align:center;"><asp:label ID="lblTotals" runat="server"></asp:label></td>
                    <td style="text-align:center;"><%#Container.DataItem("ChoiceSum")%></td>
                    <td style="text-align:center;"><%=TotalResponses%></td>
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
