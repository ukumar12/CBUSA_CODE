<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Payment Status"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If PaymentStatusID = 0 Then %>Add<% Else %>Edit<% End If %> Payment Status</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Payment Status:</td>
		<td class="field"><asp:textbox id="txtPaymentStatus" runat="server" maxlength="20" columns="20" style="width: 139px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvPaymentStatus" runat="server" Display="Dynamic" ControlToValidate="txtPaymentStatus" ErrorMessage="Field 'Payment Status' is blank"></asp:RequiredFieldValidator></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Payment Status?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

