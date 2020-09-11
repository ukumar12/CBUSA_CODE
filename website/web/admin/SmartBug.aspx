<%@ Page Language="VB" MasterPageFile="~/controls/AdminMaster.master" AutoEventWireup="false" CodeFile="SmartBug.aspx.vb" Inherits="admin_SmartBug" title="Untitled Page" %>
<asp:Content ID="ph" ContentPlaceHolderID="ph" Runat="Server">

<h4>Smart Bug Configuration</h4>

<table border=0 cellspacing=2 cellpadding=3>
<tr>
	<tr><td colspan=2><span class=smallest><span class=red>red color</span> - denotes required fields</span></td></tr>
</tr><tr>
	<td class="required"><b>Enabled?</b></td>
	<td class="field"><asp:Checkbox id="chkSmartBug" runat="server"/></td>
</tr>
</table>

<p>
<CC:OneClickButton ID="btnSubmit" Text="Save" cssclass="btn" runat="server" />&#160;<input type="button" class="btn" value="Cancel" onClick="document.location.href='main.aspx'">

</asp:Content>

