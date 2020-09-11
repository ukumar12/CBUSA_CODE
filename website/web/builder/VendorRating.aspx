<%@ Page Language="VB" AutoEventWireup="false" CodeFile="VendorRating.aspx.vb" Inherits="builder_VendorRating" %>
<%@ Register TagName="StarRatingDisplay" TagPrefix="CC" Src="~/controls/StarRatingDisplay.ascx" %>
<%@ Register TagName="TabControl" TagPrefix="CC" Src="~/controls/TabControl.ascx" %>
<%@ Register TagName="Search" Src="~/controls/SearchSql.ascx" TagPrefix="CC" %>

<CT:masterpage runat="server" id="CTMain">
    <link href="/includes/tabs.css" rel="stylesheet" type="text/css" />
<script type="text/javascript" src="/includes/tabcontent.js"></script>
<%--   <script type="text/javascript">
 
       function OpenPolicy() {
           var frm = $get('<%=frmPolicy.ClientID %>').control;
        frm._doMoveToCenter();
        frm.Open();
    }
    function ClosePolicy() {
        var cb = document.querySelector('#ae_cbPolicy');
        cb.checked = true;
        $get('<%=frmPolicy.ClientID %>').control.Close();
    }
    
</script>--%>


    <%--<CC:Search ID="ctlSearch" runat="server" KeywordsTextboxId="txtKeywords" />--%>
    <div runat="server" style="margin:5px auto;" class="pckgltgraywrpr">
        <asp:Panel ID="pnlPreferredVendor" runat="server" visible="true" style="padding-top:10px;padding-left:15px;">
                        <h1 class="largest">Select a Preferred Vendor</h1>
	                    <div style="z-index:2; width: 205px;">
		                   <%-- <asp:UpdatePanel ID="upPreferred2" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
		                        <ContentTemplate>--%>
		                            <CC:SearchList ID="slPreferredVendor2" runat="server" Table="Vendor" TextField="CompanyName" ValueField="VendorID" AllowNew="false" AutoPostback="true" Width="200px" ViewAllLength="100" Height="200px" CssClass="searchlist" />

                               <%-- </ContentTemplate>
		                    </asp:UpdatePanel>--%>
	                    </div>
                        <br />
                    </asp:Panel>
		<div class="pckghdgred">Ratings &amp; Comments</div>
		<ul id="maintab" class="shadetabs">
			<li class="selected" id="liRating"><a href="#" rel="tcontent10"><span>Add Review</span></a></li>
			<li class="" id="liComment"><a href="#" rel="tcontent11"><span>Read Comments</span></a></li>
            <%--<li class="selected"><a href="#" rel="tcontent10"><span>Add Review</span></a> <a href="#" rel="tcontent11"><span>Read Comments</span></a></li>--%>
		</ul>
		<br />
		<%--<img src="/images/rating/red.gif" width="100%" height="3" style="border-style:none" alt="" />--%>
		<br />
		<div class="tabdetails">
			<div id="tcontent10" style="display: block; background-color:#e7e7e7;">
                <div style="width:75%;margin:auto;">                
                    <%--<asp:dropdownlist id="ddlVendorList">  </asp:dropdownlist>--%>
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
							<td width="200px"><strong><%#DataBinder.Eval(Container.DataItem, "CompanyName") & "<br/><span class=""smaller"">(" & DataBinder.Eval(Container.DataItem, "Submitted") & ")</span>"%></strong> </td>
							<td><%#DataBinder.Eval(Container.DataItem, "Comment")%> </td>
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
	</div>
</CT:masterpage>