<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Take Off"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If TakeOffID = 0 Then %>Add<% Else %>Edit<% End If %> Take Off</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="optional">Title:</td>
		<td class="field"><asp:textbox id="txtTitle" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Builder ID:</td>
		<td class="field"><asp:DropDownList id="drpBuilderID" runat="server" /></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Archived:</td>
		<td class="field"><CC:DatePicker ID="dtArchived" runat="server"></CC:DatePicker></td>
		<td><CC:DateValidator Display="Dynamic" runat="server" id="dtvArchived" ControlToValidate="dtArchived" ErrorMessage="Date Field 'Archived' is invalid" /></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Take Off?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

