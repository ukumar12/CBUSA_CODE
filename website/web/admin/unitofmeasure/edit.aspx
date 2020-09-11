<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Unit Of Measure"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If UnitOfMeasureID = 0 Then %>Add<% Else %>Edit<% End If %> Unit Of Measure</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Unit Of Measure:</td>
		<td class="field"><asp:textbox id="txtUnitOfMeasure" runat="server" maxlength="20" columns="20" style="width: 139px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvUnitOfMeasure" runat="server" Display="Dynamic" ControlToValidate="txtUnitOfMeasure" ErrorMessage="Field 'Unit Of Measure' is blank"></asp:RequiredFieldValidator></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Unit Of Measure?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
