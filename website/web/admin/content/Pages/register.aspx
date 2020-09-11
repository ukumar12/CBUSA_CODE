<%@ Page ValidateRequest="false" Language="VB" MasterPageFile="~/controls/AdminMaster.master" AutoEventWireup="false" CodeFile="register.aspx.vb" Inherits="register" title="Register Existing page" %>

<asp:Content ID="Content" ContentPlaceHolderID="ph" Runat="Server">

<H4>Admin&nbsp;Content Tool - Register Existing page</H4>

If an ASPX page exists in the site and is not in the content management system, type in the URL below and click "Register Page".<br />
You'll be taken right to the edit page after it is added. The URL should be in the format: /news/default.aspx

<P>
<table>
<TR>
	<TD vAlign="top" colSpan="2"><font class="red">red color</font> - required fields</TD>
</TR>
<TR>
	<TD class="required" height="26"><STRONG>Page URL:</STRONG></TD>
	<TD class="field" height="26"><asp:textbox id="PageURL" runat="server" Width="300px"></asp:textbox></td>
	<td><asp:requiredfieldvalidator id="RequiredFieldValidator1" runat="server" ErrorMessage="Page URL cannot be blank" ControlToValidate="PageURL"></asp:requiredfieldvalidator></TD>
</TR>
<TR>
	<TD class="optional" height="26"><STRONG>Title:</STRONG></TD>
	<TD class="field" height="26"><asp:textbox id="PageTitle" runat="server" Width="300px"></asp:textbox></td>
	<td></TD>
</TR>
<TR>
	<TD class="required" height="26"><STRONG>Template:</STRONG></TD>
	<TD class="field" height="26"><asp:DropDownList ID=TemplateId runat=server></asp:DropDownList></td>
	<td><asp:requiredfieldvalidator id="Requiredfieldvalidator2" runat="server" ErrorMessage="Template cannot be blank" ControlToValidate="TemplateId"></asp:requiredfieldvalidator></TD>
</TR>
</TABLE>
</p>

<p>
<CC:OneClickButton id="btnSave" runat="server" CssClass="btn" Text="Register Page"></CC:OneClickButton>&nbsp;
<CC:OneClickButton id="btnCancel" runat="server" CssClass="btn" Text="Cancel" CausesValidation="False"></CC:OneClickButton>
</p>

</asp:Content>