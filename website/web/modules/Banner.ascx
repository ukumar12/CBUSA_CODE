<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Banner.ascx.vb" Inherits="BannerCtrl" %>
<asp:Literal EnableViewState="False" id="ltlLink" runat="server"></asp:Literal>

<div id="divDesigner" runat="server" EnableViewState="False">
<div class="contentbottom">
<span class="smaller">Banner Group:</span>
<asp:DropDownList Id="drpBannerGroupId" CausesValidation="False" runat="server" AutoPostBack="True" />
<input type="hidden" id="hdnField" runat="server" name="hdnField" />
</div>
</div>