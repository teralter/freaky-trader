'use strict';

app.controller('transactionCtrl', ['$scope', '$rootScope', '$timeout', '$filter', 'moment', 'transactionService', 'marketTickerService',
    function ($scope, $rootScope, $timeout, $filter, moment, transactionService, marketTickerService) {

        $scope.chartConfig = {
            chart: {
                height: 600,
                width: 1200,
            },
            navigator: {
                enabled: false
            },
            scrollbar: {
                enabled: false
            },
            rangeSelector: {
                enabled: false
            },
            credits: {
                enabled: false
            },
            plotOptions: {
                candlestick: {
                    dataGrouping: {
                        enabled: false,
                        forced: true,
                        units: [[
                            'minute',
                            [1]
                        ]]
                    }
                },
                series: {
                    stacking: ''
                }
            },
            series: [
                {
                    type: 'candlestick',
                    data: []
                }],
            chartType: 'stock'
        }

        $scope.chartConfig.series[0].data = [];

        $scope.stockCode = 'GAZP';
        $scope.period = 60;
        var initDate;
        var tmpCandles = [];
        var prevIndex = 0;

        function init() {
            $timeout(function () {
		//
                //marketTickerService.closeMarket();
                //marketTickerService.openMarket('2017-10-18T10:00:00', '2017-10-27T19:05:00.000', 200);

                marketTickerService.subscribe([$scope.stockCode]);
            }, 1000);
        }

        $rootScope.$on('updateStockRate', function (event, candle) {
            if (!initDate) {
                initDate = moment(candle.repDate);
                var startDate = initDate.clone().add(-1, 'hours');
                var endDate = initDate.clone().add(-1, 'seconds');
                getPrevTransactions($scope.stockCode, startDate.format('YYYY-MM-DDTHH:mm:ss'), endDate.format('YYYY-MM-DDTHH:mm:ss'), $scope.period);
            }
            if ($scope.prevLoaded) {
                proceedCandle(candle);
            } else {
                tmpCandles.push(candle);
            }

            $scope.$apply();
        });


        function proceedCandle(candle) {
            var data = $scope.chartConfig.series[0].data;
            var date = moment(candle.repDate + 'Z').set({ second: 0 }).valueOf();
            if (Math.floor((date / 1000) / $scope.period) > prevIndex) {
                //console.log('push ' + prevIndex);
                data.push([
                    date,
                    candle.open,
                    candle.high,
                    candle.low,
                    candle.close
                ]);
            } else {
                //console.log('add ' + prevIndex);
                var lastData = data[data.length - 1];
                lastData[2] = Math.max(lastData[2], candle.high);
                lastData[3] = Math.min(lastData[3], candle.low);
                lastData[4] = candle.close;
            }
            prevIndex = Math.floor((date / 1000) / $scope.period);
        }

        function getPrevTransactions(code, startDate, endDate, period) {
            transactionService.getTransactions(code, startDate, endDate, period).then(
                function (result) {
                    prevIndex = Math.floor((moment(endDate + 'Z').valueOf() / 1000) / $scope.period);

                    $scope.chartConfig.series[0].data = result.data.transactions.map(function (d) {
                        return [
                            moment(d.repDate + 'Z').valueOf(),
                            d.open,
                            d.high,
                            d.low,
                            d.close
                        ];
                    });

                    tmpCandles.forEach(function (c) {
                        proceedCandle(c);
                    });

                    $scope.prevLoaded = true;
                },
                function (error) {
                }
            );
        }

        init();
    }]);