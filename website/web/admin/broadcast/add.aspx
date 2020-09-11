<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Mailing Message" CodeFile="add.aspx.vb" Inherits="add"  %>
<%@ Register TagPrefix="CC" TagName="MailingLists" Src="~/controls/MailingLists.ascx" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Create Broadcast E-Mail</h4>

<asp:CustomValidator Display="none" EnableClientScript="false" ID="valCustom" runat="server" ErrorMessage="Please select a template or a list to copy from"></asp:CustomValidator>
<asp:CustomValidator Display="none" EnableClientScript="false" ID="valTarget" runat="server" ErrorMessage="Please select the type of email recipients"></asp:CustomValidator>

How would you like to create a new e-mail
<table border="0" cellpadding="2" cellspacing="3">
<tr valign="top"><td>1.</td><td><b>Copy from existing e-mail</b>
<div><asp:DropDownList ID="drpPastNewsletters" runat="server"></asp:DropDownList></div>
</td></tr>
<tr><td>&nbsp;</td></tr>
<tr valign="top"><td>2.</td><td><b>Create a brand new e-mail</b>

<asp:DataList id="dlTemplates" Runat="server" RepeatColumns="4" CellPadding="2" CellSpacing="2" RepeatDirection="Horizontal">
	<ItemTemplate>  
		<CC:RadioButtonEx id="rbTemplate" GroupName="Template" value='<%#Container.DataItem("TemplateId") %>' runat="server"/>
		<span class="bold"><%#DataBinder.Eval(Container.DataItem, "Name")%></span>
		<div style="padding-left:25px">
		<img alt="" src="/assets/broadcast/templates/<%#DataBinder.Eval(Container.DataItem, "ImageName")%>" />
		</div>
	</ItemTemplate>
</asp:DataList>

<div class="smaller">Because the unsubscribe options differ for subscribers and dynamically uploaded
e-mail addresses, please specify below who will be the recipients for this broadcast.</div>

<p></p>
<strong>This e-mail will be sent to:</strong>

<table border="0" cellpadding="3" cellspacing="2">
<tr valign="top">
	<th><asp:RadioButton id="rbTargetTypeMemeber" GroupName="Target" runat="server" /></th>
	<td>
		<span class="bold underline">Permanent List (You may send to 1 or all of these lists per email)</span><br />
		<CC:MailingLists ID="PermanentLists" runat="Server" IsPermanent="true" />
	</td>
</tr>
<tr valign="top">
	<th><asp:RadioButton id="rbTargetTypeDynamic" GroupName="Target" runat="server" /></th>
	<td>
		<span class="bold underline">Uploaded List (You may send to 1 or all of these lists per email)</span><br />
		<CC:MailingLists ID="DynamicLists" runat="Server" IsPermanent="false" />
	</td>
</tr>
</table>

</td></tr>
</table>

<p></p>
<CC:OneClickButton id="btnContinue" Runat="server" Text="Continue &raquo;" cssClass="btn"></CC:OneClickButton>

</asp:content>

