using System.Collections.Generic;
using System.Linq;
using FreakyTrader.Data.Context;
using FreakyTrader.Data.Entities;
using Microsoft.Extensions.Logging;

namespace FreakyTrader.Data.Repositories
{
    public class StockRepository
    {
        private readonly FreakyTraderContext _context;
        private readonly ILogger<StockRepository> _logger;

        public StockRepository(FreakyTraderContext context, ILogger<StockRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Stock> GetAllStocks()
        {
            _logger.LogTrace("GetAllStocks");
            return _context.Stocks.ToList();
        }
    }
}
