<%@ Page ValidateRequest="False" Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Mailing Message" CodeFile="preview.aspx.vb" Inherits="Preview"  %>
<%@ Register TagPrefix="CC" TagName="MailingPreview" Src="~/controls/MailingPreview.ascx" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4><asp:Literal ID="ltlName" runat="server"></asp:Literal> - E-mail Preview</h4>

<table cellpadding="0" cellspacing="0">
<tr>
<td><div><asp:LinkButton id="lnkBack" runat="server" CssClass="L1" Text="<< Go Back" /></div></td>
</tr>
</table>

<br />
<CC:MailingPreview id="PreviewCtrl" runat="server"></CC:MailingPreview>

</asp:content>

