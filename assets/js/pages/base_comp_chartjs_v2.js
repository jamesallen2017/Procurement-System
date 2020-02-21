/*
 *  Document   : base_comp_chartjs_v2.js
 *  Description: Custom JS code used in Chart.js v2 Page
 */

var BaseCompChartJSv2 = function () {
    // Chart.js v2 Charts, for more examples you can check out http://www.chartjs.org/docs
    var initChartsChartJSv2 = function () {
        // Set Global Chart.js configuration
        Chart.defaults.global.defaultFontColor = '#999';
        Chart.defaults.global.defaultFontFamily = 'Open Sans';
        Chart.defaults.global.defaultFontStyle = '600';
        Chart.defaults.scale.gridLines.color = "rgba(0,0,0,.05)";
        Chart.defaults.scale.gridLines.zeroLineColor = "rgba(0,0,0,.1)";
        Chart.defaults.global.elements.line.borderWidth = 2;
        Chart.defaults.global.elements.point.radius = 4;
        Chart.defaults.global.elements.point.hoverRadius = 6;
        Chart.defaults.global.tooltips.titleFontFamily = 'Source Sans Pro';
        Chart.defaults.global.tooltips.bodyFontFamily = 'Open Sans';
        Chart.defaults.global.tooltips.cornerRadius = 3;
        Chart.defaults.global.legend.labels.boxWidth = 15;

        // Get Chart Containers
        var $chart2BarsCon = jQuery('.js-chartjs2-bars');
        var $chart2RadarCon = jQuery('.js-chartjs2-Order');

        // Set Chart and Chart Data variables
        var $chart2Lines, $chart2Bars, $chart2Radar, $chart2Polar, $chart2Pie, $chart2Donut;
        var $chart2LinesBarsRadarData, $chart2PolarPieDonutData, $chart2ORderData;

        $.ajax({
            type: "POST",
            url: "/Home/ChartNumberofSales",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                var aData = data;
                var aLabels = aData[0];
                var aDatasets1 = aData[1];
                var aDatasets2 = aData[2];

                var $chart2LinesBarsRadarData = {

                    labels: aLabels,
                    datasets: [
                        {
                            label: 'This Year',
                            fill: true,
                            backgroundColor: 'rgba(0, 28, 58, 1)',
                            pointBackgroundColor: 'rgba(171, 227, 125, 1)',
                            pointBorderColor: '#fff',
                            pointHoverBackgroundColor: '#fff',
                            pointHoverBorderColor: 'rgba(171, 227, 125, 1)',
                            data: aDatasets1
                        },
                        {
                            label: 'Last Year',
                            fill: true,
                            backgroundColor: 'rgba(117, 120, 123, 1)',
                            pointBackgroundColor: 'rgba(171, 227, 125, 1)',
                            pointBorderColor: '#fff',
                            pointHoverBackgroundColor: '#fff',
                            pointHoverBorderColor: 'rgba(171, 227, 125, 1)',
                            data: aDatasets2

                        },
                    ]
                };

                $chart2Bars = new Chart($chart2BarsCon, { type: 'bar', data: $chart2LinesBarsRadarData });


            }
        })


        $.ajax({
            type: "POST",
            url: "/Home/ChartNumberofOrder",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                var aData = data;
                var aLabels = aData[0];
                var aDatasets1 = aData[1];
                var aDatasets2 = aData[2];

                var $chart2ORderData = {

                    labels: aLabels,
                    datasets: [
                        {
                            label: 'This Week',
                            fill: true,
                            backgroundColor: 'rgba(0, 28, 58, 1)',
                            pointBackgroundColor: 'rgba(171, 227, 125, 1)',
                            pointBorderColor: '#fff',
                            pointHoverBackgroundColor: '#fff',
                            pointHoverBorderColor: 'rgba(171, 227, 125, 1)',
                            data: aDatasets1

                        },
                        {
                            label: 'Last Week',
                            fill: true,
                            backgroundColor: 'rgba(117, 120, 123, 1)',
                            pointBackgroundColor: 'rgba(171, 227, 125, 1)',
                            pointBorderColor: '#fff',
                            pointHoverBackgroundColor: '#fff',
                            pointHoverBorderColor: 'rgba(171, 227, 125, 1)',
                            data: aDatasets2

                        },
                    ]
                };

                $chart2Bars = new Chart($chart2RadarCon, { type: 'bar', data: $chart2ORderData });


            }
        })
        // Lines/Bar/Radar Chart Data


        // Init Charts
    };

    return {
        init: function () {
            // Init charts
            initChartsChartJSv2();
        }
    };
}();

// Initialize when page loads
jQuery(function () { BaseCompChartJSv2.init(); });