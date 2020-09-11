<%@ Page Language="VB" AutoEventWireup="false" ValidateRequest="false" MasterPageFile="~/controls/AdminMasterNotes.master" Title="Send Status" CodeFile="SendStatus.aspx.vb" Inherits="SendStatus"  %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<table cellpadding="0" cellspacing="2" border="0">
    <tr>
        <td class="required">From Name:</td>
        <td class="field"><asp:TextBox  id="txtFromName" runat="server" Width="450"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="required">From Email:</td>
        <td class="field"><asp:TextBox  id="txtFromEmail" runat="server" Width="450"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="required">Recipient Name:</td>
        <td class="field"><asp:TextBox  id="txtRecipientName" runat="server" Width="450"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="required">Recipient Email:</td>
        <td class="field"><asp:TextBox  id="txtRecipientEmail" runat="server" Width="450"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="optional">BCC:</td>
        <td class="field"><asp:TextBox  id="txtBCC" runat="server" Width="450"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="required">Subject:</td>
        <td class="field"><asp:TextBox  id="txtSubject" runat="server" Width="450"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="optional">Extra Message:</td>
        <td class="field"><CC:CKEditor ID="txtMessage" runat="server" Width="750" Height="400" ToolbarSet="Basic"></CC:CKEditor></td>
    </tr>
    <tr>
        <td></td>
        <td><CC:OneClickButton ID="btnSend" runat="server" Text="Send" CssClass="btn" />
            <input type="button" onclick="javascript: parent.closeSend();" class="btn" value="Cancel" />
            
        </td></tr>
</table>

<CC:RequiredFieldValidatorFront ID="rfvtxtFromName" ControlToValidate="txtFromName" ErrorMessage="From name is required." runat="server" EnableClientScript="false" Enabled="true" Display="Dynamic" ></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtFromEmail" ControlToValidate="txtFromEmail" ErrorMessage="From email is required." runat="server" EnableClientScript="false" Enabled="true" Display="Dynamic" ></CC:RequiredFieldValidatorFront>
<CC:EmailValidatorFront ID="evtxtFromEmail" ControlToValidate="txtFromEmail" ErrorMessage="From email is invalid." runat="server" EnableClientScript="false" Enabled="true"></CC:EmailValidatorFront>

<CC:RequiredFieldValidatorFront ID="rfvRecipientName" ControlToValidate="txtRecipientName" ErrorMessage="Recipient name is required." runat="server" EnableClientScript="false" Enabled="true" Display="Dynamic" ></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvRecipientEmail" ControlToValidate="txtRecipientEmail" ErrorMessage="Recipient email is required." runat="server" EnableClientScript="false" Enabled="true" Display="Dynamic" ></CC:RequiredFieldValidatorFront>

<CC:EmailValidatorFront ID="evtxtRecipientEmail" ControlToValidate="txtRecipientEmail" ErrorMessage="Recipient email is invalid." runat="server" EnableClientScript="false" Enabled="true"></CC:EmailValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvtxtSubject" ControlToValidate="txtSubject" ErrorMessage="Subject is required." runat="server" EnableClientScript="false" Enabled="true" Display="Dynamic" ></CC:RequiredFieldValidatorFront>

</asp:content>

