<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Builder Registration"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If BuilderRegistrationID = 0 Then %>Add<% Else %>Edit<% End If %> Builder Registration</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Builder:</td>
		<td class="field"><asp:DropDownList id="drpBuilderID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvBuilderID" runat="server" Display="Dynamic" ControlToValidate="drpBuilderID" ErrorMessage="Field 'Builder I D' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Year Started:</td>
		<td class="field"><asp:textbox id="txtYearStarted" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvYearStarted" runat="server" Display="Dynamic" ControlToValidate="txtYearStarted" ErrorMessage="Field 'Year Started' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvYearStarted" ControlToValidate="txtYearStarted" ErrorMessage="Field 'Year Started' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Employees:</td>
		<td class="field"><asp:textbox id="txtEmployees" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvEmployees" runat="server" Display="Dynamic" ControlToValidate="txtEmployees" ErrorMessage="Field 'Employees' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvEmployees" ControlToValidate="txtEmployees" ErrorMessage="Field 'Employees' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Homes Built And Delivered:</td>
		<td class="field"><asp:textbox id="txtHomesBuiltAndDelivered" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvHomesBuiltAndDelivered" runat="server" Display="Dynamic" ControlToValidate="txtHomesBuiltAndDelivered" ErrorMessage="Field 'Homes Built And Delivered' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvHomesBuiltAndDelivered" ControlToValidate="txtHomesBuiltAndDelivered" ErrorMessage="Field 'Homes Built And Delivered' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Home Starts Last Year:</td>
		<td class="field"><asp:textbox id="txtHomeStartsLastYear" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvHomeStartsLastYear" runat="server" Display="Dynamic" ControlToValidate="txtHomeStartsLastYear" ErrorMessage="Field 'Home Starts Last Year' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvHomeStartsLastYear" ControlToValidate="txtHomeStartsLastYear" ErrorMessage="Field 'Home Starts Last Year' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Home Starts Next Year:</td>
		<td class="field"><asp:textbox id="txtHomeStartsNextYear" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><CC:IntegerValidator Display="Dynamic" runat="server" id="fvHomeStartsNextYear" ControlToValidate="txtHomeStartsNextYear" ErrorMessage="Field 'Home Starts Next Year' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Price Range Min:</td>
		<td class="field"><asp:textbox id="txtPriceRangeMin" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvPriceRangeMin" runat="server" Display="Dynamic" ControlToValidate="txtPriceRangeMin" ErrorMessage="Field 'Price Range Min' is blank"></asp:RequiredFieldValidator><CC:FloatValidator Display="Dynamic" runat="server" id="fvPriceRangeMin" ControlToValidate="txtPriceRangeMin" ErrorMessage="Field 'Price Range Min' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Price Range Max:</td>
		<td class="field"><asp:textbox id="txtPriceRangeMax" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvPriceRangeMax" runat="server" Display="Dynamic" ControlToValidate="txtPriceRangeMax" ErrorMessage="Field 'Price Range Max' is blank"></asp:RequiredFieldValidator><CC:FloatValidator Display="Dynamic" runat="server" id="fvPriceRangeMax" ControlToValidate="txtPriceRangeMax" ErrorMessage="Field 'Price Range Max' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Avg Cost Per Sq Ft:</td>
		<td class="field"><asp:textbox id="txtAvgCostPerSqFt" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvAvgCostPerSqFt" runat="server" Display="Dynamic" ControlToValidate="txtAvgCostPerSqFt" ErrorMessage="Field 'Avg Cost Per Sq Ft' is blank"></asp:RequiredFieldValidator><CC:FloatValidator Display="Dynamic" runat="server" id="fvAvgCostPerSqFt" ControlToValidate="txtAvgCostPerSqFt" ErrorMessage="Field 'Avg Cost Per Sq Ft' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Affiliations:</td>
		<td class="field"><asp:TextBox id="txtAffiliations" style="width: 349px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">Where You Build:</td>
		<td class="field"><asp:TextBox id="txtWhereYouBuild" style="width: 349px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
		<td><asp:RequiredFieldValidator ID="rfvWhereYouBuild" runat="server" Display="Dynamic" ControlToValidate="txtWhereYouBuild" ErrorMessage="Field 'Where You Build' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Awards:</td>
		<td class="field"><asp:TextBox id="txtAwards" style="width: 349px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
		<td></td>
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
		<td class="required">Direct Costs Last Year:</td>
		<td class="field"><asp:textbox id="txtDirectCostsLastYear" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvDirectCostsLastYear" runat="server" Display="Dynamic" ControlToValidate="txtDirectCostsLastYear" ErrorMessage="Field 'Direct Costs Last Year' is blank"></asp:RequiredFieldValidator><CC:FloatValidator Display="Dynamic" runat="server" id="fvDirectCostsLastYear" ControlToValidate="txtDirectCostsLastYear" ErrorMessage="Field 'Direct Costs Last Year' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Unsold Last Year:</td>
		<td class="field"><asp:textbox id="txtUnsoldLastYear" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvUnsoldLastYear" runat="server" Display="Dynamic" ControlToValidate="txtUnsoldLastYear" ErrorMessage="Field 'Unsold Last Year' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvUnsoldLastYear" ControlToValidate="txtUnsoldLastYear" ErrorMessage="Field 'Unsold Last Year' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Under Construction Last Year:</td>
		<td class="field"><asp:textbox id="txtUnderConstructionLastYear" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvUnderConstructionLastYear" runat="server" Display="Dynamic" ControlToValidate="txtUnderConstructionLastYear" ErrorMessage="Field 'Under Construction Last Year' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvUnderConstructionLastYear" ControlToValidate="txtUnderConstructionLastYear" ErrorMessage="Field 'Under Construction Last Year' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Complete Date:</td>
		<td class="field"><CC:DatePicker ID="dtCompleteDate" runat="server"></CC:DatePicker></td>
		<td><CC:DateValidator Display="Dynamic" runat="server" id="dtvCompleteDate" ControlToValidate="dtCompleteDate" ErrorMessage="Date Field 'Complete Date' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Registration Status:</td>
		<td class="field"><asp:DropDownList id="drpRegistrationStatusID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvRegistrationStatusID" runat="server" Display="Dynamic" ControlToValidate="drpRegistrationStatusID" ErrorMessage="Field 'Registration Status' is blank"></asp:RequiredFieldValidator></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Builder Registration?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

