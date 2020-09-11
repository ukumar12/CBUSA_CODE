<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Order Status History"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If OrderStatusHistoryID = 0 Then %>Add<% Else %>Edit<% End If %> Order Status History</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Order:</td>
		<td class="field"><asp:literal id="ltlOrderID" runat="server"></asp:literal></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">Order Status:</td>
		<td class="field"><asp:Literal ID="ltlStatus" runat="server"></asp:Literal></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">Creator Builder Account:</td>
		<td class="field"><asp:Literal ID="ltlAccount" runat="server"></asp:Literal></td>
		<td></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Return" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Order Status History?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>

</asp:content>

