<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Registration Status"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If RegistrationStatusID = 0 Then %>Add<% Else %>Edit<% End If %> Registration Status</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Registration Status:</td>
		<td class="field"><asp:textbox id="txtRegistrationStatus" runat="server" maxlength="10" columns="10" style="width: 79px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvRegistrationStatus" runat="server" Display="Dynamic" ControlToValidate="txtRegistrationStatus" ErrorMessage="Field 'Registration Status' is blank"></asp:RequiredFieldValidator></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Registration Status?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
