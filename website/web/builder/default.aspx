<%@ Page Language="VB" AutoEventWireup="false" CodeFile="default.aspx.vb" Inherits="_default" %>

<CT:MasterPage runat="server" ID="CTMain">
<asp:PlaceHolder runat="server">
    <script type="text/javascript">
        function OpenDataReminder() {
            Sys.Application.remove_load(OpenDataReminder);
            var c = $get('<%=frmMonthlyDataReminder.ClientID %>').control;
            c._doMoveToCenter();
            c.Open();
        }
    </script>
</asp:PlaceHolder>

<CC:PopupForm ID="frmMonthlyDataReminder" runat="server" OpenMode="MoveToCenter" CssClass="pform" ShowVeil="true" VeilCloses="false" CloseTriggerId="btnOK" Width="300px">
    <FormTemplate>
        <div class="pckggraywrpr" style="margin:0px;">
            <div class="pckghdgred">Monthly Data Reminder</div>
            <p class="bold center" style="margin:5px;">
                Your Monthly Data records need to be updated.<br /><br />Please use the 'Monthly Data' module at the bottom of the dashboard to update any missing months.<br /><br />Thank you.
                <br />
                <br />
                <asp:Button id="btnOK" runat="server" cssclass="btnred" text="OK" />
            </p>            
        </div>
    </FormTemplate>
    <Buttons>
        <CC:PopupFormButton ControlID="btnOK" ButtonType="ScriptOnly" />
    </Buttons>
</CC:PopupForm>

</CT:MasterPage> 
