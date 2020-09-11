<%@ Page Language="VB" AutoEventWireup="false" Title="Plans Online - Bid Status Messages" EnableEventValidation="false"    CodeFile="quoterequestmessages.aspx.vb" Inherits="POQuoteRequestMessages" %>

<%@ Register TagName="LimitTextBox" TagPrefix="CC" Src="~/controls/LimitTextBox.ascx" %>
<%@ Register Src="~/controls/Documents/MultiFileUploadNew.ascx" TagName="MultiUploadDocument"
    TagPrefix="CC" %>
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
        max-width: 400px;
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
    function ShowAddMessage() {
        document.getElementById("divAddMessage").style.display = "block";
    }
    function ToggleDecline() {
        var divDecline = document.getElementById("divDeclineBid");
        if (divDecline.style.display == "block") {
            divDecline.style.display = "none";
        } else {
            divDecline.style.display = "block";
        }
        return false;
    }
    </script>

<div class="pckgwrpr bggray">
<div class="pckghdgltblue">
    Plans Online > <a href='default.aspx'>Bid Requests</a> > <span style="font-size:20px;">Bid Request Details</span>
</div>
<div class="pckgbdy">

    <div class="pckgltgraywrpr">
        <div class="pckghdgred" style="height:15px;">
            <span style="float:left;">Bid Request Details</span>
            <span style="float:right;font-size:11px;"><a href="#" id="aShowDetails" style="display:none; " onclick="ShowDetails();">Show Details</a>
            <a href="#" id="aHideDetails"  onclick="HideDetails();">Hide Details</a></span>
        </div>
        
        <div id="tblDetails">
        <div class="myBidSummary">
	            <h3>My Bid summary</h3>
	            <table border="0" cellspacing="1" cellpadding="2">
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
            <asp:UpdatePanel id="upDetails" runat="server" updatemode="conditional">
                <ContentTemplate>
                    <table border="0" cellspacing="1" cellpadding="2" width="50%" style="margin-left:15px">
	                    <tr valign="top">
		                    <td class="required" width="100" nowrap><b>Project:</b></td>
		                    <td class="field" width="300"><asp:literal id="ltlProject" runat="server" /></td>
	                    </tr>
	                    <tr valign="top">
	                        <td class="required" width="110" nowrap><b>Builder:</b></td>
	                        <td class="field" width="300">
	                            <asp:literal id="ltlBuilder" runat="server" />
	                        </td>
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
		                    <td class="field"><b><asp:literal id="ltlStatus" runat="server" /></b> <%--(<asp:literal id="ltlStatusDate" runat="server" />)--%></td>
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
                    <div class="pckgbdy">
                        <div class="pckghdgclr" >Builder Documents</div>
                        <div style="text-align:left; background-color:White;">
                            <asp:Repeater id="rptDocuments" runat="server">
                                <HeaderTemplate>
                                    <table cellspacing="0" cellpadding="0" border="0" class="dochdrtbl" style="width:100%;margin:0px">
                                        <tr>
                                            <th>Document</th>
                                            <th>Uploaded</th>
                                        </tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr class='<%#iif(Container.ItemIndex mod 2 = 1,"alternate","row") %>'>
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
                           
                </ContentTemplate>
            </asp:UpdatePanel>
            
        </div>
    </div>
        
    <p></p>
    <asp:UpdatePanel id="upRequestInfo" runat="server" updatemode="conditional">
    <ContentTemplate>
    <div class="pckgltgraywrpr">
        <div class="pckghdgred">
            <a name="rfi"></a><span style="float:left;">Request for Information</span>
        </div>
        <table border="0" cellspacing="0" cellpadding="0" width="100%">
            <tr valign="top" id="trThread" runat="server">
		        <td >
		            <div >
                        <div class="pckghdgblue">
                            <span style="float:left;">RFI Topics</span>
                            <span style="float:right;"><asp:linkbutton id="lnkAddThread" runat="server" class="btnblue" Text="Add New Topic" /></span>
                        </div>
                        <center>
                        <div id="divAddThread" runat="server" visible="false" style="width:350px;" class="pckggraywrpr">
                            
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
                        <div style="text-align:left;"  >
                            <CC:GridView Width="100%" id="gvThreads" CellSpacing="2" CellPadding="2" class="tblcomprlen" style="margin:0px" runat="server" PageSize="50" AllowPaging="False" AllowSorting="True" HeaderText="" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
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
                        <span>All communication using the RFI section is made public to the builder and <b><u>all vendors</u></b> that are participating in this bid request</span>
                    </div>
                </td>
                </tr>
                <tr id="trPost" runat="server" visible="false">
	            <td >
                    <div> 
                        <div class="pckghdgblue">
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
                                
                                <div style="text-align:left; background-color:White; margin-top:5px; margin-bottom:5px;" >
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
			                                        <CC:ConfirmImageButton CommandName="Remove" message="Are you sure that you want to remove this post?" runat="server" ID="ConfirmImageButton1" ImageUrl="/images/admin/delete.gif" />
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
                    <span style="float:left;">Private Messaging to Builder</span>
                    <span style="float:right;"> 
                        <a name="submitbid"><asp:linkbutton id="lnkSendMessage" runat="server" class="btnblue" Text="Create a Message" /></a>
                    </span>
                </div>
                <p></p>
                <center>
                    <div class="pckgltgraywrpr center" id="divAddMsg" runat="server" visible="false" style="width:600px;">
                        <div class="pckghdgblue">Message Update Confirmation</div>
                        <div style="text-align:center;">
                            <p class="bold"><asp:Literal id="ltlMsg2" runat="server" /></p>
                        </div>
                    </div>
                
                    <div id="divSendMessage" runat="server" visible="false" style="width:750px;" class="pckggraywrpr">
                        <div class="pckghdgred" >Submit a Message</div>
                        <div class="dcmain">
                            <table>
                                <tr valign="top">
                                    <td class="required" width="110">Message:<br /></td>
                                    <td class="field"><CC:LimitTextBox  runat="server" Limit="500" columns="50" Rows="6" maxlength="500" Width="500" id="txtPrivateMessage" TextMode="MultiLine"/></td>
                                </tr>
                            </table>
                            <br />
                            <p align="center">
                                <asp:Button id="btnAddMessage2" runat="server" cssclass="btnblue" text="Send Message" />
                                <asp:Button id="btnCancelMessage2" runat="server" cssclass="btnblue" text="Cancel" CausesValidation="false" />        
                            </p>
                        </div>
                    </div>
                </center>
                <%--<CC:DivWindow ID="ctrlAddMessage" runat="server" TargetControlID="divAddMessage" TriggerId="lnkAddMessage" CloseTriggerId="btnCancelMessage" ShowVeil="true" VeilCloses="true" />--%>
                
                <CC:GridView id="gvList" Width="100%" CellSpacing="2" CellPadding="2" class="tblcomprlen" style="margin:0px" runat="server" AllowPaging="False" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="No Messages (New Bid Request)" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
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
            
            
        </ContentTemplate>
    </asp:UpdatePanel>
    
    <div class="pckgltgraywrpr">
                <%--<asp:UpdatePanel id="upCancelBid" runat="server" updatemode="conditional">
                <ContentTemplate>--%>
    
                <div class="pckghdgred">
                    <span style="float:left;">Bid Request to Builder</span>
                    <span style="float:right;">
                        <a name="submitbid"><asp:linkbutton id="lnkAddMessage" runat="server" class="btnblue" visible="false" Text="Submit a Bid" /></a>
                        
                        <input type="button" ID="btnDeclineDiv" class="btnred" onclick="ToggleDecline();" value="Decline to Bid" />
                    </span>
                </div>
                <p></p>
                <center>
                    <div class="pckgltgraywrpr center" id="divMsg" runat="server" visible="false" style="width:600px;">
                        <div class="pckghdgblue">Bid Request Message Update Confirmation</div>
                        <div style="text-align:center;">
                            <p class="bold"><asp:Literal id="ltlMsg" runat="server" /></p>
                        </div>
                    </div>
                    <div id="divDeclineBid" style="display:none;width:750px;" class="pckggraywrpr">
                        <div class="pckghdgred" >Decline to Bid</div>
                        <div class="dcmain">
                            <table>
                                <tr>
                                <td class="optional" width="110">Message:<br /></td>
                                    <td class="field"><CC:LimitTextBox  runat="server" Limit="500" columns="50" Rows="6" maxlength="500" Width="500" id="ltbDeclineMessage" TextMode="MultiLine"/></td>
                                </tr>
                            </table>
                            <p align="centre">
                            <CC:ConfirmButton id="btnDecline" runat="server" Text="Decline to Bid" Message="Are you sure that you want to decline this bid request?" cssClass="btnred"></CC:ConfirmButton>
                            <input type="button" ID="btnDeclineDivClose" class="btnred" onclick="ToggleDecline();" value="Cancel" />
                                </p>
                        </div>
                        
                    </div><br /><br />
                <%--</ContentTemplate>
    </asp:UpdatePanel>--%>
    
                    <div id="divAddMessage" runat="server" visible="true" style="background-color:#fff;width:850px;">
                        <div class="pckghdgred" ><asp:literal id="ltlSubmitBidHeader" runat="server" Text="Submit a Bid"></asp:literal></div>
                        <div class="dcmain">
                            <table style="width: 829px;height: 358px;">
                                <tr>
		                            <td class="required">Contact Name:</td>
		                            <td class="field"><asp:textbox id="txtContactName" runat="server" maxlength="100" columns="50" style="width: 219px;"></asp:textbox></td>
	                            </tr>
	                            <tr>
		                            <td class="required">Contact Email:</td>
		                            <td class="field">
		                                <asp:textbox id="txtContactEmail" runat="server" maxlength="100" columns="50" style="width: 219px;"></asp:textbox>
		                                <CC:EmailValidator Display="Dynamic" runat="server" id="fvContactEmail" ValidationGroup="Submitbids" ControlToValidate="txtContactEmail" ErrorMessage="Field 'Contact Email is not valid."  />
		                            </td>
	                            </tr>
	                            <tr> 
		                            <td class="required">Contact Phone:</td>
		                            <td class="field"><asp:textbox id="txtContactPhone" runat="server" maxlength="50" columns="50" style="width: 219px;"></asp:textbox></td>
	                            </tr>
	                            <tr id="trQuoteExpiration" runat="server">
		                            <td class="required">Bid Expiration Date:</td>
		                            <td class="field">
		                                <CC:DatePicker ID="dtExpiration" runat="server"></CC:DatePicker>
		                                <CC:DateValidator  runat="server" id="dtvExpiration" ValidationGroup="Submitbids"  ControlToValidate="dtExpiration" ErrorMessage="Expiration Date is not valid."  />
		                            </td>
	                            </tr>
	                            <tr id="trQuoteTotal" runat="server"> 
		                            <td class="required"><span class="smallest red">*</span>Bid Total <span class="smallest">(Please Include Tax)</span>:</td>
		                            <td class="field">
		                                $<asp:textbox id="txtQuoteTotal" runat="server" maxlength="50" columns="50" style="width: 50px;"></asp:textbox>
		                                <asp:CompareValidator runat="server" Operator="GreaterThan" Type="Double" ValidationGroup="Submitbids"  ControlToValidate="txtQuoteTotal" ValueToCompare="0"  ErrorMessage="Field 'Bid Total' is required and should be greater than 0" ></asp:CompareValidator>
		                                <CC:FloatValidator   runat="server" id="fvQuoteTotal" ValidationGroup="Submitbids" ControlToValidate="txtQuoteTotal" ErrorMessage="Bid Total is not valid. Please omit any commas or $ characters."  />
		                            </td>
	                            </tr>
    	                        
    	                        
	                            <tr>
		                            <td class="required"><span class="smallest red">*</span>When can you start?:<br /><span class="smallest red">(Mandatory)</span></td>
		                            <td class="field">
		                                <CC:LimitTextBox id="txtStartDate" runat="server" Limit="100" columns="50" TextMode="MultiLine" rows="2"  Width="500" ></CC:LimitTextBox>
		                            
		                                <%--<CC:DatePicker ID="dtStartDate" runat="server"></CC:DatePicker>--%>
		                                <%--<CC:RequiredDateValidator Display="Dynamic" runat="server" id="rdtvStartDate" ControlToValidate="dtStartDate" ErrorMessage="Date Field 'Start Date' is required" ValidationGroup="SubmitBid" /><CC:DateValidator Display="Dynamic" runat="server" id="dtvStartDate" ControlToValidate="dtStartDate" ErrorMessage="Date Field 'Start Date' is invalid" />--%>
		                            </td>
		                            <td></td>
	                            </tr>
	                            <tr>
		                            <td class="required"><span class="smallest red">*</span>Time required to complete the job:</td>
		                            <td class="field">
		                                <asp:textbox id="txtCompletionTime" runat="server" maxlength="50" columns="50" style="width: 219px;"></asp:textbox>
		                                <asp:RequiredFieldValidator runat="server" ValidationGroup="Submitbids"  ControlToValidate="txtCompletionTime" ErrorMessage="Field 'Completion Time' is required" ></asp:RequiredFieldValidator>
		                            </td>
		                            <td></td>
	                            </tr>
	                            <tr>
		                            <td class="required"><span class="smallest red">*</span>Payment Terms:</td>
		                            <td class="field">
		                                <%--<asp:TextBox id="txtPaymentTerms" style="width: 500px;" Columns="50" rows="6" TextMode="Multiline" runat="server"></asp:TextBox>
		                                <asp:RequiredFieldValidator ID="rfvPaymentTerms" runat="server" Display="Dynamic" ControlToValidate="txtPaymentTerms" ErrorMessage="Field 'Payment Terms' is required"  ValidationGroup="SubmitBid"></asp:RequiredFieldValidator>--%>
		                                <asp:Literal id="ltlPaymentTerms2" runat="server"></asp:Literal>
		                            </td>
		                            <td></td>
	                            </tr>
                                <tr valign="top">
                                    <td class="required" width="110">Message:<br /><span class="smallest red">(Not applicable when using the Save Changes button.)</span></td>
                                    <td class="field"><CC:LimitTextBox  runat="server" Limit="500" columns="50" Rows="6" maxlength="500" Width="500" id="txtMessage" TextMode="MultiLine"/></td>
                                </tr>
                            </table>
                            <asp:ValidationSummary ShowMessageBox="true" ShowSummary="false" ValidationGroup="Submitbids" HeaderText="Following Fields are Mandatory:"  EnableClientScript="true" runat="server"/>
                        
                <%--<CC:DivWindow ID="ctrlAddMessage" runat="server" TargetControlID="divAddMessage" TriggerId="lnkAddMessage" CloseTriggerId="btnCancelMessage" ShowVeil="true" VeilCloses="true" />--%>
                
                   
            
            <table width="100%">
	            <tr>
                    <td>
                        <div class="pckgltgraywrpr center">
                            <div class="pckghdgred">
                                Upload Detailed Quote and Supporting Documents
                            </div>
                            <div style="overflow:auto; background-color:White; margin-top:5px; margin-bottom:5px; margin-left:5px; margin-right:5px;" >
                                <CC:MultiUploadDocument id="mudUpload" runat="server" ></CC:MultiUploadDocument>
                            </div>
                            <asp:UpdatePanel ID="upDocuments" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                <div class="pckghdgblue" style="background:#e1e1e1">
                                    <asp:Button id="btnTransferDocs" runat="server" cssclass="btnred" text="Transfer Uploaded Documents" />
                                </div>
                                    <div style="text-align:left; background-color:White; margin-top:5px; margin-bottom:5px; margin-left:5px; margin-right:5px;" >
                                        <asp:Repeater id="rptVendorDocuments" runat="server">
                                        <HeaderTemplate>
                                            <table cellspacing="0" cellpadding="0" border="0" class="dochdrtbl">
                                                <tr>
                                                    <th>Document</th>
                                                    <th>Uploaded</th>
                                                    <th>Delete</th>
                                                </tr>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr class='<%#iif(Container.ItemIndex mod 2 = 1,"alternate","row") %>'>
                                                <td class="dcrow" id="td1" runat="server" >
                                                    <a id="lnkMessageTitle" runat="server" class="dctitlelink" target="_blank" href="/" ><%#DataBinder.Eval(Container.DataItem, "Title")%></a>&#160;
                                                </td>
                                                <td><%#FormatDateTime(DataBinder.Eval(Container.DataItem, "Uploaded"), DateFormat.ShortDate)%></td>
                                                <td>
                                                <CC:ConfirmImageButton CommandName="Remove" Message="Are you sure that you want to remove this document?" runat="server" ImageUrl="/images/admin/delete.gif" ID="lnkDelete" />
                                            </td>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                        </table>
                                        </FooterTemplate>
                                        </asp:Repeater>	
                                        <div class="dcrow" id="divNoVendorDocuments" runat="server"><p class="dcmessagetext">There are no documents available.</p></div> 
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
		            </td>
	            </tr>
            </table>
<br />
                            <p align="center">
                                <asp:Button id="btnSave" runat="server" Text="Save Changes" ValidationGroup="Submitbids" Message="Are you sure you want to save changes to this bid request without notifying the builder?" cssClass="btnred" ></asp:Button>
                                <asp:Button id="btnAddMessage" runat="server" cssclass="btnblue" text="Submit Bid"  ValidationGroup="Submitbids"  />
                                <asp:Button id="btnRequestInfo" runat="server" cssclass="btnblue" text="Request Information"  visible="false" />
                                <asp:Button id="btnCancelMessage" runat="server" cssclass="btnblue" visible="false" text="Close" CausesValidation="false" />        
                            </p>
    <%--<input class="btnblue" type="submit" value="Bid Requests" onclick="window.location='default.aspx?<%= GetPageParams(Components.FilterFieldType.All) %>';return false;" />--%>
</div>
</div>
                    </div>
                </center>
</div>
</CT:MasterPage>
