<%@ Control Language="VB" AutoEventWireup="false" CodeFile="VendorSelectForm.ascx.vb" Inherits="controls_VendorSelectForm" %>

<asp:PlaceHolder runat="server">
<script type="text/javascript">
    function ToggleVendors() {
        $('#<%=divForm.ClientID %>').slideToggle('slow');
    }
</script>
</asp:PlaceHolder>
<div id="divForm" runat="server" style="position:absolute;display:none;margin:5px;border:1px solid #000;background-color:#fff;width:600px;margin:auto;text-align:center;">
    <table cellpadding="0" cellspacing="0" border="0" style="width:510px;margin-left:auto;margin-right:auto;">
        <tr valign="bottom">
            <th style="text-align:center">Available Vendors</th>
            <th style="text-align:center">Selected Vendors</th>
        </tr>
    </table>
    <CC:MultiBox ID="mbVendors" runat="server" Width="510px" ListWidth="250" />
    <asp:Button ID="btnUpdate" runat="server" CssClass="btn" Text="Update" /><input type="button" class="btn" value="Close" onclick="ToggleVendors()" />
</div>
<input type="button" class="btn" value="Change Vendors" onclick="ToggleVendors()" />
