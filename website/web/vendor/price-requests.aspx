﻿<%@ Page Language="VB" AutoEventWireup="false" CodeFile="price-requests.aspx.vb" Inherits="vendor_price_requests" %>

<CT:MasterPage ID="CTMain" runat="server">

<script type="text/javascript">
    function UpdateRows(cb) {
        //var trSub = $get(cb.id.replace('cbIsSubYes', 'trSubstitute').replace('cbIsSubNo', 'trSubstitute'));
        var trSpecial = $get(cb.id.replace('cbIsSubYes', 'trSpecial').replace('cbIsSubNo', 'trSpecial'));
        var trMultiplier = $get(cb.id.replace('cbIsSubYes', 'trMultiplier').replace('cbIsSubNo', 'trMultiplier'));
        if (cb.value == 'true') {
            //trSub.style.display = '';
            trSpecial.style.display = 'none';
            trMultiplier.style.display = '';            
        } else {
        //trSub.style.display = 'none';
        trSpecial.style.display = '';
        trMultiplier.style.display = 'none';
        }
    }
</script>

<div class="pckggraywrpr">
    <div class="pckghdgred">Builder Price Requests</div>
    <asp:UpdatePanel id="upRequests" runat="server" UpdateMode="Always">
        <ContentTemplate>
        <CC:GridView ID="gvRequests" runat="server" AutoGenerateColumns="false" AllowPaging="true" AllowSorting="true" CssClass="tblcompr">
            <HeaderStyle CssClass="sbttl" />
            <RowStyle CssClass="row" />
            <AlternatingRowStyle CssClass="alternate" />
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:Button id="btnUpdate" runat="server" text="Update" cssclass="btnred" />
                        <CC:PopupForm id="frmRequest" runat="server" OpenMode="MoveToCenter" ShowVeil="true" VeilCloses="false" OpenTriggerId="btnUpdate" CloseTriggerId="btnCancel" cssclass="pform">
                            <FormTemplate>
                                <div class="pckghdgred">Update Price Request</div>
                                <div style="padding:10px;">
                                    <asp:HiddenField id="hdnRequestId" runat="server"></asp:HiddenField>
                                    <table cellpadding="3" cellspacing="0" border="0">
                                        <tr>
                                            <td><b>Builder:</b></td>
                                            <td><asp:Literal id="ltlBuilder" runat="server"></asp:Literal></td>
                                        </tr>
                                        <tr>
                                            <td><b>Product:</b></td>
                                            <td><asp:Literal id="ltlProduct" runat="server"></asp:Literal></td>
                                        </tr>
                                        <tr id="trIsSub" runat="server">
                                            <td><b>Is Substitution?</b></td>
                                            <td>
                                                <asp:RadioButton id="cbIsSubYes" runat="server" GroupName="rblIsSub" value="true" text="Yes" onclick="UpdateRows(this)"></asp:RadioButton><br />
                                                <asp:RadioButton id="cbIsSubNo" runat="server" GroupName="rblIsSub" value="false" text="No" onclick="UpdateRows(this)"></asp:RadioButton>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td><b>Vendor SKU:</b></td>
                                            <td><asp:TextBox id="txtSku" runat="server" columns="20" maxlength="30"></asp:TextBox></td>
                                        </tr>
                                        <tr id="trSpecial" runat="server">
                                            <td><b>Price:</b></td>
                                            <td><asp:TextBox id="txtPrice" runat="server" columns="10" maxlength="10"></asp:TextBox></td>
                                        </tr>
                                        <tr id="trMultiplier" runat="server">
                                            <td><b>Quantity Multiplier:</b></td>
                                            <td><asp:TextBox id="txtMultiplier" runat="server" columns="10" maxlength="10"></asp:TextBox></td>
                                        </tr>
                                    </table>
                                    <p style="text-align:center;">
                                        <asp:Button id="btnSave" runat="server" text="Save" cssclass="btnred" />
                                        <asp:Button id="btnCancel" runat="server" text="Cancel" cssclass="btnred" />
                                    </p>
                                </div>
                            </FormTemplate>
                            <Buttons>
                                <CC:PopupFormButton ControlId="btnSave" ButtonType="Postback"></CC:PopupFormButton>
                                <CC:PopupFormButton ControlId="btnCancel" ButtonType="ScriptOnly"></CC:PopupFormButton>
                            </Buttons>
                        </CC:PopupForm>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="CompanyName" HeaderText="Builder" SortExpression="CompanyName" />
                <asp:TemplateField HeaderText="Product">
                    <ItemTemplate>
                        <asp:Literal id="ltlProduct" runat="server"></asp:Literal>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Created" HeaderText="Request Date" SortExpression="Created" />
            </Columns>
        </CC:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>        
</div>

</CT:MasterPage>