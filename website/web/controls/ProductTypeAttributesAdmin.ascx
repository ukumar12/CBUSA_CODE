<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ProductTypeAttributesAdmin.ascx.vb" Inherits="controls_ProductTypeAttributesAdmin" %>
<asp:Repeater ID="rptAttributes" runat="server">
    <HeaderTemplate>
        <table cellpadding="2" cellspacing="2" border="0">
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td class="required"><%#DataBinder.Eval(Container.DataItem, "Attribute")%></td>
            <td class="field">
                <asp:PlaceHolder ID="phControl" runat="server"></asp:PlaceHolder>
            </td>
            <td>
                <asp:PlaceHolder ID="phValidator" runat="server"></asp:PlaceHolder>
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
</asp:Repeater>