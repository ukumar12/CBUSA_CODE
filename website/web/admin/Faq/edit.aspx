<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="FAQ" ValidateRequest="false"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If FaqId = 0 Then %>Add<% Else %>Edit<% End If %> FAQ</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan=2><span class=smallest><span class=red>red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">FAQ Category:</td>
		<td class="field"><asp:DropDownList id="drpFaqCategoryId" runat="server" /></td>
		<td><asp:RequiredFieldValidator ID="rfvFaqCategoryId" runat="server" Display="Dynamic" ControlToValidate="drpFAQCategoryId" ErrorMessage="Field 'FAQ Category' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Question:</td>
		<td class="field"><asp:textbox id="txtQuestion" runat="server" maxlength="400" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvQuestion" runat="server" Display="Dynamic" ControlToValidate="txtQuestion" ErrorMessage="Field 'Question' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Answer:<div class="smaller" style="font-weight:normal;">Required for active FAQ's</div></td>
		<td class="field"><CC:CKeditor id="txtAnswer" runat="server" Width="600" Height="300" /></td>
		<td></td>
	</tr>
	<tr>
		<td class="required"><b>Is Active?</b></td>
		<td class="field"><asp:CheckBox runat="server" ID="chkIsActive" /></td>
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
			<td class="field"><%=dbFaq.Email%></td>
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
			<td><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" ControlToValidate="txtFromEmail" ErrorMessage="Field 'From Email' is blank"></asp:RequiredFieldValidator><CC:EmailValidator Display="Dynamic" runat="server" id="EmailValidator1" ControlToValidate="txtFromEmail" ErrorMessage="Field 'Email' is invalid" /></td>
		</tr>
		<tr>
			<td class="required">Subject:</td>
			<td class="field"><asp:textbox id="txtSubject" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
			<td><asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Display="Dynamic" ControlToValidate="txtSubject" ErrorMessage="Field 'Subject' is blank"></asp:RequiredFieldValidator></td>
		</tr>
		<tr>
			<td class="required">Message:</td>
			<td class="field"><asp:TextBox id="txtMessage" style="width: 349px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox></td>
			<td><asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" Display="Dynamic" ControlToValidate="txtMessage" ErrorMessage="Field 'Your Message' is blank"></asp:RequiredFieldValidator></td>
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
