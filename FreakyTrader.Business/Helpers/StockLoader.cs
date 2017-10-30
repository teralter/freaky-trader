using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using FreakyTrader.Data.Entities;
using Microsoft.Extensions.Logging;

namespace FreakyTrader.Business.Helpers
{
    public class StockLoader
    {
        private readonly ILogger<StockLoader> _logger;

        public StockLoader(ILogger<StockLoader> logger)
        {
            _logger = logger;
        }

        public List<Transaction> LoadTransactions(Stock stock, DateTime startDate, DateTime endDate)
        {
            var urlAddress = $@"http://export.finam.ru/NLMK_{startDate:yyMMdd}_{endDate:yyMMdd}.csv?" +
                $@"market={stock.MarketId}&em={stock.ExtId}&code={stock.Code}&apply=0&" +
                $@"df={startDate:dd}&mf={startDate.Month - 1}&yf={startDate:yyyy}&from={startDate:dd.MM.yyyy}&dt={endDate:dd}&mt={endDate.Month - 1}&yt={endDate:yyyy}&to={endDate:dd.MM.yyyy}&" +
                $@"p=1&f={stock.Code}_{startDate:yyMMdd}_{endDate:yyMMdd}&e=.csv&cn={stock.Code}&dtf=1&tmf=1&MSOR=1&mstime=on&mstimever=1&sep=1&sep2=1&datf=7";

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            request = (HttpWebRequest)WebRequest.Create(urlAddress);
            request.Timeout = 30000;  
            response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();

            List<Transaction> result = null;

            if (stream != null)
            {
                using (var streamReader = new StreamReader(stream))
                {
                    var lines = streamReader.ReadToEnd().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    result = new List<Transaction>();
                    foreach (var line in lines)
                    {
                        var data = line.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        var transaction = new Transaction()
                        {
                            StockId = stock.StockId,
                            RepDate = DateTime.ParseExact(data[1] + data[2], "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                            Price = double.Parse(data[3], CultureInfo.InvariantCulture),
                            Volume = int.Parse(data[4])
                        };
                        result.Add(transaction);
                    }
                }
            }

            return result;
        }
    }
}
