<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MarketIndicators.ascx.vb" Inherits="modules_MarketIndicators" %>

<asp:UpdatePanel ID="upIndicators" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
<ContentTemplate>
<div class="pckgltgraywrpr" style="display:none;">
    <div class="pckghdgblue nobdr"><a href="/catalog/" class="whtlnk smaller" style="float:right;">Add New Indicator</a>Market Indicators</div>
    <div class="stacktblwrpr thememarket">
        <div class="bdbtblhdg">Vendor Product Prices</div>
        <asp:Literal ID="ltlNone" runat="server"></asp:Literal>
        <asp:Repeater ID="rptIndicators" runat="server">
            <ItemTemplate>
                <div class="pckgwhtwrpr" style="padding-bottom:0px;">
                <table class="larger" style="table-layout:fixed;">
                    <tr>
                        <th style="width:40%;">Product</th>
                        <asp:Literal ID="ltlVendorHead" runat="server"></asp:Literal>
                        <th style="width:25px;">&nbsp;</th>
                    </tr>                    
                    <tr>
                        <td><asp:Literal ID="ltlProduct" runat="server"></asp:Literal></td>
                        <asp:Literal ID="ltlVendorPrices" runat="server"></asp:Literal>
                        <td align="right"><asp:ImageButton ID="btnRemove" runat="server" AlternateText="Remove" ImageUrl="/images/global/icon-remove.gif" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"BuilderIndicatorProductID") %>' CausesValidation="false" /></td>
                    </tr>
                </table>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>
</ContentTemplate>
</asp:UpdatePanel>