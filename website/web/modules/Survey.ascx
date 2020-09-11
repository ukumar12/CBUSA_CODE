<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Survey.ascx.vb" Inherits="Survey" %>

<div class="pckgltgraywrpr">
	<div class="pckghdgred nobdr">
		Surveys
	</div>
    <div class="stacktblwrpr themeprice">

		
        <asp:Repeater ID="rptSurveys" runat="server">
            <headertemplate>
                <table class="larger" cellpadding="2" cellspacing="0" border="0">
		            <tr>
		                <th>Survey</th>
		                <th>Description</th>
		                <th>End Date</th>
		                <th></th>
		            </tr>
            </headertemplate>
            <ItemTemplate>
                <tr class='<%# iif(Container.ItemIndex mod 2 = 1,"alternate","row") %>'>
                    <td><asp:Literal ID="ltlSurveyName" runat="server"></asp:Literal></td>
                    <td><asp:Literal ID="ltlSurveyDesc" runat="server"></asp:Literal></td>
                    <td><asp:Literal ID="ltlSurveyEndDate" runat="server"></asp:Literal></td>
                    <td nowrap><asp:Literal ID="ltlSurveyStart" runat="server"></asp:Literal></td>
                </tr>
            </ItemTemplate>
            <footertemplate>
            </table>
            </footertemplate>
        </asp:Repeater>
        
        <asp:Literal id="ltlMsg" runat="server"></asp:Literal>
        
    </div>
</div>
<br />
