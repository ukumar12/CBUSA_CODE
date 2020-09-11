<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Item Template Attribute"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4><% If TemplateAttributeId = 0 Then %>Add<% Else %>Edit<% End If %> &nbsp;<asp:Literal ID="ltlTemplateName" runat="server" /> Attribute</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan=2><span class=smallest><span class=red>red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="optional">Parent Attribute:</td>
		<td class="field"><asp:DropDownList id="drpParentId" runat="server" /></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">Attribute Name:</td>
		<td class="field"><asp:textbox id="txtAttributeName" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvAttributeName" runat="server" Display="Dynamic" ControlToValidate="txtAttributeName" ErrorMessage="Field 'Attribute Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr runat="server" id="trInventory">
		<td class="required"><b>Tree Is Inventory?</b></td>
		<td class="field"><asp:CheckBox runat="server" ID="chkIsInventoryManagement" /> Inventory control applied to lowest level of tree</td>
	</tr>
	<tr>
		<td class="required">Function Type:</td>
		<td class="field">
			<asp:RadioButton runat="server" ID="rbFreeText" AutoPostBack="true" GroupName="FunctionType" Text="Free Text" /><span style="margin-left:8px;"></span>
			<asp:RadioButton runat="server" ID="rbSpecifyValue" AutoPostBack="true" GroupName="FunctionType" Text="Specify Values" /><span style="margin-left:8px;"></span>
			<asp:RadioButton runat="server" ID="rbLookupTable" AutoPostBack="true" GroupName="FunctionType" Text="Lookup Table" />
		</td>
		<td></td>
	</tr>
	<tr runat="server" id="trSpecifyValue">
		<td class="required">Specify Values:</td>
		<td class="field"><div class="smaller">Put each value on a separate line</div><asp:TextBox runat="server" ID="txtSpecifyValue" TextMode="multiLine" Rows="7" Columns="38" /></td>
		<td><asp:RequiredFieldValidator runat="server" ID="rfvtxtSpecifyValue" ControlToValidate="txtSpecifyValue" Display="dynamic" ErrorMessage="Field 'Specify Values' is required" /></td>
	</tr>
	<tr runat="server" id="trLookupTable">
		<td class="required">Lookup Table:</td>
		<td class="field"><asp:DropDownList runat="server" ID="drpTable" AutoPostBack="true" /></td>
		<td><asp:RequiredFieldValidator runat="server" ID="rfvdrpTable" ControlToValidate="drpTable" Display="dynamic" ErrorMessage="Field 'Lookup Table' is required" /></td>
	</tr>
	<tr runat="server" id="trLookupColumn">
		<td class="required">Lookup Column:</td>
		<td class="field"><asp:DropDownList runat="server" ID="drpColumn" /></td>
		<td></td>
	</tr>
	<tr runat="server" id="trLookupSKU">
		<td class="optional">SKU Column:</td>
		<td class="field"><asp:DropDownList runat="server" ID="drpSKU" /> <asp:CheckBox runat="server" Text="Include in selections" ID="chkInclude" /></td>
		<td></td>
	</tr>
	<tr runat="server" id="trLookupPrice">
		<td class="optional">Price Column:</td>
		<td class="field"><asp:DropDownList runat="server" ID="drpPrice" /></td>
		<td></td>
	</tr>
	<tr runat="server" id="trLookupWeight">
		<td class="optional">Weight Column:</td>
		<td class="field"><asp:DropDownList runat="server" ID="drpWeight" /></td>
		<td></td>
	</tr>
	<tr runat="server" id="trLookupSwatch">
		<td class="optional">Swatch Column:</td>
		<td class="field"><asp:DropDownList runat="server" ID="drpSwatch" /></td>
		<td></td>
	</tr>
	<tr runat="server" id="trAltSwatch">
		<td class="optional">Swatch Alt Column:</td>
		<td class="field"><asp:DropDownList runat="server" ID="drpAlt" /></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">Attribute Type:</td>
		<td class="field">
		  <asp:DropDownList ID="drpAttributeType" runat="server" AutoPostBack="true">
		    <asp:ListItem Value="dropdown">Dropdown</asp:ListItem>
		    <asp:ListItem Value="radio">Radio Buttons</asp:ListItem>
		    <asp:ListItem Value="swatch">Swatches</asp:ListItem>
		  </asp:DropDownList>
		</td>
		<td><asp:RequiredFieldValidator ID="rfvAttributeType" runat="server" Display="Dynamic" ControlToValidate="drpAttributeType" ErrorMessage="Field 'Attribute Type' is blank"></asp:RequiredFieldValidator></td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn" CausesValidation="false"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Item Template Attribute?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>
