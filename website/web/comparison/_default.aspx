<%@ Page Language="VB" AutoEventWireup="false" CodeFile="_default.aspx.vb" Inherits="comparison_default" %>


<CT:MasterPage ID="CTMain" runat="server">

    <CC:ListSelect ID="lsVendors" runat="server" SelectLimit="5" AddImageUrl="/images/admin/create.gif" DeleteImageUrl="/images/admin/delete.gif" AutoPostback="true" />

    <asp:UpdatePanel id="upMain" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
        <ContentTemplate>
            <asp:Repeater id="rptMain" runat="server">
                <HeaderTemplate>
                    <table width="100%" cellpadding="2" cellspacing="2" border="0">
                        <tr valign="top">
                            <td>Qty</td>
                            <td>Sku</td>
                            <td>Product Name</td>
                            <asp:Repeater id="rptHeader" runat="server" enableviewstate="false">
                                <ItemTemplate>
                                    <td><%#DataBinder.Eval(Container.DataItem, "CompanyName")%></td>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tr>
                    </table>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr valign="top">
                        <td><%#DataBinder.Eval(Container.DataItem, "Quantity")%></td>
                        <td><%#DataBinder.Eval(Container.DataItem, "Sku")%></td>
                        <td><%#DataBinder.Eval(Container.DataItem, "Product")%></td>
                        <asp:Repeater id="rptVendors" runat="server" enableviewstate="false">
                            <ItemTemplate>
                                <td class="<asp:Literal id="ltlClass" runat="server"></asp:Literal>"><%#FormatCurrency(DataBinder.Eval(Container.DataItem, "Price"))%></td>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostbackTrigger ControlID="lsVendors" EventName="SelectedChanged"></asp:AsyncPostbackTrigger>
        </Triggers>
    </asp:UpdatePanel>

</CT:MasterPage>