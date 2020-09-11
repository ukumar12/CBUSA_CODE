<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Header.ascx.vb" Inherits="HeaderCtrl" %>
<%@ Import Namespace="System.Configuration.ConfigurationManager" %>

<script type="text/javascript">
    function ShowNCPMenu() {
        var divNCP = document.getElementById("divNCPMenu");
        divNCP.style.display = "block";
    }

    function HideNCPMenu() {
        var divNCP = document.getElementById("divNCPMenu");
        divNCP.style.display = "none";
    }

    function ShowRebateMenu() {
        var divRebate = document.getElementById("divRebateMenu");
        divRebate.style.display = "block";
    }

    function HideRebateMenu() {
        var divRebate = document.getElementById("divRebateMenu");
        divRebate.style.display = "none";
    }
</script>

<div class="hdrwrpr" style="z-index:2;">

	<div class="hdrlogo"><a href="/"><img src="/images/global/hdr-logo.gif" style="width:223px; height:70px; border-style:none;" alt="CBUSA" /></a><br /></div>
	
	<div class="hdrvendor" style="z-index:2;">
		<asp:UpdatePanel ID="upPreferred" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
		    <ContentTemplate>
		        <CC:SearchList ID="slPreferredVendor" runat="server" Table="Vendor" TextField="CompanyName" ValueField="VendorID" AllowNew="false" AutoPostback="true" Width="200px" ViewAllLength="10" CssClass="searchlist" Visible="false" />
            </ContentTemplate>
		</asp:UpdatePanel>
	</div>
	
    <asp:PlaceHolder ID="phSearch" runat="server">
	<div class="hdrsrchbx">
		<input type="text" name="KEYWORD" maxlength="30" class="hdrsbox" />
	</div>

	<div class="hdrsrchbtn">
		<input type="image" src="/images/global/btn-hdr-search.gif" class="hdrsbtn" alt="Search" />
	</div>
	</asp:PlaceHolder>
    <%--<div class="hdrhelplnk">
        <a href="/Gotodashboard.aspx">go to dashboard|</a><a href="/logout.aspx">Logout</a>
    </di--%>
    <ul id="ulBuilderDashboard" runat="server" class="hdrhelplnk headLogout">
	<li><span id="spnCompanyName" runat="server"></span>: <span id="spnAccountUser" runat="server"></span></li>
        <li><a href="/Gotodashboard.aspx" title="Go To Dashboard"><i class="fa fa-tachometer" aria-hidden="true"></i></a></li>
        <li><a href="/logout.aspx" title="Logout"><i class="fa fa-sign-out" aria-hidden="true"></i></a></li>
    </ul>
    <ul id="ulVendorDashboard" runat="server" class="hdrhelplnk headLogout">
        <li><a href="/Gotodashboard.aspx" title="Go To Dashboard">Go to dashboard</a></li>
        <li><a href="/logout.aspx" title="Logout">Logout</a></li>
    </ul>

	<ul id="ulBuilderLinks" runat="server" class="hdrnav">
        <li><a href="/directory/">DIRECTORY</a></li>
        <li><a href="/builder/twoprice/">COMMITTED PURCHASES</a></li>
        <%--<li><a href="/catalog/">CATALOG</a></li>--%>
        <li><a href="/takeoffs/">TAKE-OFFS</a></li>
        <%--<li><a href="/order/history.aspx">ORDERS</a></li>--%>
        <%--<li><a href="/projects/">PROJECTS</a></li>--%>
       <%-- <li><a href="/rebates/builder-purchases.aspx">REPORTING</a></li>--%>
         <li><asp:HyperLink ID="hypBuilderReporting" runat="server" NavigateUrl="/rebates/builder-purchases.aspx">REPORTING</asp:HyperLink></li>
        <%--<li><a href="/builder/plansonline/">PLANS ONLINE</a></li>--%>
        <li onmouseover="ShowRebateMenu();" onmouseout="HideRebateMenu();">
            <a href="#">REBATES</a>
            <div id="divRebateMenu" style="display:none;width:250px;height:75px;margin-top:6px;background-color:#f7f7f7;position:absolute;box-shadow:rgba(0, 0, 0, 0.75) -1px 0px 5px 0px;"> 
	    	    <ul style="list-style:none;list-style-position:inside;padding-left:5px;">
                    <li style="padding-top:5px;padding-bottom:5px;margin-left:5px !important;"><a href="/builder/rebatestatements.aspx">REBATE ACCOUNT STATEMENT</a></li>
                    <li style="padding-top:5px;padding-bottom:5px;margin-left:5px !important;"><a href="/builder/rebatedistribution.aspx">REBATE DISTRIBUTION REPORT</a></li>
                    <li style="padding-top:5px;padding-bottom:5px;margin-left:5px !important;"><a href="/rebates/rebate-notification.aspx">PAST DUE REBATE REPORT</a></li>
                </ul>
            </div>
        </li>
        <%--<li runat ="server" id="lnkNationalContracts" visible ="false" ><asp:HyperLink ID="hypNationalContracts" runat="server" >NATIONAL CONTRACTS</asp:HyperLink> </li>--%>
        <li runat ="server" id="lnkNationalContracts" onmouseover="ShowNCPMenu();" onmouseout="HideNCPMenu();">	
            <%--<asp:HyperLink ID="hypNationalContracts" runat="server" visible ="false">NATIONAL CONTRACTS</asp:HyperLink>--%>
	        <a href="#">NATIONAL CONTRACT</a>
	        <div id="divNCPMenu" style="display:none;width:240px;height:75px;margin-top:6px;background-color:#f7f7f7;position:absolute;box-shadow:rgba(0, 0, 0, 0.75) -1px 0px 5px 0px;"> 
	    	    <ul style="list-style:none;list-style-position:inside;padding-left:5px;">
                    <li style="padding-top:5px;padding-bottom:5px;margin-left:5px !important;"><a target="_blank" id="lnkNCPContractCentral" runat="server" href="#">CONTRACTS CENTRAL</a></li>
                    <li style="padding-top:5px;padding-bottom:5px;margin-left:5px !important;"><a target="_blank" id="lnkNCPReporting" runat="server" href="#">QUARTERLY REBATE REPORT</a></li>
                    <li style="padding-top:5px;padding-bottom:5px;margin-left:5px !important;"><a target="_blank" id="lnkNCPHelp" runat="server" href="#">HELP</a></li>
                </ul>
            </div>
        </li>
	</ul>
	
	<ul id="ulVendorLinks" runat="server" class="hdrnav">
        <li><a href="/directory/">DIRECTORY</a></li>
        <li><a href="/vendor/twoprice/">COMMITTED PURCHASES</a></li>
        <li><a href="/forms/vendor-registration/sku-price.aspx">UPDATE SKUs AND PRICING</a></li>
        <%--<li><a href="/catalog/">CATALOG</a></li>--%>
        <li><a href="/vendor/invoice/">ORDERS</a></li>
        <li><a runat="server" id="lnkVendorReporting" href="/rebates/vendor-sales.aspx">REPORTING</a></li>
        <li><a href="/rebates/RebateStatements-Vendor.aspx">REBATE INVOICES</a></li>
        <%--<li><a href="/vendor/plansonline/">PLANS ONLINE</a></li>--%>
	</ul>	

	<ul id="ulPIQLinks" runat="server" class="hdrnav">
		<li><a href="/piq/accounts.aspx">USER ACCOUNTS</a>
        <li><a href="/directory/">DIRECTORY</a>
        <li><a href="/changepw.aspx">UPDATE ACCOUNT</a>
        <li><a href="/catalog/">CATALOG</a>
	</ul>	
</div>
