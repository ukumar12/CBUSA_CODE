<%@ Page Language="VB" AutoEventWireup="false" CodeFile="drops.aspx.vb" Inherits="order_drops" %>

<CT:MasterPage ID="CTMain" runat="server">
<asp:PlaceHolder runat="server">
    <script type="text/javascript">
        function OpenDropForm(drop,itemIdx) {
            var c = $get('<%=frmDrop.ClientID %>').control;
            c.get_input('hdnItemIdx').value = itemIdx;
            c.get_input('hdnOrderDropId').value = drop.id
            c.get_input('txtName').value = drop.name;
            c.get_input('dpDelivery').value = drop.delivery;
            //c.GetElementByServerId('dpDelivery_cal').value = drop.delivery;
            /*
            c.get_input('txtAddress1').value = drop.address1;
            c.get_input('txtAddress2').value = drop.address2;
            c.get_input('txtCity').value = drop.city;
            var drp = c.get_input('drpState');
            for (var i = 0; i < drp.options.length; i++) {
                if (drp.options[i].value == drop.state) {
                    drp.selectedIndex = i;
                    break;
                }
            }
            c.get_input('txtZip').value = drop.zip;
            */
            c.get_input('txtInstructions').value = drop.instructions ? drop.instructions : '';
            c.get_input('txtNotes').value = drop.notes ? drop.notes : '';
            c._doMoveToCenter();
            c.Open();
        }
        function SaveDropResult(res, ctxt) {
            if (res.error) return;
            var mgr = $get('<%=ctlDrops.ClientID %>').control;
            if (typeof res ==='object') {
                mgr.AddDrop(res.drop, res.itemIdx);
            }
            var c = $get('<%=frmDrop.ClientID %>').control;
            c.Close();
            window.location.href = window.location.href;
        }
    </script>
    
    <CC:PopupForm ID="frmDrop" runat="server" CssClass="pform" style="width:300px;" CloseTriggerId="btnCancel" Animate="true" ShowVeil="true" VeilCloses="false" ValidateCallback="true" ErrorPlaceholderId="spanError">
        <FormTemplate>
            <div class="pckggraywrpr" style="margin-bottom:0px;">
            <div class="pckghdgred">Edit Drop</div>
            <asp:HiddenField id="hdnOrderDropId" runat="server"></asp:HiddenField>
            <asp:HiddenField id="hdnItemIdx" runat="server"></asp:HiddenField>
            <table cellpadding="3" cellspacing="0" border="0">
                <tr>
                    <td colspan="2" style="padding:0px;margin:0px;"><span id="spanError" runat="server"></span></td>
                </tr>
                <tr>
                    <td><b>Name:</b></td>
                    <td><asp:TextBox id="txtName" runat="server" columns="25" maxlength="100"></asp:TextBox></td>
                </tr>
                <tr>
                    <td><b>Delivery Date:</b></td>
                    <td><asp:textbox ID="dpDelivery" runat="server"></asp:textbox></td>
                </tr>
                <tr>
                    <td><b>Instructions:</b></td>
                    <td><asp:TextBox id="txtInstructions" runat="server" textmode="MultiLine" columns="25" rows="3" maxlength="2000"></asp:TextBox></td>
                </tr>
                <tr>
                    <td><b>Notes:</b></td>
                    <td><asp:TextBox id="txtNotes" runat="server" textmode="MultiLine" columns="25" rows="5" maxlength="5000"></asp:TextBox></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button id="btnSave" runat="server" text="Save" cssclass="btnred" />
                        <asp:Button id="btnCancel" runat="server" text="Cancel" cssclass="btnred" />
                    </td>
                </tr>
            </table>
            <CC:RequiredFieldValidatorFront ID="rfvtxtName" runat='server' ControlToValidate="txtName" ErrorMessage="Name is required"></CC:RequiredFieldValidatorFront>
            <CC:DateValidatorFront ID="dvdpDelivery" runat="server" ControlToValidate="dpDelivery" ErrorMessage="Delivery Date is invalid"></CC:DateValidatorFront>
            </div>
        </FormTemplate>
        <Buttons>
            <CC:PopupFormButton ControlId="btnSave" ButtonType="Callback" ClientCallback="SaveDropResult" />
            <CC:PopupFormButton ControlId="btnCancel" ButtonType="ScriptOnly" />
        </Buttons>
    </CC:PopupForm>
    <CC:DropManager ID="ctlDrops" runat="server" OpenDropForm="OpenDropForm" />
    <p style="text-align:center;">
        <asp:Button id="btnSubmitOrder" runat="server" Text="Continue to Summary Page" cssclass="btnred" />&nbsp;
        <asp:Button id="btnCancelOrder" runat="server" Text="Cancel" cssclass="btnred" onclientclick="window.history.go(-1); return false;" />
        <asp:Button id="btnReturnOrders" runat="server" Text="Return to Order List" cssclass="btnred" />
    </p>
</asp:PlaceHolder>
</CT:MasterPage>
