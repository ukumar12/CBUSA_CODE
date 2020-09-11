<%@ Page Language="VB" ValidateRequest="False" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Builder Info"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4>Pricing Information Edit</h4>
<div id="divLabel" runat="server">
</div>
<table border="0" cellspacing="1" cellpadding="2">
	<tr>
		<td class="field"><CC:CKeditor id="txtPricingInformation" runat="server" Width="600" Height="300" /></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
