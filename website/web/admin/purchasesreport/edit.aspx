<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Purchases Report"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If PurchasesReportID = 0 Then %>Add<% Else %>Edit<% End If %> Purchases Report</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Builder ID:</td>
		<td class="field"><asp:DropDownList id="drpBuilderID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvBuilderID" runat="server" Display="Dynamic" ControlToValidate="drpBuilderID" ErrorMessage="Field 'Builder ID' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Period Year:</td>
		<td class="field"><asp:textbox id="txtPeriodYear" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvPeriodYear" runat="server" Display="Dynamic" ControlToValidate="txtPeriodYear" ErrorMessage="Field 'Period Year' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvPeriodYear" ControlToValidate="txtPeriodYear" ErrorMessage="Field 'Period Year' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Period Quarter:</td>
		<td class="field"><asp:textbox id="txtPeriodQuarter" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvPeriodQuarter" runat="server" Display="Dynamic" ControlToValidate="txtPeriodQuarter" ErrorMessage="Field 'Period Quarter' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvPeriodQuarter" ControlToValidate="txtPeriodQuarter" ErrorMessage="Field 'Period Quarter' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Creator Builder Account I D:</td>
		<td class="field"><asp:textbox id="txtCreatorBuilderAccountID" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvCreatorBuilderAccountID" runat="server" Display="Dynamic" ControlToValidate="txtCreatorBuilderAccountID" ErrorMessage="Field 'Creator Builder Account I D' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvCreatorBuilderAccountID" ControlToValidate="txtCreatorBuilderAccountID" ErrorMessage="Field 'Creator Builder Account I D' is invalid" /></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Purchases Report?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
