<%@ Page Language="VB" AutoEventWireup="false" CodeFile="vendor.aspx.vb" Inherits="vendor" %>
<%@ Register TagName="StarRatingDisplay" TagPrefix="CC" Src="~/controls/StarRatingDisplay.ascx" %>
<%@ Register TagName="TabControl" TagPrefix="CC" Src="~/controls/TabControl.ascx" %>

<CT:MasterPage runat="server" ID="CTMain">
<link href="/includes/tabs.css" rel="stylesheet" type="text/css" />
<script type="text/javascript" src="/includes/tabcontent.js"></script>

<asp:PlaceHolder runat="server">
    <script type="text/javascript">
      <%--  function ClearComments() {
            var txt = $get('<%=txtComment.ClientId %>');
            txt.innerHTML = '';
            return false;
        }--%>
        function InitSaved() {
            Sys.Application.add_load(OpenSaved);
        }
        function OpenSaved() {
            Sys.Application.remove_load(OpenSaved);
            var ctl = $get('<%=frmSaved.ClientID %>').control;
            ctl._doMoveToCenter();
            ctl.Open();
        }
    </script>
</asp:PlaceHolder>

<CC:PopupForm ID="frmSaved" runat="server" CloseTriggerId="btnOK" ShowVeil="true" VeilCloses="true" CssClass="pform" Width="300px">
    <FormTemplate>
        <div class="pckggraywrpr" style="margin-bottom:0px;">
            <div class="pckghdgred">Comment Saved</div>
            <div style="padding:10px;">
                <b>Your Ratings and Comments have been saved.</b>
                <p align="center" style="margin:20px;">
                    <asp:Button id="btnOK" runat="server" text="Close" cssclass="btnred" />
                </p>
            </div>
        </div>
    </FormTemplate>
    <Buttons>
        <CC:PopupFormButton ControlID="btnOK" ButtonType="ScriptOnly" />
    </Buttons>
</CC:PopupForm>
<div>
	<div class="pckgltgraywrpr">
		<table>
			<tr>
				<td class="vtop" style="width:300px">
                    <div class="pckghdgred"><asp:Literal id="ltrCompanyName" runat="server" /></div>
					<table style="width:100%;">
					    <col width="150px" />
					    <col width="150px" />
						<tr>
							<td colspan="2" class="vendortext">
								<CC:StarRatingDisplay id="RatingDisplay" runat="server" />
							</td>
						</tr>
						<tr ID="trWebsiteURL" runat="server">
							<td colspan="2" style="padding-left: 15px">
								<div class="vendorlink"><asp:HyperLink id="lnkWebsiteURL" runat="server" /></div>
							</td>
						</tr>
						<tr>
							<td class="vendorblock" valign="top" style="padding-left: 15px">
								<div id="divAddressInfo" runat="server" class="vendortext"></div>
							</td>
							<td class="vendorblock" valign="top" style="padding-left: 15px">
								<div id="divContactInfo" runat="server" class="vendortext"></div>
							</td>
						</tr>
                        <tr>
                            <td class="vendorblock" valign="top" style="padding-left: 15px">
                                <div id="divBillingInfo" runat="server" class="vendortext"></div>
                            </td>
                            <td class="vendorblock" valign="top" style="padding-left: 15px">
                                &nbsp;
                            </td>
                        </tr>						
                        <tr>
                            <td colspan="2" style="height:1px;padding:0px;padding-left: 15px;background-color:#ccc;"><img src="/images/spacer.gif" style="height:1px;" /></td>
                        </tr>
                        <tr id="trSubsidiary" runat="server">
                            <td class="vtop bold" style="padding-left: 15px">Subsidiary/Division Of:</td>
                            <td class="vtop" style="padding-left: 15px"><asp:Literal id="ltlSubsidiary" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <td class="vtop bold" style="padding-left: 15px">Business Type:</td>
                            <td class="vtop"><asp:Literal id="ltlBusinessType" runat="server"></asp:Literal></td>
                        </tr>                                    
                        <tr>
                            <td class="vtop bold" style="padding-left: 15px">In Business Since:</td>
                            <td class="vtop"><asp:Literal id="ltlYearStarted" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <td class="vtop bold" style="padding-left: 15px">Number of Employees:</td>
                            <td class="vtop"><asp:Literal id="ltlNumEmployees" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <td class="vtop bold" style="padding-left: 15px">Areas Supplied:</td>
                            <td class="vtop"><asp:Literal id="ltlAreasSupplied" runat="server"></asp:Literal></td>
                        </tr>
                        <%-- <tr
                            
                            <%--<td class="vtop"><asp:Literal id="Literal1" runat="server"></asp:Literal></td>
                        </tr>--%>
					</table>
				</td>
				<td class="vtop" style="width:10px;">&nbsp;</td>
				<td class="vtop" style="width:250px;">
				<div class="pckghdgred">Contact Information</div>
				<asp:Repeater id="rptContacts" runat="server">
				<ItemTemplate>
					<table style="width:100%;">
					    <col width="40%" />
					    <col width="60%" />
						<tr valign="top">
							<td class="vendortext" style="font-weight:bold;">
								<asp:Literal id="ltlPrimary" runat="server"></asp:Literal>Contact:
							</td>
							<td class="vendortext" >
								<asp:Literal id="ltlContactFullName" runat="server"></asp:Literal>
							</td>
						</tr>
						<tr valign="top" id="trTitle" runat="server">
							<td class="vendortext" style="font-weight:bold;">
								Title:
							</td>
							<td class="vendortext">
								<asp:Literal id="ltlTitle" runat="server"></asp:Literal>
							</td>
						</tr>
						<tr valign="top" id="trRoles" runat="server">
						    <td class="vendortext" style="font-weight:bold;">
						        Role(s):
						    </td>
						    <td class="vendortext">
						        <asp:Literal id="ltlRoles" runat="Server"></asp:Literal>
						    </td>
						</tr>
						<tr valign="top" id="trPhone" runat="server" style="font-weight:bold;">
							<td class="vendortext">
								Phone:
							</td>
							<td class="vendortext">
								<asp:Literal id="ltlPhone" runat="server"></asp:Literal>
							</td>
						</tr>
						<tr valign="top" id="trMobile" runat="server">
							<td class="vendortext" style="font-weight:bold;">
								Mobile:
							</td>
							<td class="vendortext">
								<asp:Literal id="ltlMobile" runat="server"></asp:Literal>
							</td>
						</tr>
						<tr valign="top" id="trFax" runat="server">
							<td class="vendortext" style="font-weight:bold;">
								Fax:
							</td>
							<td class="vendortext">
								<asp:Literal id="ltlFax" runat="server"></asp:Literal>
							</td>
						</tr>
						<tr valign="top" id="trEmail" runat="server">
							<td class="vendortext" style="font-weight:bold;">
								Email:
							</td>
							<td class="vendortext">
								<asp:Literal id="ltlEmail" runat="server"></asp:Literal>
							</td>
						</tr>
					</table>
				</ItemTemplate>
				<SeparatorTemplate>
				    <div style="height:1px;padding:0px;background-color:#ccc;"><img src="/images/spacer.gif" style="height:1px;" /></div>
				</SeparatorTemplate>
				</asp:Repeater>
				</td>
				<td class="vtop" style="width:10px;">&nbsp;</td>
				<td class="vtop" style="width:350px">
				    <div class="pckghdgred">Vendor Details</div>
				    <CC:TabControl runat='server' ID="ctlTabs" />
				</td>
			</tr>
            <tr>
                    <td><asp:Literal id="ltlVendorRating" runat="server"></asp:Literal></td>
            </tr>
		</table>
	</div>
	<br />
	<%--<div runat="server" style="margin:5px auto;" class="pckgltgraywrpr">
		<div class="pckghdgred">Ratings &amp; Comments</div>
		<ul id="maintab" class="shadetabs">
			<li class="selected"><a href="#" rel="tcontent10"><span>Add Review</span></a></li>
			<li class=""><a href="#" rel="tcontent11"><span>Read Comments</span></a></li>
            <%--<li class="selected"><a href="#" rel="tcontent10"><span>Add Review</span></a> <a href="#" rel="tcontent11"><span>Read Comments</span></a></li>--%>
		<%--</ul>
		<br />--%>
		<%--<img src="/images/rating/red.gif" width="100%" height="3" style="border-style:none" alt="" />
		<br />
		<div class="tabdetails">
			<div id="tcontent10" style="display: block; background-color:#e7e7e7;">
                <div style="width:75%;margin:auto;">                

    				<div><i>(roll over stars to rate)</i></div>
                
                    <table cellpadding="2" cellspacing="2" border="0">
                        <asp:Repeater id="rptRatings" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td class="larger bold">
                                        <asp:HiddenField id="hdnRatingCategoryID" runat="server" value='<%#DataBinder.Eval(Container.DataItem, "RatingCategoryId") %>'></asp:HiddenField>
                                        <asp:Literal id="ltlRatingCategory" runat="server" text='<%#DataBinder.Eval(Container.DataItem, "RatingCategory") %>'></asp:Literal>:
                                    </td>
                                    <td><CC:StarRating id="ctlRating" runat="server" MaxRating="10" MinRating="0" Rating='<%#DataBinder.Eval(Container.DataItem, "VendorRating") %>' StarOnImage="/images/rating/star-red.gif" StarOffImage="/images/rating/star-gr.gif"></CC:StarRating></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>                    
                    </table>
                    <p style="margin:10px;text-align:center;">
                        <strong>Make a Comment:</strong><br />
				        <asp:TextBox id="txtComment" style="width: 349px;" Columns="55" rows="5" TextMode="Multiline" runat="server"></asp:TextBox>
				        <br /><br />
                        <asp:Button id="btnSubmit" runat="server" text="Save Ratings and Comments" cssclass="btnred" ValidationGroup="CommentForm" />
				        <asp:Button id="btnClear" runat="server" text="Clear Comments" cssclass="btnred" onclientclick="return ClearComments();" />
                        <CC:RequiredFieldValidatorFront ID="rfvtxtComment" runat="server" ControlToValidate="txtComment" ErrorMessage="Please enter a comment" ValidationGroup="CommentForm"></CC:RequiredFieldValidatorFront>
                    </p>
                </div>
			</div>
			
			<div style="display: none;" id="tcontent11" class="tabcontent">
				<div class="innertab">
					<asp:Repeater id="rptComments" runat="server" enableviewstate="False">
					<HeaderTemplate>
					<table cellpadding="5" cellspacing="0" border="0">
					</HeaderTemplate>
					<ItemTemplate>
						<tr>
							<td width="200px"><strong><%#DataBinder.Eval(Container.DataItem, "CompanyName") & "<br/><span class=""smaller"">(" & DataBinder.Eval(Container.DataItem, "Submitted") & ")</span>"%</strong> </td>
							<td><%#DataBinder.Eval(Container.DataItem, "Comment") </td>
						</tr>
					</ItemTemplate>
					<SeparatorTemplate>
					    <tr>
					        <td colspan="2" style="height:1px;margin:0px;padding:0px;background-color:#000;"><img alt="" src="/images/spacer.gif" style="height:1px;border:none;" /></td>
					    </tr>
					</SeparatorTemplate>
					<FooterTemplate>
					</table>
					</FooterTemplate>
					</asp:Repeater>
					
					<div id="divCommentNone" runat="server">There are no comments for this vendor, be the first to post one! </div>
				</div>
			</div>
		
			<script type="text/javascript">
                initializetabcontent("maintab")
			</script>
		
		</div>
	</div>--%>
</div>
</CT:MasterPage>