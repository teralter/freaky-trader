'use strict';

app.factory('transactionService', ['$http',
        function ($http) {
            return {
                'getTransactions': function(code, startDate, endDate, period) {
                    return $http.get('/api/transactions?code=' + code + '&startDate=' + startDate + '&endDate=' + endDate + '&period=' + period);
                }
            }
        }]);