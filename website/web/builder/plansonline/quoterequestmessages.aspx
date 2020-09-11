<%@ Page Language="VB" AutoEventWireup="false" Title="Plans Online - Bid Status Messages" CodeFile="quoterequestmessages.aspx.vb" Inherits="POQuoteRequestMessages"  %>
<%@ Register TagName="LimitTextBox" TagPrefix="CC" Src="~/controls/LimitTextBox.ascx" %>

<CT:MasterPage ID="CTMain" runat="server">
<style>
    .myBidSummary
    {
        float: right;
        padding: 15px;
        background-color: white;
        margin: 15px 25px 25px 25px;
        border-color: #c2c2c2;
        border-width: 1px;
        border-style: solid;
    }
</style>
<script type="text/javascript">
    function ShowDetails() {
        document.getElementById("tblDetails").style.display = "block";
        document.getElementById("aHideDetails").style.display = "block";
        document.getElementById("aShowDetails").style.display = "none";
    }
    function HideDetails() {
        document.getElementById("tblDetails").style.display = "none";
        document.getElementById("aHideDetails").style.display = "none";
        document.getElementById("aShowDetails").style.display = "block";
    }
    </script>

<div class="pckgwrpr bggray">
<div class="pckghdgltblue">
    Plans Online > <a href='default.aspx' >Projects</a> > <a href='quotes.aspx'  id="hplBidRequestHeader" runat="server">Bid Requests</a> > <a href='quoterequests.aspx' id="hplBidStatusHeader" runat="server">Bid Comparison Matrix</a> > <span style="font-size:20px;">Bid Request Details</span>
</div>
<div class="pckgbdy">

<div class="pckgltgraywrpr">
    <div class="pckghdgred" style="height:15px;">
        <span style="float:left;">Bid Request Details</span>
        <span style="float:right;"><a href="#" id="aShowDetails" style="display:none; color:White;" onclick="ShowDetails();">Show Details</a>
        <a href="#" id="aHideDetails" style="color:White;" onclick="HideDetails();">Hide Details</a></span>
    </div>
    <div class="myBidSummary">
        <h3>My Bid summary</h3>
            <table border="0" cellspacing="1" cellpadding="2">
                <tr valign="top">
                <td class="required" width="110" nowrap><b>Vendor:</b></td>
		        <td class="field" width="300">
		            <asp:literal id="ltlVendor" runat="server" />
		        </td>
		        </tr>
                <tr valign="top">
                    <td class="required"><b>Vendor Contact:</b></td>
		            <td class="field"><asp:literal id="ltlVendorContact" runat="server" /></td>
                </tr>
                <tr valign="top">
                    <td class="required"><b>Expiration Date:</b></td>
                    <td class="field">
                        <asp:literal id="ltlExpiration" runat="server" />
                    </td>
                </tr>
                <tr valign="top"> 
                    <td class="required"><b>date My Company Can Start:</b></td>
		            <td class="field"><asp:literal id="ltlStartDate" runat="server" /></td>
		        </tr>
		        <tr valign="top">
                    <td class="required"><b>Completion Time:</b></td>
                    <td class="field">
                        <asp:literal id="ltlCompletionTime" runat="server" />
                    </td>
                </tr>
                <tr valign="top">
                    <td class="required"><b>Total:</b></td>
                    <td class="field">
                        <asp:literal id="ltlTotal" runat="server" />
                    </td>
                </tr>
                <tr valign="top">
                    <td class="required"><b>Payment Terms:</b></td>
                    <td class="field" colspan="3"><asp:literal id="ltlPaymentTerms" runat="server" /></td>
                </tr>
            </table>
    </div>
    <div id="tblDetails">
    <asp:UpdatePanel id="upDetails" runat="server" updatemode="conditional">
    <ContentTemplate>
        <table border="0" cellspacing="1" cellpadding="2" width="40%" style="padding:10px 0px 0px 5px;">
	        <tr valign="top">
		        <td class="required" width="110" nowrap><b>Project:</b></td>
		        <td class="field" width="300"><asp:literal id="ltlProject" runat="server" /></td>
	        </tr>
	        <tr valign="top">
                <td class="required"><b>Builder Contact:</b></td>
                <td class="field"><asp:literal id="ltlBuilderContact" runat="server" /></td>
            </tr>
            <tr valign="top">
                <td class="required"><b>Bid Request Name:</b></td>
                <td class="field"><asp:literal id="ltlQuote" runat="server" /></td>
            </tr>
            <tr valign="top">
                <td class="required"><b>Bid Request #:</b></td>
                <td class="field"><asp:literal id="ltlQuoteId" runat="server" /></td>
            </tr>
            <tr valign="top">
                <td class="required"><b>Bid Request Status:</b></td>
                <td class="field"><b><asp:literal id="ltlStatus" runat="server" /></b> (<asp:literal id="ltlStatusDate" runat="server" />)</td>
            </tr>
            <tr valign="top">
                <td class="required"><b>Latest Message Status:</b></td>
                <td class="field">
                    <b><asp:literal id="ltlRequestStatus" runat="server" /></b> (<asp:literal id="ltlRequestStatusDate" runat="server" />)
                </td>
            </tr>
            <tr valign="top">
                <td class="required"><b>Deadline:</b></td>
                <td class="field"><asp:literal id="ltlQuoteDeadline" runat="server" /></td>
            </tr>
            <asp:PlaceHolder id="phawarded" runat="server" visible="false">
	            <tr valign="top">
		            <td class="optional"><b>Awarded To Vendor:</b></td>
		            <td class="field"><asp:literal id="ltlAwardedToVendor" runat="server" /> (<asp:literal id="ltlAwardedDate" runat="server" />)</td>
		        </tr>
                <tr valign="top">
		            <td class="optional"><b>Awarded Total:</b></td>
		            <td class="field"><asp:literal id="ltlAwardedTotal" runat="server" /></td>
	            </tr>
	        </asp:PlaceHolder>
            <tr valign="top">
                <td class="required"><b>Supply Phase(s):</b></td>
                <td class="field">
                    <asp:literal id="ltlVendorCategory" runat="server" />
                </td>
            </tr>
	        <tr valign="top">
		        <td class="required"><b>Instructions:</b></td>
		        <td class="field" colspan="3"><asp:literal id="ltlInstructions" runat="server" /></td>
	        </tr>
	        </table>
	        <table border="0" cellspacing="1" cellpadding="2" width="100%">
        	<tr valign="top">
		        <td colspan="2" width="50%">
		            <div class=" pckgltgraywrpr">
                                <div class="pckghdgblue">Builder Documents</div>
                                <div style="text-align:left;">
                                    <div>
                                        <asp:Repeater id="rptDocuments" runat="server">
                                        <HeaderTemplate>
                                            <table cellspacing="0" cellpadding="0" border="0" class="dochdrtbl">
			                                    <tr>
			                                        <th>Document</th>
			                                        <th>Uploaded</th>
			                                    </tr>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr class='<%#IIf(Container.ItemIndex Mod 2 = 1, "alternate", "row") %>'>
                                                <td class="dcrow" id="tdMessageRow" runat="server" >
                                                    <a id="lnkMessageTitle" runat="server" class="dctitlelink" target="_blank" href="/" ><%#DataBinder.Eval(Container.DataItem, "Title")%></a>&#160;
                                                </td>
                                                <td><%#FormatDateTime(DataBinder.Eval(Container.DataItem, "Uploaded"), DateFormat.ShortDate)%></td>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                        </table>
                                        </FooterTemplate>
                                        </asp:Repeater>	
                                        <div class="dcrow" id="divNoCurrentDocuments" runat="server"><p class="dcmessagetext">There are no documents available.</p></div> 
	                                    
                                    </div>
                                </div>
                            </div>
                        </div>
                    </td>
		            <td colspan="2" width="50%">
                        <div class=" pckgltgraywrpr ">
                                <div class="pckghdgblue">Vendor Documents</div>
                                <div style="text-align:left;" >
                                    <div>
                                        <asp:Repeater id="rptVendorDocuments" runat="server">
                                        <HeaderTemplate>
                                            <table cellspacing="0" cellpadding="0" border="0" class="dochdrtbl">
			                                    <tr>
			                                        <th>Document</th>
			                                        <th>Uploaded</th>
			                                    </tr>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr class='<%#IIf(Container.ItemIndex Mod 2 = 1, "alternate", "row") %>'>
                                                <td class="dcrow" id="tdMessageRow" runat="server" >
                                                    <a id="lnkMessageTitle" runat="server" class="dctitlelink" target="_blank" href="/" ><%#DataBinder.Eval(Container.DataItem, "Title")%></a>&#160;
                                                </td>
                                                <td><%#FormatDateTime(DataBinder.Eval(Container.DataItem, "Uploaded"), DateFormat.ShortDate)%></td>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                        </table>
                                        </FooterTemplate>
                                        </asp:Repeater>	
                                        <div class="dcrow" id="divNoVendorDocuments" runat="server"><p class="dcmessagetext">There are no documents available.</p></div> 
	                                    
                                    </div>
                                </div>
		        </td>
	        </tr>
        </table>
    </div>
</div>
    </ContentTemplate>
</asp:UpdatePanel>
<p></p>
<asp:UpdatePanel id="upRequestInfo" runat="server" updatemode="conditional">
    <ContentTemplate>
    <div class="pckgltgraywrpr">
        <div class="pckghdgred">
            <a name="rfi"></a><span style="float:left;">Request for Information</span>
        </div>
        <table border="0" cellspacing="1" cellpadding="2" width="100%">
            <tr valign="top" id="trThread" runat="server">
		        <td>
		            <div class=" center">
                        <div class="pckghdgblue" >
                            <span style="float:left;">RFI Topics</span>
                            <span style="float:right;"><asp:linkbutton id="lnkAddThread" runat="server" class="btnblue" Text="Add New Topic" /></span>
                        </div>
                        <center>
                        <div id="divAddThread" class="pckggraywrpr" runat="server" visible="false" style="width:350px;">
                            
                            <div class="pckghdgred" >Add New Topic</div>
                            <div class="dcmain">
                                <table>
                                    <tr valign="top">
                                        <td class="required" width="100"><span class="red">*</span>Topic:<br /><span class="smallest red">(No action will be taken if left empty.)</span></td>
                                        <td class="field"><asp:TextBox  runat="server" columns="25" maxlength="250" Width="250" id="txtThread" /></td>
                                    </tr>
                                </table>
                                <br />
                                <p align="center">
                                    <asp:Button id="btnAddThread" runat="server" cssclass="btnblue" text="Save" CausesValidation="false" />
                                    <asp:Button id="btnCancelThread" runat="server" cssclass="btnblue" text="Close" CausesValidation="false" />        
                                </p>
                            </div>
                            
                        </div>
                        </center>
                        <div style="text-align:left; background-color:White;" >
                            <CC:GridView Width="100%" id="gvThreads" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="False" AllowSorting="True" HeaderText="" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom" class="tblcompr automargin" >
                                <AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
                                <RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:Button runat="server" CommandName="GetMessages" cssclass="btnred" Text="Show Posts" ID="lnkMessages" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Thread" HeaderText="Topic"></asp:BoundField>
                                    <asp:BoundField DataField="CreateDate" HeaderText="Added On" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
                                    <asp:BoundField DataField="CreatedBy" HeaderText="Added By"></asp:BoundField>
                                    <asp:TemplateField>
			                            <ItemTemplate>
			                                <CC:ConfirmImageButton CommandName="Remove" message="Are you sure that you want to remove this Topic? All posts will be deleted." runat="server" ID="lnkDelete" ImageUrl="/images/admin/delete.gif" />
			                            </ItemTemplate>
		                            </asp:TemplateField>
                                </Columns>
                            </CC:GridView>
                        </div>
                    </div>
                </td>
                </tr>
                <tr id="trPost" runat="server" visible="false">
	            <td>
                    <div > 
                        <div class="pckghdgblue" >
                            <span style="float:left;">Posts</span>
                            <span style="float:right;"><asp:linkbutton id="lnkAddPost" runat="server" visible=false class="btnblue" Text="Add New Post" /></span>
                        </div>
                            <div style="text-align:left;" >
                                <center>
                                    <p id="pThread" runat="server" visible="false"><b>Topic: </b><asp:Literal id="ltlThread" runat="server"></asp:Literal>&nbsp;<asp:linkbutton id="lnkBackThread" runat="server" class="btnblue" Text="back to Topics" /></p>
                                    <div id="divAddPost" runat="server" visible="false" style="width:350px;" class="pckggraywrpr">
                                        
                                        <div class="pckghdgred" >Add New Post</div>
                                        <div class="dcmain">
                                            <table>
                                                <tr valign="top">
                                                    <td class="required" width="100"><span class="red">*</span>Post:<br /><span class="smallest red">(No action will be taken if left empty.)</span></td>
                                                    <td class="field"><CC:LimitTextBox  runat="server" Limit="500" columns="25" Rows="3" maxlength="500" Width="250" id="txtPost" TextMode="MultiLine"/></td>
                                                </tr>
                                            </table>
                                            <br />
                                            <p align="center">
                                                <asp:Button id="btnAddPost" runat="server" cssclass="btnblue" text="Save" CausesValidation="false" />
                                                <asp:Button id="btnCancelPost" runat="server" cssclass="btnblue" text="Close" CausesValidation="false" />        
                                            </p>
                                        </div>
                                    </div>
                                </center>
                                <asp:HiddenField id="hdnThreadId" runat="server"></asp:HiddenField>
                                
                                <div style="text-align:left;" >
                                    <asp:Literal id="ltlNoThread" runat="server" text="Select thread to view posts."></asp:Literal>
                                    <CC:GridView Width="100%" id="gvPosts" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="False" AllowSorting="True" HeaderText="" EmptyDataText="There are no posts in this Topic." AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom" class="tblcompr automargin">
                                        <AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
                                        <RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
                                        <Columns>
                                            <asp:BoundField DataField="Message" HeaderText="Message"></asp:BoundField>
                                            <asp:BoundField DataField="CreateDate" HeaderText="Posted On" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
                                            <asp:BoundField DataField="CreatedBy" HeaderText="Posted By"></asp:BoundField>
                                            <asp:TemplateField>
			                                    <ItemTemplate>
			                                        <CC:ConfirmImageButton CommandName="Remove" message="Are you sure that you want to remove this post?" runat="server" ID="lnkDelete" ImageUrl="/images/admin/delete.gif" />
			                                    </ItemTemplate>
		                                    </asp:TemplateField>
                                        </Columns>
                                    </CC:GridView>
                                </div>
                            </div>
                        </div>
                    </div>
		        </td>
	        </tr>
        </table>
    </div>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdatePanel id="upMessages" runat="server" updatemode="conditional">
    <ContentTemplate>
    <div class="pckgltgraywrpr">
        <div class="pckghdgred">
            <span style="float:left;">Private Messaging to Vendor</span>
            <span style="float:right;">
                <asp:linkbutton id="lnkAddMessage" runat="server" class="btnblue" Text="Add New Message" />
            </span>
        </div>
            <p></p>
            <center>
                <div class="pckgltgraywrpr  center" id="divMsg" runat="server" visible="false" style="width:600px;">
                    <div class="pckghdgblue">Bid Request Message Update Confirmation</div>
                    <div style="text-align:center;">
                        <p class="bold"><asp:Literal id="ltlMsg" runat="server" /></p>
                    </div>
                </div>
            
            
                <div id="divAddMessage" runat="server" visible="false" style="width:650px;" class="pckggraywrpr">
                    <div class="pckghdgred" >Add Message</div>
                    <div class="dcmain">
                        <table>
                            <tr valign="top">
                                <td class="required" width="100"><span class="red">*</span>Message:<br /><span class="smallest red">(No action will be taken if left empty.)</span></td>
                                <td class="field"><CC:LimitTextBox  runat="server" Limit="500" columns="50" Rows="6" maxlength="500" Width="500" id="txtMessage" TextMode="MultiLine"/></td>
                            </tr>
                        </table>
                        <br />
                        <p align="center">
                            <asp:Button id="btnAddMessage" runat="server" cssclass="btnblue" text="Send Message" CausesValidation="false" />
                            <asp:Button id="btnRequestInfo" runat="server" cssclass="btnblue" text="Request Information" CausesValidation="false" visible="false" />
                            <asp:Button id="btnCancelMessage" runat="server" cssclass="btnblue" text="Cancel" CausesValidation="false" />        
                        </p>
                    </div>
                </div>
            </center>
            <%--<CC:DivWindow ID="ctrlAddMessage" runat="server" TargetControlID="divAddMessage" TriggerId="lnkAddMessage" CloseTriggerId="btnCancelMessage" ShowVeil="true" VeilCloses="true" />--%>

            
                    <CC:GridView id="gvList" Width="100%" CellSpacing="2" CellPadding="2" runat="server" AllowPaging="False" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="No Messages (New Bid Request)" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom" class="tblcompr automargin" >
	                    <AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	                    <RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	                    <Columns>
		                    <asp:TemplateField>
			                    <ItemTemplate>
			                    <asp:LinkButton runat="server" id="btnRead" CommandName="Read" Text="<b>Mark as Read</b>" />
			                    </ItemTemplate>
		                    </asp:TemplateField>
		                    <asp:ImageField SortExpression="IsRead" DataImageUrlField="IsRead" HeaderText="Is Read" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
		                    <asp:BoundField SortExpression="FromUser" DataField="FromUser" HeaderText="From"></asp:BoundField>
		                    <asp:BoundField SortExpression="FromName" DataField="FromName" HeaderText="User"></asp:BoundField>
		                    <asp:BoundField SortExpression="MessageQuoteStatus" DataField="MessageQuoteStatus" HeaderText="Status"></asp:BoundField>
		                    <asp:BoundField SortExpression="MessageQuoteTotal" DataField="MessageQuoteTotal" HeaderText="Total" DataFormatString="{0:c}"></asp:BoundField>
		                    <asp:BoundField SortExpression="MessageQuoteExpirationDate" DataField="MessageQuoteExpirationDate" HeaderText="Expires" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		                    <asp:BoundField SortExpression="CreateDate" DataField="CreateDate" HeaderText="Message Date" HTMLEncode="False"></asp:BoundField>
		                    <asp:TemplateField HeaderText="Message">
		                        <ItemStyle width="350" />
			                    <ItemTemplate>
			                        <span style="font-weight:normal;"><asp:literal runat="server" id="ltlMessage"  /></span>
			                    </ItemTemplate>
		                    </asp:TemplateField>
	                    </Columns>
                    </CC:GridView>
                
    </div>
    <div class="pckgltgraywrpr">
        <div class="pckghdgred">
            <span style="float:left;">Bid Request Actions</span>
            <span style="float:right;">
                <a name="award"></a>
                <CC:OneClickButton id="btnAwardDiv" runat="server" Text="Award Bid" CausesValidation="false" cssClass="btnred"></CC:OneClickButton>
                <CC:OneClickButton id="btnConfirmDecline" runat="server" Text="Decline Bid" cssClass="btnred" CausesValidation="false"></CC:OneClickButton>
            </span>
        </div>
        <p></p>
        <center>          
                <div id="divAwardBid" runat="server" visible="false" style="width:650px;" class="pckggraywrpr">
                    <div class="pckghdgred" >Award bid</div>
                    <div class="dcmain">
                        <table>
                            <tr valign="top">
                                <td class="required" width="100"><span class="red">*</span>Message:</td>
                                <td class="field"><CC:LimitTextBox  runat="server" Limit="500" columns="50" Rows="6" maxlength="500" Width="500" id="txtAwardBidMessage" TextMode="MultiLine"/></td>
                            </tr>
                        </table>
                        <br />
                        <p align="center">
                            <CC:ConfirmButton id="btnAward" runat="server" Text="Award Bid" Message="Are you sure that you want to award this bid request to this vendor? This will send the Bid Request Awarded to the current vendor and the Bid Request Declined messages to all other vendors." ValidationGroup="AwardBidGroup" cssClass="btnred"></CC:ConfirmButton>
                            <asp:Button id="btnCancelAward" runat="server" cssclass="btnblue" text="Cancel" CausesValidation="false" />        
                        </p>
                    </div>
                </div>
            </center>

        <p></p>
        <center>          
            <div id="divDeclineBid" runat="server" visible="false" style="width:650px;" class="pckggraywrpr">
                <div class="pckghdgred" >Decline bid</div>
                <div class="dcmain">
                    <table>
                        <tr valign="top">
                            <td class="required" width="100"><span class="red">*</span>Message:</td>
                            <td class="field"><CC:LimitTextBox  runat="server" Limit="500" columns="50" Rows="6" maxlength="500" Width="500" id="txtDeclineMessage" TextMode="MultiLine"/></td>
                        </tr>
                    </table>
                    <br />
                    <p align="center">
                        <CC:ConfirmButton id="btnDecline" runat="server" Text="Decline Bid" Message="Are you sure that you want to decline this bid request?" ValidationGroup="DeclineGroup" cssClass="btnred"></CC:ConfirmButton>
                        <asp:Button id="btnCancelDeclineBid" runat="server" cssclass="btnblue" text="Cancel" CausesValidation="false" />        
                    </p>
                </div>
            </div>
        </center>
    </div>
    
   
                <div class="pckghdgltblue">
    Plans Online > <a href='default.aspx' >Projects</a> > <a href='quotes.aspx' id="hplBidRequestFooter" runat="server" >Bid Requests</a> > <a href='quoterequests.aspx'  id="hplBidStatusFooter" runat="server">Bid Comparison Matrix</a> > <span style="font-size:20px;">Bid Request Details</span>
</div>
    </ContentTemplate>
</asp:UpdatePanel>
</div>



</div>
</CT:MasterPage>

