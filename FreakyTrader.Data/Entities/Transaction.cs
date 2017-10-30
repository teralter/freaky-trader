using System;

namespace FreakyTrader.Data.Entities
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int StockId { get; set; }
        public DateTime RepDate { get; set; }
        public double Price { get; set; }
        public int Volume { get; set; }

        public Stock Stock { get; set; }
    }
}
