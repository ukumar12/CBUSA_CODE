<%@ Page ValidateRequest="False" Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="" CodeFile="edit.aspx.vb" Inherits="edit"  %>
<%@ Register TagPrefix="CC" TagName="HelpMessage" Src="~/controls/HelpMessage.ascx" %>

<asp:content ContentPlaceHolderId="ph" ID="Content1" runat="server">
<h4>Departments - Edit Department Details</h4>
<table border="0" cellspacing="1" cellpadding="3">
	<tr>
		<td colspan="2"><span class="red">red color</span> - required fields</td>
	</tr>
	<tr valign="top">
		<td valign="top" class="required"><b>Name:</b></td>
		<td valign="top" class="field"><asp:Label runat="server" id="Name" /></td>
	</tr>
	<tr>
		<td class="optional">Page Title:</td>
		<td class="field"><asp:textbox id="txtPageTitle" runat="server" maxlength="255" style="width: 600px;"></asp:textbox><br />
    	<div class="smaller" style="margin-top:2px">Descriptive <b>Page Titles</b> can help your store's item pages rank higher in search engines like <b style="border:solid 1px #666666;background:#FFFFFF">&nbsp;<b style="color:#1111FF">G</b><b style="color:#FF1111">o</b><b style="color:#DDBB00">o</b><b style="color:#1111FF">g</b><b style="color:#00AD00">l</b><b style="color:#FF1111">e</b>&nbsp;</b> and <b style="border:solid 1px #666666;background:#FFFFFF">&nbsp;<b style="color:#FF0000">Yahoo!</b>&nbsp;</b></div>
		<CC:HelpMessage runat="server" HelpCode="TitleTag" ID="hlpTitle"/>
		</td>
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
	    <td colspan="2" width="650">
		    If you are uploading a large image, please be patient while we automatically
		    generate the thumbnails for your image. This may take a few moments for large
		    file sizes.
	    </td>
    </tr>
	<tr runat="server" id="trViewImage">
		<td class="optional"><b>View Image:</b></td>
		<td class="field">
			<CC:FileUpload Required="false" id="fuViewImage" Folder="/assets/department/original/" ImageDisplayFolder="/assets/department/thumbnail/" DisplayImage="True" runat="server" />
		</td>
		<td valign="top"><CC:FileUploadExtensionValidator runat="server" ID="FileUploadExtensionValidator1" ControlToValidate="fuViewImage" Extensions="gif,jpg" ErrorMessage="Only extions .gif and .jpg are allowed"></cc:FileUploadExtensionValidator></td>
	</tr>	
	<tr runat="server" id="trViewImageAlt">
		<td class="optional"><b>View Image Alt Tag:</b></td>
		<td class="field">
			<asp:TextBox runat="server" ID="txtViewImageAlt" maxlength="100" Columns="50" style="width:319px;"></asp:TextBox>
		</td>
	</tr>
	<tr>
		<td class="optional">Custom URL:</td>
		<td class="field"><asp:TextBox id="txtCustomURL" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:TextBox><br /><span class="smaller">Please begin url with "/" i.e. "/sample-url.aspx"</span><br />
		<CC:HelpMessage runat="server" HelpCode="CustomUrl" ID="hlpCustomUrl"/></td>
		<td></td>
	</tr>		
	<tr>
		<td class="optional">Description:</td>
		<td class="field"><CC:CKEditor id="txtDescription" runat="server" Width="600" Height="300" /></td>
		<td></td>
	</tr>
</table>

<p></p>
<CC:OneClickButton id="Save" runat="server" Text="Save" cssClass="btn" />
<CC:OneClickButton id="Cancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False" />

</asp:content>