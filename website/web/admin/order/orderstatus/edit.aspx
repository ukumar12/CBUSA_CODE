<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Order Status"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If OrderStatusID = 0 Then %>Add<% Else %>Edit<% End If %> Order Status</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Order Status:</td>
		<td class="field"><asp:textbox id="txtOrderStatus" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvOrderStatus" runat="server" Display="Dynamic" ControlToValidate="txtOrderStatus" ErrorMessage="Field 'Order Status' is blank"></asp:RequiredFieldValidator></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Order Status?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
