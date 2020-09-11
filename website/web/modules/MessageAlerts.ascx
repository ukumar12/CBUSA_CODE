<%@ Control EnableViewstate="False" Language="VB" AutoEventWireup="false" CodeFile="MessageAlerts.ascx.vb" Inherits="_default" %>


<div class="pckgwhtwrpr">
	<div class="pckghdgred">
		Messages and Alerts
	</div>
    <div class="amrow" id="divNoCurrentMessages" runat="server"><p class="ammessagetext">You have no current messages or alerts.</p></div>
    <asp:Repeater id="rptMessagesAlerts" runat="server" enableviewstate="False">
    <HeaderTemplate>
    </HeaderTemplate>
    <ItemTemplate>
        <div class="msgalrt" id="divAlert" runat="server">
		    <img src="/images/global/icon-alert-lg.gif" class="msgalrtimg" alt="Alert" />
		    <div class="msgalrtbdy">
			    <%#DataBinder.Eval(Container.DataItem, "Message")%>
		    </div>
	        <div class="clear">&nbsp;</div>
	    </div>
        <div class="msgread" id="divMsg" runat="server">
            <div class="msgttl"><a id="lnkMessageTitle" runat="server" class="amtitlelink" href="javascript:void(0);"><%#DataBinder.Eval(Container.DataItem, "Title")%>&#160;</a></div>
            <div class="msgdate">Posted:&#160;&#160;<%#DataBinder.Eval(Container.DataItem, "StartDate")%></div>
            <div class="msgabs"><%#DataBinder.Eval(Container.DataItem, "LeftMessage")%></div>
            <div id="divMessageDisplay" runat="server" class="window" style="border:1px solid #000;background-color:#fff;width:500px;">
                <div class="amheader">
                    <p class="amtitle"><%#DataBinder.Eval(Container.DataItem, "Title")%></p>
                </div>
                <div>
                    <p class="amposteddate"><%#DataBinder.Eval(Container.DataItem, "StartDate")%></p>
                    <p class="ammessagetext"><%#DataBinder.Eval(Container.DataItem, "Message")%></p>
                </div>
                <br />
                <asp:Button id="btnUnRead" runat="server" cssclass="btn" CommandName="UnRead" text="Mark As Unread" />
                <asp:Button id="btnSave" runat="server" cssclass="btn" CommandName="Read" text="Mark As Read" />
                <asp:Button id="btnDelete" runat="server" cssclass="btn" CommandName="Delete" text="Delete" />
                <asp:Button id="btnCancel" runat="server" cssclass="btn" text="Close" />
            </div>
            <CC:DivWindow ID="ctrlMessageDisplay" runat="server" TargetControlID="divMessageDisplay" TriggerId="lnkMessageTitle" ShowVeil="true" VeilCloses="true" />
        </div>
    </ItemTemplate>
    <FooterTemplate>
    </FooterTemplate>
    </asp:Repeater>	 
    <div class="right">
		<a id="lnkViewAll" runat="server" class="btnred" href="/" >View All</a>
	</div>
</div>
<div id="divDesigner" runat="server" EnableViewState="False">
    <div class="contentbottom">
        <span class="smaller">Return Count:</span>
        <asp:DropDownList Id="drpReturnCount" CausesValidation="False" runat="server" AutoPostBack="True" />
        <span class="smaller">Display View All Link:</span>
        <asp:DropDownList Id="drpDisplayViewAllLink" CausesValidation="False" runat="server" AutoPostBack="True" />
        <input type="hidden" id="hdnField" runat="server" name="hdnField" />
    </div>
</div>
<br />
