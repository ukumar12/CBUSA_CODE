<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="National Contracts" CodeFile="Default.aspx.vb" Inherits="admin_NationalContracts_Default" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
    <h4>National Contract Administration</h4>

    <span class="smaller">Please provide search criteria below</span>
    <asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
        <table cellpadding="2" cellspacing="2">
            <tr>
                <th valign="top">Title: </th>
                <td valign="top"><asp:textbox id="F_Title" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
            </tr>
            <tr>
                <th valign="top">Manufacturer: </th>
                <td valign="top"><asp:textbox id="F_Manufacturer" runat="server" Columns="50" MaxLength="50"></asp:textbox></td>
            </tr>
             <tr>
                <th valign="top">Contract Term: </th>
                <td valign="top"><asp:textbox id="F_ContractTerm" runat="server" Columns="50" MaxLength="100"></asp:textbox></td>
            </tr>

            <tr style="display:none">
                <th valign="top"><b>Contract Term:</b></th>
                <td valign="top" class="field">
                    <table border="0" cellpadding="0" cellspacing="0">
                         <tr>
                             <td class="smaller">From <CC:DatePicker id="F_StartDate" runat="server" /></td>
                             <td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_EndDate" runat="server" /></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="right">
                    <asp:button id="btnExport" Runat="server" Text="Export" cssClass="btn" />
                    <CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btn" />
                    <input class="btn" type="submit" value="Clear" onclick="window.location = 'default.aspx'; return false;" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <p></p>
    <CC:OneClickButton id="AddNew" Runat="server" Text="Add New Contract" cssClass="btn"></CC:OneClickButton>
    <p></p>

    <CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="False" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
        <AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	    <RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>

        <Columns>
            <asp:TemplateField>
			    <ItemTemplate>
			        <asp:HyperLink enableviewstate="False" runat="server" NavigateUrl= '<%# "edit.aspx?ContractID=" & DataBinder.Eval(Container.DataItem, "ContractID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
			    </ItemTemplate>
		    </asp:TemplateField>
		    <asp:TemplateField>
			    <ItemTemplate>
			        <CC:ConfirmLink enableviewstate="False" Message="Are you sure that you want to remove this contract?" runat="server" NavigateUrl= '<%# "delete.aspx?ContractID=" & DataBinder.Eval(Container.DataItem, "ContractID") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
			    </ItemTemplate>
		    </asp:TemplateField>

            <asp:BoundField SortExpression="Title" DataField="Title" HeaderText="Title"></asp:BoundField>
		    <asp:BoundField SortExpression="Manufacturer" DataField="Manufacturer" HeaderText="Manufacturer"></asp:BoundField>
		    <asp:BoundField SortExpression="Products" DataField="Products" HeaderText="Products"></asp:BoundField>
            <asp:BoundField SortExpression="ContractTerm" DataField="ContractTerm" HeaderText=" Contract Term"></asp:BoundField>
           
        </Columns>
    </CC:GridView>

</asp:content>


