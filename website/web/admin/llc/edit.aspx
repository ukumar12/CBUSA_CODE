<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="LLC"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
<script type="text/javascript" src="/includes/moment.js"></script>
<script type="text/javascript">
    function CheckBillingStartDate() {
        var BillingStartDate = document.getElementById("ctl00_ph_dtpBillingStartDate_txtDatePicker").value;
        var date = moment(BillingStartDate)
        var now = moment();

        if (now < date) {
            //date is in future
            var result = confirm("The Billing Start Date you specified is in the future. This is appropriate if the market has not launched yet. However, many billing plans already include a free introductory period, so you should NOT specify a future date to provide one.");
            return result;
        }
    }
</script>
<h4><% If LLCID = 0 Then %>Add<% Else %>Edit<% End If %> LLC</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">LLC:</td>
		<td class="field"><asp:textbox id="txtLLC" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvLLC" runat="server" Display="Dynamic" ControlToValidate="txtLLC" ErrorMessage="Field 'LLC' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Description:</td>
		<td class="field"><asp:TextBox id="txtDescription" style="width: 319px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
		<td></td>
	</tr>
    <tr>
		<td class="required">BuilderGroup:</td>
		<td class="field"><asp:textbox id="txtBuilderGroup" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvBuilderGroup" runat="server" Display="Dynamic" ControlToValidate="txtBuilderGroup" ErrorMessage="Field 'BuilderGroup' is blank"></asp:RequiredFieldValidator></td>
	</tr>
     <tr>
		<td class="required">CBUSA Operations Manager Email :</td>
		<td class="field"><asp:textbox id="txtOperationsManager" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvOperationsManager" runat="server" Display="Dynamic" ControlToValidate="txtOperationsManager" ErrorMessage="Field 'OperationsManager' is blank"></asp:RequiredFieldValidator></td>
	    <td> <CC:EmailValidator id="emailvalidatoremail" runat="server" ErrorMessage=" Operations Manager Email is not valid" ControlToValidate="txtOperationsManager" Display="Dynamic"></CC:EmailValidator></td>
     </tr>

     <tr>
		<td class="optional">Email Notification List(CBUSA):<br />(comma seperated email address allowed)</td>
		<td class="field"><asp:textbox id="txtEmailNotification" runat="server"  columns="50" style="width: 319px;"></asp:textbox></td>
	</tr>

	<tr>
		<td class="required">Discrepency Tolerance:</td>
		<td class="field"><asp:textbox id="txtDiscrepencyTolerance" runat="server" maxlength="8" columns="8" style="width: 67px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvDiscrepencyTolerance" runat="server" Display="Dynamic" ControlToValidate="txtDiscrepencyTolerance" ErrorMessage="Field 'Discrepency Tolerance' is blank"></asp:RequiredFieldValidator><CC:FloatValidator Display="Dynamic" runat="server" id="fvDiscrepencyTolerance" ControlToValidate="txtDiscrepencyTolerance" ErrorMessage="Field 'Discrepency Tolerance' is invalid" /></td>
	</tr>

	<tr>
		<td class="required"><b>Is Active?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblIsActive" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" />
			<asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
	</tr>
	<tr>
	    <td class="optional"><b>Exclude from reporting?</b></td>
	    <td class="field">
	        <asp:RadioButtonList runat="server" ID="rblExcludeFromReporting" RepeatDirection="Horizontal">
	            <asp:ListItem Text="Yes" Value="True"></asp:ListItem>
	            <asp:ListItem Text="No" Value="False" Selected="True"></asp:ListItem>
	        </asp:RadioButtonList>
	    </td>
	</tr>

    <tr>
	    <td class="optional"><b>Allow Excluding Vendors</b></td>
	    <td class="field">
	        <asp:RadioButtonList runat="server" ID="rblAllowExcludingVendors" RepeatDirection="Horizontal">
	            <asp:ListItem Text="Yes" Value="True"></asp:ListItem>
	            <asp:ListItem Text="No" Value="False" Selected="True"></asp:ListItem>
	        </asp:RadioButtonList>
	    </td>
	</tr>

	<tr>
		<td class="optional">Default Rebate Percentage:</td>
		<td class="field"><asp:textbox id="txtDefaultRebate" runat="server" maxlength="4" columns="4" style="width: 67px;"></asp:textbox>%</td>
		<td><CC:FloatValidator Display="Dynamic" runat="server" id="fvDefaultRebate" ControlToValidate="txtDefaultRebate" ErrorMessage="Field 'Default Rebate' is invalid." /></td>
	</tr>
    <tr>
		<td class="optional">LLC:</td>
		<td class="field"><asp:TextBox id="txtAffliateID" style="width: 319px;" runat="server"></asp:TextBox></td>
		<td></td>
	</tr>
    <tr>
		<td class="required"><b>Enable TakeOffService?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblTakeOffserivce" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" />
			<asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
	</tr>
    <tr>
		<td class="optional">Registration Billing Plan:</td>
		<td class="field">
            <asp:DropDownList ID="F_RegBillingPlan" runat="server" />
        </td>
		<td>
		</td>
	</tr>
    <tr>
		<td class="optional">Subscription Billing Plan:</td>
		<td class="field">
            <asp:DropDownList ID="F_SubBillingPlan" runat="server" />
        </td>
		<td>
		</td>
	</tr>
    <tr>
		<td class="optional">Billing Start Date:</td>
		<td class="field">
            <CC:DatePicker id="dtpBillingStartDate" runat="server" />
		</td>
		<td></td>
	</tr>
</table>

<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn" OnClientClick="return CheckBillingStartDate();"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this LLC?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

