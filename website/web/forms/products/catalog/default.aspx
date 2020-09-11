<%@ Page Language="VB" AutoEventWireup="false" CodeFile="default.aspx.vb" Inherits="forms_products_catalog_default" %>

<CT:MasterPage ID="CTMain" runat="server">
<h1>Download Catalog</h1>
<table cellpadding="5" cellspacing="0" border="0">
    <tr valign="top">
        <td>&nbsp;</td>
        <td class="fieldreq">&nbsp;</td>
        <td class="field smaller"> indicates required field</td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl"><span id="labelrblFormat" runat="server">Format:</span></td>
        <td class="fieldreq" id="barrblFormat" runat="server">&nbsp;</td>
        <td class="field">
            <asp:RadioButtonList id="rblFormat" runat="server">
                <asp:ListItem value="HTML">HTML</asp:ListItem>
                <asp:ListItem value="EXCEL">Excel</asp:ListItem>
            </asp:RadioButtonList>
        </td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl">Vendor:</td>
        <td>&nbsp;</td>
        <td class="field"><CC:AutoComplete ID="acVendor" runat="server" AllowNew="false" Table="Vendor" TextField="CompanyName" ValueField="VendorId"></CC:AutoComplete></td>
    </tr>
    <tr valign="top">
        <td class="fieldlbl"><span id="labelctvSupplyPhases" runat="server">Supply Phases:</span></td>
        <td class="fieldreq" id="barctvSupplyPhases" runat="server">&nbsp;</td>
        <td class="field"><CC:CustomTreeView ID="ctvSupplyPhases" runat="server" Type="Checkbox" /></td>
    </tr>
</table>
<asp:Button id="btnProcess" runat="server" cssclass="btn" text="View Catalog" />
<input type="button" class="btn" onclick="location.refresh();" value="Clear" />

<asp:Literal id="ltlDownloadLink" runat="server"></asp:Literal>
<CC:GridView ID="gvList" runat="server" AutoGenerateColumns="false" AllowPaging="true" AllowSorting="false" PageSize="25">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
    <Columns>
        <asp:BoundField DataField="SKU" HeaderText="CBUSA SKU" />
        <asp:BoundField DataField="Product" HeaderText="Name" />
        <asp:BoundField DataField="Size" HeaderText="Size" />
    </Columns>
</CC:GridView>

</CT:MasterPage>
