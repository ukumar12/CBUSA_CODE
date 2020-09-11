<%@ Page Language="VB" AutoEventWireup="false" CodeFile="email.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="admin_members_email" %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
<script type="text/javascript">
<!--     
	function RefreshPage() {
		var spanNewsletter = document.getElementById('spanNewsletter').style;
        var rbtnNewsletterYes = document.getElementById('<%=rbtnNewsletterYes.ClientId %>');
            
		if (rbtnNewsletterYes.checked) {
			spanNewsletter.display="block";
		} else {
			spanNewsletter.display="none";
		}
	}
	
	if (window.addEventListener) {
		window.addEventListener('load', RefreshPage, false);
	} else if (window.attachEvent) {
		window.attachEvent('onload', RefreshPage);
	}
//-->
</script>

<h4>E-Mail Preferences</h4>

Email Preferences for <asp:Label ID="txtMemberName" runat="server"></asp:Label>
| <a id="lnkBack" runat="server">&laquo; Go Back to Member Profile</a><br /><br />

 <span class=smallest><span class=red>red color</span>- indicates required field</span>
<table cellspacing="0" cellpadding="4">
<tr><td><b>Email Address:</b></td><td><asp:Literal id="ltlEmail" runat="server" /></td></tr>
</table>

<table summary="enewsletter">
<tr>
<td style="width:763px;"><b>Newsletter Registration</b></td>
</tr>
<tr valign="top">
<td style="width:763px;">
   	<div class="smaller" style="line-height:16px; margin:0 0 4px 10px;">
		Learn about sales and updates before anyone else. Sign up for our newsletter and receive periodic updates from us.
	</div>

	<div class="smaller" style="line-height:16px; margin:0 0 4px 10px;">
		<asp:radiobutton id="rbtnNewsletterYes" runat="server" GroupName="Newsletter" onclick="RefreshPage();" />
		<label for="<%=rbtnNewsletterYes.clientId %>">Yes, I would like to receive updates and special promotions via e-mail.</label>
	</div>

	<div class="smaller" style="line-height:16px; margin:0 0 4px 10px;">
		<asp:radiobutton id="rbtnNewsletterNo" runat="server" GroupName="Newsletter" onclick="RefreshPage();" />
		<label for="<%=rbtnNewsletterNo.clientId %>">No, I would not like to receive updates and special promotions via e-mail.</label>
	</div>

    <span id="spanNewsletter" style="display:block">
	<table>
	<tr>
	<td style="width:200px;">
		<table>
		<tr>
		<td>&nbsp;</td>
		<td class="smaller"><span class="red">E-mail Format</span></td>
		</tr>
		</table>

		<table summary="email format">
		<tr>
		<td><asp:radiobutton id="rbtnFormatHTML" runat="server" GroupName="NewsletterFormat" /><br /></td>
		<td class="smaller" style="padding-right:10px;"><label for="<%=rbtnFormatHTML.ClientId %>">HTML</label></td>
		<td><asp:radiobutton id="rbtnFormatTEXT" runat="server" GroupName="NewsletterFormat" /><br /></td>
		<td class="smaller"><label for="<%=rbtnFormatTEXT.ClientId %>">Plain Text</label></td>
		</tr>
		</table>

	</td>
	<td style="width:250px;">

		<table cellspacing="0" cellpadding="0" border="0" style="margin-left:5px;">
		<tr>
		<td>&nbsp;</td>
		<td class="smaller"><span id="labelchklstMailingLists" runat="server"><span class="red">Lists</span></span></td>
		</tr>
		</table>

		<table cellspacing="0" cellpadding="0" border="0" style="margin-top:4px; width: 250px;" summary="email format">
		<tr>
		<td width="100%">
		<CC:CheckboxlistEx id="chklstMailingLists" cellspacing="4" runat="server" TextAlign="right" RepeatDirection="Horizontal" RepeatColumns="3" />
		</td>
		</tr>
		</table>
		</td>

		<td style="width:350px;">
		</td>
	</tr>
	</table>
    </span> 
</td>
</tr>
</table>

<table style="width:763px; margin:10px 0 0 15px;"><tr><td>

<CC:OneClickButton CssClass="btn" id="btnSubmit" runat="server" text="Save" />&nbsp;
<CC:OneClickButton CssClass="btn" id="btnCancel" runat="server" text="Cancel" />

</td></tr></table>

<CC:CustomValidatorFront id="cvrbtnNewsletterYes" runat="server" Display="none" ControlToValidate="rbtnNewsletterYes" ErrorMessage="You must indicate if you wish to receive the newsletter" /> 
<CC:RequiredCheckboxListValidatorFront id="cvrbtnNewsletterYesAtLeastOne" runat="server" Display="none" ControlToValidate="chklstMailingLists" ErrorMessage="You must specify at least one mailing list if you are subscribing to the newsletter" /> 


</asp:content>

