<%@ Control Language="VB" AutoEventWireup="false" CodeFile="TakeoffService.ascx.vb" Inherits="modules_Builders_TakeoffService" %>
<div class="pckgltgraywrpr">
    <div class="pckghdgred nobdr">
       
        <asp:Button runat="server" CssClass = "btnred" ID="btndashboard" Text= "DashBoard" style="margin-left:630px;float:right;"  /> 
    </div>
    <div class="Contracttblwrpr themeprice">
        <asp:Repeater ID="rptTakeOFfService" runat="server">
            <HeaderTemplate>
            </HeaderTemplate>
            <ItemTemplate>
                <div class="bdbtblhdg">
                    <div class="caption">
                        <asp:Literal ID="ltlName" runat="server" /><%# Eval("Title")%>
                    </div>
                    <div class="clear">
                        &nbsp;</div>
                </div>
                <asp:Literal ID="ltlDescription" runat="server" />
                <%# Eval("Description")%>
                <asp:Literal ID="ltlContentID" runat="server" Text='<%# Eval("TakeOffServiceID")%>' Visible="false" />
                <CC:GridView ID="gvDocuments" Width="98%" CellSpacing="2" CellPadding="2" runat="server"
                    PageSize="50" AllowPaging="False" AllowSorting="False"  
                    AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom" CssClass="tblcompr">
                    <AlternatingRowStyle CssClass="alternate" />
                    <RowStyle CssClass="row" />
                    <Columns>
                        <asp:BoundField DataField="Title" HeaderText="Title" />
                        <asp:TemplateField SortExpression="Document" HeaderText="Document">
                            <ItemTemplate>
                                <asp:HyperLink EnableViewState="False" runat="server" ID="LnkDocument"><%#Eval("FileName")%></asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </CC:GridView>
            </ItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</div>