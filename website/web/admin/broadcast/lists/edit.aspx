<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Mailing List"%>
	
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If ListId = 0 Then %>Add<% Else %>Edit<% End If %> Mailing List</h4>

<% If ListId <> 0 Then %>
<i>You have already uploaded members to this list.<br>
If you upload a new file, all emails will be merged with already existing members
</i>
<p></p>
<% end if %>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Name:</td>
		<td class="field"><asp:textbox id="txtName" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvName" runat="server" Display="Dynamic" ControlToValidate="txtName" ErrorMessage="Field 'Name' is blank"></asp:RequiredFieldValidator></td>
		</td>
	</tr>
	<tr valign="top">
		<td valign="top" class="field" colspan="2">
		The filename must contain an extension <b>.txt</b> or <b>.csv</b> and must be in the standard comma delimited format<br />

		<p>
		File structure is as follows:<br/>
		<b>Member Name</b>,<b><span style="color:red;">Email Address</span></b>,<b>Email Format</b>, where email format must be: HTML or TEXT
		</p>
		</td>
	</tr>
	<tr>
		<td class="required">Filename:</td>
		<td class="field"><CC:FileUpload ID="fuFilename" Folder="/assets/broadcast/lists/" runat="server" Required="true" /></td>
		<td><CC:RequiredFileUploadValidator Display="dynamic" ID="rfuvfileName" ControlToValidate = "fuFilename" runat="server" ErrorMessage="File is required"></CC:RequiredFileUploadValidator>
			<CC:FileUploadExtensionValidator Extensions="txt,csv" ID="vExtFileName" runat="server" Display="Dynamic" ControlToValidate="fuFilename" ErrorMessage="Invalid file extension"></CC:FileUploadExtensionValidator>
		</td>	
	</tr>
		<tr>
		<td class="required">Status:</td>
		<td class="field"><asp:DropDownList ID="drpStatus" runat="server">
            <asp:ListItem>Active</asp:ListItem>
            <asp:ListItem>Disabled</asp:ListItem>
        </asp:DropDownList></td>
		<td>
		<asp:RequiredFieldValidator ID="rfvStatus" runat="server" Display="Dynamic" ControlToValidate="drpStatus" ErrorMessage="Field 'Status' is blank"></asp:RequiredFieldValidator></td>
	</tr>
</table>

<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Mailing List?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
