<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Billing Plan"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If BillingPlanId = 0 Then %>Add<% Else %>Edit<% End If %> Billing Plan</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Display Value:</td>
		<td class="field"><asp:textbox id="txtBillingPlan" runat="server" maxlength="250" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvBillingPlan" runat="server" Display="Dynamic" ControlToValidate="txtBillingPlan" ErrorMessage="Field 'Display Value' is blank"></asp:RequiredFieldValidator></td>
	</tr>
    <tr>
		<td class="optional">Sort Value:</td>
		<td class="field"><asp:TextBox id="txtSortValue" style="width: 319px;" runat="server" ToolTip="Example: 01, 02, etc..."></asp:TextBox></td>
		<td></td>
	</tr>
    <tr>
		<td class="optional">Billing Plan Type:</td>
		<td class="field">
            <asp:RadioButtonList runat="server" ID="rblBillingPlanType" RepeatDirection="Horizontal" AutoPostBack="true">
			    <asp:ListItem Text="Registration" Value="1" />
			    <asp:ListItem Text="Subscription" Value="2" Selected="True" />
			</asp:RadioButtonList>
		</td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Description:</td>
		<td class="field"><asp:TextBox id="txtDescription" style="width: 319px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
		<td></td>
	</tr>
    <tr>
		<td class="required">External System:</td>
		<td class="field"><asp:textbox id="txtExternalSystem" runat="server" maxlength="100" columns="50" style="width: 319px;" Text="Vindicia" ReadOnly="true"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvExternalSystem" runat="server" Display="Dynamic" ControlToValidate="txtExternalSystem" ErrorMessage="Field 'External System' is blank"></asp:RequiredFieldValidator></td>
	</tr>
    <tr>
		<td class="required">External Key:</td>
		<td class="field">
            <asp:DropDownList ID="ddlBillingPlans" runat="server" style="width: 319px;"></asp:DropDownList>
		</td>
		<td>
            <asp:RequiredFieldValidator ID="rfvExternalKey" runat="server" Display="Dynamic" ControlToValidate="ddlBillingPlans" ErrorMessage="Field 'External Key' is blank"></asp:RequiredFieldValidator>
		</td>
	</tr>
    <tr>
		<td class="optional">Is Default:</td>
		<td class="field">
            <asp:RadioButtonList runat="server" ID="rblIsDefault" RepeatDirection="Horizontal">
			    <asp:ListItem Text="Yes" Value="True" />
			    <asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
		<td></td>
	</tr>
	<tr>
		<td class="required"><b>Record State</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblRecordState" RepeatDirection="Horizontal">
			    <asp:ListItem Text="Active" Value="1" Selected="True" />
			    <asp:ListItem Text="Inactive" Value="0" />
                <%--<asp:ListItem Text="Deleted" Value="2" />--%>
			</asp:RadioButtonList>
		</td>
	</tr>
</table>

<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Members are currently associated with this plan. If you click OK, these users will be transferred to the default billing plan." Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>