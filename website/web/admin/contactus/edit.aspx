<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Contact Us"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If ContactUsId = 0 Then %>Add<% Else %>Edit<% End If %> Contact Us</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Full Name:</td>
		<td class="field"><asp:textbox id="txtFullName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvFullName" runat="server" Display="Dynamic" ControlToValidate="txtFullName" ErrorMessage="Field 'Full Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Email:</td>
		<td class="field"><asp:textbox id="txtEmail" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvEmail" runat="server" Display="Dynamic" ControlToValidate="txtEmail" ErrorMessage="Field 'Email' is blank"></asp:RequiredFieldValidator><CC:EmailValidator Display="Dynamic" runat="server" id="fvEmail" ControlToValidate="txtEmail" ErrorMessage="Field 'Email' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Order Number:</td>
		<td class="field"><asp:textbox id="txtOrderNumber" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Phone:</td>
		<td class="field"><asp:textbox id="txtPhone" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">Question:</td>
		<td class="field"><asp:DropDownList id="drpQuestionId" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvQuestionId" runat="server" Display="Dynamic" ControlToValidate="drpQuestionId" ErrorMessage="Field 'Question' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Your Message:</td>
		<td class="field"><asp:TextBox id="txtYourMessage" style="width: 349px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
		<td><asp:RequiredFieldValidator ID="rfvYourMessage" runat="server" Display="Dynamic" ControlToValidate="txtYourMessage" ErrorMessage="Field 'Your Message' is blank"></asp:RequiredFieldValidator></td>
	</tr>
</table>


<asp:ScriptManager runat="server" ID="sm" EnablePartialRendering="true" />

<asp:UpdatePanel runat="server" ID="up1">
<Triggers>
<asp:PostBackTrigger ControlID="btnSave" />
<asp:PostBackTrigger ControlID="btnDelete" />
<asp:PostBackTrigger ControlID="btnCancel" />
</Triggers>
<ContentTemplate>

	<asp:PlaceHolder runat="server" ID="phReplies">
	<p></p>

	<asp:Panel runat="server" ID="pnlReply">
	<table border="0" cellspacing="1" cellpadding="2">
		<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
		<tr>
			<td class="required">To Email:</td>
			<td class="field"><%=dbContactUs.Email%></td>
			<td></td>
		</tr>
		<tr>
			<td class="required">From Name:</td>
			<td class="field"><asp:textbox id="txtFromName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
			<td><asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" Display="Dynamic" ControlToValidate="txtFromName" ErrorMessage="Field 'From Name' is blank"></asp:RequiredFieldValidator></td>
		</tr>
		<tr>
			<td class="required">From Email:</td>
			<td class="field"><asp:textbox id="txtFromEmail" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
			<td><asp:RequiredFieldValidator runat="server" Display="Dynamic" ControlToValidate="txtFromEmail" ErrorMessage="Field 'From Email' is blank"></asp:RequiredFieldValidator><CC:EmailValidator Display="Dynamic" runat="server" id="EmailValidator1" ControlToValidate="txtFromEmail" ErrorMessage="Field 'Email' is invalid" /></td>
		</tr>
		<tr>
			<td class="required">Subject:</td>
			<td class="field"><asp:textbox id="txtSubject" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
			<td><asp:RequiredFieldValidator runat="server" Display="Dynamic" ControlToValidate="txtSubject" ErrorMessage="Field 'Subject' is blank"></asp:RequiredFieldValidator></td>
		</tr>
		<tr>
			<td class="required">Message:</td>
			<td class="field"><asp:TextBox id="txtMessage" style="width: 349px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
			<td><asp:RequiredFieldValidator runat="server" Display="Dynamic" ControlToValidate="txtMessage" ErrorMessage="Field 'Your Message' is blank"></asp:RequiredFieldValidator></td>
		</tr>
	</table>
	</asp:Panel>

	<CC:OneClickButton runat="server" ID="btnReply" Text="Reply" CssClass="btn" />
	<CC:OneClickButton runat="server" ID="btnSend" Text="Send Reply" CssClass="btn" />
	<CC:OneClickButton runat="server" ID="btnCancelReply" Text="Cancel" CssClass="btn" CausesValidation="false" />

	<p></p>
	<CC:GridView runat="server" ID="gvReply" EmptyDataText="There are no replies to this contact us submission." AutoGenerateColumns="false" CellSpacing="2" CellPadding="2" BorderWidth="0">
	<Columns>
		<asp:BoundField DataField="FullName" HeaderText="From Name" />
		<asp:BoundField DataField="Email" HeaderText="From Email" />
		<asp:BoundField DataField="Subject" HeaderText="Subject" />
		<asp:BoundField DataField="Message" HeaderText="Message" />
		<asp:BoundField DataField="AdminName" HeaderText="Reply Admin" />
		<asp:BoundField DataField="CreateDate" HeaderText="Reply Date" />
	</Columns>
	</CC:GridView>


	</asp:PlaceHolder>

	<p></p>
	<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
	<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Contact Us?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
	<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</ContentTemplate></asp:UpdatePanel>

</asp:content>

