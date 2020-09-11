<%@ Page Language="VB" AutoEventWireup="false" CodeFile="editquote.aspx.vb" Inherits="EditQuote"  Title="Plans Online - Projects"%>
<%@ Register Src="~/controls/Documents/MultiFileUploadNew.ascx" TagName="MultiUploadDocument" TagPrefix="CC" %>
<%@ Register Src="~/controls/LimitTextBox.ascx" TagName="LimitTextbox" TagPrefix="CC" %>

<CT:MasterPage ID="CTMain" runat="server">
<div class="pckgwrpr bggray">
<div class="pckghdgltblue">
    Plans Online > <a href='default.aspx' >Projects</a> > <a href='quotes.aspx' >Bid Requests</a> > <span style="font-size:20px;">Add/Edit Bid Request</span>
</div>
<div class="pckgbdy">
<div class="pckgltgraywrpr center" id="divMsg" runat="server" visible="false">
    <div class="pckghdgred">Bid Request Update Confirmation</div>
    <div style="text-align:center;">
        <p class="bold green"><asp:Literal id="ltlMsg" runat="server" /></p>
        <input class="btnblue" type="submit" value="Projects" onclick="window.location='default.aspx';return false;" />
        <input class="btnblue" type="submit" value="Bid Requests" onclick="window.location='quotes.aspx';return false;" />
        <input class="btnblue" type="submit" value="Bid Status" onclick="window.location='quoterequests.aspx?F_QuoteId=<%=QuoteId %>';return false;" />
        <asp:button id="btnBack" runat="server" visible="false" class="btnred" text="Bid Request Details" />
    </div>
</div>
<table border="0" cellspacing="1" cellpadding="2" width="100%">
	<tr><td colspan="2"><span class="smallest"><span class="red">*</span> - denotes required fields</span></td></tr>
	<tr valign="top">
		<td class="required"><span class="red">*</span> <b>Project:</b></td>
		<td class="field"><asp:DropDownList id="drpProjectId" runat="server" /></td>
		<td><CC:RequiredFieldValidatorFront ValidationGroup="EditQuote" EnableClientScript="False" ID="rfvProjectId" runat="server" Display="None" ControlToValidate="drpProjectId" ErrorMessage="Field 'Project' is blank"></CC:RequiredFieldValidatorFront></td>
	</tr>
	<tr valign="top">
		<td class="required"><span class="red">*</span> <b>Title:</b></td>
		<td class="field"><asp:textbox id="txtTitle" runat="server" maxlength="50" columns="50" style="width: 319px;"></asp:textbox></td>
		<td><CC:RequiredFieldValidatorFront ValidationGroup="EditQuote" EnableClientScript="False" ID="rfvTitle" runat="server" Display="None" ControlToValidate="txtTitle" ErrorMessage="Field 'Title' is blank"></CC:RequiredFieldValidatorFront></td>
	</tr>
	
	<tr valign="top">
		<td class="required"><span class="red">*</span> <b>Supply Phase(s):</b></td>
		<td class="field">
		    <asp:UpdatePanel ID="upSupplyPhases" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
		            <div class="pckgltgraywrpr center">
                        <div class="bdbtblhdg" style="text-align: left; font-size: 12px; color:#fff; padding: 10px 15px;margin-bottom:8px;">
                            Select the Supply Phase(s) that apply to this Bid Request
                            <%--<br /><span style="font-size:10px; font-weight:bold;">To obtain the best results using this tool, it is recommended that you create a New Bid Request for each individual phase.  This streamlines communication and enables you to better organize and manage mutliple bids.</span>--%>
                        </div>
                        <span style="font-size:10px;">To obtain the best results using this tool, it is recommended that you create a New Bid Request for each individual phase.  This streamlines communication and enables you to better organize and manage mutliple bids.</span>
                        <div style="margin-top:25px; margin-bottom:25px; margin-left:5px; margin-right:5px;">
                            
                            <CC:ListSelect ID="lsSupplyPhases" runat="server" AutoPostback="true" style="text-align:left; margin-left:auto; margin-right:auto;width:600px;table-layout:auto;" Height="150" AddImageUrl="/images/admin/true.gif" DeleteImageUrl="/images/admin/delete.gif" />
                        </div>
                    </div>
                    <div class="pckgltgraywrpr center">
                        <div class="bdbtblhdg" style="text-align: left; font-size: 12px; color:#fff; padding: 10px 15px;margin-bottom:8px;">Vendors in selected Supply Phase(s) that will receive Bid Requests</div>
                        <div style="text-align:left; overflow:auto; height:150px; background-color:White; margin-top:5px; margin-bottom:5px; margin-left:5px; margin-right:5px;" >
                            <b style="margin-left:10px;"><asp:Literal ID="ltlVendors" runat="server"></asp:Literal></b><br />
                            <asp:Repeater ID="rptVendors" runat="server">
                                <ItemTemplate><b style="margin-left:10px;">
                                  <div class="obpBidVendorInfo">
                                    <%#DataBinder.Eval(Container.DataItem, "CompanyName")%></b> 
                                     </div><div class="vendorFlag"><p class="vendorFlagReason"></p> 
                                <asp:LinkButton runat="server" ID="lnkVendorDelete" class="vendorFlagDelete"  Text="Remove" OnClientClick="return confirm('Would you like to remove this vendor from this bid request only?');" CommandArgument='<%# Eval("VendorId") %>' CommandName="VendorDelete"  />
                                <asp:LinkButton runat="server" ID="lnkVendorReinstate" class="vendorFlagDelete" Text="Reinstate" OnClientClick="return confirm('Are you sure you want to reinstate this vendor for this bid request?');" CommandArgument='<%# Eval("VendorId") %>' CommandName="VendorReinstate" Visible="false" />
                                <asp:Literal runat="server" ID="lnkVendorReinstateText" /> 
                               </div>
								  <div class="clear"></div>
								  <hr/>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
		</td>
	</tr>
    
	<asp:PlaceHolder id="phStep2" runat="server" >
	<tr>
		<td class="required"></td>
		<td class="field">
		    
		            <div class="pckgltgraywrpr center">
                        <div class="pckghdgred">
                            Documents
                        </div>
                        <div style="overflow:auto; background-color:White; margin-top:5px; margin-bottom:5px; margin-left:5px; margin-right:5px;" >
                            <CC:MultiUploadDocument id="mudUpload" runat="server" ></CC:MultiUploadDocument>
                        </div>
                <asp:UpdatePanel ID="upDocuments" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                        <div class="pckghdgblue">
                            <asp:Button id="btnTransferDocs" runat="server" cssclass="btnred" text="Transfer Uploaded Documents" />
                        </div>
                        <div style="text-align:left; overflow:auto;background-color:White; margin-top:5px; margin-bottom:5px;" >
                            <b style="margin-left:10px;"><asp:Literal ID="ltlDocuments" runat="server"></asp:Literal></b><br />
                            
                            <div>
                                <asp:Repeater id="rptDocuments" runat="server">
                                <HeaderTemplate>
                                    <table cellspacing="0" cellpadding="0" border="0" class="dochdrtbl">
			                            <tr>
			                                <th>Document</th>
			                                <th>Uploaded</th>
			                                <th>Delete</th>
			                            </tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr class='<%#IIf(Container.ItemIndex Mod 2 = 1, "alternate", "row") %>'>
                                        
                                        <td class="dcrow" id="tdMessageRow" runat="server" >
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
                                <div class="dcrow" id="divNoCurrentDocuments" runat="server"><p class="dcmessagetext">You have no documents available.</p></div> 
	                            <div class="btnhldrrt">
                                    
                                </div>
                            </div>
                            </div>
                        </ContentTemplate>
            </asp:UpdatePanel>
            
                        
                    </div>
                
		</td>
	</tr>
	<tr valign="top">
		<td class="required"><span class="red">*</span> <b>Deadline:</b></td>
		<td class="field"><CC:DatePicker ID="dtDeadline" runat="server"></CC:DatePicker></td>
		<td><CC:RequiredDateValidator ValidationGroup="EditQuote" EnableClientScript="False" Display="None" runat="server" id="rdtvDeadline" ControlToValidate="dtDeadline" ErrorMessage="Date Field 'Deadline' is blank" /><CC:DateValidatorFront ValidationGroup="EditQuote" EnableClientScript="False" Display="None" runat="server" id="dtvDeadline" ControlToValidate="dtDeadline" ErrorMessage="Date Field 'Deadline' is invalid" /></td>
	</tr>
	
	<tr valign="top">
		<td class="required"><span class="red">*</span> <b>Instructions:</b></td>
		<td class="field">
       
        <asp:TextBox id="txtInstructions" style="width: 800px;" Columns="55" rows="10" TextMode="Multiline" runat="server"></asp:TextBox></td>
		<td><CC:RequiredFieldValidatorFront ValidationGroup="EditQuote" EnableClientScript="False" ID="rfvInstructions" runat="server" Display="Dynamic" ControlToValidate="txtInstructions" ErrorMessage="Field 'Instructions' is blank"></CC:RequiredFieldValidatorFront></td>
	</tr>
	<tr valign="top">
		<td class="required"><b>Status:</b></td>
		<td class="field"><b><asp:literal id="ltlStatus" runat="server" /></b></td>
		<td></td>
	</tr>
	<tr valign="top">
		<td class="required"><b>Status Date:</b></td>
		<td class="field"><b><asp:literal id="ltlStatusDate" runat="server" /></b></td>
		<td></td>
	</tr>
	<asp:PlaceHolder id="phawarded" runat="server" visible="false">
	    <tr valign="top">
		    <td class="optional"><b>Awarded To Vendor:</b></td>
		    <td class="field"><asp:literal id="ltlAwardedToVendor" runat="server" /></td>
		    <td></td>
	    </tr>
	    <tr valign="top">
		    <td class="optional"><b>Awarded Date:</b></td>
		    <td class="field"><asp:literal id="ltlAwardedDate" runat="server" /></td>
		    <td></td>
	    </tr>
	    <tr valign="top">
		    <td class="optional"><b>Awarded Total:</b></td>
		    <td class="field"><asp:literal id="ltlAwardedTotal" runat="server" /></td>
		    <td></td>
	    </tr>
	</asp:PlaceHolder>
	</asp:PlaceHolder>
</table>


<p></p>
    <div style="margin: 0px 14px 13px 418px;">
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btnblue" ValidationGroup="EditQuote"></CC:OneClickButton>
<CC:ConfirmButton id="btnUpdate" runat="server" Text="Save & Send Update" Message="Are you sure that you want to send an update to all the vendors?" cssClass="btnred" ValidationGroup="EditQuote"></CC:ConfirmButton>
<CC:ConfirmButton id="btnStartBid" runat="server" Text="Submit Bid Request" Message="Are you sure that you want to start the bidding process? This will change the Bid Request Status to Bidding In Progress and send out Bid Requests to all the selected vendors." cssClass="btnred" ValidationGroup="EditQuote"></CC:ConfirmButton>
<CC:ConfirmButton id="btnStopBid" runat="server" Text="Cancel Bid Request" Message="Are you sure that you want to cancel this bid request? This will change the Bid Request Status to Cancelled. If the bidding process has already started this will send an email notification to all the selected vendors." cssClass="btnred" ValidationGroup="EditQuote"></CC:ConfirmButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Go Back" cssClass="btnblue" CausesValidation="False"></CC:OneClickButton>

</div></div>
</CT:MasterPage>
