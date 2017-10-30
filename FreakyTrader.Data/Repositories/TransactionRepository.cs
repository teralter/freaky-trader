using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using FreakyTrader.Data.Context;
using FreakyTrader.Data.DTO;
using FreakyTrader.Data.Entities;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FreakyTrader.Data.Repositories
{
    public class TransactionRepository
    {
        private readonly FreakyTraderContext _context;
        private readonly ILogger<TransactionRepository> _logger;
        private readonly IConfigurationRoot _configuration;

        public TransactionRepository(FreakyTraderContext context, ILogger<TransactionRepository> logger, IConfigurationRoot configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public void SaveTransactions(List<Transaction> transactions)
        {
            if (transactions.Count == 0)
            {
                return;
            }
            var startDate = transactions.Select(t => t.RepDate).Min();
            var endDate = transactions.Select(t => t.RepDate).Max();
            var stockId = transactions.First().StockId;
            var toDelete = _context.Transactions.Where(t => t.RepDate >= startDate && t.RepDate <= endDate && t.StockId == stockId).ToList();
            _context.Transactions.RemoveRange(toDelete);
            _context.Transactions.AddRange(transactions);
            _context.SaveChanges();

            _logger.LogInformation($"Transactions between {startDate} and {endDate} saved");
        }

        public List<CandleDTO> GetCandles(string code, DateTime startDate, DateTime endDate, int interval)
        {
            var stockId = _context.Stocks.FirstOrDefault(x => x.Code == code)?.StockId;
            using (var conection = new SqlConnection(_configuration.GetConnectionString("FreakyTraderConnection")))
            {
                conection.Open();

                var query = @"
                            declare @sDate datetime  = dateadd(second, -( datediff(second, '2017-01-01', @startDate) % @interval ), @startDate)
                            declare @eDate datetime  = dateadd(second, @interval - ( datediff(second, '2017-01-01', @endDate) % @interval ), @endDate) 

                            select  * ,
                                    datediff(second, @sDate, t.RepDate) / @interval as IntervalId ,
                                    row_number() over ( partition by datediff(second, @sDate, t.RepDate) / @interval order by t.RepDate desc, t.Price ) as CloseNumber ,
                                    row_number() over ( partition by datediff(second, @sDate, t.RepDate) / @interval order by t.RepDate, t.Price ) as OpenNumber
                            into    #tt_data
                            from    dbo.Transactions t
                            where   t.StockId = @stockId
                                    and RepDate between @sDate and @eDate
        
             
             
                            select  dateadd(second,  td.IntervalId * @interval, @sDate) as RepDate ,
                                    min(op.Price) as [Open] ,
                                    max(td.Price) as High ,
                                    min(td.Price) as Low ,
                                    min(cl.Price) as [Close] ,
                                    sum(td.Volume) as Volume
                            into    #tt_result
                            from    #tt_data td
                                    join #tt_data op on td.IntervalId = op.IntervalId
                                                        and op.OpenNumber = 1
                                    join #tt_data cl on td.IntervalId = cl.IntervalId
                                                        and cl.CloseNumber = 1
                            group by td.IntervalId ;

                            select * from #tt_result tr
                            order by tr.RepDate
    
                            drop table #tt_data
                            drop table #tt_result";

                var candles = conection.Query<CandleDTO>(query, new { stockId, startDate, endDate, interval }).ToList();

                conection.Close();

                return candles;
            }
        }
    }
}
