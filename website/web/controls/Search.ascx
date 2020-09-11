<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Search.ascx.vb" Inherits="controls_Search" %>
<%@ Register Assembly="NineRays.WebControls.FlyTreeView" Namespace="NineRays.WebControls" TagPrefix="NineRays" %>


<asp:UpdatePanel id="upFacets" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="True">
    <ContentTemplate>
        <div class="leftcolwrpr">
            <div id="divTitle" runat="server" class="pckghdgred" style="height:15px;">Supply Phase Tree</div>
            <div class="redbox">
                <ul>
                    <li id="liLLCFilter" runat="server">
                    <asp:RadioButtonList id="rblFilter" runat="server" RepeatDirection="Vertical" autopostback="true">
                        <asp:ListItem value="market"  >All Products Priced In This Market</asp:ListItem>
                        <asp:Listitem value="all">All Products In Catalog</asp:Listitem>
                    </asp:RadioButtonList>
                    <%--<asp:PlaceHolder ID="plcRadio" runat="server"></asp:PlaceHolder>
                        <ul>
                            <li><asp:Literal id="ltlLLCOnly" runat="server"></asp:Literal></li>
                            <li><asp:Literal id="ltlAllProducts" runat="server"></asp:Literal></li>
                        </ul>--%>
                    </li>
                <asp:Repeater id="rptProductType" runat="server">
                    <HeaderTemplate>
                        <li>By Product Type
                        <ul>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li><asp:Literal id="ltlLabel" runat="server"></asp:Literal></li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                        </li>
                    </FooterTemplate>
                </asp:Repeater>
                <asp:Repeater id="rptManufacturer" runat="server">
                    <HeaderTemplate>
                        <li>By Manufacturer
                        <ul>
                    </HeaderTemplate>
                    <ItemTemplate>
                            <li><asp:Literal id="ltlLabel" runat="server"></asp:Literal></li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                        </li>
                    </FooterTemplate>
                </asp:Repeater>
                <asp:Repeater id="rptUnitOfMeasure" runat="server">
                    <HeaderTemplate>
                        <li>By Unit of Measure
                        <ul>
                    </HeaderTemplate>
                    <ItemTemplate>
                            <li><asp:Literal id="ltlLabel" runat="server"></asp:Literal></li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                        </li>
                    </FooterTemplate>
                </asp:Repeater>
                </ul>
            </div>
            <CC:FrontEndTreeView ID="tvSupplyPhases" runat="server" height="500" Visible="false" />        

            <NineRays:FlyTreeView PostBackOnClick="true" OnNodeSelected="flyTreeView_NodeSelected" ID="flyTreeView" runat="server" BackColor="White" ImageSet="Classic"
            BorderColor="Silver" BorderWidth="1px" Height="320px" Padding="3px"
            OnPopulateNodes="flyTreeView_PopulateNodes" FadeEffect="True" Style="display: block">
            <DefaultStyle Font-Names="Tahoma" Font-Size="11px" ForeColor="Black" ImageUrl="$classic_folder"
                Padding="1px;3px;3px;1px" RowHeight="16px" />
            <SelectedStyle BackColor="65, 86, 122" BorderColor="40, 40, 40" BorderStyle="Solid"
                BorderWidth="1px" ForeColor="White" Padding="0px;2px;2px;0px" ImageUrl="$classic_folder_open" />
            <HoverStyle Font-Underline="True" />
            </NineRays:FlyTreeView>
            
            <div class="cblock10">
                <asp:LinkButton ID="lnkExpand" runat="server" Text="Expand All"></asp:LinkButton> <span class="pipe">|</span> <asp:LinkButton ID="lnkCollapse" runat="server" Text="Collapse All"></asp:LinkButton>
            </div>                
        </div>
        <CC:UnvalidatedPostback id="btnSearch" runat="server"></CC:UnvalidatedPostback>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="tvSupplyPhases" />
    </Triggers>
</asp:UpdatePanel>

<CC:PopupForm ID="frmLoading" runat="server" CssClass="pform" Animate="false" ShowVeil="true" VeilCloses="false">
    <FormTemplate>
        <div style="background-color:#fff;width:200px;height:80px;text-align:center;padding:30px 10px;">
            <img src="/images/loading.gif" alt="Processing..." /><br /><br />
            <h1 class="largest">Processing... Please Wait</h1>
        </div>
    </FormTemplate>
</CC:PopupForm>