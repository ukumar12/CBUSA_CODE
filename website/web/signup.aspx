<%@ Page Language="vb" AutoEventWireup="false" Inherits="Signup" CodeFile="signup.aspx.vb" %>
<CT:masterpage runat="server" id="CTMain" DefaultButton="btnSignup">

    <h2 class="hdng">Newsletter Registration</h2>
   
    <div id="divMain" runat="server">
    <table cellspacing="0" cellpadding="0" border="0" style="margin-top:10px;">
    <tr>
    <td class="fieldreq" style="padding-bottom:6px;">&nbsp;</td>
    <td class="smaller" style="padding:0 0 6px 4px;">Indicates required field</td>
    </tr>
    </table>

    <table border="0" cellspacing="2" cellpadding="0" style="margin-top:10px;">
	    <tr valign="top">
	        <td class="field" align="left" id="labeltxtEmail" runat="server"><b>Email</b></td>
		    <td class="fieldreq" id="bartxtEmail" runat="server">&nbsp;</td>
		    <td valign="top" class="field"><asp:Textbox id="txtEmail" Columns="50" MaxLength="255" runat="server"></asp:Textbox></td>
		    <CC:RequiredFieldValidatorFront id="rfvEmail" Display="Dynamic" ErrorMessage="Email field is blank" runat="server" ControlToValidate="txtEmail"></CC:requiredfieldvalidatorFront>
		    <CC:EmailValidatorFront ID="evalEmail" Display="Dynamic" ErrorMessage="Email address is invalid" runat="server" ControlToValidate="txtEmail"></CC:EmailValidatorFront>
	    </tr>
	    <tr>
	        <td class="field" align="left" id="labeltxtName" runat="server"><b>Name</b></td>
		    <td class="fieldnorm" id="bartxtName" runat="server">&nbsp;</td>
		    <td valign="top" class="field"><asp:Textbox id="txtName" Columns="50" MaxLength="255" runat="server"></asp:Textbox></td>
	    </tr>
	    <tr>
	        <td class="field" align="left" id="labelrblMimeType" runat="server"><b>E-mail format</b></td>
		    <td></td>
		    <td class="field"><asp:RadioButtonList id="rblMimeType" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem Value="HTML" Selected="True">HTML</asp:ListItem>
                <asp:ListItem Value="TEXT">Plain Text</asp:ListItem>
            </asp:RadioButtonList></td>
		    <CC:RequiredFieldValidatorFront ID="rfvMimeType" runat="server" Display="Dynamic" ControlToValidate="rblMimeType" ErrorMessage="E-mail format is blank"></CC:RequiredFieldValidatorFront>
	    </tr>
	    <tr valign="top">
		    <td class="field"><b>E-mail lists</b></td>
	        <td></td>
		    <td class="field"><CC:CheckBoxListEx ID="cblLists" runat="server" RepeatColumns="2"></CC:CheckBoxListEx></td>
	    </tr>
    </table>

    <div style="margin-top:10px">
      <CC:OneClickButton  id="btnSignup" Runat="server" Text="Sign Up Now!" CssClass="btn" />
      <input type="button" class="btn" value="Cancel" onclick="document.location.href='default.aspx'" />
    </div>
    </div>

    <div id="divConfirm" runat="server" visible="false">
    <p>The subscription for e-mail <b><asp:Literal runat="Server" id="ltlEmail"/></b> has been successfully processed.</p>
    <p>Thank you for joining the newsletter. You will be hearing from us shortly!</p>
    </div>

</CT:masterpage>

