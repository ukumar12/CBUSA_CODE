<%@ Page Language="VB" ValidateRequest="False" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Store Brand"%>
<%@ Register TagPrefix="CC" TagName="HelpMessage" Src="~/controls/HelpMessage.ascx" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If BrandId = 0 Then %>Add<% Else %>Edit<% End If %> Store Brand</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan=2><span class=smallest><span class=red>red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Name:</td>
		<td class="field"><asp:textbox id="txtName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvName" runat="server" Display="Dynamic" ControlToValidate="txtName" ErrorMessage="Field 'Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="optional">Page Title:</td>
		<td class="field"><asp:textbox id="txtPageTitle" runat="server" maxlength="255" style="width: 600px;"></asp:textbox><br />
    	<div class="smaller" style="margin-top:2px">Descriptive <b>Page Titles</b> can help your store's item pages rank higher in search engines like <b style="border:solid 1px #666666;background:#FFFFFF">&nbsp;<b style="color:#1111FF">G</b><b style="color:#FF1111">o</b><b style="color:#DDBB00">o</b><b style="color:#1111FF">g</b><b style="color:#00AD00">l</b><b style="color:#FF1111">e</b>&nbsp;</b> and <b style="border:solid 1px #666666;background:#FFFFFF">&nbsp;<b style="color:#FF0000">Yahoo!</b>&nbsp;</b></div>
		<CC:HelpMessage runat="server" HelpCode="TitleTag" ID="hlpTitle"/></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Meta Description:</td>
		<td class="field"><asp:textbox TextMode="MultiLine" Rows="3" id="txtMetaDescription" runat="server" maxlength="255" columns="50" style="width: 600px;"></asp:textbox><br />
    	<div class="smaller" style="margin-top:2px">Meta Tag Description may help your store's item pages rank higher in search engines like <b style="border:solid 1px #666666;background:#FFFFFF">&nbsp;<b style="color:#1111FF">G</b><b style="color:#FF1111">o</b><b style="color:#DDBB00">o</b><b style="color:#1111FF">g</b><b style="color:#00AD00">l</b><b style="color:#FF1111">e</b>&nbsp;</b> and <b style="border:solid 1px #666666;background:#FFFFFF">&nbsp;<b style="color:#FF0000">Yahoo!</b>&nbsp;</b></div>
    	<CC:HelpMessage runat="server" HelpCode="MetaDescription" ID="hlpMetaDescription"/>
		</td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Meta Keywords:</td>
		<td class="field"><asp:textbox TextMode="MultiLine" Rows="3" id="txtMetaKeywords" runat="server" maxlength="255" columns="50" style="width: 600px;"></asp:textbox><br />
    	<div class="smaller" style="margin-top:2px">Meta Tag Keywords may can help your store's item pages rank higher in search engines like <b style="border:solid 1px #666666;background:#FFFFFF">&nbsp;<b style="color:#1111FF">G</b><b style="color:#FF1111">o</b><b style="color:#DDBB00">o</b><b style="color:#1111FF">g</b><b style="color:#00AD00">l</b><b style="color:#FF1111">e</b>&nbsp;</b> and <b style="border:solid 1px #666666;background:#FFFFFF">&nbsp;<b style="color:#FF0000">Yahoo!</b>&nbsp;</b></div>
    	<CC:HelpMessage runat="server" HelpCode="MetaKeyWords" ID="hlpMetaKeyWords"/></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Custom URL:</td>
		<td class="field"><asp:Textbox id="txtCustomURL" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:TextBox><br /><span class="smaller">Please begin url with "/" i.e. "/sample-url.aspx"</span><br />
		<CC:HelpMessage runat="server" HelpCode="CustomUrl" ID="hlpCustomUrl"/></td>
		<td></td>
		
	</tr>		
	<tr>
		<td class="optional">Description:</td>
		<td class="field"><CC:CKEditor id="txtDescription" runat="server" Width="600" Height="300" /></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Image:</td>
		<td class="field"><CC:FileUpload ID="fuImage" Folder="/assets/brand" ImageDisplayFolder="/assets/brand" DisplayImage="True" runat="server" style="width: 319px;" /></td>
		<td><CC:FileUploadExtensionValidator Extensions="jpg,jpeg,gif,bmp,png" ID="feImage" runat="server" Display="Dynamic" ControlToValidate="fuImage" ErrorMessage="Invalid file extension"></CC:FileUploadExtensionValidator></td>
	</tr>
	<tr>
		<td class="optional">Logo:</td>
		<td class="field"><CC:FileUpload ID="fuLogo" Folder="/assets/brand/original" ImageDisplayFolder="/assets/brand/thumbnail" DisplayImage="True" runat="server" style="width: 319px;" /></td>
		<td><CC:FileUploadExtensionValidator Extensions="jpg,jpeg,gif,bmp,png" ID="feLogo" runat="server" Display="Dynamic" ControlToValidate="fuLogo" ErrorMessage="Invalid file extension"></CC:FileUploadExtensionValidator></td>
	</tr>
	<tr>
		<td class="required"><b>Is Active?</b></td>
		<td class="field"><asp:CheckBox runat="server" ID="chkIsActive" /></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Store Brand?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

