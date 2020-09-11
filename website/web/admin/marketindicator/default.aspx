<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Document" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Price Comparison Settings</h4>
<table>
    <tr>
        <td colspan="2">
             <b>Vendor Selection</b>
        </td>
    </tr>
    <tr>
        <th class="optional">Position 1:</th>
        <td><asp:Literal ID="ltrVendor1" runat="server"></asp:Literal></td>
    </tr>
    <tr>
        <th class="optional">Position 2:</th>
        <td><asp:Literal ID="ltrVendor2" runat="server"></asp:Literal></td>
    </tr>
    <tr>
        <th class="optional">Position 3:</th>
        <td><asp:Literal ID="ltrVendor3" runat="server"></asp:Literal></td>
    </tr>
</table>
<p></p>
<table>
    <tr>
        <td colspan="2">
             <b>Product Selection</b>
        </td>
    </tr>
    <tr>
        <th class="optional">Position 1:</th>
        <td><asp:Literal ID="ltrProduct1" runat="server"></asp:Literal></td>
    </tr>
    <tr>
        <th class="optional">Position 2:</th>
        <td><asp:Literal ID="ltrProduct2" runat="server"></asp:Literal></td>
    </tr>
    <tr>
        <th class="optional">Position 3:</th>
        <td><asp:Literal ID="ltrProduct3" runat="server"></asp:Literal></td>
    </tr>
    <tr>
        <th class="optional">Position 4:</th>
        <td><asp:Literal ID="ltrProduct4" runat="server"></asp:Literal></td>
    </tr>
    <tr>
        <th class="optional">Position 5:</th>
        <td><asp:Literal ID="ltrProduct5" runat="server"></asp:Literal></td>
    </tr>
</table>
<p></p>
<asp:Button id="btnEdit" runat="server" cssclass="btn" text="Edit" />
</asp:content>
