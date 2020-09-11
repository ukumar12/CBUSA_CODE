<%@ Page Language="VB" ValidateRequest="False" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Custom Text"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><asp:Literal ID="ltlTitle" runat="server"></asp:Literal></h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan=2><span class=smallest><span class=red>red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="optional">HTML/Text:</td>
		<td class="field"><CC:CKEditor id="txtValue" runat="server" Width="600" Height="300" /></td>
		<td></td>
	</tr>
</table>

<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

