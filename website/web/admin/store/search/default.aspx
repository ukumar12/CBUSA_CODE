<%@ Page AutoEventWireup="false" CodeFile="default.aspx.vb" Inherits="admin_search_default" Language="VB"
    MasterPageFile="~/controls/AdminMaster.master" Title="Search Term" %>

<asp:Content ID="Content" runat="server" ContentPlaceHolderID="ph">
    <h4>Search Term</h4>
    
    <table border="0">
        <tr>
            <th><b>Start Date:</b></th>
            <td class="field"><CC:DatePicker ID="dpStartDate" runat="server"></CC:DatePicker></td>
            <th><b>End Date:</b></th><td class="field"><CC:DatePicker ID="dpEndDate" runat="server"></CC:DatePicker></td>
            <td>
            <CC:DateValidator runat="server" ID="dvStartDate" Display="Dynamic" ControlToValidate="dpStartDate" ErrorMessage="Field 'Start Date' is invalid" ></CC:DateValidator><br />
            <CC:DateValidator runat="server" ID="dvEndDate" Display="Dynamic" ControlToValidate="dpEndDate" ErrorMessage="Field 'End Date' is invalid" ></CC:DateValidator>
            </td>
        </tr>
        <tr>
            <th><b>Keyword:</b></th>
            <td class="field"><asp:TextBox ID="txtTerm" runat="Server"></asp:TextBox></td>
            <td colspan="2"><asp:CheckBox ID="chkIsDetails" Checked="false" runat="Server" />show detailed results</td>
        </tr>
        <tr>
            <td colspan="4" align="right"><CC:OneClickButton ID="btnFilter" CssClass="btn" Text="Search" runat="server" />
            <input class="btn" type="submit" value="Clear" onclick="window.location='default.aspx';return false;" />
            </td>
        </tr>
    </table>
    
    <CC:GridView ID="gvList" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
        BorderWidth="0" CellPadding="2" CellSpacing="2" EmptyDataText="There are no records that match the search criteria"
        HeaderText="In order to change display order, please use header links" PagerSettings-Position="Bottom"
        PageSize="50" >
        <AlternatingRowStyle CssClass="alternate" VerticalAlign="Top" />
        <RowStyle CssClass="row" VerticalAlign="Top" />
        <Columns>
            <asp:TemplateField>
                <itemtemplate><CC:OneClickButton id="btnDetails" cssclass="btn" runat="server" Text="Details" CommandName="Details" /></itemtemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Term" HeaderText="Term" SortExpression="Term" />
            <asp:BoundField DataField="RemoteIP" HeaderText="Remote IP" SortExpression="RemoteIP" />
            <asp:BoundField DataField="NumberResults" HeaderText="Number Results" SortExpression="NumberResults" />
            <asp:BoundField DataField="CreateDate" DataFormatString="{0:d}" HeaderText="Create Date" HTMLEncode="False" SortExpression="CreateDate" />
            <asp:BoundField DataField="AverageResults" HeaderText="Average Results" SortExpression="AverageResults" />
            <asp:BoundField DataField="NumberOfSearches" HeaderText="Number of Searches" SortExpression="NumberOfSearches" />
            <asp:templateField headerText="Order Number" SortExpression="OrderNo">
                <itemtemplate><asp:Literal runat="server" id="ltlOrderno" /></itemtemplate>
            </asp:templateField>
              <asp:templateField headerText="Member Name" SortExpression="MemberName">
                <itemtemplate><asp:Literal runat="server" id="ltlMemberName" /></itemtemplate>
            </asp:templateField>
        </Columns>
    </CC:GridView>
</asp:Content>
