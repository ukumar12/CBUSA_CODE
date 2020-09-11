<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ProductsToCompareForm.ascx.vb" Inherits="controls_ProductsToCompareForm" %>

<script type="text/javascript">
    function UpdateWhereClauses(ctlProjects) {
        var takeoffs = $get('<%=acTakeoffs.ClientID %>').control;
        var orders = $get('<%=acOrders.ClientID %>').control;
        takeoffs.set_whereClause('ProjectID=' + ctlProjects.get_value());
        takeoffs._updateList(true);
        orders.set_whereClause('ProjectID=' + ctlProjects.get_value());
        orders._updateList(true);
    }
</script>

<asp:UpdatePanel ID="upFilterBar" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
    <ContentTemplate>
        <table cellpadding="0" cellspacing="0" border="0" class="fltrbar">
            <tr>
                <td class="hdg">Select Products From:</td>
                <td>
                    <CC:HoverList ID="hlProjects" runat="server" AutoPostback="true" Label="Projects" LabelClass="hoverlabel" CssClass="hoverlist" width="150px" EmptyText="There are no projects to display" />
                </td>
                <td>
                    <CC:AutoComplete ID="acOrders" runat="server" Table="Order" TextField="Title" ValueField="OrderId" AllowNew="false" AutoPostBack="false"></CC:AutoComplete>
                </td>
                <td>
                    <CC:AutoComplete ID="acTakeoffs" runat="server" Table="Takeoff" TextField="Title" ValueField="TakeoffId" AllowNew="false" AutoPostBack="false"></CC:AutoComplete>
                </td>
                <td>
                    <asp:ImageButton ID="btnAdd" runat="server" ImageUrl="/images/global/btn-fltrbar-add.gif" AlternateText="Add Related Products to Take-off" style="width:28px;border:none;height:26px;" />
                </td>
                <td>
                    <asp:ImageButton ID="btnUpload" runat="server" ImageUrl="/images/global/btn-fltrbar-upload.gif" AlternateText="Upload Products to Take-off" style="width:28px;border:none;height:26px;" />
                </td>
                <td>
                    <asp:ImageButton ID="btnSpecial" runat="server" ImageUrl="/images/global/btn-fltrbar-addspecial.gif" alternatetext="Add Special Order Product to Take-off" style="width:28px;border:none;height:26px;" />
                </td>
                <td>
                    <img style="width: 30px; height: 1px" alt="" src="/images/spacer.gif" /><br />
                </td>
                <td>
                    <asp:TextBox ID="txtKeywords" runat="server" style="width:154px;color:#666;" onfocus="this.value='',this.style.color='#000'" Text="Keyword Search"></asp:TextBox>
                </td>
                <td>
                    <asp:ImageButton ID="btnSearch" runat="server" ImageUrl="/images/global/btn-fltrbar-search.gif" AlternateText="Search" style="width:28px;border:none;height:26px;" />
                </td>    
            </tr>
        </table>
    </ContentTemplate>
    <Triggers>
        <asp:PostBackTrigger ControlID="btnAdd" />
        <asp:AsyncPostBackTrigger ControlID="btnUpload" />
        <asp:AsyncPostBackTrigger ControlID="btnSearch" />
    </Triggers>
</asp:UpdatePanel>