<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Builder Info" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Builder Info Administration</h4>

<p></p>

<table>
    <col width="30px" />
    <col width="150px" />
    <col width="" />
    <tr class="row">
        <td align="right">
            <asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= 'edit.aspx' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
        </td>
        <td>
            Display Text
        </td>
        <td>
            <div id="divBuilderInfo" runat="server">
            </div>
        </td>
    </tr>
</table>
</asp:content>
