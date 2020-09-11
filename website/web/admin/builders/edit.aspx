<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Builder"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
<asp:PlaceHolder ID="PlaceHolder1" runat="server">
<script type="text/javascript" src="/includes/moment.js"></script>
<script type ="text/javascript" >
    function clicked(e) {
        if (!confirm('Please confirm Market Selection')) e.preventDefault();
    }

    function CalcDate() {
        CalculateSubscriptionStartDate();
    }

    function CheckBillingStartDate() {
        var AutoBillId = document.getElementById("ctl00_ph_txtBillingSubscriptionAutobill").value;

        if (AutoBillId == '') {
            var BillingStartDate = document.getElementById("ctl00_ph_dtpBillingStartDate_txtDatePicker").value;
            var dtBillStartDate = moment(BillingStartDate);

            //var SubscriptionStartDate = document.getElementById("ctl00_ph_txtSubscriptionStartDate").value;
            //var dtSubStartDate = moment(SubscriptionStartDate);

            var now = moment();

            if (now > dtBillStartDate) {
                //date is in past
                alert("Billing/Subscription Start Date can not be in the past. Please specify a future date!");
                return false;
            } else {
                if (!confirm("Are you sure you want to update the Builder's Billing Plan?")) {
                    return false;
                }
            }
        } else {
            if (!confirm("Are you sure you want to update the Builder's Billing Plan?")) {
                return false;
            }
        }
    }

    function CalculateSubscriptionStartDate() {
        var BillingStartDate = document.getElementById("ctl00_ph_dtpBillingStartDate_txtDatePicker").value;
        var FreeTrialPeriod = document.getElementById("ctl00_ph_ddlFreeTrialPeriod").value;

        var dtBillingStartDate = moment(BillingStartDate);
        var dtSubscriptionStartDate = moment(dtBillingStartDate).add(FreeTrialPeriod, 'M').format('L');

        document.getElementById("ctl00_ph_txtSubscriptionStartDate").value = dtSubscriptionStartDate;
    }

    function SelectRegBillingPlanExternalKey() {
        var SelectedRegBillingPlanId = document.getElementById("ctl00_ph_F_RegBillingPlan").value;
        document.getElementById("ctl00_ph_drpRegBillingPlanExternalKey").value = SelectedRegBillingPlanId;
    }

    function SelectSubBillingPlanExternalKey() {
        var SelectedSubBillingPlanId = document.getElementById("ctl00_ph_F_SubBillingPlan").value;
        document.getElementById("ctl00_ph_drpSubBillingPlanExternalKey").value = SelectedSubBillingPlanId;
    }

    function SelectLLCBillingPlan() {
        var SelectedLLC = document.getElementById("ctl00_ph_drpLLC").value;

        document.getElementById("ctl00_ph_drpLLCRegBillingPlan").value = SelectedLLC;
        document.getElementById("ctl00_ph_drpLLCSubBillingPlan").value = SelectedLLC;
        document.getElementById("ctl00_ph_drpLLCBillingStartDate").value = SelectedLLC;

        var selectedRegBillingPlan = $("#ctl00_ph_drpLLCRegBillingPlan").find("option:selected").text();
        var selectedSubBillingPlan = $("#ctl00_ph_drpLLCSubBillingPlan").find("option:selected").text();
        var selectedBillingStartDate = moment($("#ctl00_ph_drpLLCBillingStartDate").find("option:selected").text()).format('L');

        var date = moment(selectedBillingStartDate);
        var now = moment();

        if (now < date) { //date is in future
            document.getElementById("ctl00_ph_dtpBillingStartDate_txtDatePicker").value = selectedBillingStartDate;
            document.getElementById("ctl00_ph_txtSubscriptionStartDate").value = selectedBillingStartDate;
        }

        document.getElementById("ctl00_ph_drpRegBillingPlanExternalKey").value = selectedRegBillingPlan;
        document.getElementById("ctl00_ph_drpSubBillingPlanExternalKey").value = selectedSubBillingPlan;

        document.getElementById("ctl00_ph_F_RegBillingPlan").value = selectedRegBillingPlan;
        document.getElementById("ctl00_ph_F_SubBillingPlan").value = selectedSubBillingPlan;
    }

    $(document).ready(function () {

        $('input.datepicker-control').datepicker({
            changeMonth: true,
            changeYear: true,
            showButtonPanel: true,
            showOn: 'both',
            //buttonImage: '/images/calendar/picker.gif',
            //buttonImageOnly: true,
            onSelect: function () { alert('override'); }
        });

        //$('#ph_dtpBillingStartDate').datepicker({
        //    onValueChanged: function (e) {
        //        console.log("Date on valuechanged: ");
        //    },
        //    onHide: function (e) {
        //        console.log("Date hide: ");
        //    },
        //    onSelect: function (e, i) {
        //        console.log("Date selected: ");
        //    },
        //    onClose: function (e) {
        //        console.log("Date closed: ");
        //    },
        //    onChangeMonthYear: function (year, month, inst) {
        //        console.log("Date changed month year: ");
        //    },
        //    onChange: function (e) {
        //        console.log("Date changed: ");
        //    }
        //});

        //$("#ph_dtpBillingStartDate_txtDatePicker").change(function () {
        //    console.log("text changed ");
        //});

        //$("button.ui-datepicker-close").click(function () {
        //    console.log("DONE clicked");
        //});

        //$("#ph_dtpBillingStartDate_txtDatePicker").click(function () {
        //    console.log("text clicked");
        //});

        //$("a").click(function () {
        //    console.log("a clicked");
        //});

        //$("#ph_dtpBillingStartDate").datepicker({
        //    //onChange: function (d, i) {
        //    //    //$(".ui-datepicker-inline").hide();
        //    //    //console.log("--on change-");
        //    //    //$(this).change();
        //    //    //$(this).hide();
        //    //    //display("Selected date: " + dateText + "; input's current value: " + this.value);
        //    //}
        //});

        //$("#ph_dtpBillingStartDate").datepicker({
        //    onClose: function (d, i) {
        //        console.log("--on close-");
        //    }
        //});




    //    $('input.datepicker-control').datepicker({
    //    changeMonth: true,
    //    changeYear: true,
    //    onChangeMonthYear: function (year, month, datePicker) {
    //        console.log("here");
    //    }
    //});
    });
</script>

</asp:PlaceHolder> 
<h4><% If BuilderID = 0 Then %>Add<% Else %>Edit<% End If %> Builder</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="optional">CRM ID:</td>
		<td class="field"><asp:textbox id="txtCRMID" runat="server" maxlength="20" columns="20" style="width: 139px;"></asp:textbox></td>
		<td>&nbsp;</td>
	</tr>
    <tr>
		<td class="required">LLC:</td>
		<td class="field">
            <asp:DropDownList ID="drpLLC" runat="server" onchange="SelectLLCBillingPlan();"></asp:DropDownList>
            <asp:DropDownList ID="drpRegBillingPlanExternalKey" runat="server" style="display:none"></asp:DropDownList>
            <asp:DropDownList ID="drpSubBillingPlanExternalKey" runat="server" style="display:none"></asp:DropDownList>
            <asp:DropDownList ID="drpLLCRegBillingPlan" runat="server" style="display:none"></asp:DropDownList>
            <asp:DropDownList ID="drpLLCSubBillingPlan" runat="server" style="display:none"></asp:DropDownList>
            <asp:DropDownList ID="drpLLCBillingStartDate" runat="server" style="display:none"></asp:DropDownList>
		</td>
		<td><asp:RequiredFieldValidator ID="rfvLLC" runat="server" Display="Dynamic" ControlToValidate="drpLLC" ErrorMessage="Field 'LLC' is blank"></asp:RequiredFieldValidator></td>
	</tr>

	<tr>
		<td class="required">Email:</td>
		<td class="field"><asp:textbox id="txtEmail" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvEmail" runat="server" Display="Dynamic" ControlToValidate="txtEmail" ErrorMessage="Field 'Email' is blank"></asp:RequiredFieldValidator>
            <%--<asp:RegularExpressionValidator runat="server" Display="Dynamic" ErrorMessage="Please enter a valid email" ControlToValidate="txtEmail" ID="revEmail" ValidationExpression="^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,30})$"></asp:RegularExpressionValidator>--%>
            <CC:EmailValidator ControlToValidate="txtEmail" ID="revEmail" ErrorMessage="Please enter a valid email" runat="server"></CC:EmailValidator>
		</td>
	</tr>
	 
	<tr>
		<td class="field"></td>
		<td class="field"><div id="divAvailability"></div></td>
	</tr>
	 
	<tr>
		<td class="required">Company Name:</td>
		<td class="field"><asp:textbox id="txtCompanyName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvCompanyName" runat="server" Display="Dynamic" ControlToValidate="txtCompanyName" ErrorMessage="Field 'Company Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
    <tr>
		<td class="optional">Accounting Alias:</td>
		<td class="field"><asp:textbox id="txtAlias" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
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
		<td class="field"><asp:DropDownList ID="drpState" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvState" runat="server" Display="Dynamic" ControlToValidate="drpState" ErrorMessage="Field 'State' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Zip:</td>
		<td class="field"><asp:textbox id="txtZip" runat="server" maxlength="15" columns="15" style="width: 109px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvZip" runat="server" Display="Dynamic" ControlToValidate="txtZip" ErrorMessage="Field 'Zip' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Phone:</td>
		<td class="field"><asp:textbox id="txtPhone" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvPhone" runat="server" Display="Dynamic" ControlToValidate="txtPhone" ErrorMessage="Field 'Phone' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Mobile:</td>
		<td class="field"><asp:textbox id="txtMobile" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Fax:</td>
		<td class="field"><asp:textbox id="txtFax" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Website URL:</td>
		<td class="field"><asp:textbox id="txtWebsiteURL" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox><br/><span class="smaller">http:// or https:// are required</span></td>
		<td><CC:URLValidator Display="Dynamic" runat="server" id="lnkvWebsiteURL" ControlToValidate="txtWebsiteURL" ErrorMessage="Link 'Website URL' is invalid" /></td>
	</tr>
	<tr style ="display :none">
		<td class="required">Registration Status:</td>
		<td class="field"><asp:DropDownList id="drpRegistrationStatusID" runat="server" /></td>
		
	</tr>
	<tr style ="display :none">
		<td class="optional">Preferred Vendor:</td>
		<td class="field"><asp:DropDownList id="drpPreferredVendorID" runat="server" /></td>
		<td></td>
	</tr>
	<tr>
		<td class="required"><b>Is Active?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblIsActive" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" Selected="True" />
			<asp:ListItem Text="No" Value="False"  />
			</asp:RadioButtonList>
		</td>
	</tr>
	<tr>
		<td class="required"><b>Is Plans Online?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblIsPlansOnline" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" Selected="True"/>
			<asp:ListItem Text="No" Value="False"   />
			</asp:RadioButtonList>
		</td>
		<td></td>
	</tr>
	<tr>
		<td class="required"><b>Is New?</b><br /><span class="smallest">Must pay registration fee.</span></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblIsNew" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True"  Selected="True" />
			<asp:ListItem Text="No" Value="False"/>
			</asp:RadioButtonList>
		</td>
	</tr>
	
	<tr>
	    <td class="required">Skip Entitlement Check?</td>
	    <td class="field">
	        <asp:RadioButtonList runat="server" ID="rblSkipEntitlementCheck" RepeatDirection="Horizontal">
	            <asp:ListItem Text="Yes" Value="True"></asp:ListItem>
	            <asp:ListItem Text="No" Value="False" Selected="True" ></asp:ListItem>
	        </asp:RadioButtonList>
	    </td>
	</tr>


    <tr>
	    <td class="required">Access to All Documents ?</td>
	    <td class="field">
	        <asp:RadioButtonList runat="server" ID="rblDocumentAccess" RepeatDirection="Horizontal">
	            <asp:ListItem Text="Yes" Value="True" Selected="True"></asp:ListItem>
	            <asp:ListItem Text="No" Value="False" ></asp:ListItem>
	        </asp:RadioButtonList>
	    </td>
	</tr>

    <tr>
		<td class="optional"><b>Show in Quarterly Reporting?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblQuarterlyReportingOn" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" Selected="True" />
			<asp:ListItem Text="No" Value="False" />
			</asp:RadioButtonList>
		</td>
		<td></td>
	</tr>




   <%-- <tr>
		<td class="required"><b>Rebates Email Preference?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="RblEmailPreference" RepeatDirection="Vertical">
			<asp:ListItem Text="Automatically send Past Due Rebate Notice to all vendors once every three weeks" Value="False"  Selected="True" />
			<asp:ListItem Text="Manually send Past Due Rebate Notice to selected vendors" Value="True" />
			</asp:RadioButtonList>
		</td>
	</tr>--%>

</table>
<p></p>
<CC:ConfirmButton id="btnSave" OnClientClick=""  Message="Please confirm market Selection" runat="server" Text="Save" cssClass="btn"></CC:ConfirmButton>
<CC:ConfirmButton id="btnDelete" Visible ="false"  runat="server" Message="Are you sure want to delete this Builder?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>
<CC:OneClickButton id="btnRegistrations" runat="server" Text="View Builder Registrations" cssClass="btn" CausesValidation="False"></CC:OneClickButton>
<p></p>
    
<table border="0" cellspacing="1" cellpadding="2">
    <tr><td colspan="3"><span style="font-size:14px;color:maroon;"><b>BILLING PLAN INFO</b></span></td></tr>
    <tr>
	    <td class="optional">Registration Billing Plan:</td>
		<td class="field">
            <asp:DropDownList ID="F_RegBillingPlan" runat="server" onchange="SelectRegBillingPlanExternalKey();" />
            <asp:HiddenField ID="hdnPrevRegBillingPlan" runat="server" />
        </td>
		<td></td>
	</tr>

    <tr>
		<td class="optional">Subscription Start Date:</td>
		<td class="field">
            <CC:DatePicker id="dtpBillingStartDate" runat="server" onChangeMonthYear="true" />
            <asp:TextBox ID="txtBillingStartDate" runat="server" Enabled="false"></asp:TextBox>
            <input type="hidden" id="hdnDtpDate" />
            <asp:HiddenField ID="hdnPrevBillingStartDate" runat="server" />
		</td>
		<td></td>
	</tr>

    <tr>
	    <td class="optional">Subscription Billing Plan:</td>
		<td class="field">
            <asp:DropDownList ID="F_SubBillingPlan" runat="server" onchange="SelectSubBillingPlanExternalKey();" />
        </td>
		<td></td>
	</tr>

    <tr>
	    <td class="optional">Subscription Free Trial:</td>
		<td class="field">
            <asp:DropDownList ID="ddlFreeTrialPeriod" runat="server" onchange="CalculateSubscriptionStartDate();" />
        </td>
		<td></td>
	</tr>

    <tr>
		<td class="optional">Effective Billing Date:</td>
		<td class="field">
            <asp:textbox id="txtSubscriptionStartDate" runat="server" maxlength="50" columns="50" style="width:100px;" Enabled="false" ReadOnly="true"></asp:textbox>
		</td>
		<td></td>
	</tr>

    <tr>
		<td class="optional">Billing Last Success:</td>
		<td class="field">
            <asp:textbox id="txtBillingLastSuccess" runat="server" maxlength="50" columns="50" style="width:100px;" Enabled="false" ReadOnly="true"></asp:textbox>
		</td>
		<td></td>
	</tr>

    <tr>
		<td class="optional">Billing Subscription Autobill:</td>
		<td class="field">
            <asp:textbox id="txtBillingSubscriptionAutobill" runat="server" maxlength="50" columns="50" style="width: 319px;" Enabled="false" ReadOnly="true"></asp:textbox>
		</td>
		<td></td>
	</tr>

    <tr>
		<td class="optional">Billing Membership Autobill:</td>
		<td class="field">
            <asp:textbox id="txtBillingMembershipAutobill" runat="server" maxlength="50" columns="50" style="width: 319px;" Enabled="false" ReadOnly="true"></asp:textbox>
		</td>
		<td></td>
	</tr>

    <tr>
		<td class="optional">Billing Processor ID:</td>
		<td class="field">
            <asp:textbox id="txtBillingProcessorID" runat="server" maxlength="50" columns="50" style="width: 319px;" Enabled="false" ReadOnly="true"></asp:textbox>
		</td>
		<td></td>
	</tr>

</table>
<p></p>
<CC:ConfirmButton id="btnUpgradeBillingPlan" OnClientClick="return CheckBillingStartDate();"  Message="Are you sure you want to upgrade the Builder's Billing Plan?" runat="server" Text="Update Billing Info" cssClass="btn"></CC:ConfirmButton>

</asp:content>
