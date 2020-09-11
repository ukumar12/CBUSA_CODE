<%@ Control Language="VB" AutoEventWireup="false" CodeFile="VendorInvoices.ascx.vb" Inherits="controls_VendorInvoices" %>


<table cellpadding="0" cellspacing="1" border="0">
    <tr>
        <th>Invoice Amount</th>
        <th>Invoice Number</th>
        <th>Date of Sale</th>
        <th>&nbsp;</th>
    </tr>
    <asp:PlaceHolder ID="phNoInvoices" runat="server">
        <tr>
            <td colspan="4">There are no matching invoices for this builder</td>
        </tr>
    </asp:PlaceHolder>
    <asp:Repeater ID="rptInvoices" runat="server">
        <ItemTemplate>
            <tr>
                <td>
                    <asp:Literal ID="ltlAmount" runat="server" Text='<%#DataBinder.Eval(Container.DataItem,"TotalAmount") %>' visible='<%#not EditMode %>'></asp:Literal>
                    <asp:TextBox ID="txtAmount" runat="server" Columns="10" MaxLength="15" Text='<%#DataBinder.Eval(Container.DataItem,"TotalAmount") %>' Visible='<%#EditMode %>'></asp:TextBox>
                </td>
                <td>
                    <asp:Literal ID="ltlNumber" runat="server" Text='<%#DataBinder.Eval(Container.DataItem,"InvoiceNumber") %>' Visible='<%#not EditMode %>'></asp:Literal>
                    <asp:TextBox ID="txtNumber" runat="server" Text='<%#DataBinder.Eval(Container.DataItem,"InvoiceNumber") %>' Visible='<%#EditMode %>' Columns="10" MaxLength="20"></asp:TextBox>
                </td>
                <td>
                    <asp:Literal ID="ltlSaleDate" runat="server" Text='<%#FormatDateTime(DataBinder.Eval(Container.DataItem,"InvoiceDate"),vbShortDate) %>' Visible='<%#not EditMode %>'></asp:Literal>
                    <CC:DatePicker ID="dpSaleDate" runat="server" Value='<%#DataBinder.Eval(Container.DataItem,"InvoiceDate") %>' Visible='<%#EditMode %>'></CC:DatePicker>
                </td>
                <td>
                    <asp:PlaceHolder ID="phButtons" runat="server">
                        <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btnred" />
                        <CC:ConfirmButton ID="btnDelete" runat="server" Text="Delete" CssClass="btnred" />
                        <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btnred" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btnred" />
                    </asp:PlaceHolder>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
</table>
<asp:Button ID="btnAddInvoice" runat="server" CssClass="btnred" Text="Add Invoice" />