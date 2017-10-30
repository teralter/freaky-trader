'use strict';

app.factory('marketTickerService', ['$rootScope', 'Hub', '$timeout', function ($rootScope, Hub, $timeout) {

    //declaring the hub connection
    var hub = new Hub('marketTicker', {

        //client side methods
        listeners: {
            'marketStateChanged': function (state) {
                //console.log(state);
                //console.log(new Date());
                //$rootScope.$apply();
            },
            'updateStockRate': function (code, candle) {
                //console.log(code, candle);

                var camelCandel = _.transform(candle, function (result, value, key) {
                    result[_.camelCase(key)] = value;
                }, {});

                $rootScope.$broadcast('updateStockRate', camelCandel);
            }
        },

        //server side methods
        methods: ['openMarket', 'closeMarket', 'getMarketState', 'subscribe'],

        //query params sent on initial connection
        //queryParams: {
        //    'token': 'exampletoken'
        //},

        //handle connection error
        errorHandler: function (error) {
            console.error(error);
        },

        logging: true,

        //specify a non default root
        //rootPath: '/api
        rootPath: 'http://localhost:8084/signalr',

        stateChanged: function (state) {
            switch (state.newState) {
                case $.signalR.connectionState.connecting:
                    //your code here
                    break;
                case $.signalR.connectionState.connected:
                    //your code here
                    break;
                case $.signalR.connectionState.reconnecting:
                    //your code here
                    break;
                case $.signalR.connectionState.disconnected:
                    //your code here
                    break;
            }
        }
    });

    var openMarket = function (startDate, endDate, interval) {
        hub.openMarket(startDate, endDate, interval); 
    };

    var closeMarket = function () {
        hub.closeMarket();
    };

    var getMarketState = function () {
        return hub.getMarketState(); 
    }

    var subscribe = function (stockCodes) {
        return hub.subscribe(stockCodes);
    }

    return {
        openMarket: openMarket,
        closeMarket: closeMarket,
        getMarketState: getMarketState,
        subscribe: subscribe
    };
}]);