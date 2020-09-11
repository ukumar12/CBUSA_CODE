<%@ Page Language="VB" AutoEventWireup="false" CodeFile="account.aspx.vb" Inherits="Account" %>

<CT:MasterPage ID="CTMain" runat="server">
<asp:PlaceHolder ID="PlaceHolder1" runat="server">
<script type="text/javascript">

	function OpenEditAccountForm(vprod) {
		var c = $get('<%=frmEditAccount.ClientID %>').control;
	}
</script>
</asp:PlaceHolder>
<div class="cblock10">

	<p>Welcome back,<br />
	<strong><asp:Literal ID="ltdUserName" runat="server" /></strong></p>

	<h3><asp:Literal ID="ltdCompanyName" runat="server" /></h3>

	<p><div class="aiaddresstext" id="divAddress" runat="server" /></p>
	
		<ul class="lnav">
			<asp:Repeater id="rptRoles" runat="server">
				<HeaderTemplate>
				</HeaderTemplate>
				<ItemTemplate>
					<li>&gt; <a href="<%#DataBinder.Eval(Container.DataItem, "Role")%>"><%#DataBinder.Eval(Container.DataItem, "Role")%></a></li>
				</ItemTemplate>
				<FooterTemplate>
				</FooterTemplate>
			</asp:Repeater>	
		</ul>
	<div class="dvdr">&nbsp;</div>

</div>

<asp:Button id="btntemp" text="test" class="btn" runat="server" causesvalidation="false" />
<CC:PopupForm ID="frmEditAccount" runat="server" ShowVeil="true" ValidateCallback="true" ErrorPlaceholderId="spanErrors" VeilCloses="true" style="width:650px;height:650px;" CssClass="window" OpenTriggerId="btntemp" CloseTriggerId="btnCancel">
	<FormTemplate>

			<h3 >Edit Account Information</h3>

				<table>
					<tr>
						<td style="padding:0px;margin:0px;" colspan="3"><span id="spanErrors" runat="server"><img src="/images/spacer.gif" alt="" style="height:1px;" /></span></td>
					</tr>
					<tr>
						<td class="required">Company Name:</td>
						<td class="field"><asp:textbox id="txtCompanyName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
						<td><asp:RequiredFieldValidator ID="rfvCompanyName" runat="server" Display="Dynamic" ControlToValidate="txtCompanyName" ErrorMessage="Field 'Company Name' is blank"></asp:RequiredFieldValidator></td>
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
						<td class="required">Email:</td>
						<td class="field"><asp:textbox id="txtEmail" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
						<td><asp:RequiredFieldValidator ID="rfvEmail" runat="server" Display="Dynamic" ControlToValidate="txtEmail" ErrorMessage="Field 'Email' is blank"></asp:RequiredFieldValidator><CC:EmailValidator Display="Dynamic" runat="server" id="fvEmail" ControlToValidate="txtEmail" ErrorMessage="Field 'Email' is invalid" /></td>
					</tr>
					<tr>
						<td class="optional">Website URL:</td>
						<td class="field"><asp:textbox id="txtWebsiteURL" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox><br/><span class="smaller">http:// or https:// are required</span></td>
						<td><CC:URLValidator Display="Dynamic" runat="server" id="lnkvWebsiteURL" ControlToValidate="txtWebsiteURL" ErrorMessage="Link 'Website URL' is invalid" /></td>
					</tr>
				</table>
				<p></p>
				<asp:Button id="btnSave" runat="server" cssclass="btn" text="Save" />
				<asp:Button id="btnCancel" runat="server" cssclass="btn" text="Close" OnClientClick="return false;"/>

	</FormTemplate>
</CC:PopupForm>


</CT:MasterPage>