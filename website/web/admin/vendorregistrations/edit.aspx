<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Vendor Registration"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If VendorRegistrationID = 0 Then %>Add<% Else %>Edit<% End If %> Vendor Registration</h4>
<p></p>
<CC:OneClickButton id="btnEditBusiness" runat="server" CausesValidation="false" Text="Business References" cssClass="btn"></CC:OneClickButton>
<CC:OneClickButton id="btnEditCustomer" runat="server"  CausesValidation="false" Text="Customer References" cssClass="btn"></CC:OneClickButton>
<CC:OneClickButton id="btnEditFinancial" runat="server"  CausesValidation="false" Text="Financial References" cssClass="btn"></CC:OneClickButton>
<CC:OneClickButton id="btnEditMember" runat="server" CausesValidation="false"  Text="Member References" cssClass="btn"></CC:OneClickButton>
<p></p>
<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Vendor:</td>
		<td class="field"><asp:DropDownList id="drpVendorID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvVendorID" runat="server" Display="Dynamic" ControlToValidate="drpVendorID" ErrorMessage="Field 'Vendor' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Historic Vendor ID:</td>
		<td class="field"><asp:textbox id="txtHistoricVendorID" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><CC:IntegerValidator Display="Dynamic" runat="server" id="fvHistoricVendorID" ControlToValidate="txtHistoricVendorID" ErrorMessage="Field 'Historic Vendor ID' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Years In Business:</td>
		<td class="field"><asp:textbox id="txtYearStarted" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvYearStarted" runat="server" Display="Dynamic" ControlToValidate="txtYearStarted" ErrorMessage="Field 'Years In Business' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvYearStarted" ControlToValidate="txtYearStarted" ErrorMessage="Field 'Years In Business' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Employees:</td>
		<td class="field"><asp:textbox id="txtEmployees" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvEmployees" runat="server" Display="Dynamic" ControlToValidate="txtEmployees" ErrorMessage="Field 'Employees' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvEmployees" ControlToValidate="txtEmployees" ErrorMessage="Field 'Employees' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Products Offered:</td>
		<td class="field"><asp:TextBox id="txtProductsOffered" style="width: 319px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
		<td><asp:RequiredFieldValidator ID="rfvProductsOffered" runat="server" Display="Dynamic" ControlToValidate="txtProductsOffered" ErrorMessage="Field 'Products Offered' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Company Memberships:</td>
		<td class="field"><asp:TextBox id="txtCompanyMemberships" style="width: 319px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
		<td></td>
	</tr>
	<tr>
		<td class="required"><b>Is Subsidiary?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblIsSubsidiary" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" />
			<asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
		<td><asp:RequiredFieldValidator ID="rfvIsSubsidiary" runat="server" Display="Dynamic" ControlToValidate="rblIsSubsidiary" ErrorMessage="Field 'Is Subsidiary' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Subsidiary Explanation:</td>
		<td class="field"><asp:TextBox id="txtSubsidiaryExplanation" style="width: 319px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
		<td></td>
	</tr>
	<tr>
		<td class="required"><b>Had Lawsuit?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblHadLawsuit" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" />
			<asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
		<td><asp:RequiredFieldValidator ID="rfvHadLawsuit" runat="server" Display="Dynamic" ControlToValidate="rblHadLawsuit" ErrorMessage="Field 'Had Lawsuit' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Lawsuit Explanation:</td>
		<td class="field"><asp:textbox id="txtLawsuitExplanation" runat="server" maxlength="2000" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">Supply Area:</td>
		<td class="field"><asp:TextBox id="txtSupplyArea" style="width: 319px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
		<td><asp:RequiredFieldValidator ID="rfvSupplyArea" runat="server" Display="Dynamic" ControlToValidate="txtSupplyArea" ErrorMessage="Field 'Supply Area' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Primary Supply Phase:</td>
		<td class="field"><asp:DropDownList id="drpPrimarySupplyPhaseID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvdrpPrimarySupplyPhaseID" runat="server" Display="Dynamic" ControlToValidate="drpPrimarySupplyPhaseID" ErrorMessage="Field 'Primary Supply Phase' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Secondary Supply Phase:</td>
		<td class="field"><asp:DropDownList id="drpSecondarySupplyPhaseID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvdrpSecondarySupplyPhaseID" runat="server" Display="Dynamic" ControlToValidate="drpSecondarySupplyPhaseID" ErrorMessage="Field 'Secondary Supply Phase' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Preparer First Name:</td>
		<td class="field"><asp:textbox id="txtPreparerFirstName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvPreparerFirstName" runat="server" Display="Dynamic" ControlToValidate="txtPreparerFirstName" ErrorMessage="Field 'Preparer First Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Preparer Last Name:</td>
		<td class="field"><asp:textbox id="txtPreparerLastName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvPreparerLastName" runat="server" Display="Dynamic" ControlToValidate="txtPreparerLastName" ErrorMessage="Field 'Preparer Last Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required"><b>Accepts Terms?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblAcceptsTerms" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" />
			<asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
		<td><asp:RequiredFieldValidator ID="rfvAcceptsTerms" runat="server" Display="Dynamic" ControlToValidate="rblAcceptsTerms" ErrorMessage="Field 'Accepts Terms' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Notes:</td>
		<td class="field"><asp:TextBox id="txtNotes" style="width: 319px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">Submitted:</td>
		<td class="field"><CC:DatePicker ID="dtSubmitted" runat="server"></CC:DatePicker></td>
		<td><CC:RequiredDateValidator Display="Dynamic" runat="server" id="rdtvSubmitted" ControlToValidate="dtSubmitted" ErrorMessage="Date Field 'Submitted' is blank" /><CC:DateValidator Display="Dynamic" runat="server" id="dtvSubmitted" ControlToValidate="dtSubmitted" ErrorMessage="Date Field 'Submitted' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Updated:</td>
		<td class="field"><CC:DatePicker ID="dtUpdated" runat="server"></CC:DatePicker></td>
		<td><CC:DateValidator Display="Dynamic" runat="server" id="dtvUpdated" ControlToValidate="dtUpdated" ErrorMessage="Date Field 'Updated' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Registration Status:</td>
		<td class="field"><asp:DropDownList id="drpRegistrationStatusID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvRegistrationStatusID" runat="server" Display="Dynamic" ControlToValidate="drpRegistrationStatusID" ErrorMessage="Field 'Registration Status' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Approved:</td>
		<td class="field"><CC:DatePicker ID="dtApproved" runat="server"></CC:DatePicker></td>
		<td><CC:DateValidator Display="Dynamic" runat="server" id="dtvApproved" ControlToValidate="dtApproved" ErrorMessage="Date Field 'Approved' is invalid" /></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Vendor Registration?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>
<p></p>
</asp:content>

