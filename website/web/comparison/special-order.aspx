<%@ Page Language="VB" AutoEventWireup="false" CodeFile="special-order.aspx.vb" Inherits="comparison_special_order" %>

<CT:MasterPage ID="CTMain" runat="server">
<h1>Special Order Products</h1>

<table cellpadding="2" cellspacing="2" border="0">
    <tr valign="top">
        <th>Builder</th>
        <th>Product</th>
        <th>Description</th>
        <th>Quantity</th>
        <th>Unit of Measure</th>
        <th>Product SKU</th>
        <th>Vendor Extended Price</th>
    </tr>
    <asp:Repeater id="rptRows" runat="server">
        <ItemTemplate>
            <tr valign="top">
                <td>
                    <%#DataBinder.Eval(Container.DataItem, "CompanyName")%>
                    <asp:HiddenField id="hdnID" runat="server" value='<%#DataBinder.Eval(Container.DataItem,"SpecialOrderProductId") %>'></asp:HiddenField>
                </td>
                <td><%#DataBinder.Eval(Container.DataItem, "SpecialOrderProduct")%></td>
                <td><%#DataBinder.Eval(Container.DataItem, "Description")%></td>
                <td><%#DataBinder.Eval(Container.DataItem, "Quantity")%></td>
                <td><%#DataBinder.Eval(Container.DataItem, "UnitOfMeasure")%></td>
                <td>
                    <asp:TextBox id="txtSku" runat="server" columns="30" maxlength="30"></asp:TextBox>
                    <asp:RequiredFieldValidator id="rfvtxtSku" runat="server" ControlToValidate="txtSku" ErrorMessage="SKU is empty"></asp:RequiredFieldValidator>
                </td>
                <td>
                    <asp:TextBox id="txtPrice" runat="server" columns="20" maxlength="20"></asp:TextBox>
                    <asp:RequiredFieldValidator id="rfvPrice" runat="server" ControlToValidate="txtPrice" ErrorMessage="Price is empty"></asp:RequiredFieldValidator>
                    <CC:FloatValidator ID="fvPrice" runat="server" ControlToValidate="txtPrice" ErrorMessage="Price is invalid"></CC:FloatValidator>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
</table>
<p style="text-align:center;margin:10px;">
    <asp:Button id="btnSubmit" runat="server" text="Submit" />
</p>    
</CT:MasterPage>