<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="How Heard"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If HowHeardId = 0 Then %>Add<% Else %>Edit<% End If %> Discovery Source Entry</h4>

<script language="javascript" type="text/javascript">

	function ShowInputLabel() {
		
		var tr = document.getElementById('trUserInputLabel');
		var chk = document.getElementById('<%=chkIsUserInput.ClientId %>');
		if (chk.checked) {
			tr.style.display = '';
		} else {
			tr.style.display = 'none';
		}
		
	}

</script>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">How Heard:</td>
		<td class="field"><asp:textbox id="txtHowHeard" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvHowHeard" runat="server" Display="Dynamic" ControlToValidate="txtHowHeard" ErrorMessage="Field 'How Heard' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional"><b>Require User Input?</b></td>
		<td class="field"><asp:CheckBox runat="server" ID="chkIsUserInput" onClick="ShowInputLabel();" Text="Force user to input a value" /></td>
		<td></td>
	</tr>
	<tr id="trUserInputLabel">
		<td class="optional">User Input Label:</td>
		<td class="field"><asp:textbox id="txtUserInputLabel" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
</table>

<script language="javascript" type="text/javascript">ShowInputLabel();</script>

<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Discovery Source Entry?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>