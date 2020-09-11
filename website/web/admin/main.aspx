<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Americaneagle.com" CodeFile="main.aspx.vb" Inherits="main" %>
<%@ Register TagPrefix="Ctrl" TagName="AdminLastLoginActivity" Src="~/controls/AdminLastLoginActivity.ascx" %>
<asp:content ID="Content" runat="server" contentplaceholderid="ph">
<Ctrl:AdminLastLoginActivity id=ctrlAdminLastLoginActivity runat=server></Ctrl:AdminLastLoginActivity>
</asp:content>