<%@ Page MaintainScrollPositionOnPostback="true" ValidateRequest="False" Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Advertiser Link"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If BannerId = 0 Then%>Add<% Else %>Edit<% End If %> Banner</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="red">red color</span> - denotes required fields</td></tr>
	<tr>
		<td class="required">Name:</td>
		<td class="field"><asp:textbox id="txtName" runat="server" maxlength="255" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvName" runat="server" Display="Dynamic" ControlToValidate="txtName" ErrorMessage="Field 'Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
		<td class="required">Banner Type:</td>
		<td class="field">
		<asp:RadioButtonList id="rblBannerType" runat="server" RepeatDirection="Horizontal" AutoPostBack="True">
		<asp:ListItem Text="Image" Value="Image"></asp:ListItem>
		<asp:ListItem Text="Flash" Value="Flash"></asp:ListItem>
		<asp:ListItem Text="Custom" Value="Custom"></asp:ListItem>
		</asp:RadioButtonList>
        </td>
		<td></td>
	</tr>
	<tr id="trDimensionsAuto" runat="server">
		<td class="required">Dimensions:</td>
		<td class="field"><asp:Literal id="ltlDimensions" runat="server"></asp:Literal></td>
		<td></td>
	</tr>
	<tr id="trDimensions" runat="server">
		<td class="required">Dimensions:</td>
		<td class="field">
		<asp:textbox id="txtWidth" runat="server" maxlength="4" columns="4" /> x <asp:textbox id="txtHeight" runat="server" maxlength="4" columns="4" />
		</td>
    	<td>
	    <asp:RequiredFieldValidator ID="rvtxtWidth" runat="server" Display="Dynamic" ControlToValidate="txtWidth" ErrorMessage="Field 'Width' is blank"></asp:RequiredFieldValidator>
	    <asp:RequiredFieldValidator ID="rvtxtHeight" runat="server" Display="Dynamic" ControlToValidate="txtHeight" ErrorMessage="Field 'Height' is blank"></asp:RequiredFieldValidator>
	    <CC:IntegerValidator ID="ivtxtWidth" runat="server" Display="Dynamic" ControlToValidate="txtWidth" ErrorMessage="Field 'Width' must contain valid integer value" />
	    <CC:IntegerValidator ID="ivtxtHeight" runat="server" Display="Dynamic" ControlToValidate="txtHeight" ErrorMessage="Field 'Height' must contain valid integer value" />
		</td>
	</tr>
	<tr id="trFile" runat="server">
		<td class="required">File:</td>
		<td class="field"><CC:FileUpload ID="fuFilename" Folder="/assets/banner/" ImageDisplayFolder="/assets/banner/" runat="server" style="width:300px;" /></td>
	</tr>
	<tr id="trAlt" runat="server">
		<td class="optional">Alt Tag:</td>
		<td class="field"><asp:textbox id="txtAltText" runat="server" maxlength="255" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr id="trHTML" runat="server">
		<td class="optional">HTML:</td>
		<td class="field"><asp:textbox id="txtHTML" runat="server" maxlength="255" columns="50" Rows="5" TextMode="MultiLine" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">Forward URL:</td>
		<td class="field"><asp:textbox id="txtLink" runat="server" maxlength="255" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvLink" runat="server" Display="Dynamic" ControlToValidate="txtLink" ErrorMessage="Field 'Forward URL' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr id="trTarget" runat="server">
		<td class="optional">Target:</td>
		<td class="field">
		<asp:DropDownList id="drpTarget" runat="server">
		<asp:ListItem Text="same window" Value=""></asp:ListItem>
		<asp:ListItem Text="new window" Value="_blank"></asp:ListItem>
		</asp:DropDownList>
		</td>
		<td></td>
	</tr>
	<tr>
		<td class="optional"><b>Is Banner Active?</b></td>
		<td class="field"><asp:CheckBox runat="server" ID="chkIsActive" /></td>
	</tr>
	<tr>
		<td class="optional"><b>Is E-commerce Tracking Enabled?</b></td>
		<td class="field"><asp:CheckBox runat="server" ID="chkIsOrderTracking" /></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Advertiser Link?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

