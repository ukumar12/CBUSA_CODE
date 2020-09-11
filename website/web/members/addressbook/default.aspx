<%@ Page Language="vb" AutoEventWireup="false" Inherits="member_addressbook_default" CodeFile="default.aspx.vb" %>

<CT:masterpage runat="server" id="CTMain">

<h2 class="hdng">My Address Book</h2>

<CC:OneClickButton id="btnAdd" runat="server" Text="Add New Address" CssClass="btn" />

<!-- cart table -->
<div id="divItems" runat="server">
<asp:repeater id="rptAddressBook" EnableViewState="False" runat="server">
<HeaderTemplate>
   <table cellspacing="0" cellpadding="2" border="0" class="bdrtop bdrright" style="width:702px; margin-top:10px;" summary="shopping cart table">
    <tr>
        <th style="padding:6px 0 6px 15px;">&nbsp;</td>
        <th style="padding:6px 0 6px 0;">&nbsp;</td>
        <th style="padding:6px 0 6px 0;">Label</td>
        <th style="padding:6px 0 6px 0;">Name</td>	        
        <th style="padding:6px 0 6px 0;">Address</td>
        </tr>      
    </HeaderTemplate>
    <ItemTemplate>
        <!-- row-->
        <tr valign="top">
        <td class="center bdrbottom bdrleft" style="padding-top: 5px; padding-bottom: 5px;">
            <CC:OneClickButton id="btnEdit" commandname="Edit" AlternateText="Edit" runat="server" CssClass="btn" Text="Edit" />
        </td>
        <td class="blue bdrbottom" style="padding-top: 5px; padding-bottom: 5px;">
            <CC:ConfirmButton Message="Are you sure you want to remove this address?" id="btnDelete" commandname="Remove" AlternateText="Remove" runat="server" CssClass="btn" Text="Delete" />
        </td>
        <td class="blue bdrbottom" style="padding-top: 5px; padding-bottom: 5px;">
            <asp:literal id="ltlLabel" runat="server" />
        </td>
        <td class="bdrbottom" style="padding-top: 5px; padding-bottom: 5px;">
            <asp:literal id="ltlName" runat="server" />
        </td>
        <td class="blue bdrbottom" style="padding-top: 5px; padding-bottom: 5px;">
            <asp:literal id="ltlAddress" runat="server" />
        </td>
        </tr>      
    </ItemTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
</asp:repeater>
</div>      

<div id="divNoItems" runat="server">
There are currently no entries in your address book
</div>

</CT:masterpage>