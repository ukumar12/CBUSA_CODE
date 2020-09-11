<%@ Page Language="VB" AutoEventWireup="false" CodeFile="recommend.aspx.vb" Inherits="forms_products_recommend" %>

<CT:MasterPage ID="CTMain" runat="server">
<h1>Recommend Product</h1>
<table cellpadding="5" cellspacing="0" border="0">
    <tr valign="top">
        <td>&nbsp;</td>
        <td class="fieldreq">&nbsp;</td>
        <td class="field smaller"> indicates required field</td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl"><span id="labeltxtProductName" runat="server">Product Name:</span></td>
        <td class="fieldreq" id="barttxtProductName" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox id="txtProductName" runat="server" columns="100" maxlength="100" style="width:319px;"></asp:TextBox></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl">Description:</td>
        <td>&nbsp;</td>
        <td class="field"><asp:TextBox id="txtDescription" runat="server" TextMode="Multiline" rows="4" columns="100" style="width:319px;"></asp:TextBox></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl">Manufacturer:</td>
        <td>&nbsp;</td>
        <td class="field"><CC:AutoComplete ID="acManufacturer" runat="server" AllowNew="true" Table="Manufacturer" TextField="Manufacturer" ValueField="ManufacturerId"></CC:AutoComplete></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl">Size:</td>
        <td>&nbsp;</td>
        <td class="field"><asp:TextBox id="txtSize" runat="server" columns="100" maxlength="100" style="width:319px;"></asp:TextBox></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl">Unit of Measure:</td>
        <td>&nbsp;</td>
        <td class="field"><asp:DropDownList id="drpUnitOfMeasure" runat="server"></asp:DropDownList></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl">Grade:</td>
        <td>&nbsp;</td>
        <td class="field"><asp:TextBox id="txtGrade" runat="server" columns="20" maxlength="20" style="width:75px;"></asp:TextBox></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl"><span id="labeldrpProductType" runat="server">Product Type:</span></td>
        <td class="fieldreq" id="bardrpProductType" runat="server">&nbsp;</td>
        <td class="field"><asp:DropDownList id="drpProductType" runat="server"></asp:DropDownList></td>
    </tr>
</table>
<asp:Button id="btnSave" runat="server" CssClass="btn" Text="Recommend Product" ValidationGroup="RecommendProduct" />

<CC:RequiredFieldValidatorFront ID="rfvProductName" runat="server" ControlToValidate="txtProductName" ErrorMessage="Field 'Product Name' is empty" ValidationGroup="RecommendProduct"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvProductType" runat="server" ControlToValidate="drpProductType" ErrorMessage="Field 'Product Type' is empty" ValidationGroup="RecommendProduct"></CC:RequiredFieldValidatorFront>
</CT:MasterPage>