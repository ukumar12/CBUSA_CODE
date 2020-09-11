<%@ Control Language="VB" AutoEventWireup="false" CodeFile="_ProductsToCompareForm.ascx.vb" Inherits="controls_ProductsToCompareForm" %>

<script type="text/javascript">
    function ToggleCompareProducts() {
        var pnl = $('#<%=divProducts.ClientId %>');
        if (pnl[0].childNodes.length == 0) return;
        pnl.slideToggle('slow');
        var btn = $get('<%=btnView.ClientId %>');
        if (btn.value == 'Show Products') {
            btn.value = 'Hide Products';
        } else {
            btn.value = 'Show Products';
        }
    }
</script>
<table cellpadding="0" cellspacing="0" border="0" class="fltrbar">
    <tr>
        <td class="hdg">Select Products From:</td>
        <td>
            <CC:AutoComplete ID="acProjects" runat="server" Table="Project" TextField="ProjectName" ValueField="ProjectId" AllowNew="false" AutoPostBack="true"></CC:AutoComplete>
        </td>
        <td>
            <CC:AutoComplete ID="acOrders" runat="server" Table="Order" TextField="Title" ValueField="OrderId" AllowNew="false" AutoPostBack="true"></CC:AutoComplete>
        </td>
        <td>
            <CC:AutoComplete ID="acTakeoffs" runat="server" Table="Takeoff" TextField="Title" ValueField="TakeoffId" AllowNew="false" AutoPostBack="true"></CC:AutoComplete>
        </td>
        <td>
            <a href="http://cbusa2.design.americaneagle.com/templates/foo.asp"><img title="Add Related Products to Take-off" style="width:28px;border:none;height:26px;" alt="Add Related Products to Take-off" src="/images/global/btn-fltrbar-add.gif" /></a><br />
        </td>
        <td>
            <a href="http://cbusa2.design.americaneagle.com/templates/foo.asp"><img title="Upload Products to Take-off" style="width: 28px; border:none; HEIGHT: 26px;" alt="Upload Products to Take-off" src="/images/global/btn-fltrbar-upload.gif" /></a><br />
        </td>
        <td>
            <a href="http://cbusa2.design.americaneagle.com/templates/foo.asp"><img title="Add Special Order Product to Take-off" style="width: 28px; border:none; height: 26px;" alt="Add Special Order Product to Take-off" src="/images/global/btn-fltrbar-addspecial.gif" /></a><br />
        </td>
        <td>
            <img style="width: 30px; height: 1px" alt="" src="/images/spacer.gif" /><br />
        </td>
        <td>
            <asp:TextBox ID="txtKeywords" runat="server" style="width:154px;color:#666;" onfocus="this.value='',this.style.color='#000'" Text="Keyword Search"></asp:TextBox>
        </td>
        <td>
            <a href="http://cbusa2.design.americaneagle.com/templates/foo.asp"><img style="width: 28px; border:none; height: 26px;" alt="Search" src="/images/global/btn-fltrbar-search.gif" /></a><br />
        </td>    
    </tr>
    <tr valign="top">
        <td colspan="3" style="padding:5px 50px">
            <asp:UpdatePanel ID="upProducts" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                <ContentTemplate>
                    <h6 align="center">Selected Products: <asp:Literal ID="ltlSelected" runat="server"><br />Select a Project, Order or TakeOff above to view products.</asp:Literal></h6>
                    <div id="divProducts" runat="server" style="display:none;">
                        <asp:Repeater ID="rptProducts" runat="server">
                            <HeaderTemplate>
                                <table cellpadding="5" cellspacing="0" border="0">
                                    <tr valign="top">
                                        <th>Product</th>
                                        <th>Quantity</th>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr id="trTitle" runat="server" valign="top" visible="false">
                                    <td colspan="2"><span class="bold" id="spanTitle" runat="server"></span></td>
                                </tr>
                                <tr valign="top">
                                    <td><%#IIf(IsDBNull(DataBinder.Eval(Container.DataItem, "SpecialOrderProduct")), DataBinder.Eval(Container.DataItem, "Product"), DataBinder.Eval(Container.DataItem, "SpecialOrderProduct"))%></td>
                                    <td><%#IIf(IsDBNull(DataBinder.Eval(Container.DataItem, "SpecialOrderProduct")), DataBinder.Eval(Container.DataItem, "Quantity"), String.Empty)%></td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                    <asp:Button ID="btnView" runat="server" CssClass="btn" Text="Show Products" OnClientClick="ToggleCompareProducts();" />
                    <asp:Button ID="btnAdd" runat="server" CssClass="btn" Text="Add To TakeOff" />
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="acProjects" EventName="ValueUpdated" />
                    <asp:AsyncPostBackTrigger ControlID="acOrders" EventName="ValueUpdated" />
                    <asp:AsyncPostBackTrigger ControlID="acTakeoffs" EventName="ValueUpdated" />
                    <asp:AsyncPostBackTrigger ControlID="btnAdd" />
                </Triggers>
            </asp:UpdatePanel>        
        </td>
    </tr>
</table>
