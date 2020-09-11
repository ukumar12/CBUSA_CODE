<%@ Control Language="VB" AutoEventWireup="false" CodeFile="_BuilderMonthlyData.ascx.vb" Inherits="modules_BuilderMonthlyData1" %>

<div class="pckgwhtwrpr themeprice">
    <div class="pckghdgred"><span class="smaller" style="float:right;">Show: <asp:DropDownList id="F_Interval" runat="server" AutoPostBack="true"></asp:DropDownList></span>Monthly Data</div>
    <asp:UpdatePanel ID="upData" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
        <table class="dshbrdtbl" cellpadding="0" cellspacing="0" border="0">
            <tr>
                <th>Month</th>
                <th>Started Units</th>
                <th>Sold Units</th>
                <th>Closing Units</th>
                <th>Unsold Units</th>
                <th>&nbsp;</th>
            </tr>
            <asp:Repeater ID="rptData" runat="server">
                <ItemTemplate>
                    <tr>
                        <td style="white-space:nowrap;">
                            <asp:Literal ID="ltlMonth" runat="server"></asp:Literal>
                            <asp:DropDownList ID="drpMonth" runat="server" SelectedValue='<%# DataBinder.Eval(Container.DataItem,"Month") %>'></asp:DropDownList>
                            <asp:DropDownList ID="drpYear" runat="server" SelectedValue='<%# DataBinder.Eval(Container.DataItem,"Year") %>'></asp:DropDownList><br />
                            <asp:RequiredFieldValidator ID="rfvddrpMonth" runat="server" ControlToValidate="drpMonth" ErrorMessage="Month is empty" ValidationGroup="MonthlyData"></asp:RequiredFieldValidator>
                            <asp:RequiredFieldValidator ID="rfvdrpYear" runat="server" ControlToValidate="drpYear" ErrorMessage="Year is empty" ValidationGroup="MonthlyData"></asp:RequiredFieldValidator>
                        </td>
                        <td>
                            <asp:Literal ID="ltlStarted" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"StartedUnits") %>'></asp:Literal>
                            <asp:TextBox ID="txtStarted" runat="server" Columns="9" MaxLength="10" Text='<%#DataBinder.Eval(Container.DataItem,"StartedUnits") %>'></asp:TextBox><br />
                            <asp:RequiredFieldValidator ID="rfvtxtStarted" runat="server" Display="Dynamic" ControlToValidate="txtStarted" ErrorMessage="Started Units is empty" ValidationGroup="MonthlyData"></asp:RequiredFieldValidator>
                            <CC:IntegerValidator ID="ivtxtStarted" runat="server" Display="Dynamic" ControlToValidate="txtStarted" ErrorMessage="Started Units is invalid" ValidationGroup="MonthlyData"></CC:IntegerValidator>
                        </td>
                        <td>
                            <asp:Literal ID="ltlSold" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"SoldUnits") %>'></asp:Literal>
                            <asp:TextBox ID="txtSold" runat="server" Columns="9" MaxLength="10" Text='<%#DataBinder.Eval(Container.DataItem,"SoldUnits") %>'></asp:TextBox><br />
                            <asp:RequiredFieldValidator ID="rfvtxtSold" runat="server" Display="Dynamic" ControlToValidate="txtSold" ErrorMessage="Sold Units is empty" ValidationGroup="MonthlyData"></asp:RequiredFieldValidator>
                            <CC:IntegerValidator ID="ivtxtSold" runat="server" Display="Dynamic" ControlToValidate="txtSold" ErrorMessage="Sold Units is invalid" ValidationGroup="MonthlyData"></CC:IntegerValidator>
                        </td>
                        <td>
                            <asp:Literal ID="ltlClosing" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"ClosingUnits") %>'></asp:Literal>
                            <asp:TextBox ID="txtClosing" runat="server" Columns="9" MaxLength="10" Text='<%#DataBinder.Eval(Container.DataItem,"ClosingUnits") %>'></asp:TextBox><br />
                            <asp:RequiredFieldValidator ID="rfvtxtClosing" runat="server" Display="Dynamic" ControlToValidate="txtClosing" ErrorMessage="Closing Units is empty" ValidationGroup="MonthlyData"></asp:RequiredFieldValidator>
                            <CC:IntegerValidator ID="ivtxtClosing" runat="server" Display="Dynamic" ControlToValidate="txtClosing" ErrorMessage="Closing Units is invalid" ValidationGroup="MonthlyData"></CC:IntegerValidator>
                        </td>
                        <td>
                            <asp:Literal ID="ltlUnsold" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"UnsoldUnits") %>'></asp:Literal>
                            <asp:TextBox ID="txtUnsold" runat="server" Columns="9" MaxLength="10" Text='<%#DataBinder.Eval(Container.DataItem,"UnsoldUnits") %>'></asp:TextBox><br />
                            <asp:RequiredFieldValidator ID="rfvtxtUnsold" runat="server" Display="Dynamic" ControlToValidate="txtUnsold" ErrorMessage="Unsold Units is empty" ValidationGroup="MonthlyData"></asp:RequiredFieldValidator>
                            <CC:IntegerValidator ID="ivtxtUnsold" runat="server" Display="Dynamic" ControlToValidate="txtUnsold" ErrorMessage="Unsold Units is invalid" ValidationGroup="MonthlyData"></CC:IntegerValidator>
                        </td>
                        <td style="white-space:nowrap;">
                            <asp:Button ID="btnEdit" runat="server" CssClass="btnblue" Text="Edit" CommandName="Edit" />
                            <asp:Button ID="btnSave" runat="server" CssClass="btnblue" Text="Save" CommandName="Save" />
                            <asp:Button ID="btnCancel" runat="server" CssClass="btnblue" Text="Cancel" CommandName="Cancel" />
                            <asp:Button ID="btnDelete" runat="server" CssClass="btnblue" Text="Delete" CommandName="Delete" />
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
        <asp:Literal ID="ltlNoItems" runat="server"></asp:Literal>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnAdd" />
        <asp:AsyncPostBackTrigger ControlID="F_Interval" />
    </Triggers>
    </asp:UpdatePanel>
    <p style="text-align:right;">
        <asp:Button ID="btnAdd" runat="server" CssClass="btnred" Text="Add Month" CausesValidation="false" />
    </p>
</div>