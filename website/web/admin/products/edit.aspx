<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Product"%>
<%@Register TagName="ProductTypeAttributesAdmin" TagPrefix="CC" Src="~/controls/ProductTypeAttributesAdmin.ascx" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
<asp:ScriptManager ID="AjaxManager" runat="server" EnablePartialRendering="true" EnablePageMethods="true"></asp:ScriptManager>
	
<h4><% If ProductID = 0 Then %>Add<% Else %>Edit<% End If %> Product</h4>

<table border="0" cellspacing="1" cellpadding="2">
	<tr><td colspan="2"><span class="smallest"><span class="red">red color</span> - denotes required fields</span></td></tr>
	<tr>
		<td class="required">Name:</td>
		<td class="field"><asp:textbox id="txtProduct" runat="server" maxlength="255" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><asp:RequiredFieldValidator ID="rfvProduct" runat="server" Display="Dynamic" ControlToValidate="txtProduct" ErrorMessage="Field 'Name' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
	    <td class="required">Supply Phase(s):</td>
	    <td class="field"><CC:CustomTreeView ID="ctvSupplyPhase" runat="server" Type="Checkbox"></CC:CustomTreeView></td>
	</tr>
	<tr>
		<td class="optional">LLC Pricing Required:</td>
		<td class="field">
		  <CC:CheckBoxListEx ID="cblcblPricingRequired" runat="server" RepeatColumns="3"></CC:CheckBoxListEx></td>
	</tr>
	<tr>
		<td class="optional">Description:</td>
		<td class="field"><asp:textbox id="txtDescription" runat="server" maxlength="2000" TextMode="MultiLine" Rows="3" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Manufacturer:</td>
		<td class="field"><CC:AutoComplete ID="acManufacturerID" runat="server" Table="Manufacturer" TextField="Manufacturer" ValueField="ManufacturerId" AllowNew="true" MinLength="1"></CC:AutoComplete></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Size:</td>
		<td class="field"><asp:textbox id="txtSize" runat="server" maxlength="100" columns="50" style="width: 319px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Size Unit Of Measure:</td>
		<td class="field"><asp:DropDownList id="drpSizeUnitOfMeasureID" runat="server" /></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Width:</td>
		<td class="field"><asp:textbox id="txtWidth" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><CC:IntegerValidator Display="Dynamic" runat="server" id="fvWidth" ControlToValidate="txtWidth" ErrorMessage="Field 'Width' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Width Unit Of Measure:</td>
		<td class="field"><asp:DropDownList id="drpWidthUnitOfMeasureID" runat="server" /></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Length:</td>
		<td class="field"><asp:textbox id="txtLength" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><CC:IntegerValidator Display="Dynamic" runat="server" id="fvLength" ControlToValidate="txtLength" ErrorMessage="Field 'Length' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Length Unit Of Measure:</td>
		<td class="field"><asp:DropDownList id="drpLengthUnitOfMeasureID" runat="server" /></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Height:</td>
		<td class="field"><asp:textbox id="txtHeight" runat="server" maxlength="4" columns="4" style="width: 43px;"></asp:textbox></td>
		<td><CC:IntegerValidator Display="Dynamic" runat="server" id="fvHeight" ControlToValidate="txtHeight" ErrorMessage="Field 'Height' is invalid" /></td>
	</tr>
	<tr>
		<td class="optional">Height Unit Of Measure:</td>
		<td class="field"><asp:DropDownList id="drpHeightUnitOfMeasureID" runat="server" /></td>
		<td></td>
	</tr>
	<tr>
		<td class="optional">Grade:</td>
		<td class="field"><asp:textbox id="txtGrade" runat="server" maxlength="20" columns="20" style="width: 139px;"></asp:textbox></td>
		<td></td>
	</tr>
	<tr>
		<td class="required">Product Type:</td>
		<td class="field"><asp:DropDownList id="drpProductTypeID" runat="server" AutoPostback="true" /></td>
		<td><asp:RequiredFieldValidator ID="rfvProductTypeID" runat="server" Display="Dynamic" ControlToValidate="drpProductTypeID" ErrorMessage="Field 'Product Type' is blank"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
	    <td class="required">Attributes:</td>
	    <td class="field">
	        <asp:UpdatePanel ID="upAttributes" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
	            <ContentTemplate>
                    <CC:ProductTypeAttributesAdmin ID="ctlAttributes" runat="server" />
	            </ContentTemplate>
	            <Triggers>
	                <asp:AsyncPostBackTrigger ControlID="drpProductTypeID" EventName="SelectedIndexChanged" />
	            </Triggers>
	        </asp:UpdatePanel>
	    </td>
	    <td></td>
	</tr>
	<tr>
		<td class="required"><b>Is Active?</b></td>
		<td class="field">
			<asp:RadioButtonList runat="server" ID="rblIsActive" RepeatDirection="Horizontal">
			<asp:ListItem Text="Yes" Value="True" />
			<asp:ListItem Text="No" Value="False" Selected="True" />
			</asp:RadioButtonList>
		</td>
	</tr>
</table>


<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:ConfirmButton id="btnDelete" runat="server" Message="Are you sure want to delete this Product?" Text="Delete" cssClass="btn" CausesValidation="False"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>

</asp:content>

