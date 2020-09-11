<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Attribute.ascx.vb" Inherits="Controls.Attribute" %>
<tr>
	<td><asp:Literal runat="server" ID="ltlAttributeName" /></td>
	<td>
		<asp:DropDownList runat="server" ID="drpAttribute" AutoPostBack="true" />
		<asp:PlaceHolder runat="server" ID="phAttribute" />
		<input type="hidden" runat="server" ID="hdnAttribute" value="0" />
	</td>
</tr>