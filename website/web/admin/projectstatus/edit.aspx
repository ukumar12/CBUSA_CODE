<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Project Status"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If ProjectStatusID = 0 Then %>Add<% Else %>Edit<% End If %> Project Status</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Project Status:</td>
		<td class="field"><asp:textbox id="txtProjectStatus" runat="server" maxlength="30" columns="30" style="width: 199px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvProjectStatus" runat="server" Display="Dynamic" ControlToValidate="txtProjectStatus" ErrorMessage="Field 'Project Status' is blank"></asp:RequiredFieldValidator></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Project Status?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
