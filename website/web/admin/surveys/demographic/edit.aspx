<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Survey_Question_Demographic_Edit"  Title="Survey Question Demographic"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If SurveyQuestionDemographicId = 0 Then %>Add<% Else %>Edit<% End If %> Survey Question Demographic</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan=2><span class=smallest><span class=red>red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Demographic Id:</td>
		<td class="field"><asp:DropDownList id="drpDemographicId" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvDemographicId" runat="server" Display="Dynamic" ControlToValidate="drpDemographicId" ErrorMessage="Field 'Demographic Id' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Display Text:</td>
		<td class="field"><asp:TextBox id="txtDisplayText" style="width: 349px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
		<td><asp:RequiredFieldValidator ID="rfvDisplayText" runat="server" Display="Dynamic" ControlToValidate="txtDisplayText" ErrorMessage="Field 'Display Text' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required"><b>Is Required?</b></td>
		<td class="field"><asp:CheckBox runat="server" ID="chkIsRequired" /></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Survey Question Demographic?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
