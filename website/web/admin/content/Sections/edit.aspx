<%@ Page ValidateRequest="false" Language="VB" MasterPageFile="~/controls/AdminMaster.master" AutoEventWireup="false" CodeFile="edit.aspx.vb" Inherits="edit" title="Content Tool Template" %>

<asp:Content ID="Content" ContentPlaceHolderID="ph" Runat="Server">

<H4>Admin&nbsp;Content Tool - Register New Website Section</H4>

If an section exists in the site and is not in the content management system as a section, type in the URL and name for the section below and click "Register Section". The URL should be in the format: /news/ (be sure to include the trailing slash). You may also select a default template for all files (except those specified otherwise) to use. 

<P>
<table>
<TR>
	<TD vAlign="top" colSpan="2"><font class="red">red color</font> - required fields</TD>
</TR>
<TR>
	<TD class="required" height="26"><STRONG>Section Name:</STRONG></TD>
	<TD class="field" height="26"><asp:textbox id="SectionName" runat="server" Width="300px"></asp:textbox></td>
	<td><asp:requiredfieldvalidator id="RequiredFieldValidator1" runat="server" ErrorMessage="Section Name cannot be blank" ControlToValidate="SectionName"></asp:requiredfieldvalidator></TD>
</TR>
<TR>
	<TD class="required" height="26"><STRONG>Folder Name:</STRONG></TD>
	<TD class="field" height="26"><asp:textbox id="Folder" runat="server" Width="300px"></asp:textbox></td>
	<td><asp:requiredfieldvalidator id="Requiredfieldvalidator2" runat="server" ErrorMessage="Section Folder cannot be blank" ControlToValidate="Folder"></asp:requiredfieldvalidator></TD>
</TR>
</TABLE>
</p>

<p>
<CC:OneClickButton id="btnSave" runat="server" CssClass="btn" Text="Register Section"></CC:OneClickButton>&nbsp;
<CC:OneClickButton id="btnCancel" runat="server" CssClass="btn" Text="Cancel" CausesValidation="False"></CC:OneClickButton>
</p>

</asp:Content>