<%@ Page Language="VB" ValidateRequest="False" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="admin_survey_choice_Edit"  Title="Survey Question Choice"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If ChoiceId = 0 Then %>Add<% Else %>Edit<% End If %> Survey Question Choice</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan=2><span class=smallest><span class=red>red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Name:</td>
		<td class="field"><asp:textbox id="txtName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvName" runat="server" Display="Dynamic" ControlToValidate="txtName" ErrorMessage="Field 'Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Display Text:</td>
		<td class="field"><CC:CKeditor id="txtDisplayText" runat="server" Width="600" Height="300" /></td>
		<td><CC:RequiredCKValidator ID="rfvDisplayText" runat="server" Display="Dynamic" ControlToValidate="txtDisplayText" ErrorMessage="Field 'Display Text' is blank"/></td>
	</tr>
	<tr>
		<td class="optional">Show Response Field:</td>
		<td class="field"><asp:checkbox id="chkShowResponseField" runat="server" /></td>
		<td></td>
	</tr>
	<!--
	<tr>
		<td class="optional">Skip To Page Id:</td>
		<td class="field"><asp:DropDownList id="drpSkipToPageId" runat="server" /></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Child Question Id:</td>
		<td class="field"><asp:DropDownList id="drpChildQuestionId" runat="server" /></td>
		<td></td>
	</tr>-->
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Survey Question Choice?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
