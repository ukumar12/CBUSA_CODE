<%@ Page Language="VB" AutoEventWireup="false" CodeFile="default.aspx.vb" Inherits="_default" %>
<%@ Register TagPrefix="CC" TagName="Notification" Src="~/controls/Notification.ascx" %>

<CT:MasterPage runat="server" ID="CTMain">
<div class="msgblock" id="divNotification" runat="server" visible="false">
    <div class="iconhldr"><img src="/images/global/icon-message.gif" style="width:34px; height:34px; border-style:none;" alt="" /></div>
    <div class="msghldr">
        <div class="multimsgbox">
            <div class="btnclose"><asp:ImageButton ID="btnClose" runat="Server" CausesValidation="False" ImageUrl="/images/global/btn-close.gif" style="width:20px; height:20px; border-style:none;" alt="" /></div>
            <div class="msgtxt">Your export file is being prepared.
            Due to the large number of records requested, this will take 1-5 minutes.
            An email will be sent when the export is complete and your file is ready.</div>
            <div style="clear:both;"></div>
        </div>
    </div>
    <div style="clear:both;"></div>
</div>
<CC:Notification ID="Notification1" runat="server" />
<asp:Panel ID="pnlSearch" cssclass="pckggraywrpr" DefaultButton="btnSearch" runat="server" style="width:575px;margin:10px auto;margin-top:0px;">
	<div class="pckghdgred">Filter Members</div>
	<span class="smaller">Please provide search criteria below</span>
	<table cellpadding="2" cellspacing="2">
		<tr>
			<td class="dtsearchlabel" style="vertical-align:inherit;">
				<b>Member Type:</b>
			</td>
			<td class="dtsearchfield" >
			    <asp:UpdatePanel id="upMemberType" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
				    <ContentTemplate>
				        <asp:RadioButtonList runat="server" ID="F_MemberType" RepeatDirection="Horizontal" AutoPostBack="true"></asp:RadioButtonList>
				    </ContentTemplate>
				</asp:UpdatePanel>
			</td>

			<td class="dtsearchlabel" >
				<b>Company Name:</b>
			</td>
			<td class="dtsearchfield " >
				<asp:textbox id="F_CompanyName" runat="server" Columns="20" MaxLength="50" />
			</td>
		</tr>

		<tr>
			<td class="dtsearchlabel" >
				<b>Contact First Name:</b>
			</td>
			<td class="dtsearchfield" >
				<asp:textbox id="F_ContactFirstName" runat="server" Columns="20" MaxLength="50" />
			</td>

			<td class="dtsearchlabel" >
				<b>Contact Last Name:</b>
			</td>
			<td class="dtsearchfield" >
				<asp:textbox id="F_ContactLastName" runat="server" Columns="20" MaxLength="50" />
			</td>
		</tr>

		<tr>
			<td class="dtsearchlabel" id="tdSupplyPhase" runat="server">
				<asp:UpdatePanel id="upSupplyPhaseTitle" runat="server" UpdateMode="Conditional">
				<Triggers>
				<asp:AsyncPostBackTrigger ControlID="F_MemberType" EventName="SelectedIndexChanged"></asp:AsyncPostBackTrigger>
				</Triggers>
				<ContentTemplate>
				<b id="SupplyPhase" runat="server">Supply Phase:</b>
				</ContentTemplate>
				</asp:UpdatePanel>
			</td>
			<td class="dtsearchfield ">
				<asp:UpdatePanel id="upSupplyPhaseDropDown" runat="server" UpdateMode="Conditional">
				<Triggers>
				<asp:AsyncPostBackTrigger ControlID="F_MemberType" EventName="SelectedIndexChanged"></asp:AsyncPostBackTrigger>
				</Triggers>
				<ContentTemplate>
				<asp:DropDownList ID="F_SupplyPhase" runat="server"></asp:DropDownList>
				</ContentTemplate>
				</asp:UpdatePanel>
			</td>

			<td colspan="2" align="right" class="">
				<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btnred" />
				<input class="btnred" type="button" value="Clear" onclick="window.location='default.aspx';return false;" />
			</td>
		</tr>
	</table>
</asp:Panel>

<div id="divWrapper" runat="server" style="width:100%; margin:auto;">

<asp:Repeater id="rptDirectory" runat="server" >
<ItemTemplate>
<div id="divWrapper2" runat="server" class="pckggraywrpr" style="width:100%;margin:10px auto;">
<div class="pckghdgred VendorButtons" id="divCompanyName" runat="server">
    
	<asp:LinkButton id="btnPreferredVendorDetails" runat="server" CssClass="btnblue" Style="float:right;" Text="Preferred Vendor Details" />
	<asp:LinkButton id="btnPreferredVendorPhotos" runat="server" CssClass="btnblue" Style="float:right; margin-right:5px;" Text="Preferred Vendor Gallery" />
	<asp:LinkButton id="btnPiqDetails" runat="server" cssclass="btnblue" style="float:right;" text="Incentive Program Details" />
    <div class="rating">
       <%-- <CC:StarRatingDisplay id="RatingDisplay" runat="server" />--%>
 <asp:Literal id="ltlStarRating" runat="server"></asp:Literal>
        <asp:Literal id="ltlReviewDetails" runat="server"></asp:Literal>
        </div>
	<asp:Literal id="ltlCompanyName" runat="server"></asp:Literal>
</div>
<table id="table" runat="server" class="dshbrdtbl tblprcng largest" style="width:100%;table-layout:fixed;">
    <tr>
    <td colspan="4">
    <asp:Literal runat="server" id="ltlLogo"></asp:Literal>
    </td>
    </tr>
	<tr>
		<td id="tdWebsiteURL" runat="server" colspan="2" class="dttext">
			<a target="_blank" href="" id="hrefWebsiteURL" runat="server" ></a><br />
			<asp:Literal id="tdAddressInfo" runat="server"></asp:Literal>
		</td>
		<td colspan="2" id="tdPrimaryContact" runat="server">&nbsp;</td>
	</tr>
       <tr>
		<td colspan ="2" id="tdMarkets" runat="server" class="dttext">
			<b class="smaller nopad">Markets:</b><br />
			<span id="spanMarkets" runat="server"></span>
		</td>
	    
    </tr>
	<tr>
		<td colspan ="2" id="tdSupplyPhase" runat="server" class="dttext">
			<b class="smaller nopad">Supply Phase(s):</b><br />
			<span id="spanSupplyPhase" runat="server"></span>
		</td>
	    <td colspan="2" id="trOtherContacts" runat="server">
		    <p align="center">
		        <a style="cursor:pointer;" onclick='<%# "$(""#"& Container.FindControl("divContacts").ClientID &""").slideToggle(""fast"",null);" %>'>View All Contacts</a>
		    </p>
	        <div id="divContacts" runat="server" style="padding:20px; background-color:#fff;display:none;">
	        </div>
		</td>
    </tr>

 

</table>
</div>
</ItemTemplate>
</asp:Repeater>
<CC:Navigator ID="ctlNavigate" runat="server" MaxPerPage="5" PagerSize="5" />
<asp:Panel id="pnlNoResults" runat="server" visible="false" CssClass="center">There are no results that match your search criteria.</asp:Panel>

</div>

<asp:Panel ID="pnlPrint" runat="server">
    <div style="text-align: center;">
        <input type="button" class="btnred" value="Print This Page" onclick="window.open('<%=PrintUrl%>', 'PrintPage', ''); return false;" />
        <asp:Button id="btnExport" runat="server" text="Export to Excel" cssclass="btnred" />
    </div>    
</asp:Panel>

</CT:MasterPage>

