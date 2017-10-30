using System;
using FreakyTrader.Business.Helpers;
using FreakyTrader.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FreakyTrader.Console
{
    public class App
    {
        private readonly StockLoader _stockLoader;

        private readonly StockRepository _stockRepo;
        private readonly TransactionRepository _transactionRepo;
        private readonly ILogger<App> _logger;

        public App(StockLoader stockLoader, StockRepository stockRepo, TransactionRepository transactionRepo, ILogger<App> logger)
        {
            _stockLoader = stockLoader;
            _stockRepo = stockRepo;
            _transactionRepo = transactionRepo;
            _logger = logger;
        }

        

        public void Run()
        {
            _logger.LogInformation($"This is a console application");

            var startDate = new DateTime(2017, 10, 16);
            var endDate = new DateTime(2017, 10, 29);

            var stocks = _stockRepo.GetAllStocks();

            foreach (var stock in stocks)
            {
                var tDate = startDate;
                while (tDate <= endDate)
                {
                    var transactions = _stockLoader.LoadTransactions(stock, tDate, tDate);
                    _transactionRepo.SaveTransactions(transactions);
                    tDate = tDate.AddDays(1);
                }
            }

            _logger.LogInformation($"Done");

            System.Console.ReadKey();
        }
    }
}
