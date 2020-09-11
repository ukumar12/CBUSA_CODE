<%@ Page Language="VB" AutoEventWireup="false" CodeFile="default.aspx.vb" Inherits="takeoffs_vendor"  %>

<CT:MasterPage runat="server" ID="CTMain">
    <asp:PlaceHolder runat="server">
        <div class="pckggraywrpr">
            <div class="pckghdgred">Committed Purchase Events</div>
                <%--<asp:button id ="btnDashBoard" text="Go to DashBoard" class="btnred" runat="server" />--%>
            <%--<div style="padding:10px;">
            </div>--%>
            <CC:GridView id="gvList" CellSpacing="2" CellPadding="2" CssClass="tblcomprlen" runat="server" PageSize="10" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText= "There are currently no active Committed Purchase Events available to your company." AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	            <HeaderStyle CssClass="sbttl" />
	            <AlternatingRowStyle CssClass="alternate" VerticalAlign="Top" Font-Size="Larger"></AlternatingRowStyle>
	            <RowStyle CssClass="row" VerticalAlign="Top" Font-Size="Larger" Font-Bold="true"></RowStyle>
	            <Columns>
		            <asp:TemplateField>
                        <headertemplate>
                            Commited Purchase Event
                        </headertemplate>
			            <ItemTemplate>
                            <asp:linkbutton ID="lnkCommitted" runat="server" CommandName="ChangeTakeoffPrices" CommandArgument="" Style="color:black"></asp:linkbutton>
			            </ItemTemplate>
		            </asp:TemplateField>
                    <asp:TemplateField>
                        <headertemplate>
                            Status
                        </headertemplate>
			            <ItemTemplate>
			                <asp:literal id="ltlStatus" runat="server" />
			            </ItemTemplate>
		            </asp:TemplateField>
                    <asp:BoundField DataField="StartDate" HeaderText="Start Date" SortExpression="StartDate" DataFormatString="{0:M/d/yyyy}" />
                    <asp:BoundField DataField="EndDate" HeaderText="End Date" SortExpression="EndDate" DataFormatString="{0:M/d/yyyy}" />
	            </Columns>
            </CC:GridView>
        </div>
    </asp:PlaceHolder>
</CT:MasterPage>