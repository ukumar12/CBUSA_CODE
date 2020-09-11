<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Contact Us Question"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If QuestionId = 0 Then %>Add<% Else %>Edit<% End If %> Contact Us Question</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Question:</td>
		<td class="field"><asp:textbox id="txtQuestion" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvQuestion" runat="server" Display="Dynamic" ControlToValidate="txtQuestion" ErrorMessage="Field 'Question' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Email Address:</td>
		<td class="field"><asp:textbox id="txtEmailAddress" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><CC:EmailValidator Display="Dynamic" runat="server" id="fvEmailAddress" ControlToValidate="txtEmailAddress" ErrorMessage="Field 'Email Address' is invalid" /></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Contact Us Question?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

