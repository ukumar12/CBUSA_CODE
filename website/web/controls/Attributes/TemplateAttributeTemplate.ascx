<%@ Control Language="VB" AutoEventWireup="false" CodeFile="TemplateAttributeTemplate.ascx.vb" Inherits="Controls.TemplateAttributeTemplate" %>
<tr>
<td>
<table border="0">
<tr>
<td class="bold larger">
<%#IIf(IsDBNull(CType(Container, RepeaterItem).DataItem("ParentId")), CType(Container, RepeaterItem).DataItem("AttributeName"), "")%>
<asp:LinkButton runat="server" ID="lt" CommandName="Toggle" Visible='<%#Not IsDBNull(CType(Container, RepeaterItem).DataItem("ParentId"))%>'>Show <%#CType(Container, RepeaterItem).DataItem("AttributeName")%></asp:LinkButton>
<sup>(<%#CType(Container, RepeaterItem).DataItem("iCount")%>)</sup>
</td>
<td>
<CC:OneClickButton ID="a" CommandName="Add" runat="server" CausesValidation="false" CssClass="btn" Text="Add"/>
</td>
<td>
<asp:LinkButton Visible='<%#Not IsDBNull(CType(Container, RepeaterItem).DataItem("ParentId"))%>' CommandName="Copy" CssClass="smaller" runat="server" ID="c" Text='<%#"Copy these " & CType(Container, RepeaterItem).DataItem("AttributeName") & " attributes to all"%>' onclientclick="return confirm(cpy);" />
</td>
</tr>
</table>
</td>
</tr>
<tr>
<td>
<div id="t" runat="server">
<div id="divEdit" runat="server" visible="False">
<input type="hidden" id="hdnFunctionType" runat="server" value='<%# DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "FunctionType")%>' />
<input type="hidden" id="hdnTemplateAttributeId" runat="server" value='<%# DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "TemplateAttributeId")%>' />
<input type="hidden" id="hdnTempAttributeId" runat="server" value="" />
<input type="hidden" id="hdnParentTempAttributeId" runat="server" value='<%# DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "ParentTempAttributeId")%>' />
<table border="0">
<tr>
<td colspan="3" runat="server" id="tdErrorMessage" visible="false" class="red"></td>
</tr>
<tr>
<td class="required">Value:</td>
<td><asp:textbox id="txtValue" runat="server" maxlength="50" columns="50" style="width: 150px;" /><asp:DropDownList runat="server" ID="drpValue" /></td>
<td><asp:RequiredFieldValidator ID="rfvValue" ValidationGroup="Attributes" runat="server" Display="Dynamic" ControlToValidate="txtValue" ErrorMessage="Field 'Value' is blank"></asp:RequiredFieldValidator></td>
</tr>
<tr>
<td class="optional">Additional SKU:</td>
<td><asp:textbox id="txtSKU" runat="server" maxlength="20" columns="20" style="width: 150px;" /></td>
</tr>
<tr>
<td class="required">Additional Price:</td>
<td><asp:textbox id="txtPrice" runat="server" maxlength="10" columns="10" text="0" style="width: 50px;" /></td>
<td><asp:RequiredFieldValidator runat="server" ID="rp" ControlToValidate="txtPrice" ValidationGroup="Attributes" Display="dynamic" ErrorMessage="Field 'Additional Price' is required" /><CC:FloatValidator ID="fvPrice" ValidationGroup="Attributes" runat="server" Display="Dynamic" ControlToValidate="txtPrice" ErrorMessage="Field 'Price' is invalid"></CC:FloatValidator></td>
</tr>
<tr>
<td class="required">Additional Weight:</td>
<td><asp:textbox id="txtWeight" runat="server" maxlength="10" columns="10" style="width: 50px;" /></td>
<td><asp:RequiredFieldValidator runat="server" ID="rw" ControlToValidate="txtWeight" ValidationGroup="Attributes" Display="dynamic" ErrorMessage="Field 'Additional Weight' is required" /><CC:FloatValidator ID="fvWeight" ValidationGroup="Attributes" runat="server" Display="Dynamic" ControlToValidate="txtWeight" ErrorMessage="Field 'Weight' is invalid"></CC:FloatValidator></td>
</tr>
<asp:PlaceHolder id="phImage" runat="server" Visible="true">
<tr>
<td class="optional">Swatch Image:</td>
<td><input type="hidden" id="hdnImageName" runat="server" /><div id="divImageName" runat="server" />
<asp:LinkButton Text="select" id="lnkUpload" runat="server">[upload]</asp:LinkButton>&nbsp;<a href="#" id="lnkClear" runat="server">[clear]</a>
<div id="divUpload" style="visibility:hidden;" runat="server">
<iframe runat="server" src="" id="frmUpload" style="width:400px;height:1px;z-index:1000;"></iframe>
</div>
</td>
<td></td>
</tr>
<tr>
<td class="optional">Swatch Alt Tag:</td>
<td><asp:textbox id="txtImageAlt" runat="server" maxlength="255" columns="40" style="width: 250px;" /></td>
</tr>
</asp:PlaceHolder>
<asp:PlaceHolder id="phProductImage" runat="server" Visible="true">
 <tr>
<td class="optional">Product Image:</td>
<td><input type="hidden" id="hdnProductImageName" runat="server" /><div id="divProductImageName" runat="server" />
<asp:LinkButton Text="select" id="lnkProductImageUpload" runat="server">[upload]</asp:LinkButton>&nbsp;<a href="#" id="lnkProductImageClear" runat="server">[clear]</a>
<div id="divProductImageUpload" style="visibility:hidden;" runat="server">
<iframe runat="server" src="" id="frmProductImageUpload" style="width:400px;height:1px;z-index:1000;"></iframe>
</div>
</td>
<td></td>
</tr>
<tr>
<td class="optional">Product Alt Tag:</td>
<td><asp:textbox id="txtProductAlt" runat="server" maxlength="255" columns="40" style="width: 250px;" /></td>
</tr>
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="phi">
<tr>
<td class="required">Inventory Qty:</td>
<td><asp:TextBox ID="txtiq" runat="server" MaxLength="10" Columns="10" style="width: 120px;" /></td>
<td><asp:RequiredFieldValidator runat="server" id="rviq" ControlToValidate="txtiq" ValidationGroup="Attributes" Display="dynamic" ErrorMessage="Field 'Inventory Qty' is required" /><CC:IntegerValidator ID="iviq" ValidationGroup="Attributes" runat="server" Display="Dynamic" ControlToValidate="txtiq" ErrorMessage="Field 'Inventory Quantity' is invalid"></CC:IntegerValidator></td>
</tr>
<tr>
<td class="optional">Inv Warning Lvl:</td>
<td><asp:TextBox ID="txtiw" runat="server" MaxLength="10" Columns="10" style="width: 120px;" /></td>
<td><CC:IntegerValidator ID="iviw" ValidationGroup="Attributes" runat="server" Display="Dynamic" ControlToValidate="txtiw" ErrorMessage="Field 'Inv Warning Lvl' is invalid"></CC:IntegerValidator></td>
</tr>
<tr>
<td class="optional">Inv Action Lvl:</td>
<td><asp:TextBox ID="txtia" runat="server" MaxLength="10" Columns="10" style="width: 120px;" /></td>
<td><CC:IntegerValidator ID="ivia" ValidationGroup="Attributes" runat="server" Display="Dynamic" ControlToValidate="txtia" ErrorMessage="Field 'Inv Action Lvl' is invalid"></CC:IntegerValidator></td>
</tr>
<tr>
<td class="required">Inventory Action:</td>
<td><asp:DropDownList runat="server" ID="dia"><asp:ListItem Text="Inherit" Value="" /><asp:ListItem Text="Disable" Value="Disable" /><asp:ListItem Text="Out of Stock" Value="OutOfStock" /><asp:ListItem Text="Backorder" Value="Backorder" /></asp:DropDownList></td>
<td></td>
</tr>
<tr id="trbo" runat="server">
<td class="optional">Backorder Date:</td>
<td><CC:DatePicker runat="server" ID="dpbd" /></td>
<td><CC:DateValidator runat="server" ControlToValidate="dpbd" ErrorMessage="Field 'Backorder Date' is invalid" Display="dynamic" ValidationGroup="Attributes" /></td>
</tr>
</asp:PlaceHolder>
<tr>
<td class="optional">Is Active?</td>
<td><asp:CheckBox runat="server" ID="chkIsActive" /></td>
</tr>
</table>
<CC:OneClickButton id="btnAttributeSave" CommandName="Save" ValidationGroup="Attributes" CommandArgument="" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:OneClickButton id="btnAttributeCancel" CommandName="Cancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>
</div>        
<p></p>
<asp:Repeater runat="server" ID="rl">
<HeaderTemplate>
<table border="0" cellpadding="2">
<tr>
<th></th>
<th></th>
<th>Value</th>
<th>Add SKU</th>
<th>Add Price</th>
<asp:PlaceHolder runat="server" ID="phih">
<th>Inventory</th>
</asp:PlaceHolder>
<th runat="server" id="thSwatch">Swatch</th>
<th>Product</th>
<th>Is Active?</th>
<th><%#CType(Container.NamingContainer.NamingContainer.NamingContainer, RepeaterItem).Controls(0).ID%></th>
<th></th>
</tr>
</HeaderTemplate>
<ItemTemplate>
<tr class="row">
<td><asp:ImageButton CommandName="Update" CommandArgument='<%# DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "TempAttributeId")%>' CausesValidation="false" runat="server" ImageUrl="/images/admin/edit.gif" ID="le"></asp:ImageButton></td>
<td><asp:ImageButton CommandName="Delete" CommandArgument='<%# DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "TempAttributeId")%>' CausesValidation="false" runat="server" ImageUrl="/images/admin/delete.gif" ID="ld" OnClientClick="return confirm(ctx)"></asp:ImageButton></td>
<td><%#CType(Container, RepeaterItem).DataItem("AttributeValue")%></td>
<td><%#CType(Container, RepeaterItem).DataItem("SKU")%></td>
<td><%#FormatCurrency(CType(Container, RepeaterItem).DataItem("Price"))%></td>
<asp:PlaceHolder runat="server" ID="phie">
<td>Quantity: <%#CType(Container, RepeaterItem).DataItem("InventoryQty")%><br />
Warning Level: <%#CType(Container, RepeaterItem).DataItem("InventoryWarningThreshold")%><br />
Action: <%#IIf(IsDBNull(CType(Container, RepeaterItem).DataItem("InventoryAction")), "Inherited", CType(Container, RepeaterItem).DataItem("InventoryAction"))%><br />
Action Level: <%#CType(Container, RepeaterItem).DataItem("InventoryActionThreshold")%><br />
B/O Date: <%#IIf(IsDBNull(CType(Container, RepeaterItem).DataItem("BackorderDate")), "", CDate(IIf(IsDBNull(CType(Container, RepeaterItem).DataItem("BackorderDate")), Nothing, CType(Container, RepeaterItem).DataItem("BackorderDate"))).ToShortDateString)%></td>
</asp:PlaceHolder>
<td runat="server" id="tdSwatch"><%#IIf(Not IsDBNull(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "ImageName")), "<img src=""/assets/item/swatch/small/" & DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "ImageName") & """ />", "")%></td>
<td><%#IIf(Not IsDBNull(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "ProductImage")), "<img src=""/assets/item/cart/" & DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "ProductImage") & """ />", "")%></td>
<td class="center"><%#IIf(CType(Container, RepeaterItem).DataItem("IsActive"), "<img src=""/images/admin/True.gif"" class=""nobdr"" />", "")%></td>
<td><asp:ImageButton CommandName="Up" CommandArgument='<%# DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "TempAttributeId")%>' runat="server" ImageUrl="/images/admin/moveup.gif" ID="u"></asp:ImageButton></td>
<td><asp:ImageButton CommandName="Down" CommandArgument='<%# DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "TempAttributeId")%>' runat="server" ImageUrl="/images/admin/movedown.gif" ID="d"></asp:ImageButton></td>
</tr>
<tr class="row" runat="server" id="rc">
<td></td>
<td></td>
<td colspan="8">
<asp:repeater id="ra" Runat="server">
<HeaderTemplate>
<table border="0">
</HeaderTemplate>
<ItemTemplate>
</ItemTemplate>
<FooterTemplate>
</table>
</FooterTemplate>
</asp:repeater>
</td>
</tr>
</ItemTemplate>
<AlternatingItemTemplate>
<tr class="alternate">
<td><asp:ImageButton CommandName="Update" CommandArgument='<%# DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "TempAttributeId")%>' CausesValidation="false" runat="server" ImageUrl="/images/admin/edit.gif" ID="le"></asp:ImageButton></td>
<td><asp:ImageButton CommandName="Delete" CommandArgument='<%# DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "TempAttributeId")%>' CausesValidation="false" runat="server" ImageUrl="/images/admin/delete.gif" ID="ld" OnClientClick="return confirm(ctx)"></asp:ImageButton></td>
<td><%#CType(Container, RepeaterItem).DataItem("AttributeValue")%></td>
<td><%#CType(Container, RepeaterItem).DataItem("SKU")%></td>
<td><%#FormatCurrency(CType(Container, RepeaterItem).DataItem("Price"))%></td>
<asp:PlaceHolder runat="server" ID="phie">
<td>Quantity: <%#CType(Container, RepeaterItem).DataItem("InventoryQty")%><br />
Warning Level: <%#CType(Container, RepeaterItem).DataItem("InventoryWarningThreshold")%><br />
Action: <%#IIf(IsDBNull(CType(Container, RepeaterItem).DataItem("InventoryAction")), "Inherited", CType(Container, RepeaterItem).DataItem("InventoryAction"))%><br />
Action Level: <%#CType(Container, RepeaterItem).DataItem("InventoryActionThreshold")%><br />
B/O Date: <%#IIf(IsDBNull(CType(Container, RepeaterItem).DataItem("BackorderDate")), "", CDate(IIf(IsDBNull(CType(Container, RepeaterItem).DataItem("BackorderDate")), Nothing, CType(Container, RepeaterItem).DataItem("BackorderDate"))).ToShortDateString)%></td>
</asp:PlaceHolder>
<td runat="server" id="tdSwatch"><%#IIf(Not IsDBNull(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "ImageName")), "<img src=""/assets/item/swatch/small/" & DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "ImageName") & """ />", "")%></td>
<td><%#IIf(Not IsDBNull(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "ProductImage")), "<img src=""/assets/item/cart/" & DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "ProductImage") & """ />", "")%></td>
<td class="center"><%#IIf(CType(Container, RepeaterItem).DataItem("IsActive"), "<img src=""/images/admin/True.gif"" class=""nobdr"" />", "")%></td>
<td><asp:ImageButton CommandName="Up" CommandArgument='<%# DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "TempAttributeId")%>' runat="server" ImageUrl="/images/admin/moveup.gif" ID="u"></asp:ImageButton></td>
<td><asp:ImageButton CommandName="Down" CommandArgument='<%# DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "TempAttributeId")%>' runat="server" ImageUrl="/images/admin/movedown.gif" ID="d"></asp:ImageButton></td>
</tr>
<tr class="alternate" runat="server" id="rc">
<td></td>
<td></td>
<td colspan="8">
<asp:repeater id="ra" Runat="server">
<HeaderTemplate>
<table border="0">
</HeaderTemplate>
<ItemTemplate>
</ItemTemplate>
<FooterTemplate>
</table>
</FooterTemplate>
</asp:repeater>
</td>
</tr>
</AlternatingItemTemplate>
<FooterTemplate>
</table>
</FooterTemplate>
</asp:Repeater>
</div>
</td>
</tr>
