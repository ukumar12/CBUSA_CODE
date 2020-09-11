<%@ Page Language="VB" AutoEventWireup="false" CodeFile="PrevTakeoff.aspx.vb" Inherits="builder_TwoPrice_PrevTakeoff" %>
<link rel="stylesheet" type="text/css" href="/includes/style.css">


    <div class="pckggraywrpr automargin">
        <div class="pckghdgred">Add Products From Existing TakeOffs</div>  
        <form id="Prevtakeoff" runat="server">
        <asp:GridView ID="gvListPrevTakeOff" runat="server" CellSpacing="2" CellPadding="2" CssClass="tblcompr" PageSize="20" SortBy ="Saved" SortOrder="Desc" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" OnPageIndexChanging="gvListPrevTakeOff_PageIndexChanging" >
            <HeaderStyle CssClass="sbttl" />
            <AlternatingRowStyle CssClass="alternate" VerticalAlign="Top" Font-Size="Larger"></AlternatingRowStyle>
            <RowStyle CssClass="row" VerticalAlign="Top" Font-Size="Larger" Font-Bold="true"></RowStyle>
            <Columns>
                <asp:templatefield>
		                <ItemStyle width="20px" />
		                <ItemTemplate>
		                    <asp:CheckBox id="cbInclude" runat="server"></asp:CheckBox>
                            </ItemTemplate>
		            </asp:templatefield>
                <asp:boundfield datafield="Project" headertext="Project" />
                <asp:boundfield datafield="Title" headertext="Title" />
                <asp:boundfield datafield="Saved" headertext="Saved" />
            </Columns>
            
        </asp:GridView>
            
        <asp:button id="btnAddTakeOffProducts" runat="server" text="Add Products from Existing Takeoff(s)" cssclass="btnred" />
        <asp:button id="btnCloseWin" runat="server" text="Close window" cssclass="btnred" />
            </form>
    </div>

