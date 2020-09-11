function createPurchaseVolumeHistoryGraph() {
    var strQtrYears = $("#hdnQuarterYearValues").val();
    var strBuilderPurchases = $("#hdnBuilderPurchases").val();
    var strLLCAveragePurchases = $("#hdnLLCAvgPurchases").val();

    var QtrYearAxis = strQtrYears.split(",");
    var BuilderPurchases = strBuilderPurchases.split(",");
    var LLCAvgPurchases = strLLCAveragePurchases.split(",");

    $("#divPurchaseVolumeHistory").kendoChart({
        legend: {
            position: "bottom",
			align: "center",
			orientation: "horizontal",
            spacing: 80,
			labels: {
                font: "9px Arial,Helvetica,sans-serif",
                margin: 20
			},
        },
        chartArea: {
            width: 375,
            height: 250,
			margin: 20,
            background: ""
        },
		plotArea: {
            width: 375,
            height: 240,
            background: ""
        },
        seriesDefaults: {
            type: "line",
            style: "smooth"
        },
        series: [{
            name: "Your Purchases",
            color: "#095394",
            data: BuilderPurchases
        }, {
            name: "Market Average",
            color: "#808080",
            data: LLCAvgPurchases
        }],
        valueAxis: {
            majorUnit: 200,
            labels: {
                font: "8px Arial,Helvetica,sans-serif",
                format: "{0}K"
            },
            line: {
                visible: false
            },
            axisCrossingValue: -10
        },
        categoryAxis: {
            categories: QtrYearAxis,
            majorGridLines: {
                visible: false
            },
            labels: {
                font: "8px Arial,Helvetica,sans-serif",
                rotation: "auto"
            }
        },
        tooltip: {
            visible: true,
            format: "{0}K",
            color: "#ffffff",
            template: "#= series.name #: #= value #K"
        }
    });
	
	$("#divPurchaseVolumeHistory").data("kendoChart").redraw();
}

function AssignControls() {
    $("div.row-toggle").click(function () {
        var dataCategory = $(this).data("category");
        $("tr.vendor-row[data-category='" + dataCategory + "']").slideToggle(100);

        var ifaCaret = $("i.row-toggle[data-category='" + dataCategory + "']");

        if (ifaCaret.hasClass("fa-caret-up")) {
            ifaCaret.removeClass("fa-caret-up");
            ifaCaret.addClass("fa-caret-down");
        } else {
            ifaCaret.removeClass("fa-caret-down");
            ifaCaret.addClass("fa-caret-up");
        }
    });

    $("#mulselCategory").kendoMultiSelect().data("kendoMultiSelect");
    $("#mulselBuilder").kendoMultiSelect().data("kendoMultiSelect");
}

function PopulateFilterSelection() {
    var selCategories = $("#mulselCategory").data("kendoMultiSelect").value();
    var strCategories = "";

    for (var i = 0; i < selCategories.length; i++) {
        strCategories = strCategories + selCategories[i] + ",";
    }

    var selBuilders = $("#mulselBuilder").data("kendoMultiSelect").value();
    var strBuilders = "";

    for (var i = 0; i < selBuilders.length; i++) {
        strBuilders = strBuilders + selBuilders[i] + ",";
    }

    $("#hdnSelectedCategories").val(strCategories);
    $("#hdnSelectedBuilders").val(strBuilders);
}

$(document).ready();

$(document).ready(function () {

    createPurchaseVolumeHistoryGraph();
    $(document).bind("kendo:skinChange", createPurchaseVolumeHistoryGraph);

    $(".market-vendor-use").clone(true).appendTo('#divVendorUseMatrix').addClass('clone');  

    $("tr.vendor-row").slideToggle(1000);
    AssignControls();

    var IsPostBack = $("#hdnPostBack").val();

    if (IsPostBack == "true") {
        $("#txtDummy").focus();
    }

    window.parent.postMessage({
        'viz': 'dashboard',
        'location': window.location.href
    }, "*");
});
