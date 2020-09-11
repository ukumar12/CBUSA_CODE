<%@ Page Language="VB" ValidateRequest="False" AutoEventWireup="false" CodeFile="lookup.aspx.vb" MasterPageFile="~/controls/AdminMasterNotes.master" Inherits="Lookup"  Title="Store Image Lookup"%>
<%@ Register TagPrefix="CC" TagName="StoreDepartmentTree" Src="~/controls/StoreDepartmentTree.ascx" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<asp:Panel ID="pnlUpload" runat="server">
<CC:RequiredFileUploadValidator ControlToValidate="fuImageName" id="frvSwatch" runat="server" ErrorMessage="File is required"></CC:RequiredFileUploadValidator>
<CC:FileUploadExtensionValidator Extensions="jpg,jpeg,gif,bmp,png" ID="fuvSwatch" runat="server" Display="Dynamic" ControlToValidate="fuImageName" ErrorMessage="Invalid file extension"></CC:FileUploadExtensionValidator>

<table>
<tr id="">
    <td class="optional">Image:</td>
    <td class="field"><CC:FileUpload ID="fuImageName" Folder="/assets/item/original" DisplayImage="True" runat="server" style="width: 319px;" /></td>
</tr>
</table>

<CC:OneClickButton id="btnSave" runat="server" Text="Preview" cssClass="btn"></CC:OneClickButton>
<input class="btn" type="button" onclick="parent.HideUploadFrame('<%=divUpload%>','<%=frmUpload%>');" value="Close">
</asp:Panel>

<asp:Panel ID="pnlPreview" runat="server" Visible="False">

<img src="<%=ImageFolder & Viewstate("ImageName")%>" />

<p></p>
<CC:OneClickButton id="btnSelect" runat="server" Text="Select" cssClass="btn"></CC:OneClickButton>
<CC:OneClickButton ID="btnMakeChanges" CssClass="btn" runat="server" Text="Make Changes" />
<input class="btn" type="button" onclick="parent.HideUploadFrame('<%=divUpload%>','<%=frmUpload%>');" value="Close">
</asp:Panel>

</asp:content>
