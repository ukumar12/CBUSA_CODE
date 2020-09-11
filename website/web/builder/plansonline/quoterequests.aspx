<%@ Page Language="VB" AutoEventWireup="false" Title="Plans Online - Bid Comparison Matrix" CodeFile="quoterequests.aspx.vb" Inherits="POQuoteRequests"  %>
<%@ Register Src="~/controls/LimitTextBox.ascx" TagName="LimitTextbox" TagPrefix="CC" %>

<CT:MasterPage ID="CTMain" runat="server">
<script type="text/javascript">
    function ShowSearch() {
        document.getElementById("tblSearch").style.display = "block";
    }
    function HideSearch() {
        document.getElementById("tblSearch").style.display = "none";
    }
   
</script>
 
<div class="pckgwrpr bggray">
<div class="pckghdgltblue" >   
     Plans Online > <a href='default.aspx' >Projects</a> > <a href='quotes.aspx'  id="lnkBidRequest" runat="server">Bid Requests</a> > <span style="font-size:20px;">Bid Comparison Matrix</span>
    <span style="float:right;">
    
    <asp:LinkButton id="lnkBidReminders" runat="server"  runat="server" OnClientClick="location.href='#pnlMessages';return false"  class="btnblue">Send Bid Reminders</asp:LinkButton >
    

        <a href="#" id="aShowSearch" class="btnblue" onclick="ShowSearch();">Search Bid Comparison Matrix</a>&nbsp;&nbsp;
         
    </span>
</div>
<div class="pckgbdy">
<asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">

<table cellpadding="2" cellspacing="2" id="tblSearch" style="display:none;color: white">
<tr>
<th valign="top"><b>Project:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_ProjectId" runat="server" /></td>
<th valign="top"><b>Supply Phase:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_VendorCategoryId" runat="server" /></td>
</tr>
<tr>
<th valign="top"><b>Bid Request:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_QuoteId" runat="server" /></td>
<th valign="top"><b>Bid Request #:</b></th>
<td valign="top" class="field"><asp:textbox id="F_QuoteNumber" runat="server" Columns="50" MaxLength="100" /></td>
</tr>
<tr>
<th valign="top"><b>Vendor:</b></th>
<td valign="top" class="field"><asp:DropDownList ID="F_VendorId" runat="server" /></td>
<th valign="top"><b>Request Status:</b></th>
<td valign="top" class="field">
    <asp:DropDownList ID="F_RequestStatus" runat="server" >
        <asp:ListItem Value="All">-- ALL --</asp:ListItem>
		<asp:ListItem Value="Active">Active</asp:ListItem>
        
		<asp:ListItem Value="New">New</asp:ListItem>
		<asp:ListItem Value="Request Information">Request Information</asp:ListItem>
		<asp:ListItem Value="Awarded">Awarded</asp:ListItem>
		<asp:ListItem Value="Declined By Builder">Declined By Builder</asp:ListItem>
		<asp:ListItem Value="Declined By Vendor">Declined By Vendor</asp:ListItem>
		<asp:ListItem Value="Exited Market">Exited Market</asp:ListItem>
		<asp:ListItem Value="Cancelled">Cancelled</asp:ListItem>
        
       
    </asp:DropDownList>
</td>
</tr>
<tr>
<th valign="top">Vendor Contact Name:</th>
<td valign="top" class="field"><asp:textbox id="F_VendorContactName" runat="server" Columns="50" MaxLength="100"></asp:textbox></td>
<th valign="top">Vendor Contact Email:</th>
<td valign="top" class="field"><asp:textbox id="F_VendorContactEmail" runat="server" Columns="50" MaxLength="100"></asp:textbox></td>
</tr>
<tr>
<th valign="top"><b>Builder Document:</b></th>
<td valign="top" class="field"><asp:textbox id="F_BuilderDocument" runat="server" Columns="50" MaxLength="100" /></td>
<th valign="top"><b>Vendor Document:</b></th>
<td valign="top" class="field"><asp:textbox id="F_VendorDocument" runat="server" Columns="50" MaxLength="100" /></td>
</tr>
<tr>
<th valign="top"><b>Total:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0" style="color:#0f2e51">
<tr><td class="smaller">From<asp:textbox id="F_QuoteTotalLBound" runat="server" Columns="5" MaxLength="10"/></td><td class="smaller">To<asp:textbox id="F_QuoteTotalUbound" runat="server" Columns="5" MaxLength="10"/></td>
</tr>
</table>
</td>
<th valign="top"><b>Expiration Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0" style="color:#0f2e51">
<tr><td class="smaller">From <CC:DatePicker id="F_QuoteExpirationDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_QuoteExpirationDateUbound" runat="server" /></td>
</tr>
</table>
</td>
</tr>
<tr>
<th valign="top"><b>Create Date:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0" style="color:#0f2e51">
<tr><td class="smaller">From <CC:DatePicker id="F_CreateDateLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_CreateDateUbound" runat="server" /></td>
</tr>
</table>
</td>
<th valign="top"><b>Deadline:</b></th>
<td valign="top" class="field">
<table border="0" cellpadding="0" cellspacing="0" style="color:#0f2e51">
<tr><td class="smaller">From <CC:DatePicker id="F_DeadlineLbound" runat="server" /></td><td>&nbsp;</td><td class="smaller">To  <CC:DatePicker id="F_DeadlineUbound" runat="server" /></td>
</tr>
</table>
</td>
</tr>
<tr>
<td colspan="4" align="right">
<CC:OneClickButton id="btnSearch" Runat="server" Text="Search" cssClass="btnred" />
<input class="btnred" type="submit" value="Clear" onclick="window.location='quoterequests.aspx';return false;" />
<input class="btnblue" type="submit" value="Bid Requests" onclick="window.location='quotes.aspx';return false;" />
<input class="btnblue" type="submit" value="Projects" onclick="window.location='default.aspx';return false;" />
<a href="#" id="a1" class="btnblue" onclick="HideSearch();">Hide Search</a>
</td>
</tr>
</table>
</asp:Panel>


<asp:Literal id="ltlProjectHeader" runat="server"></asp:Literal>


<CC:GridView  id="gvList" class="tblcomprlen" style="margin: 5px 0 15px 0px;" Width="100%" CellSpacing="2" CellPadding="2" runat="server" PageSize="10" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links.<br>*Details (Unread Messages from Vendor / Total Number of Messages from Vendor).<br>*RFIs (Number of Posts added Today by Vendor / Total Number of Posts)" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
	<Columns>
		<asp:TemplateField SortExpression="Project" HeaderText="Project">
		<ItemStyle />
			<ItemTemplate>
			    <asp:literal id="ltlProject" runat="server"></asp:literal>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField SortExpression="Quote" HeaderText="Bid Request">
		<ItemStyle />
			<ItemTemplate>
			    <asp:literal id="ltlQuote" runat="server"></asp:literal>
			</ItemTemplate>
		</asp:TemplateField>
		
		<asp:TemplateField SortExpression="Vendor" HeaderText="Vendor">
		<ItemStyle width="150" />
			<ItemTemplate>
			    <asp:literal id="ltlContact" runat="server"></asp:literal>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField SortExpression="QuoteTotal" DataField="QuoteTotal" HeaderText="Total" DataFormatString="{0:c}"></asp:BoundField>
		<asp:BoundField SortExpression="QuoteExpirationDate" DataField="QuoteExpirationDate" HeaderText="Expires" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField SortExpression="StartDate" DataField="StartDate" HeaderText="Can Start" DataFormatString="{0:d}" HTMLEncode="False"></asp:BoundField>
		<asp:BoundField SortExpression="CompletionTime" DataField="CompletionTime" HeaderText="Time to Complete"></asp:BoundField>
		<asp:BoundField DataField="PaymentTerms" HeaderText="Payment Terms"></asp:BoundField>
		<asp:TemplateField HeaderText="Rebate %">
		<ItemStyle width="100" />
			<ItemTemplate>
			    <asp:literal id="ltlRebate" runat="server"></asp:literal>
			</ItemTemplate>
		</asp:TemplateField>
        <%--<asp:BoundField SortExpression="RequestStatus" DataField="RequestStatus" HeaderText="Request Status"></asp:BoundField>--%>
		<asp:TemplateField SortExpression="LastMessageDate" HeaderText="Latest Message">
		<ItemStyle width="200" />
			<ItemTemplate>
			    <asp:literal id="ltlMessage" runat="server"></asp:literal>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
		    <ItemStyle CssClass="ActionButtons" width="100" />
			<ItemTemplate>
			<asp:HyperLink enableviewstate="False" class="btnred" style="width:100px;" runat="server" NavigateUrl= '<%# "quoterequestmessages.aspx?QuoteRequestId=" & DataBinder.Eval(Container.DataItem, "QuoteRequestId") & "&" & GetPageParams(Components.FilterFieldType.All)%>' ID="lnkQuoteRequests">Details <b>(<%#DataBinder.Eval(Container.DataItem, "UnreadMessages")%>/<%#DataBinder.Eval(Container.DataItem, "TotalMessages")%>)</b></asp:HyperLink>
			<asp:HyperLink enableviewstate="False" class="btnred" style="width:100px;" runat="server" NavigateUrl= '<%# "quoterequestmessages.aspx?QuoteRequestId=" & DataBinder.Eval(Container.DataItem, "QuoteRequestId") & "&" & GetPageParams(Components.FilterFieldType.All) & "&#rfi" %>' ID="lnkRFI">RFIs <b>(<%#DataBinder.Eval(Container.DataItem, "NewPosts")%>/<%#DataBinder.Eval(Container.DataItem, "TotalPosts")%>)</b></asp:HyperLink>
			<asp:HyperLink enableviewstate="False" class="btnblue" style="width:100px;" runat="server" NavigateUrl= '<%# "quoterequestmessages.aspx?QuoteRequestId=" & DataBinder.Eval(Container.DataItem, "QuoteRequestId") & "&" & GetPageParams(Components.FilterFieldType.All) & "&award=y#award" %>' ID="lnkAward">Award Bid</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateField>
	</Columns>
</CC:GridView>

<asp:Panel id="pnlMessages" runat="server">
   <asp:UpdatePanel id="upMessages" runat="server" updatemode="conditional">
        <ContentTemplate>
       <div class="pckgltgraywrpr ">
           <div class="pckghdgltblue" >
            <span style="float:left;">Bid Reminders</span>
            <span style="float:right;">
                <asp:linkbutton id="lnkAddMessage" runat="server" class="btnblue" Text="Send Bid Reminders" />
            </span>
        </div>
            <center>
            <div style="text-align:center;">
            <div class="pckghdgblue">Reminders are only sent to vendors that have not yet responded to the bid request.</div>
                        <p class="bold green"><asp:Literal id="ltlReminderMsg" runat="server" /></p>
    
                    </div>
          
                <div id="divAddMessage" runat="server" visible="false" style="border:1px solid #c2c2c2;width:650px;margin-bottom:5px;">
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

            
                    <CC:GridView id="GridView1" Width="100%" CellSpacing="2" CellPadding="2" runat="server" AllowPaging="False" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="No Messages (New Bid Request)" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
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
			                        <span style="font-weight:normal;"><asp:literal runat="server" id="Literal1"  /></span>
			                    </ItemTemplate>
		                    </asp:TemplateField>
	                    </Columns>
                    </CC:GridView>
                
    </div>
         </ContentTemplate>
    </asp:UpdatePanel>
</asp:Panel>
</div>
</div>
 
</CT:MasterPage>

