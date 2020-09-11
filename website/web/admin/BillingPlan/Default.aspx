<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Billing Plans" CodeFile="default.aspx.vb" Inherits="Index"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Billing Plans</h4>

<span class="smaller">Please provide search criteria below</span>
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
    <table cellpadding="2" cellspacing="2">
        <tr>
            <th valign="top">Billing Plan:</th>
            <td valign="top" class="field">
                <asp:textbox id="F_BillingPlan" runat="server" Columns="50" MaxLength="50"></asp:textbox>
            </td>
        </tr>
        <tr>
            <th valign="top"><b>Record State:</b></th>
            <td valign="top" class="field">
	            <asp:DropDownList ID="F_RecordState" runat="server">
		            <asp:ListItem Value="">-- ALL --</asp:ListItem>
		            <asp:ListItem Value="1">Active</asp:ListItem>
		            <asp:ListItem Value="0">Inactive</asp:ListItem>
                    <asp:ListItem Value="2">Deleted</asp:ListItem>
	            </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td colspan="2" align="right">
                <CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
                <input class="btn" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />
            </td>
        </tr>
    </table>
</asp:Panel>
<p></p>
<CC:OneClickButton id="AddNew" Runat="server" Text="Add New Billing Plan" cssClass="btn"></CC:OneClickButton>
<p></p>

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?BillingPlanId=" & DataBinder.Eval(Container.DataItem, "BillingPlanId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
			<CC:ConfirmLink enableviewstate="False" Message="Members are currently associated with this plan. If you click OK, these users will be transferred to the default billing plan." runat="server" NavigateUrl= '<%# "delete.aspx?BillingPlanId=" & DataBinder.Eval(Container.DataItem, "BillingPlanId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="DisplayValue" DataField="DisplayValue" HeaderText="Billing Plan"></asp:BoundField>
		<asp:BoundField SortExpression="CreatedOn" DataField="CreatedOn" HeaderText="Created On" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
        <asp:BoundField SortExpression="ActiveState" DataField="ActiveState" HeaderText="Record State"></asp:BoundField>
        <asp:BoundField DataField="IsDefault" HeaderText="" />
        <asp:ImageField DataImageUrlField="IsDefault" HeaderText="Is Default" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
	</Columns>
</CC:GridView>

</asp:content>

