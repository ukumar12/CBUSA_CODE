<%@ Control Language="VB" AutoEventWireup="false" CodeFile="BuilderMonthlyData.ascx.vb" Inherits="modules_BuilderMonthlyData" %>


<asp:PlaceHolder runat="server">
<script type="text/javascript">
    function OpenSavedPopup() {
        Sys.Application.remove_load(OpenSavedPopup);
        var c = $get("<%=frmSaved.ClientID %>").control;
        //c._doMoveToCenter();
        c.Open();
    }
</script>
</asp:PlaceHolder>

<CC:PopupForm ID="frmSaved" runat="server" CssClass="pform" ShowVeil="true" VeilCloses="true" CloseTriggerId="btnOK" Width="300px">
    <FormTemplate>
        <div class="pckggraywrpr" style="margin:0px;">
            <div class="pckghdgred">Monthly Data Updated</div>
            <p class="bold center">
                Your changes have been successfully saved.<br /><br />
                <asp:Button ID="btnOK" runat="server" Text="OK" CssClass="btnred" />
            </p>
        </div>
    </FormTemplate>
    <Buttons>
        <CC:PopupFormButton ControlID="btnOK" ButtonType="ScriptOnly" />
    </Buttons>
</CC:PopupForm>

<asp:UpdatePanel ID="upMonthlyData" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="pckgwhtwrpr themeprice">
            <div class="pckghdgred">
                <div class="smaller" style="float:right;">
                    <asp:RadioButtonList ID="rblView" runat="server" RepeatDirection="Horizontal" AutoPostBack="true">
                        <asp:ListItem Text="Current Month" Value="Current"></asp:ListItem>
                        <asp:ListItem Text="Missing Months" Value="Missing"></asp:ListItem>
                        <asp:ListItem Text="All Months" Value="All"></asp:ListItem>
                    </asp:RadioButtonList>
                </div>
                <div class="smaller" style="float:right;">
                    Show: 
                </div>
                Monthly Data
            </div>
            
            <table cellpadding="5" cellspacing="0" border="0" style="width:100%;">
                <tr>
                    <th>Month</th>
                    <th>Starts</th>
                    <th>Sales</th>
                    <th>Closings</th>
                    <th>60 Day Projected Starts</th>
                    <th>
                        <asp:Button id="btnSave" runat="server" CssClass="btnred" Text="Save Data" ValidationGroup="MonthlyData" />
                    </th>
                </tr>
                <tr id="trError" runat="server" visible="false">
                    <td colspan="6">
                        <CT:ErrorMessage ID="ctlErrors" runat="server"></CT:ErrorMessage>
                    </td>
                </tr>               
                <asp:Repeater ID="rptData" runat="server">
                    <ItemTemplate>
                        <tr id="trMonth" runat="server">
                            <td>
                                <asp:Literal ID="ltlMonth" runat="server"></asp:Literal>
                                <asp:HiddenField ID="hdnMonth" runat="server" />
                            </td>
                            <td>
                                <asp:TextBox ID="txtStartedUnits" runat="server" columns="5"></asp:TextBox>
                                <CC:IntegerValidator ID="ivStartedUnits" runat="server" ControlToValidate="txtStartedUnits" Display="Dynamic" ErrorMessage="Invalid Entry" ValidationGroup="MonthlyData"></CC:IntegerValidator>
                            </td>
                            <td>
                                <asp:TextBox ID="txtSoldUnits" runat="server" Columns="5"></asp:TextBox>
                                <CC:IntegerValidator ID="ivSoldUnits" runat="server" ControlToValidate="txtSoldUnits" Display="Dynamic" ErrorMessage="Invalid Entry" ValidationGroup="MonthlyData"></CC:IntegerValidator>
                            </td>
                            <td>
                                <asp:TextBox ID="txtClosedUnits" runat="server" Columns="5"></asp:TextBox>
                                <CC:IntegerValidator id="ivClosedUnits" runat="server" ControlToValidate="txtClosedUnits" Display="Dynamic" ErrorMessage="Invalid Entry" ValidationGroup="MonthlyData"></CC:IntegerValidator>
                            </td>
                            <td>
                                <asp:TextBox ID="txtUnsoldSpecs" runat="server" columns="5"></asp:TextBox>
                                <CC:IntegerValidator ID="ivUnsoldSpecs" runat="server" ControlToValidate="txtUnsoldSpecs" Display="Dynamic" ErrorMessage="Invalid Entry" ValidationGroup="MonthlyData"></CC:IntegerValidator>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </div>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="rblView" />
        <asp:AsyncPostBackTrigger ControlID="btnSave" />
    </Triggers>
</asp:UpdatePanel>