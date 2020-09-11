<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Project"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If ProjectID = 0 Then %>Add<% Else %>Edit<% End If %> Project</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Builder:</td>
		<td class="field"><asp:DropDownList id="drpBuilderID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvBuilderID" runat="server" Display="Dynamic" ControlToValidate="drpBuilderID" ErrorMessage="Field 'Builder' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Project Name:</td>
		<td class="field"><asp:textbox id="txtProjectName" runat="server" maxlength="150" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvProjectName" runat="server" Display="Dynamic" ControlToValidate="txtProjectName" ErrorMessage="Field 'Project Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Lot Number:</td>
		<td class="field"><asp:textbox id="txtLotNumber" runat="server" maxlength="20" columns="20" style="width: 139px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Subdivision:</td>
		<td class="field"><asp:textbox id="txtSubdivision" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">Address:</td>
		<td class="field"><asp:textbox id="txtAddress" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvAddress" runat="server" Display="Dynamic" ControlToValidate="txtAddress" ErrorMessage="Field 'Address' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Address 2:</td>
		<td class="field"><asp:textbox id="txtAddress2" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">City:</td>
		<td class="field"><asp:textbox id="txtCity" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvCity" runat="server" Display="Dynamic" ControlToValidate="txtCity" ErrorMessage="Field 'City' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">State:</td>
		<td class="field"><asp:DropDownList id="drpState" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvState" runat="server" Display="Dynamic" ControlToValidate="drpState" ErrorMessage="Field 'State' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Zip:</td>
		<td class="field"><CC:Zip id="ctrlZip" runat="server" /></td>
		<td><CC:RequiredZipValidator ID="rfvZip" runat="server" Display="Dynamic" ControlToValidate="ctrlZip" ErrorMessage="Field 'Zip' is blank"></CC:RequiredZipValidator><CC:ZipValidator Display="Dynamic" runat="server" id="fvZip" ControlToValidate="ctrlZip" ErrorMessage="Zip 'Zip' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">County:</td>
		<td class="field"><asp:textbox id="txtCounty" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvCounty" runat="server" Display="Dynamic" ControlToValidate="txtCounty" ErrorMessage="Field 'County' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Portfolio:</td>
		<td class="field"><asp:DropDownList id="drpPortfolioID" runat="server" /></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">Project Status:</td>
		<td class="field"><asp:DropDownList id="drpProjectStatusID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvProjectStatusID" runat="server" Display="Dynamic" ControlToValidate="drpProjectStatusID" ErrorMessage="Field 'Project Status' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Start Date:</td>
		<td class="field"><CC:DatePicker ID="dtStartDate" runat="server"></CC:DatePicker></td>
		<td><CC:DateValidator Display="Dynamic" runat="server" id="dtvStartDate" ControlToValidate="dtStartDate" ErrorMessage="Date Field 'Start Date' is invalid" /></td>
	</tr>
	<tr>
		<td class="required"><b>Is Archived?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblIsArchived" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" />
			<asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
		<td><asp:RequiredFieldValidator ID="rfvIsArchived" runat="server" Display="Dynamic" ControlToValidate="rblIsArchived" ErrorMessage="Field 'Is Archived' is blank"></asp:RequiredFieldValidator></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Project?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

