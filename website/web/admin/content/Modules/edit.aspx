<%@ Page ValidateRequest="False" Language="VB" MasterPageFile="~/controls/AdminMaster.master" AutoEventWireup="false" CodeFile="edit.aspx.vb" Inherits="edit" title="Add/Edit Content Module" %>

<asp:Content ID="Content" ContentPlaceHolderID="ph" Runat="Server">

<h4>Content Tool - Add / Edit Module</h4>

<table cellSpacing="2" cellPadding="3" border="0">
<tr>
	<td colSpan="2"><span class="red">red color</span> - required fields</td>
</tr>
<tr>
	<td class="required"><b>Module Name:</b></td>
	<td class="field"><asp:textbox id="Name" runat="server" maxlength="100" Width="339px"></asp:textbox></td>
	<td><asp:requiredfieldvalidator id="RequiredFieldValidator1" runat="server" ErrorMessage="Name is blank" ControlToValidate="Name" Display="Dynamic"></asp:requiredfieldvalidator></td>
</tr>
<tr>
	<td class="required"><b>Control URL:</b></td>
	<td class="field"><asp:textbox id="ControlURL" runat="server" maxlength="255" Width="339px"></asp:textbox></td>
	<td><asp:requiredfieldvalidator id="RequiredFieldValidator2" runat="server" ErrorMessage="Control URL is blank" ControlToValidate="ControlURL" Display="Dynamic"></asp:requiredfieldvalidator></td>
</tr>
<tr>
	<td class="optional"><b>Args:</b></td>
	<td class="field"><asp:textbox id="Args" runat="server" maxlength="50" columns="50" Width="339px"></asp:textbox></td>
	<td></td>
</tr>
<tr>
	<td class="required"><b>Min. Width:</b></td>
	<td class="field"><asp:textbox id="MinWidth" runat="server" maxlength="10" Width="50px"></asp:textbox></td>
	<td><asp:requiredfieldvalidator id="RequiredFieldValidator3" runat="server" ErrorMessage="Min. Width is blank" ControlToValidate="MinWidth" Display="Dynamic"></asp:requiredfieldvalidator></td>
</tr>
<tr>
	<td class="required"><b>Max. Width:</b></td>
	<td class="field"><asp:textbox id="MaxWidth" runat="server" maxlength="10" Width="50px"></asp:textbox></td>
	<td><asp:requiredfieldvalidator id="RequiredFieldValidator4" runat="server" ErrorMessage="Max. Width is blank" ControlToValidate="MaxWidth" Display="Dynamic"></asp:requiredfieldvalidator></td>
</tr>
<tr id="trSkipIndexing" runat="server">
	<td class="optional"><b>Skip idev&reg; indexing?</b></td>
	<td class="field"><asp:CheckBox runat="server" ID="chkSkipIndexing" /></td>
	<td></td>
</tr>
<tr>
	<td class="optional" valign=top><b>Module HTML:</b></td>
	<td class="field"><asp:textbox id="HTML" runat="server" Width="339px" Rows="6" TextMode="MultiLine" Wrap=true></asp:textbox></td>
	<td></td>
</tr>
</table>

<p>
	<CC:OneClickButton id="Save" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
	<CC:ConfirmButton id="Delete" runat="server" Message="Are you sure want to delete this Group?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
	<CC:OneClickButton id="Cancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>
</p>

</asp:Content>

