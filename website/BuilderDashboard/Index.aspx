<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Index.aspx.vb" Inherits="Index" EnableViewState="true" %>
<html lang="en">
<head>
<!-- Required meta tags -->
<meta charset="utf-8">
<meta http-equiv="X-UA-Compatible" content="IE=edge">
<meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
<link href="Content/images/favicon.ico" rel="shortcut icon" type="image/x-icon" />
<title>Dashboard</title>

<!-- Stylesheet -->
<link rel="stylesheet" href="Content/css/bootstrap.min.css" />
<link rel="stylesheet" href="Content/css/font-awesome.min.css" />
<link rel="stylesheet" href="Content/css/style.css" />
<link rel="stylesheet" href="Content/css/global.css" />
<link href="https://fonts.googleapis.com/css?family=Montserrat:300i,400,500,600,700&display=swap" rel="stylesheet" />
<link rel="stylesheet" href="Content/css/responsive.css" />
<link rel="stylesheet" href="https://kendo.cdn.telerik.com/2018.1.221/styles/kendo.common.min.css" />
<link rel="stylesheet" href="Content/kendo/kendo.default.min.css" />
<link rel="stylesheet" href="Content/kendo/kendo.default.mobile.min.css" />

<!-- jquery library -->
<script src="Content/Scripts/jquery.min.js"></script>
<script src="Content/Scripts/bootstrap.min.js"></script>
<script src="Content/Scripts/kendo.all.min.js"></script>
<script src="Content/Scripts/Custom/BuilderDashboard.js"></script>

<!--[if lt IE 9]>
      <script src="https://oss.maxcdn.com/html5shiv/3.7.3/html5shiv.min.js"></script>
      <script src="https://oss.maxcdn.com/respond/1.4.2/respond.min.js"></script>
    <![endif]-->
</head>

<body>
<form id="frmBuilderDashboard" runat="server">

    
<!-- Start Header -->
<div class="wrapper-area">
    <div class="container-fluid">
        <header>
            <div class="row">
                <!-- top tiles -->
                <div class="col-sm-6">
                    <div class="row tile_count">

                        <div class="col-sm-12">
                    	    <h2 class="main-heading text-uppercase">Purchase Volume</h2>
                        </div>

                        <div class="col-md-4 col-sm-4">
                            <div class="tile_stats_count">
                                <div class="count_bottom"><asp:Label ID="lblReportingQtrYear" runat="server"></asp:Label> Purchases</div>
                                <span class="count_top">
                                    <span class="count"><asp:Label ID="lblReportingQtrTotalPurchase" runat="server"></asp:Label></span>
                                    <span id="spnReportingQtrProfitLoss" runat="server">
                                        <span id="spnReportingQtrUpDownIndicator" runat="server" class="block-area"><i class="fa fa-caret-up"></i></span>
                                        <asp:Label ID="lblReportingQtrUpDownIndicator" runat="server"></asp:Label>
                                    </span>
                                </span>
                            </div>
                        </div>

                        <div class="col-md-4 col-sm-4">
                            <div class="tile_stats_count">
                                <div class="count_bottom">Rolling 4-Qtr Purchases</div>
                                <span class="count_top">
                                    <span class="count"><asp:Label ID="lblFourQtrTotalPurchase" runat="server"></asp:Label></span> 
                                    <span id="spnFourQtrProfitLoss" runat="server">
                                        <span id="spnFourQtrUpDownIndicator" runat="server" class="block-area"><i class="fa fa-sort-desc"></i></span>
                                        <asp:Label ID="lblFourQtrUpDownIndicator" runat="server"></asp:Label>
                                    </span>
                                </span>
                            </div>
                        </div>

                        <div class="col-md-4 col-sm-4">
                            <div class="tile_stats_count">
                                <div class="count_bottom">Purchase Volume Rank</div>
                                <span class="count_top">
                                    <span class="count text-center full-width">
                                        <asp:Label ID="lblPurchaseVolumeRank" runat="server"></asp:Label>
                                    </span> 
                                </span>
                            </div>
                        </div>

                    </div>

                    <div class="row tile_count">

                 	    <div class="col-sm-12">
                    	    <h2 class="main-heading redBg text-uppercase">Vendor Usage</h2>
                        </div>

                        <div class="col-md-4 col-sm-4">
                            <div class="tile_stats_count red-border">
                                <div class="count_bottom"><asp:Label ID="lblVendorUsagePercentQtrYear" runat="server"></asp:Label> Vendor Usage %</div>
                                <span class="count_top">
                                    <span class="count text-center">
                                        <asp:Label ID="lblVendorUsagePercent" runat="server"></asp:Label>
                                    </span>
                                    <span id="spnVendorUsagePercentProfitLoss" runat="server">
                                        <span id="spnVendorUsagePercentIndicator" runat="server" class="block-area"><i class="fa fa-sort-desc"></i></span>
                                        <asp:Label ID="lblVendorUsagePercentUpDownIndicator" runat="server"></asp:Label>
                                    </span>
                                </span>
                            </div>
                        </div>

                        <div class="col-md-4 col-sm-4">
                            <div class="tile_stats_count red-border">
                                <div class="count_bottom">Rolling 4-Qtr Vendors Used</div>
                                <span class="count_top">
                                    <span class="count">
                                        <asp:Label ID="lblFourQtrVendorUse" runat="server"></asp:Label>
                                    </span>
                                    <span id="spnFourQtrVendorUseProfitLoss" runat="server">
                                        <span id="spnFourQtrVendorUsePercentIndicator" runat="server" class="block-area"><i class="fa fa-sort-desc"></i></span>
                                        <asp:Label ID="lblFourQtrVendorUsePercentUpDownIndicator" runat="server"></asp:Label>
                                    </span>
                                </span>
                            </div>
                        </div>
                    
                        <div class="col-md-4 col-sm-4">
                            <div class="tile_stats_count red-border">
                                <div class="count_bottom">Vendor Use Rank</div>
                                <span class="count_top">
                                    <span class="count text-center full-width">
                                        <asp:Label ID="lblVendorUseRank" runat="server"></asp:Label>
                                    </span> 
                                </span>
                            </div>
                        </div>

                    </div>
                </div>
                <!-- top tiles -->
                
                <div class="col-sm-6">
            	    <div class="row volume-history">
                        <div class="col-sm-12" style="margin-left:-20px;">
                            <h2 class="main-heading text-uppercase">Your purchase volume history</h2>
                            <div class="demo-section k-content wide">
                                <div id="divPurchaseVolumeHistory"></div>
                            </div>
                            <asp:HiddenField ID="hdnQuarterYearValues" runat="server" ClientIDMode="Static" Value="" />
                            <asp:HiddenField ID="hdnBuilderPurchases" runat="server" ClientIDMode="Static" Value="" />
                            <asp:HiddenField ID="hdnLLCAvgPurchases" runat="server" ClientIDMode="Static" Value="" />
                        </div>
                    </div>
                </div>

            </div>

        </header>
    <!-- End Header -->

    <!-- Start Body -->
    <main>
        <div class="clearfix"></div>

        <div class="row">
            <div class="col-sm-12">
                <div class="quarter-section">
                    <h2 class="main-heading text-uppercase">YOUR VENDOR UTILIZATION FOR THIS QUARTER</h2>

                    <div class="row duplicate-section">

                    	<div id="divDrywallWrapper" runat="server" class="col-sm-4">
                            <div id="divDrywall" runat="server" class="quarter-block">
                                
                                <div class="quarter-left">
                                    <figure class="figure">
                                      <img class="img-responsive" src="Content/images/Drywall.jpg" alt="Drywall">
                                      <figcaption class="figure-caption">Drywall <asp:Label ID="lblDrywallTotalSalesVolume" runat="server" CssClass="product-price"></asp:Label></figcaption>
                                    </figure>
                                </div>
                                <div class="quarter-right">
                                    <ul id="ulDrywallTopVendors" runat="server"></ul>
                                </div>
                                <div class="quarter-block-tagline">
                                    <asp:Label ID="lblDrywallMarketSpendPercent" runat="server"></asp:Label> the total market spend
                                </div>
                            </div>
                        </div>

                        <div id="divFlooringWrapper" runat="server" class="col-sm-4">
                            <div id="divFlooring" runat="server" class="quarter-block">
                                
                                <div class="quarter-left">
                                    <figure class="figure">
                                      <img class="img-responsive" src="Content/images/Flooring.jpg" alt="Flooring">
                                      <figcaption class="figure-caption">Flooring <asp:Label ID="lblFlooringTotalSalesVolume" runat="server" CssClass="product-price"></asp:Label></figcaption>
                                    </figure>
                                </div>
                                <div class="quarter-right">
                                    <ul id="ulFlooringTopVendors" runat="server"></ul>
                                </div>
                                <div class="quarter-block-tagline">
                                    <asp:Label ID="lblFlooringMarketSpendPercent" runat="server"></asp:Label> the total market spend
                                </div>
                            </div>
                        </div>

                        <div id="divGarageDoorWrapper" runat="server" class="col-sm-4">
                            <div id="divGarageDoor" runat="server" class="quarter-block">
                                
                                <div class="quarter-left">
                                    <figure class="figure">
                                      <img class="img-responsive" src="Content/images/GarageDoor.jpg" alt="Garage Doors">
                                      <figcaption class="figure-caption">Garage Doors <asp:Label ID="lblGarageDoorsTotalSalesVolume" runat="server" CssClass="product-price"></asp:Label></figcaption>
                                    </figure>
                                </div>
                                <div class="quarter-right">
                                    <ul id="ulGarageDoorsTopVendors" runat="server"></ul>
                                </div>
                                <div class="quarter-block-tagline">
                                    <asp:Label ID="lblGarageDoorsMarketSpendPercent" runat="server"></asp:Label> the total market spend
                                </div>
                            </div>
                        </div>
                        
                    	<div id="divHVACWrapper" runat="server" class="col-sm-4">
                            <div id="divHVAC" runat="server" class="quarter-block">
                                
                                <div class="quarter-left">
                                    <figure class="figure">
                                      <img class="img-responsive" src="Content/images/HVAC.jpg" alt="HVAC">
                                      <figcaption class="figure-caption">HVAC <asp:Label ID="lblHVACTotalSalesVolume" runat="server" CssClass="product-price"></asp:Label></figcaption>
                                    </figure>
                                </div>
                                <div class="quarter-right">
                                    <ul id="ulHVACTopVendors" runat="server"></ul>
                                </div>
                                <div class="quarter-block-tagline">
                                    <asp:Label ID="lblHVACMarketSpendPercent" runat="server"></asp:Label> the total market spend
                                </div>
                            </div>
                        </div>

                        <div id="divInsulationWrapper" runat="server" class="col-sm-4">
                            <div id="divInsulation" runat="server" class="quarter-block">
                                
                                <div class="quarter-left">
                                    <figure class="figure">
                                      <img class="img-responsive" src="Content/images/Insulation.jpg" alt="Insulation">
                                      <figcaption class="figure-caption">Insulation <asp:Label ID="lblInsulationTotalSalesVolume" runat="server" CssClass="product-price"></asp:Label></figcaption>
                                    </figure>
                                </div>
                                <div class="quarter-right">
                                    <ul id="ulInsulationTopVendors" runat="server"></ul>
                                </div>
                                <div class="quarter-block-tagline">
                                    <asp:Label ID="lblInsulationMarketSpendPercent" runat="server"></asp:Label> the total market spend
                                </div>
                            </div>
                        </div>

                        <div id="divKitchenAndBathWrapper" runat="server" class="col-sm-4">
                            <div id="divKitchenAndBath" runat="server" class="quarter-block">
                                
                                <div class="quarter-left">
                                    <figure class="figure">
                                      <img class="img-responsive" src="Content/images/KitchenBath.jpg" alt="Kitchen and Bath">
                                      <figcaption class="figure-caption">Kitchen and Bath <asp:Label ID="lblKitchenBathTotalSalesVolume" runat="server" CssClass="product-price"></asp:Label></figcaption>
                                    </figure>
                                </div>
                                <div class="quarter-right">
                                    <ul id="ulKitchenBathTopVendors" runat="server"></ul>
                                </div>
                                <div class="quarter-block-tagline">
                                    <asp:Label ID="lblKitchenBathMarketSpendPercent" runat="server"></asp:Label> the total market spend
                                </div>
                            </div>
                        </div>
                        
                    	<div id="divLightingWrapper" runat="server" class="col-sm-4">
                            <div id="divLighting" runat="server" class="quarter-block">
                                
                                <div class="quarter-left">
                                    <figure class="figure">
                                      <img class="img-responsive" src="Content/images/Lighting.jpg" alt="Lighting">
                                      <figcaption class="figure-caption">Lighting <asp:Label ID="lblLightingTotalSalesVolume" runat="server" CssClass="product-price"></asp:Label></figcaption>
                                    </figure>
                                </div>
                                <div class="quarter-right">
                                    <ul id="ulLightingTopVendors" runat="server"></ul>
                                </div>
                                <div class="quarter-block-tagline">
                                    <asp:Label ID="lblLightingMarketSpendPercent" runat="server"></asp:Label> the total market spend
                                </div>
                            </div>
                        </div>

                        <div id="divRoofingWrapper" runat="server" class="col-sm-4">
                            <div id="divRoofing" runat="server" class="quarter-block">
                                
                                <div class="quarter-left">
                                    <figure class="figure">
                                      <img class="img-responsive" src="Content/images/Roofing.jpg" alt="Roofing">
                                      <figcaption class="figure-caption">Roofing <asp:Label ID="lblRoofingTotalSalesVolume" runat="server" CssClass="product-price"></asp:Label></figcaption>
                                    </figure>
                                </div>
                                <div class="quarter-right">
                                    <ul id="ulRoofingTopVendors" runat="server"></ul>
                                </div>
                                <div class="quarter-block-tagline">
                                    <asp:Label ID="lblRoofingMarketSpendPercent" runat="server"></asp:Label> the total market spend
                                </div>
                            </div>
                        </div>

                        <div id="divSupplyHouseWrapper" runat="server" class="col-sm-4">
                            <div id="divSupplyHouse" runat="server" class="quarter-block">
                                
                                <div class="quarter-left">
                                    <figure class="figure">
                                      <img class="img-responsive" src="Content/images/SupplyHouse.jpg" alt="Supply House">
                                      <figcaption class="figure-caption">Supply House <asp:Label ID="lblSupplyHouseTotalSalesVolume" runat="server" CssClass="product-price"></asp:Label></figcaption>
                                    </figure>
                                </div>
                                <div class="quarter-right">
                                    <ul id="ulSupplyHouseTopVendors" runat="server"></ul>
                                </div>
                                <div class="quarter-block-tagline">
                                    <asp:Label ID="lblSupplyHouseMarketSpendPercent" runat="server"></asp:Label> the total market spend
                                </div>
                            </div>
                        </div>
                    </div>

                    <h2 class="main-heading text-uppercase">MARKET-WIDE VENDOR UTILIZATION FOR ROLLING 4-QUARTER PURCHASES</h2>
                    <div class="row demo-section k-content market-vendor-utilization-main">
                        <div class="col-sm-5">
                            <select id="mulselCategory" runat="server" multiple="true" data-placeholder="Select Categories" style="width:100%;padding-right:0px;"></select>
                        </div>
                        <div class="col-sm-5">
                            <select id="mulselBuilder" runat="server" multiple="true" data-placeholder="Select Builders" style="width:100%;padding-right:0px;"></select>
                        </div>
                        <div class="col-sm-2">
                            <asp:Button ID="btnBuilderCategoryFilter" runat="server" Text="GO" class="btn btn-default" style="padding-top:3px;padding-bottom:3px;" OnClientClick="PopulateFilterSelection();" />
                            <asp:HiddenField ID="hdnSelectedCategories" runat="server" value="" ViewStateMode="Enabled" />
                            <asp:HiddenField ID="hdnSelectedBuilders" runat="server" value="" ViewStateMode="Enabled" />
                        </div>
                    </div>
                    <div>&nbsp;</div>
                    <div id="divVendorUseMatrix" class="market-vendorusage-wrapper">
                        <div class="table-wrapper-inner">
                            <table id="tblVendorUse" runat="server" class="market-vendor-use">

                            </table>
                        </div>
                        <input id="txtDummy" type="text" value="" style="display:block;width:0px;height:0px;border:none;" />
                        <input id="hdnPostBack" runat="server" type="hidden" value="false" enableviewstate="true" />
                    </div>
                </div>
            </div>
        </div>
    </main>
    
    </div>
</div>
<!-- End Body -->
</form>

</body>
</html>