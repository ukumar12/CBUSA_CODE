<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="N C P Manufacturer"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If NCPManufacturerID = 0 Then %>Add<% Else %>Edit<% End If %> N C P Manufacturer</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	 
	<tr>
		<td class="optional">Company Name:</td>
		<td class="field"><asp:textbox id="txtCompanyName" runat="server" maxlength="300" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
    <tr>
		<td class="optional">Website:</td>
		<td class="field"><asp:textbox id="txtWebsite" runat="server" maxlength="150" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
    <tr>
		<td><b><i>Mailing Information:</i> </b></td>
		<td></td>
		<td></td>
	</tr>

	<tr>
		<td class="optional">Mailing Address:</td>
		<td class="field"><asp:textbox id="txtMailingAddress" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Mailing City:</td>
		<td class="field"><asp:textbox id="txtMailingCity" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Mailing State:</td>
		<td class="field"><asp:DropDownList ID="drpMailingState" runat="server" /></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Mailing Zip:</td>
		<td class="field"><asp:textbox id="txtMailingZip" runat="server" maxlength="15" columns="15" style="width: 109px;"></asp:textbox></td>
		<td></td>
	</tr>
	  <tr>
		<td><b><i>Primary Contact Information:</i> </b></td>
		<td></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Primary Contact Name:</td>
		<td class="field"><asp:textbox id="txtPrimaryContactName" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Primary Contact Email:</td>
		<td class="field"><asp:textbox id="txtPrimaryContactEmail" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Primary Contact Phone:</td>
		<td class="field"><asp:textbox id="txtPrimaryContactPhone" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>

     <tr>
		<td><b><i>AP Contact Information:</i> </b></td>
		<td></td>
		<td></td>
	</tr>

	<tr>
		<td class="optional">A P Contact Name:</td>
		<td class="field"><asp:textbox id="txtAPContactName" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">A P Contact Email:</td>
		<td class="field"><asp:textbox id="txtAPContactEmail" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">A P Contact Phone:</td>
		<td class="field"><asp:textbox id="txtAPContactPhone" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Payment Terms:</td>
		<td class="field"><asp:textbox id="txtPaymentTerms" runat="server" maxlength="500" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this N C P Manufacturer?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
