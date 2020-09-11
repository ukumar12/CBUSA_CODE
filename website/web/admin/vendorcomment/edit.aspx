<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Vendor Comment"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If VendorCommentID = 0 Then %>Add<% Else %>Edit<% End If %> Vendor Comment</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Vendor:</td>
		<td class="field"><asp:Literal ID="ltlVendor" runat="server"></asp:Literal></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">Builder:</td>
		<td class="field"><asp:Literal ID="ltlBuilder" runat="server"></asp:Literal></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">Comment:</td>
		<td class="field"><asp:TextBox id="txtComment" style="width: 319px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
		<td><asp:RequiredFieldValidator ID="rfvComment" runat="server" Display="Dynamic" ControlToValidate="txtComment" ErrorMessage="Field 'Comment' is blank"></asp:RequiredFieldValidator></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Vendor Comment?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

