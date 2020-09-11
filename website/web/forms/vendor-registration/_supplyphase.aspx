<%@ Page Language="VB" AutoEventWireup="false" CodeFile="_supplyphase.aspx.vb" Inherits="supply_phase" EnableEventValidation="false" %>

<CT:MasterPage id="CTMain" runat="server">

<table class="tblnwto" cellpadding="0" cellspacing="0" border="0">
    <tr valign="top">
        <td class="leftcol">
            <div class="pckghdgred">Supply Phase Tree</div>
            <asp:UpdatePanel id="upFacets" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="True">
                <ContentTemplate>
                    <CC:FrontEndTreeView ID="tvSupplyPhases" runat="server" />                       
                </ContentTemplate>
            </asp:UpdatePanel>
        </td>      
        <td>
            <div class="maincolwrpr" style="margin-top:-2px;">
                <div class="pckghdgred">My Supply Phases</div>
            </div>
            <asp:UpdatePanel id="upSupplyPhases" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="False">
                <ContentTemplate>
                    <table class="tbltodata"  cellpadding="0" cellspacing="0" border="0">
                        <tr>
                            <th>List View</th>
                            <th>Tree View</th>
                        </tr>
                        <tr>
                            <td valign="top">
                            
                                <p class="red bold"><asp:Literal id="ltlErrMsg" runat="server"></asp:Literal></p>
                                <table cellspacing="0" cellpadding="0" border="0" class="tbltodata">

                                
                                <asp:Repeater id="rptSupplyPhases" runat="server">
                                    <HeaderTemplate>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td width="100%">
                                                <%#DataBinder.Eval(Container.DataItem, "SupplyPhase")%>
                                            </td>
                                            <td></td>
                                            <td>
                                                <asp:ImageButton ID="btnDeleteSupplyPhase" runat="server" ImageUrl="/images/admin/delete.gif" CausesValidation="false" />
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                    <tr>
                                        <td colspan=2></td>
                                    </tr>
                                    </table>
                                    </FooterTemplate>
                                </asp:Repeater>                  
                            </td>
                            <td>
                                <p>Parent supply phases will display automatically. </p>
                                <CC:FrontEndTreeView ID="tvVendorSupplyPhases" runat="server" />
                            </td>
                        </tr>
                    </table>
                    <p></p>
                    <table id="tblSku" runat="server" width="100%">
                        <tr align="center">
                            <td><asp:button ID="btnSKUPrice" runat="server" Text="Enter SKUs and prices" CssClass="btnred" /></p></td>
                            <td><asp:button ID="btnSKU" runat="server" Text="Enter prices later" CssClass="btnred" /></p></td>
                            <td><asp:button ID="btnPrice" runat="server" Text="Enter SKUs later" CssClass="btnred" /></p></td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
            
        </td>
    </tr>
</table>

</CT:MasterPage>