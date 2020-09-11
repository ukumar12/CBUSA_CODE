<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PriceComparison.ascx.vb" Inherits="PriceComparison" %>
<script language="javascript">
    $(document).ready(function() {
    $('#<%= btnUpdateComp.ClientID %>').hide();
    //$('#<%= btnUpdateComp.ClientID %>').trigger('click');

	window.addEventListener('message', function (event) {
        if (event.data.viz == "Dis") {
            var data = event.data;
            document.getElementById('ifrmNCPDashboard').height = data.height;
            $('#divNCPDashboard').show();
            $('#divNCPLoader').hide();
        }
        else
        {
            if (event.data.viz == "dashboard") {
                $('#divBuilderDashboard').show();
                $('#divDashboardLoader').hide();
            }
        }
    }, false);
    });
</script>


<div class="pckgltgraywrpr">
	<div class="pckghdgred nobdr">
		National Contracts
	</div>
    <div id="divNCPDashboard" class="stacktblwrpr themeprice" style="display:none">
		<iframe id="ifrmNCPDashboard" src="https://cbusa.azurewebsites.net/CbusaBuilder/dashboard/index?bldruserid=<%= Session("BuilderAccountId")%>" height="100" scrolling="no" style="border:none;overflow:hidden"></iframe>
    </div>
    <div id="divNCPLoader" class="stacktblwrpr themeprice">
	    <div style="background-color:#fff;height:80px;text-align:center;">
            <img src="/images/loading.gif" alt="Loading..." /><br /><br />
            <h1 class="largest">Loading NCP Dashboard... Please Wait</h1>
        </div>
    </div>
</div>

<br />

<div id="divBuilderDashboardOuter" runat="server" class="pckgltgraywrpr">
    <div class="pckghdgred nobdr">
		<span id="spnReportingQtrYear" runat="server"></span> Builder Performance Report
	</div>
    <div id="divBuilderDashboard" class="stacktblwrpr themeprice" style="display:none">
		<iframe id="ifrmBuilderDashboard" src="https://dev.custombuilders-usa.com/builderdashboard/index.aspx?BuilderId=<%= Session("BuilderId")%>" height="500" width="789" style="border:none;overflow-x:scroll;"></iframe>
    </div>
    <div id="divDashboardLoader" class="stacktblwrpr themeprice">
	    <div style="background-color:#fff;height:80px;text-align:center;">
            <img src="/images/loading.gif" alt="Loading..." /><br /><br />
            <h1 class="largest">Loading Builder Performance Report... Please Wait</h1>
        </div>
    </div>
</div>

<div class="pckgltgraywrpr" style="display:none;">
	<div class="pckghdgred nobdr">
		Price Comparisons
	</div>
    <div class="stacktblwrpr themeprice">
		<div class="bdbtblhdg">
			<div class="caption">Current Jobs</div>
		    <div class="clear">&nbsp;</div>
		</div>
		<table class="larger" cellpadding="2" cellspacing="0" border="0">
		    <tr>
		        <th>Project</th>
		        <th>Takeoff</th>
		        <th>Created</th>
		        <th>Last Updated</th>
		        <th>&nbsp;</th>
		    </tr>
        <asp:Repeater ID="rptJobs" runat="server">
            <ItemTemplate>
                <tr class='<%# iif(Container.ItemIndex mod 2 = 1,"alternate","row") %>'>
                    <td><asp:Literal ID="ltlProject" runat="server"></asp:Literal></td>
                    <td><asp:Literal ID="ltlTakeoff" runat="server"></asp:Literal></td>
                    <td><asp:Literal ID="ltlCreated" runat="server"></asp:Literal></td>
                    <td><asp:Literal ID="ltlUpdated" runat="server"></asp:Literal></td>
                    <td nowrap><asp:Literal ID="ltlLink" runat="server"></asp:Literal></td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
        </table>
    </div>
    <asp:button ID="btnUpdateComp" runat="server" CssClass="btnblue" Text="Update Comparisons" />
    <div class="stacktblwrpr themeprice">
		<div class="bdbtblhdg">
			<div class="caption">My Comparison Templates</div>
            <div class="clear">&nbsp;</div>
        </div>
        <asp:UpdatePanel ID="upComparisons" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
        <ContentTemplate>
        <div id="divProgress" visible="false" runat="server" style="background-color: #fff: height: 80px; text-align: center; padding: 30px 10px;">
            <img src="/images/loading.gif" alt="Processing..." /><br /><br />
            <h1 class="largest">Updating Comparisons... Please Wait</h1>
        </div>
        <asp:Repeater ID="rptComparisons" runat="server">
            <ItemTemplate>
                <table class="larger" cellpadding="2" cellspacing="0" border="0" style="table-layout:fixed;width:100%;margin:5px auto;">
                    <tr class='<%# iif(Container.ItemIndex mod 2 = 1,"alternate","row") %>'>
                        <th style="width:25%;">Takeoff</th>
                        <asp:Literal ID="ltlVendorHeaders" runat="server"></asp:Literal>
                        <th></th>
                    </tr>
                    <tr class='<%# iif(Container.ItemIndex mod 2 = 1,"alternate","row") %>'>
                        <td><asp:Literal ID="ltlTitle" runat="server"></asp:Literal></td>
                        <asp:Literal ID="ltlVendorPrices" runat="server"></asp:Literal>
                        <td align="right"><asp:ImageButton ID="btnRemove" runat="server" AlternateText="Remove" ImageUrl="/images/global/icon-remove.gif" CommandName="Remove" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"PriceComparisonID") %>' /></td>
                    </tr>
                    
                </table>
            </ItemTemplate>
        </asp:Repeater>
        </ContentTemplate>
        </asp:UpdatePanel>
        
    </div>
    
    <div class="stacktblwrpr themeprice" style="display:none">
		<div class="bdbtblhdg">
			<div class="caption">CBUSA Eagle One Comparisons</div>
		    <div class="clear">&nbsp;</div>
		</div>
		<asp:UpdatePanel ID="upAdmin" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
        <ContentTemplate>
        <div id="divProgressEO" visible="false" runat="server" style="background-color: #fff: height: 80px; text-align: center; padding: 30px 10px;">
            <img src="/images/loading.gif" alt="Processing..." /><br /><br />
            <h1 class="largest">Updating Comparisons... Please Wait</h1>
        </div>
        <asp:Repeater ID="rptAdmin" runat="server">
            <ItemTemplate>
                <table class="larger" cellpadding="2" cellspacing="0" border="0" style="table-layout:fixed;width:100%;">
                    <tr class='<%# iif(Container.ItemIndex mod 2 = 1,"alternate","row") %>'>
                        <th style="width:25%;">Takeoff</th>
                        <asp:Literal ID="ltlVendorHeaders" runat="server"></asp:Literal>
                        <th></th>
                    </tr>
                    <tr class='<%# iif(Container.ItemIndex mod 2 = 1,"alternate","row") %>'>
                        <td><asp:Literal ID="ltlTitle" runat="server"></asp:Literal></td>
                        <asp:Literal ID="ltlVendorPrices" runat="server"></asp:Literal>
                        <td align="right"><asp:ImageButton ID="btnRemove" runat="server" AlternateText="Remove" ImageUrl="/images/global/icon-remove.gif" CommandName="Remove" CausesValidation="false" Visible="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"PriceComparisonID") %>' /></td>
                    </tr>

                </table>
            </ItemTemplate>
        </asp:Repeater>
        </ContentTemplate>
        </asp:UpdatePanel>    
    </div>
</div>
<br />
