<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Vendor Product Price Request"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If VendorProductPriceRequestID = 0 Then %>Add<% Else %>Edit<% End If %> Vendor Product Price Request</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Vendor I D:</td>
		<td class="field"><asp:DropDownList id="drpVendorID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvVendorID" runat="server" Display="Dynamic" ControlToValidate="drpVendorID" ErrorMessage="Field 'Vendor I D' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Builder I D:</td>
		<td class="field"><asp:DropDownList id="drpBuilderID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvBuilderID" runat="server" Display="Dynamic" ControlToValidate="drpBuilderID" ErrorMessage="Field 'Builder I D' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Product I D:</td>
		<td class="field"><asp:DropDownList id="drpProductID" runat="server" /></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Special Order Product I D:</td>
		<td class="field"><asp:DropDownList id="drpSpecialOrderProductID" runat="server" /></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Creator Builder Account I D:</td>
		<td class="field"><asp:textbox id="txtCreatorBuilderAccountID" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><CC:IntegerValidator Display="Dynamic" runat="server" id="fvCreatorBuilderAccountID" ControlToValidate="txtCreatorBuilderAccountID" ErrorMessage="Field 'Creator Builder Account I D' is invalid" /></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Vendor Product Price Request?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

