<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Item Template"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If TemplateId = 0 Then %>Add<% Else %>Edit<% End If %> Item Template</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan=2><span class=smallest><span class=red>red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Template Name:</td>
		<td class="field"><asp:textbox id="txtTemplateName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvTemplateName" runat="server" Display="Dynamic" ControlToValidate="txtTemplateName" ErrorMessage="Field 'Template Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required"><b>Is Attributes?</b></td>
		<td class="field"><asp:CheckBox runat="server" ID="chkIsAttributes" /></td>
	</tr>
	<tr>
		<td class="required"><b>Display Mode:</b></td>
		<td class="field"><asp:DropDownList id="drpDisplayMode" runat="server"><asp:ListItem Text="Admin Driven" Value="AdminDriven" /><asp:ListItem Text="Table Layout" Value="TableLayout" /></asp:DropDownList></td>
		<td></td>
	</tr>
	<tr runat="server" visible="false">
		<td class="required"><b>Is To And From?</b></td>
		<td class="field"><asp:CheckBox runat="server" ID="chkIsToAndFrom" /></td>
	</tr>
	<tr runat="server" visible="false">
		<td class="required"><b>Is Gift Message?</b></td>
		<td class="field"><asp:CheckBox runat="server" ID="chkIsGiftMessage" /></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Item Template?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
