<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Builder Registration"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If BuilderRegistrationID = 0 Then %>Add<% Else %>Edit<% End If %> Builder Registration</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Builder ID:</td>
		<td class="field"><asp:DropDownList id="drpBuilderID" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvBuilderID" runat="server" Display="Dynamic" ControlToValidate="drpBuilderID" ErrorMessage="Field 'Builder ID' is blank"></asp:RequiredFieldValidator></td>
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
		<td class="required">Size Range Min:</td>
		<td class="field"><asp:textbox id="txtSizeRangeMin" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvSizeRangeMin" runat="server" Display="Dynamic" ControlToValidate="txtSizeRangeMin" ErrorMessage="Field 'Size Range Min' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvSizeRangeMin" ControlToValidate="txtSizeRangeMin" ErrorMessage="Field 'Size Range Min' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Size Range Max:</td>
		<td class="field"><asp:textbox id="txtSizeRangeMax" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvSizeRangeMax" runat="server" Display="Dynamic" ControlToValidate="txtSizeRangeMax" ErrorMessage="Field 'Size Range Max' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvSizeRangeMax" ControlToValidate="txtSizeRangeMax" ErrorMessage="Field 'Size Range Max' is invalid" /></td>
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
		<td class="field"><asp:textbox id="txtAvgCostPerSqFt" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvAvgCostPerSqFt" runat="server" Display="Dynamic" ControlToValidate="txtAvgCostPerSqFt" ErrorMessage="Field 'Avg Cost Per Sq Ft' is blank"></asp:RequiredFieldValidator><CC:IntegerValidator Display="Dynamic" runat="server" id="fvAvgCostPerSqFt" ControlToValidate="txtAvgCostPerSqFt" ErrorMessage="Field 'Avg Cost Per Sq Ft' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Revenue Last Year:</td>
		<td class="field"><asp:textbox id="txtRevenueLastYear" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvRevenueLastYear" runat="server" Display="Dynamic" ControlToValidate="txtRevenueLastYear" ErrorMessage="Field 'Revenue Last Year' is blank"></asp:RequiredFieldValidator><CC:FloatValidator Display="Dynamic" runat="server" id="fvRevenueLastYear" ControlToValidate="txtRevenueLastYear" ErrorMessage="Field 'Revenue Last Year' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Revenue Next Year:</td>
		<td class="field"><asp:textbox id="txtRevenueNextYear" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvRevenueNextYear" runat="server" Display="Dynamic" ControlToValidate="txtRevenueNextYear" ErrorMessage="Field 'Revenue Next Year' is blank"></asp:RequiredFieldValidator><CC:FloatValidator Display="Dynamic" runat="server" id="fvRevenueNextYear" ControlToValidate="txtRevenueNextYear" ErrorMessage="Field 'Revenue Next Year' is invalid" /></td>
	</tr>
	<tr>
		<td class="required">Total COGS:</td>
		<td class="field"><asp:textbox id="txtTotalCOGS" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvTotalCOGS" runat="server" Display="Dynamic" ControlToValidate="txtTotalCOGS" ErrorMessage="Field 'Total COGS' is blank"></asp:RequiredFieldValidator><CC:FloatValidator Display="Dynamic" runat="server" id="fvTotalCOGS" ControlToValidate="txtTotalCOGS" ErrorMessage="Field 'Total COGS' is invalid" /></td>
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
		<td class="required"><b>Accepts Terms?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblAcceptsTerms" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" />
			<asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Builder Registration?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
