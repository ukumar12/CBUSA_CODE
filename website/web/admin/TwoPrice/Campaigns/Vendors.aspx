<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Vendors.aspx.vb" Inherits="admin_TwoPrice_Campaigns_Builders" MasterPageFile="~/controls/AdminMaster.master" Title="Committed Purchase Event Builders" %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
<script src="../../../includes/jquery/jquery.megaselectlist.js" type="text/javascript"></script>
<style>
    .field {
        background: #E2EBF7;
        vertical-align: top;
        width: 588px;
        font-size: 1.3em;
        }
 .megaselectlistcolumn {
                        width: 37%;
                        margin: 0 1%;
                        float: left;
                }
                
                .megaselectlist .currentitem {
                        padding: 0;
                        border: 0.1em dotted Green;
                        color: Black;
                }
        
                /* Optional styles */
                
                .megaselectlist {
                        background-color: #F5F5F5;
                        border: 1px solid Silver;
                        margin:0 15px 0 15px;
                }
                
                .megaselectlistcolumn > h2 {
                        font-size: 1em;
                        background-color: #ECECEC;
                        text-align: center;
                        padding: 2px 0;
                        margin: 0;
                }
                
                .megaselectlist > p {
                        margin: 0.2em;
                        width:565px;
                        float:right;
                        padding-right:50px;
                }
                
                .megaselectlistcolumn > ul {
                        padding-left: 1em;
                        margin: 0;
                        color: Gray;
                        height:500px;
                        overflow:auto;
                }
                
                .megaselectlistcolumn > ul > li {
                        cursor: pointer;
                        padding: 0.1em;
                        font-size: small;
                }
                
                .megaselectlistcolumn > ul > li:hover {
                        background-color: #ECECEC;
                        color: Black;
                }
</style>
<script type="text/javascript">
    $(function () {
        $(".multiSelect").megaselectlist({
            classmodifier: "megaselectlist",
            headers: "rel",
            animate: false,
            animateevent: "click",
            multiple: true,
            itemseparator: '<br>'
        });
        $(".submit").click(function () {
            $('#<%=hdnVendors.ClientID %>').val($('#<%=drpVendors.ClientID %>').val());
        });

        //Set the Selected Values
        var selectedValues = $('#<%=hdnVendors.ClientID %>').val()
        //clear the list
        $('#megaselectlist1 p span').html('');
        //clear the selected options on load
        $('#<%=drpVendors.ClientID %>').html('');
        $('.megaselectlistcolumn ul li').each(function () {
            var thisID = $(this).attr('rel');
            var arr = selectedValues.split(',');
            if ($.inArray(thisID, arr) != -1) {
                $(this).addClass('currentitem');
                $('#<%=drpVendors.ClientID %>').append("<option value='" + thisID + "' selected='true'>" + $(this).text() + "</option>");
                $('#megaselectlist1 p span').append('<br> ' + $(this).text());
            }
            else {
                $(this).removeClass('currentitem');
                var compareText = $(this).text();
                var spanEl = $(this).parent().parent().parent().parent().find("span:contains('" + compareText + "')").text('');
                $('#<%=drpVendors.ClientID %> option').each(function () {
                    if ($(this).val() == thisID) {
                        $(this).remove();
                    }
                });
            }
        });

    });
    
</script>
<asp:HiddenField ID="hdnVendors" runat="server" />
    <asp:Panel ID="pnlMain" runat="server" Visible="false">
        <h3>
            Committed Purchase Event - Participating Vondors</h3>
        <hr />
          <h4 class="campaignTitle">
            <asp:Literal ID="ltlCamapignName" runat="server"></asp:Literal></h4>
        <h5 class="campaignLLClist">
            Markets in Committed Purchase Event:
            <asp:Literal ID="ltlLLCS" runat="server"></asp:Literal></h5>
        <div class="form">
            <div style="padding:15px 0px 9px 20px; width:425px; float:left; text-align:center;">
                 Bid Deadline:  <CC:DatePicker ID="dtBidDeadLine" runat="server"></CC:DatePicker> &nbsp;
                <CC:OneClickButton ID="btnSaveDeadlineDate" runat="server" Text="Save" CssClass="btn submit campaignBtn" style="margin:0px; padding:2px 10px; float:none;" />
            </div>
           
            <label>
               </> <a href="/admin/twoprice/campaigns/default.aspx" target="main" class="btn campaignBtn" style="text-decoration:none;color:black;">Return to event management</a>
                <CC:OneClickButton ID="btnSubmit1" runat="server" Text="Save and Continue" CssClass="btn submit campaignBtn"/></label>
				<div style="clear: both;"></div>
            <p>
                <%--<label for="example">Select an item</label>--%>
                <asp:Label ID="lblBuilders" AssociatedControlID="drpVendors" Text="Selected Vendors: "
                    runat="server"></asp:Label>
                <CC:DropDownListEx ID="drpVendors" runat="server" CssClass="multiSelect" multiple="true"
                    Style="display: none;">
                </CC:DropDownListEx>
            </p>
            <CC:OneClickButton ID="btnSubmit2" runat="server" Text="Save and Continue" CssClass="btn submit campaignBtn" />
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlSendMessage" runat="server" Visible="false">
        <h2>Send Automatic Message</h2>
            <fieldset>
                <legend>Automatic Message Format:</legend>
                <div class="field">
                    <asp:Literal ID="ltlAutoMessage" runat="server"></asp:Literal>
                </div>
            </fieldset>
            <fieldset>
                <legend>Message Details:</legend>
                <div class="field">
                    CC(seperate Emails with commas):
                    <asp:TextBox ID="txtCC" runat="server" Columns="70" Rows="1" MaxLength="255" TextMode="SingleLine"></asp:TextBox>
                    <br />Additional Message Text: 
                    <asp:Textbox ID="txtMessage" runat="server" Columns="70" Rows="10" MaxLength="255" TextMode="MultiLine"></asp:Textbox>
                </div>
                <CC:ConfirmButton ID="btnSendMessage" runat="server" CssClass="btn" Text="Send Message" Message="Are you sure you would like to continue, this Campaign's status will become 'Bidding in Progress'?"/>
            </fieldset>
                
    </asp:Panel>
    <asp:Panel ID="pnlComplete" runat="server" Visible="false">
        <div>
            <h3>Thank you!</h3>
			Your campaign emails have been sent to the appropriate Vendors.
        </div>
    </asp:Panel>
</asp:content>