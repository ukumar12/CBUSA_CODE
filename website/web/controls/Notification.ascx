<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Notification.ascx.vb" Inherits="Notification" %>
<asp:UpdatePanel runat="Server" id="pnlMessage" UpdateMode="conditional" RenderMode="Inline" >
<ContentTemplate>
<div class="msgblock" id="divNotification" runat="server" >
	<div class="iconhldr"><img src="/images/global/icon-message.gif" style="width:34px; height:34px; border-style:none;" alt="" /></div>
	<div class="msghldr">

    <asp:Repeater ID="rptNotification" runat="server">
    <ItemTemplate>
		<div class="multimsgbox">
			<div class="btnclose"><asp:ImageButton ID="btnClose" CommandName="Close" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "NotificationId")%>' runat="Server" CausesValidation="False" ImageUrl="/images/global/btn-close.gif" style="width:20px; height:20px; border-style:none;" alt="" /></div>
			<div class="msgtxt"><%#DataBinder.Eval(Container.DataItem, "Message")%></div>
			<div style="clear:both;"></div>
		</div>
    </ItemTemplate>
    </asp:Repeater>

	</div>
	<div style="clear:both;"></div>
</div>
</ContentTemplate>
</asp:UpdatePanel> 


