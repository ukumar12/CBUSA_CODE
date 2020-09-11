<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Automatic Messages"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If AutomaticMessageID = 0 Then %>Add<% Else %>Edit<% End If %> Automatic Messages</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Subject:</td>
		<td class="field"><asp:textbox id="txtSubject" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvSubject" runat="server" Display="Dynamic" ControlToValidate="txtSubject" ErrorMessage="Field 'Subject' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr id="trTitle" runat="server">
		<td class="required">Title:</td>
		<td class="field"><asp:textbox id="txtTitle" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvTitle" runat="server" Display="Dynamic" ControlToValidate="txtTitle" ErrorMessage="Field 'Title' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr id="trCondition" runat="server">
		<td class="required">Condition:</td>
		<td class="field"><asp:textbox id="txtCondition" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvCondition" runat="server" Display="Dynamic" ControlToValidate="txtCondition" ErrorMessage="Field 'Condition' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Message:</td>
		<td class="field"> </asp:TextBox>
        <CC:CKeditor id="txtMessage" runat="server" Width="600" Height="300" />
        
        </td>
		<td><%--<asp:RequiredFieldValidator ID="rfvMessage" runat="server" Display="Dynamic" ControlToValidate="txtMessage" ErrorMessage="Field 'Message' is blank"></asp:RequiredFieldValidator>--%></td>
	</tr>
	<tr id="trIsEmail" runat="server">
		<td class="required"><b>Is Email?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblIsEmail" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" />
			<asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
	</tr>
	<tr id="trIsMessage" runat="server">
		<td class="required"><b>Is Message?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblIsMessage" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" />
			<asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
	</tr>
	<tr id="trCCList" runat="server">
		<td class="optional">CC List:</td>
		<td class="field"><asp:textbox id="txtCCList" runat="server" maxlength="255" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
	    <td class="optional">Vendor Roles:<br /><span class="smaller">Vendor Messages Only</span></td>
	    <td class="field">
	        <CC:CheckBoxListEx ID="cblVendorRoles" runat="server"></CC:CheckBoxListEx>
	    </td>
	    <td></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Automatic Messages?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
