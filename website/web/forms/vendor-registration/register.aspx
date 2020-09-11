<%@ Page Language="VB" AutoEventWireup="false" CodeFile="register.aspx.vb" Inherits="forms_vendor_registration_register" %>
<%@ Register TagName="VendorRegistrationSteps" TagPrefix="CC" Src="~/controls/VendorRegistrationSteps.ascx" %>
<%@ Register TagName="InfoBox" TagPrefix="CC" Src="~/controls/infobox.ascx" %>

<CT:MasterPage ID="CTMain" runat="server">

<CC:VendorRegistrationSteps id="ctlSteps" runat="server" RegistrationStep="3"></CC:VendorRegistrationSteps>

<asp:PlaceHolder runat="server">
	<script type="text/javascript">
        function ToggleCards(show) {
            var div = $get('<%=divCardTypes.ClientID %>');
            if (show)
                div.style.display = '';
            else
                div.style.display = 'none';
        }
	</script>
</asp:PlaceHolder>
<%--<asp:button id ="btnGoToDashBoard" text="Go to DashBoard" class="btnred" runat="server" style="margin-left: 35px;" />--%>
<div class="pckgltgraywrpr" style="margin:10px auto;width:950px;"> 
<div class="pckghdgred">Company Details</div>
	<div class="pckggraywrpr" style="width:450px;margin:10px;text-align:center;float:left;">
		<div class="pckghdgclr" style="height:10px;line-height:10px;"><div class="fieldreq" style="float:left;height:10px" id="bartxtServices" runat="server">&nbsp;</div><span id="labeltxtServices" runat="server" style="float:left;">Products and Services Offered</span></div>
		<asp:TextBox id="txtServices" runat="server" TextMode="MultiLine" rows="8" style="width:90%;margin-bottom:15px;"></asp:TextBox>
	</div>
	<div class="pckggraywrpr" style="width:450px;margin:10px;text-align:center;float:left;">
		<div class="pckghdgclr" style="height:10px;line-height:10px;"><div class="fieldreq" style="float:left;height:10px" id="bartxtDiscounts" runat="server">&nbsp;</div><span id="labeltxtDiscounts" runat="server" style="float:left;">Discounts</span></div>
		<asp:TextBox id="txtDiscounts" runat="server" TextMode="MultiLine" rows="8" style="width:90%;margin-bottom:15px;"></asp:TextBox>
	</div>
	<div class="pckggraywrpr" style="width:450px;margin:10px;text-align:center;float:left;">
		<div class="pckghdgclr" style="height:10px;line-height:10px;"><span id="labeltxtRebate" runat="server"  style="float:left;">Additional Incentive Rebate Program</span></div>
		<asp:TextBox id="txtRebate" runat="server" TextMode="MultiLine" rows="8" style="width:90%;margin-bottom:15px;"></asp:TextBox>
	</div>
	<div class="pckggraywrpr" style="width:450px;margin:10px;text-align:center;float:left;">
		<div class="pckghdgclr" style="height:10px;line-height:10px;"><div class="fieldreq" style="float:left;height:10px" id="bartxtTerms" runat="server">&nbsp;</div><span id="labeltxtTerms" runat="server" style="float:left;">Payment Terms</span></div>
		<asp:TextBox id="txtTerms" runat="server" TextMode="MultiLine" rows="8" style="width:90%;margin-bottom:15px;"></asp:TextBox>
	</div>
	<div class="pckggraywrpr" style="width:450px;margin:10px;text-align:center;float:left;">
		<div class="pckghdgclr" style="height:10px;line-height:10px;"><span id="labeltxtSpecialty" runat="server"  style="float:left;">Specialty Products and Services</span></div>
		<asp:TextBox id="txtSpecialty" runat="server" TextMode="MultiLine" rows="8" style="width:90%;margin-bottom:15px;"></asp:TextBox>
	</div>
	<div class="pckggraywrpr" style="width:450px;margin:10px;text-align:center;float:left;">
	    <div class="pckghdgclr" style="height:10px;line-height:10px;"><span id="labeltxtBrands" runat="server" style="float:left;">Manufacturers/Brands Supplied</span></div>
	    <asp:TextBox id="txtBrands" runat="server" TextMode="MultiLine" rows="8" style="width:90%;margin-bottom:15px;"></asp:TextBox>
	</div>
	<div class="pckggraywrpr" style="width:450px;margin:10px;text-align:center;float:left;">
		<div class="pckghdgclr" style="height:10px;line-height:10px;"><div class="fieldreq" style="float:left;height:10px">&nbsp;</div><span style="float:left;">Do you accept credit cards?</span></div>
		<b>
			<asp:RadioButton id="rbAcceptCardsYes" runat="server" onclick="ToggleCards(true);" GroupName="AcceptCards" text="Yes"></asp:RadioButton>
			<asp:RadioButton id="rbAcceptCardsNo" runat="server" onclick="ToggleCards(false);" GroupName="AcceptCards" text="No"></asp:RadioButton>
		</b>
		<div id="divCardTypes" runat="server">
			<b>If yes, which cards do you accept?</b>
			<asp:TextBox id="txtAcceptedCards" runat="server" TextMode="MultiLine" rows="4" style="width:90%;margin-bottom:15px;"></asp:TextBox>
		</div>
	</div>
	<div class="clear">&nbsp;</div>
</div>

<p align="center">
	<asp:Button id="btnDashboard" runat="server" text="Save Changes" cssclass="btnred" ValidationGroup="Registration" />
	<asp:Button id="btnBack" runat="server" text="Go Back" cssclass="btnred" onclientclick="history.go(-1);return false;" />
	<asp:Button id="btnContinue" runat="server" text="Continue" cssclass="btnred" ValidationGroup="Registration" />
</p>
<CC:RequiredFieldValidatorFront ID="rfvtxtServices" runat="server" ControlToValidate="txtServices" ErrorMessage="Field 'Services' is empty" ValidationGroup="Registration"></CC:RequiredFieldValidatorFront>
<CC:REquiredfieldvalidatorfront ID="rfvtxtDiscounts" runat="server" ControlToValidate="txtDiscounts" ErrorMessage="Field 'Discounts' is empty" ValidationGroup="Registration"></CC:REquiredfieldvalidatorfront>
<CC:RequiredFieldValidatorFront ID="rfvtxtTerms" runat="server" ControlToValidate="txtTerms" ErrorMessage="Field 'Payment Terms' is empty" ValidationGroup="Registration"></CC:RequiredFieldValidatorFront>

</CT:MasterPage>

