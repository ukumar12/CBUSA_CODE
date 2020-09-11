<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Callouts" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Callouts Administration</h4>

<p></p>

<table>
    <col width="30px" />
    <col width="150px" />
    <col width="" />
    <tr class="row">
        <td align="right">
            <asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= 'edit.aspx?index=1&type=builder' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
        </td>
        <td>
            Builder Callout 1
        </td>
        <td>
            <div id="divBuilderCallout1" runat="server">
            </div>
        </td>
    </tr>
    <tr class="alternate">
        <td align="right">
            <asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= 'edit.aspx?index=2&type=builder' ImageUrl="/images/admin/edit.gif" ID="HyperLink1">Edit</asp:HyperLink>
        </td>
        <td>
            Builder Callout 2
        </td>
        <td>
            <div id="divBuilderCallout2" runat="server">
            </div>
        </td>
    </tr>
    <tr class="row">
        <td align="right">
            <asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= 'edit.aspx?index=1&type=vendor' ImageUrl="/images/admin/edit.gif" ID="HyperLink2">Edit</asp:HyperLink>
        </td>
        <td>
            Vendor Callout 1
        </td>
        <td>
            <div id="divVendorCallout1" runat="server">
            </div>
        </td>
    </tr>
    <tr class="alternate">
        <td align="right">
            <asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= 'edit.aspx?index=2&type=vendor' ImageUrl="/images/admin/edit.gif" ID="HyperLink3">Edit</asp:HyperLink>
        </td>
        <td>
            Vendor Callout 2
        </td>
        <td>
            <div id="divVendorCallout2" runat="server">
            </div>
        </td>
    </tr>
    <tr class="row">
        <td align="right">
            <asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= 'edit.aspx?index=1&type=piq' ImageUrl="/images/admin/edit.gif" ID="HyperLink4">Edit</asp:HyperLink>
        </td>
        <td>
            PIQ Callout 1
        </td>
        <td>
            <div id="divPIQCallout1" runat="server">
            </div>
        </td>
    </tr>
    <tr class="alternate">
        <td align="right">
            <asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= 'edit.aspx?index=2&type=piq' ImageUrl="/images/admin/edit.gif" ID="HyperLink5">Edit</asp:HyperLink>
        </td>
        <td>
            PIQ Callout 2
        </td>
        <td>
            <div id="divPIQCallout2" runat="server">
            </div>
        </td>
    </tr>
</table>
</asp:content>
