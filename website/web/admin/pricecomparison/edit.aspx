<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Admin Document"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4>Edit Price Comparison Settings</h4>
<table>
    <tr>
        <td colspan="2">
             <b>Vendor Selection</b>
        </td>
    </tr>
    <tr>
        <td class="optional">Position 1:</td>
        <td><CC:AutoComplete ID="acdVendorID1" runat="server" Table="Vendor" TextField="CompanyName" ValueField="VendorID" AllowNew="false" CssClass="aclist"></CC:AutoComplete></td>
    </tr>
    <tr>
        <td class="optional">Position 2:</td>
        <td><CC:AutoComplete ID="acdVendorID2" runat="server" Table="Vendor" TextField="CompanyName" ValueField="VendorID" AllowNew="false" CssClass="aclist"></CC:AutoComplete></td>
    </tr>
    <tr>
        <td class="optional">Position 3:</td>
        <td><CC:AutoComplete ID="acdVendorID3" runat="server" Table="Vendor" TextField="CompanyName" ValueField="VendorID" AllowNew="false" CssClass="aclist"></CC:AutoComplete></td>
    </tr>
</table>
<br />
<table>
    <tr>
        <td colspan="2">
             <b>Order Selection</b>
        </td>
    </tr>
    <tr>
        <td class="optional">Position 1:</td>
        <td><CC:AutoComplete ID="acdOrderID1" runat="server" Table="[Order]" TextField="Title" ValueField="OrderID" AllowNew="false" CssClass="aclist"></CC:AutoComplete></td>
    </tr>
    <tr>
        <td class="optional">Position 2:</td>
        <td><CC:AutoComplete ID="acdOrderID2" runat="server" Table="[Order]" TextField="Title" ValueField="OrderID" AllowNew="false" CssClass="aclist"></CC:AutoComplete></td>
    </tr>
    <tr>
        <td class="optional">Position 3:</td>
        <td><CC:AutoComplete ID="acdOrderID3" runat="server" Table="[Order]" TextField="Title" ValueField="OrderID" AllowNew="false" CssClass="aclist"></CC:AutoComplete></td>
    </tr>
    <tr>
        <td class="optional">Position 4:</td>
        <td><CC:AutoComplete ID="acdOrderID4" runat="server" Table="[Order]" TextField="Title" ValueField="OrderID" AllowNew="false" CssClass="aclist"></CC:AutoComplete></td>
    </tr>
    <tr>
        <td class="optional">Position 5:</td>
        <td><CC:AutoComplete ID="acdOrderID5" runat="server" Table="[Order]" TextField="Title" ValueField="OrderID" AllowNew="false" CssClass="aclist"></CC:AutoComplete></td>
    </tr>
</table>
<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>
</asp:content>
