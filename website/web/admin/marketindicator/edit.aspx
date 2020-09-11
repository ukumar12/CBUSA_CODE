<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Admin Document"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
	
<h4>Edit Price Comparison Settings</h4>
<table>
    <tr>
        <td colspan="2">
             <b>Vendor Selection</b>
        </td>
    </tr>
    <tr style="position:relative;z-index:10;">
        <td class="optional">Position 1:</td>
        <td><CC:SearchList ID="acdVendorID1" runat="server" Table="Vendor" TextField="CompanyName" ValueField="VendorID" AllowNew="false" CssClass="searchlist" ViewAllLength="10"></CC:SearchList></td>
    </tr>
    <tr style="position:relative;z-index:9;">
        <td class="optional">Position 2:</td>
        <td><CC:SearchList ID="acdVendorID2" runat="server" Table="Vendor" TextField="CompanyName" ValueField="VendorID" AllowNew="false" CssClass="searchlist" ViewAllLength="10"></CC:SearchList></td>
    </tr>
    <tr style="position:relative;z-index:8;">
        <td class="optional">Position 3:</td>
        <td><CC:SearchList ID="acdVendorID3" runat="server" Table="Vendor" TextField="CompanyName" ValueField="VendorID" AllowNew="false" CssClass="searchlist" ViewAllLength="10"></CC:SearchList></td>
    </tr>
</table>
<br />
<table>
    <tr>
        <td colspan="2">
             <b>Product Selection</b>
        </td>
    </tr>
    <tr style="position:relative;z-index:7;">
        <td class="optional">Position 1:</td>
        <td><CC:SearchList ID="acdProductID1" runat="server" Table="Product" TextField="Product" ValueField="ProductID" AllowNew="false" CssClass="searchlist" ViewAllLength="10"></CC:SearchList></td>
    </tr>
    <tr style="position:relative;z-index:6;">
        <td class="optional">Position 2:</td>
        <td><CC:SearchList ID="acdProductID2" runat="server" Table="Product" TextField="Product" ValueField="ProductID" AllowNew="false" CssClass="searchlist" ViewAllLength="10"></CC:SearchList></td>
    </tr>
    <tr style="position:relative;z-index:5;">
        <td class="optional">Position 3:</td>
        <td><CC:Searchlist ID="acdProductID3" runat="server" Table="Product" TextField="Product" ValueField="ProductID" AllowNew="false" CssClass="searchlist" ViewAllLength="10"></CC:SearchList></td>
    </tr>
    <tr style="position:relative;z-index:4;">
        <td class="optional">Position 4:</td>
        <td><CC:SearchList ID="acdProductID4" runat="server" Table="Product" TextField="Product" ValueField="ProductID" AllowNew="false" CssClass="searchlist" ViewAllLength="10"></CC:SearchList></td>
    </tr>
    <tr style="position:relative;z-index:3;">
        <td class="optional">Position 5:</td>
        <td><CC:SearchList ID="acdProductID5" runat="server" Table="Product" TextField="Product" ValueField="ProductID" AllowNew="false" CssClass="searchlist" ViewAllLength="10"></CC:SearchList></td>
    </tr>
</table>
<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>
</asp:content>
