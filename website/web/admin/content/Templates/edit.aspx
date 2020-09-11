<%@ Page ValidateRequest="false" Language="VB" MasterPageFile="~/controls/AdminMaster.master" AutoEventWireup="false" CodeFile="edit.aspx.vb" Inherits="edit" title="Content Tool Template" %>

<asp:Content ID="Content" ContentPlaceHolderID="ph" Runat="Server">

<H4>Content Tool - Add / Edit Template</H4>

<TABLE>
	<TR>
		<TD vAlign="top" colSpan="3"><font class="red">red color</font> - 
			required fields</TD>
	</TR>
	<TR>
		<TD class="required" height="26"><STRONG>Template Name:</STRONG></TD>
		<TD class="field" height="26">
			<asp:textbox id="TemplateName" runat="server" Width="600px"></asp:textbox>
		</td><td>	
			<asp:requiredfieldvalidator id="RequiredFieldValidator1" runat="server" ErrorMessage="Template Name cannot be blank"
				ControlToValidate="TemplateName"></asp:requiredfieldvalidator></TD>
	</TR>
	<TR>
		<TD class="required" height="26"><STRONG>Template:</STRONG></TD>
		<TD class="field" height="26">
			<asp:textbox id="Content" TextMode="MultiLine" rows="40" runat="server" Width="800px"></asp:textbox>
		</td><td>	
			<asp:requiredfieldvalidator id="Requiredfieldvalidator2" runat="server" ErrorMessage="Template Content cannot be blank"
				ControlToValidate="Content"></asp:requiredfieldvalidator></TD>
	</TR>
	<TR>
		<TD class="required" height="26"><STRONG>Print Template:</STRONG></TD>
		<TD class="field" height="26">
			<asp:textbox id="Print" TextMode="MultiLine" rows="40" runat="server" Width="800px"></asp:textbox>
		</td>
		<td></TD>
	</TR>
</TABLE>

<p>
<CC:OneClickButton id="btnSave" runat="server" CssClass="btn" Text="Save"></CC:OneClickButton>&nbsp;
<CC:OneClickButton id="btnCancel" runat="server" CssClass="btn" Text="Cancel" CausesValidation="False"></CC:OneClickButton></P>
</p>

</asp:Content>