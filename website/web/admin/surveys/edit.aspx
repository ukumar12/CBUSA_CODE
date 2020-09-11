<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Admin_Survey_Edit"  Title="Survey"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If SurveyId = 0 Then %>Add<% Else %>Edit<% End If %> Survey</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan=2><span class=smallest><span class=red>red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Name:</td>
		<td class="field"><asp:textbox id="txtName" runat="server" maxlength="50" columns="50" style="width: 125px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvName" runat="server" Display="Dynamic" ControlToValidate="txtName" ErrorMessage="Field 'Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Display Title:</td>
		<td class="field"><asp:textbox id="txtDisplayTitle" runat="server" maxlength="255" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvDisplayTitle" runat="server" Display="Dynamic" ControlToValidate="txtDisplayTitle" ErrorMessage="Field 'Display Title' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required"><b>Is Active?</b></td>
		<%--<td class="field"><asp:CheckBox runat="server" ID="chkIsActive" onClick="this.checked ? document.getElementById('trOrderFollowUp').style.display='block' : document.getElementById('trOrderFollowUp').style.display='none'" /></td>--%>
		<td class="field"><asp:CheckBox runat="server" ID="chkIsActive" /></td>
	</tr>
	<tr>
		<td class="required"><b>User Permissions</b></td>
		<td class="field">
		    <asp:CheckBox runat="server" ID="chkIsBuilder" Text="Builder" />
		    <asp:CheckBox runat="server" ID="chkIsVendor" Text="Vendor" />
		    <asp:CheckBox runat="server" ID="chkIsPIQ" Text="PIQ" />
		</td>
	</tr>
	<tr id="trOrderFollowUp" style="display:none;" runat="server" visible="false">
		<td class="required"><b>Is Order Follow-Up?</b></td>
		<td class="field"><asp:CheckBox runat="server" ID="chkIsFollowUp" /><br /><span class="red">Only one survey can be marked as "Order Follow-Up"<br />This option requires users to place an order before completing<br />Customers may only complete the survey once per order</span></td>
	</tr>
	<tr>
		<td class="optional">Description:</td>
		<td class="field"><asp:TextBox id="txtDescription" style="width: 319px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Start Date:</td>
		<td class="field"><CC:DatePicker ID="dtStartDate" runat="server"></CC:DatePicker></td>
		<td><CC:DateValidator Display="Dynamic" runat="server" id="dtvStartDate" ControlToValidate="dtStartDate" ErrorMessage="Date Field 'Start Date' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">End Date:</td>
		<td class="field"><CC:DatePicker ID="dtEndDate" runat="server"></CC:DatePicker></td>
		<td><CC:DateValidator Display="Dynamic" runat="server" id="dtvEndDate" ControlToValidate="dtEndDate" ErrorMessage="Date Field 'End Date' is invalid" /></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Survey?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

<%--<script language="javascript">
<!--
document.getElementById('<%=chkIsActive.ClientID%>').checked ? document.getElementById('trOrderFollowUp').style.display='block' : document.getElementById('trOrderFollowUp').style.display='none';
//-->
</script>--%>

</asp:content>
