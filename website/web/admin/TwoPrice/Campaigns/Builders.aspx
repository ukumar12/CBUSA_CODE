<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Builders.aspx.vb" Inherits="admin_TwoPrice_Campaigns_Builders" MasterPageFile="~/controls/AdminMaster.master" Title="Committed Purchase Event Builders"%>
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
                        width: 47%;
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
                        margin: 1.2em;
                        width:308px;
                        float:right;
                }
                
                .megaselectlistcolumn > ul {
                        padding-left: 1em;
                        margin: 0;
                        color: Gray;
                        height:auto;
                        overflow:auto;
                }
                
                .megaselectlistcolumn > ul > li {
                        cursor: pointer;
                        padding: 0.1em;
                        font-size: small;
                }
                
                .megaselectlistcolumn > ul > li:hover {
                        background-color: #bfcdec !important;
                        color: Black;
                }
                .megaselectlistoptions 
                {
                    width:731px;
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
            itemseparator: "<br>",
        });
        $(".submit").click(function () {
            $('#<%=hdnBuilders.ClientID %>').val($('#<%=drpbuilders.ClientID %>').val());
        });
		
		$(".megaselectlistcolumn ul li:even").css("background","#E5E5E5");
		$(".megaselectlistcolumn ul li:odd").css("background","#ffffff");

        var selectedValues = $('#<%=hdnBuilders.ClientID %>').val()
        //clear the list
        $('#megaselectlist1 p span').html('');
        //clear the selected options on load
        $('#<%=drpBuilders.ClientID %>').html('');
        $('.megaselectlistcolumn ul li').each(function () {
            var thisID = $(this).attr('rel');
            var arr = selectedValues.split(',');
            if ($.inArray(thisID, arr) != -1) {
                $(this).addClass('currentitem');
                $('#<%=drpBuilders.ClientID %>').append("<option value='" + thisID + "' selected='true'>" + $(this).text() + "</option>");
                $('#megaselectlist1 p span').append('<br> ' + $(this).text());
            }
            else {
                $(this).removeClass('currentitem');
                var compareText = $(this).text();
                var spanEl = $(this).parent().parent().parent().parent().find("span:contains('" + compareText + "')").text('');
                $('#<%=drpBuilders.ClientID %> option').each(function () {
                    if ($(this).val() == thisID) {
                        $(this).remove();
                    }
                });
            }
        });

    });
    
</script>
<asp:HiddenField ID="hdnBuilders" runat="server" />
    <asp:Panel ID="pnlMain" runat="server" Visible="false">
        <h3>
            Committed Purchase Event - Participating Builders</h3>
        <hr />
        <h4 class="campaignTitle">
            <asp:Literal ID="ltlCamapignName" runat="server"></asp:Literal></h4>
        <h5 class="campaignLLClist">
            Markets in Committed Purchase Event:<br />
            <asp:Literal ID="ltlLLCS" runat="server"></asp:Literal></h5>
        <div class="form">
            <label>
                <a href="/admin/twoprice/campaigns/default.aspx" target="main" class="btn campaignBtn" style="text-decoration:none;color:black;">Return to event management</a>
                <CC:OneClickButton ID="btnSubmit1" runat="server" Text="Submit" CssClass="btn submit campaignBtn" /></label>
				<div style="clear: both;"></div>
            <p>
                <%--<label for="example">Select an item</label>--%>
                <asp:Label ID="lblBuilders" AssociatedControlID="drpBuilders" Text="Selected builders"
                    runat="server"></asp:Label>
                <CC:DropDownListEx ID="drpbuilders" runat="server" CssClass="multiSelect" multiple="true"
                    Style="display: none;">
                </CC:DropDownListEx>
				
            </p>
			
            <CC:OneClickButton ID="btnSubmit2" runat="server" Text="Submit" CssClass="btn submit campaignBtn" />
			
        </div>
		
    </asp:Panel>
</asp:content>