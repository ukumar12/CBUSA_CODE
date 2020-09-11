<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Special Order Product"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If SpecialOrderProductID = 0 Then %>Add<% Else %>Edit<% End If %> Special Order Product</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Builder I D:</td>
		<td class="field"><asp:DropDownList id="drpBuilderID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvBuilderID" runat="server" Display="Dynamic" ControlToValidate="drpBuilderID" ErrorMessage="Field 'Builder I D' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Product Name:</td>
		<td class="field"><asp:textbox id="txtSpecialOrderProduct" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvSpecialOrderProduct" runat="server" Display="Dynamic" ControlToValidate="txtSpecialOrderProduct" ErrorMessage="Field 'Product Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Description:</td>
		<td class="field"><asp:TextBox id="txtDescription" style="width: 319px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
		<td><asp:RequiredFieldValidator ID="rfvDescription" runat="server" Display="Dynamic" ControlToValidate="txtDescription" ErrorMessage="Field 'Description' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Unit Of Measure ID:</td>
		<td class="field"><asp:DropDownList id="drpUnitOfMeasureID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvUnitOfMeasureID" runat="server" Display="Dynamic" ControlToValidate="drpUnitOfMeasureID" ErrorMessage="Field 'Unit Of Measure ID' is blank"></asp:RequiredFieldValidator></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Special Order Product?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
