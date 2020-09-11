<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Excluded Search Words"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If ExcludeSearchWordId = 0 Then %>Add<% Else %>Edit<% End If %> Excluded Search Words</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Exclude Search Word:</td>
		<td class="field"><asp:textbox id="txtExcludeSearchWord" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvExcludeSearchWord" runat="server" Display="Dynamic" ControlToValidate="txtExcludeSearchWord" ErrorMessage="Field 'Exclude Search Word' is blank"></asp:RequiredFieldValidator></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Excluded Search Words?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
