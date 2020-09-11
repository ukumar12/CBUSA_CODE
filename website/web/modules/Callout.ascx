<%@ Control EnableViewstate="False" Language="VB" AutoEventWireup="false" CodeFile="Callout.ascx.vb" Inherits="Callout" %>
<div id="divDesigner" runat="server" EnableViewState="False">
    <div class="contentbottom">
        <span class="smaller">Index:</span>
        <asp:DropDownList Id="drpIndex" CausesValidation="False" runat="server" AutoPostBack="True" />
        <span class="smaller">Default Type:</span>
        <asp:DropDownList Id="drpType" CausesValidation="False" runat="server" AutoPostBack="True" />
        <input type="hidden" id="hdnField" runat="server" name="hdnField" />
    </div>
</div>

<div class="splitcolpad">
	<div id="divContent" runat="server"></div>
    <div class="clear">&nbsp;</div>
</div>

