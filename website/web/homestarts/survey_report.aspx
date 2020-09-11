<%@ Page Language="VB" AutoEventWireup="false" CodeFile="survey_report.aspx.vb" Inherits="homestarts_survey_report" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>


    <form id="form1" runat="server">

        <table style="width: 100%">
            <tr>
                <td style=""></td>
                <td style="width: 45%; text-align: center;">
                    <img src="img/cbusa-emailHeader.jpg" />
                    <hr />
                </td>
                <td style=""></td>
            </tr>
            <tr style="height: 450px;">
                <td style=""></td>
                <td style="width: 45%; text-align: center;" valign="top">
                    <table style="width: 100%">
                        <tr>
                            <td colspan="3" align="center">
                                <table style="width: 100%;">
                                    <tr>
                                        <td align="left">
                                            <asp:Label runat="server" ID="lblCompanyName" CssClass="info" Text=""></asp:Label></td>
                                        <td align="right">
                                            <asp:Label runat="server" ID="lblUserName" CssClass="info" Text=""></asp:Label></td>

                                    </tr>
                                </table>
                            </td>
                        </tr>

                        <tr style="height: 10px;">
                            <td colspan="3"></td>

                        </tr>
                        <tr>
                            <td style="width: 1%"></td>
                            <td style="width: 98%; text-align: center"><span class="survey-header">Monthly Home Starts Survey</span></td>
                            <td style="width: 1%"></td>
                        </tr>
                        <tr style="height: 15px;">
                            <td colspan="3"></td>

                        </tr>
                        <tr>
                            <td style="width: 1%"></td>
                            <td style="width: 98%;">
                                <asp:Panel runat="server" ID="pnlChart" Visible="false">
                                    <table style="width: 100%">
                                        <tr>
                                            <td colspan="2" align="center">
                                                <span class="chart_header1">Network</span><br />
                                                <span class="chart_header2">(<%= NetworkPer %>% of CBUSA network reported)</span>
                                                <br /><br />
                                                <div style="width: 100%;">
                                                    <canvas id="canvasNetwork"></canvas>
                                                </div>

                                            </td>
                                        </tr>
                                        <tr style="height: 30px;">
                                            <td colspan="2"></td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" align="center">

                                                <span class="chart_header1"><%= MarketName %></span><br />
                                                <span class="chart_header2">(<%= MarketPer %>% of your market reported)</span>
                                                <br /><br />
                                                <div style="width: 100%;">
                                                    <canvas id="canvasMarket"></canvas>
                                                </div>

                                            </td>
                                        </tr>


                                    </table>
                                </asp:Panel>

                                <asp:Panel runat="server" ID="pnlException" Visible="false">
                                    <div class="message">
                                        <asp:Literal runat="server" ID="ltrlException"></asp:Literal>
                                    </div>
                                </asp:Panel>
                            </td>
                            <td style="width: 1%"></td>
                        </tr>
                    </table>

                </td>
                <td style=""></td>

            </tr>
            <tr style="height: 25px;">
                <td colspan="3"></td>
            </tr>

            <tr>
                <td style=""></td>
                <td style="width: 45%; text-align: center;">
                    <%--<img src="img/cbusa-emailFooter.jpg" />--%>
                    <div class="footer">
                        <span>WWW.CBUSA.US</span>
                    </div>
                </td>
                <td style=""></td>
            </tr>

        </table>

    </form>
</body>
</html>
<style>
    html {
        font-family: Arial,Helvetica,Verdana,sans-serif;
    }

    .chart_header1 {
        color: #0e2d50;
        font-size: 18px;
        font-weight: bold;
        text-align: center;
    }

    .chart_header2 {
        color: #0e2d50;
        font-size: 12px;
        font-weight: bold;
        text-align: center;
    }

    #chartjs-tooltip {
        opacity: 0;
        position: absolute;
        background: rgba(0, 0, 0, .7);
        color: white;
        padding: 3px;
        border-radius: 3px;
        -webkit-transition: all .1s ease;
        transition: all .1s ease;
        pointer-events: none;
        -webkit-transform: translate(-50%, 0);
        transform: translate(-50%, 0);
    }

    hr {
        padding: 2px 20px;
        background-color: #ef0b35;
        border: none;
    }

    .survey-header {
        color: #0e2d50;
        font-size: 25px;
        font-weight: bold;
        text-align: center;
        /*margin-left: -100px;*/
    }

    .info {
        font-weight: 600;
        color: #656263;
        font-size: 18px;
    }

    .message {
        padding: 10px;
        line-height: 20px;
        color: #656263;
        font-size: 16px;
        font-weight: 50;
    }

    .messag a, message a {
        color: #AFE7FB;
    }

    .error {
        color: red;
        font-size: 10px;
    }

    .footer {
        background-color: #0e2d50;
        width: 100%;
        height: 80px;
    }

        .footer span {
            text-align: center;
            vertical-align: middle;
            color: #fff;
            font-size: 11px;
            font-weight: bold;
            margin-top: 38px;
            display: inline-block;
        }
</style>

<script src="scripts/Chart.min.js"></script>
<script>

    window.onload = function () {
    
        LoadNetworkGraph();
        LoadMarketGraph()
    } 
   

    function LoadNetworkGraph() {
        var ctx = document.getElementById('canvasNetwork').getContext('2d');
        window.myBar = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: [],
                datasets: <%=NetworkDataJson %>
                },
            options: {
                hover: {
                    animationDuration: 0
                },
                animation: {
                    duration: 1,
                    onComplete: function () {
                        
                        var chartInstance = this.chart,
                            ctx = chartInstance.ctx;
                        ctx.font = Chart.helpers.fontString(Chart.defaults.global.defaultFontSize, Chart.defaults.global.defaultFontStyle, Chart.defaults.global.defaultFontFamily);
                        ctx.textAlign = 'center';
                        ctx.textBaseline = 'bottom';
                        var gradient = ctx.createLinearGradient(0, 0, 50, 0);
                        gradient.addColorStop("0", "magenta");
                        gradient.addColorStop("0.5", "blue");
                       

                        this.data.datasets.forEach(function (dataset, i) {
                            var meta = chartInstance.controller.getDatasetMeta(i);
                            meta.data.forEach(function (bar, index) {
                                var data = dataset.data[index]; 
                                //ctx.fillText(data, bar._model.x, bar._model.y-5);
                               
                                if( parseInt( bar._model.y) < 200){
                                    gradient.addColorStop("1.0", "white");
                                    ctx.fillStyle = gradient;
                                    ctx.fillText(data, bar._model.x, bar._model.y+25);
                                }else
                                {
                                    gradient.addColorStop("1.0", "black");
                                    ctx.fillStyle = gradient;
                                    ctx.fillText(data, bar._model.x, bar._model.y-5);
                                }
                            });
                        });
                    }
                },
                responsive: true,
                legend: {
                    position: 'bottom',
                },
                title: {
                    display: false,
                    text: 'Network',
                    fontSize: 18,
                    fontColor:'#325897'
                },
                scales: {
                    yAxes: [{
                        ticks: {
                            beginAtZero: true
                        }
                    }]
                },
                tooltips: {
                    // Disable the on-canvas tooltip
                    enabled: false,

                    custom: function (tooltipModel) {
                        // Tooltip Element
                        
                        var tooltipEl = document.getElementById('chartjs-tooltip');

                        // Create element on first render
                        if (!tooltipEl) {
                            tooltipEl = document.createElement('div');
                            tooltipEl.id = 'chartjs-tooltip';
                            tooltipEl.innerHTML = '<table></table>';
                            document.body.appendChild(tooltipEl);
                        }

                        // Hide if no tooltip
                        if (tooltipModel.opacity === 0) {
                            tooltipEl.style.opacity = 0;
                            return;
                        }

                        // Set caret Position
                        tooltipEl.classList.remove('above', 'below', 'no-transform');
                        if (tooltipModel.yAlign) {
                            tooltipEl.classList.add(tooltipModel.yAlign);
                        } else {
                            tooltipEl.classList.add('no-transform');
                        }

                        function getBody(bodyItem) {
                            return bodyItem.lines;
                        }

                        // Set Text
                        if (tooltipModel.body) {
                            var bodyLines = tooltipModel.body.map(getBody);

                            var innerHtml = '<thead>';
                            innerHtml += '</thead><tbody>';

                            bodyLines.forEach(function (body, i) {
                                var colors = tooltipModel.labelColors[i];
                                var style = 'background:' + colors.backgroundColor;
                                style += '; border-color:' + colors.borderColor;
                                style += '; border-width: 2px';
                                var span = '<span style="' + style + '"></span>';
                                innerHtml += '<tr><td>' + span + body + '</td></tr>';
                            });
                            innerHtml += '</tbody>';

                            var tableRoot = tooltipEl.querySelector('table');
                            tableRoot.innerHTML = innerHtml;
                        }

                        // `this` will be the overall tooltip
                        var position = this._chart.canvas.getBoundingClientRect();

                        // Display, position, and set styles for font
                        tooltipEl.style.opacity = 1;
                        tooltipEl.style.position = 'absolute';
                        tooltipEl.style.left = position.left + window.pageXOffset + tooltipModel.caretX + 'px';
                        tooltipEl.style.top = position.top + window.pageYOffset + tooltipModel.caretY + 'px';
                        tooltipEl.style.fontFamily = tooltipModel._bodyFontFamily;
                        tooltipEl.style.fontSize = tooltipModel.bodyFontSize + 'px';
                        tooltipEl.style.fontStyle = tooltipModel._bodyFontStyle;
                        tooltipEl.style.padding = tooltipModel.yPadding + 'px ' + tooltipModel.xPadding + 'px';
                        tooltipEl.style.pointerEvents = 'none';
                    }
                }

            }
        });
    };

    
    function LoadMarketGraph() {
        var ctx = document.getElementById('canvasMarket').getContext('2d');
        window.myBar = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: [],
                datasets: <%=MarketDataJson %>
                },
            options: {
                animation: {
                    duration: 1,
                    onComplete: function () {
                        var chartInstance = this.chart,
                            ctx = chartInstance.ctx;
                        ctx.font = Chart.helpers.fontString(Chart.defaults.global.defaultFontSize, Chart.defaults.global.defaultFontStyle, Chart.defaults.global.defaultFontFamily);
                        ctx.textAlign = 'center';
                        ctx.textBaseline = 'bottom';
                        var gradient = ctx.createLinearGradient(0, 0, 50, 0);
                        gradient.addColorStop("0", "magenta");
                        gradient.addColorStop("0.5", "blue");
              

                        this.data.datasets.forEach(function (dataset, i) {
                            var meta = chartInstance.controller.getDatasetMeta(i);
                            meta.data.forEach(function (bar, index) {
                                var data = dataset.data[index];  
                                if( parseInt( bar._model.y) < 200){
                                    gradient.addColorStop("1.0", "white");
                                    ctx.fillStyle = gradient;
                                    ctx.fillText(data, bar._model.x, bar._model.y+25);
                                }else
                                {
                                    gradient.addColorStop("1.0", "black");
                                    ctx.fillStyle = gradient;
                                    ctx.fillText(data, bar._model.x, bar._model.y-5);
                                }
                            });
                        });
                    }
                },
                responsive: true,
                legend: {
                    position: 'bottom',
                },
                title: {
                    display: false,
                    text: '<%=MarketName %>',
                    fontSize: 18,
                    fontColor:'#325897'
                },
                scales: {
                    yAxes: [{
                        ticks: {
                            beginAtZero: true
                        }
                    }]
                },
                tooltips: {
                    // Disable the on-canvas tooltip
                    enabled: false,
                    custom: function (tooltipModel) {
                        // Tooltip Element
                        
                        var tooltipEl = document.getElementById('chartjs-tooltip');

                        // Create element on first render
                        if (!tooltipEl) {
                            tooltipEl = document.createElement('div');
                            tooltipEl.id = 'chartjs-tooltip';
                            tooltipEl.innerHTML = '<table></table>';
                            document.body.appendChild(tooltipEl);
                        }

                        // Hide if no tooltip
                        if (tooltipModel.opacity === 0) {
                            tooltipEl.style.opacity = 0;
                            return;
                        }

                        // Set caret Position
                        tooltipEl.classList.remove('above', 'below', 'no-transform');
                        if (tooltipModel.yAlign) {
                            tooltipEl.classList.add(tooltipModel.yAlign);
                        } else {
                            tooltipEl.classList.add('no-transform');
                        }

                        function getBody(bodyItem) {
                            return bodyItem.lines;
                        }

                        // Set Text
                        if (tooltipModel.body) {
                            var bodyLines = tooltipModel.body.map(getBody);

                            var innerHtml = '<thead>';
                            innerHtml += '</thead><tbody>';

                            bodyLines.forEach(function (body, i) {
                                var colors = tooltipModel.labelColors[i];
                                var style = 'background:' + colors.backgroundColor;
                                style += '; border-color:' + colors.borderColor;
                                style += '; border-width: 2px';
                                var span = '<span style="' + style + '"></span>';
                                innerHtml += '<tr><td>' + span + body + '</td></tr>';
                            });
                            innerHtml += '</tbody>';

                            var tableRoot = tooltipEl.querySelector('table');
                            tableRoot.innerHTML = innerHtml;
                        }

                        // `this` will be the overall tooltip
                        var position = this._chart.canvas.getBoundingClientRect();

                        // Display, position, and set styles for font
                        tooltipEl.style.opacity = 1;
                        tooltipEl.style.position = 'absolute';
                        tooltipEl.style.left = position.left + window.pageXOffset + tooltipModel.caretX + 'px';
                        tooltipEl.style.top = position.top + window.pageYOffset + tooltipModel.caretY + 'px';
                        tooltipEl.style.fontFamily = tooltipModel._bodyFontFamily;
                        tooltipEl.style.fontSize = tooltipModel.bodyFontSize + 'px';
                        tooltipEl.style.fontStyle = tooltipModel._bodyFontStyle;
                        tooltipEl.style.padding = tooltipModel.yPadding + 'px ' + tooltipModel.xPadding + 'px';
                        tooltipEl.style.pointerEvents = 'none';
                    }                
                },
                hover: {
                    animationDuration: 0
                },

            }
        });
    };




</script>
