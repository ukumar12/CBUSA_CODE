<%@ Page Language="VB" MasterPageFile="~/controls/AdminMaster.master" AutoEventWireup="false" CodeFile="default.aspx.vb" Inherits="index" title="User Group Administration" %>
<asp:Content ID="Content" ContentPlaceHolderID="ph" Runat="Server">

<h4>User Group Administration</h4>

<p>
<CC:OneClickButton id="AddNew" cssClass="btn" Text="Add New User Group" Runat="server" />
</p>

<table cellspacing="0" cellpadding="0" border="0" id="tblList" runat="server">
<tr>
<td>
<asp:datagrid id="dgList" runat="server" PageSize="20" AllowPaging="True" AutoGenerateColumns="False"
        CellSpacing="2" CellPadding="2" AllowSorting="True" BorderWidth="0" Width="203px">
        <AlternatingItemStyle CssClass="alternate"></AlternatingItemStyle>
        <ItemStyle CssClass="row"></ItemStyle>
        <HeaderStyle CssClass="header"></HeaderStyle>
        <Columns>
            <asp:TemplateColumn>
                <ItemTemplate>
                    <asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?GroupId=" & DataBinder.Eval(Container.DataItem, "GroupId")  & "&" & params %>' ImageUrl="/images/admin/edit.gif" ID="Hyperlink2">Edit</asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn>
                <ItemTemplate>
                    <CC:ConfirmLink enableviewstate="False" Message="Are you sure that you want to remove this group?" runat="server" NavigateUrl= '<%# "delete.aspx?GroupId=" & DataBinder.Eval(Container.DataItem, "GroupId") & "&" & params %>' ImageUrl="/images/admin/delete.gif" ID="Confirmlink1">Delete</CC:ConfirmLink>
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn SortExpression="Y">
                <HeaderTemplate>
                    <asp:LinkButton enableviewstate="False" CommandArgument="DESCRIPTION" CommandName="sort" id="Linkbutton1"
                        runat="server">Group Name</asp:LinkButton>
                    <asp:LinkButton enableviewstate="False" visible='<%#Viewstate("F_SortBy") = "DESCRIPTION" and Viewstate("F_SortOrder") = "ASC" %>' CommandArgument="DESCRIPTION" CommandName="sort" id="Linkbutton2" runat="server">
                        <img border="0" src="/images/admin/Asc3.gif" alt="Ascending"></asp:LinkButton>
                    <asp:LinkButton enableviewstate="False" visible='<%#Viewstate("F_SortBy") = "DESCRIPTION" and Viewstate("F_SortOrder") = "DESC" %>' CommandArgument="DESCRIPTION" CommandName="sort" id="Linkbutton3" runat="server">
                        <img border="0" src="/images/admin/Desc3.gif" alt="Descending"></asp:LinkButton>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label enableviewstate="False" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DESCRIPTION") %>' ID="Label2">
                    </asp:Label>
                </ItemTemplate>
            </asp:TemplateColumn>
        </Columns>
        <PagerStyle Visible="False"></PagerStyle>
    </asp:datagrid></td>
</tr>
<tr>
    <td><CC:Navigator id="myNavigator" runat="server" PagerSize="10"/></td>
</tr>
</table>

<asp:placeholder id="plcNoRecords" runat="server" visible="false">There are no records that match the search criteria</asp:placeholder>

</asp:Content>

