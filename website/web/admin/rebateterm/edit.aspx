<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Rebate Term"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If RebateTermsID = 0 Then %>Add<% Else %>Edit<% End If %> Rebate Term</h4>

<asp:Literal id="ltlMsg" runat="server"></asp:Literal>
<table cellpadding="4" cellspacing="1" border="0" class="tblcompr largest">
    <tr class="subttl">
        <th>Quarter</th>
        <th>Rebate Percentage</th>
        <th>New Rebate Percentage</th>
        <th>&nbsp;</th>
        <th>Update History</th>
    </tr>
    <tr>
        <td><asp:Literal id="ltlCurrentQuarter" runat="server" /></td>
        <td><asp:Literal id="ltlCurrentRebate" runat="server" />%</td>
        <td><asp:TextBox id="txtNewRebate" runat="server" columns="1" maxlength="4" />%</td>
        <td><asp:Button id="btnUpdateRebate" runat="server" text="Update" cssclass="btn" /></td>
        <td><asp:Literal id="ltlLogMsg" runat="server" /></td>
    </tr>
</table>

<p></p>
<CC:OneClickButton id="btnCancel" runat="server" Text="Back" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
