<%@ Control Language="VB" AutoEventWireup="false" CodeFile="InfoBox.ascx.vb" Inherits="controls_InfoBox" %>

<img src="/images/utility/question.gif" alt="Click for Help" style="width:28px;height:24px;padding:0px;margin:0px;cursor:pointer;border:none;" onclick="OpenInfo(event,'<%=frmInfo.ClientID %>');" />
<CC:PopupForm ID="frmInfo" runat="server" CssClass="pform" CloseTriggerId="spanClose" VeilCloses="true" ShowVeil="true">
    <FormTemplate>
        <div class="pckggraywrpr" style="margin-bottom:0px;">
            <div class="pckghdgred" style="height:15px;"><span id="spanClose" runat="server" class="bold smaller" style="cursor:pointer;float:right;margin-right:10px;">CLOSE</span><asp:Literal ID="ltlTitle" runat="server"></asp:Literal></div>
        </div>
        <div style="padding:10px;text-align:left;" class="largest">
            <asp:Literal ID="ltlContent" runat="server"></asp:Literal>
        </div>
    </FormTemplate>
</CC:PopupForm>
